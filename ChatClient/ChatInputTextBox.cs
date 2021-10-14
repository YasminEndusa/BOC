using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChatClient
{
	class ChatInputTextBox : TextBox
	{
		private const int WM_PAINT = 0xF;

		private const int WM_SETFOCUS = 0x7;
		private const int WM_KILLFOCUS = 0x8;

		private const int WM_SETFONT = 0x30;

		private const int WM_MOUSEMOVE = 0x200;
		private const int WM_LBUTTONDOWN = 0x201;
		private const int WM_RBUTTONDOWN = 0x204;
		private const int WM_MBUTTONDOWN = 0x207;
		private const int WM_LBUTTONUP = 0x202;
		private const int WM_RBUTTONUP = 0x205;
		private const int WM_MBUTTONUP = 0x208;
		private const int WM_LBUTTONDBLCLK = 0x203;
		private const int WM_RBUTTONDBLCLK = 0x206;
		private const int WM_MBUTTONDBLCLK = 0x209;

		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private const int WM_CHAR = 0x0102;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			int fileIconX = (int)((this.Height / 2.0) - (ImageManager.FileIcon.Height / 2.0));
			int fileIconY = this.Width - 3 - ImageManager.FileIcon.Width;
			Point fileIconLocation = new Point(fileIconX, fileIconY);

			int imageIconX = (int)((this.Height / 2.0) - (ImageManager.ImageIcon.Height / 2.0));
			int imageIconY = this.Width - 6 - ImageManager.ImageIcon.Width - ImageManager.FileIcon.Width;
			Point imageIconLocation = new Point(imageIconX, imageIconY);

			Graphics g = e.Graphics;
			g.DrawImage(ImageManager.FileIcon, fileIconLocation);
			g.DrawImage(ImageManager.ImageIcon, imageIconLocation);
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
		}

		[DllImport("user32")]
		private static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32")]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32")]
		private static extern int GetClientRect(IntPtr hWnd, ref RECT rect);

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
	}
}