var checkedIds = {};
productinventory = {
    initialize: function () {
        //productinventory.editor.events.onKendoGridRowValidate()


    },
    editor: {
        events: {
            onKendoGridEdit: function (e) {
                var uButton = e.container.find(".k-button.k-grid-update"); //update button
                var cButton = e.container.find(".k-button.k-grid-cancel"); //cancel button


                uButton.attr("title", "Save");
                cButton.attr("title", "Cancel");

                uButton.html(`<span class="fas fa-save" title = "Save"></span>`)
                cButton.html(`<span class="fas fa-ban" title = "Cancel"></span>`)

                //let ProductInventoryId = $(`#Id`).val()
                //let model = e.model;
                ////if (model.isNew()) {
                //    e.container.find("input[name=ProductInventoryUser_ProductInventory]").val(ProductInventoryId).trigger("change")
                //}
            },
            onGridDeleteClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")

                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    productinventory.editor.services.deleteProductInventory(data)
                     .done(function (response) {
                         if (response.Success) {

                             alert("User deleted.");
                             application.grid.refreshGrid($('#gridProductInventory'))
                         } else {
                             alert(response.Message);
                         }
                     }).fail(productinventory.common.catchError);
                })
            },
            onGridAddClick: function (e) {
                var tdElement = e.target.closest("td")
                var id = tdElement.getAttribute("data-id")

                productinventory.editor.events.onOpenModal(id)

            },
            onOpenModal: function (id) {
                var _modal = $(`#modalApplicationUser`)
                $('[name="User.Id"]').val(id)

                productinventory.editor.events.onGridUserLoad(id)

                _modal.modal("show")
            },
            onReturnFalse: function () {
                return false;
            },
            onSetValue: function (input) {
                var grid = $("#grid").data("kendoGrid");
                var dataItem = grid.dataItem($(input).closest("tr"));
                var checked = $(input)[0].checked;
                dataItem.set("IsSelected", checked);
            },
            onGridUserLoad: function (id) {
                dataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: `${window.rootUrl}product_inventory/GetApplicationsUser?id=${id}`,
                            dataType: "jsonp"
                        },
                        update: {
                            url: `${window.rootUrl}product_inventory/UpdateApplicationUsers`,
                            dataType: "jsonp"
                        },
                        parameterMap: function (options, operation) {
                            if (operation !== "read" && options.models) {
                                return { models: kendo.stringify(options.models) };
                            }
                        }
                    },
                    batch: true,
                    pageSize: 20,
                    schema: {
                        model: {
                            id: "Id",
                            fields: {
                                ApplicationCode: { editable: false },
                                ApplicationName: { editable: false }
                            }
                        }
                    }
                });

                $("#grid").kendoGrid({
                    dataSource: dataSource,
                    pageable: true,
                    height: 430,
                    toolbar: ["save", "cancel"],
                    columns: [
                        { template: '#=productinventory.editor.events.onDirtyField(data,"IsSelected")#<input type="checkbox" #= IsSelected ? \'checked="checked"\' : "" # class="chkbx k-checkbox k-checkbox-md k-rounded-md" />', title: "IsSelected", width: 110, attributes: { class: "k-text-center" } },
                        "ApplicationCode",
                        "ApplicationName",
                    ],
                    editable: true
                });

                $("#grid .k-grid-content").on("change", "input.chkbx", function (e) {
                    var grid = $("#grid").data("kendoGrid"),
                        dataItem = grid.dataItem($(e.target).closest("tr"));

                    dataItem.set("Discontinued", this.checked);
                });
            },
            onDirtyField: function (data, fieldName) {
                var hasClass = $("[data-uid=" + data.uid + "]").find(".k-dirty-cell").length < 1;
                if (data.dirty && data.dirtyFields[fieldName] && hasClass) {
                    return "<span class='k-dirty'></span>"
                }
                else {
                    return "";
                }
            }
        },
        services: {
            deleteProductInventory: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = `${window.rootUrl}product_inventory/deletesubscriberuser`;
                return $.post(url, data);
            },
            getUserApplications: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = `${window.rootUrl}product_inventory/getuserapplications`;
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
        onKendoGridError: function (e) {
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
                    alert("User successfully saved!")
                }
                application.grid.refreshGrid($('#gridProductInventory'))
            } else if (e.type == "update") {
                if (e.response.Success == false) {
                    alert(e.response.Message)
                } else {
                    alert("User successfully updated!")
                }
                application.grid.refreshGrid($('#gridProductInventory'))
            }
        }
    }
}