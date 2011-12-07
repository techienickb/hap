using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HAP.AD;
using System.Web.Security;

/// <summary>
/// An abstract HTTP Handler that provides resumable file downloads in ASP.NET.
/// 
/// Created by:
///     Scott Mitchell
///     mitchell@4guysfromrolla.com
///     http://www.4guysfromrolla.com/ScottMitchell.shtml
/// </summary>
/// <remarks>
/// This class is a fairly close port of Alexander Schaaf's ZIPHandler HTTP Handler, which I found online at:
/// 
///     Tracking and Resuming Large File Downloads in ASP.NET
///     http://www.devx.com/dotnet/Article/22533/1954
/// 
/// I also found a similar version of this code in the download for the September 2006 issue of MSDN Magazine:
/// http://download.microsoft.com/download/f/2/7/f279e71e-efb0-4155-873d-5554a0608523/MSDNMag2006_09.exe
/// 
/// Alexander's code is in Visual Basic and was written for ASP.NET version 1.x. I ported the code to C#,
/// refactored portions of the code, and made use of functionality and features introduced in .NET 2.0 and 3.5.
/// </remarks>
public abstract class RangeRequestHandlerBase : IHttpHandler
{
    #region Constants
    private const string MULTIPART_BOUNDARY = "<q1w2e3r4t5y6u7i8o9p0>";
    private const string MULTIPART_CONTENTTYPE = "multipart/byteranges; boundary=" + MULTIPART_BOUNDARY;
    private const string DEFAULT_CONTENTTYPE = "application/octet-stream";
    private const string HTTP_HEADER_ACCEPT_RANGES = "Accept-Ranges";
    private const string HTTP_HEADER_ACCEPT_RANGES_BYTES = "bytes";
    private const string HTTP_HEADER_ACCEPT_RANGES_NONE = "none";
    private const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
    private const string HTTP_HEADER_CONTENT_RANGE = "Content-Range";
    private const string HTTP_HEADER_CONTENT_LENGTH = "Content-Length";
    private const string HTTP_HEADER_ENTITY_TAG = "ETag";
    private const string HTTP_HEADER_LAST_MODIFIED = "Last-Modified";
    private const string HTTP_HEADER_RANGE = "Range";
    private const string HTTP_HEADER_IF_RANGE = "If-Range";
    private const string HTTP_HEADER_IF_MATCH = "If-Match";
    private const string HTTP_HEADER_IF_NONE_MATCH = "If-None-Match";
    private const string HTTP_HEADER_IF_MODIFIED_SINCE = "If-Modified-Since";
    private const string HTTP_HEADER_IF_UNMODIFIED_SINCE = "If-Unmodified-Since";
    private const string HTTP_HEADER_UNLESS_MODIFIED_SINCE = "Unless-Modified-Since";
    private const string HTTP_METHOD_GET = "GET";
    private const string HTTP_METHOD_HEAD = "HEAD";

    private const int DEBUGGING_SLEEP_TIME = 0;
    #endregion

    #region Constructor
    public RangeRequestHandlerBase()
    {
        ADUser.ImpersonateContained();
        this.ProcessRequestCheckSteps =
            new Func<HttpContext, bool>[]
            {
                CheckAuthorizationRules,
                CheckHttpMethod,
                CheckFileRequested,
                CheckRangesRequested,
                CheckIfModifiedSinceHeader,
                CheckIfUnmodifiedSinceHeader,
                CheckIfMatchHeader,
                CheckIfNoneMatchHeader,
                CheckIfRangeHeader
            };
    }
    #endregion

    #region Properties
    /// <summary>
    /// Indicates if the HTTP request is for multiple ranges.
    /// </summary>
    public bool IsMultipartRequest { get; private set; }

    /// <summary>
    /// Indicates if the HTTP request is for one or more ranges.
    /// </summary>
    public bool IsRangeRequest { get; private set; }

    /// <summary>
    /// The start byte(s) for the requested range(s).
    /// </summary>
    public long[] StartRangeBytes { get; private set; }

    /// <summary>
    /// The end byte(s) for the requested range(s).
    /// </summary>
    public long[] EndRangeBytes { get; private set; }

