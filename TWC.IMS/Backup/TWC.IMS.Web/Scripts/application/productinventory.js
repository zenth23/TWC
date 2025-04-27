productinventory = {
    initialize: function () {
        //this.common.error_handler()
    },
    editor: {
        events: {
            onOpenModalproductinventory: function (id) {
                $(`#productinventory_Id`).val('')
                $(`#productinventory_UniqueKey`).val('')
                //$(`#productinventory_Name`).val('')
                $(`#productinventory-errormessage`).empty()
               

                if (id === 0) {
                    $(`#modalproductinventoryTitle`).html('Add New productinventory ')
                    $(`#productinventory_Id`).val(id)
                } else {
                    $(`#modalproductinventoryTitle`).html('Update productinventory ')

                    productinventory
                    .editor
                    .services
                    .getApplicationList(id)
                    .done(function (result) {
                        $(`#productinventory_Id`).val(result.Id)
                        $(`#productinventory_UniqueKey`).val(result.UniqueKey)
                        //$(`#productinventory_Name`).val(result.Name)

                       
                    })
                    .fail(productinventory.common.catchError)
                }

                $(`#modalproductinventory`).modal('show')
            },
            onOpenModalApplication: function (id) {
                $(`#productinventory_Id`).val('')
                $(`#productinventory_UniqueKey`).val('')
                //$(`#productinventory_Name`).val('')
                $(`#productinventory-errormessage`).empty()
                $(`#tblApplication tbody`).empty()

                if (id === 0) {
                    $(`#modalproductinventoryTitle`).html('Add New productinventory ')
                    $(`#productinventory_Id`).val(id)
                } else {
                    $(`#modalproductinventoryTitle`).html('Update productinventory ')

                    productinventory
                    .editor
                    .services
                    .getApplicationList(id)
                    .done(function (result) {
                        $(`#productinventory_Id`).val(result.Id)
                        $(`#productinventory_UniqueKey`).val(result.UniqueKey)
                        //$(`#productinventory_Name`).val(result.Name)

                       
                    })
                    .fail(productinventory.common.catchError)
                }

                $(`#modalApplication`).modal('show')
            },
            onSubmitproductinventory: function (e) {
                e.preventDefault()
                debugger
                var id = $(`#productinventory_Id`).val()
                var code = $(`#productinventory_UniqueKey`).val()
                //var name = $(`#productinventory_Name`).val()

                $(`#productinventory-errormessage`).empty()

                var isValid = true

                if (!code) {
                    $(`#productinventory-errormessage`).append(`<li>Code is required</li>`);
                    isValid = false;
                }

                if (!name) {
                    $(`#productinventory-errormessage`).append(`<li>Name is required</li>`);
                    isValid = false;
                }

                if (isValid) {
                    confirm("Confirmation", "Are you sure you want to save?").done(function () {
                        let formData = $(`#frmproductinventory`).serialize()

                        productinventory.editor.services.saveProductInventory(formData)
                        .done(function (result) {
                            if (result.Success) {
                                alert("productinventory  is saved.");
                                //productinventory.initialize()
                                //productinventory.editor.common.loadApplication(id)
                                application.grid.refreshGrid($('#gridProductInventory'))
                                $(`#productinventory_Id`).val('')
                                $(`#productinventory_UniqueKey`).val('')
                                //$(`#productinventory_Name`).val('')
                                $(`#productinventory-errormessage`).empty()
                                $(`#tblApplication tbody`).empty()
                                $(`#modalproductinventory`).modal('hide')
                            } else {
                                alert(result.Message)
                            }
                        })
                        .fail(productinventory.common.catchError);
                    })
                }
            },
            onproductinventoryTabClick: function (id) {
                productinventory.editor.common.loadApplication(id)
            },
            onproductinventoryDivClick: function (selected) {
                $(`.selectable`).removeClass(`selected`)

                $(selected).addClass(`selected`)

                if ($(selected).hasClass('selected')) {
                    $(selected).removeClass(`selected`)
                } else {
                    $(selected).addClass(`selected`)
                }
            },
            onGridAddClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")
                window.location = `${window.rootUrl}product_inventory/details?id=${id}`
            },
            onGridDeleteClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")

                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    productinventory.editor.services.deleteproductinventory(data)
                     .done(function (response) {
                         if (response.Success) {

                             alert("productinventory  deleted.");
                             application.grid.refreshGrid($('#gridProductInventory'))
                         } else {
                             alert(response.Message);
                         }
                     }).fail(productinventory.common.catchError);
                })
            }
        },
        common: {
            loadproductinventory: function () {
              
            },
            loadApplication: function (id) {
                var appendToDiv = ``;
                appendToDiv += `<div class='list-group-item list-group-item-action'>
                                <div class='row'>
                                   <div class='col-sm-8'>
                                       <a id='tab-{id}' style='cursor:pointer' data-toggle='list' role='tab'>{code} - {name}</a>
                                   </div>
                                   <div class='col-sm-4 text-right'>
                                       <button class ='btn btn-sm btn-outline-primary' onclick='productinventory.editor.events.onOpenModalproductinventory({id})'><span class ='fas fa-edit'></span></button>
                                       <button class='btn btn-sm btn-outline-warning' onclick='productinventory.editor.events.onDeleteApplication({id})'><span class="fas fa-trash"></span></button>
                                   </div>
                                </div>
                            </div>`;

                var appListDiv = $("#tabApplication")
                var url = `${window.rootUrl}product_inventory/GetApplications?productinventoryId=${id}`
                kendo.ui.progress(appListDiv, true)

                $.getJSON(url)
                 .done(function (response) {

                     appListDiv.html("");
                     for (var i = 0; i < response.length; i++) {
                         var templateValue = appendToDiv.replace(/{id}/g, response[i].Id)
                         templateValue = templateValue.replace(/{code}/g, response[i].Code)
                         templateValue = templateValue.replace(/{name}/g, response[i].Name)

                         appListDiv.append(templateValue)
                     }

                     kendo.ui.progress(appListDiv, false)
                 })
                 .fail(function (x, t, e) {
                     kendo.ui.progress(appListDiv, false)

                     console.log({ x: x, t: t, e: e })
                     let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message
                     alert(msg)
                 })
            }
        },
        services: {
            getproductinventory: function (id) {
                let url = `${window.rootUrl}product_inventory/getproductinventory?id=${id}`;
                return $.getJSON(url);
            },
            getApplicationList: function (id) {
                let url = `${window.rootUrl}product_inventory/getapplicationlist?id=${id}`;
                return $.getJSON(url);
            },
            saveProductInventory: function (data) {
                let token = $(`#frmproductinventory`).find(`input[name="__RequestVerificationToken"]`).val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = `${window.rootUrl}product_inventory/SaveProductInventory`;
                return $.post(url, data);
            },
            deleteproductinventory: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "product_inventory/DeleteProductInventory";
                return $.post(url, data);
            },
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
        fixElementSequence: function (element) {
            var count = 0

            $(element).each(function (i) {
                $('input, select, textarea', $(this)).each(function () {
                    var input_id = $(this).attr("Id") == "" ? "" : $(this).attr("Id")
                    var input_name = $(this).attr("UniqueKey") == "" ? "" : $(this).attr("UniqueKey")
                    var area_describedby = $(this).attr("area-describedby") == "" ? "" : $(this).attr("area-describedby")
                    var start1 = input_id.indexOf("[") + 1
                    var end1 = input_id.indexOf("]")
                    var start2 = input_name.indexOf("[") + 1
                    var end2 = input_name.indexOf("]")
                    var start3 = area_describedby.indexOf("[") + 1
                    var end3 = area_describedby.indexOf("]")

                    input_id = input_id.length > 0 ? input_id.replace(input_id.substring(start1, end1), count) : ""
                    input_name = input_name.length > 0 ? input_name.replace(input_name.substring(start2, end2), count) : ""
                    area_describedby = area_describedby.length > 0 ? area_describedby.replace(area_describedby.substring(start3, end3), count) : ""

                    $(this).attr({ id: input_id, name: input_name, "area-describedby": area_describedby })
                    $(this).trigger("change")
                })

                $('label', $(this)).each(function () {
                    var label_id = $(this).attr("for") == "" ? "" : $(this).attr("for")
                    var start1 = label_id.indexOf("[") + 1
                    var end1 = label_id.indexOf("]")

                    label_id = label_id.length > 0 ? label_id.replace(label_id.substring(start1, end1), count) : ""
                    label_id.length > 0 ? $(this).attr({ 'for': label_id }) : ""
                })

                $('span', $(this)).each(function () {
                    var span_id = $(this).attr("data-valmsg-for") == "" ? "" : $(this).attr("data-valmsg-for")
                    var span_id2 = $(this).attr("id") == "" ? "" : $(this).attr("id")
                    var start1 = span_id.indexOf("[") + 1
                    var end1 = span_id.indexOf("]")
                    var start1 = span_id2.indexOf("[") + 1
                    var end1 = span_id2.indexOf("]")

                    span_id = span_id.length > 0 ? span_id.replace(span_id.substring(start1, end1), count) : ""
                    span_id2 = span_id2.length > 0 ? span_id2.replace(span_id2.substring(start2, end2), count) : ""

                    span_id.length > 0 ? $(this).attr({ 'data-valmsg-for': span_id }) : ""
                    span_id2.length > 0 ? $(this).attr({ 'id': span_id2 }) : ""
                })
                count++
            })
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
        onSort: function (e) {
            var gridData = e.sender.dataSource.data()
            gridData.forEach(function (element) {
                if (!element.Code) {
                    e.preventDefault()
                }
            });
        },
        onEdit: function (e) {
            var uButton = e.container.find(".k-button.k-grid-update"); //update button
            var cButton = e.container.find(".k-button.k-grid-cancel"); //cancel button

            uButton.html(`<span class="fas fa-save"></span>`)
            cButton.html(`<span class="fas fa-ban"></span>`)
        },
        onRequestEnd: function (e) {
            if (e.type == "create") {
                if (e.response.Success == false) {
                    alert(e.response.Message)
                } else {
                    alert("Product Inventory successfully saved!")
                }
                application.grid.refreshGrid($('#gridProductInventory'))
            } else if (e.type == "update") {
                if (e.response.Success == false) {
                    alert(e.response.Message)
                } else {
                    alert("Product Inventory successfully updated!")
                }
                application.grid.refreshGrid($('#gridProductInventory'))
            }
        }
    }
}