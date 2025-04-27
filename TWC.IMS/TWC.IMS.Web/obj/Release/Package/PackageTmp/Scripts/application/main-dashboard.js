$(function () {
    totalSalesReport();
    totalAverageRevenuePerUnit();
    createBillingProgressChart();
    
    //Download Multiple
    $(document).on("click", "#btnDownloadMultiple", function (e) {
        e.preventDefault();
        let reqToken = $('input[name="__RequestVerificationToken"]').val();

        let fullyApprovedList = getFullyApprovedList();
        let arr = fullyApprovedList.arr;
        let message = fullyApprovedList.message;

        if (arr.length == 0) {
            alert("Please select one or more billing request");
            return false;
        }

        confirm("Confirm Download", `Download the following request/s? (${arr.length} items)<br> ${message} `)
        .done(function () {
            let data = {
                __RequestVerificationToken: reqToken,
                billingRequests: arr
            }
            downloadMultipleBillingRequest(data);
        });
    });

    $(document).on("click", ".card-click", function () {
        let val = this.getAttribute('value');
        if (val) {
            if (filter.status == val) {
                filter.status = "";
                removeGridFilter();
                $(this).removeClass("shadow");
            } else {
                filter.status = val;
                $(".card-click").removeClass("shadow");
                $(this).addClass("shadow");
                filterGridResults();
            }
        }
    });    

    $(document).on("hide.bs.modal", "#remarksModal", function () {
        $("span[data-valmsg-for='Remarks']").text("");
        $("#Remarks").val(null);
    });

    $('#Remarks').on("input", function () {
        let maxlength = $(this).attr("maxlength");
        let currentLength = $(this).val().length;

        if (currentLength >= maxlength) {
            $("span[data-valmsg-for='Remarks']").text(`Maximum number of characters is ${maxlength}.`);
            $("#btnVoidMultiple").attr('disabled', true);
            $("#btnReject").attr('disabled', true);
        } else {
            //console.log(maxlength - currentLength + " chars left");
            $("span[data-valmsg-for='Remarks']").text("");
            $("#btnVoidMultiple").attr('disabled', false);
        }
    });

    $(document).on("click", "#btnVoidMultiple", function (e) {
        e.preventDefault();
        let reqToken = $('input[name="__RequestVerificationToken"]').val();

        let remarks = $("#Remarks").val();

        if (remarks)
            remarks = $.trim(remarks);

        if (remarks == '' || remarks == null || remarks == undefined || remarks.length == 0) {
            $("span[data-valmsg-for='Remarks']").text("Remarks are required.");
        } else {
            $("span[data-valmsg-for='Remarks']").text("");

            let fullyApprovedList = getFullyApprovedList();
            let arr = fullyApprovedList.arr;
            let message = fullyApprovedList.message;

            if (arr.length == 0) {
                alert("Please select one or more billing request");
                return false;
            }

            confirm("Confirm Voiding", `Void the following request/s? (${arr.length} items)<br> ${message} `)
            .done(function () {
                let data = {
                    __RequestVerificationToken: reqToken,
                    billingRequests: arr,
                    remarks: remarks
                }
                voidMultipleBillingRequest(data);
            });
        }
    });
})

function onChangeDurationFrom() {
    let start = $("#DurationFromDatePicker").data('kendoDatePicker');
    let end = $("#DurationToDatePicker").data('kendoDatePicker');

    let startDate = start.value(),
    endDate = end.value();

    if (startDate) {
        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate());
        end.min(startDate);
    } else if (endDate) {
        start.max(new Date(endDate));
        end.min(new Date('1900, 01, 01'));
    } else if (!endDate && !startDate) {
        end.min(new Date('1900, 01, 01'));
        start.max(new Date('2099, 12, 01'));
    }
    validateDateFilter();
}

function onChangeDurationTo() {
    let start = $("#DurationFromDatePicker").data('kendoDatePicker');
    let end = $("#DurationToDatePicker").data('kendoDatePicker');

    let endDate = end.value(),
    startDate = start.value();

    if (endDate) {
        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate());
        start.max(endDate);
    } else if (startDate) {
        end.min(new Date(startDate));
        start.max(new Date('2099, 12, 01'));
    } else if (!endDate && !startDate) {
        end.min(new Date('1900, 01, 01'));
        start.max(new Date('2099, 12, 01'));
    }
    validateDateFilter();
}

