
$(function () {
    $('#btnExportToExcelErrorLogs').on('click', function (e) {
        e.preventDefault();
        try {
            let grid = $('#gridErrorLogs').data('kendoGrid');
            grid.saveAsExcel();
        } catch (e) {
            console.log('Export to Excel Error:');
            console.log(e);
        }
    });
    
    //getErrorLogCounters();
});

function gridDateFilterErrorLogs() {
    let date = $('#DateFilterErrorLogs').val();
    return { date: date };
}

function loadGridDataErrorLogs() {
    application.grid.refreshGrid($('#gridErrorLogs'));
    application.chart.refreshChart($('#moduleHitsChart'));
    getErrorLogCounters();
}

function chartDateFilterErrorLogs() {
    let date = $('#DateFilterErrorLogs').val();
    return {
        d: date
    }
}

function getErrorLogCounters() {
    let date = $('#DateFilterErrorLogs').val();
    let url = window.rootUrl + 'AdminDashboardCharts/GetErrorLogCounters';
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'JSON',
        data: { d: date }
    })
    .done((r) => {
        let errorCount = r.TotalErrors;
        let userCount = r.TotalLoggedUsers;
        $('#totalLoggedEL').text(kendo.format('{0:n0}', errorCount));
        $('#totalLoggedUsersEL').text(kendo.format('{0:n0}', userCount));
    });
}