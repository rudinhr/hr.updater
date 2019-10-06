using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HrUpdater
{
    public class HrUpdateEngine
    {
        public static void Update(string filename, Action<string> logAction=null)
        {
            string jsonStr = File.ReadAllText(filename);
            var defs = ((JArray)JObject.Parse(jsonStr).GetValue("Updates")).ToObject<List<HrUpdateEngineDefinition>>();
            Update(defs, logAction);
        }
        public static void Update(List<HrUpdateEngineDefinition> definitions, Action<string> logAction)
        {
            foreach(var definition in definitions)
            {
                Update(definition, logAction);
            }
        }
        public static void Update(HrUpdateEngineDefinition definition, Action<string> logAction = null)
        {
            Update(definition.Url, definition.DestinationPath, definition.Username, definition.Password, logAction);
        }
        public static void Update(string updateUrl, string destinationFolder, string username="", string password="", Action<string> logAction=null)
        {
            logAction?.Invoke("Start");
            var fgetter = FileContentGetterFactory.Instance.CreateFileGetter(updateUrl, username, password);
            var jsonByte = fgetter.GetFileContent(UpdaterUtil.UpdaterDefinitionFilename);
            string jsonStr = Encoding.UTF8.GetString(jsonByte);
            var newDefinition = JObject.Parse(jsonStr).ToObject<HrUpdaterDefinition>();
            if(!Directory.Exists(destinationFolder))
            {
                logAction?.Invoke($"Create Directory {destinationFolder}");
                Directory.CreateDirectory(destinationFolder);
            }

            var oldDefinitionFilename = $"{destinationFolder}\\{UpdaterUtil.UpdaterDefinitionFilename}";
            HrUpdaterDefinition oldDefinition = null;
            if(!File.Exists(oldDefinitionFilename))
            {
                oldDefinition = new HrUpdaterDefinition();
            }
            else
            {
                oldDefinition =
                    JObject.Parse(
                        File.ReadAllText(oldDefinitionFilename)
                    ).ToObject<HrUpdaterDefinition>();
            }

            var tmpFolder = $"{destinationFolder}\\tmpupdate";
            if (!Directory.Exists(tmpFolder))
            {
                logAction?.Invoke($"Create Directory {tmpFolder}");
                Directory.CreateDirectory(tmpFolder);
            }
            var tmpDefinitionFilename = $"{tmpFolder}\\{UpdaterUtil.UpdaterDefinitionFilename}";

            File.WriteAllBytes(
                        tmpDefinitionFilename
                        , jsonByte);
            var files = oldDefinition.GetDiffFiles(newDefinition);
            if(files!=null)
            {
                foreach (var f in files)
                {
                    var targetFile = $"{tmpFolder}{f.FileName}";
                    logAction?.Invoke($"Download file {targetFile}");
                    var targetDir = Path.GetDirectoryName(targetFile);
                    if (!Directory.Exists(targetDir))
                    {
                        logAction?.Invoke($"Create Directory {targetDir}");
                        Directory.CreateDirectory(targetDir);
                    }
                    var tmpByte = fgetter.GetFileContent(f.FileName);
                    File.WriteAllBytes($"{tmpFolder}{f.FileName}", tmpByte);
                }
                foreach(var f in files)
                {
                    var targetFile = $"{destinationFolder}{f.FileName}";
                    var srcFile = $"{tmpFolder}{f.FileName}";
                    var targetDir = Path.GetDirectoryName(targetFile);
                    if(!Directory.Exists(targetDir))
                    {
                        logAction?.Invoke($"Create Directory {targetDir}");
                        Directory.CreateDirectory(targetDir);
                    }
                    if (File.Exists(targetFile))
                    {
                        logAction?.Invoke($"Delete {targetFile}");
                        File.Delete(targetFile);
                    }
                    logAction?.Invoke($"Copy {srcFile} to {targetFile}");
                    File.Copy(srcFile, targetFile);
                }

                if (File.Exists(oldDefinitionFilename))
                {
                    logAction?.Invoke($"Delete {oldDefinitionFilename}");
                    File.Delete(oldDefinitionFilename);
                }
                logAction?.Invoke($"Copy {tmpDefinitionFilename} to {oldDefinitionFilename}");
                File.Copy(tmpDefinitionFilename, oldDefinitionFilename);
                logAction?.Invoke("Finished");
            }
            else
            {
                logAction?.Invoke("No update available");
            }            
        }
    }
}
