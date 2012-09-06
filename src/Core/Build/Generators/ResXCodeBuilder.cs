﻿// ResXCodeBuilder.cs
// Script#/Core/Build
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScriptSharp.Generators {

    public sealed class ResXCodeBuilder {

        private static readonly string ResourcesHeader =
@"//------------------------------------------------------------------------------
// <auto-generated>
// Resources.g.cs
// Do not edit directly. This file has been auto-generated from .resx resources.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace {0} {{
";

        private static readonly string ResourcesFooter =
@"}";

        private StringBuilder _codeBuilder;

        public void Start(string namespaceName) {
            _codeBuilder = new StringBuilder();
            _codeBuilder.AppendFormat(ResourcesHeader, namespaceName);
        }

        public string End() {
            _codeBuilder.AppendLine(ResourcesFooter);
            return _codeBuilder.ToString();
        }

        public void GenerateCode(string resourceFileName, string resourceFileContent) {
            if (IsLocalizedResourceFile(resourceFileName)) {
                return;
            }

            List<ResXItem> resourceItems = ResXParser.ParseResxMarkup(resourceFileContent);
            if (resourceItems.Count == 0) {
                return;
            }

            string className = Path.GetFileNameWithoutExtension(resourceFileName);

            _codeBuilder.AppendLine();
            _codeBuilder.AppendFormat("    /// <summary>{0} resources class</summary>", className);
            _codeBuilder.AppendLine();
            _codeBuilder.AppendLine("    [Resources]");
            _codeBuilder.AppendFormat("    [GeneratedCodeAttribute(\"{0}\", \"{1}\")]",
                                     this.GetType().Name,
                                     typeof(ResXCodeBuilder).Assembly.GetName().Version.ToString());
            _codeBuilder.AppendLine();
            _codeBuilder.AppendFormat("    internal static class {0} {{", className);
            _codeBuilder.AppendLine();

            foreach (ResXItem resourceItem in resourceItems) {
                _codeBuilder.AppendLine();
                if (resourceItem.Comment.Length != 0) {
                    _codeBuilder.AppendFormat("        /// <summary>{0}</summary>", resourceItem.Comment);
                    _codeBuilder.AppendLine();
                }
                _codeBuilder.AppendFormat("        public static readonly string {0} = null;", resourceItem.Name);
                _codeBuilder.AppendLine();
            }

            _codeBuilder.AppendLine("    }");
        }

        private static bool IsLocalizedResourceFile(string resourceFileName) {
            string locale = ResourceFile.GetLocale(resourceFileName);
            return (String.IsNullOrEmpty(locale) == false);
        }
    }
}
