using PangyaAPI.UpdateList;
using PangyaUpdate.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace PangyaUpdate
{
    public class UpdateUnit : Patcher
    {
        private int count;

        public void OnPatchProgress(ProgressBar barProcess, ProgressBar barUpdate, Label lbProcessDesc, Label lblPatchVer, Label lbFile)
        {
            var directoryTemp = Path.GetTempPath();
            var getCurrentDirectory = Directory.GetCurrentDirectory();
            long bar = 0;
            count = 0;
            var reg = new PangyaReg();
            uint? _patchNum = 0;
            string patchVer = "0";
            // UI Control
            barProcess.Invoke((MethodInvoker)delegate { barProcess.Minimum = barProcess.Minimum; });
            lblPatchVer.Invoke((MethodInvoker)delegate { lblPatchVer.Text = $"Ver {patch_version} Update "/*coloque aqui a informação necessária*/; });
            if (reg.ExistRegMain())
            {
                _patchNum = uint.Parse(reg.getMainValue("PatchNum"));
                patchVer = reg.getMainValue("Ver");
                if (_patchNum.Value != patch_num)
                {
                    reg.CreateMainReg(patch_version, patch_num);//registra ou atualiza aqui :D
                }
                else if (patchVer != patch_version)
                {
                    reg.CreateMainReg(patch_version, patch_num);//registra ou atualiza aqui :D
                }
            }
            if (_patchNum != patch_num)
            {
                foreach (var item in UpdateListInfo.UpdateFiles.Files)
                {
                    // UI Control
                    barProcess.Invoke((MethodInvoker)delegate { barProcess.Value = ++count; });

                    lbProcessDesc.Invoke((MethodInvoker)delegate { lbProcessDesc.Text = item.Name; });

                    var fileInfo = item;
                    string text = fileInfo.Name;
                    string text2 = fileInfo.Dir;
                    long size = fileInfo.Size;
                    int crcUpt = fileInfo.Crc;

                    string path = string.IsNullOrEmpty(text2) ? getCurrentDirectory + "\\" + text : getCurrentDirectory + "\\" + text2 + "\\" + text;

                    if (File.Exists(path))
                    {
                        var FileInfo = new FileInfo(path);
                        var crcFile = path.getCrcFile();
                        if ((FileInfo.LastWriteTime.Month != item.Date.Month && FileInfo.CreationTimeUtc.Day != item.Date.Day && FileInfo.CreationTimeUtc.Year != item.Date.Year))
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else if (FileInfo.Length != size)
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else if (crcUpt != crcFile)
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else
                        {
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = ""/*coloque aqui o texto necessário*/; });
                            Debug.WriteLine($"Success: {fileInfo.Name}");
                        }
                    }
                    else
                    {
                        DownloadFile(path, directoryTemp, text, size, out bar);
                        Debug.WriteLine($"Success: {fileInfo.Name}");
                        // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                        barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                        lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                    }
                }
            }
            else if (patchVer != patch_version)
            {
                foreach (var item in UpdateListInfo.UpdateFiles.Files)
                {
                    // UI Control
                    barProcess.Invoke((MethodInvoker)delegate { barProcess.Value = ++count; });

                    lbProcessDesc.Invoke((MethodInvoker)delegate { lbProcessDesc.Text = item.Name; });

                    var fileInfo = item;
                    string text = fileInfo.Name;
                    string text2 = fileInfo.Dir;
                    long size = fileInfo.Size;
                    int crcUpt = fileInfo.Crc;

                    string path = string.IsNullOrEmpty(text2) ? getCurrentDirectory + "\\" + text : getCurrentDirectory + "\\" + text2 + "\\" + text;

                    if (File.Exists(path))
                    {
                        var FileInfo = new FileInfo(path);
                        var crcFile = path.getCrcFile();
                        if ((FileInfo.LastWriteTime.Month != item.Date.Month && FileInfo.CreationTimeUtc.Day != item.Date.Day && FileInfo.CreationTimeUtc.Year != item.Date.Year))
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else if (FileInfo.Length != size)
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else if (crcUpt != crcFile)
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else
                        {
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = ""/*coloque aqui o texto necessário*/; });
                            Debug.WriteLine($"Success: {fileInfo.Name}");
                        }
                    }
                    else
                    {
                        DownloadFile(path, directoryTemp, text, size, out bar);
                        Debug.WriteLine($"Success: {fileInfo.Name}");
                        // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                        barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                        lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                    }
                }

            }
            else
            {
                foreach (var item in UpdateListInfo.UpdateFiles.Files.Where(c => c.Name != "projectg700gb+.pak"))
                {
                    // UI Control
                    barProcess.Invoke((MethodInvoker)delegate { barProcess.Value = ++count; });

                    lbProcessDesc.Invoke((MethodInvoker)delegate { lbProcessDesc.Text = item.Name; });

                    var fileInfo = item;
                    string text = fileInfo.Name;
                    string text2 = fileInfo.Dir;
                    long size = fileInfo.Size;
                    int crcUpt = fileInfo.Crc;
                    string path = string.IsNullOrEmpty(text2) ? getCurrentDirectory + "\\" + text : getCurrentDirectory + "\\" + text2 + "\\" + text;
                    if (File.Exists(path))
                    {
                        var FileInfo = new FileInfo(path);
                        var crcFile = path.getCrcFile();
                        if ((FileInfo.LastWriteTime.Month != item.Date.Month && FileInfo.CreationTimeUtc.Day != item.Date.Day && FileInfo.CreationTimeUtc.Year != item.Date.Year))
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else if (FileInfo.Length != size)
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else if (crcUpt != crcFile)
                        {
                            DownloadFile(path, directoryTemp, text, size, out bar);
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                        }
                        else
                        {
                            // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                            lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = ""/*coloque aqui o texto necessário*/; });
                            Debug.WriteLine($"Success: {fileInfo.Name}");
                        }
                    }
                    else
                    {
                        DownloadFile(path, directoryTemp, text, size, out bar);
                        Debug.WriteLine($"Success: {fileInfo.Name}");
                        // Atualizar BarUpdate, lblPatchVer, lbFile conforme necessário
                        barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = (int)bar /*coloque aqui o valor de progresso necessário*/; });
                        lbFile.Invoke((MethodInvoker)delegate { lbFile.Text = text/*coloque aqui o texto necessário*/; });
                    }
                }
            }

            BtnAbrirProjectG.Enabled = true;
            BtnResetPatch.Enabled = true;
            barProcess.Value = 1000;
            barUpdate.Value = 1000;
            lbFile.Visible = false;
            lbProcessDesc.Text = "";
            BtnAbrirProjectG.BackgroundImage = Properties.Resources.BtnAbrirProjectG;
        }
    }
}
