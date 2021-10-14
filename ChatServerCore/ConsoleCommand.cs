using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServerCore
{
	public class ConsoleCommand
	{
		public string Command;

		public ConsoleCommand(string command)
		{
			this.Command = command;
		}

		public void Execute(string parameters)
		{

		}
	}
}