    /// <summary>
    /// The size of each chunk of data streamed back to the client.
    /// </summary>
    /// <remarks>
    /// When a client makes a range request the requested file's contents are
    /// read in BufferSize chunks, with each chunk flushed to the output stream
    /// until the requested byte range has been read.
    /// </remarks>
    public virtual int BufferSize { get { return 10240; } }

    /// <summary>
    /// Indicates whether Range requests are enabled. If false, the HTTP Handler
    /// ignores the Range HTTP Header and returns the entire contents.
    /// </summary>
    public virtual bool EnableRangeRequests { get { return true; } }

    public bool IsReusable { get { return false; } }

    private Func<HttpContext, bool>[] ProcessRequestCheckSteps { get; set; }
    private FileInfo InternalRequestedFileInfo { get; set; }
    private string InternalRequestedFileEntityTag { get; set; }
    private string InternalRequestedFileMimeType { get; set; }
    private NameValueCollection InternalResponseHeaders = new NameValueCollection();
    #endregion

    #region Methods
    /// <summary>
    /// Returns a FileInfo object representing the requested content.
    /// </summary>
    public abstract FileInfo GetRequestedFileInfo(HttpContext context);

    /// <summary>
    /// Returns the Entity Tag (ETag) for the requested content.
    /// </summary>
    /// <remarks>
    /// The Entity Tag is computed by taking the physical path to the file, concatenating it with the
    /// file's created date and time, and computing the MD5 hash of that string.
    /// 
    /// A derived class MAY override this method to return an Entity Tag
    /// value computed using an alternate approach.
    /// </remarks>
    public virtual string GetRequestedFileEntityTag(HttpContext context)
    {
        FileInfo requestedFile = this.GetRequestedFileInfo(context);
        if (requestedFile == null)
            return string.Empty;

        ASCIIEncoding ascii = new ASCIIEncoding();
        byte[] sourceBytes = ascii.GetBytes(
                                string.Concat(
                                    requestedFile.FullName,
                                    "|",
                                    requestedFile.LastWriteTimeUtc
                                )
                             );

        return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(sourceBytes));
    }

    /// <summary>
    /// Returns the MIME type for the requested content.
    /// </summary>
    /// <remarks>
    /// A dervied class SHOULD override this method and return the MIME type specific
    /// to the requested content. See http://www.iana.org/assignments/media-types/ for
    /// a list of MIME types registered with the Internet Assigned Numbers Authority (IANA).
    /// </remarks>
    public virtual string GetRequestedFileMimeType(HttpContext context)
    {
        return DEFAULT_CONTENTTYPE;
    }

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            this.InternalRequestedFileInfo = this.GetRequestedFileInfo(context);
            this.InternalRequestedFileEntityTag = this.GetRequestedFileEntityTag(context);
            this.InternalRequestedFileMimeType = this.GetRequestedFileMimeType(context);

#if DEBUG
            LogRequestHttpHeaders(context.Request);
