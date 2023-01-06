using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace ZPF
{
    public static class ZIPHelper
    {
        public static string LastStatusCode = "";
        public static string LastErrorMessage = "";

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

        public static byte[] Zip(string str)
        {
            LastStatusCode = "";
            LastErrorMessage = "";

            var bytes = Encoding.UTF8.GetBytes(str);

            return Zip(bytes);
        }

        public static byte[] Zip(byte[] bytes)
        {
            LastStatusCode = "";
            LastErrorMessage = "";

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            return Encoding.UTF8.GetString(Unzip2Byte(bytes));
        }

        public static byte[] Unzip2Byte(byte[] bytes)
        {
            LastStatusCode = "";
            LastErrorMessage = "";

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return mso.ToArray();
            }
        }

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 


        //public static byte[] ZipFolder(string startPath, string zipPath)
        //{
        //   LastStatusCode = "";
        //   LastErrorMessage = "";

        //   string startPath = @".\start";
        //   string zipPath = @".\result.zip";
        //   string extractPath = @".\extract";

        //   ZipFile.CreateFromDirectory(startPath, zipPath);

        //   ZipFile.ExtractToDirectory(zipPath, extractPath);
        //}

        //public static string UnzipFolder(byte[] bytes)
        //{
        //   LastStatusCode = "";
        //   LastErrorMessage = "";

        //   string startPath = @".\start";
        //   string zipPath = @".\result.zip";
        //   string extractPath = @".\extract";

        //   ZipFile.CreateFromDirectory(startPath, zipPath);

        //   ZipFile.ExtractToDirectory(zipPath, extractPath);
        //}

        // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
    }
}
