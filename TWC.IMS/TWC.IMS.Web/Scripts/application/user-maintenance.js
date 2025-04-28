
$(function () {
    application.activateTabViaUrl(window.location.href);

    // refresh grid on tab change
    // to fix grid freeze pane distortion
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        let targetGrid = $(e.target).attr("data-grid") // activated tab
        application.grid.refreshGrid($('#' + targetGrid));
    });
});

function resetPassword(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var name = dataItem.AccountModel.UserName;
    var userId = dataItem.AccountModel.UserId;
    var reqToken = $('input[name="__RequestVerificationToken"]').val();

    window.confirm('Confirm', "Click OK to proceed with password reset for user '" + name + "'.")
            .then(function () {
                var url = window.rootUrl + 'account/forcepasswordreset';
                $.ajax({
                    url: url,
                    data: { userId: userId, __RequestVerificationToken: reqToken },
                    type: 'POST',
                    dataType: 'json',
                    beforeSend: function () { },
                    success: function (response) {
                        application.grid.refreshGrid($('#gridUsers'));
                        var r = response;
                        window.alert(r.Message, r.Status);
                    },
                    error: function (x, t, e) {
                        console.log('error');
                        console.log(e);
                        if (x.responseText.indexOf('<!') == 0) {
                            var newDoc = document.open("text/html", "replace");
                            newDoc.write(x.responseText);
                            newDoc.close();
                        }
                        else {
                            var msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                            window.alert(msg, t);
                        }
                    }
                });
            }, function () {

            });
}

function lock(e) {
    e.preventDefault();

    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let name = dataItem.AccountModel.UserName;
    let userId = dataItem.AccountModel.UserId;
    let reqToken = $('input[name="__RequestVerificationToken"]').val();

    window.confirm('Confirm', "Click OK to lock user '" + name + "'.")
          .then(function () {
              let url = window.rootUrl + 'maintenance/lockuser';
              $.ajax({
                  url: url,
                  data: { userId: userId, username: name, __RequestVerificationToken: reqToken },
                  type: 'POST',
                  dataType: 'json',
                  beforeSend: function () { },
                  success: function (response) {
                      application.grid.refreshGrid($('#gridUsers'));
                      let r = response;
                      window.alert(r.Message, r.Status);
                  },
                  error: function (x, t, e) {
                      console.log('error');
                      console.log(e);
                      if (x.responseText.indexOf('<!') == 0) {
                          let newDoc = document.open("text/html", "replace");
                          newDoc.write(x.responseText);
                          newDoc.close();
                      }
                      else {
                          let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                          window.alert(msg, t);
                      }
                  }
              });
          }, function () {

          });
}

function unlock(e) {
    e.preventDefault();

    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let name = dataItem.AccountModel.UserName;
    let userId = dataItem.AccountModel.UserId;
    let reqToken = $('input[name="__RequestVerificationToken"]').val();

    window.confirm('Confirm', "Click OK to unlock user '" + name + "'.")
        .then(function () {
            let url = window.rootUrl + 'usermaintenance/unlockuser';
            $.ajax({
                url: url,
                data: { userId: userId, username: name, __RequestVerificationToken: reqToken },
                type: 'POST',
                dataType: 'json',
                beforeSend: function () { },
                success: function (response) {
                    application.grid.refreshGrid($('#gridUsers'));
                    let r = response;
                    window.alert(r.Message, r.Status);
                },
                error: function (x, t, e) {
                    console.log('error');
                    console.log(e);
                    if (x.responseText.indexOf('<!') == 0) {
                        let newDoc = document.open("text/html", "replace");
                        newDoc.write(x.responseText);
                        newDoc.close();
                    }
                    else {
                        let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                        window.alert(msg, t);
                    }
                }
            });
        }, function () {

        });
}

