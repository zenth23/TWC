

$(function () {
    $(document).on('click', '#btnCheckConfigs', (e) => {
        e.preventDefault();
        $('#modalConfigsList').modal('show');
    });
            
    $(document).on('click', '#btnRefreshChecklistGrid', (e) => {
        e.preventDefault();
        application.grid.refreshGrid($('#gridConfigsList'));
    });
});