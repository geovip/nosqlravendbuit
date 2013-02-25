using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace UIT.NoSQL.Web.Controllers
{
    public class FilesController : Controller
    {
        private string _StorageRoot;
        private string StorageRoot
        {
            get { return _StorageRoot; }
        }

        public FilesController()
        {
            _StorageRoot = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), ConfigurationManager.AppSettings["DIR_FILE_UPLOADS"]);
        }

        [HttpGet]
        public ActionResult List()
        {
            var fileData = new List<ViewDataUploadFilesResult>();

            DirectoryInfo dir = new DirectoryInfo(StorageRoot);
            if (dir.Exists)
            {
                string[] extensions = MimeTypes.ImageMimeTypes.Keys.ToArray();

                FileInfo[] files = dir.EnumerateFiles()
                         .Where(f => extensions.Contains(f.Extension.ToLower()))
                         .ToArray();

                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        var fileId = file.Name.Substring(0, 20);
                        var fileNameEncoded = HttpUtility.HtmlEncode(file.Name.Substring(21));
                        var relativePath = "/Files/" + fileId + "/" + fileNameEncoded;

                        fileData.Add(new ViewDataUploadFilesResult()
                        {
                            url = relativePath,
                            thumbnail_url = relativePath, //@"data:image/png;base64," + EncodeFile(fullName),
                            name = fileNameEncoded,
                            type = MimeTypes.ImageMimeTypes[file.Extension],
                            size = Convert.ToInt32(file.Length),
                            delete_url = relativePath,
                            delete_type = "DELETE"
                        });
                    }
                }
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        public ActionResult Find(string id, string filename)
        {
            if (id == null || filename == null)
            {
                return HttpNotFound();
            }

            var filePath = Path.Combine(_StorageRoot, id + "-" + filename);

            FileStreamResult result = new FileStreamResult(new System.IO.FileStream(filePath, System.IO.FileMode.Open), GetMimeType(filePath));
            result.FileDownloadName = filename;

            return result;
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Uploads()
        {
            var fileData = new List<ViewDataUploadFilesResult>();

            foreach (string file in Request.Files)
            {
                UploadWholeFile(Request, fileData);
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        private void UploadWholeFile(HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];

                var fileId = IDGen.NewID();
                var fileName = Path.GetFileName(file.FileName);
                var fileNameEncoded = HttpUtility.HtmlEncode(fileName);
                var fullPath = Path.Combine(StorageRoot, fileId + "-" + fileName);

                file.SaveAs(fullPath);

                //  encode html
                //statuses.Add(new ViewDataUploadFilesResult()
                //{
                //    url = "/Files/" + fileId + "/" + fileNameEncoded,
                //    thumbnail_url = "/Files/" + fileId + "/" + fileNameEncoded, //@"data:image/png;base64," + EncodeFile(fullName),
                //    name = fileNameEncoded,
                //    type = file.ContentType,
                //    size = file.ContentLength,
                //    delete_url = "/Files/" + fileId + "/" + fileNameEncoded,
                //    delete_type = "DELETE"
                //});

                // tieng viet
                statuses.Add(new ViewDataUploadFilesResult()
                {
                    url = "/Files/" + fileId + "/" + fileName,
                    thumbnail_url = "/Files/" + fileId + "/" + fileName, //@"data:image/png;base64," + EncodeFile(fullName),
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "/Files/" + fileId + "/" + fileName,
                    delete_type = "DELETE"
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        public ActionResult Delete(string id, string filename)
        {
            if (id == null || filename == null)
            {
                return HttpNotFound();
            }

            var filePath = Path.Combine(_StorageRoot, id + "-" + filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var fileNameForDownload = filename.Substring(21, filename.Length - 21);
            var filePath = Path.Combine(_StorageRoot, filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileNameForDownload + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        private string GetMimeType(string filePath)
        {
            return GetMimeType(new FileInfo(filePath));
        }
        private string GetMimeType(FileInfo file)
        {
            return MimeTypes.ImageMimeTypes[file.Extension];
        }

    }

    public class ViewDataUploadFilesResult
    {
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
    }

    public static class MimeTypes
    {
        public static Dictionary<string, string> ImageMimeTypes = new Dictionary<string, string>
		{
			{ ".gif", "image/gif" },
            { ".jpeg", "image/jpeg" },
			{ ".jpg", "image/jpeg" },
			{ ".png", "image/png" },
		};
    }

    public class IDGen
    {
        private static readonly IDGen _instance = new IDGen();

        // 0, 1, O, I omitted intentionally giving 32 (2^5) symbols
        private static char[] _charMap = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static IDGen GetInstance()
        {
            return _instance;
        }

        private RNGCryptoServiceProvider _provider = new RNGCryptoServiceProvider();

        private IDGen()
        {
        }

        private void GetNext(byte[] bytes)
        {
            _provider.GetBytes(bytes);
        }

        public static string NewID()
        {
            return _instance.GetBase32UniqueId(20);
        }

        private string GetBase32UniqueId(int numDigits)
        {
            return GetBase32UniqueId(new byte[0], numDigits);
        }

        private string GetBase32UniqueId(byte[] basis, int numDigits)
        {
            int byteCount = 16;
            var randBytes = new byte[byteCount - basis.Length];
            GetNext(randBytes);
            var bytes = new byte[byteCount];
            Array.Copy(basis, 0, bytes, byteCount - basis.Length, basis.Length);
            Array.Copy(randBytes, 0, bytes, 0, randBytes.Length);

            ulong lo = (((ulong)BitConverter.ToUInt32(bytes, 8)) << 32) | BitConverter.ToUInt32(bytes, 12); // BitConverter.ToUInt64(bytes, 8);
            ulong hi = (((ulong)BitConverter.ToUInt32(bytes, 0)) << 32) | BitConverter.ToUInt32(bytes, 4);  // BitConverter.ToUInt64(bytes, 0);
            ulong mask = 0x1F;

            var chars = new char[26];
            int charIdx = 25;

            ulong work = lo;
            for (int i = 0; i < 26; i++)
            {
                if (i == 12)
                {
                    work = ((hi & 0x01) << 4) & lo;
                }
                else if (i == 13)
                {
                    work = hi >> 1;
                }
                byte digit = (byte)(work & mask);
                chars[charIdx] = _charMap[digit];
                charIdx--;
                work = work >> 5;
            }

            var ret = new string(chars, 26 - numDigits, numDigits);
            return ret;
        }
    }
}
