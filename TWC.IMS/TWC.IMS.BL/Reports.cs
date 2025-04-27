using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Reporting.WebForms;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Models.HelperModels;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class Reports : IDisposable
    {
        #region PRIVATE MEMBERS
        private string _username;
        private BL.ModuleAccesses _maBL = null;
        private BL.RolePermissions _rpBL = null;
        private BL.RoleDetails _rdBL = null;
        private BL.UserDetails _udBL = null;

        private string GetFileExtension(ReportFileType reportFileType)
        {
            switch (reportFileType)
            {
                case ReportFileType.EXCEL:
                    return ".xls";

                case ReportFileType.EXCELOPENXML:
                    return ".xlsx";

                case ReportFileType.PDF:
                    return ".pdf";

                case ReportFileType.WORD:
                    return ".doc";

                case ReportFileType.WORDOPENXML:
                    return ".docx";

                default:
                    return ".png";
            }
        }

        private byte[] GenerateReport<T>(ReportFileType reportFileType, string reportPath, string reportName, string datasetName, string outputFilename, IEnumerable<T> datasource)
        {
            try
            {
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.AsyncRendering = false;
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.DisplayName = outputFilename;
                reportViewer.LocalReport.ReportPath = string.Format("{0}\\{1}.rdlc", reportPath, reportName);
                reportViewer.LocalReport.DataSources.Clear();
                //reportViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource dataSource = new ReportDataSource(datasetName, datasource);
                reportViewer.LocalReport.DataSources.Add(dataSource);
                
                string deviceInfo = null;
                if (reportFileType == ReportFileType.IMAGE)
                    deviceInfo = "<DeviceInfo><OutputFormat>PNG</OutputFormat></DeviceInfo>";

                return reportViewer.LocalReport.Render(reportFileType.ToString(), deviceInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

        public Reports(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<Tuple<string, byte[]>> GenerateUserAccessMatrixReportAsync(ReportFileType reportFileType, string reportPath)
        {
            string reportName = "UserAccessMatrixReport";
            string datasetName = "dsUserAccessMatrix";
            string fileExtension = this.GetFileExtension(reportFileType);
            string outputFilename = $"etg_projectmold_uam_{Guid.NewGuid()}{fileExtension}";

            using (_rpBL = new RolePermissions(_username))
            {
                var list = await _rpBL.GetListForUserAccessMatrixAsync().ConfigureAwait(false);
                var datasource = list.Select(a => new UserAccessMatrixReportModel
                {
                    UserRole = a.AspNetRole.Name,
                    Module = a.ModuleAccess.Module.Name,
                    Access = a.ModuleAccess.Access.Name
                });
                var fileBytes = this.GenerateReport(reportFileType, reportPath, reportName, datasetName, outputFilename, datasource);
                return Tuple.Create(outputFilename, fileBytes);
            }
        }

        public async Task<Tuple<string, byte[]>> GenerateAccessControlListReportAsync(ReportFileType reportFileType, string reportPath)
        {
            string reportName = "AccessControlListReport";
            string datasetName = "dsAccessControlList";
            string fileExtension = this.GetFileExtension(reportFileType);
            string outputFilename = $"etg_projectmold_acl_{Guid.NewGuid()}{fileExtension}";

            using (_rpBL = new RolePermissions(_username))
            {
                var list = await _rpBL.GetListForAccessControlListAsync().ConfigureAwait(false);
                var fileBytes = this.GenerateReport(reportFileType, reportPath, reportName, datasetName, outputFilename, list);
                return Tuple.Create(outputFilename, fileBytes);
            }
        }

        public async Task<Tuple<string, byte[]>> GenerateRoleMasterdataReportAsync(ReportFileType reportFileType, string reportPath)
        {
            string reportName = "RoleMasterdataReport";
            string datasetName = "dsRoleMasterdata";
            string fileExtension = this.GetFileExtension(reportFileType);
            string outputFilename = $"etg_projectmold_role_{Guid.NewGuid()}{fileExtension}";

            using (_rdBL = new BL.RoleDetails(_username))
            {
                var list = await _rdBL.GetListAsync().ConfigureAwait(false);
                var datasource = list.Select(a => new RoleMasterdataReportModel
                {
                    Name = a.AspNetRole.Name,
                    Description = a.Description,
                    Active = a.IsActive,
                    Administrator = a.IsAdmin
                });
                var fileBytes = this.GenerateReport(reportFileType, reportPath, reportName, datasetName, outputFilename, datasource);
                return Tuple.Create(outputFilename, fileBytes);
            }
        }

        public async Task<Tuple<string, byte[]>> GenerateUserMasterdataReportAsync(ReportFileType reportFileType, string reportPath)
        {
            string reportName = "UserMasterdataReport";
            string datasetName = "dsUserMasterdata";
            string fileExtension = this.GetFileExtension(reportFileType);
            string outputFilename = $"etg_projectmold_user_{Guid.NewGuid()}{fileExtension}";

            using (_udBL = new BL.UserDetails(_username))
            {
                var list = await _udBL.GetListAsync().ConfigureAwait(false);
                var datasource = list.Select(a => new UserMasterdataReportModel
                {
                    ActivationDate = a.ActivationDatetime,
                    DeactivationDate = a.DeactivationDatetime,
                    Email = a.AspNetUser?.Email,
                    ExpirationDate = a.ExpirationDatetime,
                    LastLoginDate = a.LastLoginDatetime,
                    EmployeeId = a.EmployeeId,
                    Name = a.FullName,
                    Status = a.Status,
                    Username = a.AspNetUser?.UserName,
                    UserRole = a.AspNetUser?.AspNetRoles.FirstOrDefault()?.Name
                });
                var fileBytes = this.GenerateReport(reportFileType, reportPath, reportName, datasetName, outputFilename, datasource);
                return Tuple.Create(outputFilename, fileBytes);
            }
        }

        public async Task<Tuple<string, byte[]>> GenerateSystemModulesMasterdataReportAsync(ReportFileType reportFileType, string reportPath)
        {
            string reportName = "SystemModulesMasterdataReport";
            string datasetName = "dsSystemModules";
            string fileExtension = this.GetFileExtension(reportFileType);
            string outputFilename = $"etg_projectmold_systemmodules_{Guid.NewGuid()}{fileExtension}";

            using (_maBL = new BL.ModuleAccesses(_username))
            {
                var list = await _maBL.GetListAsync().ConfigureAwait(false);
                var datasource = list.Select(a => new SystemModulesMasterdataReport
                {
                    Module = a.Module.Name,
                    Access = a.Access.Name
                });
                var fileBytes = this.GenerateReport(reportFileType, reportPath, reportName, datasetName, outputFilename, datasource);
                return Tuple.Create(outputFilename, fileBytes);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_maBL != null)
                    {
                        _maBL.Dispose();
                        _maBL = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Reports() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
