$(function () {
   
    $("#btnDropDown").click(function (e) {
        let subscriptionType = $("#LicenseSubscriptionTypeCode").data("kendoDropDownList").text();
        let url = window.rootUrl + "reportgenerator/GetLatestReport"
        let currentYear = (new Date).getFullYear();
        var reportName = "Yearly_Sales_Report_for_" + currentYear + "_" + subscriptionType + ".xlsx";

        $.get(url, {fileName : reportName}, success);
    });

    //Export to excel
    $("#btnExportToExcel").click(function (e) {
        e.preventDefault();
        let currentYear = (new Date).getFullYear();
        let reportName = 'Yearly Sales Report for ' + currentYear;
        let $g = $("#gridYearlySalesReport");
        let grid = $g.data("kendoGrid");

        grid.bind("excelExport", function (x) {
            let currencyCode = x.data[0].LicenseCurrencyCode;
            let workbook = x.workbook;
            let sheet = workbook.sheets[0];
            sheet.frozenRows = 2;
            sheet.frozenColumns = 2;
            sheet.mergedCells = ["A1:O1"];
            sheet.name = reportName;
            // add header
            if (sheet.rows[0].cells.length > 1) {
                let headerTitle = [{
                    value: reportName + " (" + currencyCode + ")",
                    background: "#eb4651",
                    textAlign: "left",
                    verticalAlign: "center",
                    fontSize: 18,
                    color: "#ffffff"
                }];
                sheet.rows.unshift({
                    cells: headerTitle,
                    type: "header",
                    height: 40
                });
            }

            for (var rowIndex = 2; rowIndex < sheet.rows.length; rowIndex++) {
                let row = sheet.rows[rowIndex];

                for (var cellIndex = 0; cellIndex <= 1; cellIndex++) {
                    let cells = row.cells[cellIndex];
                    cells.textAlign = "left";

                    if (row.type == "footer") {
                        cells.borderTop = 2;
                    }
                }

                for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                    let cells = row.cells[cellIndex];

                    if (row.type == "footer") {
                        cells.borderTop = 2;
                        cells.bold = true;
                    }
                    if (cells.value == null || cells.value <= 0) {
                        cells.value = "-";
                        cells.textAlign = "right";
                    }
                    else {
                        row.cells[cellIndex].format = "#,##0.00";
                        cells.textAlign = "right";
                    }
                }
            }
        });
        grid.saveAsExcel();
    }); 
});

//------------ excel export override file name 
function exportExcel(e) {
    let ddlValue = $("#LicenseSubscriptionTypeCode").data("kendoDropDownList").text();
    let currentYear = (new Date).getFullYear();
    e.workbook.fileName = "Yearly_Sales_Report_for_" + currentYear + "_" + ddlValue + ".xlsx";
}

function filterGrid() {
    let cbValue = $("#LicenseSubscriptionTypeCode").data("kendoDropDownList").text();
    return { subscriptionTypeCode: cbValue };
}

function onDDLChange() {
    let url = window.rootUrl + 'reportgenerator/ReadDashboardYearlyReportsUnits';
    let cbValue = $("#LicenseSubscriptionTypeCode").data("kendoDropDownList").text();
    $.get(url, { subscriptionTypeCode: cbValue });
    application.grid.refreshGrid($('#gridYearlySalesReport'));
}

function success(response) {
    if (response.status == "SUCCESS") {
        let genDateTime = kendo.toString(kendo.parseDate(response.data.Created), "dd-MMM-yyyy hh:mm:ss tt")
        let expDateTime = kendo.toString(kendo.parseDate(response.data.ExpirationDate), "dd-MMM-yyyy hh:mm:ss tt")    
        let currentDateTime = response.currentDate;
        let subscriptionType = $("#LicenseSubscriptionTypeCode").data("kendoDropDownList").text();
        let reportName = response.data.ReportName;
        let link = window.rootUrl + 'reportgenerator/downloadreport?report=' + reportName;

        if (response.data.ExpirationDate < currentDateTime) {
            $("#ddl-container").removeAttr("class");
            $("#ddl-container").empty().append('<span class="dropdown-item"> No latest download </span>');
        }
        else {
            $("#ddl-container").removeAttr("class");
            $("#ddl-container").empty().append('<a class="dropdown-item" href=' + link + ' title="Download Report">' +
                                                    '<span class="fas fa-download"></span>' +
                                                    '<span>' + ' ' + reportName + '</span>' +
                                                    '<span class="ml-3" style="display: block;">' +
                                                        'Gen. Date:' +
                                                        '<span> ' + genDateTime + '</span>' +
                                                    '</span>' +
                                                    '<span class="ml-3" style="display: block;">' +
                                                        'Exp. Date:' +
                                                        '<span> ' + expDateTime + '</span>' +
                                                    '</span>' +
                                               '</a>');
        }
    }
    else {
        $("#ddl-container").removeAttr("class");
        $("#ddl-container").empty().append('<span class="dropdown-item"> No latest download </span>');
    }
}