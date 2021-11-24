using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chronoTerminal;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Reflection;
using System.Media;
using System.IO.Compression;

namespace CHOSH
{
    public class Variable
    {
        public string Name;
        public string Value;

        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class Command
    {
        public string cmd;
        public string[] args;
        public string[] flgs;

        public Command(string command, string[] arguments, string[] flags)
        {
            cmd = command;
            args = arguments;
            flgs = flags;
        }
    }

    class chosh
    {
        public static List<Variable> variables = new();
        public static string[] extensions = Directory.GetFiles(Assembly.GetEntryAssembly().Location[..Assembly.GetEntryAssembly().Location.LastIndexOf("\\")] + @"/ext/");

        public static void Exec(Command command)
        {
            try
            {
                switch (command.cmd)
                {
                    case "@csharp":
                        Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.EvaluateAsync("1");
                        break;
                    case "quso":
                        QUSO.quso.Exec(command.args, command.flgs);
                        break;
                    case "var":
                        Var(command.args);
                        break;
                    case "print":
                        if (!sublib.elemExists(command.flgs, "noline")) { Console.WriteLine(command.args[0]); } else { Console.Write(command.args[0]); }
                        break;
                    case "cd":
                        try { if (Directory.Exists(command.args[0])) { Directory.SetCurrentDirectory(command.args[0]); } else { sublib.Warn("Error going to Directory!", "The given Directory does not exist!"); } } catch { sublib.Warn("Error going to Directory!", "An unknown error occured."); }
                        break;
                    case "makedir":
                        fileaction("mkdir", command.args);
                        break;
                    case "makefile":
                        fileaction("mkfile", command.args);
                        break;
                    case "deldir":
                        fileaction("deldir", command.args);
                        break;
                    case "delfile":
                        fileaction("delfile", command.args);
                        break;
                    case "movefile":
                        fileaction("movefile", command.args);
                        break;
                    case "movedir":
                        fileaction("movedir", command.args);
                        break;
                    case "writefile":
                        fileaction("write", command.args);
                        break;
                    case "waitforkey":
                        Console.ReadKey();
                        break;
                    case "info":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.White; Console.WriteLine(" /\u0305\u0305\u0305\u0305\u0305\u0305\u0305\u0305\u0305\u0305\u0305| |\u0305\u0305|     |\u0305\u0305|    /\u0305\u0305\u0305\u0305\u0305\u0305\u0305\\     /\u0305\u0305\u0305/   |\u0305\u0305|     |\u0305\u0305|   CHOSH Version 1.0\n/   /\u0305\u0305\u0305\u0305\u0305\u0305\u0305\u0305  |  |     |  |   / /\u0305\u0305\u0305\u0305\u0305\\ \\   /   /    |  |     |  |   Credits: Developer: Luis Weinzierl @ BlackBird\n|  /           |  |_____|  |  / /       \\ \\  \\   \\    |  |_____|  |            Jabsy owner: Rick @ Rick.707\n|  |           |           |  | |       | |   \\   \\   |           |\n|  \\           |  |\u0305\u0305\u0305\u0305\u0305|  |  \\ \\       / /   /   /   |  |\u0305\u0305\u0305\u0305\u0305|  |\n\\   \\________  |  |     |  |   \\ \\_____/ /   /   /    |  |     |  |\n \\___________| |__|     |__|    \\_______/   /___/     |__|     |__|\n");
                        break;
                    case "csharp":
                        Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.RunAsync(command.args[0], ScriptOptions.Default.WithImports("System"));
                        break;
                    case "shortmode":
                        Console.Clear();
                        Program.shortMode = sublib.switchBool(Program.shortMode);
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "title":
                        Console.Title = command.args[0];
                        break;
                    case "fgcolor":
                        Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.RunAsync($"Console.ForegroundColor = ConsoleColor.{command.args[0]}", ScriptOptions.Default.WithImports("System"));
                        break;
                    case "bgcolor":
                        Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.RunAsync($"Console.BackgroundColor = ConsoleColor.{command.args[0]}", ScriptOptions.Default.WithImports("System"));
                        break;
                    case "py2":
                        ScriptEngine pyengine = Python.CreateEngine();
                        pyengine.Execute(command.args[0]);
                        break;
                    case "txt":
                        txtEditor();
                        break;
                    case "list":
                        foreach (string folder in Directory.GetDirectories(Directory.GetCurrentDirectory())) { int ind_slash = folder.LastIndexOf("/"); if (ind_slash == -1) { ind_slash = 0; } else { ind_slash += 1; } int ind_backslash = folder.LastIndexOf("\\"); if (ind_backslash == -1) { ind_backslash = 0; } else { ind_backslash += 1; } int ind_dot = folder.LastIndexOf(".");Console.BackgroundColor = ConsoleColor.DarkGreen; Console.Write(folder[ind_slash..][ind_backslash..] + "/");Console.BackgroundColor = ConsoleColor.Black; Console.WriteLine(); }
                        foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory())) { int ind_slash = file.LastIndexOf("/"); if (ind_slash == -1) { ind_slash = 0; } else { ind_slash += 1;  } int ind_backslash = file.LastIndexOf("\\"); if (ind_backslash == -1) { ind_backslash = 0; } else { ind_backslash += 1; } Console.WriteLine(file[ind_slash..][ind_backslash..]); }
                        break;
                    case "ps":
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "powershell.exe",
                                Arguments = command.args[0],
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();

                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            Console.WriteLine(line);
                        }

