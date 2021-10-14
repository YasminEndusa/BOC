using System;
using System.IO;
using System.Security.Cryptography;

namespace ChatAPI
{
	public class RijndaelEncryption : IDisposable
	{
		public const string PREFIX_RIJNDAEL = "-rij-";

		private readonly RijndaelManaged provider = new RijndaelManaged();
		private byte[] Key { get { return this.provider.Key; } set { this.provider.Key = value; } }
		private byte[] IV { get { return this.provider.IV; } set { this.provider.IV = value; } }

		public RijndaelEncryption(string key = null, string vector = null)
		{
			if (!string.IsNullOrEmpty(key))
			{
				this.SetKeyFromString(key);
			}
			else
			{
				this.provider.GenerateKey();
			}

			if (!string.IsNullOrEmpty(vector))
			{
				this.SetVectorFromString(vector);
			}
			else
			{
				this.provider.GenerateIV();
			}
		}

		public string GetKeyString()
		{
			return Convert.ToBase64String(this.Key);
		}

		public void SetKeyFromString(string key)
		{
			this.Key = Convert.FromBase64String(key);
		}

		public string GetVectorString()
		{
			return Convert.ToBase64String(this.IV);
		}

		public void SetVectorFromString(string vector)
		{
			this.IV = Convert.FromBase64String(vector);
		}

		public string Encrypt(string text)
		{
			byte[] encryptedBytes;

			using (ICryptoTransform crypto = this.provider.CreateEncryptor(this.Key, this.IV))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, crypto, CryptoStreamMode.Write))
					{
						using (StreamWriter sw = new StreamWriter(cs))
						{
							sw.Write(text);
						}
					}

					encryptedBytes = ms.ToArray();
				}
			}

			string encrypted = Convert.ToBase64String(encryptedBytes);
			return encrypted;
		}

		public string Decrypt(string text)
		{
			byte[] encryptedBytes = Convert.FromBase64String(text);
			string decrypted;

			using (ICryptoTransform crypto = this.provider.CreateDecryptor(this.Key, this.IV))
			{
				using (MemoryStream ms = new MemoryStream(encryptedBytes))
				{
					using (CryptoStream cs = new CryptoStream(ms, crypto, CryptoStreamMode.Read))
					{
						using (StreamReader sr = new StreamReader(cs))
						{
							decrypted = sr.ReadToEnd();
						}
					}
				}
			}

			return decrypted;
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}
	}
}