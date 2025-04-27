using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class FileUploadController : BaseController
    {
        #region PRIVATE MEMBERS
        private byte[] GetBytes(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private string GetUploadPath(string sessionId)
        {
            var rootPath = Server.MapPath("~/TempFiles/Uploads/" + sessionId);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            return rootPath;
        }


        #endregion

        // GET: FileUpload
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            var files = Request.Files;
            if (files.Count > 0)
            {
                try
                {
                    string filePath = Path.Combine(GetUploadPath("test"), files[0].FileName);
                    using (FileStream fs = new FileStream(filePath, FileMode.Append))
                    {
                        var bytes = GetBytes(files[0].InputStream);
                        await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                    }
#if DEBUG
                    TempData["xxx"] = Convert.ToInt32(TempData.Peek("xxx")) + 1;
                    int xxx = Convert.ToInt32(TempData.Peek("xxx"));
                    if (xxx >= 50)
                    {
                        TempData["xxx"] = 0;
                        return Json(new { status = false, message = "Something went wrong." });
                    }
#endif
                    return Json(new { status = true });
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.Message });
                }
            }
            return Json(new { status = false, message = "Something went wrong." });
        }
    }
}