                        process.WaitForExit();
                        break;
                    case "restart":
                        Program.Main(Array.Empty<string>());
                        break;
                    case "exit":
                        Program.terminate = true;
                        break;
                    case "playsound":
                        using (var soundPlayer = new SoundPlayer(command.args[0]))
                        {
                            soundPlayer.PlaySync(); // can also use soundPlayer.PlaySync()
                        }
                        break;
                    case "chex":
                        ExecCHEX(command.args[0]);
                        break;
                    default:
                        bool isextcmd = false;
                        foreach (string file in extensions)
                        {
                            if (file.Remove(file.LastIndexOf("."))[(file.LastIndexOf("/") + 1)..] == command.cmd)
                            {
                                string argsbuffer = "";
                                if (command.args.Length > 0 && file.EndsWith(".cs"))
                                {
                                    foreach (string elem in command.args)
                                    {
                                        argsbuffer += $"\"{elem.Replace("\"", "\\\"")}\",";
                                    }
                                    argsbuffer = argsbuffer[0..^1];
                                }
                                else if (command.args.Length > 0 && file.EndsWith(".py"))
                                {
                                    foreach (string elem in command.args)
                                    {
                                        argsbuffer += $"'{elem}',";
                                    }
                                    argsbuffer = argsbuffer[0..^1];
                                }
                                isextcmd = true;
                                if (file.EndsWith(".cs"))
                                {
                                    Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.RunAsync("string[] args = {" + argsbuffer + "};\n" + File.ReadAllText(file), ScriptOptions.Default.WithImports("System"));
                                }
                                else if (file.EndsWith(".py"))
                                {
                                    ScriptEngine pyextengine = Python.CreateEngine();
                                    pyextengine.Execute($"args = [{argsbuffer}]\n" + File.ReadAllText(file));
                                }
                                else if (file.EndsWith(".chosh"))
                                {
                                    Program.Main(new string[] { file });
                                }
                            }
                        }
                        if (!isextcmd)
                        {
                            sublib.Warn("Unknown Command.");
                        }

                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Var(string[] args)
        {

            if (args[1] == "=" && !args[0].Contains(" "))
            {
                bool doesexist = false;
                int index = 0;
                int i = 0;
                foreach (Variable elem in variables)
                {
                    if (elem.Name == args[0])
                    {
                        doesexist = true;
                        index = i;
                    }
                    i++;
                }
                if (doesexist)
                {
                    variables[index] = new Variable(args[0], args[2]);
                }
                else
                {
                    variables.Add(new Variable(args[0], args[2]));
                }

            }
            else if (args[1] == "+=" && !args[0].Contains(" "))
            {
                bool doesexist = false;
                int index = 0;
                int i = 0;
                foreach (Variable elem in variables)
                {
                    if (elem.Name == args[0])
                    {
                        doesexist = true;
                        index = i;
                    }
                    i++;
                }
                if (doesexist)
                {
                    string oldval = variables[index].Value;
                    variables[index] = new Variable(args[0], oldval + args[2]);
                }
                else
                {
                    sublib.Warn("Variable does not exist yet and thus cannot be appended.");
                }
            }
            else
            {
                sublib.Warn("Wrong Format.", "Declare or change variables like this: var NAME = VALUE. The name cannot contain spaces!");
            }
        }

