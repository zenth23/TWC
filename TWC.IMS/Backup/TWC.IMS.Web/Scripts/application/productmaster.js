productmaster = {
    initialize: function () {
        //this.common.error_handler()
    },
    editor: {
        events: {
            onOpenModalproductmaster: function (id) {
                $(`#productmaster_Id`).val('')
                $(`#productmaster_UniqueKey`).val('')
                //$(`#productmaster_Name`).val('')
                $(`#productmaster-errormessage`).empty()
               

                if (id === 0) {
                    $(`#modalproductmasterTitle`).html('Add New productmaster ')
                    $(`#productmaster_Id`).val(id)
                } else {
                    $(`#modalproductmasterTitle`).html('Update productmaster ')

                    productmaster
                    .editor
                    .services
                    .getApplicationList(id)
                    .done(function (result) {
                        $(`#productmaster_Id`).val(result.Id)
                        $(`#productmaster_UniqueKey`).val(result.UniqueKey)
                        //$(`#productmaster_Name`).val(result.Name)

                       
                    })
                    .fail(productmaster.common.catchError)
                }

                $(`#modalproductmaster`).modal('show')
            },
            onOpenModalApplication: function (id) {
                $(`#productmaster_Id`).val('')
                $(`#productmaster_UniqueKey`).val('')
                //$(`#productmaster_Name`).val('')
                $(`#productmaster-errormessage`).empty()
                $(`#tblApplication tbody`).empty()

                if (id === 0) {
                    $(`#modalproductmasterTitle`).html('Add New productmaster ')
                    $(`#productmaster_Id`).val(id)
                } else {
                    $(`#modalproductmasterTitle`).html('Update productmaster ')

                    productmaster
                    .editor
                    .services
                    .getApplicationList(id)
                    .done(function (result) {
                        $(`#productmaster_Id`).val(result.Id)
                        $(`#productmaster_UniqueKey`).val(result.UniqueKey)
                        //$(`#productmaster_Name`).val(result.Name)

                       
                    })
                    .fail(productmaster.common.catchError)
                }

                $(`#modalApplication`).modal('show')
            },
            onSubmitproductmaster: function (e) {
                e.preventDefault()
                debugger
                var id = $(`#productmaster_Id`).val()
                var code = $(`#productmaster_UniqueKey`).val()
                //var name = $(`#productmaster_Name`).val()

                $(`#productmaster-errormessage`).empty()

                var isValid = true

                if (!code) {
                    $(`#productmaster-errormessage`).append(`<li>Code is required</li>`);
                    isValid = false;
                }

                if (!name) {
                    $(`#productmaster-errormessage`).append(`<li>Name is required</li>`);
                    isValid = false;
                }

                if (isValid) {
                    confirm("Confirmation", "Are you sure you want to save?").done(function () {
                        let formData = $(`#frmproductmaster`).serialize()

                        productmaster.editor.services.saveProductMaster(formData)
                        .done(function (result) {
                            if (result.Success) {
                                alert("productmaster  is saved.");
                                //productmaster.initialize()
                                //productmaster.editor.common.loadApplication(id)
                                application.grid.refreshGrid($('#gridProductMaster'))
                                $(`#productmaster_Id`).val('')
                                $(`#productmaster_UniqueKey`).val('')
                                //$(`#productmaster_Name`).val('')
                                $(`#productmaster-errormessage`).empty()
                                $(`#tblApplication tbody`).empty()
                                $(`#modalproductmaster`).modal('hide')
                            } else {
                                alert(result.Message)
                            }
                        })
                        .fail(productmaster.common.catchError);
                    })
                }
            },
            onproductmasterTabClick: function (id) {
                productmaster.editor.common.loadApplication(id)
            },
            onproductmasterDivClick: function (selected) {
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
                window.location = `${window.rootUrl}product_master/details?id=${id}`
            },
            onGridDeleteClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")

                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    productmaster.editor.services.deleteproductmaster(data)
                     .done(function (response) {
                         if (response.Success) {

                             alert("productmaster  deleted.");
                             application.grid.refreshGrid($('#gridProductMaster'))
                         } else {
                             alert(response.Message);
                         }
                     }).fail(productmaster.common.catchError);
                })
            },
            onImageGalleryModalOpen: function(e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")
                
                var dataItem = $("#gridProductMaster").data("kendoGrid").dataSource.get(id);
                $(".modal-title").html(dataItem.product_name);

                $("#hiddenSelectedProduct").val(id);
                $("#gridProductMasterImages").data("kendoGrid").dataSource.read();

                $("#imageGalleryModal").modal("show");
            },
            onImageSelected: function(e) {
                e.preventDefault()

                var dataItem = e.sender.dataItem(e.sender.select());

                $("#image-toolbar").show();
                $("#image-holder").show();

                // $("#IsMain")[0].checked = dataItem.IsMain;
                // $("input[name='IsMain']").val(dataItem.IsMain ? "true" : "false");

                $("#image-holder").html("<img id='product-image' img-id='"+dataItem.Id+"' src='"+dataItem.FilePath+"' class='product-img' />")
            },
            onImageGalleryModalClose: function(e) {
                $("#hiddenSelectedProduct").val("");
                $("#imageGalleryModal").modal("hide");

                $("#image-toolbar").hide();
                $("#image-holder").hide();

                $("#imageUpload").data("kendoUpload").clearAllFiles();
            },

            onImageDelete: function(e) {
                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    var id = $("#product-image").attr("img-id");
                    productmaster.editor.services.deleteImage({id: id})
                    .done(function (response) {
                        if (response.Success) {
                            
                            application.grid.refreshGrid($('#gridProductMasterImages'))

                            $("#image-toolbar").hide();
                            $("#image-holder").hide();

                            alert("Image deleted.");
                            
                        } else {
                            alert(response.Message);
                        }
                    }).fail(productmaster.common.catchError);
                })
            },
            onSetImageAsPreview: function(e) {
                confirm("Confirmation", "Are you sure you want to set as preview image?").done(function () {
                    var id = $("#product-image").attr("img-id");
                    productmaster.editor.services.setAsPreview({id: id})
                    .done(function (response) {
                        if (response.Success) {
                            application.grid.refreshGrid($('#gridProductMasterImages'))
                            //alert("Set as preview image successful.");
                            
                        } else {
                            alert(response.Message);
                        }
                    }).fail(productmaster.common.catchError);
                })
            },
            onUploadComplete: function(e) {
                $(".k-upload-status").remove();
                application.grid.refreshGrid($('#gridProductMasterImages'))
            }
            
        },
        common: {
            loadproductmaster: function () {
              
            },
            loadApplication: function (id) {
                var appendToDiv = ``;
                appendToDiv += `<div class='list-group-item list-group-item-action'>
                                <div class='row'>
                                   <div class='col-sm-8'>
                                       <a id='tab-{id}' style='cursor:pointer' data-toggle='list' role='tab'>{code} - {name}</a>
                                   </div>
                                   <div class='col-sm-4 text-right'>
                                       <button class ='btn btn-sm btn-outline-primary' onclick='productmaster.editor.events.onOpenModalproductmaster({id})'><span class ='fas fa-edit'></span></button>
                                       <button class='btn btn-sm btn-outline-warning' onclick='productmaster.editor.events.onDeleteApplication({id})'><span class="fas fa-trash"></span></button>
                                   </div>
                                </div>
                            </div>`;

                var appListDiv = $("#tabApplication")
                var url = `${window.rootUrl}product_master/GetApplications?productmasterId=${id}`
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
            getproductmaster: function (id) {
                let url = `${window.rootUrl}product_master/getproductmaster?id=${id}`;
                return $.getJSON(url);
            },
            getApplicationList: function (id) {
                let url = `${window.rootUrl}product_master/getapplicationlist?id=${id}`;
                return $.getJSON(url);
            },
            saveProductMaster: function (data) {
                let token = $(`#frmproductmaster`).find(`input[name="__RequestVerificationToken"]`).val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = `${window.rootUrl}product_master/SaveProductMaster`;
                return $.post(url, data);
            },
            deleteproductmaster: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "product_master/DeleteProductMaster";
                return $.post(url, data);
            },
            deleteImage: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "products/RemoveImage";
                return $.post(url, data);
            },
            setAsPreview: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "products/SetAsPrevImage";
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
                    alert("Product Master successfully saved!")
                }
                application.grid.refreshGrid($('#gridProductMaster'))
            } else if (e.type == "update") {
                if (e.response.Success == false) {
                    alert(e.response.Message)
                } else {
                    alert("Product Master successfully updated!")
                }
                application.grid.refreshGrid($('#gridProductMaster'))
            }
        },
        uploadParam: function(e) {
             e.data = { id: $("#hiddenSelectedProduct").val()};
        },
        imageGalleryParam: function(e) {
           return { id: $("#hiddenSelectedProduct").val()};
        }
    }
}