using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrUpdater
{
    public class HrUpdaterDefinition
    {
        public string ProjectName { get; set; } = "";
        public string CurrentVersion { get; set; } = "";
        public string Hash { get; set; } = "";
        public List<FileHash> Files { get; set; } = new List<FileHash>();

        public List<FileHash> GetDiffFiles(HrUpdaterDefinition source)
        {
            if(Hash.Equals(source.Hash))
            {
                return null;
            }
            else {
                //indexing
                Dictionary<string, FileHash> dict = new Dictionary<string, FileHash>();
                foreach (var f in Files)
                {
                    dict.Add(f.FileName.ToLower(), f);
                }
                var result = new List<FileHash>();
                foreach(var f in source.Files)
                {
                    var key = f.FileName.ToLower();
                    if (dict.ContainsKey(key))
                    {
                        if (!dict[key].Hash.Equals(f.Hash))
                            result.Add(f);
                    }
                    else
                    {
                        result.Add(f);
                    }
                }
                return result;
            }
        }
    }

        
        
        public class FileHash
        {
            public string FileName { get; set; }
            public string Hash { get; set; }

            public override string ToString()
            {
                return $"{FileName} - {Hash}";
            }
        }
}
