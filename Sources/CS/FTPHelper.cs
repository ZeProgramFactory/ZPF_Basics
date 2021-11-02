using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ZPF
{
   public static class FTPHelper
   {
      public static string LastStatusCode = "";
      public static string LastErrorMessage = "";

      public static bool Upload(string Server, string FileName, string Login, string Pwd, string Data)
      {
         // https://docs.microsoft.com/fr-fr/dotnet/framework/network-programming/how-to-upload-files-with-ftp
         // - - -  - - - 

         LastStatusCode = "";
         LastErrorMessage = "";

         // Get the object used to communicate with the server.
         FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{Server}/{FileName}");

         // This example assumes the FTP site uses anonymous logon.
         //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
         request.Credentials = new NetworkCredential(Login, Pwd);

         request.Method = WebRequestMethods.Ftp.UploadFile;

         // Copy the contents of the file to the request stream.
         byte[] fileContents;
         fileContents = Encoding.UTF8.GetBytes(Data);
         request.ContentLength = fileContents.Length;

         using (Stream requestStream = request.GetRequestStream())
         {
            requestStream.Write(fileContents, 0, fileContents.Length);
         }

         using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
         {
            Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");

            LastStatusCode = response.StatusCode.ToString();
            LastErrorMessage = response.StatusDescription;
         }

         // - - -  - - - 

         // 226 Successfully transferred ...
         return LastErrorMessage.Trim().StartsWith("226");
      }

      public static bool UploadUtf8File(string Server, string FileName, string Login, string Pwd)
      {

         string Data = File.ReadAllText(FileName, System.Text.Encoding.UTF8);

         return FTPHelper.Upload(Server, FileName, Login, Pwd, Data);
      }

      public static string DirList(string Server, string Login, string Pwd)
      {
         // https://docs.microsoft.com/fr-fr/dotnet/framework/network-programming/how-to-list-directory-contents-with-ftp
         // - - -  - - - 

         LastStatusCode = "";
         LastErrorMessage = "";

         // Get the object used to communicate with the server.
         FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{Server}/");
         request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

         // This example assumes the FTP site uses anonymous logon.
         //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
         request.Credentials = new NetworkCredential(Login, Pwd);

         FtpWebResponse response = (FtpWebResponse)request.GetResponse();

         Stream responseStream = response.GetResponseStream();
         StreamReader reader = new StreamReader(responseStream);

         string Data = reader.ReadToEnd();
         Console.WriteLine($"Directory List Complete, status {response.StatusDescription}");

         LastStatusCode = response.StatusCode.ToString();
         LastErrorMessage = response.StatusDescription;

         reader.Close();
         response.Close();

         // - - -  - - - 

         // 226 Successfully transferred 

         if (LastErrorMessage.Trim().StartsWith("226"))
         {
            return Data;
         }
         else
         {
            return null;
         };
      }

      public static string Download(string Server, string FileName, string Login, string Pwd)
      {
         // https://docs.microsoft.com/fr-fr/dotnet/framework/network-programming/how-to-download-files-with-ftp
         // - - -  - - - 

         LastStatusCode = "";
         LastErrorMessage = "";

         // Get the object used to communicate with the server.
         FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{Server}/{FileName}");
         request.Method = WebRequestMethods.Ftp.DownloadFile;

         // This example assumes the FTP site uses anonymous logon.
         //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
         request.Credentials = new NetworkCredential(Login, Pwd);

         FtpWebResponse response = (FtpWebResponse)request.GetResponse();

         Stream responseStream = response.GetResponseStream();
         StreamReader reader = new StreamReader(responseStream);

         string Data = reader.ReadToEnd();
         Console.WriteLine($"Download Complete, status {response.StatusDescription}");

         LastStatusCode = response.StatusCode.ToString();
         LastErrorMessage = response.StatusDescription;

         reader.Close();
         response.Close();

         // - - -  - - - 

         // 226 Successfully transferred 

         if (LastErrorMessage.Trim().StartsWith("226"))
         {
            return Data;
         }
         else
         {
            return null;
         };
      }
   }
}
