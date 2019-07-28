using System.ComponentModel;
using System.Windows.Forms;

namespace ChatClientCore
{
	internal static class InvokeHelper
	{
		public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
		{
			if (obj.InvokeRequired)
			{
				object[] args = new object[0];
				obj.Invoke(action, args);
			}
			else
			{
				action();
			}
		}
	}
}