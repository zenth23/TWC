
$(function () {
    $('#btnExportToExcelUserActivityLogs').on('click', function (e) {
        e.preventDefault();
        try {
            let grid = $('#gridUserActivityLogs').data('kendoGrid');
            grid.saveAsExcel();
        } catch (e) {
            console.log('Export to Excel Error:');
            console.log(e);
        }
    });

    //getUserActivityLogCounters();
});

function gridDateFilterUserActivityLogs() {
    let date = $('#DateFilterUserActivityLogs').val();
    let gridDS = $('#gridUserActivityLogs').data('kendoGrid').dataSource;
    let filter = gridDS.filter();
    let sort = gridDS.sort();
    let page = gridDS.page();
    let pageSize = gridDS.pageSize();
    return {
        date: date,
        filter: filter,
        sort: sort,
        page: page,
        pageSize: pageSize
    };
}

function loadGridDataUserActivityLogs() {
    application.grid.refreshGrid($('#gridUserActivityLogs'));
    application.chart.refreshChart($('#pageHitsChart'));
    getUserActivityLogCounters();
}

function chartDateFilterUserActivityLogs() {
    let date = $('#DateFilterUserActivityLogs').val();
    return {
        d: date
    }
}

function getUserActivityLogCounters() {
    let date = $('#DateFilterUserActivityLogs').val();
    let url = window.rootUrl + 'AdminDashboardCharts/GetUserActivityLogCounters';
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'JSON',
        data: { d: date }
    })
    .done((r) => {
        let errorCount = r.TotalErrors;
        let userCount = r.TotalLoggedUsers;
        $('#totalLoggedUals').text(kendo.format('{0:n0}', errorCount));
        $('#totalLoggedUsersUal').text(kendo.format('{0:n0}', userCount));
    });
}