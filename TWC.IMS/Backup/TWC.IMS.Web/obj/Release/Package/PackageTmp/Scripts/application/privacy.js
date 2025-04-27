
$(function () {
    setTimeout(function () {
        showPrivacyStatement();
    }, 1500);

    $('#btnAccept').on('click', function (e) {
        e.preventDefault();
        let agree = $('#chkAgreement').prop('checked');
        if (agree == true) {
            let reqToken = $('input[name="__RequestVerificationToken"]').val();
            let url = window.rootUrl + 'home/createpscookie';
            $.post(url, { __RequestVerificationToken: reqToken }, function (r) {
                //console.log('createpscookie');
                //console.log(r);
                // close modal
                $('#modalPrivacyStatement').modal('hide');
            });
        }
        else
            alert('You must agree on the privacy statement first.', 'Privacy Statement');
    });
});

function showPrivacyStatement() {
    let reqToken = $('input[name="__RequestVerificationToken"]').val();
    let url = window.rootUrl + 'home/getpscookie';
    $.post(url, { __RequestVerificationToken: reqToken }, function (r) {
        //console.log('getpscookie');
        //console.log(r);
        if (r.Agreed == false)
            $('#modalPrivacyStatement').modal('show');
        else
            $('#modalPrivacyStatement').modal('hide');
    });
}