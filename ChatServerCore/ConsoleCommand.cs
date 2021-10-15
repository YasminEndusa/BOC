﻿using System.Collections.Generic;

namespace ChatServerCore
{
	public abstract class ConsoleCommand
	{
		public string Command;

		public ConsoleCommand()
		{
			this.Command = string.Empty;
		}

		public abstract void Execute(List<KeyValuePair<string, string>> parameters);
	}
}