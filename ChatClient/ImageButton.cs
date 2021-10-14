using System.Drawing;
using System.Windows.Forms;

namespace ChatClient
{
	class ImageButton : Button
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (this.Image != null)
			{
				Rectangle r = this.ClientRectangle;
				e.Graphics.DrawImage(this.Image, r);
			}
		}
	}
}