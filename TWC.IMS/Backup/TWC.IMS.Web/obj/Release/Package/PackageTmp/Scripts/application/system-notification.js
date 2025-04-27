// hub
$.connection.hub.start().done(function () {
    // console.log("SignalR Connected");
});

var notifhub = $.connection.systemNotificationHub;
notifhub.client.notifyUser = function (item) {

    $("#topNotif").find("div.no-notif").remove();

    $("#topNotif").find("div[notif-title='"+item.Title+"'][notif-caption='"+item.Caption+"']").remove();

    var template = '<div class="dropdown-item" style="padding: 0px" notif-id="{item.Id}" notif-title="{item.Title}" notif-caption="{item.Caption}" onclick="systemNotif.events.notificationOnClick(this)" notif-isviewed="0" notif-url="{item.Url}"><div class="card notif-card bg-highlight"><div class="card-body notif-card-body"><h5 class=card-title text-primary">{item.Title}</h5><p class="card-text"><span class="font-weight-bolder" >{item.Caption}</span> <br />{item.Description}</p><p class="card-text text-right text-gray">{item.StrCreatedOn}</p></div></div></div>';

    var notif = template.replace(/{item.Title}/g, item.Title);
    notif = notif.replace(/{item.Caption}/g, item.Caption);
    notif = notif.replace(/{item.Id}/g, item.Id);
    notif = notif.replace(/{item.Description}/g, item.Description);
    notif = notif.replace(/{item.Url}/g, item.Url);
    notif = notif.replace(/{item.StrCreatedOn}/g, item.StrCreatedOn);
    
    $("#topNotif").prepend(notif);

   
};

notifhub.client.updateBadgeNumber = function (badgeNumber) {
    if(badgeNumber > 0) {
        $("#notif-badge").html(badgeNumber);
        $("#notif-badge").removeAttr("style");
    } else {

    }
}

systemNotif = {
    initialize: function() {
        var topNotif = $("#topNotif");
        topNotif.html('<div class="text-center"><div class="spinner-grow spinner-grow-sm text-primary"></div></div>')
        var url = window.rootUrl + "common/systemnotifications";
        $.get(url)
        .done(function(partialView) {
            
            topNotif.html(partialView);
        }).fail(function() {
            topNotif.html("")
        })
    },
    events: {
        bellOnClick: function(elem) {
            $("#notif-badge").attr("style", "display:none !important")
        },
        notificationOnClick: function(elem) {
            var url = $(elem).attr("notif-url");
            if(url !== "#")
                $('#systemSpinner').show();

            var isViewed = $(elem).attr("notif-isviewed") === "1";
            if(!isViewed) {
                var data = {
                    Id: $(elem).attr("notif-id"),
                    __RequestVerificationToken: $(elem).closest("form").find('input[name="__RequestVerificationToken"]').val()
                };

                systemNotif.services
                        .seen(data)
                        .done(function() {
                            if(url !== "#")
                                window.location = window.rootUrl + url
                        })
            } else {
                if(url !== "#")
                    window.location = window.rootUrl + url
            }
        }
    },
    services: {
        seen: function(data) {
            var url = window.rootUrl + "common/SystemNotificationSeen"
            return $.post(url, data);
        }
    }
}

$(function() {
    systemNotif.initialize();
})