function onOpenDurationFrom() {
    let start = $("#DurationFromDatePicker").data('kendoDatePicker');
    let end = $("#DurationToDatePicker").data('kendoDatePicker');
    let startDate = start.value(),
        endDate = end.value();

    if (startDate && endDate) {
        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate());
        end.min(startDate);

        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate());
        start.max(endDate);

    } else if (endDate && !startDate) {
        start.max(new Date(endDate));
        end.min(new Date('1900, 01, 01'));
    } else if (!endDate && !startDate) {
        end.min(new Date('1900, 01, 01'));
        start.max(new Date('2099, 12, 01'));
    }
}

function onOpenDurationTo() {
    let start = $("#DurationFromDatePicker").data('kendoDatePicker');
    let end = $("#DurationToDatePicker").data('kendoDatePicker');
    let endDate = end.value(),
        startDate = start.value();

    if (startDate && endDate) {
        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate());
        start.max(endDate);

        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate());
        end.min(startDate);

    } else if (startDate && !endDate) {
        end.min(new Date(startDate));
        start.max(new Date('2099, 12, 01'));
    } else if (!endDate && !startDate) {
        end.min(new Date('1900, 01, 01'));
        start.max(new Date('2099, 12, 01'));
    }
}

function validateDateFilter() {
    let dStart = $("#DurationFromDatePicker").data('kendoDatePicker').value();
    let dEnd = $("#DurationToDatePicker").data('kendoDatePicker').value();

    if ((dStart <= dEnd) || (dStart != null && dEnd == null) || (dStart !== null && dEnd != null)) {
        $("#gridBillingRequests").data('kendoGrid').dataSource.read();
    }
}

let filter = {
    status: ""
}

function filterGridResults() {
    let grid = $("#gridBillingRequests").data("kendoGrid");
    let dataSource = grid.dataSource;

    dataSource.filter([
        {
            field: "CurrentStatus",
            operator: "Contains",
            value: filter.status
        }
    ]);
}

function removeGridFilter() {
    let grid = $("#gridBillingRequests").data("kendoGrid");
    let dataSource = grid.dataSource;
    dataSource.filter({});
}

function gridBillingRequestsFilter() {
    let start = kendo.toString($('#DurationFromDatePicker').data('kendoDatePicker').value(), 'dd-MMM-yyyy');
    let end = kendo.toString($('#DurationToDatePicker').data('kendoDatePicker').value(), 'dd-MMM-yyyy');
    return {
        durationFrom: start,
        durationTo: end
    }
}

function gridBillingRequestsOnChange() {
    removeInvalidBr();
}

function onErrorGridBillingRequests(e) {
    alert(e.errors.Message, "ERROR")
}

function onChangeGridBillingRequests(e) {
    getWorkflowActions();
}

