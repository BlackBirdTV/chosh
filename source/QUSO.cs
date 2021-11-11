using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHOSH;
using chronoTerminal;

namespace QUSO
{
    class quso
    {
        public static void Exec(string[] _args, string[] _flags)
        {
            string cmd = _args[0];
            List<string> args = _args.ToList();
            args.RemoveAt(0);
            switch (cmd)
            {
                case "ext":
                    ext(args, _flags);
                    break;
            }
        }

        static void ext(List<string> args, string[] flags)
        {
            string cmd = args[0];
            args.RemoveAt(0);
            switch (cmd)
            {
                case "update":
                    chosh.extensions = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/CHOSH/extensions/");
                    break;
                case "add":
                    Console.WriteLine(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/CHOSH/extensions/" + args[0].Remove(args[0].LastIndexOf(".")) + ".cs"));
                    if(!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/CHOSH/extensions/" + args[0].Remove(args[0].LastIndexOf(".")) + ".cs") && !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/CHOSH/extensions/" + args[0].Remove(args[0].LastIndexOf(".")) + ".py"))
                    {
                        File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/CHOSH/extensions/" + args[0]).Close();
                        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/CHOSH/extensions/" + args[0], chosh.txtEditor());
                    }
                    else
                    {
                        sublib.Alert("Command already exists!");
                    }
                    break;
            }
        }
    }
}
