using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatClient
{
	class ChatMessageControl : Control
	{
		public event EventHandler<OpenImageEventArgs> OpenImage;

		private static readonly Font _font = new Font(FontFamily.GenericSansSerif, 10f);

		private readonly string _username;
		private readonly string _message;
		private readonly Image _image = null;
		private Image _thumb = null;
		private Rectangle _imageRect = Rectangle.Empty;
		
		public ChatMessageControl(string username, string message, Image image)
		{
			this.DoubleBuffered = true;

			this._username = username;
			this._message = message;
			this._image = image;
			this.CreateThumbnail();
			this.Height = 1;
			this.Invalidate();
		}

		private void CreateThumbnail()
		{
			if (this._image != null)
			{
				if (this._image.Width > 200 || this._image.Height > 100)
				{
					double widthFactor = this._image.Width / 200.0;
					double heightFactor = this._image.Height / 100.0;
					double factor;

					if (widthFactor > heightFactor)
					{
						factor = widthFactor;
					}
					else
					{
						factor = heightFactor;
					}

					int newWidth = (int)(this._image.Width / factor);
					int newHeight = (int)(this._image.Height / factor);

					this._thumb = new Bitmap(this._image, new Size(newWidth, newHeight));
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics g = e.Graphics;

			this.RecalculateSize(g);

			Pen p = Pens.Black;
			g.DrawRectangle(p, new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1)));

			SizeF usernameSize = g.MeasureString(this._username, _font, this.Width - 10);
			SizeF messageSize = g.MeasureString(this._message, _font, this.Width - 10);

			g.DrawString(this._username, _font, Brushes.Black, new PointF(5f, 5f));
			g.DrawString(this._message, _font, Brushes.Black, new RectangleF(new PointF(5f, usernameSize.Height + 5f), messageSize));

			if (this._image != null)
			{
				int y = (int)(usernameSize.Height + 5 + messageSize.Height + 5);
				Point pos = new Point(5, y);

				if (this._thumb != null)
				{
					this.DrawImage(g, this._thumb, pos);
				}
				else
				{
					this.DrawImage(g, this._image, pos);
				}
			}
		}

		private void DrawImage(Graphics g, Image img, Point pos)
		{
			g.DrawImage(img, pos);
			this._imageRect = new Rectangle(pos, img.Size);
		}

		private void RecalculateSize(Graphics g)
		{
			SizeF usernameSize = g.MeasureString(this._username, _font, this.Width - 10);
			SizeF messageSize = g.MeasureString(this._message, _font, this.Width - 10);

			int tmpHeight = (int)(5 + usernameSize.Height + 5 + messageSize.Height + 5);

			if (this._image != null)
			{
				if(this._thumb != null)
				{
					this.Height = tmpHeight + 10 + this._thumb.Height;
				}
				else
				{
					this.Height = tmpHeight + 10 + this._image.Height;
				}
			}
			else
			{
				this.Height = tmpHeight;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this._image?.Dispose();
			this._thumb?.Dispose();
			base.Dispose(disposing);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (this._image != null && this._imageRect != Rectangle.Empty)
			{
				if (e.Button == MouseButtons.Left && e.X >= this._imageRect.Left && e.X <= this._imageRect.Right && e.Y >= this._imageRect.Top && e.Y <= this._imageRect.Bottom)
				{
					this.OpenImage?.Invoke(this, new OpenImageEventArgs(this._image));
				}
			}
		}
	}
}