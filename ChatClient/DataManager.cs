using System;
using System.IO;
using Newtonsoft.Json;

namespace ChatClient
{
	internal static class DataManager
	{
		internal static Settings Settings = new Settings();
		private static FileInfo SettingsFile;

		static DataManager()
		{
			InitFileSystem();
			LoadSettings();
		}

		private static void InitFileSystem()
		{
			string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			saveFolder = Path.Combine(saveFolder, "BOC");

			if (!Directory.Exists(saveFolder))
			{
				_ = Directory.CreateDirectory(saveFolder);
			}

			SettingsFile = new FileInfo(Path.Combine(saveFolder, "settings.json"));
		}

		private static void LoadSettings()
		{
			if (SettingsFile.Exists)
			{
				string json = string.Empty;

				using (FileStream fs = new FileStream(SettingsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader sr = new StreamReader(fs))
					{
						json = sr.ReadToEnd();
					}
				}

				if (!string.IsNullOrWhiteSpace(json))
				{
					Settings = JsonConvert.DeserializeObject<Settings>(json);

					if (Settings == null)
					{
						Settings = new Settings();
					}
				}
			}
		}

		public static void SaveSettings()
		{
			string json = JsonConvert.SerializeObject(Settings);

			using (FileStream fs = new FileStream(SettingsFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(json);
				}
			}
		}
	}
}
