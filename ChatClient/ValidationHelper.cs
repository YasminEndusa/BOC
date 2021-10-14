using System.Net;
using System.Text.RegularExpressions;

namespace ChatClient
{
	public static class ValidationHelper
	{
		public static bool TryParsePort(string s, out int port)
		{
			port = 0;
			int tmpPort;

			if (int.TryParse(s, out tmpPort))
			{
				if (tmpPort >= IPEndPoint.MinPort && tmpPort <= IPEndPoint.MaxPort)
				{
					port = tmpPort;
					return true;
				}
			}

			return false;
		}

		public static bool TryParseIPAddress(string s, out IPAddress address)
		{
			address = null;

			if (!Regex.IsMatch(s, "\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}"))
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(s))
			{
				return false;
			}

			IPAddress tmpAddress;

			if (IPAddress.TryParse(s, out tmpAddress))
			{
				address = tmpAddress;
				return true;
			}

			return false;
		}
	}
}