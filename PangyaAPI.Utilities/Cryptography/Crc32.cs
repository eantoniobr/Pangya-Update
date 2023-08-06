using System;

namespace PangyaAPI.Utilities.Cryptography
{
    public class Crc32
    {
        public class Table
        {
            public uint[] TableData { get; } = new uint[256];
        }

        private static Table SimpleMakeTable(uint poly)
        {
            Table table = new Table();
            SimplePopulateTable(poly, table);
            return table;
        }

        private static void SimplePopulateTable(uint poly, Table table)
        {
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) == 1)
                    {
                        crc = (crc >> 1) ^ poly;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
                table.TableData[i] = crc;
            }
        }

        private readonly uint[] table;
        private static readonly Table IEEETable = SimpleMakeTable(0xedb88320); // CRC32 padrão IEEE
        public Crc32(Table customTable = null)
        {
            table = customTable?.TableData ?? IEEETable.TableData;
        }
        public Crc32()
        {
            table = SimpleMakeTable(0x04c11db7).TableData;//pangya crc32
        }
        public uint ComputeChecksum(byte[] bytes)
        {
            uint crc = 0xffffffff;
            foreach (byte b in bytes)
            {
                byte index = (byte)(((crc) & 0xff) ^ b);
                crc = (crc >> 8) ^ table[index];
            }
            return ~crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            return BitConverter.GetBytes(ComputeChecksum(bytes));
        }

        public uint CalculateHash(byte[] buffer, int start, int size, Table customTable = null)
        {
            Crc32 crc32 = new Crc32(customTable);
            byte[] data = new byte[size];
            Array.Copy(buffer, start, data, 0, size);
            return crc32.ComputeChecksum(data);
        }

        public uint CalculateHash(byte[] buffer, Table customTable = null)
        {
            customTable = SimpleMakeTable(0x04c11db7);
            return CalculateHash(buffer, 0, buffer.Length, customTable);
        }
    }
}
