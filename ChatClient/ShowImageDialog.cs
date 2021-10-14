using System.Drawing;
using System.Windows.Forms;

namespace ChatClient
{
	public partial class ShowImageDialog : Form
	{
		public ShowImageDialog()
		{
			this.InitializeComponent();
		}

		public void SetImage(Image image)
		{
			this.pictureBox1.Image = image;
		}

		private void ShowImageDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
	}
}