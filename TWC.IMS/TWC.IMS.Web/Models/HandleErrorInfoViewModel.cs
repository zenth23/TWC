using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.HelperClasses
{
    [Serializable]
    public class HandleErrorInfoViewModel : HandleErrorInfo
    {
        private string _errorHeader;
        private const string _OOPS_IMAGES_PATH_NAME = "OOPS_IMAGES_PATH";
        private const string _OOPS_DEFAULT_IMAGE = "oops-1024x683_1.jpg";
        private const string _OOPS_DEFAULT_HEADER_NAME = "Oops...";
        private const string _OOPS_DEFAULT_IMAGE_FILE_EXT = ".jpg";

        public string OopsErrorImage { get; set; } // since this is public, changed from field to property to avoid FxCop violation        

        public HandleErrorInfoViewModel(Exception exception, string controllerName, string actionName, string errorHeader, string oopsImagesPath)
        : base(exception, controllerName, actionName)
        {
            if (string.IsNullOrWhiteSpace(errorHeader))
                _errorHeader = _OOPS_DEFAULT_HEADER_NAME;   // worst case
            else
                _errorHeader = errorHeader;

            #region set oops image
            if (!string.IsNullOrEmpty(oopsImagesPath))
            {
                string[] files = (HttpContext.Current.Cache[_OOPS_IMAGES_PATH_NAME]) as string[];
                int fileCount = 0;
                if (files == null)
                {
                    files = Directory.GetFiles(oopsImagesPath, $"*{_OOPS_DEFAULT_IMAGE_FILE_EXT}", SearchOption.TopDirectoryOnly);
                    HttpContext.Current.Cache[_OOPS_IMAGES_PATH_NAME] = files;
                }
                fileCount = files.Length;

                int maxRetries = 3;
                for (int i = 0; i < maxRetries; i++)
                {
                    var random = new Random();
                    int randomNo = random.Next(1, fileCount);

                    var filename = files.Select(a => new FileInfo(a)).FirstOrDefault(a => a.Name.EndsWith($"_{randomNo}{_OOPS_DEFAULT_IMAGE_FILE_EXT}"));
                    if (filename != null)
                    {
                        OopsErrorImage = filename.Name;
                        if (filename.Exists)
                            break;
                    }
                }
            }
            else
                OopsErrorImage = _OOPS_DEFAULT_IMAGE;
            #endregion
        }

        public string ErrorHeader
        {
            get
            {
                return this._errorHeader;
            }

            set
            {
                value = this._errorHeader;
            }
        }
    }
}