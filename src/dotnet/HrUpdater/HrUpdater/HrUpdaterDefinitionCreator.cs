using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrUpdater
{
    public class HrUpdaterDefinitionCreator
    {
        static HrUpdaterDefinitionCreator _instance = null;
        public static HrUpdaterDefinitionCreator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HrUpdaterDefinitionCreator();
                return _instance;
            }
        }

        public HrUpdaterDefinition Create(string projectName, string path)
        {
            var result = new HrUpdaterDefinition();
            result.ProjectName = projectName;
            result.CurrentVersion = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            result.Hash = UpdaterUtil.CreateMD5(result.ProjectName + result.CurrentVersion);
            var files = GetFiles(path);
            List<Task> tasks = new List<Task>();
            foreach (var file in files)
            {
                var task =
                Task.Factory.StartNew(
                () =>
                {
                    var fi = GetFileInfo(file);
                    if (!fi.FileName.Equals(path + "\\" + UpdaterUtil.UpdaterDefinitionFilename, StringComparison.OrdinalIgnoreCase))
                    {
                        fi.FileName = fi.FileName.Substring(path.Length);
                        result.Files.Add(fi);
                        Console.WriteLine(fi);
                    }
                }
                );

                tasks.Add(task);
            }
            var taskArray = tasks.ToArray();
            Task.WaitAll(taskArray);
            return result;
        }



        private FileHash GetFileInfo(string filename)
        {
            var result = new FileHash();
            result.FileName = filename;
            result.Hash = UpdaterUtil.CreateMD5(File.ReadAllBytes(filename));
            return result;
        }

        private List<string> GetFiles(string path)
        {
            List<string> result = new List<string>();
            var files = Directory.EnumerateFiles(path);
            foreach (var file in files)
            {
                result.Add(file);
            }
            var directories = Directory.EnumerateDirectories(path);
            foreach (var dr in directories)
            {
                result.AddRange(GetFiles(dr));
            }
            return result;
        }


    }

}
