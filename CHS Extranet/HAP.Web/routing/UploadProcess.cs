using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace HAP.Web.routing
{
    public delegate void FileUploadCompletedEvent(object sender, FileUploadCompletedEventArgs args);
    public class FileUploadCompletedEventArgs
    {
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public FileUploadCompletedEventArgs() { }

        public FileUploadCompletedEventArgs(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }

    public class UploadProcess
    {
        public event FileUploadCompletedEvent FileUploadCompleted;

        public UploadProcess()
        {
        }

        private HttpContext _context;

        public void ProcessRequest(HttpContext context, string uploadPath)
        {
            _context = context;
            string filename = context.Request.QueryString["filename"];
            bool complete = string.IsNullOrEmpty(context.Request.QueryString["Complete"]) ? true : bool.Parse(context.Request.QueryString["Complete"]);
            long startByte = string.IsNullOrEmpty(context.Request.QueryString["StartByte"]) ? 0 : long.Parse(context.Request.QueryString["StartByte"]); ;

            string filePath = Path.Combine(uploadPath, filename);

            if (startByte > 0 && File.Exists(filePath))
            {

                using (FileStream fs = File.Open(filePath, FileMode.Append))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
                    fs.Dispose();
                }
            }
            else
            {
                using (FileStream fs = File.Create(filePath))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
                    fs.Dispose();
                }
            }
            if (complete)
            {
                if (FileUploadCompleted != null)
                {
                    FileUploadCompletedEventArgs args = new FileUploadCompletedEventArgs(filename, filePath);
                    FileUploadCompleted(this, args);
                }

            }
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }
    }
}