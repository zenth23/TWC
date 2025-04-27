$(document).ready(function () {
    $("hr.my-4").hide();
    $("#reportLink").hide();
    $("div.form-group.row").prepend("<div class='col-sm-12 col-md-12 col-lg-12 col-xl-12'><br/></div>");
    $("h3").prepend("<span class='fas fa-chart-line'></span> ");
    $("#gridYearlySalesReport").height(600);
});
