
// EDITOR - OPTIONAL
$.extend(workflow, {
    editor: {
        _selected: {
            workflowId: 0,
            entityId: 0,
        },
        initialize: function () {
            this.common.loadWorkflowEntities();
            this.common.loadCriteriaEvents();
        },
        events: {
            onTabClick: function (entityId) {

                if (workflow.editor._selected.entityId != entityId) {
                    workflow.editor._selected.entityId = entityId;
                    workflow.generateUI(entityId);
                }
            },
            allowDrop: function (ev) {
                ev.preventDefault();
            },
            dragEnter: function (ev) {
                ev.preventDefault();

                $(".bg-div-drag").removeClass("bg-div-drag");
                workflow._options.editor.counter++;
                $(ev.currentTarget).addClass("bg-div-drag");
            },
            dragLeave: function (ev) {
                ev.preventDefault();
                workflow._options.editor.counter--;
                if (workflow._options.editor.counter <= 0)
                    $(ev.currentTarget).removeClass("bg-div-drag");
            },
            drag: function (ev) {
                ev.dataTransfer.setData("text", ev.target.id);
            },
            drop: function (ev) {
                ev.preventDefault();

                $(".bg-div-drag").removeClass("bg-div-drag");
                workflow._options.editor.counter = 0;
                let data = ev.dataTransfer.getData("text");
                let slit = data.split('_');
                let workflowId = $("#" + data).data("workflowid");
                let sortIndex = $(ev.currentTarget).data("sortindex");
                if (slit[1] != sortIndex) {
                    let dataParam = { sortIndex: sortIndex, workflowId: workflowId }
                    workflow.editor.services.addWfBoxToSortIndex(dataParam)
                        .done(function(response) {
                            if (response.Success) {
                                workflow.generateUI(workflow.editor._selected.entityId);
                            }
                            else {
                                alert(response.Message);
                            }
                        }).fail(workflow.common.catchError);
                }
            },
            approvalBoxOnSelect: function (e) {
                if (e.target.parentElement.localName != 'a') {
                    let selected = $(".card-selected");
                    let removeSelected = false;
                    selected.each(function (index, value) {
                        $(value).removeClass("card-selected");
                        if ($(e.currentTarget).attr("id") == $(value).attr("id")) {
                            removeSelected = true;
                        }
                    })
                    if (removeSelected == false) {
                        $(e.currentTarget).addClass("card-selected");
                        workflow.editor._selected.workflowId = $(e.currentTarget).data("workflowid");
                        $("#btnDelete").attr("disabled", false);
                        $("#btnEdit").attr("disabled", false);
                    }
                    else {
                        workflow.editor._selected.workflowId = 0;
                        $("#btnDelete").attr("disabled", true);
                        $("#btnEdit").attr("disabled", true);
                    }
                }
            },
            onSaveWfBox: function () {

                window.confirm("Confirm", "Are you sure that you want to save workflow?")
                        .then(function () {
                            let validator = workflow.editor.validators.wfBoxModal();
                            let isValid = validator.validate();
                            let wfEntryModal = $(workflow._options.editor.modal);
                            if (isValid) {
                                let id = wfEntryModal.find('#Id').val();
                                let position = wfEntryModal.find('input[name="rbtnPosition"]:checked').val();
                                let label = wfEntryModal.find('#Label').val();
                                let roleDetail = wfEntryModal.find("#WorkflowSetup_RoleDetail").data('kendoComboBox');
                                let approvalType = wfEntryModal.find('#WorkflowSetup_ApprovalType').data('kendoComboBox');
                                let userDetail = wfEntryModal.find("#WorkflowSetup_UserDetail").data('kendoComboBox');
                                let statusAfterApproved = wfEntryModal.find("#WorkflowSetup_StatusAfterApproved").data('kendoComboBox');
                                let statusAfterRejected = wfEntryModal.find("#WorkflowSetup_StatusAfterRejected").data('kendoComboBox');
                                let rowVersion = wfEntryModal.find('#Base64RowVersion').val();
                                
                                let data = {
                                    Id: id,
                                    SelectedWorkflowId: workflow.editor._selected.workflowId,
                                    WorkflowEntityId: workflow.editor._selected.entityId,
                                    Position: position,
                                    ApprovalTypeId: approvalType.value(),
                                    RoleId: roleDetail.value(),
                                    Label: label,
                                    UserId: userDetail.value(),
                                    StatusAfterApproved: statusAfterApproved.value(),
                                    StatusAfterRejected: statusAfterRejected.value(),
                                    RowVersion: rowVersion
                                };
                                
                                workflow.editor.services.saveWfBox(data)
                                    .done(function(response) {
                                        if (response.Success) {
                                            wfEntryModal.modal('toggle');
                                            workflow.generateUI(workflow.editor._selected.entityId);

                                            workflow.editor._selected.workflowId = 0;
                                            $("#btnDelete").attr("disabled", true);
                                            $("#btnEdit").attr("disabled", true);
                                        }
                                        else {
                                            alert(response.Message);
                                        }
                                    }).fail(workflow.common.catchError);
                            }
                        })

                
            },
            onMoveWfBox: function (movementType, sortIndex, message) {

                // movementType
                // 0 = MOVE TO FIRST
                // 1 = MOVE TO LEFT
                // 2 = MOVE TO RIGHT
                // . = MOVE TO END

                let len = $(".cardholder").length;
                if (movementType <= 1 && sortIndex == 1) {
                    alert("Can't move any further.");
                }
                else if (movementType > 1 && sortIndex == len) {
                    alert("Can't move any further.");
                }
                else {
                    window.confirm("Confirm", "Are you sure that you want to move this " + message + "?")
                        .then(function () {
                            let data = {
                                sortIndex: sortIndex,
                                type: movementType,
                                workflowEntityId: workflow.editor._selected.entityId
                            };
                            
                            workflow.editor.services.moveWfBox(data)
                                .done(function(response) {
                                    if (response.Success) {
                                        workflow.generateUI(workflow.editor._selected.entityId)
                                    }
                                    else {
                                        alert(response.Message);
                                    }
                                })
                        })
                }
            },
            onAddWfBox: function () {
                if (workflow.editor._selected.workflowId == 0) {
                    $("#positionDiv").hide();
                }
                else {
                    $("#positionDiv").show();
                }

                $(workflow._options.editor.modal).modal('toggle');
                $('#modalWorkflowTitle').html("Add Workflow");

                $('#WorkflowSetup_WorkflowEntity').val(workflow.editor._selected.entityId);
                $('#Id').val(0);
                $('#Label').val("");
                $("#WorkflowSetup_RoleDetail").data('kendoComboBox').value(null);
                $('#WorkflowSetup_ApprovalType').data('kendoComboBox').value(null);
                $('#WorkflowSetup_StatusAfterApproved').data('kendoComboBox').value(null);
                $('#WorkflowSetup_StatusAfterRejected').data('kendoComboBox').value(null);

                $(".userdiv").hide();
                $('#WorkflowSetup_UserDetail').data("kendoComboBox").value(null);
            },
            onEditWfBox: function () {
                if (workflow.editor._selected.workflowId == 0) {
                    alert("Please select a card to edit.");
                }
                else {
                    let data = { workflowId: workflow.editor._selected.workflowId };
                    workflow.editor.services.getWfBox(data)
                        .done(function(response) {
                            $("#positionDiv").hide();
                            $('#modalWorkflowTitle').html("Edit Workflow");
                            $('#Id').val(response.Id);
                            $('#Label').val(response.Label);
                            $("#WorkflowSetup_RoleDetail").data('kendoComboBox').value(response.WorkflowSetup_RoleDetail);
                            $("#WorkflowSetup_UserDetail").data('kendoComboBox').dataSource.read();
                            $("#WorkflowSetup_UserDetail").data('kendoComboBox').value(response.WorkflowSetup_UserDetail);
                            $('#WorkflowSetup_ApprovalType').data('kendoComboBox').value(response.WorkflowSetup_ApprovalType);
                            $('#WorkflowSetup_StatusAfterApproved').data('kendoComboBox').value(response.WorkflowSetup_StatusAfterApproved);
                            $('#WorkflowSetup_StatusAfterRejected').data('kendoComboBox').value(response.WorkflowSetup_StatusAfterRejected);
                            $('#WorkflowSetup_WorkflowEntity').val(workflow.editor._selected.entityId);
                            $('#WorkflowSetup_ApprovalType').data('kendoComboBox').trigger("change");
                            $('#Base64RowVersion').val(response.Base64RowVersion);
                            console.log(response.Base64RowVersion)
                            $(workflow._options.editor.modal).modal('show');
                        }).fail(workflow.common.catchError);
                }
            },
            onDeleteWfBox: function () {
                if (workflow.editor._selected.workflowId == 0) {
                    alert("Please select a card to delete.");
                }
                else {
                    window.confirm("Confirm", "Are you sure that you want to delete this?").then(function () {
                        let data = { workflowId: workflow.editor._selected.workflowId }
                        workflow.editor.services.deleteWfBox(data)
                            .done(function(response) {
                                if (response.Success) {
                                    workflow.generateUI(workflow.editor._selected.entityId)
                                }
                                else {
                                    alert(response.Message);
                                }
                            }).fail(workflow.common.catchError);
                    })
                }
            },
            onDeleteAllWfBox: function (sortIndex) {
                if (sortIndex == 0) {
                    alert("Invalid sort index.");
                }
                else {
                    window.confirm("Confirm", "Are you sure that you want to delete these?").then(function () {
                        let data = { workflowEntityId: workflow.editor._selected.entityId, sortIndex: sortIndex };
                        workflow.editor.services.deleteAllWfBoxes(data)
                            .done(function(response) {
                                if (response.Success) {
                                    workflow.generateUI(workflow.editor._selected.entityId)
                                }
                                else {
                                    alert(response.Message);
                                }
                            })
                    })
                }
            },
            onSaveWfBlock: function (e) {
                e.preventDefault();
                $("#wfentity-errormessage").empty();

                let id = $("#WFBlockId").val();
                let name = $("#WFBlockName").val().trim();


                var criteria = $("#tblCriteria").find("tbody")
                                .children("tr")
                                .map(function(i, obj) {
                                    var name = $(obj).find("input[name='wfentity-name']").val().trim();
                                    var dataType = $(obj).find("input[name='wfentity-datatype']").data("kendoDropDownList").text();
                                    var valueObj = $(obj).find("input[name='wfentity-value']");
                                    var value = "";
                                    if(dataType === "System.DateTime") {
                                        var dateObj = valueObj.data("kendoDatePicker").value();
                                        value = commonHelper.extractDate(dateObj);
                                    } else if(dataType === "System.Boolean") {
                                        value = valueObj.data("kendoDropDownList").text();
                                    } else {
                                        value = valueObj.val().trim();
                                    }

                                    return {
                                        Name: name,
                                        Value: value,
                                        DataType: dataType
                                    }
                                }).toArray();

                let data = {
                    Id: id === null || id === "" || id === undefined ? 0 : id,
                    Name: name,
                    WorkflowEntityCriterias: criteria
                }

                // console.log(data); return;

                // VALIDATIONS
                var isValid = true;
                var isNotValid = commonHelper.isNullUndefinedOrWhiteSpace;
                if(isNotValid(data.Name)) {
                    $("#wfentity-errormessage").append("<li>Name is required</li>");
                    isValid = false;
                } 
                    
                if(data.WorkflowEntityCriterias.length > 0) {
                    var critIsValid = true;
                    for(var i = 0; i < data.WorkflowEntityCriterias.length; i++) {
                        if(isNotValid(data.WorkflowEntityCriterias[i].Name) 
                            || isNotValid(data.WorkflowEntityCriterias[i].Value) ) {
                            critIsValid = false; break;
                        }

                        if(data.WorkflowEntityCriterias[i].DataType === "System.DateTime") {

                        }
                    }

                    if(!critIsValid) {
                        $("#wfentity-errormessage").append("<li>Criteria is invalid</li>");
                        isValid = false;
                    }
                } else {
                    $("#wfentity-errormessage").append("<li>Criteria is required</li>");
                    isValid = false;
                }
                
                
                if(isValid) {
                    confirm("Confirmation", "Are you sure you want to save?").done(function () {
                        workflow.editor.services.saveWfBlock(data)
                        .done(function (response) {
                            if (response.Success) {
                                $("#modalBlockWorkflow").modal("hide");
                                alert("Workflow block saved.");
                                workflow.editor.common.loadWorkflowEntities();
                            } else {
                                alert(response.Message);
                            }
                        }).fail(workflow.common.catchError);

                        $("#WFBlockId").val("");
                        $("#WFBlockName").val("");
                        $("#tblCriteria").find("tbody").html("");
                    })
                }
            },
            onOpenModalWfBlock: function (id, title) {

                $("#WFBlockId").val("");
                $("#WFBlockName").val("");
                $("#tblCriteria").find("tbody").html("");
                $("#wfentity-errormessage").html("");

                var entityId = id === null || id === "" || id === undefined ? 0 : id;

                if (entityId === 0) {
                    $("#modalBlockWorkflowTitle").html("New Workflow Block");
                    $("#modalBlockWorkflow").modal("show");
                }
                else {
                    $("#modalBlockWorkflowTitle").html("Update Workflow Block")

                    workflow.editor
                            .services
                            .getWorkflowEntity(entityId)
                            .done(function(response) {
                                $("#WFBlockId").val(response.Id);
                                $("#WFBlockName").val(response.Name);
                                
                                var tbody = $("#tblCriteria").find("tbody");
                                for(var i = 0; i < response.WorkflowEntityCriterias.length; i++) {
                                    var name = response.WorkflowEntityCriterias[i].Name;
                                    var value = response.WorkflowEntityCriterias[i].Value;
                                    var dataType = response.WorkflowEntityCriterias[i].DataType;

                                    tbody.append("<tr><td><input value='"+dataType+"' name='wfentity-datatype' style='width:100% !important' /></td><td><input type='text' name='wfentity-name' value='"+name+"' class='form-control' /></td><td><input name='wfentity-value'  value='"+value+"'  type='text' class='form-control' /></td><td><button type='button' class='btn btn-light' onclick='workflow.editor.events.onRemoveCritFields(event, this)'><span class='fas fa-trash text-danger' ></span></button></td></tr>")
                                    
                                    var lastTR = tbody.find("tr:last");
                                    lastTR.find("input[name='wfentity-datatype']")
                                      .kendoDropDownList(workflow.editor.common.getDataTypeOptions());

                                    var inputVal = lastTR.find("input[name='wfentity-value']");

                                    if(dataType === "System.DateTime") {
                                        inputVal.removeClass("form-control")
                                        inputVal.attr("style", 'width:100% !important')
                                        inputVal.kendoDatePicker();
                                    } else if(dataType === "System.Boolean") {
                                        inputVal.removeClass("form-control")
                                        inputVal.attr("style", 'width:100% !important')
                                        inputVal.kendoDropDownList({
                                            dataSource: ["True", "False"]
                                        })
                                    } else if(dataType === "System.Int32") {
                                        inputVal.addClass("ctrl-int32");
                                    } else if(dataType === "System.Double") {
                                        inputVal.addClass("ctrl-double");
                                    }
                                }
                                $("#modalBlockWorkflow").modal("show");
                            })
                            .fail(workflow.common.catchError)

                    
                }

            },
            onDeleteWfBlock: function (id) {
                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    workflow.editor.services.deleteWfBlock(data)
                     .done(function (response) {
                         if (response.Success) {

                             alert("Workflow block deleted.");
                             workflow.editor.common.loadWorkflowEntities();
                             workflow.clear();
                         } else {
                             alert(response.Message);
                         }
                     }).fail(workflow.common.catchError);
                })
            },
            roleOnChange: function (e) {
                $('#WorkflowSetup_UserDetail').data("kendoComboBox").dataSource.read();
            },
            approvalTypeOnChange: function (e) {
                commonHelper.getConfigValue("APPROVALTYPE_EXCLUSIVE")
                    .done(function(response) {
                        if (e.sender.value() === response.Value)
                            $(".userdiv").fadeIn(500);
                        else {
                            $(".userdiv").hide();
                            $('#WorkflowSetup_UserDetail').data("kendoComboBox").value(null);
                        }
                        $('#WorkflowSetup_UserDetail').data("kendoComboBox").dataSource.read();
                    })
            },
            onChangeParallelType: function (e) {
                let comboBox = e.sender;
                let element = $(comboBox.element);
                let entityId = element.attr("entityId");
                let sortIndex = element.attr("sortIndex");
                let origValue = element.attr("origValue");
                let selectedValue = comboBox.value()

                if (comboBox.selectedIndex !== -1) {
                    var model = {
                        workflowEntityId: element.attr("entityId"),
                        sortIndex: element.attr("sortIndex"),
                        parTypeId: selectedValue
                    };
                   

                    kendo.ui.progress(element.closest("div.parHolder"), true);
                    
                    workflow.editor.services.changeParallelType(model)
                        .done(function (response) {
                            if (response.Success) {
                                // workflow.generateUI(workflow.editor._selected.entityId);
                                element.attr("origValue", selectedValue)
                                kendo.ui.progress(element.closest("div.parHolder"), false);
                            } else {
                                alert(response.Message);
                                kendo.ui.progress(element.closest("div.parHolder"), false);
                            }
                        }).fail(function (x, t, e) {
                            kendo.ui.progress(element.closest("div.parHolder"), false);
                            console.log({ x: x, t: t, e: e })
                            let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                            alert(msg);
                        })
                } else {
                    comboBox.value(origValue);
                }
            },
            onAddCriteriaFields: function(e, elem) {
                e.preventDefault();
                var tbody = $(elem).closest("div").find("tbody");
                tbody.append("<tr><td><input name='wfentity-datatype' /></td><td><input type='text' name='wfentity-name' class='form-control' /></td><td><input name='wfentity-value' type='text' class='form-control' /></td><td><button type='button' class='btn btn-light' onclick='workflow.editor.events.onRemoveCritFields(event, this)'><span class='fas fa-trash text-danger' ></span></button></td></tr>");

                tbody.find("tr:last")
                     .find("input[name='wfentity-datatype']")
                     .kendoDropDownList(workflow.editor.common.getDataTypeOptions());
            },
            onRemoveCritFields: function(e, elem) {
                e.preventDefault();
                $(elem).closest("tr").remove();
            }
        },
        common: {
            loadWorkflowEntities: function () {

                var template = '';
                template += '<div class="list-group-item list-group-item-action">';
                template += '   <div class="row">';
                template += '       <div class="col-sm-8">';
                template += '           <a id="tab-{id}" onclick="workflow.editor.events.onTabClick({id})" style="cursor:pointer" data-toggle="list" role="tab">{name}</a>';
                template += '       </div>';
                template += '       <div class="col-sm-4 text-right">';
                template += '           <button class="btn btn-sm btn-outline-primary"  onclick="workflow.editor.events.onOpenModalWfBlock({id}, ' + "'{name}'" + ')"><span class="fas fa-edit"></span></button>';
                template += '           <button class="btn btn-sm btn-outline-warning"  onclick="workflow.editor.events.onDeleteWfBlock({id})"><span class="fas fa-trash"></span></button>';
                template += '       </div>';
                template += '   </div>';
                template += '</div>';
    
                var wfBlockListDiv = $("#list-tab");
                var url = window.rootUrl + "workflow/GetWorkflowEntities";
                kendo.ui.progress(wfBlockListDiv, true);
    
                $.getJSON(url)
                 .done(function (response) {
    
                     wfBlockListDiv.html("");
                     for (var i = 0; i < response.length; i++) {
                         var templateValue = template.replace(/{id}/g, response[i].Id);
                         templateValue = templateValue.replace(/{name}/g, response[i].Name);
    
                         wfBlockListDiv.append(templateValue);
                     }
    
                     kendo.ui.progress(wfBlockListDiv, false);
    
                 })
                 .fail(function (x, t, e) {
                     kendo.ui.progress(wfBlockListDiv, false);
    
                     console.log({ x: x, t: t, e: e })
                     let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                     alert(msg);
                 })
            },
            getDataTypeOptions:  function() {
                return {
                    dataSource: {
                        transport: {
                            read: {
                                url: window.rootUrl + "common/getdatatypes",
                                dataType: "json"
                            }
                        }
                    },
                    change: function(e) {
                        var comboBox = e.sender;
                        var selectedValue = comboBox.text();
                        var elem = comboBox.wrapper;
                        
                        var inputVal = $(elem).closest("tr").find("input[name='wfentity-value']");
                        var td = inputVal.closest("td");
                        td.empty();
                        td.append("<input name='wfentity-value' type='text' class='form-control' />")

                        inputVal = $(elem).closest("tr").find("input[name='wfentity-value']");
                        if(selectedValue === "System.DateTime") {
                            inputVal.removeClass("form-control")
                            inputVal.attr("style", 'width:100% !important')
                            inputVal.kendoDatePicker();
                        } else if(selectedValue === "System.Boolean") {
                            inputVal.removeClass("form-control")
                            inputVal.attr("style", 'width:100% !important')
                            inputVal.kendoDropDownList({
                                dataSource: ["True", "False"]
                            })
                        } else if(selectedValue === "System.Int32") {
                            inputVal.addClass("ctrl-int32");
                        } else if(selectedValue === "System.Double") {
                            inputVal.addClass("ctrl-double");
                        }
                    }
                 }
            },
            isFloatKey: function(evt, dotCount) {
                var charCode = (evt.which) ? evt.which : evt.keyCode
                if (charCode == 46 && dotCount < 1)
                    return true;
        
                return !(charCode > 31 && (charCode < 48 || charCode > 57));
            },
            isIntKey: function (evt) {
                var charCode = (evt.which) ? evt.which : evt.keyCode
                return !(charCode > 31 && (charCode < 48 || charCode > 57));
            },
            loadCriteriaEvents: function() {
                
                var isFloatKey = workflow.editor.common.isFloatKey;
                var isIntKey = workflow.editor.common.isIntKey;
                $(".ctrl-double, .ctrl-int32").each(function () {
                    $(this).attr("autocomplete", "off")
                })
            
                // VALIDATE EVERY KEY TYPED IN NUMBER FIELDS
                $("body").on("keypress", ".ctrl-double", function (e) {
                    var dotCount = 0;
                    for (var i = 0; i < this.value.length; i++) {
                        if (this.value.charAt(i) == '.') {
                            dotCount++;
                            break;
                        }
                    }
            
                    var valid = isFloatKey(e, dotCount);
                    return valid;
                });
            
                $("body").on("keypress", ".ctrl-int32", function (e) {
                    var valid = isIntKey(e);
                    return valid;
                });
            
                $('body').on("paste", ".ctrl-double, .ctrl-int32", function (e) {
                    e.preventDefault();
                });
            }
        },
        filters: {
            searchKeyApprovalType: function () {
                let combobox = $('#WorkflowSetup_ApprovalType').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyRole: function () {
                let combobox = $('#WorkflowSetup_RoleDetail').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyUser: function () {
                let combobox = $('#WorkflowSetup_UserDetail').data("kendoComboBox");
                let roleId = $('#WorkflowSetup_RoleDetail').data("kendoComboBox").value();
                return { searchKey: combobox.text(), roleId: roleId };
            },
            searchKeyAfterApproved: function () {
                let combobox = $('#WorkflowSetup_StatusAfterApproved').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyAfterRejected: function () {
                let combobox = $('#WorkflowSetup_StatusAfterRejected').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyParallel: function (elem) {
                let combobox = $(elem).data("kendoComboBox");
                return { searchKey: combobox.text() };
            }
        },
        services: {
            saveWfBox: function(data) {
                let token = $(workflow._options.editor.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/SaveWorkflow";
                return $.post(url, data);
            },
            moveWfBox: function(data) {
                let token = $(workflow._options.editor.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/MoveWorkflow";
                return $.post(url, data);
            },
            addWfBoxToSortIndex: function(data) {
                let token = $(workflow._options.editor.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/AddWorkflowToSortIndex";
                return $.post(url, data);
            },
            getWfBox: function(data) {
                let url = window.rootUrl + "Workflow/GetWorkflowById";
                return $.getJSON(url, data);
            },
            deleteWfBox: function(data) {
                let token = $(workflow._options.editor.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/DeleteWorkflow";
                return $.post(url, data);
            },
            deleteAllWfBoxes: function(data){
                let token = $(workflow._options.editor.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/DeleteWorkflowBySortIndex";
                return $.post(url, data);
            },
            changeParallelType: function(data){
                let token = $(workflow._options.editor.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/UpdateParallelTypeBySortIndex";
                return $.post(url, data);
            },
            saveWfBlock: function(data){
                let token = $("#workflowblockform").find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/saveworkflowentity";
                return $.post(url, data);
            },
            deleteWfBlock: function(data){
                let token = $("#workflowblockform").find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "Workflow/deleteworkflowentity";
                return $.post(url, data);
            },
            getWorkflowEntity: function(id) {
                let url = window.rootUrl + "Workflow/getworkflowentity?id=" + id;
                return $.getJSON(url);
            }
        },
        validators: {
            wfBoxModal: function(){
                return $(workflow._options.editor.form).kendoValidator({
                    rules: {
                        requiredFields: function (input) {
                            let isNullOrWhiteSpace = commonHelper.isNullUndefinedOrWhiteSpace;
                            let wfEntryModal = $(workflow._options.editor.modal);
                            
                            if(input.is("[id=WorkflowSetup_ApprovalType]")) {
                                let comboBox = wfEntryModal.find('#WorkflowSetup_ApprovalType').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=WorkflowSetup_RoleDetail]")) {
                                let comboBox = wfEntryModal.find('#WorkflowSetup_RoleDetail').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=Label]")) {
                                let label = wfEntryModal.find('#Label');
                                return !isNullOrWhiteSpace(label.val())
                            }

                            if(input.is("[id=WorkflowSetup_StatusAfterApproved]")) {
                                let comboBox = wfEntryModal.find('#WorkflowSetup_StatusAfterApproved').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=WorkflowSetup_StatusAfterRejected]")) {
                                let comboBox = wfEntryModal.find('#WorkflowSetup_StatusAfterRejected').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=WorkflowSetup_UserDetail]")) {
                                var exclusive = $("#APPROVALTYPE_EXCLUSIVE").val()
                                let comboBox = wfEntryModal.find('#WorkflowSetup_UserDetail').data('kendoComboBox');
                                let approvalType = wfEntryModal.find('#WorkflowSetup_ApprovalType').data('kendoComboBox');
                                
                                if (approvalType.value() == exclusive && comboBox.selectedIndex === -1) {
                                    return false;
                                }
                                return true;
                            }

                            return true;
                        }
                    },
                    messages: {
                        requiredFields: function (input) {
                            return "This field is required";
                        }
                    }
                }).data("kendoValidator");
            }
        }
    }
})
