using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CSharp;

namespace AnjLab.FX.MSBuild.Tasks
{

    public class ProductInfo: Task
    {
        readonly string[] _imports = new []
                                         {
                                             "System", 
                                             "System.Reflection", 
                                             "System.Runtime.CompilerServices", 
                                             "System.Runtime.InteropServices", 
                                             "AnjLab.FX.Sys"
                                         };

        private string _infoFile;
        private string _outputFile;

        [Required]
        public string InfoFile
        {
            get { return _infoFile; }
            set { _infoFile = value; }
        }

        [Output]
        public string OutputFile
        {
            get { return _outputFile; }
            set { _outputFile = value; }
        }

        private int _majorVersion;
        private int _minorVersion;
        private DateTime _startDate;
        private int _revision;
        private int _buildNumber;
        private string _company;
        private string _product;

        [Output]
        public string Version
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}", _majorVersion, _minorVersion,
                                     _buildNumber, _revision);
            }
        }

        public override bool Execute()
        {
            if (!ReadVersionFile())
                return true;

            _buildNumber = GenerateBuildNumber();
            if (!GetRevistionFromEnvironment() && !GetRevisionFromSvn())
            {
                _revision = 0;
            }

            GenerateFile();
            
            return true;
        }

        private bool GetRevistionFromEnvironment() {
            string version = Environment.GetEnvironmentVariable("BUILD_VCS_NUMBER.1");
            if (string.IsNullOrEmpty(version))
            {
                return false;
            }
            return int.TryParse(version, out _revision);
        }

        private void GenerateFile()
        {
            using (var writer = new StreamWriter(_outputFile))
            {
                var provider = new CSharpCodeProvider();
                CodeCompileUnit ccu = BuildCompilationUnit();
                provider.GenerateCodeFromCompileUnit(ccu, writer, new CodeGeneratorOptions());
            }
        }

        private CodeCompileUnit BuildCompilationUnit()
        {
            var ccu = new CodeCompileUnit();
            var cn = new CodeNamespace();
            foreach (string import in _imports)
            {
                cn.Imports.Add(new CodeNamespaceImport(import));
            }
            ccu.Namespaces.Add(cn);
            string version = Version;
            var cad = new CodeAttributeDeclaration("AssemblyVersion",
                                                   new CodeAttributeArgument(
                                                       new CodePrimitiveExpression(version)));
            ccu.AssemblyCustomAttributes.Add(cad);

            cad = new CodeAttributeDeclaration("AssemblyFileVersion",
                                               new CodeAttributeArgument(
                                                   new CodePrimitiveExpression(version)));
            ccu.AssemblyCustomAttributes.Add(cad);


            cad = new CodeAttributeDeclaration("AssemblyBuildDate",
                                               new CodeAttributeArgument(
                                                   new CodePrimitiveExpression(
                                                       DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
            ccu.AssemblyCustomAttributes.Add(cad);

            if (!string.IsNullOrEmpty(_company))
            {
                cad = new CodeAttributeDeclaration("AssemblyCompany",
                                                   new CodeAttributeArgument(
                                                       new CodePrimitiveExpression(_company)));
                ccu.AssemblyCustomAttributes.Add(cad);
            }
            if (!string.IsNullOrEmpty(_product))
            {
                cad = new CodeAttributeDeclaration("AssemblyProduct",
                                                   new CodeAttributeArgument(
                                                       new CodePrimitiveExpression(_product)));
                ccu.AssemblyCustomAttributes.Add(cad);
            }
            return ccu;
        }

        private bool GetRevisionFromSvn()
        {
            try
            {
                var psi = new ProcessStartInfo("svn")
                              {
                                  Arguments = "info --xml",
                                  WorkingDirectory = Path.GetDirectoryName(_infoFile),
                                  UseShellExecute = false,
                                  RedirectStandardOutput = true,
                                  RedirectStandardError = true,
                                  CreateNoWindow = true
                              };
                var p = new Process {StartInfo = psi};
                p.Start();
                if (!p.WaitForExit(10*1000))
                {
                    return false;
                }
                string result = p.StandardOutput.ReadToEnd();

                var doc = new XmlDocument();
                doc.LoadXml(result);
                _revision = int.Parse(doc.DocumentElement["entry"].Attributes["revision"].Value, CultureInfo.InvariantCulture);
                return true;
            } 
            catch(Exception e)
            {
                Log.LogMessage("Error during svn info. {0}", e);
            }
            return true;
        }

        private int GenerateBuildNumber()
        {
            if (DateTime.Now.Date <= _startDate.Date)
            {
                return 0;
            }

            return (int)(DateTime.Now.Date - _startDate.Date).TotalDays;
        }

        public bool ReadVersionFile()
        {
            if (!File.Exists(_infoFile))
            {
                Log.LogMessage("Can't find product info file. Exit.");
                return false;
            }

            try
            {
                var doc = new XmlDocument();
                doc.Load(_infoFile);
                ReadTag(doc.DocumentElement, "major-version", out _majorVersion);
                ReadTag(doc.DocumentElement, "minor-version", out _minorVersion);
                ReadTag(doc.DocumentElement, "start-date", out _startDate);
                ReadTag(doc.DocumentElement, "company", out _company);
                ReadTag(doc.DocumentElement, "product", out _product);
                return true;
            } 
            catch( Exception e)
            {
                Log.LogMessage("Error during parsing version file. Exit. {0}", e);
            }
            return false;
        }

        private static void ReadTag<T>(XmlNode node, string name, out T value)
        {
            if (node[name] == null)
            {
                throw new InvalidOperationException(string.Format("Can't find tag with name {0}", name));
            }

            value =(T)Convert.ChangeType(node[name].InnerText, typeof (T), CultureInfo.InvariantCulture);
        }
    }
}
