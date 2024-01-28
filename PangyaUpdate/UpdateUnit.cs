using PangyaAPI.UpdateList;
using PangyaUpdate.Tools;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace PangyaUpdate
{
    public class UpdateUnit : Patcher
    {
        private int count;

        public void OnPatchProgress(ProgressBar barProcess, ProgressBar barUpdate, Label lbProcessDesc, Label lblPatchVer)
        {
            var getCurrentDirectory = Path.GetTempPath();
            var directoryTemp = Directory.GetCurrentDirectory();
            long bar = 0;
            count = 0;
            var reg = new PangyaReg();
            uint? _patchNum = 0;
            string patchVer = "0";
            // UI Control
            barProcess.Invoke((MethodInvoker)delegate { barProcess.Minimum = barProcess.Minimum; });
            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Minimum = barProcess.Minimum; });
            lblPatchVer.Invoke((MethodInvoker)delegate { lblPatchVer.Text = $"Ver {patch_version} Update "; });
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

            if (_patchNum != patch_num || patchVer != patch_version)
            {
                foreach (var item in UpdateListInfo.UpdateFiles.Files)
                {
                    // UI Control
                    barProcess.Invoke((MethodInvoker)delegate { barProcess.Value = ++count; });
                    lbProcessDesc.Invoke((MethodInvoker)delegate { lbProcessDesc.Text = item.Name; });
                    string path = string.IsNullOrEmpty(item.Dir) ? directoryTemp +  "\\" + item.Name : directoryTemp + item.Dir + "\\" + item.Name;
                    if (File.Exists(path))
                    {
                        var FileInfo = new FileInfo(path);

                        var LastWriteTimeUtc = FileInfo.CreationTime;
                        if (!LastWriteTimeUtc.Verify(item.Date) || FileInfo.Length != item.Size/* || item.Crc != path.getCrcFile()*/)
                        {
                            DownloadFile(item.Dir.Replace("\\", ""), path, item.Name, item.Size, out bar, item.Date);                             
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = 1000; });
                        }
                    }
                    else
                    {
                        DownloadFile(item.Dir.Replace("\\", ""), path, item.Name, item.Size, out bar, item.Date);
                        Debug.WriteLine($"Success: {item.Name}");
                        barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = 1000; });
                    }
                }
            }
            else
            {
                foreach (var item in UpdateListInfo.UpdateFiles.Files.Where(c => c.Name != "projectg700gb+.pak").Where(c => c.Name != "update.exe"))
                {
                    barUpdate.SetState(1);
                    barProcess.SetState(2);
                    // UI Control
                    barProcess.Invoke((MethodInvoker)delegate { barProcess.Value = ++count; });

                    lbProcessDesc.Invoke((MethodInvoker)delegate { lbProcessDesc.Text = item.Name; });
                    var fileInfo = item;
                    string text = fileInfo.Name;
                    string text2 = fileInfo.Dir;
                    long size = fileInfo.Size;
                    int crcUpt = fileInfo.Crc;
                    string path = string.IsNullOrEmpty(text2) ? directoryTemp +  "\\" + text : directoryTemp + text2 + "\\" + text;
                    if (File.Exists(path))
                    {
                        var FileInfo = new FileInfo(path);
                        var LastWriteTimeUtc = FileInfo.CreationTime;
                        if (!LastWriteTimeUtc.Verify(item.Date) || FileInfo.Length != item.Size/* || item.Crc != path.getCrcFile()*/)
                        {
                            DownloadFile(item.Dir.Replace("\\", ""), path, item.Name, item.Size, out bar, item.Date);
                            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = 1000; });
                        }
                    }
                    else
                    {
                        DownloadFile(text2.Replace("\\", ""), path, text, size, out bar, item.Date);
                        Debug.WriteLine($"Success: {fileInfo.Name}");                           
                        barUpdate.Invoke((MethodInvoker)delegate { barProcess.Value = 1000; });
                    }
                }
            }

            BtnAbrirProjectG.Enabled = true;
            BtnResetPatch.Enabled = true;
            barProcess.Invoke((MethodInvoker)delegate { barProcess.Value = 1000; });
            barUpdate.Invoke((MethodInvoker)delegate { barUpdate.Value = 1000; });
            lbProcessDesc.Text = "";
            BtnAbrirProjectG.BackgroundImage = Properties.Resources.BtnJogar_OK;
        }                

    }
}
