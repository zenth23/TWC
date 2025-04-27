Location = {
    initialize: function () {
        //this.common.error_handler()

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
            onOpenModalLocation: function (id) {
                $(`#Location_Id`).val('')
                $(`#Location_UniqueKey`).val('')
                //$(`#Location_Name`).val('')
                $(`#Location-errormessage`).empty()


                if (id === 0) {
                    $(`#modalLocationTitle`).html('Add New Location ')
                    $(`#Location_Id`).val(id)
                } else {
                    $(`#modalLocationTitle`).html('Update Location ')

                    Location
                    .editor
                    .services
                    .getApplicationList(id)
                    .done(function (result) {
                        $(`#Location_Id`).val(result.Id)
                        $(`#Location_UniqueKey`).val(result.UniqueKey)
                        //$(`#Location_Name`).val(result.Name)


                    })
                    .fail(Location.common.catchError)
                }

                $(`#modalLocation`).modal('show')
            },
            onOpenModalApplication: function (id) {
                $(`#Location_Id`).val('')
                $(`#Location_UniqueKey`).val('')
                //$(`#Location_Name`).val('')
                $(`#Location-errormessage`).empty()
                $(`#tblApplication tbody`).empty()

                if (id === 0) {
                    $(`#modalLocationTitle`).html('Add New Location ')
                    $(`#Location_Id`).val(id)
                } else {
                    $(`#modalLocationTitle`).html('Update Location ')

                    Location
                    .editor
                    .services
                    .getApplicationList(id)
                    .done(function (result) {
                        $(`#Location_Id`).val(result.Id)
                        $(`#Location_UniqueKey`).val(result.UniqueKey)
                        //$(`#Location_Name`).val(result.Name)


                    })
                    .fail(Location.common.catchError)
                }

                $(`#modalApplication`).modal('show')
            },
            onSubmitLocation: function (e) {
                e.preventDefault()
                debugger
                var id = $(`#Location_Id`).val()
                var code = $(`#Location_UniqueKey`).val()
                //var name = $(`#Location_Name`).val()

                $(`#Location-errormessage`).empty()

                var isValid = true

                if (!code) {
                    $(`#Location-errormessage`).append(`<li>Code is required</li>`);
                    isValid = false;
                }

                if (!name) {
                    $(`#Location-errormessage`).append(`<li>Name is required</li>`);
                    isValid = false;
                }

                if (isValid) {
                    confirm("Confirmation", "Are you sure you want to save?").done(function () {
                        let formData = $(`#frmLocation`).serialize()

                        Location.editor.services.saveLocation(formData)
                        .done(function (result) {
                            if (result.Success) {
                                alert("Location  is saved.");
                                //Location.initialize()
                                //Location.editor.common.loadApplication(id)
                                application.grid.refreshGrid($('#gridLocation'))
                                $(`#Location_Id`).val('')
                                $(`#Location_UniqueKey`).val('')
                                //$(`#Location_Name`).val('')
                                $(`#Location-errormessage`).empty()
                                $(`#tblApplication tbody`).empty()
                                $(`#modalLocation`).modal('hide')
                            } else {
                                alert(result.Message)
                            }
                        })
                        .fail(Location.common.catchError);
                    })
                }
            },
            onLocationTabClick: function (id) {
                Location.editor.common.loadApplication(id)
            },
            onLocationDivClick: function (selected) {
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
                window.location = `${window.rootUrl}Locations/details?id=${id}`
            },
            onGridDeleteClick: function (e) {
                e.preventDefault()
                var trElement = e.target.closest("td")
                var id = trElement.getAttribute("data-id")

                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    Location.editor.services.deleteLocation(data)
                     .done(function (response) {
                         if (response.Success) {

                             alert("Location  deleted.");
                             application.grid.refreshGrid($('#gridLocation'))
                         } else {
                             alert(response.Message);
                         }
                     }).fail(Location.common.catchError);
                })
            }
        },
        common: {
            loadLocation: function () {

            },
            loadApplication: function (id) {
                var appendToDiv = ``;
                appendToDiv += `<div class='list-group-item list-group-item-action'>
                                <div class='row'>
                                   <div class='col-sm-8'>
                                       <a id='tab-{id}' style='cursor:pointer' data-toggle='list' role='tab'>{code} - {name}</a>
                                   </div>
                                   <div class='col-sm-4 text-right'>
                                       <button class ='btn btn-sm btn-outline-primary' onclick='Location.editor.events.onOpenModalLocation({id})'><span class ='fas fa-edit'></span></button>
                                       <button class='btn btn-sm btn-outline-warning' onclick='Location.editor.events.onDeleteApplication({id})'><span class="fas fa-trash"></span></button>
                                   </div>
                                </div>
                            </div>`;

                var appListDiv = $("#tabApplication")
                var url = `${window.rootUrl}Locations/GetApplications?LocationId=${id}`
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
            getLocation: function (id) {
                let url = `${window.rootUrl}Locations/getLocation?id=${id}`;
                return $.getJSON(url);
            },
            getApplicationList: function (id) {
                let url = `${window.rootUrl}Locations/getapplicationlist?id=${id}`;
                return $.getJSON(url);
            },
            saveLocation: function (data) {
                let token = $(`#frmLocation`).find(`input[name="__RequestVerificationToken"]`).val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = `${window.rootUrl}Locations/SaveLocation`;
                return $.post(url, data);
            },
            deleteLocation: function (data) {
                let token = $('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Locations/DeleteLocation";
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


            $("input.text-box").addClass("k-textbox")
        },
        onRequestEnd: function (e) {
            if (e.type == "create") {
                if (e.response.Success == false) {
                    alert(e.response.Message);
                    var grid = $("#gridLocation").data("kendoGrid");
                    grid.one("dataBinding", function (args) {
                        args.preventDefault();
                        kendo.ui.progress($("#gridLocation"), false);
                    });
                } else {
                    alert("Location successfully saved!")
                    application.grid.refreshGrid($('#gridLocation'))
                }
            } else if (e.type == "update") {
                if (e.response.Success == false) {
                    alert(e.response.Message);
                    var grid = $("#gridLocation").data("kendoGrid");
                    grid.one("dataBinding", function (args) {
                        args.preventDefault();
                        kendo.ui.progress($("#gridLocation"), false);
                    });
                } else {
                    alert("Location successfully updated!")
                    application.grid.refreshGrid($('#gridLocation'))
                }
            }
        }
    }
}