using PangyaAPI.UpdateList;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace PangyaUpdate
{
    public class Patcher
    {
        public string Notice { get; set; }
        private string link_file_download { get; set; }
        string link_file_updatelist { get; set; }
        public event ProgressEventHandler OnProgressEvent;
        public delegate void ProgressEventHandler(ProgressBar barProcess = null, ProgressBar barUpdate = null, Label lbProcessDesc = null, Label lblPatchVer = null, Label lbFile = null);
        public Document UpdateListInfo { get; set; }
        public uint patch_num { get; set; }
        public string patch_version { get; set; }
        ProgressBar barProcess { get; set; }
        ProgressBar barUpdate { get; set; }
        Label lbProcessDesc { get; set; }
        Label lblPatchVer { get; set; }
        Label lbFile { get; set; }
        public Button BtnAbrirProjectG { get; set; }
        public Button BtnResetPatch { get; set; }
        public Patcher()
        {

            Notice = "http://gameraze.com.br/Notes/Patcher";
            link_file_download = "http://pangya21.xyz/new/Service/S4_Patch";
            link_file_updatelist = link_file_download + link_file_updatelist;
        }
       

        public void RePatch()
        {
            DownloadUpdateList();
        }

        public void DownloadUpdateList()
        {
            if (Process.GetProcessesByName("ProjectG.exe").Length > 0)
            {
                MessageBox.Show("Please close the game!");
                Environment.Exit(0);
            }

            Task.Run(() =>
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                        byte[] data = webClient.DownloadData(link_file_updatelist);

                        var TempFile = Path.GetTempPath();
                        File.WriteAllBytes(TempFile + @"\updatelist", data);

                        XMLParser updatelist2 = new XMLParser(TempFile, "US");
                        updatelist2.DecryptFile();
                        UpdateListInfo = updatelist2.getFiles();
                        File.Delete(TempFile + @"\updatelist");
                        patch_num = (uint)UpdateListInfo.PatchNumber.Value;
                        patch_version = UpdateListInfo.PatchVersion.Value;

                        OnProgressEvent?.Invoke(barProcess, barUpdate, lbProcessDesc, lblPatchVer, lbFile);
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Response.ToString());
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message);
                }
            });
        }

        public void DownloadFile(string path, string dir, string fileName, long size, out long sumProgress)
        {
            sumProgress = 0;
            OnProgressEvent?.Invoke(barProcess, barUpdate, lbProcessDesc, lblPatchVer, lbFile);
            using (Stream stream = WebRequest.Create($"{link_file_download}/{fileName}.zip").GetResponse().GetResponseStream())
            {
                path = Path.Combine(Path.GetTempPath(), "temp.zip");

                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), dir)))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), dir));
                }

                using (Stream fileStream = File.Create(path))
                {
                    byte[] buffer = new byte[size];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        sumProgress += bytesRead;

                        OnProgressEvent?.Invoke(barProcess, barUpdate, lbProcessDesc, lblPatchVer, lbFile);
                    }
                }

                var result = ExistFile(fileName, dir);
                if (result)
                {
                    Verificacao(fileName, dir);
                }
            }
        }

        void Verificacao(string fileName, string dir)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), dir);

            using (var zip = PangyaAPI.ZIP.ZipFile.Read(Path.Combine(Directory.GetCurrentDirectory(), "temp.zip")))
            {
                var size1 = zip.Entries.First().UncompressedSize;
                var size2 = new FileInfo(Path.Combine(basePath, fileName)).Length;

                if (size1 != size2)
                {
                    if (File.Exists(Path.Combine(basePath, fileName)))
                    {
                        File.Delete(Path.Combine(basePath, fileName));
                    }

                    zip.ExtractSelectedEntries(fileName, "", basePath);
                    OnProgressEvent?.Invoke(barProcess, barUpdate, lbProcessDesc, lblPatchVer, lbFile);
                    Debug.WriteLine("[Verificacao]: verificado e atualizando o arquivo....");
                }
                else
                {
                    Debug.WriteLine("[Verificacao]: não atualizou o arquivo....");
                }
            }
        }

        bool ExistFile(string fileName, string dir)
        {
            var basePath = Directory.GetCurrentDirectory();
            var path = Path.GetTempPath() + "\\temp.zip";

            using (var zip = PangyaAPI.ZIP.ZipFile.Read(path))
            {
                var size1 = zip.Entries.First().UncompressedSize;
                var size2 = File.OpenRead(Path.Combine(basePath, dir, fileName)).Length;

                File.Delete(basePath + "\\temp.zip");

                if (size1 != size2)
                {
                    File.Move(path, basePath + "\\temp.zip");
                    System.Diagnostics.Debug.WriteLine("[ExistFile]: checou o arquivo e está movendo....");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[ExistFile]: arquivo foi checado com sucesso 1....");
                    return false;
                }
            }
        }

        public void SetValues(ProgressBar _barProcess, ProgressBar _barUpdate, Label _lbProcessDesc, Label _lblPatchVer, Label _lbFile)
        {
            barProcess = _barProcess;
            barUpdate = _barUpdate;
            lbProcessDesc = _lbProcessDesc;
            lblPatchVer = _lblPatchVer;
            lbFile = _lbFile;
        }
        public void SetValues(ProgressBar _barProcess, ProgressBar _barUpdate, Label _lbProcessDesc, Label _lblPatchVer, Label _lbFile, Button Start, Button Reset)
        {
            barProcess = _barProcess;
            barUpdate = _barUpdate;
            lbProcessDesc = _lbProcessDesc;
            lblPatchVer = _lblPatchVer;
            lbFile = _lbFile;
            BtnAbrirProjectG = Start;
            BtnResetPatch = Reset;
        }
    }
}
