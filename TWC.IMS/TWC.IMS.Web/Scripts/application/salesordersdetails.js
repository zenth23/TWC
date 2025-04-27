salesOrders = {
    initialize: function () {
        // CHANGE DEFAULT VALIDATION MESSAGE
        $("body").on("focusout, blur" 
                    , "#SalesType_id, #location_id, #product_id, #quantity, #Cost, #InvoiceNumber" 
                    , salesOrders.editor.events.changeValidationMessage);
        $("body").on("change", "#IsGold", salesOrders.editor.events.isGoldChanged);
        $("body").on("click", "#downloadTemplateButton", salesOrders.editor.events.templateOnDownload);
        $("body").on("click", "#openUploadModalButton", salesOrders.editor.events.onOpenUploadModal);

        $(".k-grid").on("mousedown", ".k-grid-header th", function (e) {
            var grid = $(this).closest(".k-grid");
            var editRow = grid.find(".k-grid-edit-row");

            if (editRow.length > 0) {qtyCostOnChange
                alert("Please complete the editing operation before sorting or filtering");
                e.preventDefault();
            }
        });

        $('body').on('keypress', "#InvoiceNumber", function (event) {
            var regex = new RegExp("^[a-zA-Z0-9]+$");
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (!regex.test(key)) {
                event.preventDefault();
                return false;
            }
        });

       

    },
    editor: {
        events: {
            qtyCostOnChange: function(e) {
                let isChecked = $("#IsGold").is(":checked"); 
                let kWeight = $("#Weight").data("kendoNumericTextBox");
                let kCost = $("#Cost").data("kendoNumericTextBox");
                let kQty = $("#quantity").data("kendoNumericTextBox");
                
                if (kWeight !== undefined && kCost !== undefined) {
                    let qty =  isChecked === false? kQty.value() : kWeight.value();
                    let cost = kCost.value() === null ? 0 : kCost.value();
                    let amt = kendo.toString(qty * cost, "N2");
                    $('td[data-container-for="Cost"]').next().html(amt);
                }
            },
            isGoldChanged: function(e) {
                //var isChecked = $(this)[0].checked;
                let isChecked = $("#IsGold").is(":checked"); 
                var weightNb = $("#Weight").data("kendoNumericTextBox");
                weightNb.enable(isChecked);

                var quantityNb = $("#quantity").data("kendoNumericTextBox");
                quantityNb.enable(!isChecked);

                //if (isChecked) {
                   
                //    $('td[data-container-for="Weight"]').next().html(1);
                //} else {
                        
                //}
               
              
             
            },
            productsOnDataBound: function(e) {
                if (e.sender.value() == "0") {
                    e.sender.value(null)
                }
            },
            onEdit: function (e) {
                if (e.model.SalesType_id === 0 && e.model.location_id === 0) {
                    var location_id = $("#main_location_id").data("kendoDropDownList").value();
                    var SalesType_id = $("#main_SalesType_id").data("kendoDropDownList").value();
                    var ProductNumber_id = $("#main_ProductNumber_id").data("kendoDropDownList").value();

                    e.model.location_id = location_id;
                    e.model.SalesType_id = SalesType_id;
                    e.model.product_id = ProductNumber_id;

                    $("#location_id").data("kendoDropDownList").bind("dataBound", function (ddl) { ddl.sender.value(location_id); })
                    $("#SalesType_id").data("kendoDropDownList").bind("dataBound", function (ddl) { ddl.sender.value(SalesType_id); })
                    $("#product_id").data("kendoDropDownList").bind("dataBound", function (ddl) { ddl.sender.value(ProductNumber_id); })
                }

                var uButton = e.container.find(".k-button.k-grid-update"); //update button
                var cButton = e.container.find(".k-button.k-grid-cancel"); //cancel button
                uButton.removeClass("k-state-disabled");
                uButton.html(`<span class="fas fa-save"></span>`)
                cButton.html(`<span class="fas fa-ban"></span>`)


                $(uButton).click(function(e) {
                    setTimeout(function() {
                        var fields = ["SalesType_id", "location_id", "product_id", "Cost" , "quantity", "Weight", "InvoiceNumber"]
                        salesOrders.editor.events.changeAllValidationMessage(fields);
                    }, 0);
                    
                })


                $("input.text-box").addClass("k-textbox");

                $("#IsGold").trigger("change");
                
            },
            onGridDeleteClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")

                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    salesOrders.editor.services.deleteSalesOrder(data)
                    .done(function (response) {
                        if (response.Success) {
                            alert("Sales Order deleted.");
                            application.grid.refreshGrid($('#gridSalesOrders'))
                        } else {
                            alert(response.Message);
                        }
                    }).fail(salesOrders.common.catchError);
                })
            },
            changeValidationMessage: function (e) {
                var field = $(e.target);
                var value = field.val();
                if (value == "") {
                    $('div[data-valmsg-for="' + this.id + '"]').find("span.k-tooltip-content").html("This field is required.");

                }
            },
            changeAllValidationMessage: function (ids) {
                var customs = ["product_id"]
                for (var i = 0; i < ids.length; i++) {
                    if (!customs.includes(ids[i])) {
                        $('div[data-valmsg-for="' + ids[i] + '"]').find("span.k-tooltip-content").html("This field is required.");
                    } else {
                        var cb = $("#" + ids[i]).data("kendoComboBox");
                        if (cb !== undefined) {
                            if (cb.selectedIndex === -1) {
                                var td = $("input#" + ids[i]).closest("td");
                                td.find("div.k-tooltip-error").remove();
                                td.append('<div class="k-tooltip k-tooltip-error k-validator-tooltip k-invalid-msg field-validation-error" data-for="' + ids[i] + '" id="' + ids[i] + '_validationMessage" data-valmsg-for="' + ids[i] + '"><span class="k-tooltip-icon k-icon k-i-warning"></span><span class="k-tooltip-content">This field is required.</span><span class="k-callout k-callout-n"></span></div>');
                            } else {
                                $("#" + ids[i] + "_hidden").val(cb.value());
                            }
                        }
                    }
                }
            },
            templateOnDownload: function (e) {
                e.preventDefault();
                var url = window.rootUrl + "salesorders/downloadtemplate"
                window.open(url, '_blank').focus();
            },
            onOpenUploadModal: function (e) {
                $("#uploadExcelModal").modal("show");
            },
            onUploadModalClose: function (e) {
                $("#uploadExcelModal").modal("hide");

                $("#gridExcelUpload").data("kendoGrid").dataSource.data([]);
                $("#excelUpload").data("kendoUpload").clearAllFiles();
                $("#excel_location_id").data("kendoDropDownList").value(null);
                $("#excel_SalesType_id").data("kendoDropDownList").value(null);
            },
            onUploadSelected: function () {
                setTimeout(function () { kendo.ui.progress($("#gridExcelUpload"), true); }, 0)
            },
            onUploadComplete: function (e) {
                var grid = $("#gridExcelUpload").data("kendoGrid");
                grid.dataSource.data(e.response);
                //qtyCostOnChange();
                kendo.ui.progress($("#gridExcelUpload"), false);
            },
            onSaveExcelContent: function (e) {
                var grid = $("#gridExcelUpload").data("kendoGrid");
                var data = grid.dataSource.data();
                var withInvalid = false;

                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].ValidationMessage !== ""
                            && data[i].ValidationMessage !== null) {
                            withInvalid = true; break;
                        }
                    }
                } else {
                    alert("Excel file is empty."); return;
                }

                if (withInvalid)
                    alert("Excel file contains invalid items.");
                else {
                    var isValid = $(".modal-validator").kendoValidator({
                        messages: {
                            required: function (input) {
                                return "This field is required."
                            }
                        }
                    }).data("kendoValidator").validate();

                    if (isValid) {
                        var locationId = $("#excel_location_id").data("kendoDropDownList").value();
                        var SalesTypeId = $("#excel_SalesType_id").data("kendoDropDownList").value();

                        var paramData = $.map(data, function (item) {
                            return {
                                LocationId: locationId,
                                SalesTypeId: SalesTypeId,
                                ProductName: item.ProductName,
                                InvoiceNumber: item.InvoiceNumber,
                                Weight: item.Weight,
                                Quantity: item.quantity,
                                Cost: item.Cost
                            }
                        });

                        kendo.ui.progress($("#uploadExcelModal"), true);

                        salesOrders
                            .editor
                            .services
                            .uploadSalesOrders({ salesOrders: paramData })
                            .done(function (response) {
                                if (response.Success) {
                                    $("#gridExcelUpload").data("kendoGrid").dataSource.data([]);
                                    $("#excelUpload").data("kendoUpload").clearAllFiles();
                                    $("#excel_location_id").data("kendoDropDownList").value(null);
                                    $("#excel_SalesType_id").data("kendoDropDownList").value(null);

                                    kendo.ui.progress($("#uploadExcelModal"), false);
                                    $("#uploadExcelModal").modal("hide");

                                    application.grid.refreshGrid($('#gridSalesOrders'));
                                } else {
                                    alert(response.Message)
                                }
                            }).fail(salesOrders.common.catchError);
                    }
                }
            }
        },
        services: {
            deleteSalesOrder: function(data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "salesorders/delete";
                return $.post(url, data);
            },
            uploadSalesOrders: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });
                
                let url = window.rootUrl + "salesorders/upload";
                return $.post(url, data);
            }
        }
    },
    common: {
        catchError: function (x, t, e) {
            $('#systemSpinner').hide();
            console.log('error');
            console.log(x, t, e);
            let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
            window.alert(msg, t);
        },
        errorHandler: function (e) {
            if (e.errors) {
                var message = "Errors:\n";
                $.each(e.errors, function (key, value) {
                    if ('errors' in value) {
                        $.each(value.errors, function () {
                            message += this + "\n";
                        });
                    }
                });
                alert(message);
            }
        },
        onRequestEnd: function (e) {
            if (e.type == "create") {
                if (e.response.Success == false) {
                    alert(e.response.Message)
                } else {
                    alert("Sales order successfully saved!")
                }
                application.grid.refreshGrid($('#gridSalesOrders'))
            } else if (e.type == "update") {
                if (e.response.Success == false) {
                    alert(e.response.Message)
                } else {
                    alert("Sales order successfully updated!")
                }
                application.grid.refreshGrid($('#gridSalesOrders'))
            }
        }
    }

}
salesOrders.saveSalesOrder = function () {
    var salesOrderData = {
        location_id: parseInt($("#main_location_id").data("kendoDropDownList").value()) || 0,
        SalesType_id: parseInt($("#main_SalesType_id").data("kendoDropDownList").value()) || 0,
        InvoiceNumber: $("#InvoiceNumber").val().trim(),
        SalesOrderDetails: []
    };

    console.log("DEBUG: Before collecting order details", salesOrderData);

    if (!salesOrderData.location_id || !salesOrderData.SalesType_id) {
        alert("Please select a valid Location and Sales Type.");
        return;
    }

    $("#salesOrderTable tbody tr").each(function () {
        var row = $(this);
        var productDropdown = row.find("input.product-dropdown").data("kendoDropDownList");

        if (!productDropdown || !productDropdown.value()) {
            alert("Please select a valid product before saving.");
            return;
        }

        var productId = parseInt(productDropdown.value()) || 0;
        var quantity = parseFloat(row.find(".quantity-input").val()) || 0;
        var weight = parseFloat(row.find(".weight-input").val()) || 0;
        var cost = parseFloat(row.find(".cost-input").val()) || 0;
        var isGold = row.find(".isGold-checkbox").is(":checked");

        if (productId > 0 && (quantity > 0 || weight > 0)) {
            salesOrderData.SalesOrderDetails.push({
                SalesOrderDetail_Product: productId,
                Qty: quantity,
                Weight: weight,
                Cost: cost,
                isGold: isGold
            });
        }
    });

    if (salesOrderData.SalesOrderDetails.length === 0) {
        alert("No valid product details entered! Ensure each row has a selected product, quantity, and cost.");
        return;
    }

    console.log("DEBUG: Final sales order data", JSON.stringify(salesOrderData));

    $.ajax({
        url: "/SalesOrdersDetails/Save",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ model: salesOrderData }),
        success: function (response) {
            if (response.Success) {
                alert("Sales order saved successfully!");
                
                window.location.href = "/SalesOrdersDetails/Index";
            } else {
                alert("Error: " + response.Message);
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", xhr.responseText);
            alert("An error occurred while saving the sales order.");
        }
    });
};



