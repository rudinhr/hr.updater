using HrUpdater;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HrUpdateCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var impargs = new StringArgs();
            impargs.AddArgument("help",
                "HrUpdaterCreator -project [project code] [-command [arg]...] ...");
            impargs.AddArgument("project"
                , "specified project name"
                + "\nsyntax: -project [tablename]"
                + "\nsample: -project IHRS");
            impargs.AddArgument("dir",
                "specified directory target, default startup path"
                + "\nsyntax : -dir [target directory]"
                + "\nsample : -dir \"C:\\HRD\""
                );

            impargs.LoadArgs(args);
            if(!impargs.GetValue("project").IsSet)
            {
                Console.WriteLine("Invalid Syntax, see -help");
                Console.ReadLine();
            }
            else if(impargs.GetValue("help").IsSet)
            {
                System.Console.WriteLine(impargs.GetHelp());
            }
            else
            {
                var projectName = impargs.GetValue("project").Value;
                var path = Directory.GetCurrentDirectory();
                if (impargs.GetValue("dir").IsSet)
                {
                    path = impargs.GetValue("dir").Value;
                }
                var definition = HrUpdaterDefinitionCreator.Instance.Create(projectName, path);
                var json = JObject.FromObject(definition).ToString();
                File.WriteAllText(path + "\\update.json"
                    , json);                
            }
            
        }
    }
}
