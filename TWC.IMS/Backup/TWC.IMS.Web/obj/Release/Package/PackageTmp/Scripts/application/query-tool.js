
$(function () {
    // VARIABLES
    let inputTableId = $("#TableId");
    var _tableColumns = [];
    var lineItemCounter = 0;
    var logicalOperators = ["AND", "OR"]
    var conditions = [
        { dataType: "int", conditions: [">", ">=", "=", "<", "<=", "IS NULL", "IS NOT NULL"] },
        { dataType: "datetime", conditions: [">", ">=", "=", "<", "<=", "IS NULL", "IS NOT NULL"] },
        { dataType: "datetimeoffset", conditions: [">", ">=", "=", "<", "<=", "IS NULL", "IS NOT NULL"] },
        { dataType: "float", conditions: [">", ">=", "=", "<", "<=", "IS NULL", "IS NOT NULL"] },
        { dataType: "bit", conditions: ["=", "IS NULL", "IS NOT NULL"] },
        { dataType: "decimal", conditions: [">", ">=", "=", "<", "<=", "IS NULL", "IS NOT NULL"] },
        { dataType: "varchar", conditions: ["=", "contains", "IS NULL", "IS NOT NULL"] },
        { dataType: "char", conditions: ["="] },
        { dataType: "timestamp", conditions: [">", ">=", "=", "<", "<=", "IS NULL", "IS NOT NULL"] },
        { dataType: "nvarchar", conditions: ["=", "contains", "IS NULL", "IS NOT NULL"] },
        { dataType: "nchar", conditions: ["=", "IS NULL", "IS NOT NULL"] },
    ]


    //////////////////////////////
    // CONTROLS INITIALIZATION //
    //////////////////////////////

    // TABLE COMBO BOX
    inputTableId.kendoComboBox({
        dataTextField: "TableName",
        dataValueField: "TableId",
        optionLabel: 'Select Table',
        select: onSelectTableId,
        index: -1,
        //dataBound: onDataBoundTableId,
        dataSource: {
            transport: {
                read: {
                    dataType: "json",
                    url: window.rootUrl + 'support/gettablename?id=-1',
                }
            }
        }
    });
    inputTableId.data("kendoComboBox")
    inputTableId.bind("select", onSelectTableId);

    ////////////////////////////
    ////////// EVENTS //////////
    ////////////////////////////

    // ON SELECT TABLE
    function onSelectTableId(e) {
        if (e != undefined) {

            $("#queryFiltersBody").html("")
            $.getJSON(window.rootUrl + 'Support/Columns?id=' + e.dataItem.TableId)
                .done(function (response) {
                    $("#addFilter").attr("disabled", false);
                    _tableColumns = response;
                });
        }
    }

    // ADD FILTER ON CLICK
    $("body").on("click", "#addFilter", function (e) {
        lineItemCounter++;

        let filterLine = "";
        filterLine += ("<tr name='filterLine' id='filterLine_" + lineItemCounter + "'>")
        filterLine += ("<td width='5%'><button type='button' onclick='removeFilterOnClick(" + lineItemCounter + ")' class='btn btn-danger btn-sm' title='Remove filter' id='removeFilter'><span class='fas fa-minus'></span></button></td>")
        filterLine += ("<td width='10%'><input style='' name='QueryFilter_LogicalOperator' id='QueryFilter_LogicalOperator_" + lineItemCounter + "' /></td>")
        filterLine += ("<td width='25%'><input name='QueryFilter_Field' id='QueryFilter_Field_" + lineItemCounter + "' /></td>")
        filterLine += ("<td width='25%'><input name='QueryFilter_Condition' id='QueryFilter_Condition_" + lineItemCounter + "' /></td>")
        filterLine += ("<td width='35%'><input name='QueryFilter_Value' id='QueryFilter_Value_" + lineItemCounter + "' /></td>")
        filterLine += ("</tr>")
        $("#queryFiltersBody").append(filterLine);

        $("#QueryFilter_LogicalOperator_" + lineItemCounter).kendoDropDownList({
            dataSource: logicalOperators,
            width: 50
        });
        $("#QueryFilter_Value_" + lineItemCounter).kendoMaskedTextBox();
        $("#QueryFilter_Field_" + lineItemCounter).kendoDropDownList({
            dataValueField: "ColumnName",
            dataTextField: "ColumnName",
            dataSource: {
                data: _tableColumns
            },
            change: function (e) {
                let control = e.sender;
                let dataItem = control.dataItem();

                let condition = conditions.filter(function (data) {
                    return data.dataType === dataItem.DataType;
                });

                condition = condition.length > 0 ? condition[0] : null;
                if (condition !== null) {
                    let condCtrl = $("#QueryFilter_Condition_" + control.element[0].id.split("_")[2]).data("kendoDropDownList");
                    condCtrl.setDataSource(condition.conditions)
                    condCtrl.select(0);
                }
            }
        });
        $("#QueryFilter_Condition_" + lineItemCounter).kendoDropDownList({
            change: function (e) {
                let control = e.sender;
                let valCtrl = $("#QueryFilter_Value_" + lineItemCounter);//.data("kendoDropDownList");

                if (control.value() === "IS NULL" || control.value() === "IS NOT NULL") {
                    valCtrl.hide();
                } else {
                    valCtrl.show();
                }
            }
        });
                
        $("#QueryFilter_Field_" + lineItemCounter).data("kendoDropDownList").trigger("change");

        removeLogicalOperator(0);
    })
});

function removeFilterOnClick(trId) {
    $("#filterLine_" + trId).remove()
    removeLogicalOperator(0);
}

function removeLogicalOperator(index) {
    let logOperator = $("input[name='QueryFilter_LogicalOperator']")[index];
    if (logOperator !== null && logOperator !== undefined) {
        let dp = $("#" + logOperator.id).data("kendoDropDownList")
        dp.select(-1);
        dp.wrapper.hide();
    }
}

// FILTER BUTTON
$("#filter").click(function (e) {
    $("#gridHolder").html("");
    $("#gridHolder").html("Loading...");

    let filters = []
    $("tr[name=filterLine]").each(function () {
        let logicalOperatorCtrl = $(this).find("input[name='QueryFilter_LogicalOperator']").data("kendoDropDownList");
        let fieldCtrl = $(this).find("input[name='QueryFilter_Field']").data("kendoDropDownList");
        let conditionCtrl = $(this).find("input[name='QueryFilter_Condition']").data("kendoDropDownList");
        let valueCtrl = $(this).find("input[name='QueryFilter_Value']").data("kendoMaskedTextBox");

        filters.push({
            LogicalOperator: logicalOperatorCtrl.value(),
            Field: fieldCtrl.value(),
            Condition: conditionCtrl.value(),
            Value: valueCtrl.value()
        })
    })

    let condition = {
        TableName: $("#TableId").data("kendoComboBox").text(),
        Filters: filters,
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    }

    $.post(window.rootUrl + "Support/QueryTable", condition)
    .done(function (response, t, e) {

        //console.log({ response, t, e })
        //console.log('error');
        //console.log(e);
        if (response.indexOf('<!') == 0) {
            let newDoc = document.open("text/html", "replace");
            newDoc.write(response);
            newDoc.close();
        }
        else {
            $("#gridHolder").html(response);
        }
    })
})