#endif

            // Parse the Range header (if it exists), populating the StartRangeBytes and EndRangeBytes arrays
            ParseRequestHeaderRanges(context);

            // Perform each check; exit if any check returns false
            foreach (var check in ProcessRequestCheckSteps)
                if (check(context) == false)
                {
#if DEBUG
                    LogResponseHttpHeaders(context.Response);
#endif
                    return;
                }

            // Checks passed, process request!

            if (!this.EnableRangeRequests || !this.IsRangeRequest)
                ReturnEntireEntity(context);
            else
                ReturnPartialEntity(context);
        }
        finally { ADUser.EndContainedImpersonate(); }
    }

    private HAP.AD.User _ADUser = null;
    public HAP.AD.User ADUser
    {
        get
        {
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == HAP.Web.Configuration.AuthMode.Windows) return ((HAP.AD.User)Membership.GetUser());
            if (_ADUser == null)
            {
                _ADUser = new User();
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                _ADUser.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            return _ADUser;
        }
    }

    private void ReturnEntireEntity(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        Response.StatusCode = 200;  // OK
        WriteCommonResponseHeaders(Response, this.InternalRequestedFileInfo.Length, this.InternalRequestedFileMimeType);

#if DEBUG
        LogResponseHttpHeaders(Response);
#endif
        ReturnChunkedResponse(context);
    }

    private void ReturnPartialEntity(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        Response.StatusCode = 206;  // Partial response
        // Specify the byte range being returned for non-multipart requests
        if (this.IsMultipartRequest == false)
            AddHeader(Response, HTTP_HEADER_CONTENT_RANGE,
                                    string.Format("bytes {0}-{1}/{2}",
                                                    this.StartRangeBytes[0].ToString(),
                                                    this.EndRangeBytes[0].ToString(),
                                                    this.InternalRequestedFileInfo.Length.ToString()
                                                    )
                                );

        WriteCommonResponseHeaders(Response,
                                    ComputeContentLength(),
                                    this.IsMultipartRequest ? MULTIPART_CONTENTTYPE : this.InternalRequestedFileMimeType);

#if DEBUG
        LogResponseHttpHeaders(Response);
#endif

        if (Request.HttpMethod.Equals(HTTP_METHOD_HEAD) == false)
            ReturnChunkedResponse(context);
    }

    private void ReturnChunkedResponse(HttpContext context)
    {
        HttpResponse Response = context.Response;

        byte[] buffer = new byte[this.BufferSize];
        using (FileStream fs = this.InternalRequestedFileInfo.OpenRead())
        {
            for (int i = 0; i < this.StartRangeBytes.Length; i++)
            {
                // Position the stream at the starting byte
                fs.Seek(this.StartRangeBytes[i], SeekOrigin.Begin);

                int bytesToReadRemaining = Convert.ToInt32(this.EndRangeBytes[i] - this.StartRangeBytes[i]) + 1;

                // Output multipart boundary, if needed
                if (this.IsMultipartRequest)
                {
                    Response.Output.Write("--" + MULTIPART_BOUNDARY);
                    Response.Output.Write(string.Format("{0}: {1}", HTTP_HEADER_CONTENT_TYPE, this.InternalRequestedFileMimeType));
                    Response.Output.Write(string.Format("{0}: bytes {1}-{2}/{3}",
                                                            HTTP_HEADER_CONTENT_RANGE,
                                                            this.StartRangeBytes[i].ToString(),
                                                            this.EndRangeBytes[i].ToString(),
                                                            this.InternalRequestedFileInfo.Length.ToString()
                                                        )
                                         );
                    Response.Output.WriteLine();
                }

                // Stream out the requested chunks for the current range request
                while (bytesToReadRemaining > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        int chunkSize = fs.Read(buffer, 0, this.BufferSize < bytesToReadRemaining ? this.BufferSize : bytesToReadRemaining);
                        Response.OutputStream.Write(buffer, 0, chunkSize);

                        bytesToReadRemaining -= chunkSize;

                        Response.Flush();

#if DEBUG
                        System.Threading.Thread.Sleep(DEBUGGING_SLEEP_TIME);
#endif
                    }
                    else
                    {
                        // Client disconnected - quit
                        return;
                    }
                }
            }

            fs.Close();
        }
    }

    private int ComputeContentLength()
    {
        int contentLength = 0;

        for (int i = 0; i < this.StartRangeBytes.Length; i++)
        {
            contentLength += Convert.ToInt32(this.EndRangeBytes[i] - this.StartRangeBytes[i]) + 1;

            if (this.IsMultipartRequest)
                contentLength += MULTIPART_BOUNDARY.Length
                                + this.InternalRequestedFileMimeType.Length
                                + this.StartRangeBytes[i].ToString().Length
                                + this.EndRangeBytes[i].ToString().Length
                                + this.InternalRequestedFileInfo.Length.ToString().Length
                                + 49;       // Length needed for multipart header
        }

        if (this.IsMultipartRequest)
            contentLength += MULTIPART_BOUNDARY.Length
                                + 8;    // Length of dash and line break

        return contentLength;
    }

    private void WriteCommonResponseHeaders(HttpResponse Response, long contentLength, string contentType)
    {
        AddHeader(Response, HTTP_HEADER_CONTENT_LENGTH, contentLength.ToString());
        AddHeader(Response, HTTP_HEADER_CONTENT_TYPE, contentType);
        AddHeader(Response, HTTP_HEADER_LAST_MODIFIED, this.InternalRequestedFileInfo.LastWriteTimeUtc.ToString("r"));
        AddHeader(Response, HTTP_HEADER_ENTITY_TAG, string.Concat("\"", this.InternalRequestedFileEntityTag, "\""));
        if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["inline"]))
            AddHeader(Response, "Content-Disposition", "attachment; filename=\"" + this.InternalRequestedFileInfo.Name + "\"");
        else AddHeader(Response, "Content-Disposition", "inline; filename=\"" + this.InternalRequestedFileInfo.Name + "\"");

        if (this.EnableRangeRequests)
            AddHeader(Response, HTTP_HEADER_ACCEPT_RANGES, HTTP_HEADER_ACCEPT_RANGES_BYTES);
        else
            AddHeader(Response, HTTP_HEADER_ACCEPT_RANGES, HTTP_HEADER_ACCEPT_RANGES_NONE);
    }

    private string RetrieveHeader(HttpRequest Request, string headerName, string defaultValue)
    {
        return string.IsNullOrEmpty(Request.Headers[headerName]) ? defaultValue : Request.Headers[headerName].Replace("\"", string.Empty);
    }

    protected virtual void ParseRequestHeaderRanges(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        string rangeHeader = RetrieveHeader(Request, HTTP_HEADER_RANGE, string.Empty);

        if (string.IsNullOrEmpty(rangeHeader))
        {
            // No Range HTTP Header supplied; send back entire contents
            this.StartRangeBytes = new long[] { 0 };
            this.EndRangeBytes = new long[] { this.InternalRequestedFileInfo.Length - 1 };
            this.IsRangeRequest = false;
            this.IsMultipartRequest = false;
        }
        else
        {
            // rangeHeader contains the value of the Range HTTP Header and can have values like:
            //      Range: bytes=0-1            * Get bytes 0 and 1, inclusive
            //      Range: bytes=0-500          * Get bytes 0 to 500 (the first 501 bytes), inclusive
            //      Range: bytes=400-1000       * Get bytes 500 to 1000 (501 bytes in total), inclusive
            //      Range: bytes=-200           * Get the last 200 bytes
            //      Range: bytes=500-           * Get all bytes from byte 500 to the end
            //
            // Can also have multiple ranges delimited by commas, as in:
            //      Range: bytes=0-500,600-1000 * Get bytes 0-500 (the first 501 bytes), inclusive plus bytes 600-1000 (401 bytes) inclusive

            // Remove "Ranges" and break up the ranges
            string[] ranges = rangeHeader.Replace("bytes=", string.Empty).Split(",".ToCharArray());

            this.StartRangeBytes = new long[ranges.Length];
            this.EndRangeBytes = new long[ranges.Length];

            this.IsRangeRequest = true;
            this.IsMultipartRequest = (this.StartRangeBytes.Length > 1);

            for (int i = 0; i < ranges.Length; i++)
            {
                const int START = 0, END = 1;

                // Get the START and END values for the current range
                string[] currentRange = ranges[i].Split("-".ToCharArray());

                if (string.IsNullOrEmpty(currentRange[END]))
                    // No end specified
                    this.EndRangeBytes[i] = this.InternalRequestedFileInfo.Length - 1;
                else
                    // An end was specified
                    this.EndRangeBytes[i] = long.Parse(currentRange[END]);

                if (string.IsNullOrEmpty(currentRange[START]))
                {
                    // No beginning specified, get last n bytes of file
                    this.StartRangeBytes[i] = this.InternalRequestedFileInfo.Length - 1 - this.EndRangeBytes[i];
                    this.EndRangeBytes[i] = this.InternalRequestedFileInfo.Length - 1;
                }
                else
                {
                    // A normal begin value
                    this.StartRangeBytes[i] = long.Parse(currentRange[0]);
                }
            }
        }
    }

    /// <summary>
    /// Adds an HTTP Response Header
    /// </summary>
    /// <remarks>
    /// This method is used to store the Response Headers in a private, member variable,
    /// InternalResponseHeaders, so that the Response Headers may be accesed in the
    /// LogResponseHttpHeaders method, if needed. The Response.Headers property can only
    /// be accessed directly when using IIS 7's Integrated Pipeline mode. This workaround
    /// permits logging of Response Headers when using Classic mode or a web server other
    /// than IIS 7.
    /// </remarks>
    protected void AddHeader(HttpResponse Response, string name, string value)
    {
        InternalResponseHeaders.Add(name, value);

        Response.AddHeader(name, value);
    }

    #region Process Request Step Checks
    protected virtual bool CheckAuthorizationRules(HttpContext context)
    {
        // Any authorization checks should be implemented in the derived class
        return true;
    }

    protected virtual bool CheckHttpMethod(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        if (!Request.HttpMethod.Equals(HTTP_METHOD_GET) &&
            !Request.HttpMethod.Equals(HTTP_METHOD_HEAD))
        {
            Response.StatusCode = 501;  // Not Implemented
            return false;
        }

        return true;
    }

    protected virtual bool CheckFileRequested(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        if (this.InternalRequestedFileInfo == null)
        {
            Response.StatusCode = 404;  // Not Found
            return false;
        }
        if (this.InternalRequestedFileInfo.Length > int.MaxValue)
        {
            Response.StatusCode = 413; // Request Entity Too Large
            return false;
        }

        return true;
    }

    protected virtual bool CheckRangesRequested(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        for (int i = 0; i < this.StartRangeBytes.Length; i++)
        {
            if (this.StartRangeBytes[i] > this.InternalRequestedFileInfo.Length - 1 ||
                this.EndRangeBytes[i] > this.InternalRequestedFileInfo.Length - 1)
            {
                context.Response.StatusCode = 400; // Bad Request
                return false;
            }

            if (this.StartRangeBytes[i] < 0 || this.EndRangeBytes[i] < 0)
            {
                context.Response.StatusCode = 400; // Bad Request
                return false;
            }

            if (this.EndRangeBytes[i] < this.StartRangeBytes[i])
            {
                context.Response.StatusCode = 400; // Bad Request
                return false;
            }
        }

        return true;
    }

    protected virtual bool CheckIfModifiedSinceHeader(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        string ifModifiedSinceHeader = RetrieveHeader(Request, HTTP_HEADER_IF_MODIFIED_SINCE, string.Empty);

        if (!string.IsNullOrEmpty(ifModifiedSinceHeader))
        {
            // Determine the date
            DateTime ifModifiedSinceDate;
            DateTime.TryParse(ifModifiedSinceHeader, out ifModifiedSinceDate);

            if (ifModifiedSinceDate == DateTime.MinValue)
                // Could not parse date... do not continue on with check
                return true;

            DateTime requestedFileModifiedDate = this.InternalRequestedFileInfo.LastWriteTimeUtc;
            requestedFileModifiedDate = new DateTime(
                                            requestedFileModifiedDate.Year,
                                            requestedFileModifiedDate.Month,
                                            requestedFileModifiedDate.Day,
                                            requestedFileModifiedDate.Hour,
                                            requestedFileModifiedDate.Minute,
                                            requestedFileModifiedDate.Second
                                        );
            ifModifiedSinceDate = ifModifiedSinceDate.ToUniversalTime();

            if (requestedFileModifiedDate <= ifModifiedSinceDate)
            {
                // File was created before specified date
                Response.StatusCode = 304;  // Not Modified
                return false;
            }
        }

        return true;
    }

    protected virtual bool CheckIfUnmodifiedSinceHeader(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        string ifUnmodifiedSinceHeader = RetrieveHeader(Request, HTTP_HEADER_IF_UNMODIFIED_SINCE, string.Empty);

        if (string.IsNullOrEmpty(ifUnmodifiedSinceHeader))
            // Look for Unless-Modified-Since header
            ifUnmodifiedSinceHeader = RetrieveHeader(Request, HTTP_HEADER_UNLESS_MODIFIED_SINCE, string.Empty);

        if (!string.IsNullOrEmpty(ifUnmodifiedSinceHeader))
        {
            // Determine the date
            DateTime ifUnmodifiedSinceDate;
            DateTime.TryParse(ifUnmodifiedSinceHeader, out ifUnmodifiedSinceDate);

            DateTime requestedFileModifiedDate = this.InternalRequestedFileInfo.LastWriteTimeUtc;
            requestedFileModifiedDate = new DateTime(
                                            requestedFileModifiedDate.Year,
                                            requestedFileModifiedDate.Month,
                                            requestedFileModifiedDate.Day,
                                            requestedFileModifiedDate.Hour,
                                            requestedFileModifiedDate.Minute,
                                            requestedFileModifiedDate.Second
                                        );
            if (ifUnmodifiedSinceDate != DateTime.MinValue)
                ifUnmodifiedSinceDate = ifUnmodifiedSinceDate.ToUniversalTime();

            if (requestedFileModifiedDate > ifUnmodifiedSinceDate)
            {
                // Could not convert value into date or file was created after specified date
                Response.StatusCode = 412;  // Precondition failed
                return false;
            }
        }

        return true;
    }

    protected virtual bool CheckIfMatchHeader(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        string ifMatchHeader = RetrieveHeader(Request, HTTP_HEADER_IF_MATCH, string.Empty);

        if (string.IsNullOrEmpty(ifMatchHeader) || ifMatchHeader == "*")
            return true;        // Match found

        // Look for a matching ETag value in ifMatchHeader
        string[] entityIds = ifMatchHeader.Replace("bytes=", string.Empty).Split(",".ToCharArray());

        foreach (string entityId in entityIds)
        {
            if (this.InternalRequestedFileEntityTag == entityId)
                return true;        // Match found
        }

        // If we reach here, no match found
        Response.StatusCode = 412;  // Precondition failed
        return false;
    }

    protected virtual bool CheckIfNoneMatchHeader(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        string ifNoneMatchHeader = RetrieveHeader(Request, HTTP_HEADER_IF_NONE_MATCH, string.Empty);

        if (string.IsNullOrEmpty(ifNoneMatchHeader))
            return true;

        if (ifNoneMatchHeader == "*")
        {
            // Logically invalid request
            Response.StatusCode = 412;  // Precondition failed
            return false;
        }

        // Look for a matching ETag value in ifNoneMatchHeader
        string[] entityIds = ifNoneMatchHeader.Replace("bytes=", string.Empty).Split(",".ToCharArray());

        foreach (string entityId in entityIds)
        {
            if (this.InternalRequestedFileEntityTag == entityId)
            {
                AddHeader(Response, HTTP_HEADER_ENTITY_TAG, string.Concat("\"", entityId, "\""));
                Response.StatusCode = 304;  // Not modified
                return false;        // Match found
            }
        }

        // No match found
        return true;
    }

    protected virtual bool CheckIfRangeHeader(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        string ifRangeHeader = RetrieveHeader(Request, HTTP_HEADER_IF_RANGE, this.InternalRequestedFileEntityTag);

        if (this.IsRangeRequest && ifRangeHeader != this.InternalRequestedFileEntityTag)
        {
            ReturnEntireEntity(context);
            return false;
        }

        return true;
    }
    #endregion

    #region Logging Methods
    private void LogRequestHttpHeaders(HttpRequest Request)
    {
        string output = string.Concat("REQUEST INFORMATION (", Request.HttpMethod, ")", Environment.NewLine);
        foreach (string name in Request.Headers.Keys)
        {
            output += string.Format("{0}: {1}{2}", name, Request.Headers[name], Environment.NewLine);
        }

        output += Environment.NewLine + Environment.NewLine;

        HAP.Web.Logging.EventViewer.Log(HttpContext.Current.Request.RawUrl, output, System.Diagnostics.EventLogEntryType.Information);
    }

    private void LogResponseHttpHeaders(HttpResponse Response)
    {
        string output = string.Concat("RESPONSE INFORMATION (", Response.StatusCode.ToString(), ")", Environment.NewLine);
        foreach (string name in InternalResponseHeaders.Keys)
        {
            output += string.Format("{0}: {1}{2}", name, InternalResponseHeaders[name], Environment.NewLine);
        }

        output += Environment.NewLine + Environment.NewLine;
        HAP.Web.Logging.EventViewer.Log(HttpContext.Current.Request.RawUrl, output, System.Diagnostics.EventLogEntryType.Information);
    }
    #endregion
    #endregion
}