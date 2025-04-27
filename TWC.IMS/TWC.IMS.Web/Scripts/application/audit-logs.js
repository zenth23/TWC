
$(function () {
    $('#btnExportToExcelAuditLogs').on('click', function (e) {
        e.preventDefault();
        try {
            let grid = $('#gridAuditLogs').data('kendoGrid');
            grid.saveAsExcel();
        } catch (e) {
            console.log('Export to Excel Error:');
            console.log(e);
        }
    });

    //getAuditLogCounters();
});

function gridDateFilterAuditLogs() {
    let date = $('#DateFilterAuditLogs').val();
    return { date: date };
}

function loadGridDataAuditLogs() {
    application.grid.refreshGrid($('#gridAuditLogs'));
    application.chart.refreshChart($('#tableHitsChart'));
    getAuditLogCounters();
}

function chartDateFilterAuditLogs() {
    let date = $('#DateFilterAuditLogs').val();
    return {
        d: date
    }
}

function getAuditLogCounters() {
    let date = $('#DateFilterAuditLogs').val();
    let url = window.rootUrl + 'AdminDashboardCharts/GetAuditLogCounters';
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'JSON',
        data: { d: date }
    })
    .done((r) => {
        let errorCount = r.TotalErrors;
        let userCount = r.TotalLoggedUsers;
        $('#totalLoggedAL').text(kendo.format('{0:n0}', errorCount));
        $('#totalLoggedUsersAL').text(kendo.format('{0:n0}', userCount));
    });
}