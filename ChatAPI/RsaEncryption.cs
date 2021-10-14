using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatAPI
{
	public class RsaEncryption : IDisposable
	{
		public const string PREFIX_UNENCRYPTED = "-raw-";
		public const string PREFIX_RSA = "-rsa-";

		private readonly RSACryptoServiceProvider provider = new RSACryptoServiceProvider(2048);

		public RsaEncryption()
		{

		}

		public RsaEncryption(string publicKey)
		{
			this.provider.FromXmlString(publicKey);
		}

		public string GetPublicKey()
		{
			return this.provider.ToXmlString(false);
		}

		public string Encrypt(string value)
		{
			byte[] valueBytes = Encoding.UTF8.GetBytes(value);
			byte[] encryptedBytes = this.provider.Encrypt(valueBytes, false);
			string encrypted = Convert.ToBase64String(encryptedBytes);

			return encrypted;
		}

		public string Decrypt(string encrypted)
		{
			byte[] encryptedBytes = Convert.FromBase64String(encrypted);
			byte[] decryptedBytes = this.provider.Decrypt(encryptedBytes, false);
			string decrypted = Encoding.UTF8.GetString(decryptedBytes);

			return decrypted;
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}
	}
}