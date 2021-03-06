﻿using System;
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
using PuyoTools.GUI;
using NAudio.Wave;

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

        private void RepackAFS_DragDrop(object sender, DragEventArgs e)
        {
            var data = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            afsPath = data[0];
            if (Directory.Exists(afsPath))
            {
                //Move wav/ahx files to temp dir
                string tempPath = Path.Combine(Path.GetDirectoryName(afsPath), Path.GetFileName(afsPath) + "_new");
                Directory.CreateDirectory(tempPath);
                foreach (var file in Directory.GetFiles(afsPath, "*", SearchOption.AllDirectories))
                    File.Copy(file, Path.Combine(tempPath, Path.GetFileName(file)));
                //For each file in temp dir...
                foreach (var file in Directory.GetFiles(tempPath))
                {
                    //Convert WAV files to extensionless AHX
                    if (Path.GetExtension(file) == ".wav")
                    {
                        //Convert Stereo to Mono
                        StereoToMono(file, Path.Combine(Path.GetDirectoryName(file), "mono.wav"));
                        File.Delete(file);
                        RunCMD($"radx_encode.exe -a \"{Path.Combine(Path.GetDirectoryName(file), "mono.wav")}\" \"{file.Replace(".wav", "")}\"");
                        using (WaitForFile(file.Replace(".ahx", ""), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                        foreach (var process in Process.GetProcessesByName("cmd"))
                            process.Kill();
                        File.Delete(file);
                    }
                    else if (Path.GetExtension(file) == ".ahx")
                    {
                        File.Move(file, file.Replace(".ahx", ""));
                        using (WaitForFile(file.Replace(".ahx", ""), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                    }
                    else if (Path.GetExtension(file).Contains("."))
                        File.Delete(file);
                }
                //Repack AFS
                List<string> files = Directory.GetFiles(tempPath).ToList();
                PuyoTools.GUI.ArchiveCreator.Run(files);
            }
            
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
                        File.Move(file, Path.Combine(extractedFilesPath, Path.GetFileName(Path.GetDirectoryName(file)).Replace("_wav","") + ".adx"));
                    else
                        File.Move(file, Path.Combine(extractedFilesPath, Path.GetFileName(Path.GetDirectoryName(file)).Replace("_wav", "") + "_silent.adx"));
                }
                Thread.Sleep(1000);
                RecursiveDelete(new DirectoryInfo(extractedFilesPath));
                lbl_Status.Text = "Done extracting CSB";
                if (chkBox_Convert.Checked)
                {
                    lbl_Status.Text = "Converting ADX to WAV...";
                    foreach (var adx in Directory.GetFiles(extractedFilesPath, "*.adx"))
                    {
                        RunCMD($"radx_decode.exe \"{adx}\" \"{adx.Replace(".adx", ".wav")}\"");
                        using (WaitForFile(adx.Replace(".adx", ".wav"), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                        foreach (var process in Process.GetProcessesByName("cmd"))
                            process.Kill();
                        File.Delete(adx);
                    }
                    lbl_Status.Text = "Done Converting ADX";
                }
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
            var data = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (Directory.Exists(data[0]))
            {
                //Recreate folder structure
                extractedFilesPath = data[0];
                List<string> files = Directory.GetFiles(extractedFilesPath, "*", SearchOption.AllDirectories).ToList();
                string newPath = Path.Combine(Path.Combine(extractedFilesPath, "Synth"), Path.GetFileNameWithoutExtension(files[0]).Substring(0, Path.GetFileNameWithoutExtension(files[0]).Length - 6));
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                Directory.CreateDirectory(newPath);
                foreach (var file in Directory.GetFiles(extractedFilesPath, "*", SearchOption.AllDirectories))
                {
                    //Create new folder in Synth folder named after each wav
                    string newFolder = Path.Combine(newPath, Path.GetFileNameWithoutExtension(file)).Replace("_silent", "") + "_wav";
                    Directory.CreateDirectory(newFolder);
                    if (Path.GetExtension(file) == ".wav")
                    {
                        //Convert WAV to ADX
                        RunCMD($"radx_encode.exe -n \"{file}\" \"{Path.Combine(newFolder, "Intro.adx")}\"");
                        using (WaitForFile(Path.Combine(newFolder, "intro.adx"), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                        foreach (var process in Process.GetProcessesByName("cmd"))
                            process.Kill();
                        File.Delete(file);
                    }
                    else if (Path.GetExtension(file) == ".adx")
                    {
                        File.Move(file, Path.Combine(newFolder, "intro.adx"));
                    }
                }
                RunCMD($"CsbEditor.exe \"{extractedFilesPath}\"");

                Thread.Sleep(1000);
                lbl_Status.Text = "Done Repacking CSB";
            }
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

        public static void StereoToMono(string sourceFile, string outputFile)
        {
            using (var waveFileReader = new WaveFileReader(sourceFile))
            {
                var outFormat = new WaveFormat(waveFileReader.WaveFormat.SampleRate, 1);
                using (var resampler = new MediaFoundationResampler(waveFileReader, outFormat))
                {
                    WaveFileWriter.CreateWaveFile(outputFile, resampler);
                }
            }
        }


    }
}
