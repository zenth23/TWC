
$(function () {
    $('a.nav-link').on('show.bs.tab', function (e) {
        // load corresponding grids/charts before activating the tab
        //console.log({ e });
        //console.log(e.currentTarget);
        //console.log($(e.currentTarget).data('module'));

        let type = $(e.currentTarget).data('module');
        switch (type) {
            case 'errorlog':
                if (_modules.indexOf(type) == -1) {
                    loadGridDataErrorLogs();
                }
                break;
            case 'emaillog':
                if (_modules.indexOf(type) == -1) {
                    loadGridDataEmailLogs();
                }
                break;
            case 'auditlog':
                if (_modules.indexOf(type) == -1) {
                    loadGridDataAuditLogs();
                }
                break;
            case 'useractivitylog':
                if (_modules.indexOf(type) == -1) {
                    loadGridDataUserActivityLogs();
                }
                break;
        }
        if (_modules.indexOf(type) == -1) {
            _modules.push(type);
        }
    });

    getLogCounters();
});

var _modules = [];

function viewDetails(e) {
    e.preventDefault();

    let $obj = $(e.currentTarget);
    let controllerName = $obj.attr('data-cname');
    let route = $obj.attr('data-route');
    let propName = $obj.attr('data-propname');
    let title = $obj.attr('title');

    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let key = dataItem[propName];

    let delay = 0;
    let url = window.rootUrl + controllerName + '/details';
    $.ajax({
        url: url,
        data: {
            key: key,
            isPartial: true
        },
        method: 'GET',
        beforeSend: () => {
            delay = setTimeout(() => {
                $('#systemSpinner').modal('show');
            }, 500);
        }
    })
    .done((r) => {
        $('#detailsModalBody').html(r);
        $('#detailsModalTitle').text(title);
        $('#detailsModal').modal('show');
    })
    .fail((x, s, e) => {
        console.log({ x });
        console.log({ s });
        console.log({ e });

        alert('Something went wrong.', 'Oops!');
    })
    .always(() => {
        $('#systemSpinner').modal('hide');
        clearTimeout(delay);
    });
}

function loadChartsData() {
    //$('#systemSpinner').modal('show');
    application.grid.refreshGrid($('#gridUserRelatedMetrics'));
    application.chart.refreshChart($('#errorFrequencyChart'));
    application.chart.refreshChart($('#errorDistributionPieChart'));
    application.chart.refreshChart($('#versionDistributionDonutChart'));
    //application.chart.refreshChart($('#errorRateChart'));
    application.chart.refreshChart($('#errorRateChartCvp'));

    // sparkline is not a chart
    let $trend = $('#errorTrendChart').data('kendoSparkline');
    $trend.dataSource.read();

    getLogCounters();

    //setTimeout(() => {
    //    $('#systemSpinner').modal('hide');
    //}, 500);
}

function chartDateFilter() {
    let date = $('#DateFilterCharts').val();
    return {
        d: date
    }
}

function getLogCounters() {
    let date = $('#DateFilterCharts').val();
    let url = window.rootUrl + 'AdminDashboardCharts/GetLogCounters';
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'JSON',
        data: { d: date }
    })
    .done((r) => {
        let errorCount = r.TotalErrors;
        let userCount = r.TotalLoggedUsers;
        $('#totalLoggedErrors').text(kendo.format('{0:n0}', errorCount));
        $('#totalLoggedUsers').text(kendo.format('{0:n0}', userCount));

        let ecm = r.ErrorCountByMethodList;
        if (ecm.length > 0) {
            let ebm1_name = ecm[0].MethodName;
            let ebm1_count = ecm[0].ErrorCount;
            $('#errorByMethod_name_1').text(ebm1_name).attr('title', ebm1_name);
            $('#errorByMethod_count_1').text(ebm1_count);
        }
        else {
            $('#errorByMethod_name_1').text('-').attr('title', '-');
            $('#errorByMethod_count_1').text('0');
        }

        if (ecm.length > 1) {
            let ebm2_name = ecm[1].MethodName;
            let ebm2_count = ecm[1].ErrorCount;
            $('#errorByMethod_name_2').text(ebm2_name).attr('title', ebm2_name);
            $('#errorByMethod_count_2').text(ebm2_count);
        }
        else {
            $('#errorByMethod_name_2').text('-').attr('title', '-');
            $('#errorByMethod_count_2').text('0');
        }

        if (ecm.length > 2) {
            let ebm3_name = ecm[2].MethodName;
            let ebm3_count = ecm[2].ErrorCount;
            $('#errorByMethod_name_3').text(ebm3_name).attr('title', ebm3_name);
            $('#errorByMethod_count_3').text(ebm3_count);
        }
        else {
            $('#errorByMethod_name_3').text('-').attr('title', '-');
            $('#errorByMethod_count_3').text('0');
        }

        if (ecm.length > 3) {
            let ebm4_name = ecm[3].MethodName;
            let ebm4_count = ecm[3].ErrorCount;
            $('#errorByMethod_name_4').text(ebm4_name).attr('title', ebm4_name);
            $('#errorByMethod_count_4').text(ebm4_count);
        }
        else {
            $('#errorByMethod_name_4').text('-').attr('title', '-');
            $('#errorByMethod_count_4').text('0');
        }
    });
}