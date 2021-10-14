using System.Collections.Generic;
using System.Linq;

namespace ChatServerCore
{
	public static class CommandManager
	{
		public const string COMMAND_START = "start";
		public const string COMMAND_EXIT = "exit";
		public const string COMMAND_ADDUSER = "adduser";
		public const string COMMAND_CHANGEUSERPASS = "passwd";

		private static readonly List<ConsoleCommand> _consoleCommands = new List<ConsoleCommand>();

		static CommandManager()
		{
			RegisterCommand(new ConsoleCommand(COMMAND_START));
			RegisterCommand(new ConsoleCommand(COMMAND_EXIT));
			RegisterCommand(new ConsoleCommand(COMMAND_ADDUSER));
			RegisterCommand(new ConsoleCommand(COMMAND_CHANGEUSERPASS));
		}

		public static void RegisterCommand(ConsoleCommand command)
		{
			if (!_consoleCommands.Any(cc => { return cc.Command == command.Command; }))
			{
				_consoleCommands.Add(command);
			}
		}

		public static ConsoleCommand GetCommand(string command)
		{
			return _consoleCommands.FirstOrDefault(cc => { return cc.Command == command; });
		}

		public static string ParseConsoleInput(string consoleInput, out Dictionary<string, string> parameters)
		{
			parameters = new Dictionary<string, string>();
			string[] inputs = consoleInput.Split(' ');

			for (int i = 1; i < inputs.Length; i++)
			{
				parameters.Add(inputs[i], null);
			}

			return inputs[0];
		}
	}
}