using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatServerCore
{
	public static class CommandManager
	{
		private static readonly List<ConsoleCommand> _consoleCommands = new List<ConsoleCommand>();

		static CommandManager()
		{
			RegisterCommand(new ConsoleCommands.StartCommand());
			RegisterCommand(new ConsoleCommands.ExitCommand());
			RegisterCommand(new ConsoleCommands.ManageUserCommand());
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

		public static string ParseConsoleInput(string consoleInput, out List<KeyValuePair<string, string>> parameters)
		{
			parameters = new List<KeyValuePair<string, string>>();
			string[] inputs = consoleInput.Split(' ');

			for (int i = 1; i < inputs.Length; i++)
			{
				string tmpParam = inputs[i];

				if (tmpParam.Contains(':'))
				{
					string[] tmpParamParts = tmpParam.Split(':');
					string paramName = tmpParamParts[0];
					string paramValue = string.Join(':', tmpParamParts.Skip(1).ToArray());

					parameters.Add(new KeyValuePair<string, string>(paramName, paramValue));
				}
				else
				{
					parameters.Add(new KeyValuePair<string, string>(tmpParam, null));
				}
			}

			return inputs[0];
		}

		public static void HandleCommandInput(string consoleInput)
		{
			List<KeyValuePair<string, string>> parameters;

			string commandString = ParseConsoleInput(consoleInput.Remove(0, 1), out parameters);
			ConsoleCommand command = GetCommand(commandString);

			command.Execute(parameters);
		}
	}
}