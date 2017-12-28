using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Zhuang.Commons.WebDAV
{
    public enum WebDAVMethod
    {
        PROPFIND,
        PROPPATCH,
        MKCOL,
        DELETE,
        PUT,
        COPY,
        MOVE,
        LOCK,
        UNLOCK
    }

    public class MyWebDAVClient
    {
        private string _userName;
        private string _password;
        private string _siteBaseUrl;
        private ICredentials _credentials;

        public MyWebDAVClient(string siteUrl, string userName, string password, string domain = null)
        {
            _userName = userName;
            _password = password;
            _siteBaseUrl = siteUrl;

            if (!_siteBaseUrl.EndsWith("/"))
                _siteBaseUrl = _siteBaseUrl + "/";

            //CredentialCache credentialCache = new CredentialCache();
            //credentialCache.Add(new Uri(_davBaseUrl), "NTLM", new NetworkCredential(_davUserName, _davPassword));
            //_credentials = credentialCache;
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

        #region 上传
        public void UploadFile(string localFileFullName)
        {
            FileInfo file = new FileInfo(localFileFullName);
            using (FileStream fs = file.OpenRead())
            {
                UploadFile(fs, file.Name);
            }
        }

        public void UploadFile(Stream inStream, string fileName)
        {
            WebClient client = new WebClient();
            client.Credentials = _credentials;
            Uri uri = new Uri(GetFullUrl(fileName));

            int buffLength = 2048;
            byte[] buffer = new byte[buffLength];
            int contentLen;
            using (Stream outStream = client.OpenWrite(uri, "PUT"))
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
        public void DownloadFile(string localPath, string davFileName)
        {

            string fileName = Path.GetFileName(localPath);
            if (string.IsNullOrEmpty(fileName))
            {
                localPath = Path.Combine(localPath, Path.GetFileName(davFileName));
            }

            using (FileStream outStream = new FileStream(localPath, FileMode.Create))
            {
                using (Stream ftpStream = DownloadFile(davFileName))
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

        public Stream DownloadFile(string davFileName)
        {
            Uri uriTarget = new Uri(GetFullUrl(davFileName));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriTarget);
            request.Credentials = _credentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }
        #endregion

        #region 增删查改
        public void DeleteFile(string davFileName)
        {
            SendWebDavRequest(GetFullUrl(davFileName), WebDAVMethod.DELETE);
        }

        public void RemoveDirectory(string davDirectoryName)
        {
            SendWebDavRequest(GetFullUrl(davDirectoryName), WebDAVMethod.DELETE);
        }

        public void MakeDirectory(string davDirectoryName)
        {
            SendWebDavRequest(GetFullUrl(davDirectoryName), WebDAVMethod.MKCOL);
        }

        public void MakeDirectoryBySmart(string davDirectoryName)
        {
            davDirectoryName = davDirectoryName.Trim('/', '\\');
            davDirectoryName = Regex.Replace(davDirectoryName, @"\\+|/+", "/");

            string[] ftpDirectoryNames = davDirectoryName.Split('/');

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

        public bool CheckDirectoryOrFileExists(string davDirectoryOrFileName)
        {
            try
            {
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(GetFullUrl(davDirectoryOrFileName));
                Request.Credentials = _credentials;
                Request.GetResponse();
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;
                if (HttpStatusCode.NotFound == response.StatusCode)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        private void SendWebDavRequest(string url, WebDAVMethod method)
        {
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(url);
            Request.Headers.Add("Translate: f");
            Request.Credentials = _credentials;
            Request.Method = method.ToString();
            HttpWebResponse Response;
            Response = (HttpWebResponse)Request.GetResponse();
        }

        private string GetFullUrl(string directoryOrFileName)
        {
            string result = _siteBaseUrl + directoryOrFileName;
            return result;
        }
    }
}
