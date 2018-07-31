using AltaLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace AltaArchiveApp.Services
{
    static class FTPService
    {
        static string host = "ftp://192.168.11.5";
        static public string Host { get { return host; } set { if (!String.IsNullOrEmpty(value)) host = value; } }

        static string login = "anonymous";
        static public string Login
        {
            get { return login; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    login = value;
            }
        }

        static string password = "v.kovalev@altatender.kz";
        static public string Password { get { return password; } set { password = value; } }

        static private int bufferSize = 2048;

        static internal bool CheckExistDirectory(string path, string directory)
        {
            List<string> directories = new List<string>();
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + "/" + path.Replace("\\", "/"));
                request.Credentials = new NetworkCredential(login, password);

                request.UseBinary = false;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var line = reader.ReadLine();
                        while (line != null)
                        {
                            directories.Add(line);
                            line = reader.ReadLine();
                        }
                    }
                }

                request = null;
                var filename = Path.GetFileName(path);
                if (directories.Contains(string.IsNullOrEmpty(filename) ? directory : string.Format("{0}/{1}", filename, directory)))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppJournal.Write("AltaArchiveApp.FTPService.CreateDirectory: " + ex.Message);
                return false;
            }
        }


        static internal bool CreateDirectory(string newDirectory)
        {
            try
            {
                string[] directories = newDirectory.Replace("\\", "/").Split('/');
                string fullPath = "";
                foreach (string directory in directories)
                {
                    if (!CheckExistDirectory(fullPath, directory))
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}/{2}", host, fullPath, directory));
                        request.Credentials = new NetworkCredential(login, password);

                        request.UseBinary = true;
                        request.UsePassive = true;
                        request.KeepAlive = true;
                        request.Method = WebRequestMethods.Ftp.MakeDirectory;
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        response.Close();
                        request = null;
                    }

                    fullPath += string.IsNullOrEmpty(fullPath) ? directory : "/" + directory;
                }
                return true;
            }
            catch (Exception ex)
            {
                AppJournal.Write("AltaArchiveApp.FTPService.CreateDirectory: " + ex);
                return false;
            }
        }


        static internal bool Upload(string localFile, string remoteFile)
        {
            FileStream localFileStream = new FileStream(localFile, FileMode.Open);
            return localFileStream != null ?  Upload(localFileStream, remoteFile) : false;
        }

        static internal bool Upload(Stream localStream, string remoteFile)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + "/" + remoteFile);
                request.Credentials = new NetworkCredential(login, password);
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;

                Stream stream = request.GetRequestStream();

                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesSent != 0)
                    {
                        stream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
                    AppJournal.Write("AltaArchiveApp.FTPService.Upload: " + ex.Message);
                    request = null;
                    stream.Close();
                    return false;
                }
                stream.Close();
                request = null;
                return true;
            }
            catch (Exception ex)
            {
                AppJournal.Write("AltaArchiveApp.FTPService.Upload: " + ex.Message);
                return false;
            }
        }

        static internal bool Donwload(string path, string saveAs)
        {
            var memoryStream = Donwload(path);
            if (memoryStream == null)
            {
                return false;
            }
            using (var fileStream = new FileStream(saveAs, FileMode.Create))
            {
                if (fileStream == null)
                {
                    return false;
                }
                memoryStream.CopyTo(fileStream);
            }
            memoryStream.Close();
            return true;
        }

        static internal Stream Donwload(string path)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(host + "/" + path);
                request.Credentials = new NetworkCredential(login, password);
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responceStream = response.GetResponseStream();
                Stream localStream = new MemoryStream();

                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = responceStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        localStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = responceStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); return null; }
                localStream.Seek(0, SeekOrigin.Begin);
                responceStream.Close();
                response.Close();
                request = null;
                return localStream;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return null;
        }


        static internal bool Exist(string fileName)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + "/" + fileName);
                request.Credentials = new NetworkCredential(login, password);
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                request = null;
                return true;
            }
            catch (Exception ex)
            {
                AppJournal.Write("AltaArchiveApp.FTPService.Exist: " + ex.Message);
                return false;
            }
        }
    }
}
