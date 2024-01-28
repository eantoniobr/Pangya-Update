using PangyaAPI.UpdateList;
using PangyaUpdate.Tools;
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
        public delegate void ProgressEventHandler(ProgressBar barProcess = null, ProgressBar barUpdate = null, Label lbProcessDesc = null, Label lblPatchVer = null);
        public Document UpdateListInfo { get; set; }
        public uint patch_num { get; set; }
        public string patch_version { get; set; }
        ProgressBar barProcess { get; set; }
        ProgressBar barUpdate { get; set; }
        Label lbProcessDesc { get; set; }
        Label lblPatchVer { get; set; }
        public Button BtnAbrirProjectG { get; set; }
        public Button BtnResetPatch { get; set; }
        public Patcher()
        {
            UpdateListInfo = new Document();
            Notice = "http://144.217.169.230/game";
            link_file_download = "http://149.56.186.102/new/Service/S4_Patch";
            link_file_updatelist = link_file_download + @"\updatelist";
        }

        public void SetPatchNum()
        {
            File.WriteAllText(Path.GetTempPath() + "\\patch.dat", patch_num.ToString());
        }

        public void GetPatchNum()
        {
            string filePath = Path.GetTempPath() + "\\patch.dat";
            if (File.Exists(filePath))
            {
                string patchNumString = File.ReadAllText(filePath);
                if (uint.TryParse(patchNumString, out uint number))
                {
                    patch_num = number;
                    return;
                }
            }

            patch_num = 823;
            File.WriteAllText(filePath, "823");
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
                        OnProgressEvent?.Invoke(barProcess, barUpdate, lbProcessDesc, lblPatchVer);
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show("WebException: " + ex.Response.ToString(), "Pangya Laucher");
                }
                catch (Exception ex2)
                {
                    MessageBox.Show("ExceptionLog: " + ex2.ToString(), "Pangya Laucher");
                }
            });
        }

        public void DownloadFile(string path, string dir, string fileName, long size, out long sumProgress, DateTime DateFile)
        {
            sumProgress = 0;
            string dir_og = path;
            using (var response = WebRequest.Create($"{link_file_download}/{fileName}.zip").GetResponse())
            using (var stream = response.GetResponseStream())
            {

                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), path)))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), path));
                }
                 path = Path.Combine(Path.GetTempPath(), "temp.zip");

                using (Stream fileStream = File.Create(path))
                {
                    byte[] buffer = new byte[size];
                    int bytesRead;
                    long totalBytesRead = 0;
                    long totalFileSize = response.ContentLength; // Assuming stream is the source stream of your file

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);

                        totalBytesRead += bytesRead;

                        // Calculate progress percentage and update your progress bar
                        sumProgress = (int)((totalBytesRead * 1000) / totalFileSize);
                        UpdateProgressBar(ref sumProgress);
                    }
                }

                var result = ExistFile(fileName, dir_og);
                if (result)
                {
                    Verificacao(fileName, dir_og, DateFile);
                }
            }
        }
        void UpdateProgressBar(ref long progressPercentage)
        {
            // Use your progress bar control to set the value
            // progressBar.Value = progressPercentage;

            // Ensure that you are not exceeding the maximum value of the progress bar
            if (progressPercentage > 1000)
            {
                progressPercentage = 1000;
            }
            else if (progressPercentage > 100)
            {
                progressPercentage = 1000;
            }
        }


        void Verificacao(string fileName, string dir, DateTime DateFile)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), dir);

            using (var zip = PangyaAPI.ZIP.ZipFile.Read(Path.Combine(Directory.GetCurrentDirectory(), "temp.zip")))
            {
                var size1 = zip.Entries.First().UncompressedSize;
                var size2 = File.Exists(Path.Combine(basePath, fileName)) ? new FileInfo(Path.Combine(basePath, fileName)).Length : 0;

                if (size1 != size2)
                {
                    if (File.Exists(Path.Combine(basePath, fileName)))
                    {
                        File.Delete(Path.Combine(basePath, fileName));
                    }
                    var fileZip = zip.Entries.First();
                    fileZip.CreationTime = DateFile;
                    fileZip.LastModified = DateFile;
                    fileZip.ModifiedTime = DateFile;
                    fileZip.Extract(basePath);
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
            var path = Path.GetTempPath() + "temp.zip";

            using (var zip = PangyaAPI.ZIP.ZipFile.Read(path))
            {
                var size1 = zip.Entries.First().UncompressedSize;
                var size2 = File.Exists(Path.Combine(basePath, dir, fileName))? File.OpenRead(Path.Combine(basePath, dir, fileName)).Length : 0;

                File.Delete(basePath + "\\temp.zip");

                if (size1 != size2)
                {
                    File.Move(path, basePath + "\\temp.zip");
                   Debug.WriteLine("[ExistFile]: checou o arquivo e está movendo....");
                    return true;
                }
                else
                {
                   Debug.WriteLine("[ExistFile]: arquivo foi checado com sucesso 1....");
                    return false;
                }
            }
        }

        public void SetValues(ProgressBar _barProcess, ProgressBar _barUpdate, Label _lbProcessDesc, Label _lblPatchVer)
        {
            barProcess = _barProcess;
            barUpdate = _barUpdate;
            lbProcessDesc = _lbProcessDesc;
            lblPatchVer = _lblPatchVer;
        }
        public void SetValues(ProgressBar _barProcess, ProgressBar _barUpdate, Label _lbProcessDesc, Label _lblPatchVer, Button Start, Button Reset)
        {
            barProcess = _barProcess;
            barUpdate = _barUpdate;
            lbProcessDesc = _lbProcessDesc;
            lblPatchVer = _lblPatchVer;
            BtnAbrirProjectG = Start;
            BtnResetPatch = Reset;
            barUpdate.SetState(1);
            barProcess.SetState(2);
        }
    }
}
