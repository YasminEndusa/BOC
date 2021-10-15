using System.Collections.Generic;
using System.Linq;

namespace ChatServerCore.ClientCommands
{
	public static class ClientCommandManager
	{
		private static readonly List<ClientCommand> _clientCommands = new List<ClientCommand>();

		static ClientCommandManager()
		{
			RegisterCommand(new ManageUserCommand());
		}

		public static void RegisterCommand(ClientCommand command)
		{
			if (!_clientCommands.Any(cc => { return cc.Command == command.Command; }))
			{
				_clientCommands.Add(command);
			}
		}

		public static ClientCommand GetCommand(string command)
		{
			return _clientCommands.FirstOrDefault(cc => { return cc.Command == command; });
		}

		public static string ParseInput(string input, out List<KeyValuePair<string, string>> parameters)
		{
			parameters = new List<KeyValuePair<string, string>>();
			string[] inputs = input.Split(' ');

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

		public static void HandleCommandInput(ChatClient client, string input)
		{
			List<KeyValuePair<string, string>> parameters;

			string commandString = ParseInput(input.Remove(0, 1), out parameters);
			ClientCommand command = GetCommand(commandString);

			command.Execute(client, parameters);
		}
	}
}