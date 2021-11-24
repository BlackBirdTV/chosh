using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHOSH;

namespace chronoTerminal
{
    class sublib
    {
        public static Command Parse(string stdin)
        {
            if (stdin.Contains("#")) { stdin = stdin.Remove(stdin.LastIndexOf("#")); }
            List<string> stdout_buffer = stdin.Split().ToList();
            for(int i = 0; i < stdout_buffer.Count; i++)
            {
                if(stdout_buffer[i] == "")
                {
                    stdout_buffer.RemoveAt(i);
                    i--;
                }
                else
                {
                    break;
                }
            }
            // Variables
            bool instr = false;
            string str_buffer = "";
            string stdout_cmd = stdout_buffer[0];
            List<string> stdout_args = new();
            List<string> stdout_flags = new();
            char stringchar = ' ';

            // Actual Parser
            stdout_buffer.RemoveAt(0);

            foreach (string elem in stdout_buffer)
            {
                if ((elem.StartsWith("\"") && elem.EndsWith("\"") && elem != "\"") || (elem.StartsWith("\'") && elem.EndsWith("\'") && elem != "\""))
                {
                    stdout_args.Add(elem[1..^1]);
                }
                else if (elem == "\"" || elem == "\'")
                {
                    if (instr)
                    {
                        if (elem[0] == stringchar)
                        {
                            instr = false;
                            stdout_args.Add(str_buffer);
                            str_buffer = "";
                        }
                        else
                        {
                            str_buffer += elem + " ";
                        }
                    }
                    else
                    {
                        instr = true;
                        stringchar = elem[0];
                        str_buffer += " ";
                    }

                }
                else if (elem.StartsWith("\"") || elem.StartsWith("\'"))
                {
                    if (instr)
                    {
                        if(elem[0] == stringchar)
                        {
                            instr = false;
                            stdout_args.Add(str_buffer);
                            str_buffer = "";
                        }
                        else
                        {
                            str_buffer += elem + " ";
                        }
                    }
                    else
                    {
                        instr = true;
                        stringchar = elem[0];
                        str_buffer += elem[1..] + " ";
                    }
                }
                else if (elem.EndsWith("\"") || elem.EndsWith("\'"))
                {
                    if (instr)
                    {
                        if (elem[^1] == stringchar)
                        {
                            instr = false;
                            str_buffer += elem[0..^1];
                            stdout_args.Add(str_buffer);
                            str_buffer = "";
                        }
                        else
                        {
                            str_buffer += elem + " ";
                        }
                    }
                    else
                    {
                        instr = true;
                        stringchar = elem[^1];
                    }
                }
                else if (instr)
                {
                    str_buffer += elem + " ";
                }
                else
                {
                    if (elem.StartsWith("--"))
                    {
                        stdout_flags.Add(elem[2..]);
                    }
                    else
                    {
                        stdout_args.Add(elem);
                    }
                    
                }
            }

            // Process variables
            for (int i = 0; i < stdout_args.ToArray().Length; i++)
            {
                if (stdout_args[i].Contains("[") && stdout_args[i].Contains("]") && Program.env == "chosh")
                {
                    foreach (Variable variable in chosh.variables)
                    {
                        if (stdout_args[i].Contains($"[{variable.Name}]"))
                        {
                            stdout_args[i] = stdout_args[i].Replace($"[{variable.Name}]", variable.Value);
                        }
                    }
                }
                if (stdout_args[i].Contains("%getinp"))
                {
                    stdout_args[i] = stdout_args[i].Replace("%getinp", Console.ReadLine());
                }
                if (stdout_args[i].Contains("%txteditor"))
                {
                    stdout_args[i] = stdout_args[i].Replace("%txteditor", chosh.txtEditorNS());
                }
                if (stdout_args[i].StartsWith("%calc:"))
                {
                    stdout_args[i] = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.EvaluateAsync(stdout_args[i][6..]).Result.ToString(); ;
                }
                if (stdout_args[i].StartsWith("%file:")) 
                {
                    stdout_args[i] = File.ReadAllText(stdout_args[i][6..]);
                }
            }

            // Return
            return new Command(stdout_cmd, stdout_args.ToArray(), stdout_flags.ToArray());
        }

        public static void Alert(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = Program.consoleForeground;
        }

        public static void Alert(string error, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(error);
            Console.ForegroundColor = Program.consoleForeground;
            Console.WriteLine($" {message}");
        }

        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = Program.consoleForeground;
        }

        public static void Warn(string error, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.ForegroundColor = Program.consoleForeground;
            Console.WriteLine($" {message}");
        }

        public static void Approve(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = Program.consoleForeground;
        }

        public static void Approve(string error, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(error);
            Console.ForegroundColor = Program.consoleForeground;
            Console.WriteLine($" {message}");
        }

        public static bool elemExists(string[] elements, string elemtofind)
        {
            bool doesExist = false;
            foreach (string elem in elements)
            {
                if (elem == elemtofind)
                {
                    doesExist = true;
                }
            }
            return doesExist;
        }
        public static bool switchBool(bool toswitch)
        {
            if (toswitch) { toswitch = false; } else { toswitch = true;  }
            return toswitch;
        }
        public static void printCenter(string s)
        {
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
        }
    }
}
