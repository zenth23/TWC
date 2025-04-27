
$(function () {
    application.UI.markRequiredFields();
    application.UI.showValidationSummary();

    _mode = $('#Mode').val();

    $(document).on('click', '#btnReset', function () { application.UI.resetForm(); });

});

var _mode = '';
var _isRTRequired;
var _isMgrRequired;

function checkUsernameAvailability(e) {
    let username = $(e.target).val();
    let reqToken = $('input[name="__RequestVerificationToken"]').val();
    let data = {
        username: username,
        __RequestVerificationToken: reqToken
    };
    $.post(window.rootUrl + 'useraccounts/IsUsernameAvailable',
        data,
        function (response) {
            if (response == true)
                $('#h4IsUsernameAvailable').css('display', 'inline-block');
            else
                $('#h4IsUsernameAvailable').css('display', 'none');
        });
}

function dataUsernameAndSearchKey(e) {
    let filter = e.filter.filters[0];
    let key = typeof filter == 'undefined' ? '' : filter.value;
    let username = $('#AccountModel_UserName').val();
    return { username: username, text: key };
}

function getUserid() {
    let userid = $('#AccountModel_UserId').val();
    return { userid: userid };
}

function getId() {
    let id = $('#Id').val();
    return { id: id };
}

/* ROLE */
function searchKeyRole() {
    let combobox = $('#User_Role').data("kendoComboBox");
    return { searchKey: combobox.text() };
}

function userRoleOnDataBound(e) {

}

function userRoleOnSelect(e) {

}