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

            if (editRow.length > 0) {
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
            qtyCostOnChange: function (e) {
                let kQuantity = $("#quantity").data("kendoNumericTextBox");
                let kCost = $("#Cost").data("kendoNumericTextBox");

                if (kQuantity !== undefined && kCost !== undefined) {
                    let qty = kQuantity.value() === null ? 0 : kQuantity.value();
                    let cost = kCost.value() === null ? 0 : kCost.value();
                    let amt = kendo.toString(qty * cost, "N2");
                    $('td[data-container-for="Cost"]').next().html(amt);
                }
            },
            isGoldChanged: function (e) {
                var isChecked = $(this)[0].checked;
                var weightNb = $("#Weight").data("kendoNumericTextBox");
                weightNb.enable(isChecked);
            },
            productsOnDataBound: function (e) {
                if (e.sender.value() == "0") {
                    e.sender.value(null)
                }
            },
            onEdit: function (e) {
                if (e.model.SalesType_id === 0 && e.model.location_id === 0) {
                    var location_id = $("#main_location_id").data("kendoDropDownList").value();
                    var SalesType_id = $("#main_SalesType_id").data("kendoDropDownList").value();

                    e.model.location_id = location_id;
                    e.model.SalesType_id = SalesType_id;

                    $("#location_id").data("kendoDropDownList").bind("dataBound", function (ddl) { ddl.sender.value(location_id); })
                    $("#SalesType_id").data("kendoDropDownList").bind("dataBound", function (ddl) { ddl.sender.value(SalesType_id); })
                }

                var uButton = e.container.find(".k-button.k-grid-update"); //update button
                var cButton = e.container.find(".k-button.k-grid-cancel"); //cancel button
                uButton.removeClass("k-state-disabled");
                uButton.html(`<span class="fas fa-save"></span>`)
                cButton.html(`<span class="fas fa-ban"></span>`)


                $(uButton).click(function (e) {
                    setTimeout(function () {
                        var fields = ["SalesType_id", "location_id", "product_id", "Cost", "quantity", "Weight", "InvoiceNumber"]
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
                $('div[data-valmsg-for="' + this.id + '"]').find("span.k-tooltip-content").html("This field is required.");
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
            deleteSalesOrder: function (data) {
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