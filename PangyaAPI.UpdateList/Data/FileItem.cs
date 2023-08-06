using PangyaAPI.Utilities.Cryptography;
using PangyaAPI.ZIP.Crc;
using System;
using System.Data.HashFunction.CRC;
using System.IO;
using System.Security.Cryptography;

namespace PangyaAPI.UpdateList.Data
{
    public class FileItem
    {
        public int FileID { get; set; }
        //fname
        public string Name { get; set; }
        //fdir
        public string dir { get; set; }
        //fsize
        public long Size { get; set; }
        //fcrc32
        public int crc { get; set; }
        public string Date { get; set; }
        public string time { get; set; }
        public string PackedName { get; set; }
        public string PackedSize { get; set; }

        public string FullName { get; set; }

        public DateTime FullDate { get; set; }
        public bool ToUpdate { get; set; }
        public int _Crc
        {
            get
            {
                var fileName = @GetLocal();
                try
                {
                    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                    {
                        var crc32 = new PangyaAPI.ZIP.Crc.CRC32();
                        return crc32.GetCrc32(fs);
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        public string GetLocal()
        {
            if (IsPath())
            {
                return Directory.GetCurrentDirectory() + "\\" + FullName;
            }

            return Directory.GetCurrentDirectory() + "\\" + Name;
        }
        public bool IsPath()
        {
            return !string.IsNullOrEmpty(this.dir) || !string.IsNullOrEmpty(this.FullName);
        }
        public bool CheckUpdate()
        {
            if (Name.Contains(".err"))
            {
                return false;
            }
            var fileName = @GetLocal();
            try
            {
                int hash = 0;
                hash = new Crc32Ex().NativeCalculateHash32(fileName);

                if (hash != crc)//crc não consegue calcular valor baixo?
                {
                    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                    {
                        var fileSize = fs.Length;
                        return !(Size == fileSize);//aqui faço uma proteção entao
                    }                   
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private const int ChunkSize = 1024 * 1024; // Tamanho de cada chunk em bytes (1 MB)

        public bool IsFileChanged(string filePath, string savedHash)
        {
            string currentHash = ComputeFileHashWithChunks(filePath);

            return !string.Equals(currentHash, savedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeFileHashWithChunks(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
            {
                using (var sha256 = SHA256.Create())
                {
                    long totalLength = fileStream.Length;
                    byte[] buffer = new byte[ChunkSize];
                    byte[] combinedHash;

                    for (long bytesRead = 0; bytesRead < totalLength; bytesRead += ChunkSize)
                    {
                        int chunkSize = (int)Math.Min(ChunkSize, totalLength - bytesRead);
                        fileStream.Read(buffer, 0, chunkSize);
                        sha256.TransformBlock(buffer, 0, chunkSize, null, 0);
                    }

                    sha256.TransformFinalBlock(new byte[0], 0, 0);
                    combinedHash = sha256.Hash;

                    return BitConverter.ToString(combinedHash).Replace("-", "").ToLower();
                }
            }
        }
    }
}