
$(function () {
    $('#modalSsessionExpiredWarning').modal('hide');

    //_clientAlive = 0;
    localStorage.setItem(_twcims_ksa_clientAlive, 0);

    checkAlive();
    activeBeat();
    heartBeat();

    $('#btnSessionExpiredOK').on('click', () => {
        $('#modalSsessionExpiredWarning').modal('hide');
        pingServer('hbeat');
        //_hold = 0;
        localStorage.setItem(_twcims_ksa_hold, 0);
    });
});

// TODO: change these values per project
var _twcims_ksa_renew = "_twcims_ksa_renew";
var _twcims_ksa_hold = "_twcims_ksa_hold";
var _twcims_ksa_clientAlive = "_twcims_ksa_clientAlive";
//--------------------------------------------------

var _timeout = (_sessionTimeout - 1) * 60 * 1000; // in ms
var _beatTime = 1 * 60 * 1000; // 1 min, in ms
var _sessionLogoffUrl = window.rootUrl + "account/forcesignout";
var _sessionUrl = window.rootUrl + "home/keepsessionalive";
var _renew = 0;
var _hold = 0;
var _counter = 30; // in seconds
var _hBeat;
var _abeat;
var _popTimer;
var _clientAlive = 0;

localStorage.setItem(_twcims_ksa_renew, _renew);
localStorage.setItem(_twcims_ksa_hold, _hold);

function heartBeat() {
    _hBeat = setTimeout(() => {
        //console.log('heartBeat');
        _clientAlive = localStorage.getItem(_twcims_ksa_clientAlive);
        if (_clientAlive == 0) {
            //_renew = 0;
            //_hold = 1;
            localStorage.setItem(_twcims_ksa_renew, 0);
            localStorage.setItem(_twcims_ksa_hold, 1);

            countdownDisplay();

            $('#modalSsessionExpiredWarning').modal('show');

            setTimeout(() => {
                _renew = localStorage.getItem(_twcims_ksa_renew);
                if (_renew == 0 || _renew == undefined) {
                    $.get(_sessionLogoffUrl, () => {
                        window.location.reload(true);
                    });
                }
                $('#modalSsessionExpiredWarning').modal('hide');

            }, _counter * 1000);
        }
        else {
            pingServer('forcedbeat');
        }
    }, _timeout);
}

function activeBeat() {
    _abeat = setTimeout(() => {
        //console.log('activeBeat');
        _clientAlive = localStorage.getItem(_twcims_ksa_clientAlive);
        if (_clientAlive == 1) {
            pingServer('abeat');
        }
        else {
            clearTimeout(_abeat);
            activeBeat();
        }
    }, _beatTime);
}

function checkAlive() {
    //console.log('checkAlive');
    $('body').on('mousemove keydown', () => {
        _hold = localStorage.getItem(_twcims_ksa_hold);
        if (_hold == 0) {
            //_clientAlive = 1;
            localStorage.setItem(_twcims_ksa_clientAlive, 1);

            //_renew = 1;
            localStorage.setItem(_twcims_ksa_renew, 1);
        }
    });
}

function pingServer(src) {
    //console.log('pingServer: ' + src);
    $.ajax({
        type: 'POST',
        url: _sessionUrl,
        success: (data) => {

        },
        error: (data) => {
            alert('Unable to connect to server. Please contact your system administrator.', 'Session Timeout');
            console.log('Error posting to ' + _sessionUrl);
        }
    })
    .always(() => {
        reset();
    });
}

function reset() {
    clearTimeout(_abeat);
    clearTimeout(_hBeat);
    clearTimeout(_popTimer);

    //_clientAlive = 0;
    localStorage.setItem(_twcims_ksa_clientAlive, 0);

    heartBeat();
    activeBeat();
    checkAlive();

    //_renew = 1;
    localStorage.setItem(_twcims_ksa_renew, 1);
}

function countdownDisplay() {
    //console.log('countdownDisplay');
    let dialogDisplaySeconds = _counter;
    _popTimer = setInterval(() => {
        $('#seconds-timer').html(dialogDisplaySeconds);
        if (dialogDisplaySeconds > 0)
            dialogDisplaySeconds -= 1;
    }, 1000);
}