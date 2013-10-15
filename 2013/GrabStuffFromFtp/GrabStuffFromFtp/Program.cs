using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GrabStuffFromFtp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string userName = "USERNAME";
            const string password = "PASSWORD";
            const string ftpPath = "ftp://waws-prod-am2-001.ftp.azurewebsites.windows.net/LogFiles/http/RawLogs/";
            const string destinationPath = @"C:\Users\Robert\Desktop\demo";


            string[] files = GetFileListFromFtp(ftpPath, userName, password);
            foreach (string file in files)
            {
                Console.WriteLine("Download File {0} from FTP.", file);
                DownloadSingleFileFromFtp(ftpPath, file, destinationPath, userName, password);
            }

            Console.WriteLine("And done...");

            Console.ReadLine();
        }


        public static string[] GetFileListFromFtp(string path, string userName, string password)
        {
            var result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                var reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(path));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(userName, password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                return result.ToString().Split('\n');
            }
            catch (Exception)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }

                throw;
            }
        }

        public static void DownloadSingleFileFromFtp(string sourcePath, string file, string destinationPath, string userName, string password)
        {
            var reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(sourcePath + file));
            reqFTP.Credentials = new NetworkCredential(userName, password);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFTP.UseBinary = true;
            reqFTP.Proxy = null;
            reqFTP.UsePassive = false;
            var response = (FtpWebResponse)reqFTP.GetResponse();
            var responseStream = response.GetResponseStream();
            var writeStream = new FileStream(Path.Combine(destinationPath, file), FileMode.Create);
            const int length = 2048;
            var buffer = new Byte[length];
            if (responseStream != null)
            {
                int bytesRead = responseStream.Read(buffer, 0, length);
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, length);
                }
            }
            writeStream.Close();
            response.Close();

        }
    }
}
