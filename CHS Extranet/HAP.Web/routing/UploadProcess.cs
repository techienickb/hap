using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace HAP.Web.routing
{
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
                }
            }
            else
            {
                using (FileStream fs = File.Create(filePath))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
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