        static void fileaction(string action, string[] args)
        {
            switch (action)
            {
                case "mkdir":
                    if (!Directory.Exists(args[0]))
                    {
                        try { Directory.CreateDirectory(args[0]); }
                        catch { sublib.Warn("Error creating directory!", "An unknown error occured."); }
                    } else { sublib.Warn("Error creating directory!", "The Directory already exists."); }
                    break;
                case "mkfile":
                    if (!File.Exists(args[0]))
                    {
                        try { File.Create(args[0]).Close(); if (args.Length > 1) { File.WriteAllText(args[0], args[1]); } }
                        catch { sublib.Warn("Error creating file!", "An unknown error occured."); }
                    }
                    else { sublib.Warn("Error creating file!", "The file already exists."); }
                    break;
                case "deldir":
                    if (Directory.Exists(args[0]))
                    {
                        try { Directory.Delete(args[0]); }
                        catch { sublib.Warn("Error deleting directory!", "An unknown error occured."); }
                    }
                    else { sublib.Warn("Error deleting directory!", "The Directory doesn't exist."); }
                    break;
                case "delfile":
                    if (File.Exists(args[0]))
                    {
                        try { File.Delete(args[0]); }
                        catch { sublib.Warn("Error deleting file!", "An unknown error occured."); }
                    }
                    else { sublib.Warn("Error deleting file!", "The File doesn't exist."); }
                    break;
                case "movefile":
                    if (File.Exists(args[0]))
                    {
                        try { File.Move(args[0], args[1]); }
                        catch { sublib.Warn("Error renaming file!", "An unknown error occured."); }
                    }
                    else { sublib.Warn("Error renaming file!", "The File doesn't exist."); }
                    break;
                case "movedir":
                    if (Directory.Exists(args[0]))
                    {
                        try { Directory.Move(args[0], args[1]); }
                        catch { sublib.Warn("Error renaming Directory!", "An unknown error occured."); }
                    }
                    else { sublib.Warn("Error renaming Directory!", "The Directory doesn't exist."); }
                    break;
                case "write":
                    File.WriteAllText(args[0], args[1]);
                    break;
            }
        }

        public static string txtEditor()
        {
            string text = "";
            bool hasended = false;
            Console.WriteLine("CHOSH txtEditor");
            Console.WriteLine("You can end the editor by typing %END or save to File by typing %SAVE [filename]");
            while(hasended == false)
            {
                Console.Write("] ");
                string line = Console.ReadLine();
                if(line == "%END")
                {
                    hasended = true;
                }
                else
                {
                    text += $"{line}\n";
                }
            }
            return text;
        }

        public static string txtEditorNS()
        {
            string text = "";
            bool hasended = false;
            Console.WriteLine("CHOSH txtEditor");
            Console.WriteLine("You can end the editor by typing %END");
            while (hasended == false)
            {
                Console.Write("] ");
                string line = Console.ReadLine();
                if (line == "%END")
                {
                    hasended = true;
                }
                else
                {
                    text += $"{line}\n";
                }
            }
            return text;
        }

        public static void ExecCHEX(string file)
        {
            char slash = '\\';
            if (file.Contains("/"))
            {
                slash = '/';
            }
            string FolderName = file[file.LastIndexOf(slash)..file.LastIndexOf(".")];
            string assetsFolder = $"{Path.GetTempPath() + slash}chexAppCache{slash + FolderName + slash}";
            if (Directory.Exists(assetsFolder))
            {
                Directory.Delete(assetsFolder, true);
            }
            ZipFile.ExtractToDirectory(file, assetsFolder);
            if (File.Exists($"{assetsFolder}start.chosh"))
            {
                foreach (string line in File.ReadAllLines($"{assetsFolder}start.chosh"))
                {
                    string parsedline = "";
                    if (line.Contains("%assets:"))
                    {
                        parsedline = line.Replace("%assets:", assetsFolder);
                    }
                    Program.Run(parsedline);
                }
            }
        }
    }
}
