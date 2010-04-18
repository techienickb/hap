using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace HAP.Web
{

    /// <summary>
    /// Summary description for FileUploadProcess
    /// </summary>

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

    public class FileUploadProcess
    {
        public event FileUploadCompletedEvent FileUploadCompleted;
        /// <summary>
        /// Determines if uploaded files should be renamed according to the user uploading them, otherwise if
        /// multiple users upload a file of the same name, it would try to save the file to the same name, throwing an error.
        /// Another way to prevent this is to create a seperate folder for each file.
        /// </summary>
        private bool uniqueUserUpload;
        public bool UniqueUserUpload
        {
            get { return uniqueUserUpload; }
            set { uniqueUserUpload = value; }
        }

        public FileUploadProcess()
        {
        }

        private HttpContext _context;

        public string Username
        {
            get
            {
                if (_context.User.Identity.Name.Contains("\\"))
                    return _context.User.Identity.Name.Remove(0, _context.User.Identity.Name.IndexOf('\\') + 1);
                else return _context.User.Identity.Name;
            }
        }

        public void ProcessRequest(HttpContext context, string uploadPath)
        {
            _context = context;
            string filename = context.Request.QueryString["filename"];
            bool complete = string.IsNullOrEmpty(context.Request.QueryString["Complete"]) ? true : bool.Parse(context.Request.QueryString["Complete"]);
            bool getBytes = string.IsNullOrEmpty(context.Request.QueryString["GetBytes"]) ? false : bool.Parse(context.Request.QueryString["GetBytes"]);
            long startByte = string.IsNullOrEmpty(context.Request.QueryString["StartByte"]) ? 0 : long.Parse(context.Request.QueryString["StartByte"]); ;

            string filePath;
            if (UniqueUserUpload)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    filePath = Path.Combine(uploadPath, string.Format("{0}_{1}", Username.Replace("\\", ""), filename));
                }
                else
                {
                    if (context.Session["fileUploadUser"] == null)
                        context.Session["fileUploadUser"] = Guid.NewGuid();
                    filePath = Path.Combine(uploadPath, string.Format("{0}_{1}", context.Session["fileUploadUser"], filename));
                }
            }
            else
                filePath = Path.Combine(uploadPath, filename);

            if (getBytes)
            {
                FileInfo fi = new FileInfo(filePath);
                if (!fi.Exists)
                    context.Response.Write("0");
                else
                    context.Response.Write(fi.Length.ToString());

                context.Response.Flush();
                return;
            }
            else
            {

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