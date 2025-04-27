
$(function () {
    application.UI.markRequiredFields();
    application.activateTabViaUrl(window.location.href);

    $(document).on('click', '#btnAvatarUpload', function (e) {
        e.preventDefault();
        // clear upload files list
        $('.k-upload-files.k-reset').find('li').remove();
        // remove upload 'done' status
        $('.k-upload-status.k-upload-status-total').remove();
        // then show modal
        $('#avatarUploadModal').modal('show');
    });

    $(document).on('click', '#btnVerifyEmail', (e) => {
        e.preventDefault();
        if (!_emailHasChanged) {
            let $email = $('#Email');
            if ($email.valid()) {
                let url = window.rootUrl + 'manage/VerifyEmailAddress';
                let reqToken = $('input[name="__RequestVerificationToken"]').val();
                $.post(url, {
                    __RequestVerificationToken: reqToken
                }, (response) => {
                    alert(response, 'Email Verification Sent');
                });
            }
        }
        else
            alert('To use this new email address, you need to save your changes first.', 'Email Has Changed')
    });

    $(document).on('click', '#btnVerifyPhone', (e) => {
        e.preventDefault();
        let $pn = $('#PhoneNumber');
        if ($pn.valid()) {
            let pn = $pn.val();
            let url = window.rootUrl + 'manage/VerifyPhoneNumber?pn=' + btoa(pn);
            window.location = url;
        }
    });

    $(document).on('click', '#TwoFactorSwitch', (e) => {
        //$('#TwoFactorForm').submit();
        e.preventDefault();
        let isChecked = $('#TwoFactorSwitch').prop('checked');
        let url = window.rootUrl;
        let reqToken = $('input[name="__RequestVerificationToken"]').val();

        console.log({ isChecked });
        if (isChecked) {
            url += 'manage/EnableTwoFactorAuthentication';
            $.post(url, { __RequestVerificationToken: reqToken }).always(() => {
                window.location = window.rootUrl + 'manage/profile#account';
            });
        }
        else {
            url += 'manage/sendcode';
            window.location = url;
        }
    });

    $(document).on('click', '#GoogleAuthSwitch', (e) => {
        e.preventDefault();
        let isChecked = $('#GoogleAuthSwitch').prop('checked');
        let url = window.rootUrl;
        let reqToken = $('input[name="__RequestVerificationToken"]').val();

        console.log({ isChecked });
        if (isChecked) {
            url += 'manage/EnableGoogleAuthenticator';
            //window.location = url;
        }
        else {
            url += 'manage/DisableGoogleAuthenticator';
            //$.post(url, { __RequestVerificationToken: reqToken }).always(() => {
            //    window.location.reload(true);
            //});
        }
        window.location = url;
    });

    $(document).on('click', '#MicrosoftAuthSwitch', (e) => {
        e.preventDefault();
        let isChecked = $('#MicrosoftAuthSwitch').prop('checked');
        let url = window.rootUrl;
        let reqToken = $('input[name="__RequestVerificationToken"]').val();

        console.log({ isChecked });
        if (isChecked) {
            url += 'manage/EnableMicrosoftAuthenticator';
        }
        else {
            url += 'manage/DisableMicrosoftAuthenticator';
            //$.post(url, { __RequestVerificationToken: reqToken }).always(() => {
            //    window.location.reload(true);
            //});
        }
        window.location = url;
    });

    $(document).on('keyup', '#Email', (e) => {
        let origEmail = $('#OrigEmail').val();
        let newEmail = $('#Email').val();
        if (origEmail.toLowerCase() !== newEmail.toLowerCase()) {
            _emailHasChanged = true;
        }
        else {
            _emailHasChanged = false;
        }
    });

    loadAvatar();
});

var _emailHasChanged = false;

function loadAvatar() {
    var url = window.rootUrl + 'manage/GetAvatarBase64String';
    $.get(url, function (response) {
        $('#avatar-holder').html('<img class="avatar" src="' + response + '" />');
    });
}

function updateAvatarDisplayImage(e) {
    loadAvatar();
}

function onAvatarUploadError(e) {
    var err = e.XMLHttpRequest.responseText;
    alert(err);
}

function switchTab(tabId) {
    $('#profileTabs a[href="#' + tabId + '"]').tab('show');
}