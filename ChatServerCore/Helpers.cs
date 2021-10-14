using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatServerCore
{
	internal static class Helpers
	{
		internal static string ComputeHash(string input)
		{
			string output = string.Empty;

			using (SHA512Managed sha512 = new SHA512Managed())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] outputBytes = sha512.ComputeHash(inputBytes);
				output = BitConverter.ToString(outputBytes);
			}

			return output;
		}
	}
}