using System.Text;

namespace PangyaUpdate.Tools
{
	public class Crypto
	{
		public class XorStr
		{
			private string str;

			private byte[] data;

			public XorStr(byte key, string str)
			{
				this.str = str;
				data = Encrypt(key, str);
			}

			public XorStr(byte key, byte[] data)
			{
				this.data = data;
				str = Decrypt(key, data);
			}

			public static byte[] Encrypt(byte key, string str)
			{
				byte[] array = new byte[str.Length];
				int num = 0;
				int num2 = key;
				while (num < str.Length)
				{
					array[num] = (byte)(str[num] ^ table_[num2 % table_.Length]);
					num++;
					num2++;
				}
				return array;
			}

			public static string Decrypt(byte key, byte[] data)
			{
				StringBuilder stringBuilder = new StringBuilder(data.Length);
				int num = 0;
				int num2 = key;
				while (num < data.Length)
				{
					stringBuilder.Append((char)(data[num] ^ table_[num2 % table_.Length]));
					num++;
					num2++;
				}
				return stringBuilder.ToString();
			}

			public override string ToString()
			{
				return str;
			}

			public byte[] ToBytes()
			{
				return data;
			}
		}

		public static class XTEA
		{
			public static uint[] PangyaUSKey = new uint[4] { 66455465u, 57629246u, 17826484u, 78315754u };

			public static uint[] Encipher(int rounds, uint[] data, uint[] key)
			{
				uint num = 1640531527u;
				uint num2 = 0u;
				uint num3 = data[0];
				uint num4 = data[1];
				for (int i = 0; i < rounds; i++)
				{
					num3 += (((num4 << 4) ^ (num4 >> 5)) + num4) ^ (num2 + key[num2 & 3]);
					num2 -= num;
					num4 += (((num3 << 4) ^ (num3 >> 5)) + num3) ^ (num2 + key[(num2 >> 11) & 3]);
				}
				data[0] = num3;
				data[1] = num4;
				return data;
			}

			public static uint[] Decipher(int rounds, uint[] data, uint[] key)
			{
				uint num = 1640531527u;
				uint num2 = 3816266640u;
				uint num3 = data[0];
				uint num4 = data[1];
				for (int i = 0; i < rounds; i++)
				{
					num4 -= (((num3 << 4) ^ (num3 >> 5)) + num3) ^ (num2 + key[(num2 >> 11) & 3]);
					num2 += num;
					num3 -= (((num4 << 4) ^ (num4 >> 5)) + num4) ^ (num2 + key[num2 & 3]);
				}
				data[0] = num3;
				data[1] = num4;
				return data;
			}
		}

		private static byte[] table_ = new byte[256]
		{
		61, 105, 142, 48, 250, 88, 214, 165, 62, 38,
		151, 93, 174, 59, 99, 183, 115, 212, 157, 145,
		8, 115, 85, 54, 37, 15, 249, 189, 240, 222,
		175, 66, 166, 106, 144, 132, 68, 131, 90, 138,
		81, 143, 196, 199, 125, 184, 163, 49, 117, 204,
		180, 101, 58, 39, 228, 217, 87, 229, 194, 30,
		58, 38, 229, 16, 133, 222, 100, 167, 183, 223,
		1, 37, 24, 78, 33, 99, 85, 118, 215, 202,
		216, 58, 102, 114, 10, 82, 103, 208, 251, 17,
		222, 119, 231, 207, 146, 15, 191, 147, 215, 195,
		30, 113, 5, 83, 178, 21, 251, 69, 234, 206,
		199, 225, 171, 104, 17, 225, 38, 219, 38, 247,
		106, 85, 17, 236, 140, 216, 169, 53, 225, 17,
		177, 213, 245, 36, 10, 240, 186, 134, 205, 120,
		53, 147, 93, 183, 2, 43, 11, 63, 226, 24,
		149, 45, 175, 138, 254, 145, 206, 245, 159, 106,
		254, 27, 58, 206, 44, 118, 68, 207, 41, 128,
		177, 220, 111, 186, 65, 219, 43, 242, 28, 83,
		146, 114, 23, 80, 179, 166, 15, 217, 115, 238,
		107, 116, 225, 200, 81, 210, 169, 158, 113, 21,
		149, 63, 96, 204, 161, 39, 45, 158, 69, 117,
		119, 192, 171, 4, 5, 160, 199, 21, 253, 109,
		107, 215, 168, 23, 203, 117, 151, 215, 80, 124,
		23, 47, 157, 220, 22, 233, 215, 13, 108, 28,
		185, 191, 240, 133, 23, 8, 224, 112, 126, 238,
		229, 29, 49, 132, 122, 4
		};

		public static void Crypt(byte key, ref byte[] buffer)
		{
			int num = 0;
			int num2 = key;
			while (num < buffer.Length)
			{
				buffer[num] ^= table_[num2 % table_.Length];
				num++;
				num2++;
			}
		}
	}
}