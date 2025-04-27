
$(function () {
    $('#btnExportToExcel').on('click', function (e) {
        e.preventDefault();
        try {
            let grid = $('#gridDatabaseArchivingLogs').data('kendoGrid');
            grid.saveAsExcel();
        } catch (e) {
            console.log('Export to Excel Error:');
            console.log(e);
        }
    });
});

function gridDateFilter() {
    let date = $('#DateFilter').val();
    return { date: date };
}

function loadGridData() {
    application.grid.refreshGrid($('#gridDatabaseArchivingLogs'));
}

function logggg(e) {
    console.log(e);
    console.log(e.sender);
}