$(document).on("click", "#addRow", function () {
    var table = $("#salesOrderTable tbody");
    var rowCount = $("#salesOrderTable tbody tr").length;

    var newRow = $(`
        <tr>
            <td><input class="product-dropdown dropdown-lg" id="ProductNumber_${rowCount}" style="width: 100%;" /></td>
            <td><input type="text" class="form-control quantity-input" name="quantity[]" oninput="calculateAmount(this)"></td>
            <td><input type="text" class="form-control weight-input" name="weight[]" disabled oninput="calculateAmount(this)"></td>
            <td><input type="text" class="form-control cost-input" name="cost[]" oninput="calculateAmount(this)"></td>
            <td><input type="checkbox" class="form-check-input isGold-checkbox" onchange="toggleFields(this)"></td>
            <td><input type="text" class="form-control amount-input" name="amount[]" readonly></td>
            <td><button type="button" class="btn btn-danger remove-row">Remove</button></td>
        </tr>
    `);

    table.append(newRow);

    console.log("DEBUG: Row added. Checking for .product-dropdown...");
    console.log("Row has .product-dropdown?", newRow.find(".product-dropdown").length > 0);

    // ✅ Convert the new .product-dropdown input into a Kendo DropDownList
    newRow.find(".product-dropdown").kendoDropDownList({
        optionLabel: "Select Product",
        dataTextField: "product_name",
        dataValueField: "Id",
        filter: "contains",
        dataSource: {
            transport: {
                read: {
                    url: "/Common/GetProductsForDdl",
                    dataType: "json"
                }
            }
        }
    });

    console.log("DEBUG: Kendo DropDownList initialized.");
});

$(document).ready(function () {
    console.log("DEBUG: Initializing Kendo DropDownList for existing rows...");

    $(".product-dropdown").each(function () {
        console.log("DEBUG: Applying KendoDropDownList to:", this);

        $(this).kendoDropDownList({
            optionLabel: "Select Product",
            dataTextField: "product_name",
            dataValueField: "Id",
            filter: "contains",
            dataSource: {
                transport: {
                    read: {
                        url: "/Common/GetProductsForDdl",
                        dataType: "json"
                    }
                }
            }
        });
    });

    console.log("DEBUG: Kendo DropDownList initialization completed.");
});

function onEditClick(event) {
    event.preventDefault(); 


    var grid = $("#gridSalesOrders").data("kendoGrid"); // Make sure the grid ID matches yours
    var row = $(event.currentTarget).closest("tr"); // Get the clicked row
    var dataItem = grid.dataItem(row); // Get the data bound to the row

    if (dataItem && dataItem.Id) {
        // Redirect to details.cshtml with the SalesOrder ID
        window.location.href = "/SalesOrdersDetails/Details?id=" + dataItem.Id;
    } else {
        alert("Error: Could not retrieve Sales Order ID.");
    }
}




