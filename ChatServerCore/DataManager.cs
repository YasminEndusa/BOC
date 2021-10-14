using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ChatServerCore
{
	internal static class DataManager
	{
		internal static Dictionary<string, string> Users = new Dictionary<string, string>();
		private static FileInfo UsersFile;

		static DataManager()
		{
			InitFileSystem();
			LoadUsers();
		}

		private static void InitFileSystem()
		{
			Assembly entry = Assembly.GetExecutingAssembly();
			string loc = entry.Location;
			FileInfo fileInfo = new FileInfo(loc);
			DirectoryInfo directoryInfo = fileInfo.Directory;

			UsersFile = new FileInfo(Path.Combine(directoryInfo.FullName, "users.json"));
		}

		private static void LoadUsers()
		{
			if (UsersFile.Exists)
			{
				string json = string.Empty;

				using (FileStream fs = new FileStream(UsersFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader sr = new StreamReader(fs))
					{
						json = sr.ReadToEnd();
					}
				}

				if (!string.IsNullOrWhiteSpace(json))
				{
					Users = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				}
			}
		}

		public static void SaveUsers()
		{
			string json = JsonConvert.SerializeObject(Users);

			using (FileStream fs = new FileStream(UsersFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(json);
				}
			}
		}
	}
}