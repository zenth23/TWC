
$(function () {
    setTimeout(function () {
        showCookieNotice();
    }, 500);

    $('#cookieNoticeClose').on('click', function (e) {
        $('#divCookieNotice').slideUp('slow');
    });

    $('#btnAcceptCookie').on('click', function (e) {
        e.preventDefault();
        let reqToken = $('input[name="__RequestVerificationToken"]').val();
        let url = window.rootUrl + 'home/createcncookie';
        $.post(url, { __RequestVerificationToken: reqToken }, function (r) {
            //console.log('createcncookie');
            //console.log(r);
            $('#divCookieNotice').hide();
        });
    });
});

function showCookieNotice() {
    let reqToken = $('input[name="__RequestVerificationToken"]').val();
    let url = window.rootUrl + 'home/getcncookie';
    $.post(url, { __RequestVerificationToken: reqToken }, function (r) {
        //console.log('getcncookie');
        //console.log(r);
        if (r.Agreed == false)
            $('#divCookieNotice').slideDown('slow');
        else
            $('#divCookieNotice').hide();
    });
}