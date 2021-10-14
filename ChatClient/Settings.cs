using System.Net;
using Newtonsoft.Json;

namespace ChatClient
{
	internal class Settings
	{
		[JsonIgnore]
		public IPAddress _serverAddress = null;
		public int ServerPort = 0;
		public string Username = string.Empty;
		public string Password = string.Empty;

		public string ServerAddress { get { if (this._serverAddress == null) { return null; } else { return this._serverAddress.ToString(); } } set { _ = IPAddress.TryParse(value, out this._serverAddress); } }

		public bool Validate()
		{
			return this.ValidateServerAddress() && this.ValidateServerPort();
		}

		public bool ValidateServerAddress()
		{
			return this.ServerAddress != null;
		}

		public bool ValidateServerPort()
		{
			return this.ServerPort >= IPEndPoint.MinPort && this.ServerPort <= IPEndPoint.MaxPort;
		}
	}
}