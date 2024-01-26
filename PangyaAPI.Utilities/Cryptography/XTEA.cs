using PangyaAPI.Utilities;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace PangyaAPI.Utilities.Cryptography
{
    public class XTEA
    {

        static void EncryptXTEA(uint[] keys, ref uint dst0, ref uint dst1, ref uint src0, ref uint src1)
        {
            uint count = 4;
            uint padrao, padrao2;
            uint temp, var1, var2;
            uint valor = 0; // Delta
            const uint valor2 = 0x61C88647; // Soma

            dst0 = src0;
            dst1 = src1;

            padrao = dst0;

            while (count > 0)
            {
                var1 = (((dst1 >> 5) ^ (dst1 << 4)) + dst1);
                var2 = keys[(valor & 3)] + valor;
                temp = padrao + (var1 ^ var2);
                valor -= valor2;

                for (int i = 0; i < 3; i++)
                {
                    var1 = (((temp >> 5) ^ (temp << 4)) + temp);
                    var2 = keys[((valor >> 11) & 3)] + valor;
                    padrao2 = dst1 + (var1 ^ var2);
                    dst1 = padrao2; // p2

                    var1 = (((padrao2 >> 5) ^ (padrao2 << 4)) + padrao2);
                    var2 = keys[(valor & 3)] + valor;
                    temp = temp + (var1 ^ var2);
                    valor -= valor2;
                }

                dst0 = padrao = temp;
                var1 = (((temp >> 5) ^ (temp << 4)) + temp);
                var2 = keys[((valor >> 11) & 3)] + valor;
                temp = dst1 + (var1 ^ var2);
                dst1 = temp;

                count--;
            }
        }

        static void DecryptXTEA(uint[] keys, ref uint dst0, ref uint dst1, ref uint src0, ref uint src1)
        {
            uint count = 4;
            uint padrao, padrao2;
            uint temp, var1, var2;
            uint valor = 0xE3779B90; // Delta
            const uint valor2 = 0x61C88647; // Soma

            dst0 = src0;
            dst1 = src1;

            padrao = dst1;

            while (count > 0)
            {
                var1 = (((dst0 >> 5) ^ (dst0 << 4)) + dst0);
                var2 = keys[((valor >> 11) & 3)] + valor;
                temp = padrao - (var1 ^ var2);
                valor += valor2;

                for (int i = 0; i < 3; i++)
                {
                    var1 = (((temp >> 5) ^ (temp << 4)) + temp);
                    var2 = keys[(valor & 3)] + valor;
                    padrao2 = dst0 - (var1 ^ var2);
                    dst0 = padrao2; // p2

                    var1 = (((padrao2 >> 5) ^ (padrao2 << 4)) + padrao2);
                    var2 = keys[((valor >> 11) & 3)] + valor;
                    temp = temp - (var1 ^ var2);
                    valor += valor2;
                }

                dst1 = padrao = temp;
                var1 = (((temp >> 5) ^ (temp << 4)) + temp);
                var2 = keys[(valor & 3)] + valor;
                temp = dst0 - (var1 ^ var2);
                dst0 = temp;

                count--;
            }
        }

        public static void EncryptUpdatelist(uint[] Keys, ref byte[] dst, byte[] src, uint tamanho)
        {
            uint size = tamanho;
            uint count_encrypt = 0;

            uint size_int = (uint)(((size / sizeof(uint))) + ((size % 4 != 0) ? 1 : 0));

            uint[] Dst = new uint[size_int];
            uint[] Src = new uint[size_int];

            Array.Clear(Src, 0, (int)size_int);
            Array.Clear(Dst, 0, (int)size_int);

            Buffer.BlockCopy(src, 0, Src, 0, (int)tamanho);

            uint count = size / 8;

            while (count > 0)
            {
                EncryptXTEA(Keys, ref Dst[count_encrypt], ref Dst[count_encrypt + 1], ref Src[count_encrypt], ref Src[count_encrypt + 1]);

                count_encrypt += 2;
                count--;
            }

            Buffer.BlockCopy(Dst, 0, dst, 0, (int)size);
        }

        public static void DecryptUpdatelist(uint[] Keys, ref byte[] dst, byte[] src, uint tamanho)
        {
            uint size = tamanho;
            uint count_decrypt = 0;

            uint size_int = (uint)((size / sizeof(uint)) + ((size % 4 != 0) ? 1 : 0));

            uint[] Dst = new uint[size_int];
            uint[] Src = new uint[size_int];

            Buffer.BlockCopy(src, 0, Src, 0, (int)size);

            Array.Clear(Dst, 0, (int)size_int);

            uint count = size / 8;

            while (count > 0)
            {
                DecryptXTEA(Keys, ref Dst[count_decrypt], ref Dst[count_decrypt + 1], ref Src[count_decrypt], ref Src[count_decrypt + 1]);

                count_decrypt += 2;
                count--;
            }

            Buffer.BlockCopy(Dst, 0, dst, 0, (int)tamanho);
        }

        static void EncipherStream(uint[] key, Stream r, Stream w, out byte[] _result)
        {
            uint tamanho = (uint)r.Length;
            byte[] buffer = new byte[tamanho];
            var dst = new byte[tamanho];
            r.Read(buffer, 0, (int)tamanho);
            EncryptUpdatelist(key, ref dst, buffer, tamanho);
            w.Write(dst, 0, dst.Length);
            _result = ((MemoryStream)w).ToArray();
            r.Close();
            w.Close();
        }

        static void DecipherStream(uint[] key, Stream r, Stream w, out byte[] _result)
        {
            uint tamanho = (uint)r.Length;
            byte[] buffer = new byte[tamanho];
            r.Read(buffer, 0, (int)tamanho);
            var dst = new byte[tamanho];
            DecryptUpdatelist(key, ref dst, buffer, tamanho);
            //tamanho = (uint)(dst.ToList().LastIndexOf(10) + 1);  //ultimo byte deve ser "10" "0A", remove todos os bytes nulos
            w.Write(dst, 0, (int)tamanho);
            _result = ((MemoryStream)w).ToArray();
            r.Close();
            w.Close();
        }

        public static void EncipherStreamPadNull(uint[] key, Stream r, out byte[] _result)
        {
            MemoryStream w = new MemoryStream();
            EncipherStream(key, r, w, out _result);
        }

        public static void DecipherStreamTrimNull(uint[] key, Stream r, out byte[] _result)
        {
            MemoryStream w = new MemoryStream();
            DecipherStream(key, r, w, out _result);
        }

        public static void EncipherStreamPadNull(uint[] key, byte[] data_r, out byte[] _result)
        {
            MemoryStream w = new MemoryStream();
            EncipherStream(key, new MemoryStream(data_r), w, out _result);
        }

        public static void DecipherStreamTrimNull(uint[] key, byte[] data_r, out byte[] _result)
        {
            MemoryStream w = new MemoryStream();
            DecipherStream(key, new MemoryStream(data_r), w, out _result);
        }
    }
}
