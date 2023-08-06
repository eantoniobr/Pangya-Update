using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PangyaAPI.UpdateList.Data
{
    public class FileItems
    {
        public string patchVer { get; set; }
        public uint patchNum { get; set; }
        public uint Count { get; set; }
        public List<FileItem> files { get; set; }
        public void Add(FileItem item)
        {
            if (files == null)
            {
                files = new List<FileItem>((int)Count);
            }
            files.Add(item);
        }
    }
}
