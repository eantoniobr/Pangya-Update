using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
namespace PangyaAPI.UpdateList
{
    #region data

    public class DocumentInfo
    {
        public string Version { get; set; }
        public string Encoding { get; set; }
        public string Standalone { get; set; }
        public Document Document { get; set; }
        public DocumentInfo()
        {
            Document = new Document();
            Version = "1.0";
            Encoding = "1.0";
            Standalone = "yes";
        }
    }

    [XmlRoot("root")]
    public class Document
    {
        [XmlElement("patchVer")]
        public PatchVersion PatchVersion { get; set; }

        [XmlElement("patchNum")]
        public PatchNumber PatchNumber { get; set; }

        [XmlElement("updatelistVer")]
        public UpdateListVersion UpdateListVersion { get; set; }

        [XmlElement("updatefiles")]
        public UpdateFiles UpdateFiles { get; set; }

        public Document()
        {
            PatchVersion = new PatchVersion();
            PatchNumber = new PatchNumber();
            UpdateListVersion = new UpdateListVersion();
            UpdateFiles = new UpdateFiles();
        }

        public Document(PatchVersion value, PatchNumber value2, UpdateListVersion value3, UpdateFiles value4)
        {
            PatchVersion = value;
            PatchNumber = value2;
            UpdateListVersion = value3;
            UpdateFiles = value4;
        }

