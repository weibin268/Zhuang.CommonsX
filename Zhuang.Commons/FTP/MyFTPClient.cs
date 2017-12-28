using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Zhuang.Commons.FTP
{
    public class MyFTPClient
    {
        private string _ftpServerIP;
        private string _userName;
        private string _password;
        private string _ftpWorkingDirectory;
        private string _ftpBaseUri;
        private ICredentials _credentials;

        public MyFTPClient(string ftpServerIP, string userName, string password, string domain = null)
        {
            _ftpServerIP = ftpServerIP;
            _userName = userName;
            _password = password;

            _ftpBaseUri = "ftp://" + _ftpServerIP + "/";
            if (!string.IsNullOrEmpty(_ftpWorkingDirectory))
            {
                _ftpBaseUri = _ftpBaseUri + _ftpWorkingDirectory + "/";
            }

            _credentials = new NetworkCredential(_userName, _password);
            if (domain == null)
            {
                _credentials = new NetworkCredential(_userName, _password);
            }
            else
            {
                _credentials = new NetworkCredential(_userName, _password, domain);
            }
        }

        public void ChangeWorkingDirectory(string ftpWorkingDirectory)
        {
            _ftpWorkingDirectory = ftpWorkingDirectory;
            _ftpBaseUri = "ftp://" + _ftpServerIP + "/" + _ftpWorkingDirectory + "/";
        }

        #region 上传
        public void UploadFile(string localFileFullName)
        {
            FileInfo file = new FileInfo(localFileFullName);
            using (FileStream fs = file.OpenRead())
            {
                UploadFile(fs, file.Name);
            }
        }

        public void UploadFile(Stream inStream, string ftpFileName)
        {
            string uri = _ftpBaseUri + ftpFileName;

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            request.Credentials = _credentials;
            //request.KeepAlive = false;//设置为false时，上传同个文件，第二次请求会报错
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            //request.ContentLength = file.Length;
            int buffLength = 2048;
            byte[] buffer = new byte[buffLength];
            int contentLen;
            using (Stream outStream = request.GetRequestStream())
            {
                contentLen = inStream.Read(buffer, 0, buffLength);
                while (contentLen != 0)
                {
                    outStream.Write(buffer, 0, contentLen);
                    contentLen = inStream.Read(buffer, 0, buffLength);
                }
            }
        }
        #endregion

        #region 下载
        public void DownloadFile(string localPath, string ftpFileName)
        {
            string fileName = Path.GetFileName(localPath);
            if (string.IsNullOrEmpty(fileName))
            {
                localPath = Path.Combine(localPath, Path.GetFileName(ftpFileName));
            }

            using (FileStream outStream = new FileStream(localPath, FileMode.Create))
            {
                using (Stream ftpStream = DownloadFile(ftpFileName))
                {
                    int buffLength = 2048;
                    int readCount;
                    byte[] buffer = new byte[buffLength];

                    readCount = ftpStream.Read(buffer, 0, buffLength);
                    while (readCount > 0)
                    {
                        outStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, buffLength);
                    }
                }
            }

        }

        public Stream DownloadFile(string ftpFileName)
        {
            MemoryStream ms = new MemoryStream();

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpBaseUri + ftpFileName));
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.Credentials = _credentials;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (var rs = response.GetResponseStream())
            {
                //rs.CopyTo(ms);

                int buffLength = 2048;
                int readCount;
                byte[] buffer = new byte[buffLength];

                readCount = rs.Read(buffer, 0, buffLength);
                while (readCount > 0)
                {
                    ms.Write(buffer, 0, readCount);
                    readCount = rs.Read(buffer, 0, buffLength);
                }

                ms.Seek(0, SeekOrigin.Begin);
            }

            return ms;
        }
        #endregion

        #region 增删查改
        public void DeleteFile(string ftFileName)
        {
            string uri = _ftpBaseUri + ftFileName;
            FtpWebRequest request;
            request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            request.Credentials = _credentials;
            //request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                response.GetResponseStream().Close();
            }
        }

        public void RemoveDirectory(string ftpDirectoryName)
        {
            string uri = _ftpBaseUri + ftpDirectoryName;
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            reqFTP.Credentials = _credentials;
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;

            string result = String.Empty;
            using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
            {
                response.GetResponseStream().Close();
            }

        }

        public void ReName(string oldFilename, string newFilename)
        {

            var request = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpBaseUri + oldFilename));
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = newFilename;
            request.UseBinary = true;
            request.Credentials = _credentials;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                response.GetResponseStream().Close();
            }
        }

        public void MakeDirectory(string ftpDirectoryName)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpBaseUri + ftpDirectoryName));
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.UseBinary = true;
            request.Credentials = _credentials;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                response.GetResponseStream().Close();
            }
        }

        public void MakeDirectoryBySmart(string ftpDirectoryName)
        {
            ftpDirectoryName = ftpDirectoryName.Trim('/', '\\');
            ftpDirectoryName = Regex.Replace(ftpDirectoryName, @"\\+|/+", "/");

            string[] ftpDirectoryNames = ftpDirectoryName.Split('/');

            string toMake = string.Empty;
            foreach (var tempName in ftpDirectoryNames)
            {
                toMake = toMake + tempName + "/";
                if (!CheckDirectoryOrFileExists(toMake))
                {
                    MakeDirectory(toMake);
                }
            }
        }
        #endregion

        #region 获取信息
        public IEnumerable<string> ListDirectoryDetails()
        {
            IList<string> lsResult = new List<string>();

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpBaseUri));
            request.UseBinary = true;
            request.Credentials = _credentials;
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            using (WebResponse response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Default))
                    {
                        string strLine = reader.ReadLine();
                        while (strLine != null)
                        {
                            lsResult.Add(strLine);
                            strLine = reader.ReadLine();
                        }
                    }
                }
                return lsResult;
            }
        }

        public IEnumerable<string> ListDirectoryFiles()
        {
            IList<string> lsResult = new List<string>();
            var dirDetails = ListDirectoryDetails();
            foreach (var Item in dirDetails)
            {
                if ((Item.Length > 0))
                {
                    if ((Item[0] != 'd') && (Item.ToUpper().IndexOf("<DIR>") < 0))
                    {
                        lsResult.Add(Item);
                    }
                }
            }

            return lsResult;
        }

        public IEnumerable<string> ListDirectoryFileNames()
        {
            IList<string> lsResult = new List<string>();

            var files = ListDirectoryFiles();

            foreach (var item in files)
            {
                lsResult.Add(item.Split(' ')[item.Split(' ').Length - 1]);
            }

            return lsResult;
        }

        //public IEnumerable<string> ListDirectory()
        //{
        //    IList<string> lsResult = new List<string>();

        //    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri));
        //    request.UseBinary = true;
        //    request.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
        //    request.Method = WebRequestMethods.Ftp.ListDirectory;
        //    using (WebResponse response = request.GetResponse())
        //    {
        //        using (var stream = response.GetResponseStream())
        //        {
        //            using (StreamReader reader = new StreamReader(stream, Encoding.Default))
        //            {
        //                string strLine = reader.ReadLine();
        //                while (strLine != null)
        //                {
        //                    lsResult.Add(strLine);
        //                    strLine = reader.ReadLine();
        //                }
        //            }
        //        }
        //        return lsResult;
        //    }
        //}

        public long GetFileSize(string filename)
        {
            long fileSize = 0;

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpBaseUri + filename));
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.UseBinary = true;
            request.Credentials = _credentials;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                response.GetResponseStream().Close();
                fileSize = response.ContentLength;
            }

            return fileSize;
        }

        public bool CheckDirectoryOrFileExists(string ftpDirectoryOrFileName)
        {
            bool result = true;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpBaseUri + ftpDirectoryOrFileName);
                request.Credentials = _credentials;
                //request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;//在web测环境测试有问题，目录不存在是不会抛异常。
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                }
            }
            catch (WebException)
            {
                result = false;
            }

            return result;
        }
        #endregion

    }
}
