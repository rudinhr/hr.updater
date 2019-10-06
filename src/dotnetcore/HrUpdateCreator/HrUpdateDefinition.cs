using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HrUpdateCreator
{
    public class HrUpdateDefinition
    {
        public string ProjectName {get;set;}
        public string CurrentVersion {get;set;}
        public string Hash {get;set;}
        public List<FileHash> Files{get;set;} = new List<FileHash>();
    }

    public class HrUpdateDefinitionCreator {
        static HrUpdateDefinitionCreator _instance=null;
        public static HrUpdateDefinitionCreator Instance{
            get{
                if(_instance == null)
                    _instance = new HrUpdateDefinitionCreator();
                return _instance;
            }
        }

        public HrUpdateDefinition Create(string projectName, string path){
            var result = new HrUpdateDefinition();
            result.ProjectName = projectName;
            result.CurrentVersion = DateTime.Now.ToString("yyyyMMddHHmmssfff");            
            result.Hash = UpdaterUtil.CreateMD5(result.ProjectName+result.CurrentVersion);
            var files = GetFiles(path);
            List<Task> tasks = new List<Task>();
            foreach(var file in files)
            {
                var task = GetFileInfoAsync(file).ContinueWith(
                    (x)=>{
                        var fi = x.Result;
                        fi.FileName = fi.FileName.Substring(path.Length);
                        result.Files.Add(fi);
                        Console.WriteLine(fi);
                    }
                );
                tasks.Add(task);
            }
            var taskArray = tasks.ToArray();
            Task.WaitAll(taskArray);
            return result;
        }



        private async Task<FileHash> GetFileInfoAsync(string filename)
        {
            var result = new FileHash();
            result.FileName = filename;
            result.Hash = UpdaterUtil.CreateMD5(await File.ReadAllBytesAsync(filename));            
            
            return result;
        }

        private List<string> GetFiles(string path)
        {
            List<string> result = new List<string>();
            var files = Directory.EnumerateFiles(path);
            foreach(var file in files)
            {
                result.Add(file);
            }
            var directories = Directory.EnumerateDirectories(path);
            foreach(var dr in directories)
            {
                result.AddRange(GetFiles(dr));
            }
            return result;
        }

        
    }

    public class FileHash {
        public string FileName{get;set;}
        public string Hash{get;set;}

        public override string ToString(){
            return $"{FileName} - {Hash}";
        }
    }
}