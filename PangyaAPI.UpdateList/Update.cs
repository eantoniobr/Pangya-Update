using PangyaAPI.UpdateList.Data;
using PangyaAPI.Utilities;
using PangyaAPI.Utilities.BinaryModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using static PangyaAPI.UpdateList.UpdateList;
namespace PangyaAPI.UpdateList
{
    public class Patch
    {
        public enum ProgressStatus
        {
            INITIAL = 0,
            VERIFYING = 1,
            DOWNLOADING = 2,
            DONE = 3,
            UPDATE,
            RUN
        }

        public delegate void ProgressEventHandler(ProgressStatus status, string name = "", long progress = 0L, long size = 0L, int total_progress = 0, int max_progress = 1000);
        public static uint patch_num;
        public static string patch_version;
        public static string Notice = "http://gameraze.com.br/Notes/Patcher";
        private static string link_file_download = "http://pangya21.xyz/pangya/season4/patch/";
        static string link_file_updatelist = "updatelist";
        static string upt_decript = Path.GetTempPath() + "\\updatelist_decrypt.xml";
        public static event ProgressEventHandler OnProgressEvent;
        static FileItems UpdateListInfo { get; set; }
        public static void SetConfigUpdate()
        {
            try
            {
                IniHandle ini = new IniHandle("Config.ini");

                Notice = ini.ReadString("OPTION", "Banner", "http://gameraze.com.br/Notes/Patcher");
                link_file_download = ini.ReadString("OPTION", "Initial_Path", "http://pangya21.xyz/pangya/season4/patch/");
                link_file_updatelist = link_file_download + link_file_updatelist;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public static void SetPatchNum(uint number)
        {
            patch_num = number;
            File.WriteAllText(Path.GetTempPath() + "\\patch.dat", number.ToString());
        }

        public static void GetPatchNum()
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

            patch_num = 1;
            File.WriteAllText(filePath, "1");
        }

        public static void RePatch()
        {
            patch_num = 0u;
            Download();
        }

        public static void Download()
        {
            if (Process.GetProcessesByName("ProjectG.exe").Length > 0)
            {
                MessageBox.Show("Please close the game!");
                Environment.Exit(0);
            }

            var update_file = true;
            var directory_temp = Path.GetTempPath();
            var getcurrentdirectory = Directory.GetCurrentDirectory();

            new Thread(() =>
            {
                try
                {
                    OnProgressEvent?.Invoke(ProgressStatus.INITIAL);
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                        byte[] data = webClient.DownloadData(link_file_updatelist);

                        var upt = new UpdateList();
                        upt.DecryptEncryptFile(data, KeyEnum.JP);
                        XMLParser updatelist2 = new XMLParser();
                        upt.SaveFile(Path.GetTempPath(), OperacaoEnum.Decrypt);

                        UpdateListInfo = updatelist2.getFiles(upt_decript, true);

                        uint num = patch_num;
                        patch_num = UpdateListInfo.patchNum;
                        patch_version = UpdateListInfo.patchVer;
                        OnProgressEvent?.Invoke(ProgressStatus.RUN);

                        if (num != patch_num)
                        {
                            int num2 = (int)UpdateListInfo.Count;
                            long num3 = 0L;
                            long sum_progress = 0L;
                            long size_test = 0;

                            for (int j = 0; j < num2; j++)
                            {

                                num3 += (long)UpdateListInfo.files[j].Size;
                            }

                            for (int k = 0; k < num2; k++)
                            {
                                var fileInfo = UpdateListInfo.files[k];
                                string text = fileInfo.Name;
                                string text2 = fileInfo.dir;
                                long size = fileInfo.Size;
                                int num4 = fileInfo.crc;

                                string path = Path.Combine(Directory.GetCurrentDirectory(), text2, text);
                                OnProgressEvent?.Invoke(ProgressStatus.VERIFYING, text, 0L, size, (int)((double)sum_progress / (double)num3 * 1000.0));

                                if (File.Exists(path))
                                {
                                    update_file = fileInfo.CheckUpdate();
                                    if (fileInfo.Name.Contains(".ini"))
                                    {
                                        fileInfo.CheckUpdate();
                                    }
                                    size_test = GetFileSize(path);
                                }
                                if (IsExist(size, size_test))
                                {
                                    if (update_file)
                                    {
                                        DownloadFile(path, directory_temp, text2, text, getcurrentdirectory, size, sum_progress, num3);
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"Sucess: {fileInfo.Name}");
                                    }
                                }
                            }
                            SetPatchNum(num);
                        }
                    }
                    OnProgressEvent?.Invoke(ProgressStatus.DONE, "", 0L, 0L);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Response.ToString());
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message);
                }
            }).Start();
        }

        internal static void Progress(ProgressStatus status, string name, long position, long length, int v)
        {
            OnProgressEvent?.Invoke(status, name, position, length, v);
        }

        public static bool Modificado(string url, DateTime desde)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                request.Method = "Head";
                request.AllowAutoRedirect = false;
                request.IfModifiedSince = desde;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    return response.StatusCode != HttpStatusCode.NotModified;
                }
            }
            return false;
        }

        static public DateTime PegarDataCriacao(string caminhoBD)
        {
            FileInfo arquivo = new FileInfo(caminhoBD);
            DateTime dataCriacao = arquivo.LastWriteTime;

            return dataCriacao;
        }

        static bool Verificacao(string real_filename, string txt2)
        {
            var basepath = Path.Combine(Directory.GetCurrentDirectory(), txt2);
            using (var zip = PangyaAPI.ZIP.ZipFile.Read(Path.Combine(Directory.GetCurrentDirectory(), "temp.zip")))
            {
                var creation = zip.Entries.First().LastModified;
                var size1 = zip.Entries.First().UncompressedSize;
                var size2 = new FileInfo(Path.Combine(basepath, real_filename)).Length;
                var createin = DateTime.Now;

                if (!string.IsNullOrEmpty(txt2))
                {
                    createin = PegarDataCriacao(Path.Combine(basepath, real_filename));
                    if (size1 != size2)
                    {
                        if (File.Exists(Path.Combine(basepath, real_filename)))
                        {
                            File.Delete(Path.Combine(basepath, real_filename));
                        }
                        zip.ExtractSelectedEntries(real_filename, "", basepath);
                        OnProgressEvent?.Invoke(ProgressStatus.UPDATE, real_filename, 0, size1, 1000);
                        System.Diagnostics.Debug.WriteLine("[Verificacao]: verificado e atualizando o arquivo....");

                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[Verificacao]: nao atualizou o arquivo....");
                        return false;
                    }
                }
                System.Diagnostics.Debug.WriteLine("[Verificacao]: nao atualizou o arquivo 2....");
                return false;
            }
        }
        static bool IsExist(long size, long size2)
        {
            return size2 == size;
        }
        static bool ExistFile(string real_filename, string txt2)
        {
            var basepath = Directory.GetCurrentDirectory();
            var path = Path.GetTempPath() + "\\temp.zip";

            using (var zip = PangyaAPI.ZIP.ZipFile.Read(path))
            {
                var creation = zip.Entries.First().LastModified;
                var size1 = zip.Entries.First().UncompressedSize;
                var size2 = File.OpenRead(basepath + "\\" + txt2 + "\\" + real_filename).Length;
                var createin = DateTime.Now;
                if (txt2 != "")
                {
                    File.Delete(basepath + "\\temp.zip");
                    if (size1 != size2)
                    {
                        File.Move(path, basepath + "\\temp.zip");
                        System.Diagnostics.Debug.WriteLine("[ExistFile]: checou o arquivo e esta movendo....");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[ExistFile]: arquivo foi checado com sucesso 1....");
                        return false;
                    }
                }
                System.Diagnostics.Debug.WriteLine("[ExistFile]: arquivo foi checado com sucesso 2....");
                return false;
            }
        }
        static void DownloadFile(string path, string directory_temp, string text2, string text, string getcurrentdirectory, long size, long sum_progress, long num3)
        {
            OnProgressEvent?.Invoke(ProgressStatus.DOWNLOADING, text, 0L, size, (int)((double)sum_progress / (double)num3 * 1000.0));
            var webhost = link_file_download + "/" + text + ".zip";
            try
            {
                using (Stream stream2 = WebRequest.Create(webhost).GetResponse().GetResponseStream())
                {
                    var size_t = stream2.Length;
                    path = Path.Combine(directory_temp, "temp.zip");
                    if (!string.IsNullOrEmpty(text2) && !Directory.Exists(Path.Combine(getcurrentdirectory, text2)))
                    {
                        Directory.CreateDirectory(Path.Combine(getcurrentdirectory, text2));
                    }

                    using (Stream stream3 = File.Create(path))
                    {
                        byte[] array = new byte[size];
                        int num5;
                        while ((num5 = stream2.Read(array, 0, array.Length)) > 0)
                        {
                            stream3.Write(array, 0, num5);
                            OnProgressEvent?.Invoke(ProgressStatus.DOWNLOADING, text, stream3.Position, size, (int)((double)(sum_progress += num5) / (double)num3 * 1000.0));
                        }
                    }

                    var result = ExistFile(text, text2);
                    if (result)
                    {
                        Verificacao(text, text2);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        static int GetChecksumBuffered(string name, long sum_size, ref long sum_progress)
        {
            FileStream fileStream;
            Crc32Ex crc32;
            fileStream = new FileStream(name, FileMode.Open, FileAccess.Read);
            crc32 = new Crc32Ex();
            byte[] array;
            array = new byte[fileStream.Length];
            int num;
            num = 0;
            int num2;
            while ((num2 = fileStream.Read(array, num, array.Length - num)) > 0)
            {
                num += num2;
                if (num == array.Length)
                {
                    int num3;
                    num3 = fileStream.ReadByte();
                    if (num3 == -1)
                    {
                        return crc32.NativeCalculateHash32(array);
                    }
                    byte[] array2;
                    array2 = new byte[array.Length * 2];
                    Array.Copy(array, array2, array.Length);
                    array2[num] = (byte)num3;
                    array = array2;
                    num++;
                }
                Progress(ProgressStatus.VERIFYING, name, fileStream.Position, fileStream.Length, (int)((double)(sum_progress += num2) / (double)sum_size * 1000.0));
            }
            byte[] array3;
            array3 = new byte[num];
            Array.Copy(array, array3, num);
            Progress(ProgressStatus.VERIFYING, name, 0, 0, (int)((double)(sum_progress += num2) / (double)sum_size * 1000.0));
            fileStream.Close();
            return crc32.NativeCalculateHash32(array3);
        }
        public static long GetFileSize(string filePath)
        {
            try
            {
                IntPtr fileHandle = CreateFile(filePath, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

                if (fileHandle != IntPtr.Zero)
                {
                    if (GetFileSizeEx(fileHandle, out long fileSize))
                    {
                        return fileSize;
                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao ler o tamanho do arquivo: " + ex.Message);
                return -1;
            }
        }
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetFileSizeEx(IntPtr hFile, out long lpFileSize);
    }
}
