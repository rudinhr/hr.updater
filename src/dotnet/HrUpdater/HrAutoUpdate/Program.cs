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
            

            string filename = "default.json";

            impargs.LoadArgs(args);
            if(impargs.GetValue("file").IsSet)
            {
                filename = impargs.GetValue("file").Value;
            }
            try
            {
                HrUpdateEngine.Update(filename, (msg) => Console.WriteLine(msg));
            }catch(Exception ex)
            {
                Console.WriteLine("Update Error " + ex.Message);
            }
            Console.ReadLine();

        }
    }
}
