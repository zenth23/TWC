
$(function () {
    $('#btnExportToExcelEmailLogs').on('click', function (e) {
        e.preventDefault();
        try {
            let grid = $('#gridEmailLogs').data('kendoGrid');
            grid.saveAsExcel();
        } catch (e) {
            console.log('Export to Excel Error:');
            console.log(e);
        }
    });
    //getEmailLogCounters();
});

function gridDateFilterEmailLogs() {
    let date = $('#DateFilterEmailLogs').val();
    return { date: date };
}

function loadGridDataEmailLogs() {
    application.grid.refreshGrid($('#gridEmailLogs'));
    application.chart.refreshChart($('#recipientHitsChart'));
    getEmailLogCounters();
}

function chartDateFilterEmailLogs() {
    let date = $('#DateFilterEmailLogs').val();
    return {
        d: date
    }
}

function getEmailLogCounters() {
    let date = $('#DateFilterEmailLogs').val();
    let url = window.rootUrl + 'AdminDashboardCharts/GetEmailLogCounters';
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'JSON',
        data: { d: date }
    })
    .done((r) => {
        let errorCount = r.TotalErrors;
        let userCount = r.TotalLoggedUsers;
        $('#totalLoggedEmailLogs').text(kendo.format('{0:n0}', errorCount));
        $('#totalLoggedUsersEmailLogs').text(kendo.format('{0:n0}', userCount));
    });
}