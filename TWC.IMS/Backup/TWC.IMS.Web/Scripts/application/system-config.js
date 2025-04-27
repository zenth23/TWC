

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

function gridStatusObjectsConfigOnError(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message, 'ERROR');
    }
}

function gridStatusObjectsConfigOnSync(e) {
    this.read();
}