function getWorkflowActions() {
    let controllerName = "DemoSources";
    let methodName = "GetCounterValuesAsync"
    let url = window.rootUrl + controllerName + '/' + methodName;
    let grid = $("#gridBillingRequests").data("kendoGrid");
    let dataSource = grid.dataSource;

    let workflowActions;
    let forApproval = 0;
    let approved = 0;
    let rejected = 0;
    let voided = 0;

    $.get(url, (response) => {
        if (response.list) {
            workflowActions = response.list;
            if (workflowActions) {

                let lblForApproval = workflowActions.WORKFLOW_BRA_FORAPPROVAL_STATUS ? workflowActions.WORKFLOW_BRA_FORAPPROVAL_STATUS.Name : "For Approval";
                let lblApproved = workflowActions.WORKFLOW_BRA_APPROVED_STATUS ? workflowActions.WORKFLOW_BRA_APPROVED_STATUS.Name : "Approved";
                let lblRejected = workflowActions.WORKFLOW_BRA_REJECT_STATUS ? workflowActions.WORKFLOW_BRA_REJECT_STATUS.Name : "Rejected";
                let lblVoided = workflowActions.WORKFLOW_BRA_VOID_STATUS ? workflowActions.WORKFLOW_BRA_VOID_STATUS.Name : "Voided";

                if (dataSource._data) {
                    dataSource._data.forEach(function (item, index) {
                        let status = item.CurrentStatus;

                        let tr = grid.tbody.find("tr[data-uid='" + item.uid + "']");

                        if (status == lblApproved) {
                            approved += 1;
                            $(tr).find("h5 > span").addClass("badge-success");
                        } else if (status == lblRejected) {
                            rejected += 1;
                            $(tr).find("h5 > span").addClass("badge-primary");
                        } else if (status == lblVoided) {
                            voided += 1;
                            $(tr).find("h5 > span").addClass("badge-dark");
                        } else if (status == lblForApproval) {
                            forApproval += 1;
                            $(tr).find("h5 > span").addClass("badge-warning");
                        } else {
                            elem.addClass("badge-info");
                        }

                    })

                    //CARDS - Counters
                    //set label
                    $("#countForApproval").siblings("p").text(lblApproved.forApproval);
                    $("#countApproved").siblings("p").text(lblApproved);
                    $("#countRejected").siblings("p").text(lblRejected);
                    $("#countVoided").siblings("p").text(lblVoided);

                    //set value
                    $("#countForApproval").closest(".card").attr("value", lblForApproval);
                    $("#countApproved").closest(".card").attr("value", lblApproved);
                    $("#countRejected").closest(".card").attr("value", lblRejected);
                    $("#countVoided").closest(".card").attr("value", lblVoided);

                    //set count text
                    $("#countForApproval").text(forApproval);
                    $("#countApproved").text(approved);
                    $("#countRejected").text(rejected);
                    $("#countVoided").text(voided);
                }
            }

            if (dataSource.filter() == null) {
                filter.status = "";
                $(".card-click").removeClass("shadow");
            }

        } else {
            console.log("Failed to get workflow actions.")
        }
    });
}

function getFullyApprovedList() {
    let grid = $("#gridBillingRequests").data("kendoGrid");
    let message = "";
    let arr = [];

    $.each(grid.selectedKeyNames(), function (index, x) {
        let item = grid.dataSource.get(x);
        if (item.IsLocked == true && (item.Remarks == '' || item.Remarks == undefined || item.Remarks == null) && item.IsCreator) {
            let durationFrom = kendo.toString(kendo.parseDate(item.DurationFrom), 'dd-MMM-yyyy');
            let durationTo = kendo.toString(kendo.parseDate(item.DurationTo), 'dd-MMM-yyyy');

            arr.push({
                Id: item.Id,
                UniqueKey: item.UniqueKey,
                BusinessUnitCode: item.BusinessUnitCode,
                DurationFrom: durationFrom,
                DurationTo: durationTo,
                BRReferenceNo: item.BRReferenceNo
            });

            let msg;

            if (durationFrom == durationTo)
                msg = `${kendo.toString(kendo.parseDate(item.DurationFrom), "MMMM yyyy")}`;
            else
                msg = `from ${kendo.toString(kendo.parseDate(item.DurationFrom), "MMMM")} to ${kendo.toString(kendo.parseDate(item.DurationTo), "MMMM yyyy")}`;

            message += `[${item.BRReferenceNo
            }] ${item.BusinessUnitCode} ${msg}<br>`
        }
    });

    return {
        arr: arr,
        message: message
    }
}

function removeRowSelection(tr) {
    tr.removeClass("k-state-selected");
    let a = tr.find('input[data-role="checkbox"]');
    $(a).prop('checked', false);
    $(a).attr('aria-label', 'Select row');
    $(a).attr('aria-checked', false);
}

function onDataBound(e) {
    let view = e.sender.dataSource.view();
    $('.chart-overlay-no-data').toggle(view.length === 0);
}

