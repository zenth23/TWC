using TWC.IMS.Common;
using TWC.IMS.Models.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DbModels = TWC.IMS.Models;

namespace TWC.IMS.Web.HelperClasses
{
    public class WorkflowEngine
    {
        private string _username;
        private Application _appInstance;
        private string _userRole;
        private bool _isMobileDevice;
        private string _clientIPAddress;
        private string _userAgent;

        public WorkflowEngine(string username, string userRole, bool isMobileDevice, string clientIPAddress, string userAgent)
        {
            _username = username;
            _appInstance = Application.Instance;
            _userRole = userRole;
            _isMobileDevice = isMobileDevice;
            _clientIPAddress = clientIPAddress;
            _userAgent = userAgent;
        }

        // GET WORKFLOW ENTITY FROM WorkflowEditor THEN CONVERT IT TO TRANSACTIONAL APPROVAL ENTITY (for easy saving later on..)
    

        public async Task<int> SendSystemNotificationAsync(Hubs.SystemNotificationHub notifHub, DbModels.SystemNotification notifObj)
        {
            try
            {
                using (var sysNotifBL = new BL.SystemNotifications(_username))
                {
                    var userId = notifObj.UserDetail.UserDetail_AspNetUser;

                    notifObj.UserDetail = null;
                    var id = await sysNotifBL.InsertAsync(notifObj).ConfigureAwait(false);

                    var notifTask = notifHub.NotifyAsync(userId, notifObj);
                    var notifListTask = sysNotifBL.GetByUserAsync(notifObj.SystemNotification_UserDetail);

                    await notifTask.ConfigureAwait(false);
                    var notifList = await notifListTask.ConfigureAwait(false);

                    var newNotifsCount = notifList.GroupBy(x => new { x.Title, x.Caption })
                                                  .Select(x => x.OrderBy(o => o.IsViewed).FirstOrDefault())
                                                  .Count(x => !x.IsViewed);

                    await notifHub.UpdateBadgeNumberAsync(userId, newNotifsCount).ConfigureAwait(false);

                    return id;
                }
            }
            catch (Exception ex)
            {
                var paramData = $"{await notifObj.Describe().ConfigureAwait(false)}";
                var _ = Logger.ErrorAsync(ex.Message, _username, _appInstance.ApplicationVersion, _userRole, _appInstance.Environment, _clientIPAddress, _userAgent, ex, isMobileDevice: _isMobileDevice, paramData: paramData);

                return 0;
            }
        }

    }
}