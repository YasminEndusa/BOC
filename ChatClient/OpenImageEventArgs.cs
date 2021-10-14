using System;
using System.Drawing;

namespace ChatClient
{
	public class OpenImageEventArgs : EventArgs
	{
		public Image Image { get; private set; }

		public OpenImageEventArgs(Image img)
		{
			this.Image = img;
		}
	}
}