        public void AddFile(string key, FileItem value)
        {
            UpdateFiles.Files.Add(value);
            UpdateFiles.Count++;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(); // Move to the root element

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "patchVer":
                            PatchVersion.Value = reader.GetAttribute("value");
                            reader.Read(); // Move to the next node
                            break;
                        case "patchNum":
                            PatchNumber.Value = int.Parse(reader.GetAttribute("value"));
                            reader.Read(); // Move to the next node
                            break;
                        case "updatelistVer":
                            UpdateListVersion.Value = reader.GetAttribute("value");
                            reader.Read(); // Move to the next node
                            break;
                        case "updatefiles":
                            UpdateFiles = new UpdateFiles
                            {
                                Count = int.Parse(reader.GetAttribute("count"))
                            };
                            UpdateFiles.ReadXml(reader);
                            break;
                        default:
                            reader.Skip(); // Skip unknown elements
                            break;
                    }
                }
                else
                {
                    reader.Read(); // Move to the next node
                }
            }

            reader.ReadEndElement(); // Consume the end tag of the root element
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("patchVer", PatchVersion.Value);
            writer.WriteElementString("patchNum", PatchNumber.Value.ToString());
            writer.WriteElementString("updatelistVer", UpdateListVersion.Value);
            UpdateFiles.WriteXml(writer);
        }


        public string SerializeToXml()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Document));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Use XmlWriterSettings to configure the XmlTextWriter
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = Encoding.GetEncoding("euc-kr"),
                    Indent = true,
                };

                using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
                {
                    //applying "empty" namespace will produce no namespaces
                    var emptyNamespaces = new XmlSerializerNamespaces();
                    emptyNamespaces.Add("", "any-non-empty-string");  //remove algumas coisas do xml

                    writer.WriteStartDocument(true);
                    xmlSerializer.Serialize(writer, this, emptyNamespaces);
                }

                // Reset the stream position before reading
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.GetEncoding("euc-kr")))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }

    public class PatchVersion
    {
        [XmlAttribute("value")]
        public string Value { get; set; }

        public PatchVersion()
        {
            Value = "GB.R7.824";
        }
    }

    public class PatchNumber
    {
        [XmlAttribute("value")]
        public int Value { get; set; }

        public PatchNumber()
        {
            Value = 823;
        }
    }

    public class UpdateListVersion
    {
        [XmlAttribute("value")]
        public string Value { get; set; }

        public UpdateListVersion()
        {
            Value = "20090331";
        }
    }

    public class UpdateFiles
    {
        [XmlAttribute("count")]
        public int Count { get; set; }
        //[XmlArray("fileinfos")]  não tem no xml
        [XmlElement("fileinfo", ElementName = "fileinfo")]
        public List<FileItem> Files { get; set; }

        public UpdateFiles()
        {
            Files = new List<FileItem>();
        }

        public bool ContainsKey(string name)
        {
            return Files.Any(c => c.Name == name);
        }

        public FileItem getItem(string name)
        {
            return Files.Where(c => c.Name == name).First();
        }

        public List<FileItem> getFileExists(bool exist = true)
        {
            var Temp = new List<FileItem>();
            var directoryTemp = Directory.GetCurrentDirectory();
            foreach (var item in Files)
            {
                string text = item.Name;
                string text2 = item.Dir;
                string path = string.IsNullOrEmpty(text2) ? directoryTemp + "\\" + text : directoryTemp + text2 + "\\" + text;

                if (File.Exists(path) == exist)
                    Temp.Add(item);
            }
            return Temp;
        }
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(); // Move to the start of the updatefiles element
            Files = new List<FileItem>();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "fileinfo")
                {
                    var fileItem = new FileItem();
                    fileItem.ReadXml(reader);
                    Files.Add(fileItem);
                }
                else
                {
                    reader.Skip(); // Skip unknown elements
                }
            }

            reader.ReadEndElement(); // Consume the end tag of updatefiles
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("count", Count.ToString());

            writer.WriteStartElement("fileinfos");
            foreach (var fileItem in Files)
            {
                writer.WriteStartElement("fileinfo");
                fileItem.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Close the fileinfos element
        }
    }

    public class FileItem : IXmlSerializable
    {
        [XmlAttribute("fname")]
        public string Name { get; set; }

        [XmlAttribute("fdir")]
        public string Dir { get; set; }

        [XmlAttribute("fsize")]
        public long Size { get; set; }

        [XmlAttribute("fcrc")]
        public int Crc { get; set; }

        [XmlAttribute("fdate")]
        public DateTime Date { get; set; }

        [XmlAttribute("ftime")]
        public DateTime Time { get; set; }

        [XmlAttribute("pname")]
        public string PName { get; set; }

        [XmlAttribute("psize")]
        public long PSize { get; set; }
                              
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader fileName)
        {
            Name = fileName.GetAttribute("fname");
            Dir = fileName.GetAttribute("fdir");
            Size = long.Parse(fileName.GetAttribute("fsize"));
            Crc = int.Parse(fileName.GetAttribute("fcrc"));
            var date = fileName.GetAttribute("fdate") + " " +fileName.GetAttribute("ftime");
            Date = DateTime.Parse(date);
            Time = DateTime.Parse(date);
            PName = fileName.GetAttribute("pname");//compression
            PSize = long.Parse(fileName.GetAttribute("psize"));//compression
            fileName.Read(); // ler o proximo
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("fname", Name);
            writer.WriteAttributeString("fdir", Dir);
            writer.WriteAttributeString("fsize", Size.ToString());
            writer.WriteAttributeString("fcrc", Crc.ToString());
            writer.WriteAttributeString("fdate", Date.ToString("yyyy-MM-dd"));
            writer.WriteAttributeString("ftime", Time.ToString("HH:mm:ss"));
            writer.WriteAttributeString("pname", PName);
            writer.WriteAttributeString("psize", PSize.ToString());
        }
    }
    public class FileItemEx : FileItem
    {
        public string FullPath { get; set; }
        public string CheckSum { get; set; }
        public int Index { get; set; }

        public int Tipo { get; set; }
        public bool Main { get; set; } = true;//significa se esta em pasta ou no inicio
    }
    public class PangyaZipFiles
    {
        public string _localZipFolder { get; set; }
        /// <summary>
        /// arquivos zipados/ atualizados
        /// </summary>
        public Dictionary<string, FileItemEx> UpdateFileList { get; set; }

        public PangyaZipFiles(string _root, Dictionary<string, FileItemEx> pairs)
        {
            UpdateFileList = pairs;
            _localZipFolder = _root;
        }
    }
    #endregion
    public class XMLParser
    {
        public string FilePath { get; set; }
        public Document item { get; set; }
        public UpdateList updateList { get; set; }
        public XMLParser(string _file)
        {
            FilePath = _file;

            item = new Document();
        }
        public XMLParser()
        {
            item = new Document();
        }
        public XMLParser(string _file, string Key) : this(_file)
        {
            updateList = new UpdateList(_file + "updatelist", Key);
        }

        public XMLParser(byte[] data, string _file, string Key)
        {
            updateList = new UpdateList(_file, Key);
            updateList.Document = data;
        }

        public void Initial(string _file, string Key)
        {
            FilePath = _file + "updatelist";
            updateList = new UpdateList(FilePath, Key);
        }

        private Document getFiles(string xmlString)
        {
            try
            {
                if (xmlString.Contains("root") == false)
                {
                    xmlString = xmlString.ToStringXML();
                }
                item.ReadXml(XElement.Parse(xmlString).CreateReader());
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XMLParse.getFiles: " + ex.Message);
                return item;
            }
        }

        public Document getFiles()
        {
            try
            {
                var xmlString = getFileFormatXml();
                if (xmlString.Contains("root") == false)
                {
                    xmlString = xmlString.ToStringXML();
                }
                item.ReadXml(XElement.Parse(xmlString).CreateReader());
                item.UpdateFiles.Files= item.UpdateFiles.Files.OrderBy(c => c.Dir == "").ToList();
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XMLParse.getFiles: " + ex.Message);
                return item;
            }
        }

        public bool createNewEmptyXML(List<FileItem> files, string _version, string _number)
        {
            try
            {
                item.PatchVersion.Value = _version;
                item.PatchNumber.Value = int.Parse(_number);
                item.UpdateFiles.Count = files.Count;
                item.UpdateFiles.Files = files;
                var scontent = item.SerializeToXml().RemoveToStringXML();
                updateList.Document = Encoding.UTF8.GetBytes(scontent);
                return EncryptFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("XMLParse.WriteXMLFiles: " + ex.Message);
                return false;
            }
        }

        public void Test()
        {
            // Criando um objeto Document com alguns dados de exemplo
            var document = new Document(new PatchVersion { Value = "GB.R7.824.00" }, new PatchNumber { Value = 823 }, new UpdateListVersion { Value = "20090331" }, new UpdateFiles
            {
                Count = 3,
                Files = new List<FileItem>
                {
                    {
                        new FileItem
                        {
                            Name = "8077150d.png",
                            Dir = "\\emblem",
                            Size = 539,
                            Crc = -42043499,
                            Date = DateTime.Parse("2013-12-15"),
                            Time = DateTime.Parse("02:22:10"),
                            PName = "8077150d.png.zip",
                            PSize = 692
                        }
                    } , {
                        new FileItem
                        {
                            Name = "8077150A.png",
                            Dir = "\\emblem",
                            Size = 539,
                            Crc = -42043499,
                            Date = DateTime.Parse("2013-12-15"),
                            Time = DateTime.Parse("02:22:10"),
                            PName = "8077150A.png.zip",
                            PSize = 612
                        }
                    }
                    , {
                        new FileItem
                        {
                            Name = "8077150Y.png",
                            Dir = "\\emblem",
                            Size = 539,
                            Crc = -42043499,
                            Date = DateTime.Parse("2013-12-15"),
                            Time = DateTime.Parse("02:22:10"),
                            PName = "8077150Y.png.zip",
                            PSize = 622
                        }
                    }
                }
            });

            // Serializando o objeto Document para XML
            string xmlString = document.SerializeToXml();
            // Deserializando o objeto XML para Document
            Document deserializedDocument = getFiles(xmlString);
            File.WriteAllText("updatelisttest.xml", xmlString);  //escreve em arquivo
        }

        public void TestXML()
        {

            string xmlString = "<?xml version=\"1.0\" encoding=\"euc-kr\" standalone=\"yes\" ?>\r\n<patchVer value=\"JP.R7.983.01\" />\r\n<patchNum value=\"1\" />\r\n<updatelistVer value=\"20090331\" />\r\n<updatefiles count=\"266\">\r\n\t<fileinfo fname=\"pangya_000.jpg\" fdir=\"capture\\\" fsize=\"392530\" fcrc=\"-131548682\" fdate=\"2023-12-15\" ftime=\"15:51:20\" pname=\"pangya_000.jpg.zip\"  psize=\"382955\" />\r\n\t<fileinfo fname=\"073D3515.png\" fdir=\"emblem\\\" fsize=\"3859\" fcrc=\"-131557704\" fdate=\"2023-11-24\" ftime=\"15:45:30\" pname=\"073D3515.png.zip\"  psize=\"3859\" />\r\n\t<fileinfo fname=\"1B3430F6.png\" fdir=\"emblem\\\" fsize=\"901\" fcrc=\"-101860714\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"1B3430F6.png.zip\"  psize=\"901\" />\r\n\t<fileinfo fname=\"324ae334.png\" fdir=\"emblem\\\" fsize=\"1909\" fcrc=\"-125755276\" fdate=\"2023-11-17\" ftime=\"15:26:32\" pname=\"324ae334.png.zip\"  psize=\"1868\" />\r\n\t<fileinfo fname=\"3980E37D.png\" fdir=\"emblem\\\" fsize=\"1181\" fcrc=\"-9361280\" fdate=\"2023-11-24\" ftime=\"15:45:24\" pname=\"3980E37D.png.zip\"  psize=\"1181\" />\r\n\t<fileinfo fname=\"46B3FE14.png\" fdir=\"emblem\\\" fsize=\"1079\" fcrc=\"-12104093\" fdate=\"2023-11-24\" ftime=\"16:03:45\" pname=\"46B3FE14.png.zip\"  psize=\"1079\" />\r\n\t<fileinfo fname=\"4C2BF44E.png\" fdir=\"emblem\\\" fsize=\"1636\" fcrc=\"-115250300\" fdate=\"2023-11-24\" ftime=\"16:03:45\" pname=\"4C2BF44E.png.zip\"  psize=\"1636\" />\r\n\t<fileinfo fname=\"52b4e5cb.png\" fdir=\"emblem\\\" fsize=\"1407\" fcrc=\"-15199648\" fdate=\"2023-11-24\" ftime=\"17:47:12\" pname=\"52b4e5cb.png.zip\"  psize=\"1392\" />\r\n\t<fileinfo fname=\"5707377b.png\" fdir=\"emblem\\\" fsize=\"924\" fcrc=\"-68499646\" fdate=\"2023-11-06\" ftime=\"22:14:49\" pname=\"5707377b.png.zip\"  psize=\"924\" />\r\n\t<fileinfo fname=\"571A8EB6.png\" fdir=\"emblem\\\" fsize=\"1547\" fcrc=\"-71542047\" fdate=\"2023-11-24\" ftime=\"15:45:25\" pname=\"571A8EB6.png.zip\"  psize=\"1547\" />\r\n\t<fileinfo fname=\"5D6B1DC1.png\" fdir=\"emblem\\\" fsize=\"622\" fcrc=\"-22869746\" fdate=\"2023-11-24\" ftime=\"16:03:45\" pname=\"5D6B1DC1.png.zip\"  psize=\"622\" />\r\n\t<fileinfo fname=\"65695169.png\" fdir=\"emblem\\\" fsize=\"3138\" fcrc=\"-3731964\" fdate=\"2023-11-10\" ftime=\"01:08:20\" pname=\"65695169.png.zip\"  psize=\"2441\" />\r\n\t<fileinfo fname=\"6E56AFA3.png\" fdir=\"emblem\\\" fsize=\"1016\" fcrc=\"-45607042\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"6E56AFA3.png.zip\"  psize=\"1016\" />\r\n\t<fileinfo fname=\"6_0.png\" fdir=\"emblem\\\" fsize=\"446\" fcrc=\"-88145621\" fdate=\"2023-12-06\" ftime=\"02:25:35\" pname=\"6_0.png.zip\"  psize=\"446\" />\r\n\t<fileinfo fname=\"78FE774B.png\" fdir=\"emblem\\\" fsize=\"446\" fcrc=\"-88145621\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"78FE774B.png.zip\"  psize=\"446\" />\r\n\t<fileinfo fname=\"7c475b18.png\" fdir=\"emblem\\\" fsize=\"1371\" fcrc=\"-123989910\" fdate=\"2023-11-11\" ftime=\"15:54:27\" pname=\"7c475b18.png.zip\"  psize=\"1261\" />\r\n\t<fileinfo fname=\"7C997459.png\" fdir=\"emblem\\\" fsize=\"1139\" fcrc=\"-51400714\" fdate=\"2023-11-24\" ftime=\"15:45:25\" pname=\"7C997459.png.zip\"  psize=\"1139\" />\r\n\t<fileinfo fname=\"8077150d.png\" fdir=\"\\emblem\" fsize=\"539\" fcrc=\"-42043499\" fdate=\"2013-12-15\" ftime=\"02:22:10\" pname=\"8077150d.png.zip\"  psize=\"692\" />\r\n\t<fileinfo fname=\"8234D20A.png\" fdir=\"emblem\\\" fsize=\"3812\" fcrc=\"-106254870\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"8234D20A.png.zip\"  psize=\"3812\" />\r\n\t<fileinfo fname=\"8d3067f5.png\" fdir=\"emblem\\\" fsize=\"846\" fcrc=\"-69774796\" fdate=\"2023-10-28\" ftime=\"12:52:18\" pname=\"8d3067f5.png.zip\"  psize=\"846\" />\r\n\t<fileinfo fname=\"9033a3b3.png\" fdir=\"emblem\\\" fsize=\"1116\" fcrc=\"-32751771\" fdate=\"2023-11-09\" ftime=\"13:38:14\" pname=\"9033a3b3.png.zip\"  psize=\"1116\" />\r\n\t<fileinfo fname=\"96302E20.png\" fdir=\"emblem\\\" fsize=\"1240\" fcrc=\"-34493746\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"96302E20.png.zip\"  psize=\"1240\" />\r\n\t<fileinfo fname=\"96AFD0ED.png\" fdir=\"emblem\\\" fsize=\"1202\" fcrc=\"-100827767\" fdate=\"2023-11-24\" ftime=\"16:03:45\" pname=\"96AFD0ED.png.zip\"  psize=\"1202\" />\r\n\t<fileinfo fname=\"9affaabc.png\" fdir=\"\\emblem\" fsize=\"1542\" fcrc=\"-50796433\" fdate=\"2016-05-15\" ftime=\"18:49:42\" pname=\"9affaabc.png.zip\"  psize=\"1695\" />\r\n\t<fileinfo fname=\"a50af6ea.png\" fdir=\"emblem\\\" fsize=\"3744\" fcrc=\"-53129917\" fdate=\"2023-10-31\" ftime=\"14:16:19\" pname=\"a50af6ea.png.zip\"  psize=\"3744\" />\r\n\t<fileinfo fname=\"b14cb68e.png\" fdir=\"emblem\\\" fsize=\"4004\" fcrc=\"-106719429\" fdate=\"2023-11-09\" ftime=\"14:07:04\" pname=\"b14cb68e.png.zip\"  psize=\"4004\" />\r\n\t<fileinfo fname=\"B4F74D02.png\" fdir=\"emblem\\\" fsize=\"1115\" fcrc=\"-60906488\" fdate=\"2023-11-24\" ftime=\"15:45:25\" pname=\"B4F74D02.png.zip\"  psize=\"1115\" />\r\n\t<fileinfo fname=\"BF38999A.png\" fdir=\"emblem\\\" fsize=\"1723\" fcrc=\"-100056698\" fdate=\"2023-11-24\" ftime=\"15:45:25\" pname=\"BF38999A.png.zip\"  psize=\"1723\" />\r\n\t<fileinfo fname=\"c8bc333a.png\" fdir=\"emblem\\\" fsize=\"1375\" fcrc=\"-119209024\" fdate=\"2023-11-07\" ftime=\"18:31:16\" pname=\"c8bc333a.png.zip\"  psize=\"1375\" />\r\n\t<fileinfo fname=\"cad660b9.png\" fdir=\"emblem\\\" fsize=\"1569\" fcrc=\"-22272994\" fdate=\"2020-01-28\" ftime=\"13:35:40\" pname=\"cad660b9.png.zip\"  psize=\"1569\" />\r\n\t<fileinfo fname=\"d3cc3861.png\" fdir=\"\\emblem\" fsize=\"1288\" fcrc=\"-23609696\" fdate=\"2014-01-04\" ftime=\"03:52:22\" pname=\"d3cc3861.png.zip\"  psize=\"1411\" />\r\n\t<fileinfo fname=\"D7313954.png\" fdir=\"emblem\\\" fsize=\"4112\" fcrc=\"-52317464\" fdate=\"2023-11-24\" ftime=\"15:45:30\" pname=\"D7313954.png.zip\"  psize=\"4112\" />\r\n\t<fileinfo fname=\"E39A31D0.png\" fdir=\"emblem\\\" fsize=\"516\" fcrc=\"-50522666\" fdate=\"2023-11-24\" ftime=\"16:03:45\" pname=\"E39A31D0.png.zip\"  psize=\"516\" />\r\n\t<fileinfo fname=\"E695B2EF.png\" fdir=\"emblem\\\" fsize=\"879\" fcrc=\"-7920413\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"E695B2EF.png.zip\"  psize=\"879\" />\r\n\t<fileinfo fname=\"EB08FE7F.png\" fdir=\"emblem\\\" fsize=\"1211\" fcrc=\"-72540515\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"EB08FE7F.png.zip\"  psize=\"1211\" />\r\n\t<fileinfo fname=\"F1F37001.png\" fdir=\"emblem\\\" fsize=\"1418\" fcrc=\"-101335132\" fdate=\"2023-11-24\" ftime=\"15:45:31\" pname=\"F1F37001.png.zip\"  psize=\"1418\" />\r\n\t<fileinfo fname=\"f861a5a6.png\" fdir=\"\\emblem\" fsize=\"1272\" fcrc=\"-8418412\" fdate=\"2012-09-10\" ftime=\"04:32:46\" pname=\"f861a5a6.png.zip\"  psize=\"1425\" />\r\n\t<fileinfo fname=\"f8706712.png\" fdir=\"emblem\\\" fsize=\"1669\" fcrc=\"-36060534\" fdate=\"2023-11-22\" ftime=\"17:01:15\" pname=\"f8706712.png.zip\"  psize=\"1669\" />\r\n\t<fileinfo fname=\"GUILDMARK.png\" fdir=\"\\emblem\" fsize=\"289\" fcrc=\"-22999400\" fdate=\"2010-09-09\" ftime=\"05:27:28\" pname=\"GUILDMARK.png.zip\"  psize=\"444\" />\r\n\t<fileinfo fname=\"2016020301.jpg\" fdir=\"\\extrares\" fsize=\"256810\" fcrc=\"-128791172\" fdate=\"2016-02-03\" ftime=\"10:24:01\" pname=\"2016020301.jpg.zip\"  psize=\"256594\" />\r\n\t<fileinfo fname=\"2016020302.jpg\" fdir=\"\\extrares\" fsize=\"349885\" fcrc=\"-106294471\" fdate=\"2016-02-03\" ftime=\"10:24:01\" pname=\"2016020302.jpg.zip\"  psize=\"349599\" />\r\n\t<fileinfo fname=\"2016021702.jpg\" fdir=\"\\extrares\" fsize=\"252605\" fcrc=\"-84307025\" fdate=\"2016-02-17\" ftime=\"10:28:42\" pname=\"2016021702.jpg.zip\"  psize=\"252177\" />\r\n\t<fileinfo fname=\"2016030202.jpg\" fdir=\"\\extrares\" fsize=\"186732\" fcrc=\"-74972068\" fdate=\"2016-03-02\" ftime=\"07:43:12\" pname=\"2016030202.jpg.zip\"  psize=\"186724\" />\r\n\t<fileinfo fname=\"2016033002.jpg\" fdir=\"\\extrares\" fsize=\"215245\" fcrc=\"-85404488\" fdate=\"2016-03-30\" ftime=\"08:21:09\" pname=\"2016033002.jpg.zip\"  psize=\"212080\" />\r\n\t<fileinfo fname=\"2016041501.jpg\" fdir=\"\\extrares\" fsize=\"208027\" fcrc=\"-80137001\" fdate=\"2016-04-15\" ftime=\"07:17:21\" pname=\"2016041501.jpg.zip\"  psize=\"207299\" />\r\n\t<fileinfo fname=\"2016041502.jpg\" fdir=\"\\extrares\" fsize=\"248952\" fcrc=\"-106455771\" fdate=\"2016-04-15\" ftime=\"07:17:21\" pname=\"2016041502.jpg.zip\"  psize=\"248151\" />\r\n\t<fileinfo fname=\"2016121500.jpg\" fdir=\"\\extrares\" fsize=\"102610\" fcrc=\"-40221953\" fdate=\"2012-06-13\" ftime=\"13:07:36\" pname=\"2016121500.jpg.zip\"  psize=\"102407\" />\r\n\t<fileinfo fname=\"2016121600.jpg\" fdir=\"\\extrares\" fsize=\"89468\" fcrc=\"-29660975\" fdate=\"2016-12-16\" ftime=\"16:18:03\" pname=\"2016121600.jpg.zip\"  psize=\"89458\" />\r\n\t<fileinfo fname=\"2016121601.jpg\" fdir=\"\\extrares\" fsize=\"69022\" fcrc=\"-52111893\" fdate=\"2016-12-16\" ftime=\"04:45:19\" pname=\"2016121601.jpg.zip\"  psize=\"69018\" />\r\n\t<fileinfo fname=\"background_01.jpg\" fdir=\"\\extrares\" fsize=\"383873\" fcrc=\"-31674039\" fdate=\"2014-05-26\" ftime=\"10:07:31\" pname=\"background_01.jpg.zip\"  psize=\"371410\" />\r\n\t<fileinfo fname=\"background_02.jpg\" fdir=\"\\extrares\" fsize=\"501801\" fcrc=\"-107302697\" fdate=\"2014-05-26\" ftime=\"10:07:31\" pname=\"background_02.jpg.zip\"  psize=\"487292\" />\r\n\t<fileinfo fname=\"background_03.jpg\" fdir=\"\\extrares\" fsize=\"435915\" fcrc=\"-100695203\" fdate=\"2014-05-26\" ftime=\"10:07:33\" pname=\"background_03.jpg.zip\"  psize=\"411860\" />\r\n\t<fileinfo fname=\"background_04.jpg\" fdir=\"\\extrares\" fsize=\"385840\" fcrc=\"-64002318\" fdate=\"2014-05-26\" ftime=\"10:07:33\" pname=\"background_04.jpg.zip\"  psize=\"363435\" />\r\n\t<fileinfo fname=\"background_05.jpg\" fdir=\"\\extrares\" fsize=\"412915\" fcrc=\"-120897557\" fdate=\"2014-05-26\" ftime=\"10:07:34\" pname=\"background_05.jpg.zip\"  psize=\"396332\" />\r\n\t<fileinfo fname=\"background_06.jpg\" fdir=\"\\extrares\" fsize=\"244694\" fcrc=\"-80131116\" fdate=\"2014-05-26\" ftime=\"10:07:35\" pname=\"background_06.jpg.zip\"  psize=\"218167\" />\r\n\t<fileinfo fname=\"background_07.jpg\" fdir=\"\\extrares\" fsize=\"343250\" fcrc=\"-25225756\" fdate=\"2014-05-26\" ftime=\"10:07:36\" pname=\"background_07.jpg.zip\"  psize=\"316411\" />\r\n\t<fileinfo fname=\"background_08.jpg\" fdir=\"\\extrares\" fsize=\"435915\" fcrc=\"-71003541\" fdate=\"2014-05-26\" ftime=\"10:07:36\" pname=\"background_08.jpg.zip\"  psize=\"421332\" />\r\n\t<fileinfo fname=\"background_09.jpg\" fdir=\"\\extrares\" fsize=\"634516\" fcrc=\"-74290838\" fdate=\"2014-05-26\" ftime=\"10:07:38\" pname=\"background_09.jpg.zip\"  psize=\"590198\" />\r\n\t<fileinfo fname=\"background_10.jpg\" fdir=\"\\extrares\" fsize=\"398970\" fcrc=\"-79515148\" fdate=\"2014-05-26\" ftime=\"10:07:39\" pname=\"background_10.jpg.zip\"  psize=\"382298\" />\r\n\t<fileinfo fname=\"background_11.jpg\" fdir=\"\\extrares\" fsize=\"391694\" fcrc=\"-43641345\" fdate=\"2014-05-26\" ftime=\"10:07:40\" pname=\"background_11.jpg.zip\"  psize=\"370274\" />\r\n\t<fileinfo fname=\"background_12.jpg\" fdir=\"\\extrares\" fsize=\"274178\" fcrc=\"-121907608\" fdate=\"2014-05-26\" ftime=\"10:07:40\" pname=\"background_12.jpg.zip\"  psize=\"251859\" />\r\n\t<fileinfo fname=\"background_13.jpg\" fdir=\"\\extrares\" fsize=\"379000\" fcrc=\"-43686749\" fdate=\"2014-05-26\" ftime=\"10:07:42\" pname=\"background_13.jpg.zip\"  psize=\"363851\" />\r\n\t<fileinfo fname=\"background_14.jpg\" fdir=\"\\extrares\" fsize=\"446462\" fcrc=\"-100168234\" fdate=\"2014-05-26\" ftime=\"10:07:42\" pname=\"background_14.jpg.zip\"  psize=\"425125\" />\r\n\t<fileinfo fname=\"icon_holes_n.tga\" fdir=\"extrares\\\" fsize=\"25276\" fcrc=\"-89964984\" fdate=\"2023-06-10\" ftime=\"00:22:24\" pname=\"icon_holes_n.tga.zip\"  psize=\"12342\" />\r\n\t<fileinfo fname=\"icon_holes_o.tga\" fdir=\"extrares\\\" fsize=\"25276\" fcrc=\"-84781945\" fdate=\"2023-06-10\" ftime=\"00:22:24\" pname=\"icon_holes_o.tga.zip\"  psize=\"11988\" />\r\n\t<fileinfo fname=\"loading_background.jpg\" fdir=\"\\extrares\" fsize=\"413121\" fcrc=\"-131674658\" fdate=\"2014-05-26\" ftime=\"10:07:52\" pname=\"loading_background.jpg.zip\"  psize=\"391403\" />\r\n\t<fileinfo fname=\"main_bg.jpg\" fdir=\"\\extrares\" fsize=\"308938\" fcrc=\"-98933725\" fdate=\"2014-05-26\" ftime=\"10:07:52\" pname=\"main_bg.jpg.zip\"  psize=\"281603\" />\r\n\t<fileinfo fname=\"main_bg_2.jpg\" fdir=\"\\extrares\" fsize=\"446105\" fcrc=\"-96775409\" fdate=\"2014-05-26\" ftime=\"10:07:54\" pname=\"main_bg_2.jpg.zip\"  psize=\"432364\" />\r\n\t<fileinfo fname=\"main_bg_3.jpg\" fdir=\"\\extrares\" fsize=\"426767\" fcrc=\"-14635932\" fdate=\"2014-05-26\" ftime=\"10:07:54\" pname=\"main_bg_3.jpg.zip\"  psize=\"396596\" />\r\n\t<fileinfo fname=\"main_bg_4.jpg\" fdir=\"\\extrares\" fsize=\"333649\" fcrc=\"-8379088\" fdate=\"2014-05-26\" ftime=\"10:07:56\" pname=\"main_bg_4.jpg.zip\"  psize=\"304827\" />\r\n\t<fileinfo fname=\"main_bg_5.jpg\" fdir=\"\\extrares\" fsize=\"446202\" fcrc=\"-51244672\" fdate=\"2014-05-26\" ftime=\"10:07:56\" pname=\"main_bg_5.jpg.zip\"  psize=\"416224\" />\r\n\t<fileinfo fname=\"main_bg_7.jpg\" fdir=\"\\extrares\" fsize=\"209533\" fcrc=\"-10295200\" fdate=\"2014-05-26\" ftime=\"10:07:57\" pname=\"main_bg_7.jpg.zip\"  psize=\"185353\" />\r\n\t<fileinfo fname=\"main_bg_8.jpg\" fdir=\"\\extrares\" fsize=\"453569\" fcrc=\"-79218340\" fdate=\"2014-05-26\" ftime=\"10:07:58\" pname=\"main_bg_8.jpg.zip\"  psize=\"425867\" />\r\n\t<fileinfo fname=\"0npgg.erl\" fdir=\"\\GameGuard\" fsize=\"29049\" fcrc=\"-105754320\" fdate=\"2016-12-09\" ftime=\"23:12:17\" pname=\"0npgg.erl.zip\"  psize=\"29196\" />\r\n\t<fileinfo fname=\"0npgl.erl\" fdir=\"\\GameGuard\" fsize=\"32042\" fcrc=\"-117526424\" fdate=\"2016-12-09\" ftime=\"23:15:32\" pname=\"0npgl.erl.zip\"  psize=\"32189\" />\r\n\t<fileinfo fname=\"0npgm.erl\" fdir=\"GameGuard\\\" fsize=\"303546\" fcrc=\"-231003\" fdate=\"2016-11-30\" ftime=\"18:18:10\" pname=\"0npgm.erl.zip\"  psize=\"303546\" />\r\n\t<fileinfo fname=\"0npgmup.erl\" fdir=\"\\GameGuard\" fsize=\"21795\" fcrc=\"-22933911\" fdate=\"2016-12-09\" ftime=\"23:12:06\" pname=\"0npgmup.erl.zip\"  psize=\"21946\" />\r\n\t<fileinfo fname=\"0npsc.erl\" fdir=\"\\GameGuard\" fsize=\"39234\" fcrc=\"-46787442\" fdate=\"2016-12-09\" ftime=\"23:15:32\" pname=\"0npsc.erl.zip\"  psize=\"39386\" />\r\n\t<fileinfo fname=\"1npgg.erl\" fdir=\"\\GameGuard\" fsize=\"31552\" fcrc=\"-22336916\" fdate=\"2016-11-30\" ftime=\"18:08:10\" pname=\"1npgg.erl.zip\"  psize=\"31699\" />\r\n\t<fileinfo fname=\"1npgl.erl\" fdir=\"\\GameGuard\" fsize=\"34609\" fcrc=\"-71171640\" fdate=\"2016-11-30\" ftime=\"18:17:46\" pname=\"1npgl.erl.zip\"  psize=\"34761\" />\r\n\t<fileinfo fname=\"1npgl0.erl\" fdir=\"GameGuard\\\" fsize=\"34669\" fcrc=\"-2419111\" fdate=\"2016-11-30\" ftime=\"18:06:55\" pname=\"1npgl0.erl.zip\"  psize=\"34669\" />\r\n\t<fileinfo fname=\"1npgm.erl\" fdir=\"\\GameGuard\" fsize=\"303546\" fcrc=\"-231003\" fdate=\"2016-11-30\" ftime=\"18:18:10\" pname=\"1npgm.erl.zip\"  psize=\"303738\" />\r\n\t<fileinfo fname=\"1npgmup.erl\" fdir=\"\\GameGuard\" fsize=\"22437\" fcrc=\"-47164883\" fdate=\"2016-11-30\" ftime=\"18:08:00\" pname=\"1npgmup.erl.zip\"  psize=\"22588\" />\r\n\t<fileinfo fname=\"1npsc.erl\" fdir=\"\\GameGuard\" fsize=\"44204\" fcrc=\"-105689470\" fdate=\"2016-11-30\" ftime=\"18:17:46\" pname=\"1npsc.erl.zip\"  psize=\"44356\" />\r\n\t<fileinfo fname=\"event.erv\" fdir=\"\\GameGuard\" fsize=\"69632\" fcrc=\"-122426723\" fdate=\"2020-06-19\" ftime=\"18:03:32\" pname=\"event.erv.zip\"  psize=\"3835\" />\r\n\t<fileinfo fname=\"GameGuard.des\" fdir=\"\\GameGuard\" fsize=\"527176\" fcrc=\"-89502247\" fdate=\"2015-09-10\" ftime=\"07:30:00\" pname=\"GameGuard.des.zip\"  psize=\"518823\" />\r\n\t<fileinfo fname=\"GameGuard.ver\" fdir=\"\\GameGuard\" fsize=\"25\" fcrc=\"-17189482\" fdate=\"2020-06-19\" ftime=\"18:03:23\" pname=\"GameGuard.ver.zip\"  psize=\"177\" />\r\n\t<fileinfo fname=\"GameMon.des\" fdir=\"\\GameGuard\" fsize=\"3837224\" fcrc=\"-6879001\" fdate=\"2016-01-21\" ftime=\"12:16:00\" pname=\"GameMon.des.zip\"  psize=\"3791935\" />\r\n\t<fileinfo fname=\"GameMon64.des\" fdir=\"\\GameGuard\" fsize=\"3357520\" fcrc=\"-49070002\" fdate=\"2015-12-09\" ftime=\"15:25:00\" pname=\"GameMon64.des.zip\"  psize=\"3329062\" />\r\n\t<fileinfo fname=\"ggerror.des\" fdir=\"\\GameGuard\" fsize=\"191960\" fcrc=\"-79976523\" fdate=\"2015-09-07\" ftime=\"12:47:00\" pname=\"ggerror.des.zip\"  psize=\"165060\" />\r\n\t<fileinfo fname=\"ggexp.des\" fdir=\"\\GameGuard\" fsize=\"744376\" fcrc=\"-58683765\" fdate=\"2014-09-14\" ftime=\"07:47:22\" pname=\"ggexp.des.zip\"  psize=\"732120\" />\r\n\t<fileinfo fname=\"ggscan.des\" fdir=\"\\GameGuard\" fsize=\"1310416\" fcrc=\"-106969074\" fdate=\"2013-11-03\" ftime=\"13:06:42\" pname=\"ggscan.des.zip\"  psize=\"1297789\" />\r\n\t<fileinfo fname=\"npgg.erl\" fdir=\"\\GameGuard\" fsize=\"35938\" fcrc=\"-130608624\" fdate=\"2020-06-19\" ftime=\"18:03:26\" pname=\"npgg.erl.zip\"  psize=\"36088\" />\r\n\t<fileinfo fname=\"npgg9x.des\" fdir=\"\\GameGuard\" fsize=\"52392\" fcrc=\"-99964211\" fdate=\"2013-07-23\" ftime=\"21:43:24\" pname=\"npgg9x.des.zip\"  psize=\"47182\" />\r\n\t<fileinfo fname=\"npggNT.des\" fdir=\"\\GameGuard\" fsize=\"167688\" fcrc=\"-64202265\" fdate=\"2016-01-17\" ftime=\"11:46:00\" pname=\"npggNT.des.zip\"  psize=\"162420\" />\r\n\t<fileinfo fname=\"npggNT64.des\" fdir=\"\\GameGuard\" fsize=\"126728\" fcrc=\"-115095316\" fdate=\"2015-08-18\" ftime=\"04:22:00\" pname=\"npggNT64.des.zip\"  psize=\"121801\" />\r\n\t<fileinfo fname=\"npgl.erl\" fdir=\"\\GameGuard\" fsize=\"17188\" fcrc=\"-88666230\" fdate=\"2020-06-19\" ftime=\"18:03:35\" pname=\"npgl.erl.zip\"  psize=\"17333\" />\r\n\t<fileinfo fname=\"npgmup.des\" fdir=\"\\GameGuard\" fsize=\"234768\" fcrc=\"-77607328\" fdate=\"2016-11-10\" ftime=\"14:20:34\" pname=\"npgmup.des.zip\"  psize=\"124608\" />\r\n\t<fileinfo fname=\"npgmup.des.new\" fdir=\"GameGuard\\\" fsize=\"231176\" fcrc=\"-767197\" fdate=\"2016-05-09\" ftime=\"15:23:00\" pname=\"npgmup.des.new.zip\"  psize=\"126464\" />\r\n\t<fileinfo fname=\"npgmup.erl\" fdir=\"\\GameGuard\" fsize=\"18934\" fcrc=\"-117706301\" fdate=\"2020-06-19\" ftime=\"18:03:26\" pname=\"npgmup.erl.zip\"  psize=\"19083\" />\r\n\t<fileinfo fname=\"npsc.des\" fdir=\"\\GameGuard\" fsize=\"177544\" fcrc=\"-53258281\" fdate=\"2015-12-15\" ftime=\"16:37:00\" pname=\"npsc.des.zip\"  psize=\"172221\" />\r\n\t<fileinfo fname=\"npsc.erl\" fdir=\"GameGuard\\\" fsize=\"39234\" fcrc=\"-46787442\" fdate=\"2016-12-09\" ftime=\"23:15:32\" pname=\"npsc.erl.zip\"  psize=\"39234\" />\r\n\t<fileinfo fname=\"npsys.des\" fdir=\"\\GameGuard\" fsize=\"91996\" fcrc=\"-51644005\" fdate=\"2020-06-19\" ftime=\"20:55:27\" pname=\"npsys.des.zip\"  psize=\"13815\" />\r\n\t<fileinfo fname=\"PangyaUS.ini\" fdir=\"\\GameGuard\" fsize=\"955\" fcrc=\"-117119363\" fdate=\"2016-02-19\" ftime=\"12:41:12\" pname=\"PangyaUS.ini.zip\"  psize=\"1108\" />\r\n\t<fileinfo fname=\"Splash.jpg\" fdir=\"\\GameGuard\" fsize=\"42345\" fcrc=\"-93302372\" fdate=\"2013-07-23\" ftime=\"21:43:12\" pname=\"Splash.jpg.zip\"  psize=\"42146\" />\r\n\t<fileinfo fname=\"binkawin.asi\" fdir=\"\\mss\" fsize=\"56320\" fcrc=\"-13704566\" fdate=\"2013-10-22\" ftime=\"08:41:57\" pname=\"binkawin.asi.zip\"  psize=\"32364\" />\r\n\t<fileinfo fname=\"mssa3d.m3d\" fdir=\"\\mss\" fsize=\"72704\" fcrc=\"-32969048\" fdate=\"2009-05-02\" ftime=\"03:24:34\" pname=\"mssa3d.m3d.zip\"  psize=\"38613\" />\r\n\t<fileinfo fname=\"mssdolby.flt\" fdir=\"\\mss\" fsize=\"7680\" fcrc=\"-33883344\" fdate=\"2013-10-22\" ftime=\"08:41:56\" pname=\"mssdolby.flt.zip\"  psize=\"2998\" />\r\n\t<fileinfo fname=\"mssds3d.flt\" fdir=\"\\mss\" fsize=\"13312\" fcrc=\"-109768088\" fdate=\"2013-10-22\" ftime=\"08:41:59\" pname=\"mssds3d.flt.zip\"  psize=\"6557\" />\r\n\t<fileinfo fname=\"mssds3d.m3d\" fdir=\"\\mss\" fsize=\"56320\" fcrc=\"-32353220\" fdate=\"2009-05-02\" ftime=\"03:24:34\" pname=\"mssds3d.m3d.zip\"  psize=\"28811\" />\r\n\t<fileinfo fname=\"mssdsp.flt\" fdir=\"\\mss\" fsize=\"55296\" fcrc=\"-112210543\" fdate=\"2013-10-22\" ftime=\"08:41:56\" pname=\"mssdsp.flt.zip\"  psize=\"19359\" />\r\n\t<fileinfo fname=\"mssdx7.m3d\" fdir=\"\\mss\" fsize=\"65536\" fcrc=\"-18372934\" fdate=\"2009-05-02\" ftime=\"03:24:34\" pname=\"mssdx7.m3d.zip\"  psize=\"31000\" />\r\n\t<fileinfo fname=\"msseax.flt\" fdir=\"\\mss\" fsize=\"55808\" fcrc=\"-34224150\" fdate=\"2013-10-22\" ftime=\"08:41:59\" pname=\"msseax.flt.zip\"  psize=\"19568\" />\r\n\t<fileinfo fname=\"msseax.m3d\" fdir=\"\\mss\" fsize=\"143872\" fcrc=\"-103183360\" fdate=\"2009-05-02\" ftime=\"03:24:34\" pname=\"msseax.m3d.zip\"  psize=\"68499\" />\r\n\t<fileinfo fname=\"mssmp3.asi\" fdir=\"\\mss\" fsize=\"71680\" fcrc=\"-131938102\" fdate=\"2013-10-22\" ftime=\"08:42:19\" pname=\"mssmp3.asi.zip\"  psize=\"30437\" />\r\n\t<fileinfo fname=\"mssrsx.m3d\" fdir=\"\\mss\" fsize=\"372224\" fcrc=\"-121954384\" fdate=\"2009-05-02\" ftime=\"03:24:34\" pname=\"mssrsx.m3d.zip\"  psize=\"222715\" />\r\n\t<fileinfo fname=\"msssoft.m3d\" fdir=\"\\mss\" fsize=\"79360\" fcrc=\"-21705338\" fdate=\"2009-05-02\" ftime=\"03:24:34\" pname=\"msssoft.m3d.zip\"  psize=\"45611\" />\r\n\t<fileinfo fname=\"msssrs.flt\" fdir=\"\\mss\" fsize=\"13312\" fcrc=\"-62542884\" fdate=\"2013-10-22\" ftime=\"08:41:59\" pname=\"msssrs.flt.zip\"  psize=\"4999\" />\r\n\t<fileinfo fname=\"mssvoice.asi\" fdir=\"\\mss\" fsize=\"153088\" fcrc=\"-133834552\" fdate=\"2013-10-22\" ftime=\"08:41:59\" pname=\"mssvoice.asi.zip\"  psize=\"79436\" />\r\n\t<fileinfo fname=\"score_notice_0.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-116021960\" fdate=\"2011-10-18\" ftime=\"12:38:52\" pname=\"score_notice_0.tga.zip\"  psize=\"17166\" />\r\n\t<fileinfo fname=\"score_notice_1.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-108411046\" fdate=\"2011-10-18\" ftime=\"12:38:06\" pname=\"score_notice_1.tga.zip\"  psize=\"15922\" />\r\n\t<fileinfo fname=\"score_notice_10.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-119119388\" fdate=\"2012-12-19\" ftime=\"10:22:44\" pname=\"score_notice_10.tga.zip\"  psize=\"13955\" />\r\n\t<fileinfo fname=\"score_notice_11.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-116021960\" fdate=\"2011-10-18\" ftime=\"12:38:53\" pname=\"score_notice_11.tga.zip\"  psize=\"17168\" />\r\n\t<fileinfo fname=\"score_notice_12.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-108411046\" fdate=\"2011-10-18\" ftime=\"12:38:07\" pname=\"score_notice_12.tga.zip\"  psize=\"15924\" />\r\n\t<fileinfo fname=\"score_notice_14.tga\" fdir=\"\\score_ppl\" fsize=\"28074\" fcrc=\"-72068785\" fdate=\"2014-11-27\" ftime=\"08:12:02\" pname=\"score_notice_14.tga.zip\"  psize=\"15825\" />\r\n\t<fileinfo fname=\"score_notice_2.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-48289454\" fdate=\"2011-10-18\" ftime=\"12:38:42\" pname=\"score_notice_2.tga.zip\"  psize=\"13925\" />\r\n\t<fileinfo fname=\"score_notice_3.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-33905646\" fdate=\"2011-10-18\" ftime=\"12:39:04\" pname=\"score_notice_3.tga.zip\"  psize=\"15563\" />\r\n\t<fileinfo fname=\"score_notice_4.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-48946263\" fdate=\"2011-10-18\" ftime=\"12:39:28\" pname=\"score_notice_4.tga.zip\"  psize=\"14179\" />\r\n\t<fileinfo fname=\"score_notice_5.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-77593632\" fdate=\"2011-10-18\" ftime=\"12:38:34\" pname=\"score_notice_5.tga.zip\"  psize=\"16500\" />\r\n\t<fileinfo fname=\"score_notice_6.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-126675972\" fdate=\"2011-10-18\" ftime=\"12:38:24\" pname=\"score_notice_6.tga.zip\"  psize=\"15711\" />\r\n\t<fileinfo fname=\"score_notice_7.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-29839791\" fdate=\"2011-10-18\" ftime=\"12:38:16\" pname=\"score_notice_7.tga.zip\"  psize=\"16002\" />\r\n\t<fileinfo fname=\"score_notice_8.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-57346233\" fdate=\"2011-10-18\" ftime=\"12:39:18\" pname=\"score_notice_8.tga.zip\"  psize=\"15777\" />\r\n\t<fileinfo fname=\"score_notice_9.tga\" fdir=\"\\score_ppl\" fsize=\"28100\" fcrc=\"-89314360\" fdate=\"2011-10-18\" ftime=\"12:39:10\" pname=\"score_notice_9.tga.zip\"  psize=\"15534\" />\r\n\t<fileinfo fname=\"score_ppl.ini\" fdir=\"\\score_ppl\" fsize=\"243\" fcrc=\"-56703246\" fdate=\"2014-12-08\" ftime=\"12:39:43\" pname=\"score_ppl.ini.zip\"  psize=\"210\" />\r\n\t<fileinfo fname=\"bs_notice_popup00.jpg\" fdir=\"\" fsize=\"99137\" fcrc=\"-63990497\" fdate=\"2014-10-30\" ftime=\"07:00:41\" pname=\"bs_notice_popup00.jpg.zip\"  psize=\"98907\" />\r\n\t<fileinfo fname=\"bs_notice_popup01.jpg\" fdir=\"\" fsize=\"26245\" fcrc=\"-994444\" fdate=\"2006-11-02\" ftime=\"02:56:52\" pname=\"bs_notice_popup01.jpg.zip\"  psize=\"25116\" />\r\n\t<fileinfo fname=\"bs_ppl.ini\" fdir=\"\" fsize=\"44\" fcrc=\"-33193750\" fdate=\"2010-10-06\" ftime=\"02:49:38\" pname=\"bs_ppl.ini.zip\"  psize=\"175\" />\r\n\t<fileinfo fname=\"d3dx9_31.dll\" fdir=\"\" fsize=\"2414360\" fcrc=\"-59733014\" fdate=\"2006-09-28\" ftime=\"07:05:20\" pname=\"d3dx9_31.dll.zip\"  psize=\"1114668\" />\r\n\t<fileinfo fname=\"dbghelp.dll\" fdir=\"\" fsize=\"1017856\" fcrc=\"-14084487\" fdate=\"2009-05-02\" ftime=\"01:02:26\" pname=\"dbghelp.dll.zip\"  psize=\"447494\" />\r\n\t<fileinfo fname=\"debug.dll\" fdir=\"\" fsize=\"394240\" fcrc=\"-44101278\" fdate=\"2021-01-23\" ftime=\"15:13:38\" pname=\"debug.dll.zip\"  psize=\"96989\" />\r\n\t<fileinfo fname=\"Disco_Notice.jpg\" fdir=\"\" fsize=\"107162\" fcrc=\"-19597618\" fdate=\"2010-02-22\" ftime=\"12:44:12\" pname=\"Disco_Notice.jpg.zip\"  psize=\"87310\" />\r\n\t<fileinfo fname=\"english.dat\" fdir=\"\" fsize=\"138632\" fcrc=\"-93927286\" fdate=\"2015-01-14\" ftime=\"03:07:41\" pname=\"english.dat.zip\"  psize=\"44274\" />\r\n\t<fileinfo fname=\"event01.jpg\" fdir=\"\" fsize=\"290418\" fcrc=\"-1609984\" fdate=\"2009-05-02\" ftime=\"01:02:26\" pname=\"event01.jpg.zip\"  psize=\"271071\" />\r\n\t<fileinfo fname=\"event02.jpg\" fdir=\"\" fsize=\"221494\" fcrc=\"-124113566\" fdate=\"2010-02-11\" ftime=\"09:56:36\" pname=\"event02.jpg.zip\"  psize=\"220434\" />\r\n\t<fileinfo fname=\"event_notice.ini\" fdir=\"\" fsize=\"16\" fcrc=\"-31482765\" fdate=\"2010-02-23\" ftime=\"06:29:54\" pname=\"event_notice.ini.zip\"  psize=\"174\" />\r\n\t<fileinfo fname=\"GameGuard.des\" fdir=\"\" fsize=\"527176\" fcrc=\"-89502247\" fdate=\"2015-09-10\" ftime=\"22:30:00\" pname=\"GameGuard.des.zip\"  psize=\"518823\" />\r\n\t<fileinfo fname=\"ijl15.dll\" fdir=\"\" fsize=\"39936\" fcrc=\"-106088595\" fdate=\"2020-06-19\" ftime=\"19:25:30\" pname=\"ijl15.dll.zip\"  psize=\"11459\" />\r\n\t<fileinfo fname=\"korea.dat\" fdir=\"\" fsize=\"132816\" fcrc=\"-37431574\" fdate=\"2015-01-14\" ftime=\"03:07:41\" pname=\"korea.dat.zip\"  psize=\"47129\" />\r\n\t<fileinfo fname=\"LoadingRes.dll\" fdir=\"\" fsize=\"3202560\" fcrc=\"-123680795\" fdate=\"2012-06-07\" ftime=\"02:38:38\" pname=\"LoadingRes.dll.zip\"  psize=\"1081511\" />\r\n\t<fileinfo fname=\"Mss32.dll\" fdir=\"\" fsize=\"450048\" fcrc=\"-119624108\" fdate=\"2013-10-22\" ftime=\"08:41:58\" pname=\"Mss32.dll.zip\"  psize=\"224276\" />\r\n\t<fileinfo fname=\"msvcp100.dll\" fdir=\"\" fsize=\"421200\" fcrc=\"-99492291\" fdate=\"2011-06-10\" ftime=\"16:58:52\" pname=\"msvcp100.dll.zip\"  psize=\"133870\" />\r\n\t<fileinfo fname=\"msvcp110.dll\" fdir=\"\" fsize=\"372736\" fcrc=\"-54093955\" fdate=\"2009-05-02\" ftime=\"01:02:26\" pname=\"msvcp110.dll.zip\"  psize=\"141371\" />\r\n\t<fileinfo fname=\"msvcp60.dll\" fdir=\"\" fsize=\"401462\" fcrc=\"-9781742\" fdate=\"2009-05-01\" ftime=\"23:57:22\" pname=\"msvcp60.dll.zip\"  psize=\"116244\" />\r\n\t<fileinfo fname=\"msvcp71.dll\" fdir=\"\" fsize=\"499712\" fcrc=\"-111616482\" fdate=\"2003-03-18\" ftime=\"19:14:50\" pname=\"msvcp71.dll.zip\"  psize=\"131454\" />\r\n\t<fileinfo fname=\"msvcr100.dll\" fdir=\"\" fsize=\"773968\" fcrc=\"-113091680\" fdate=\"2011-06-10\" ftime=\"16:58:52\" pname=\"msvcr100.dll.zip\"  psize=\"413415\" />\r\n\t<fileinfo fname=\"msvcr71.dll\" fdir=\"\" fsize=\"348160\" fcrc=\"-38596652\" fdate=\"2005-07-04\" ftime=\"00:57:10\" pname=\"msvcr71.dll.zip\"  psize=\"181282\" />\r\n\t<fileinfo fname=\"notice_popup.ini\" fdir=\"\" fsize=\"158\" fcrc=\"-24135109\" fdate=\"2012-06-14\" ftime=\"05:32:20\" pname=\"notice_popup.ini.zip\"  psize=\"202\" />\r\n\t<fileinfo fname=\"notice_popup00.jpg\" fdir=\"\" fsize=\"101735\" fcrc=\"-71298760\" fdate=\"2012-06-13\" ftime=\"13:07:36\" pname=\"notice_popup00.jpg.zip\"  psize=\"100620\" />\r\n\t<fileinfo fname=\"notice_popup01.jpg\" fdir=\"\" fsize=\"101658\" fcrc=\"-74346688\" fdate=\"2012-06-13\" ftime=\"13:07:36\" pname=\"notice_popup01.jpg.zip\"  psize=\"101390\" />\r\n\t<fileinfo fname=\"PangyaSetup.bmp\" fdir=\"\" fsize=\"1064056\" fcrc=\"-101319240\" fdate=\"2014-09-24\" ftime=\"07:31:37\" pname=\"PangyaSetup.bmp.zip\"  psize=\"445883\" />\r\n\t<fileinfo fname=\"PangyaUS.ini\" fdir=\"\" fsize=\"955\" fcrc=\"-117119363\" fdate=\"2016-02-19\" ftime=\"12:41:12\" pname=\"PangyaUS.ini.zip\"  psize=\"1108\" />\r\n\t<fileinfo fname=\"pcbang.ini\" fdir=\"\" fsize=\"18\" fcrc=\"-109006406\" fdate=\"2009-07-03\" ftime=\"11:13:50\" pname=\"pcbang.ini.zip\"  psize=\"164\" />\r\n\t<fileinfo fname=\"ppl.ini\" fdir=\"\" fsize=\"70\" fcrc=\"-80368986\" fdate=\"2009-05-02\" ftime=\"01:02:28\" pname=\"ppl.ini.zip\"  psize=\"198\" />\r\n\t<fileinfo fname=\"ProjectG.exe\" fdir=\"\" fsize=\"4893184\" fcrc=\"-79973773\" fdate=\"2020-06-19\" ftime=\"19:27:14\" pname=\"ProjectG.exe.zip\"  psize=\"4812397\" />\r\n\t<fileinfo fname=\"ProjectG2.exe\" fdir=\"\" fsize=\"4893184\" fcrc=\"-72393209\" fdate=\"2022-07-02\" ftime=\"03:02:22\" pname=\"ProjectG2.exe.zip\"  psize=\"4816301\" />\r\n\t<fileinfo fname=\"projectg700gb+.pak\" fdir=\"\" fsize=\"1131135933\" fcrc=\"-56387215\" fdate=\"2013-01-28\" ftime=\"10:27:04\" pname=\"projectg700gb+.pak.zip\"  psize=\"985981577\" />\r\n\t<fileinfo fname=\"projectg701gb.pak\" fdir=\"\" fsize=\"34623194\" fcrc=\"-45295530\" fdate=\"2013-01-24\" ftime=\"02:00:22\" pname=\"projectg701gb.pak.zip\"  psize=\"31437290\" />\r\n\t<fileinfo fname=\"projectg702gb.pak\" fdir=\"\" fsize=\"15239796\" fcrc=\"-32552034\" fdate=\"2013-02-07\" ftime=\"01:46:34\" pname=\"projectg702gb.pak.zip\"  psize=\"14098907\" />\r\n\t<fileinfo fname=\"projectg703gb.pak\" fdir=\"\" fsize=\"24582797\" fcrc=\"-11010886\" fdate=\"2013-02-21\" ftime=\"05:56:50\" pname=\"projectg703gb.pak.zip\"  psize=\"22575882\" />\r\n\t<fileinfo fname=\"projectg704gb.pak\" fdir=\"\" fsize=\"21833094\" fcrc=\"-30021510\" fdate=\"2013-03-07\" ftime=\"01:31:16\" pname=\"projectg704gb.pak.zip\"  psize=\"19972316\" />\r\n\t<fileinfo fname=\"projectg705gb.pak\" fdir=\"\" fsize=\"14288536\" fcrc=\"-86621681\" fdate=\"2013-03-20\" ftime=\"10:55:34\" pname=\"projectg705gb.pak.zip\"  psize=\"7692071\" />\r\n\t<fileinfo fname=\"projectg706gb.pak\" fdir=\"\" fsize=\"2216493\" fcrc=\"-52921138\" fdate=\"2013-03-28\" ftime=\"03:54:08\" pname=\"projectg706gb.pak.zip\"  psize=\"1800175\" />\r\n\t<fileinfo fname=\"projectg707gb.pak\" fdir=\"\" fsize=\"1132935\" fcrc=\"-20185024\" fdate=\"2013-04-11\" ftime=\"08:37:16\" pname=\"projectg707gb.pak.zip\"  psize=\"1074347\" />\r\n\t<fileinfo fname=\"projectg708gb.pak\" fdir=\"\" fsize=\"2415875\" fcrc=\"-30112550\" fdate=\"2013-04-25\" ftime=\"02:23:16\" pname=\"projectg708gb.pak.zip\"  psize=\"2120467\" />\r\n\t<fileinfo fname=\"projectg709gb.pak\" fdir=\"\" fsize=\"6010788\" fcrc=\"-55852533\" fdate=\"2013-05-02\" ftime=\"09:50:48\" pname=\"projectg709gb.pak.zip\"  psize=\"5512101\" />\r\n\t<fileinfo fname=\"projectg710gb.pak\" fdir=\"\" fsize=\"28531214\" fcrc=\"-88919435\" fdate=\"2013-05-30\" ftime=\"03:20:22\" pname=\"projectg710gb.pak.zip\"  psize=\"26254425\" />\r\n\t<fileinfo fname=\"projectg711gb.pak\" fdir=\"\" fsize=\"2344996\" fcrc=\"-134074380\" fdate=\"2013-06-12\" ftime=\"05:18:10\" pname=\"projectg711gb.pak.zip\"  psize=\"2153911\" />\r\n\t<fileinfo fname=\"projectg712gb.pak\" fdir=\"\" fsize=\"87129682\" fcrc=\"-127881492\" fdate=\"2013-06-27\" ftime=\"01:19:58\" pname=\"projectg712gb.pak.zip\"  psize=\"80637596\" />\r\n\t<fileinfo fname=\"projectg713gb.pak\" fdir=\"\" fsize=\"20955121\" fcrc=\"-6176161\" fdate=\"2013-07-11\" ftime=\"06:06:08\" pname=\"projectg713gb.pak.zip\"  psize=\"19352200\" />\r\n\t<fileinfo fname=\"projectg714gb.pak\" fdir=\"\" fsize=\"2450071\" fcrc=\"-94212689\" fdate=\"2013-07-25\" ftime=\"01:21:34\" pname=\"projectg714gb.pak.zip\"  psize=\"2289379\" />\r\n\t<fileinfo fname=\"projectg715gb.pak\" fdir=\"\" fsize=\"15514337\" fcrc=\"-43024619\" fdate=\"2013-08-07\" ftime=\"01:40:58\" pname=\"projectg715gb.pak.zip\"  psize=\"14177159\" />\r\n\t<fileinfo fname=\"projectg716gb.pak\" fdir=\"\" fsize=\"47787218\" fcrc=\"-102941268\" fdate=\"2013-08-22\" ftime=\"05:55:44\" pname=\"projectg716gb.pak.zip\"  psize=\"42066913\" />\r\n\t<fileinfo fname=\"projectg717gb.pak\" fdir=\"\" fsize=\"11171541\" fcrc=\"-125631485\" fdate=\"2013-09-05\" ftime=\"04:54:24\" pname=\"projectg717gb.pak.zip\"  psize=\"9186692\" />\r\n\t<fileinfo fname=\"projectg718gb.pak\" fdir=\"\" fsize=\"12054184\" fcrc=\"-75942451\" fdate=\"2013-09-26\" ftime=\"03:06:56\" pname=\"projectg718gb.pak.zip\"  psize=\"11139663\" />\r\n\t<fileinfo fname=\"projectg719gb.pak\" fdir=\"\" fsize=\"70194759\" fcrc=\"-118444959\" fdate=\"2013-10-10\" ftime=\"04:30:10\" pname=\"projectg719gb.pak.zip\"  psize=\"65190418\" />\r\n\t<fileinfo fname=\"projectg720gb.pak\" fdir=\"\" fsize=\"10520232\" fcrc=\"-24845782\" fdate=\"2013-10-24\" ftime=\"07:16:34\" pname=\"projectg720gb.pak.zip\"  psize=\"9380752\" />\r\n\t<fileinfo fname=\"projectg721gb.pak\" fdir=\"\" fsize=\"65310059\" fcrc=\"-94047024\" fdate=\"2013-11-07\" ftime=\"03:01:08\" pname=\"projectg721gb.pak.zip\"  psize=\"60320687\" />\r\n\t<fileinfo fname=\"projectg722gb.pak\" fdir=\"\" fsize=\"6298820\" fcrc=\"-60799505\" fdate=\"2013-11-29\" ftime=\"10:53:12\" pname=\"projectg722gb.pak.zip\"  psize=\"6108470\" />\r\n\t<fileinfo fname=\"projectg723gb.pak\" fdir=\"\" fsize=\"49746620\" fcrc=\"-52356612\" fdate=\"2013-12-05\" ftime=\"07:18:18\" pname=\"projectg723gb.pak.zip\"  psize=\"46324869\" />\r\n\t<fileinfo fname=\"projectg724gb.pak\" fdir=\"\" fsize=\"22406189\" fcrc=\"-50852166\" fdate=\"2013-12-24\" ftime=\"04:33:30\" pname=\"projectg724gb.pak.zip\"  psize=\"20499096\" />\r\n\t<fileinfo fname=\"projectg725gb.pak\" fdir=\"\" fsize=\"19212780\" fcrc=\"-7284700\" fdate=\"2014-01-14\" ftime=\"07:31:02\" pname=\"projectg725gb.pak.zip\"  psize=\"17813389\" />\r\n\t<fileinfo fname=\"projectg726gb.pak\" fdir=\"\" fsize=\"98079282\" fcrc=\"-100204086\" fdate=\"2014-01-23\" ftime=\"04:26:30\" pname=\"projectg726gb.pak.zip\"  psize=\"90929132\" />\r\n\t<fileinfo fname=\"projectg727gb.pak\" fdir=\"\" fsize=\"4847135\" fcrc=\"-47870926\" fdate=\"2014-02-06\" ftime=\"06:42:54\" pname=\"projectg727gb.pak.zip\"  psize=\"4535313\" />\r\n\t<fileinfo fname=\"projectg728gb.pak\" fdir=\"\" fsize=\"18179679\" fcrc=\"-74735066\" fdate=\"2014-02-20\" ftime=\"05:37:20\" pname=\"projectg728gb.pak.zip\"  psize=\"16922383\" />\r\n\t<fileinfo fname=\"projectg729gb.pak\" fdir=\"\" fsize=\"3965534\" fcrc=\"-69185586\" fdate=\"2014-03-06\" ftime=\"07:24:54\" pname=\"projectg729gb.pak.zip\"  psize=\"3617890\" />\r\n\t<fileinfo fname=\"projectg730gb.pak\" fdir=\"\" fsize=\"1798075\" fcrc=\"-121769209\" fdate=\"2014-03-20\" ftime=\"06:56:30\" pname=\"projectg730gb.pak.zip\"  psize=\"1762329\" />\r\n\t<fileinfo fname=\"projectg801gb.pak\" fdir=\"\" fsize=\"146411728\" fcrc=\"-64907833\" fdate=\"2014-04-03\" ftime=\"01:37:24\" pname=\"projectg801gb.pak.zip\"  psize=\"133464912\" />\r\n\t<fileinfo fname=\"projectg802gb.pak\" fdir=\"\" fsize=\"2601359\" fcrc=\"-53452325\" fdate=\"2014-04-17\" ftime=\"08:08:30\" pname=\"projectg802gb.pak.zip\"  psize=\"2445079\" />\r\n\t<fileinfo fname=\"projectg803gb.pak\" fdir=\"\" fsize=\"2047572\" fcrc=\"-8203691\" fdate=\"2014-05-14\" ftime=\"00:50:52\" pname=\"projectg803gb.pak.zip\"  psize=\"1997922\" />\r\n\t<fileinfo fname=\"projectg804gb.pak\" fdir=\"\" fsize=\"2526902\" fcrc=\"-101675880\" fdate=\"2014-05-22\" ftime=\"05:44:02\" pname=\"projectg804gb.pak.zip\"  psize=\"2372991\" />\r\n\t<fileinfo fname=\"projectg805gb.pak\" fdir=\"\" fsize=\"52443759\" fcrc=\"-133364734\" fdate=\"2014-06-03\" ftime=\"06:19:26\" pname=\"projectg805gb.pak.zip\"  psize=\"48651363\" />\r\n\t<fileinfo fname=\"projectg806gb.pak\" fdir=\"\" fsize=\"1278977\" fcrc=\"-11084013\" fdate=\"2014-06-19\" ftime=\"04:06:42\" pname=\"projectg806gb.pak.zip\"  psize=\"1193341\" />\r\n\t<fileinfo fname=\"projectg807gb.pak\" fdir=\"\" fsize=\"3190594\" fcrc=\"-127189857\" fdate=\"2014-07-03\" ftime=\"01:11:06\" pname=\"projectg807gb.pak.zip\"  psize=\"3083014\" />\r\n\t<fileinfo fname=\"projectg808gb.pak\" fdir=\"\" fsize=\"1774352\" fcrc=\"-57445371\" fdate=\"2014-07-17\" ftime=\"01:18:35\" pname=\"projectg808gb.pak.zip\"  psize=\"1724599\" />\r\n\t<fileinfo fname=\"projectg809gb.pak\" fdir=\"\" fsize=\"10005375\" fcrc=\"-98244966\" fdate=\"2014-08-01\" ftime=\"06:50:56\" pname=\"projectg809gb.pak.zip\"  psize=\"9183853\" />\r\n\t<fileinfo fname=\"projectg810gb.pak\" fdir=\"\" fsize=\"1304699\" fcrc=\"-114452937\" fdate=\"2014-08-14\" ftime=\"05:24:25\" pname=\"projectg810gb.pak.zip\"  psize=\"1241148\" />\r\n\t<fileinfo fname=\"projectg811gb.pak\" fdir=\"\" fsize=\"29660331\" fcrc=\"-75768890\" fdate=\"2014-08-28\" ftime=\"06:32:03\" pname=\"projectg811gb.pak.zip\"  psize=\"27393487\" />\r\n\t<fileinfo fname=\"projectg812gb.pak\" fdir=\"\" fsize=\"1518454\" fcrc=\"-53340207\" fdate=\"2014-09-18\" ftime=\"03:34:58\" pname=\"projectg812gb.pak.zip\"  psize=\"1488918\" />\r\n\t<fileinfo fname=\"projectg813gb.pak\" fdir=\"\" fsize=\"73387108\" fcrc=\"-68244178\" fdate=\"2014-09-30\" ftime=\"12:26:46\" pname=\"projectg813gb.pak.zip\"  psize=\"65168184\" />\r\n\t<fileinfo fname=\"projectg814gb.pak\" fdir=\"\" fsize=\"20580095\" fcrc=\"-116659315\" fdate=\"2014-10-16\" ftime=\"01:08:03\" pname=\"projectg814gb.pak.zip\"  psize=\"15337643\" />\r\n\t<fileinfo fname=\"projectg815gb.pak\" fdir=\"\" fsize=\"34809385\" fcrc=\"-18854760\" fdate=\"2014-10-30\" ftime=\"11:49:32\" pname=\"projectg815gb.pak.zip\"  psize=\"31340347\" />\r\n\t<fileinfo fname=\"projectg816gb.pak\" fdir=\"\" fsize=\"24636412\" fcrc=\"-4198367\" fdate=\"2014-11-13\" ftime=\"07:01:08\" pname=\"projectg816gb.pak.zip\"  psize=\"22745543\" />\r\n\t<fileinfo fname=\"projectg817gb.pak\" fdir=\"\" fsize=\"7931648\" fcrc=\"-7790689\" fdate=\"2014-12-01\" ftime=\"08:52:09\" pname=\"projectg817gb.pak.zip\"  psize=\"6959495\" />\r\n\t<fileinfo fname=\"projectg818gb.pak\" fdir=\"\" fsize=\"32903785\" fcrc=\"-11516814\" fdate=\"2014-12-11\" ftime=\"02:58:34\" pname=\"projectg818gb.pak.zip\"  psize=\"30073170\" />\r\n\t<fileinfo fname=\"projectg819gb.pak\" fdir=\"\" fsize=\"33405471\" fcrc=\"-49467732\" fdate=\"2014-12-23\" ftime=\"07:23:16\" pname=\"projectg819gb.pak.zip\"  psize=\"30304615\" />\r\n\t<fileinfo fname=\"projectg820gb.pak\" fdir=\"\" fsize=\"151568780\" fcrc=\"-129271729\" fdate=\"2015-01-07\" ftime=\"06:46:03\" pname=\"projectg820gb.pak.zip\"  psize=\"139890649\" />\r\n\t<fileinfo fname=\"projectg821gb.pak\" fdir=\"\" fsize=\"19735208\" fcrc=\"-120764905\" fdate=\"2015-01-22\" ftime=\"02:21:19\" pname=\"projectg821gb.pak.zip\"  psize=\"16151663\" />\r\n\t<fileinfo fname=\"projectg822gb.pak\" fdir=\"\" fsize=\"9392462\" fcrc=\"-118562121\" fdate=\"2015-02-10\" ftime=\"10:01:09\" pname=\"projectg822gb.pak.zip\"  psize=\"8451394\" />\r\n\t<fileinfo fname=\"projectg823gb.pak\" fdir=\"\" fsize=\"2391647\" fcrc=\"-129405643\" fdate=\"2015-02-26\" ftime=\"04:41:37\" pname=\"projectg823gb.pak.zip\"  psize=\"2341277\" />\r\n\t<fileinfo fname=\"projectg823gba.pak\" fdir=\"\" fsize=\"1019962\" fcrc=\"-2796052\" fdate=\"2015-04-14\" ftime=\"09:13:22\" pname=\"projectg823gba.pak.zip\"  psize=\"1001060\" />\r\n\t<fileinfo fname=\"projectg823gbb.pak\" fdir=\"\" fsize=\"2021302\" fcrc=\"-83967863\" fdate=\"2015-04-28\" ftime=\"13:41:42\" pname=\"projectg823gbb.pak.zip\"  psize=\"1950380\" />\r\n\t<fileinfo fname=\"projectg824gb.pak\" fdir=\"\" fsize=\"50495361\" fcrc=\"-9905511\" fdate=\"2015-03-12\" ftime=\"03:25:02\" pname=\"projectg824gb.pak.zip\"  psize=\"46515146\" />\r\n\t<fileinfo fname=\"projectg824gba.pak\" fdir=\"\" fsize=\"3091947\" fcrc=\"-95036906\" fdate=\"2015-05-08\" ftime=\"14:38:32\" pname=\"projectg824gba.pak.zip\"  psize=\"2844752\" />\r\n\t<fileinfo fname=\"projectg824gbb.pak\" fdir=\"\" fsize=\"1895020\" fcrc=\"-122378472\" fdate=\"2015-06-23\" ftime=\"15:43:51\" pname=\"projectg824gbb.pak.zip\"  psize=\"1862519\" />\r\n\t<fileinfo fname=\"projectg824gbc.pak\" fdir=\"\" fsize=\"2129131\" fcrc=\"-133660086\" fdate=\"2015-07-27\" ftime=\"09:35:30\" pname=\"projectg824gbc.pak.zip\"  psize=\"2080890\" />\r\n\t<fileinfo fname=\"projectg825gb.pak\" fdir=\"\" fsize=\"6310417\" fcrc=\"-125799280\" fdate=\"2015-09-08\" ftime=\"09:53:12\" pname=\"projectg825gb.pak.zip\"  psize=\"5873579\" />\r\n\t<fileinfo fname=\"projectg825gba.pak\" fdir=\"\" fsize=\"2357164\" fcrc=\"-102906557\" fdate=\"2016-01-13\" ftime=\"03:56:14\" pname=\"projectg825gba.pak.zip\"  psize=\"2288483\" />\r\n\t<fileinfo fname=\"projectg825gbb.pak\" fdir=\"\" fsize=\"2035443\" fcrc=\"-93260093\" fdate=\"2016-02-17\" ftime=\"10:25:59\" pname=\"projectg825gbb.pak.zip\"  psize=\"1997757\" />\r\n\t<fileinfo fname=\"projectg825gbc.pak\" fdir=\"\" fsize=\"690312\" fcrc=\"-90408369\" fdate=\"2016-04-25\" ftime=\"08:57:49\" pname=\"projectg825gbc.pak.zip\"  psize=\"682707\" />\r\n\t<fileinfo fname=\"projectg826.pak\" fdir=\"\" fsize=\"21619910\" fcrc=\"-123029786\" fdate=\"2024-01-04\" ftime=\"20:00:54\" pname=\"projectg826.pak.zip\"  psize=\"18126838\" />\r\n\t<fileinfo fname=\"projectg826gba.pak\" fdir=\"\" fsize=\"1118672\" fcrc=\"-33380581\" fdate=\"2024-01-04\" ftime=\"20:01:53\" pname=\"projectg826gba.pak.zip\"  psize=\"812736\" />\r\n\t<fileinfo fname=\"projectg826gbb.pak\" fdir=\"\" fsize=\"19834030\" fcrc=\"-44921790\" fdate=\"2024-01-04\" ftime=\"22:47:53\" pname=\"projectg826gbb.pak.zip\"  psize=\"17246409\" />\r\n\t<fileinfo fname=\"projectg826gbc.pak\" fdir=\"\" fsize=\"1242763\" fcrc=\"-22575490\" fdate=\"2024-01-04\" ftime=\"22:46:42\" pname=\"projectg826gbc.pak.zip\"  psize=\"932512\" />\r\n\t<fileinfo fname=\"projectg827gb.pak\" fdir=\"\" fsize=\"3303003\" fcrc=\"-87752062\" fdate=\"2024-01-04\" ftime=\"22:52:47\" pname=\"projectg827gb.pak.zip\"  psize=\"2711225\" />\r\n\t<fileinfo fname=\"projectg827gba.pak\" fdir=\"\" fsize=\"48507141\" fcrc=\"-8963309\" fdate=\"2024-01-04\" ftime=\"22:55:11\" pname=\"projectg827gba.pak.zip\"  psize=\"37503810\" />\r\n\t<fileinfo fname=\"projectg827gbb.pak\" fdir=\"\" fsize=\"19365485\" fcrc=\"-62844412\" fdate=\"2024-01-04\" ftime=\"22:53:14\" pname=\"projectg827gbb.pak.zip\"  psize=\"16330740\" />\r\n\t<fileinfo fname=\"projectg827gbc.pak\" fdir=\"\" fsize=\"19834030\" fcrc=\"-44921790\" fdate=\"2024-01-04\" ftime=\"22:54:54\" pname=\"projectg827gbc.pak.zip\"  psize=\"17246409\" />\r\n\t<fileinfo fname=\"projectg828gb.pak\" fdir=\"\" fsize=\"17297356\" fcrc=\"-10079117\" fdate=\"2024-01-04\" ftime=\"23:48:22\" pname=\"projectg828gb.pak.zip\"  psize=\"14056362\" />\r\n\t<fileinfo fname=\"projectg828gba.pak\" fdir=\"\" fsize=\"45395831\" fcrc=\"-131186473\" fdate=\"2024-01-04\" ftime=\"20:03:22\" pname=\"projectg828gba.pak.zip\"  psize=\"37941918\" />\r\n\t<fileinfo fname=\"projectg828gbb.pak\" fdir=\"\" fsize=\"249338\" fcrc=\"-86897691\" fdate=\"2024-01-04\" ftime=\"20:03:41\" pname=\"projectg828gbb.pak.zip\"  psize=\"119657\" />\r\n\t<fileinfo fname=\"projectg828gbc.pak\" fdir=\"\" fsize=\"2127496\" fcrc=\"-86712209\" fdate=\"2024-01-04\" ftime=\"23:04:54\" pname=\"projectg828gbc.pak.zip\"  psize=\"1479206\" />\r\n\t<fileinfo fname=\"projectg829gb.pak\" fdir=\"\" fsize=\"21528104\" fcrc=\"-16557439\" fdate=\"2024-01-04\" ftime=\"23:06:50\" pname=\"projectg829gb.pak.zip\"  psize=\"18155396\" />\r\n\t<fileinfo fname=\"projectg829gba.pak\" fdir=\"\" fsize=\"72280714\" fcrc=\"-116667167\" fdate=\"2024-01-04\" ftime=\"23:07:51\" pname=\"projectg829gba.pak.zip\"  psize=\"54903467\" />\r\n\t<fileinfo fname=\"projectg829gbb.pak\" fdir=\"\" fsize=\"58990515\" fcrc=\"-63333555\" fdate=\"2024-01-04\" ftime=\"23:10:09\" pname=\"projectg829gbb.pak.zip\"  psize=\"46390183\" />\r\n\t<fileinfo fname=\"projectg829gbc.pak\" fdir=\"\" fsize=\"23197165\" fcrc=\"-22554532\" fdate=\"2024-01-04\" ftime=\"23:11:42\" pname=\"projectg829gbc.pak.zip\"  psize=\"18808496\" />\r\n\t<fileinfo fname=\"projectg830gb.pak\" fdir=\"\" fsize=\"830341\" fcrc=\"-125076345\" fdate=\"2024-01-04\" ftime=\"23:26:02\" pname=\"projectg830gb.pak.zip\"  psize=\"635109\" />\r\n\t<fileinfo fname=\"projectg830gba.pak\" fdir=\"\" fsize=\"16581536\" fcrc=\"-64261101\" fdate=\"2024-01-04\" ftime=\"23:27:23\" pname=\"projectg830gba.pak.zip\"  psize=\"13939251\" />\r\n\t<fileinfo fname=\"projectg830gbb.pak\" fdir=\"\" fsize=\"22722365\" fcrc=\"-107107121\" fdate=\"2024-01-04\" ftime=\"23:27:52\" pname=\"projectg830gbb.pak.zip\"  psize=\"18652038\" />\r\n\t<fileinfo fname=\"projectg830gbc.pak\" fdir=\"\" fsize=\"53551646\" fcrc=\"-3353997\" fdate=\"2024-01-04\" ftime=\"23:28:38\" pname=\"projectg830gbc.pak.zip\"  psize=\"37273833\" />\r\n\t<fileinfo fname=\"projectg831gb.pak\" fdir=\"\" fsize=\"45272818\" fcrc=\"-92240410\" fdate=\"2024-01-04\" ftime=\"23:28:18\" pname=\"projectg831gb.pak.zip\"  psize=\"33321619\" />\r\n\t<fileinfo fname=\"projectg831gba.pak\" fdir=\"\" fsize=\"2299545\" fcrc=\"-63457713\" fdate=\"2024-01-04\" ftime=\"23:32:11\" pname=\"projectg831gba.pak.zip\"  psize=\"1761958\" />\r\n\t<fileinfo fname=\"projectg831gbb.pak\" fdir=\"\" fsize=\"49716\" fcrc=\"-115788598\" fdate=\"2024-01-04\" ftime=\"20:06:42\" pname=\"projectg831gbb.pak.zip\"  psize=\"40973\" />\r\n\t<fileinfo fname=\"projectg831gbc.pak\" fdir=\"\" fsize=\"11759672\" fcrc=\"-101940002\" fdate=\"2024-01-04\" ftime=\"23:34:28\" pname=\"projectg831gbc.pak.zip\"  psize=\"9688297\" />\r\n\t<fileinfo fname=\"projectg832gb.pak\" fdir=\"\" fsize=\"62207563\" fcrc=\"-78295036\" fdate=\"2024-01-04\" ftime=\"23:34:13\" pname=\"projectg832gb.pak.zip\"  psize=\"54418617\" />\r\n\t<fileinfo fname=\"projectg832gba.pak\" fdir=\"\" fsize=\"36520989\" fcrc=\"-103817909\" fdate=\"2024-01-04\" ftime=\"23:48:34\" pname=\"projectg832gba.pak.zip\"  psize=\"34789285\" />\r\n\t<fileinfo fname=\"projectg832gbb.pak\" fdir=\"\" fsize=\"37404658\" fcrc=\"-59795525\" fdate=\"2024-01-04\" ftime=\"23:20:52\" pname=\"projectg832gbb.pak.zip\"  psize=\"27355252\" />\r\n\t<fileinfo fname=\"projectg832gbc.pak\" fdir=\"\" fsize=\"45268269\" fcrc=\"-53613057\" fdate=\"2024-01-04\" ftime=\"23:12:32\" pname=\"projectg832gbc.pak.zip\"  psize=\"37125210\" />\r\n\t<fileinfo fname=\"projectg832gbd.pak\" fdir=\"\" fsize=\"1240231\" fcrc=\"-8029457\" fdate=\"2024-01-04\" ftime=\"23:21:41\" pname=\"projectg832gbd.pak.zip\"  psize=\"911442\" />\r\n\t<fileinfo fname=\"projectg833gb.pak\" fdir=\"\" fsize=\"12663965\" fcrc=\"-19199913\" fdate=\"2024-01-04\" ftime=\"23:22:35\" pname=\"projectg833gb.pak.zip\"  psize=\"10774087\" />\r\n\t<fileinfo fname=\"projectg833gba.pak\" fdir=\"\" fsize=\"22844846\" fcrc=\"-16378289\" fdate=\"2024-01-04\" ftime=\"23:23:30\" pname=\"projectg833gba.pak.zip\"  psize=\"18449482\" />\r\n\t<fileinfo fname=\"projectg833gbb.pak\" fdir=\"\" fsize=\"38871283\" fcrc=\"-39106596\" fdate=\"2024-01-04\" ftime=\"23:24:33\" pname=\"projectg833gbb.pak.zip\"  psize=\"32982068\" />\r\n\t<fileinfo fname=\"projectg833gbc.pak\" fdir=\"\" fsize=\"1459786\" fcrc=\"-108694990\" fdate=\"2024-01-04\" ftime=\"20:09:55\" pname=\"projectg833gbc.pak.zip\"  psize=\"1375337\" />\r\n\t<fileinfo fname=\"ScreenCape.dll\" fdir=\"\" fsize=\"14336\" fcrc=\"-105887149\" fdate=\"2010-05-17\" ftime=\"05:51:06\" pname=\"ScreenCape.dll.zip\"  psize=\"7201\" />\r\n\t<fileinfo fname=\"UnogamesBR.exe\" fdir=\"\" fsize=\"750592\" fcrc=\"-35464117\" fdate=\"2018-12-19\" ftime=\"15:51:21\" pname=\"UnogamesBR.exe.zip\"  psize=\"505975\" />\r\n\t<fileinfo fname=\"update.exe\" fdir=\"\" fsize=\"3920896\" fcrc=\"-10823019\" fdate=\"2015-03-10\" ftime=\"09:15:01\" pname=\"update.exe.zip\"  psize=\"1248992\" />\r\n\t<fileinfo fname=\"wangreal.dll\" fdir=\"\" fsize=\"443392\" fcrc=\"-11975095\" fdate=\"2014-09-30\" ftime=\"01:27:27\" pname=\"wangreal.dll.zip\"  psize=\"141974\" />\r\n</updatefiles>";


            Document doc = getFiles(xmlString);
        }
        public bool EncryptFile()
        {
            return updateList.EncryptFile();
        }

        public bool DecryptFile()
        {
            return updateList.DecryptFile();
        }

        private string getFileFormatXml()
        {
            return updateList.getDocument().Replace("\0", "").ToStringXML();
        }
    }
    public static class XMLParserEx
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        /// <summary>
        /// atualiza o "localClient_filesInfos"  
        /// </summary>
        /// <param name="UpdateListInfo">arquivos carregados no updatelist</param>
        /// <param name="localClient_filesInfos">contem todos os arquivos da pasta "Pangya"</param>
        /// <returns></returns>
        public static Dictionary<string, FileItemEx> Update(this Dictionary<string, FileItemEx> localClient_filesInfos, Document UpdateListInfo)
        {
            foreach (var i in localClient_filesInfos.Keys)
            {
                if (UpdateListInfo.UpdateFiles.ContainsKey(i))
                {
                    localClient_filesInfos[i].Name = UpdateListInfo.UpdateFiles.getItem(i).Name;
                    localClient_filesInfos[i].Dir = UpdateListInfo.UpdateFiles.getItem(i).Dir;
                    localClient_filesInfos[i].Size = UpdateListInfo.UpdateFiles.getItem(i).Size;
                    localClient_filesInfos[i].Crc = UpdateListInfo.UpdateFiles.getItem(i).Crc;
                    localClient_filesInfos[i].Date = UpdateListInfo.UpdateFiles.getItem(i).Date;
                    localClient_filesInfos[i].Time = UpdateListInfo.UpdateFiles.getItem(i).Time;
                    localClient_filesInfos[i].PName = UpdateListInfo.UpdateFiles.getItem(i).PName;//compression
                    localClient_filesInfos[i].PSize = UpdateListInfo.UpdateFiles.getItem(i).PSize;//compression
                }
            }
            return localClient_filesInfos;
        }

        public static Dictionary<string, FileItemEx> Update(this Dictionary<string, FileItemEx> localClient_filesInfos, Dictionary<string, FileItemEx> UpdateListInfo)
        {
            foreach (var i in localClient_filesInfos.Keys)
            {
                if (UpdateListInfo.ContainsKey(i))
                {
                    localClient_filesInfos[i].Name = UpdateListInfo[i].Name;
                    localClient_filesInfos[i].Dir = UpdateListInfo[i].Dir;
                    localClient_filesInfos[i].Size = UpdateListInfo[i].Size;
                    localClient_filesInfos[i].Crc = UpdateListInfo[i].Crc;
                    localClient_filesInfos[i].Date = UpdateListInfo[i].Date;
                    localClient_filesInfos[i].Time = UpdateListInfo[i].Time;
                    localClient_filesInfos[i].PName = UpdateListInfo[i].PName;//compression
                    localClient_filesInfos[i].PSize = UpdateListInfo[i].PSize;//compression
                }
            }
            return localClient_filesInfos;
        }

        public static List<FileItem> GetFiles(this Dictionary<string, FileItemEx> pairs)
        {
            var collection = pairs.Values.ToList();
            var data = new List<FileItem>();
            foreach (FileItemEx Exitem in collection)
            {
                var item = (FileItem)Exitem;
                if (Exitem.Main)
                {
                    item.Dir = "";//corrigi um falha
                }
                data.Add(item);
            }
            return data;
        }

        public static string ToStringXML(this string xmlString)
        {
            var TempFile = Path.GetTempFileName();
            File.WriteAllText(TempFile, xmlString);
            List<string> xmlList = File.ReadAllLines(TempFile).ToList();
            xmlList.Insert(1, "<root>");
            xmlList.Insert(xmlList.Count, "</root>");
            File.WriteAllLines(TempFile, xmlList);
            xmlString = File.ReadAllText(TempFile);
            File.Delete(TempFile);
            return xmlString;
        }

        public static string RemoveToStringXML(this string xmlString)
        {
            var TempFile = Path.GetTempFileName();
            File.WriteAllText(TempFile, xmlString);
            List<string> xmlList = File.ReadAllLines(TempFile).ToList();
            xmlList.Remove("<root>");
            xmlList.Remove("</root>");
            File.WriteAllLines(TempFile, xmlList);
            xmlString = File.ReadAllText(TempFile);
            File.Delete(TempFile);
            return xmlString;
        }
        /// <summary>
        /// gera um data nova
        /// </summary>
        /// <param name="old">data antiga</param>
        /// <param name="hour">adiciona o tempo</param>
        /// <returns></returns>
        public static DateTime GetDate(this DateTime old, int hour)
        {
            var now = new DateTime(
                old.Year,
                old.Month,
                old.Day,
                old.Hour - hour,
                old.Minute,
                old.Second,
                old.Millisecond
            );
            return now;
        }

        public static bool Verify(this DateTime tempoA, DateTime tempoB)
        {
            // Obter a diferença entre os fusos horários (em TimeSpan)
            TimeSpan diferenca_fusos_horarios = tempoB - tempoA;

            // Ajustar o tempo em A para o mesmo fuso horário que B
           tempoA = tempoA + diferenca_fusos_horarios;
            var date = (tempoA.Month == tempoB.Month && tempoA.Day == tempoB.Day && tempoA.Year == tempoB.Year);
            var time = (tempoA.Hour == tempoB.Hour && tempoA.Minute == tempoB.Minute);
            return date && time;
        }
    }
}
