using Admin_Mouse_Shop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Admin_Mouse_Shop.Services.Classes
{
    internal class ServerService : IServerService
    {
        public void AddImage(string Path)
        {
            FileInfo name = new FileInfo(Path);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://auris.cityhost.com.ua/www/telegram-bots-maga.cx.ua/Photos/{name.Name}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(File.ReadAllText("username.txt"), File.ReadAllText("password.txt"));
            request.Proxy = null;
            request.KeepAlive = true;
            request.UseBinary = true;

            // Copy the contents of the file to the request stream.  
            byte[] fileContents = File.ReadAllBytes(Path);
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.WriteAsync(fileContents);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }
        public void DeleteImage(string Path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(File.ReadAllText("username.txt"), File.ReadAllText("password.txt"));
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }

        public void FtpUploadString(string text, string file)

        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://auris.cityhost.com.ua/www/telegram-bots-maga.cx.ua/" + file);

            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(File.ReadAllText("username.txt"), File.ReadAllText("password.txt")); 

            byte[] fileContents = Encoding.UTF8.GetBytes(text);
            Stream requestStream = request.GetRequestStream();
            requestStream.WriteAsync(fileContents);
            requestStream.Close();
        }

        public string FtpDownloadString(string file)

        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://auris.cityhost.com.ua/www/telegram-bots-maga.cx.ua/" + file);

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            request.Credentials = new NetworkCredential(File.ReadAllText("username.txt"), File.ReadAllText("password.txt")); 

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream ftpStream = response.GetResponseStream();

            StreamReader reader = new(ftpStream);

            return reader.ReadToEnd();
        }
    }
}
