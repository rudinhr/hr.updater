using HrUpdater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HrAutoUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            var impargs = new StringArgs();
            impargs.AddArgument("help",
                "HrAutoUpdate [-command [arg]...] ...");
            impargs.AddArgument("file"
                , "specified update definition filename, default.json is default"
                + "\nsyntax: -file [filename]"
                + "\nsample: HrAutoUpdate -file hrd.json");
            impargs.AddArgument("verbose"
                , "show process and wait for input key");

            string filename = "default.json";

            impargs.LoadArgs(args);


            if (impargs.GetValue("help").IsSet)
            {
                System.Console.WriteLine(impargs.GetHelp());
            }
            else {
                bool verbose = impargs.GetValue("verbose").IsSet;

                if (impargs.GetValue("file").IsSet)
                {
                    filename = impargs.GetValue("file").Value;
                }
                try
                {
                    Action<string> logAction = null;
                    if(verbose)
                        logAction = (msg) => Console.WriteLine(msg);
                    HrUpdateEngine.Update(filename, logAction);
                }
                catch (Exception ex)
                {
                    if(verbose)
                        Console.WriteLine("Update Error " + ex.Message);
                }
                if(verbose)
                {
                    Console.WriteLine("Press any key to exit");
                    Console.ReadLine();
                }
                    
            }

            

        }
    }
}