function totalSalesReport() {
    let url = window.rootUrl + 'demosources/GetTotalSales';
    $.get(url, (response) => {
        let list = response.value;
        let card = $('#totalSalesCard');
        let row = '';

        if (list != null) {
            for (var i = 0; i < list.length; i++) {
                let item = list[i];
                let totalAmountCurrentYear = item.TotalAmountCurrentYear;
                let totalAmountLastYear = item.TotalAmountPreviousYear;
                let getPercentage = item.Percentage;

                if (getPercentage == 0) {
                    if (totalAmountLastYear == 0 && totalAmountCurrentYear != 0) {

                        let total = ((totalAmountCurrentYear - totalAmountLastYear) / 100) * totalAmountLastYear;
                        if (total == 0) {
                            getPercentage = 100;
                        }
                        else {
                            getPercentage = total;
                        }
                    }
                    else if (totalAmountLastYear != 0 && totalAmountCurrentYear == 0) {
                        let total = ((totalAmountCurrentYear - totalAmountLastYear) * 100) / totalAmountLastYear;

                        if (total != 0) {
                            getPercentage = -100;
                        }
                    }
                    else {
                        getPercentage = 0;
                    }
                }
                else {
                    getPercentage = getPercentage;
                }

                let formatTotalAmountCurrentYear = application.UI.formatCurrency(totalAmountCurrentYear);
                let arrowUpElement = `<span class="fas fa-arrow-circle-up text-success mt-2" style="font-size:2rem;"></span>`;
                let arrowDownElement = `<span class="fas fa-arrow-circle-down text-danger mt-2" style="font-size:2rem;"></span>`;
                let equalElement = `<span class="fa-stack fa-lg"><i class="fas fa-circle fa-stack-2x mt-2" style="font-size:1.8rem;color:gray;"></i>` +
                                   `<i class="fas fa-equals fa-stack-1x fa-inverse mt-2" style="font-size:1rem;color:white;"></i></span>`;
                let increaseOrDecreaseIcon = getPercentage > 0 ? arrowUpElement : getPercentage == 0 ? equalElement : arrowDownElement;
                let formatPercentage = kendo.format('{0:n0}%', parseFloat(getPercentage));

                let sign = getPercentage > 0 ? '+' : '';
                let percentage = `<span class="text-muted">` + sign + formatPercentage + `</span>`;

                //template
                row += `<div class="row">` +
                            `<div class="col-7">` +
                                `<span style="font-size: 2rem;" class="wrap-text">${formatTotalAmountCurrentYear}</span>` +
                                `<span class="card-text text-muted d-block" style="font-size:1rem;">${item.LicenseCurrencyCode}</span>` +
                            `</div>` +
                            `<div class="col-5 d-flex">` +
                                `<div class="row">` +
                                    `<div class="col-4">` +
                                        increaseOrDecreaseIcon +
                                    `</div>` +
                                    `<div class="col-8">` +
                                        `<div class="row mt-2">` +
                                            `<div class="col-12">` +
                                                percentage +
                                            `</div>` +
                                        `</div>` +
                                        `<div class="row mt-n1">` +
                                            `<div class="col-12">` +
                                                `<span class="text-muted">mom</span>` +
                                            `</div>` +
                                        `</div>` +
                                    `</div>` +
                                `</div>` +
                            `</div>` +
                       `</div>`;
            }
            card = card.append(row);
        }
    });
}

function totalAverageRevenuePerUnit() {
    let url = window.rootUrl + 'demosources/GetTotalAverageRevenuePerUnit';

    $.get(url, (response) => {
        let list = response.value;

        let idArpu = $('#totalArpu');
        let idBu = $('#totalBU');
        let rowArpu = "";
        let rowBu = "";

        if (list != null) {
            for (var i = 0; i < list.length; i++) {
                let item = list[i];
                let formatTotalRevenue = application.UI.formatCurrency(item.TotalRevenue);

                //Card Average Revenue
                rowArpu += `<div class="col-6 col-md-6">` +
                                `<span style="font-size:2rem;">${formatTotalRevenue}</span>` +
                                `<span class="ml-1 text-muted d-block">${item.LicenseCurrencyCode}</span>` +
                            `</div>`;
            }
            idArpu = idArpu.append(rowArpu);
            //Card Total BusinessUnit
            idBu.append(`<div class="col-12" style="text-align:right;">` +
                            `<span style="font-size:40px;">${list[0].TotalBusinessUnits}</span>` +
                        `</div>`);
        }
    });
}

