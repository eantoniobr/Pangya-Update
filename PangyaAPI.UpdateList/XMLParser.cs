using PangyaAPI.UpdateList.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PangyaAPI.UpdateList
{
    public class XMLParser
    {
        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public FileItems getFiles(string path, bool ispath)
        {
            FileItems fileNameList = new FileItems();
            List<string> xmlList = new List<string>();
            var new_path = Path.GetTempPath() + "updatelist";
            var create_temp = Path.GetTempFileName();
            if (ispath)
            {
                xmlList = File.ReadAllLines(path).ToList();
                xmlList.Insert(1, "<root>");
                xmlList.Insert(xmlList.Count, "</root>");
            }
            else
            {
                File.WriteAllLines(create_temp, xmlList);
                xmlList = File.ReadAllLines(create_temp).ToList();
                xmlList.Insert(1, "<root>");
                xmlList.Insert(xmlList.Count, "</root>");
            }
            File.WriteAllLines(new_path, xmlList);
            XElement xDoc = XElement.Load(new_path);

            int id = 1;
            fileNameList.Count = (uint)(xDoc.Element("updatefiles").Attribute("count"));
            fileNameList.patchVer = (xDoc.Element("patchVer").Attribute("value").Value);
            fileNameList.patchNum = (uint)(xDoc.Element("patchNum").Attribute("value"));
            IEnumerable<XElement> files = xDoc.Element("updatefiles").Elements("fileinfo");
            foreach (XElement fileName in files)
            {
                FileItem _tempDict = new FileItem();
                if (fileName.Attribute("fdir").Value != "")
                {
                    string FixedFilename = fileName.Attribute("fdir").Value.Replace("\\", "/");
                    _tempDict.FullName = ReplaceFirst(FixedFilename, "/", "") + "\\" + fileName.Attribute("fname").Value;

                }
                _tempDict.FileID =id;
                _tempDict.Name = fileName.Attribute("fname").Value;
                _tempDict.dir = fileName.Attribute("fdir").Value.Replace("\\", "");
                _tempDict.Size = (long)fileName.Attribute("fsize");
                _tempDict.crc = (int)fileName.Attribute("fcrc");
                _tempDict.Date = fileName.Attribute("fdate").Value;
                _tempDict.time = fileName.Attribute("ftime").Value;
                _tempDict.FullDate = DateTime.Parse(_tempDict.Date + " " + _tempDict.time);
                _tempDict.PackedName = fileName.Attribute("pname").Value;//compression
                _tempDict.PackedSize = fileName.Attribute("psize").Value;//compression
                fileNameList.Add(_tempDict);
                id++;
            }
            File.Delete(new_path);
            File.Delete(create_temp);
            return fileNameList;
        }
    }
}
