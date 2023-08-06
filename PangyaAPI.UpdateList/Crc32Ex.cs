using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PangyaAPI.UpdateList
{
    public class Crc32Ex : PangyaAPI.Utilities.Cryptography.Crc32
    {
        //funciona
        public int NativeCalculateHash32WithProgress(Stream stream, int size, string name, long sum_size, ref long sum_progress)
        {
            int num = int.MinValue;
            byte[] array = new byte[size];
            int num2;

            if ((num2 = stream.Read(array, 0, array.Length)) > 0)
            {
                num = BitConverter.ToInt32(ComputeChecksumBytes(array), 0);
                Patch.Progress(Patch.ProgressStatus.VERIFYING, name, stream.Position, stream.Length, (int)((double)(sum_progress += num2) / (double)sum_size * 1000.0));
            }

            return num;
        }

        public int NativeCalculateHash32(byte[] stream)
        {
            int num = int.MinValue;
            if (stream.Length > 0)
            {
                num = BitConverter.ToInt32(ComputeChecksumBytes(stream), 0);
            }
            return num;
        }
        public int NativeCalculateHash32(Stream stream)
        {
            int num = int.MinValue;
            byte[] array;
            array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            if (stream.Length > 0)
            {
                num = BitConverter.ToInt32(ComputeChecksumBytes(array), 0);
            }
            return num;
        }
        public int NativeCalculateHash32(string path)
        {
            byte[] byteArray = File.ReadAllBytes(path);
            var crc = CalculateHash(byteArray);
            return (int)crc;
        }
        public static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            uint num;
            num = seed;
            for (int i = start; i < size; i++)
            {
                num = (num >> 8) ^ table[buffer[i] ^ (num & 0xFF)];
            }
            return num;
        }

        private byte[] UInt32ToBigEndianBytes(uint x)
        {
            return new byte[4]
            {
            (byte)((x >> 24) & 0xFFu),
            (byte)((x >> 16) & 0xFFu),
            (byte)((x >> 8) & 0xFFu),
            (byte)(x & 0xFFu)
            };
        }
        public unsafe uint NativeCalculateHash32WithProgress(Stream stream, string name, long sum_size, ref long sum_progress)
        {
            uint num;
            num = uint.MaxValue;
            byte[] array;
            array = new byte[1024000];
            int num2;
            while ((num2 = stream.Read(array, 0, array.Length)) > 0)
            {
                Patch.Progress(Patch.ProgressStatus.VERIFYING, name, stream.Position, stream.Length, (int)((double)(sum_progress += num2) / (double)sum_size * 1000.0));
                fixed (byte* ptr = &array[0])
                {
                    for (int i = 0; i < num2 - 32; i += 32)
                    {
                        num = num ^ *(uint*)(ptr + i) ^ *(uint*)(ptr + i + 4) ^ *(uint*)(ptr + i + 8) ^ *(uint*)(ptr + i + 12) ^ *(uint*)(ptr + i + 16) ^ *(uint*)(ptr + i + 20) ^ *(uint*)(ptr + i + 24) ^ *(uint*)(ptr + i + 28);
                    }
                }
                num = BitConverter.ToUInt32(ComputeChecksumBytes(array),0);
            }
            return num;
        }


    }
}
