using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.IO
{
    internal class ConsoleCommand
    {
        public string Name {get; }
        public string[] Args {get; }

        ConsoleCommand(string name, string[] args)
        {
            Name = name;
            Args = args;
        }

        public bool IsCommand(string command)
        {
            if(command.ToLower() == Name)
            {
                return true;
            }
            return false;
        }
        public void GetArg(int index, out string arg)
        {
            if (Args.Length <= index)
                throw new IndexOutOfRangeException();
            arg = Args[index];
        }

        public static ConsoleCommand GetInput()
        {
            string input = Console.ReadLine()?.ToLower() ?? "";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0
                ? new ConsoleCommand("", Array.Empty<string>())
                : new ConsoleCommand(parts[0], parts.Skip(1).ToArray());
        }
        public static bool TryGetIntArg(string[] args, int index, out int result)
        {
            result = 0;
            return args.Length > index && int.TryParse(args[index], out result);
        }
    }
}
