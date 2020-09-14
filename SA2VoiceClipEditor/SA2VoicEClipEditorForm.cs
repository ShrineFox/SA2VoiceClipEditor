using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PuyoTools.Modules.Archive;
using PuyoTools.Formats.Archives;
using System.Threading;
using System.Diagnostics;

namespace SA2VoiceClipEditor
{
    public partial class SA2VoiceClipEditorForm : Form
    {
        string afsPath = "";
        string csbPath = "";
        string extractedFilesPath = "";

        public SA2VoiceClipEditorForm()
        {
            InitializeComponent();
        }

        private void ExtractAFS_DragDrop(object sender, DragEventArgs e)
        {
            var data = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            var settings = new PuyoTools.GUI.ArchiveExtractor.Settings();
            if (File.Exists(data[0]) && Path.GetExtension(data[0]).ToLower() == ".afs")
            {
                lbl_Status.Text = "Extracting AFS...";
                Thread.Sleep(100);
                //Extract AFS
                afsPath = data[0];
                extractedFilesPath = Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(afsPath), "Extracted Files")), Path.GetFileNameWithoutExtension(afsPath));
                List<string> files = new List<string>();
                files.Add(afsPath);
                PuyoTools.GUI.ArchiveExtractor.Run(settings, files);

                //rename extracted files to include .ahx extension
                string[] lines = File.ReadAllLines("event_adx.txt");
                foreach (var file in Directory.GetFiles(extractedFilesPath, "*", SearchOption.AllDirectories))
                {
                    string newPath = file;
                    if (Path.GetFileName(afsPath).StartsWith("event_adx") && Path.GetFileName(newPath) != "AFS")
                    {
                        //Change path if numbered event_adx is being extracted
                        int lineNumber = Convert.ToInt32(Path.GetFileNameWithoutExtension(file));
                        string characterName = lines[lineNumber].Split('\t')[0];
                        newPath = file.Replace(Path.GetFileNameWithoutExtension(afsPath) + "\\", "").Replace("Extracted Files", $"Extracted Files\\{Path.GetFileNameWithoutExtension(afsPath)}_{characterName}");
                        if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    }
                    //Include .ahx in new path name and move
                    using (WaitForFile(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                    if (Path.GetExtension(file) != ".ahx" && !newPath.Contains("AFS"))
                        newPath = newPath + ".ahx";
                    else if (newPath.Contains("AFS") && Path.GetFileName(afsPath).StartsWith("event_adx"))
                        newPath = newPath.Replace(Path.GetFileNameWithoutExtension(afsPath) + "\\", "").Replace("Extracted Files", $"Extracted Files\\{Path.GetFileNameWithoutExtension(afsPath)}_Pilot").Replace("AFS","0000.ahx");
                    else
                        newPath = newPath.Replace("AFS", "0000.ahx");

                    File.Move(file, newPath);
                    using (WaitForFile(newPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                }
                //Delete empty event_adx extracted AFS folder
                lbl_Status.Text = "Done Extracting AFS";
                if (Path.GetFileName(afsPath).StartsWith("event_adx"))
                    Directory.Delete(extractedFilesPath, true);

                //Convert all AHX files to WAV
                if (chkBox_Convert.Checked)
                {
                    lbl_Status.Text = "Converting AHX to WAV...";
                    foreach (var ahx in Directory.GetFiles(Path.GetDirectoryName(extractedFilesPath), "*.ahx", SearchOption.AllDirectories))
                    {
                        RunCMD($"radx_decode.exe \"{ahx}\" \"{ahx.Replace(".ahx", ".wav")}\"");
                        using (WaitForFile(ahx.Replace(".ahx", ".wav"), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                        foreach (var process in Process.GetProcessesByName("cmd"))
                            process.Kill();
                        File.Delete(ahx);
                    }
                    lbl_Status.Text = "Done Converting AHX";
                }
            }
        }

        private void Rename_Files_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("cmd"))
                process.Kill();
        }

        private void RepackAFS_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void ExtractCSB_DragDrop(object sender, DragEventArgs e)
        {
            var data = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (File.Exists(data[0]) && Path.GetExtension(data[0]).ToLower() == ".csb")
            {
                csbPath = data[0];
                extractedFilesPath = Path.Combine(Path.GetDirectoryName(csbPath), Path.GetFileNameWithoutExtension(csbPath));
                RunCMD($"CsbEditor.exe \"{csbPath}\"");
                Thread.Sleep(1000);
                foreach (var file in Directory.GetFiles(extractedFilesPath, "*.adx", SearchOption.AllDirectories))
                {
                    if (new FileInfo(file).Length > 60)
                        File.Move(file, Path.Combine(extractedFilesPath, Path.GetFileName(Path.GetDirectoryName(file)) + ".adx"));
                }
                Thread.Sleep(1000);
                RecursiveDelete(new DirectoryInfo(extractedFilesPath));
                lbl_Status.Text = "Done extracting CSB";
            }
        }

        private void RunCMD(string args, bool hidden = true)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "cmd";
            start.Arguments = $"/K {args}";
            start.UseShellExecute = true;
            start.RedirectStandardOutput = false;
            if (hidden)
                start.WindowStyle = ProcessWindowStyle.Hidden;
            using (Process process = Process.Start(start))
            {

            }
        }

        private void RepackCSB_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void ExtractAFS_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void RepackAFS_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void ExtractCSB_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void RepackCSB_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
                dir.Delete(true);
            }
            //baseDir.Delete(true);
        }

        FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(100);
                }
            }

            return null;
        }
    }
}
