using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ChatClient
{
	internal static class ImageManager
	{
		internal static Image FileIcon;
		internal static Image ImageIcon;

		static ImageManager()
		{
			string imagesFolder = Path.Combine(Application.StartupPath, "img");
			FileIcon = Image.FromFile(Path.Combine(imagesFolder, "file.png"));
			ImageIcon = Image.FromFile(Path.Combine(imagesFolder, "image.png"));
		}

		public static string ImageToBase64(Image img)
		{
			string str;

			using (MemoryStream m = new MemoryStream())
			{
				img.Save(m, img.RawFormat);
				byte[] bytes = m.ToArray();
				str = Convert.ToBase64String(bytes);
			}

			return str;
		}

		public static Image ImageFromBase64(string str)
		{
			Image img;

			using (MemoryStream m = new MemoryStream())
			{
				byte[] bytes = Convert.FromBase64String(str);
				m.Write(bytes, 0, bytes.Length);
				img = Image.FromStream(m);
			}

			return img;
		}
	}
}