function createBillingProgressChart() {
    $('#barChartBillingProgress').kendoChart({
        title: { text: "Current Billing Progress" },
        dataSource: {
            transport: {
                read: window.rootUrl + 'demosources/GetBillingProgress',
                dataType: 'json'
            },
            group: {
                field: 'Category',
                dir: 'asc'
            }
            //sort: {
            //    field: 'Value',
            //    dir: 'desc'
            //}
        },
        legend: {
            visible: false
        },
        chartArea: {
            height: '200px'
        },
        seriesDefaults: {
            type: 'bar',
            spacing: -1
        },
        series: [
            {
                data: [100],
                name: 'Total'
            },
            {
                field: 'Value',
                dataField: 'Category'
            }
        ],
        valueAxis: {
            numeric: '',
            labels: {
                format: '{0}'
            },
            min: 0,
            max: 100
        },
        CategoryAxis: {
            //categories: ''
        },
        tooltip: {
            visible: true,
            format: '{0}',
            template: '#= series.name #: #=  kendo.format("{0:n0}%", parseFloat(value)) #'
        },
        theme: "fiori",
        dataBound: colorHandling,
        seriesClick: billingProgressChartSeriesClick,
        //sortable: true
        //deferred: true,
    });
}

function colorHandling(e) {
    let chart = e.sender;
    let series = chart.options.series;
    for (var i = 0; i < series.length; i++) {
        let item = series[i];

        if (item.name == "Unbilled") {
            item.color = 'rgb(179,179,179)';
        }
        else if (item.name == "For Approval") {
            item.color = '#d9831f';
        }
        else if (item.name == "Approved") {
            item.color = '#469408';
        }
        else
            item.color = '#e9e9e9';
    }
}

function billingProgressChartSeriesClick(e) {
    let $grid1 = $('#gridUnbilled').data('kendoGrid').dataSource;
    let $grid2 = $('#gridForApproval').data('kendoGrid').dataSource;
    let $grid3 = $('#gridApproved').data('kendoGrid').dataSource;

    $grid1.read();
    $grid2.read();
    $grid3.read();

    $('#modalBillingProgressDetails').modal('show');
}

function setGridTotalRowsCount(e) {
    let gridId = e.sender.element[0].id;
    let gridCount = $('#' + gridId).data('kendoGrid').dataSource.total();
    $('#' + gridId + 'Count').text(gridCount);
}

function gridBillingRequestsOnDataBound() {
    let grid = this;
    grid.tbody.find("tr[role='row']").each(function () {
        let model = grid.dataItem(this),
            notes = model.Note,
            remarks = model.Remarks,
            isLocked = model.IsLocked;

        //notes
        if (notes != '' && notes != undefined) {
            if (notes != '<hr/>')
                $(this).find(".k-grid-Notes").removeClass("k-hidden");
            else
                $(this).find(".k-grid-Notes").addClass("k-hidden");
        }
        else {
            $(this).find(".k-grid-Notes").addClass("k-hidden");
        }

        //remarks
        if (remarks != '' && remarks != undefined) {
            if (remarks != '<hr/>')
                $(this).find(".k-grid-Remarks").removeClass("k-hidden");
            else
                $(this).find(".k-grid-Remarks").addClass("k-hidden");
        }
        else {
            $(this).find(".k-grid-Remarks").addClass("k-hidden");
        }

        //checkbox
        if (isLocked == true && (remarks == '' || remarks == undefined || remarks == null) && model.IsCreator) {
            $(this).find('input[data-role="checkbox"]').removeClass("k-state-disabled").prop("disabled", false).removeClass("k-hidden");
        }
        else {
            $(this).find('input[data-role="checkbox"]').addClass("k-state-disabled").prop("disabled", 'disabled').addClass("k-hidden");
        }
    });
}

function onViewNotes(e) {
    e.preventDefault();
    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let notes = dataItem.Note;
    if (notes != '' && notes != undefined)
        window.alert(notes, 'Notes')
}

function onViewRemarks(e) {
    e.preventDefault();
    let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let remarks = dataItem.Remarks;
    if (remarks != '' && remarks != undefined)
        window.alert(remarks, 'Remarks')
}