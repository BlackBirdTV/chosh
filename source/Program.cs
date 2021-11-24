using System;
using System.IO;
using CHOSH;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace chronoTerminal
{
    class Program
    {
        public static ConsoleColor consoleForeground = ConsoleColor.White;
        public static string env;
        public static bool updateenv = false;
        static string shellPath;
        public static bool fileexec = false;
        public static bool shortMode = false;
        public static string OS = "Unknown";
        public static bool terminate = false;
        public static string FolderName = "";
        public static void Main(string[] args)
        {
            Console.Clear();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { OS = "Linux"; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { OS = "Windows"; }
            // If there are arguments given
            if(args.Length != 0)
            {
                fileexec = true;
                if (args[0].EndsWith(".chosh"))
                {
                    env = "chosh";
                    foreach (string line in File.ReadAllLines(args[0]))
                    {
                        if (line.Contains("&&"))
                        {
                            foreach (string command in line.Split("&&"))
                            {
                                string useablecommand = command;
                                while(useablecommand.StartsWith(" "))
                                {
                                    useablecommand = useablecommand[1..];
                                }
                                if (command.StartsWith("repeat")) { Main(args); chosh.variables = new System.Collections.Generic.List<Variable>(); }
                                else if (line.Length > 0 && line.Split().Length > 0)
                                {
                                    chosh.Exec(sublib.Parse(command));
                                }
                            }
                        }
                        else
                        {
                            string useableline = line;
                            while(useableline.StartsWith(" "))
                            {
                                useableline = useableline[1..];
                            }
                            if (useableline.StartsWith("repeat")) { Main(args); chosh.variables = new System.Collections.Generic.List<Variable>(); }
                            else if (useableline.Length > 0 && useableline.Split().Length > 0)
                            {
                                chosh.Exec(sublib.Parse(useableline));
                            }
                        }
                    }
                }
                else if (args[0].EndsWith(".chex"))
                {
                    chosh.ExecCHEX(args[0]);
                }
            }
            // If there are none
            else
            {
                switch (env)
                {
                    case "chosh":
                        shellPath = @"/CHOSH/";
                        break;
                }
                if (OS == "Windows")
                {
                    Directory.SetCurrentDirectory("C:/");
                }
                else if (OS == "Linux")
                {
                    
                }
                else
                {
                    Console.WriteLine("This OS is not supported!");
                    return;
                }
                string shell = "chosh";
                string version = "2.0";
                string prefix = "\u00A2";
                string machinename = Environment.MachineName;
                string username = Environment.UserName;
                Console.WriteLine($"chronoTerminal with {shell} Ver. {version} loaded.");
                while (true)
                {
                    if (terminate) { return; }
                    if (updateenv) { updateenv = false; Console.Clear(); Main(Array.Empty<string>()); return; }
                    Console.ForegroundColor = ConsoleColor.White;
                    if (!shortMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(machinename + "@" + username);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(":");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(Directory.GetCurrentDirectory());
                    }
                    Console.ForegroundColor = consoleForeground;
                    Console.Write($" {prefix} ");
                    string stdin = Console.ReadLine();
                    try
                    {
                        Run(stdin);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public static void Run(string cmd)
        {
            while (cmd.StartsWith(" "))
            {
                cmd = cmd[1..];
            }
            if (cmd.Contains("&&"))
            {
                foreach (string command in cmd.Split("&&"))
                {
                    string usablecommand = command;
                    while (usablecommand.StartsWith(" "))
                    {
                        usablecommand = usablecommand[1..];
                    }
                    if (usablecommand.StartsWith("repeat")) { Run(cmd); chosh.variables = new System.Collections.Generic.List<Variable>(); }
                    else if (usablecommand.Length > 0 && usablecommand.Split().Length > 0)
                    {
                        chosh.Exec(sublib.Parse(usablecommand));
                    }
                }
            }
            else
            {
                if (cmd.Length > 0 && cmd.Split().Length > 0)
                {
                    chosh.Exec(sublib.Parse(cmd));
                }
            }
        }
    }
}
