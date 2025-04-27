using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels = TWC.IMS.Models;
using System.Diagnostics;

namespace TWC.IMS.Web.Hubs
{

    [Authorize]
    public class SystemNotificationHub : Hub
    {
        private static string _connectionId;
        private string _userId;
        public async Task<bool> NotifyAsync(string userId, DbModels.SystemNotification x)
        {
            try
            {
                using (var signalRBL = new BL.SignalRConnection(userId))
                {
                    var vm = new Models.SystemNotificationViewModel
                    {
                        Id = x.Id,
                        UniqueKey = x.UniqueKey,
                        Url = x.Url,
                        Caption = x.Caption,
                        Created = x.Created,
                        Description = x.Description,
                        IsViewed = x.IsViewed,
                        SeenDate = x.SeenDate,
                        SystemNotification_UserDetail = x.SystemNotification_UserDetail,
                        Title = x.Title
                    };

                    var context = GlobalHost.ConnectionManager.GetHubContext<SystemNotificationHub>();
                    var connections = await signalRBL.GetListAsync(userId).ConfigureAwait(false);
                    foreach (var conObj in connections)
                    {
                        context.Clients.Client(conObj.ConnectionId).notifyUser(vm);
                    }

                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }
            catch (Exception )
            {
                return await Task.FromResult(true).ConfigureAwait(false);
            }
        }


        public async Task<bool> UpdateBadgeNumberAsync(string userId, int num)
        {
            try
            {
                using (var signalRBL = new BL.SignalRConnection(userId))
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<SystemNotificationHub>();
                    var connections = await signalRBL.GetListAsync(userId).ConfigureAwait(false);
                    foreach (var conObj in connections)
                    {
                        context.Clients.Client(conObj.ConnectionId).updateBadgeNumber(num);
                    }

                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return await Task.FromResult(true).ConfigureAwait(false);
            }
        }

        public override Task OnConnected()
        {
            var userId = GetUserId();
            using (var signalRBL = new BL.SignalRConnection(userId))
            {
                _connectionId = Context.ConnectionId;
                Common.HelperClasses.AsyncHelpers.RunSync(() => signalRBL.InsertAsync(new TWC.IMS.Models.SignalRConnection
                {
                    UserId = userId,
                    ConnectionId = _connectionId
                }));
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = GetUserId();
            using (var signalRBL = new BL.SignalRConnection(userId))
            {
                Common.HelperClasses.AsyncHelpers.RunSync(() => signalRBL.DeleteAsync(Context.ConnectionId));
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var userId = GetUserId();
            using (var signalRBL = new BL.SignalRConnection(userId))
            {
                var connections = Common.HelperClasses.AsyncHelpers.RunSync(() => signalRBL.GetListAsync(userId));
                if (!connections.Any(x => x.ConnectionId == Context.ConnectionId))
                {
                    Common.HelperClasses.AsyncHelpers.RunSync(() => signalRBL.InsertAsync(new TWC.IMS.Models.SignalRConnection
                    {
                        UserId = userId,
                        ConnectionId = Context.ConnectionId
                    }));
                }
            }
            return base.OnReconnected();
        }

        public string GetUserId()
        {
            var username = Context.User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {
                if (!string.IsNullOrWhiteSpace(_userId))
                    return _userId;

                var udBL = new BL.UserDetails(username);
                var userObj = Common.HelperClasses.AsyncHelpers.RunSync(() => udBL.GetByUsernameAsync(username));

                _userId = userObj.UserDetail_AspNetUser;
                return _userId;
            }

            return null;
        }

        public static string GetConnectionId()
        {
            return _connectionId;
        }
    }
}
