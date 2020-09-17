using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HrUpdater
{
    public interface IFileContentGetter
    {
        byte[] GetFileContent(string fileName);
    }

    
    public class HttpFileGetter : IFileContentGetter
    {
        string mUrl;
        public HttpFileGetter(string protocol, string address)
        {
            mUrl = $"{protocol}://{address}";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }
        public byte[] GetFileContent(string fileName)
        {
            HttpWebRequest http = (HttpWebRequest)HttpWebRequest.Create(
                mUrl + "/" + fileName.Replace('\\','/')
                );
            HttpWebResponse response = (HttpWebResponse)http.GetResponse();
            return ReadStream(response.GetResponseStream());                        
        }
        private byte[] ReadStream(Stream input)
        {
                int bytesBuffer = 1024;
                byte[] buffer = new byte[bytesBuffer];
                using (MemoryStream ms = new MemoryStream())
                {
                    int readBytes;
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, readBytes);
                    }
                    return ms.ToArray();
                }
        }
    }
    public class DirectFileGetter : IFileContentGetter
    {
        string mBasePath = "";
        public DirectFileGetter(string path)
        {
            mBasePath = path;
        }
        public byte[] GetFileContent(string fileName)
        {
            return File.ReadAllBytes(mBasePath +"\\" + fileName);
        }
    }
    public class FileContentGetterFactory
    {
        public static FileContentGetterFactory Instance => Singleton<FileContentGetterFactory>.Instance;
        public IFileContentGetter CreateFileGetter(string url, string username="", string password = "")
        {
            var idxProtocol = url.IndexOf("://");
            var protocol = "http";
            var address = url;
            if (idxProtocol >= 0)
            {
                protocol = url.Substring(0, idxProtocol);
                address = url.Substring(idxProtocol + 3);
            }
            switch(protocol)
            {
                case "http":
                case "https":
                    return new HttpFileGetter(protocol, address);                    
                case "file":
                    return new DirectFileGetter(address);
                default:
                    throw new Exception("Protocol not supported (File Content Getter Factory)");
            }
            

        }
    }

}