function editItemUser(e) {
    e.preventDefault();

    let $obj = $(e.currentTarget);
    let controllerName = $obj.attr('data-cname');
    let route = $obj.attr('data-route');

    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let name = dataItem.AccountModel.UserName;
    let key = dataItem.UniqueKey;
    let url = '';
    if (route == '' || typeof route == 'undefined')
        url = window.rootUrl + controllerName + '/details/' + key;
    else
        url = window.rootUrl + controllerName + '/details/' + route + '/' + key;

    window.location = url;
}

function deleteItemUser(e) {
    e.preventDefault();

    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let key = dataItem.UniqueKey;
    let name = dataItem.AccountModel.UserName;
    let reqToken = $('input[name="__RequestVerificationToken"]').val();

    window.confirmDelete("Are you sure that you want to delete '" + name + "'?")
          .then(function () {
              let url = window.rootUrl + 'maintenance/deleteuser';
              $.ajax({
                  url: url,
                  data: { key: key, username: name, __RequestVerificationToken: reqToken },
                  type: 'POST',
                  dataType: 'json',
                  beforeSend: function () { },
                  success: function (response) {
                      application.grid.refreshGrid($('#gridUsers'));
                      let r = response;
                      window.alert(r.Message, r.Status);
                  },
                  error: function (x, t, e) {
                      console.log('error');
                      console.log(e);
                      if (x.responseText.indexOf('<!') == 0) {
                          let newDoc = document.open("text/html", "replace");
                          newDoc.write(x.responseText);
                          newDoc.close();
                      }
                      else {
                          let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                          window.alert(msg, t);
                      }
                  }
              });
          }, function () {
              //kendo.alert("You chose to Cancel action.");
          });
}

function deleteItemRole(e) {
    e.preventDefault();

    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let id = dataItem.Id;
    let name = dataItem.Name;
    let reqToken = $('input[name="__RequestVerificationToken"]').val();

    window.confirmDelete("Are you sure that you want to delete '" + name + "'?")
          .then(function () {
              let url = window.rootUrl + 'maintenance/deleterole';
              $.ajax({
                  url: url,
                  data: { id: id, name: name, __RequestVerificationToken: reqToken },
                  type: 'POST',
                  dataType: 'json',
                  beforeSend: function () { },
                  success: function (response) {
                      application.grid.refreshGrid($('#gridRoles'));
                      let r = response;
                      window.alert(r.Message, r.Status);
                  },
                  error: function (x, t, e) {
                      console.log('error');
                      console.log(e);
                      if (x.responseText.indexOf('<!') == 0) {
                          let newDoc = document.open("text/html", "replace");
                          newDoc.write(x.responseText);
                          newDoc.close();
                      }
                      else {
                          let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                          window.alert(msg, t);
                      }
                  }
              });
          }, function () {
              //kendo.alert("You chose to Cancel action.");
          });
}

function gridUsersOnDataBound(e) {
    let grid = this;
    grid.tbody.find("tr[role='row']").each(function () {
        let model = grid.dataItem(this);
        let lockoutdate = model.AccountModel.LockoutEndDate;
        let loDate = kendo.parseDate(lockoutdate);
        let today = new Date();

        if (lockoutdate == '' || lockoutdate == null || lockoutdate == undefined || loDate < today) {
            if (model.AccountModel.Status == 'Locked') {
                $(this).find(".k-grid-Unlock").removeClass("k-state-disabled").prop("disabled", false);
                $(this).find(".k-grid-Lock").addClass("k-state-disabled").prop("disabled", true);
            } else {
                $(this).find(".k-grid-Unlock").addClass("k-state-disabled").prop("disabled", true);
                $(this).find(".k-grid-Lock").removeClass("k-state-disabled").prop("disabled", false);
            }
        }
        else {
            $(this).find(".k-grid-Lock").addClass("k-state-disabled").prop("disabled", true);
            $(this).find(".k-grid-Unlock").removeClass("k-state-disabled").prop("disabled", false);
        }
    });
}


