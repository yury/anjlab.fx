using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AnjLab.FX.Sys;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace AnjLab.FX.MSBuild.Tasks
{
    public class ArchiveFolder: Task
    {
        private string _pathToArchive;
        private string _archiveName;
        private string _exclude;

        [Required]
        public string PathToArchive
        {
            get { return _pathToArchive; }
            set { _pathToArchive = value; }
        }

        [Required]
        public string ArchiveName
        {
            get { return _archiveName; }
            set { _archiveName = value; }
        }

        public string Exclude
        {
            get { return _exclude; }
            set { _exclude = value; }
        }

        private string Get7ZExclude()
        {
            if (string.IsNullOrEmpty(_exclude))
                return string.Empty;
            string[] filters = _exclude.Split(';');
            return " -x!" + Lst.ToString(filters, " -x!");
        }

        public override bool Execute()
        {
            if (File.Exists(_archiveName))
            {
                try
                {
                    File.Delete(_archiveName);    
                } 
                catch
                {
                    Log.LogWarning("can't delete old archive");
                }
            }
            return Archive();
        }

        public static string _7Zip
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string s7exe = Path.Combine(path, @"7-zip\7z.exe");
                return !File.Exists(s7exe) ? s7exe.Replace("Program Files (x86)", "Program Files") : s7exe;
            }
        }

        private bool Archive()
        {
            string sevenZ = _7Zip;
            if (!File.Exists(sevenZ))
            {
                Log.LogError("can't find 7zip");
                return false;
            }
            ProcessStartInfo psi = new ProcessStartInfo(sevenZ);
            psi.Arguments = string.Format("a -r -mx9 {0} *.* {1}", _archiveName, Get7ZExclude());
            psi.WorkingDirectory = PathToArchive;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            if (!p.WaitForExit(10 * 10 * 1000))
            {
                Log.LogError("Timouted");
                Log.LogError(p.StandardError.ReadToEnd());
                return false;
            }
            return true;
        }
    }
}
