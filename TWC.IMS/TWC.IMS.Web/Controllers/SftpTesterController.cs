using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class SftpTesterController : BaseController
    {
        [NonAction]
        // GET: SftpTester
        public async Task<ActionResult> Index()
        {
            //var bbb = new TWC.IMS.Common.SftpHelper.Sftp();
            //await bbb.InitAsync().ConfigureAwait(false);
            //await bbb.UploadFileAsync("", null, "", true).ConfigureAwait(false);

            string fileName = Server.MapPath("~/content/images/logo/SQL Cookbook - Anthony Molinaro_177.pdf");
            //var file = System.IO.File.ReadAllBytes(filePath);
            string fname = System.IO.Path.GetFileName(fileName);
            //string destinationFileName = $"{Guid.NewGuid().ToString().ToLower()}_{fileName}";
            //var obj = await TWC.IMS.Common.SftpHelper.SftpFactory.SftpA();
            //var aaa = await obj.UploadFileAsync("/QAS/inbound/GBS/TEST/", file, fileName, destinationFileName, true).ConfigureAwait(false);

            var result = await TWC.IMS.Common.FileStreamUpload.UploadFileChunkAsync(fileName, @"C:\Temp\LargeFileUpload\" + fname, replaceDestinationFileIfExists: false, transferDelayInMS: 1000).ConfigureAwait(false);

            return View();
        }
    }
}