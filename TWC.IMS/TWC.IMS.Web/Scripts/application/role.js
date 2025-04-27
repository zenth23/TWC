
$(function () {
    application.UI.markRequiredFields();
    application.UI.showValidationSummary();
    //$('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover({
        trigger: 'focus'
    });

    $(document).on('click', '#btnReset', function () { application.UI.resetForm(); });

    $('#IsAdmin').on('click', function () {

    });
});

var _indexes = [];

function searchKeyRole() {
    let combobox = $('#User_Role').data("kendoComboBox");
    return { searchKey: combobox.text() };
}

function getRoleId() {
    let role = $('#Id').val();
    return { roleid: role };
}

function getIndex() {
    let c = _indexes[_indexes.length - 1];
    if (typeof c == 'undefined')
        c = -1; // initial

    let i = c + 1;
    _indexes.push(i);
    //console.log(_indexes);
    return i;
}

function removeUserFromRole(e) {
    e.preventDefault();
    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let uid = dataItem.UserId;
    let roleName = dataItem.RoleName;
    let name = dataItem.UserName;
    let reqToken = $('input[name="__RequestVerificationToken"]').val();

    window.confirmDelete("Are you sure that you want to remove user '" + name + "' from role?")
          .then(function () {
              let url = window.rootUrl + 'maintenance/removeUserFromRole';
              $.ajax({
                  url: url,
                  data: { uid: uid, role: roleName, __RequestVerificationToken: reqToken },
                  type: 'POST',
                  dataType: 'json',
                  beforeSend: function () { },
                  success: function (response) {
                      application.grid.refreshGrid($('#gridRoleUser'));
                      let r = response;
                      window.alert(r.Message, r.Status);
                  },
                  error: function (x, t, e) {
                      console.log('error');
                      console.log(e);
                      let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                      window.alert(msg, t);
                  }
              });
          }, function () {
              //kendo.alert("You chose to Cancel action.");
          });
}

function searchKeyTeamLead() {
    let combobox = $('#Hierarchy_RoleHierarchy_RoleReportsTo').data("kendoComboBox"),
        roleId = $('#RoleId').val();

    return {
        searchKey: combobox.text(),
        roleId: roleId
    };
}

function searchKeyManager() {
    let combobox = $('#Hierarchy_RoleHierarchy_RoleManager').data("kendoComboBox"),
        roleId = $('#RoleId').val(),
        teamLead = $('#Hierarchy_RoleHierarchy_RoleReportsTo').data('kendoComboBox'),
        teamLeadId = teamLead.value();

    return {
        searchKey: combobox.text(),
        roleId: roleId,
        teamLeadId: teamLeadId
    };
}