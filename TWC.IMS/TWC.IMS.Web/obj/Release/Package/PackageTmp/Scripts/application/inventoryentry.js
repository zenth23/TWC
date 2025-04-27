inventoryentry = {
    initialize: function () {
        console.log("initialize function called");
       $("body").on("click", "#downloadIETemplateButton", inventoryentry.editor.events.templateOnDownload);
        $("body").on("click", "#openUploadModalButton", inventoryentry.editor.events.onOpenUploadModal);
        // CHANGE DEFAULT VALIDATION MESSAGE
        $("body").on("focusout, blur" 
                    , "#location_id, #product_id, #category_id, #quantity, #entry_date, #received_date" 
                    , inventoryentry.editor.events.changeValidationMessage);

        // DATE PICKER VALIDATION
        $("body").on("change", "#received_date, #entry_date", inventoryentry.editor.events.dateOnChange);


     
        $(".k-grid").on("mousedown", ".k-grid-header th", function (e) {
            var grid = $(this).closest(".k-grid");
            var editRow = grid.find(".k-grid-edit-row");

            if (editRow.length > 0) {
                alert("Please complete the editing operation before sorting or filtering");
                e.preventDefault();
            }
        });
    },
    editor: {
        events: {
            
            onEdit: function(e) {
               
                var uButton = e.container.find(".k-button.k-grid-update"); //update button
                var cButton = e.container.find(".k-button.k-grid-cancel"); //cancel button
                uButton.removeClass("k-state-disabled");
                uButton.html(`<span class="fas fa-save"></span>`)
                cButton.html(`<span class="fas fa-ban"></span>`)

                $(uButton).click(function(e) {
                    setTimeout(function() {
                        var fields = [ "location_id", "product_id", "category_id" , "quantity", "entry_date", "received_date"]
                        inventoryentry.editor.events.changeAllValidationMessage(fields);

                        $("#entry_date, #received_date").trigger("change");
                    }, 10);
                    
                })
            },
            onGridDeleteClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")

                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    inventoryentry.editor.services.deleteinventoryentry(data)
                    .done(function (response) {
                        if (response.Success) {
                            alert("Inventory entry deleted.");
                            application.grid.refreshGrid($('#gridInventoryEntries'))
                        } else {
                            alert(response.Message);
                        }
                    }).fail(inventoryentry.common.catchError);
                })
            },
            changeValidationMessage: function(e) {
                $('div[data-valmsg-for="'+ this.id +'"]').find("span.k-tooltip-content").html("This field is required.");
            },
            changeAllValidationMessage: function(ids) {
                for(var i = 0; i < ids.length; i++) {
                    $('div[data-valmsg-for="'+ ids[i] +'"]').find("span.k-tooltip-content").html("This field is required.");
                }
            },
            dateOnChange: function(e) {
                var id = $(this).attr("id");
                var val = $("#" + id).val()

                var datePicker = $("#" + id).data("kendoDatePicker");
                console.log(val)
                datePicker.value(val)
                if(datePicker.value() === null) {

                    var td = $(datePicker.element).closest('td[role="gridcell"]');
                    var popup = td.find('div.k-invalid-msg[data-for="'+id+'"]');

                    if(popup.length > 0) {
                        $(popup).html('<span class="k-tooltip-icon k-icon k-i-warning"></span><span class="k-tooltip-content">This field is required.</span><span class="k-callout k-callout-n"></span>');
                    } else {
                        $(td).append('<div class="k-tooltip k-tooltip-error k-validator-tooltip k-invalid-msg field-validation-error" data-for="'+id+'" id="'+id+'_validationMessage" data-valmsg-for="'+id+'"><span class="k-tooltip-icon k-icon k-i-warning"></span><span class="k-tooltip-content">This field is required.</span><span class="k-callout k-callout-n"></span></div>');
                    } 

                    setTimeout(function() {
                        var popup = td.find('div.k-invalid-msg[data-for="'+id+'"]');
                        $(popup).removeClass("k-hidden");
                    }, 10)

                    if(val !== "" || val !== null)
                        $("#" + id).val(val)
                }

                //var entryDatePicker = $("#entry_date").data("kendoDatePicker");
                //var receivedDatePicker = $("#received_date").data("kendoDatePicker");


                
            },
            templateOnDownload: function (e) {
                e.preventDefault();
                var url = window.rootUrl + "inventory_entries/downloadtemplate"
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
                //$("#excel_SalesType_id").data("kendoDropDownList").value(null);
            },
            onUploadSelected: function () {
                setTimeout(function () { kendo.ui.progress($("#gridExcelUpload"), true); }, 0)
            },
            onUploadComplete: function (e) {
                var grid = $("#gridExcelUpload").data("kendoGrid");
                for (var i = 0; i < e.response.length; i++) {
                    e.response[i].entry_date = new Date(parseInt(e.response[i].entry_date.match(/([0-9]+)/)[0]));
                    e.response[i].received_date = new Date(parseInt(e.response[i].received_date.match(/([0-9]+)/)[0]));
                }
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
                        //var SalesTypeId = $("#excel_SalesType_id").data("kendoDropDownList").value();

                        var paramData = $.map(data, function (item) {
                            return {
                                LocationId: locationId,
                                //SalesTypeId: SalesTypeId,
                                ProductName: item.ProductName,
                                CategoryName: item.CategoryName,
                                Quantity: item.quantity,
                                EntryDate: kendo.toString(item.entry_date, "d"),
                                ReceivedDate: kendo.toString(item.received_date, "d"),
                                Remarks: item.remarks
                            }
                        });

                        kendo.ui.progress($("#uploadExcelModal"), true);

                        inventoryentry
                            .editor
                            .services
                            .uploadInventoryEntries({ inventoryEntries: paramData })
                            .done(function (response) {
                                if (response.Success) {
                                    $("#gridExcelUpload").data("kendoGrid").dataSource.data([]);
                                    $("#excelUpload").data("kendoUpload").clearAllFiles();
                                    $("#excel_location_id").data("kendoDropDownList").value(null);
                                    //$("#excel_SalesType_id").data("kendoDropDownList").value(null);

                                    kendo.ui.progress($("#uploadExcelModal"), false);
                                    $("#uploadExcelModal").modal("hide");

                                    application.grid.refreshGrid($('#gridInventoryEntries'));
                                } else {
                                    $("#gridExcelUpload").data("kendoGrid").dataSource.data([]);
                                    $("#excelUpload").data("kendoUpload").clearAllFiles();
                                    kendo.ui.progress($("#uploadExcelModal"), false);
                                    alert(response.Message)

                                }
                            }).fail(inventoryentry.common.catchError);
                    }
                    else {
                        $("#gridExcelUpload").data("kendoGrid").dataSource.data([]);
                        $("#excelUpload").data("kendoUpload").clearAllFiles();
                        $("#excel_location_id").data("kendoDropDownList").value(null);

                    }
                }
            }
        },
        services: {
            deleteinventoryentry: function(data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "inventory_entries/delete"; // pending action method
                return $.post(url, data);
            },
            uploadInventoryEntries: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "inventory_entries/upload";
                return $.post(url, data);
            }
        }
    },
    common: {
        catchError: function (x, t, e) {
            $('#systemSpinner').hide();
            console.log('error');
            console.log(e);
            let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
          
            window.alert(msg, t);
        },
        errorHandler: function (e) {
            console.log(e.errors)
           
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
                    alert(e.response.Message);
                    var grid = $("#gridInventoryEntries").data("kendoGrid");
                    grid.one("dataBinding", function (args) {
                        args.preventDefault();
                        kendo.ui.progress($("#gridInventoryEntries"), false);
                    });
                } else {
                    alert("Inventory Entry successfully saved!");
                    application.grid.refreshGrid($('#gridInventoryEntries'))
                }
            } else if (e.type == "update") {
                if (e.response.Success == false) {
                    alert(e.response.Message);
                    var grid = $("#gridInventoryEntries").data("kendoGrid");
                    grid.one("dataBinding", function (args) {
                        args.preventDefault();
                        kendo.ui.progress($("#gridInventoryEntries"), false);
                    });
                } else {
                    alert("Inventory Entry successfully updated!");
                    application.grid.refreshGrid($('#gridInventoryEntries'))
                }
               
            }
        }
    }

}