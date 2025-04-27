
// ADHOC - CREATE A COPY OF THIS IF NEEDED TO CHANGE/ADD FUNCTIONALITY
$.extend(workflow, {
    adhoc: {
        _selected: {
            workflowId: 0,
            get entityId() {
                return $("#ApprovalWorkflow_ApprovalEntity").val()
            } 
        },
        initialize: function () {
        },
        events: {
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
                        workflow.adhoc._selected.workflowId = $(e.currentTarget).data("workflowid");
                        $("#btnDelete").attr("disabled", false);
                        $("#btnEdit").attr("disabled", false);
                    }
                    else {
                        workflow.adhoc._selected.workflowId = 0;
                        $("#btnDelete").attr("disabled", true);
                        $("#btnEdit").attr("disabled", true);
                    }
                }
            },
            onSaveWfBox: function () {
                window.confirm("Confirm", "Are you sure that you want to save workflow?")
                        .then(function () {
                            let validator = workflow.adhoc.validators.wfBoxModal();
                            let isValid = validator.validate();
                            let wfEntryModal = $(workflow._options.adhoc.modal);
                            if (isValid) {
                                let id = wfEntryModal.find('#ApprovalWorkflow_Id').val();
                                let position = wfEntryModal.find('input[name="rbtnPosition"]:checked').val();
                                let label = wfEntryModal.find('#Label').val();
                                let roleDetail = wfEntryModal.find("#ApprovalWorkflow_RoleDetail").data('kendoComboBox');
                                let approvalType = wfEntryModal.find('#ApprovalWorkflow_ApprovalType').data('kendoComboBox');
                                let userDetail = wfEntryModal.find("#ApprovalWorkflow_UserDetail").data('kendoComboBox');
                                let statusAfterApproved = wfEntryModal.find("#ApprovalWorkflow_StatusAfterApproved").data('kendoComboBox');
                                let statusAfterRejected = wfEntryModal.find("#ApprovalWorkflow_StatusAfterRejected").data('kendoComboBox');
                                let rowVersion = wfEntryModal.find("#ApprovalWorkflow_Base64RowVersion").val()

                                let data = {
                                    Id: id,
                                    SelectedWorkflowId: workflow.adhoc._selected.workflowId,
                                    WorkflowEntityId: workflow.adhoc._selected.entityId,
                                    Position: position,
                                    ApprovalTypeId: approvalType.value(),
                                    RoleId: roleDetail.value(),
                                    Label: label,
                                    UserId: userDetail.value(),
                                    StatusAfterApproved: statusAfterApproved.value(),
                                    StatusAfterRejected: statusAfterRejected.value(),
                                    RowVersion: rowVersion
                                };
                            
                                workflow.adhoc.services.saveWfBox(data)
                                    .done(function(response) {
                                        if (response.Success) {
                                            wfEntryModal.modal('toggle');
                                            workflow.generateUI(workflow.adhoc._selected.entityId);

                                            workflow.adhoc._selected.workflowId = 0;
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
            onAddWfBox: function (e) {
                e.preventDefault();
                if (workflow.adhoc._selected.workflowId == 0) {
                    $("#positionDiv").hide();
                }
                else {
                    $("#positionDiv").show();
                }

                $(workflow._options.adhoc.modal).modal('toggle');
                $('#modalWorkflowTitle').html("Add Workflow");

                $('#ApprovalWorkflow_ApprovalEntity').val(workflow.adhoc._selected.entityId);
                $('#ApprovalWorkflow_Id').val(0);
                $('#Label').val("");
                $("#ApprovalWorkflow_RoleDetail").data('kendoComboBox').value(null);
                $('#ApprovalWorkflow_ApprovalType').data('kendoComboBox').value(null);
                $('#ApprovalWorkflow_StatusAfterApproved').data('kendoComboBox').value(null);
                $('#ApprovalWorkflow_StatusAfterRejected').data('kendoComboBox').value(null);

                $(".userdiv").hide();
                $('#ApprovalWorkflow_UserDetail').data("kendoComboBox").value(null);
            },
            onEditWfBox: function (e) {
                e.preventDefault();
                if (workflow.adhoc._selected.workflowId == 0) {
                    alert("Please select a card to edit.");
                }
                else {
                    let data = { workflowId: workflow.adhoc._selected.workflowId };
                    workflow.adhoc.services.getWfBox(data)
                        .done(function(response) {
                            if(response.Success) {
                                $("#positionDiv").hide();
                                $('#modalWorkflowTitle').html("Edit Workflow");
                                $('#ApprovalWorkflow_Id').val(response.Workflow.Id);
                                $('#Label').val(response.Workflow.Label);
                                $("#ApprovalWorkflow_RoleDetail").data('kendoComboBox').value(response.Workflow.ApprovalWorkflow_RoleDetail);
                                $("#ApprovalWorkflow_UserDetail").data('kendoComboBox').dataSource.read();
                                $("#ApprovalWorkflow_UserDetail").data('kendoComboBox').value(response.Workflow.ApprovalWorkflow_UserDetail);
                                $('#ApprovalWorkflow_ApprovalType').data('kendoComboBox').value(response.Workflow.ApprovalWorkflow_ApprovalType);
                                $('#ApprovalWorkflow_StatusAfterApproved').data('kendoComboBox').value(response.Workflow.ApprovalWorkflow_StatusAfterApproved);
                                $('#ApprovalWorkflow_StatusAfterRejected').data('kendoComboBox').value(response.Workflow.ApprovalWorkflow_StatusAfterRejected);
                                $('#ApprovalWorkflow_ApprovalEntity').val(workflow.adhoc._selected.entityId);
                                $('#ApprovalWorkflow_ApprovalType').data('kendoComboBox').trigger("change");
                                $('#ApprovalWorkflow_Base64RowVersion').val(response.Workflow.Base64RowVersion);
                                
                                $(workflow._options.adhoc.modal).modal('show');
                            } else {
                                alert(response.Message)
                            }
                        }).fail(workflow.common.catchError);
                }
            },
            onDeleteWfBox: function (e) {
                e.preventDefault();
                if (workflow.adhoc._selected.workflowId == 0) {
                    alert("Please select a card to delete.");
                }
                else {
                    window.confirm("Confirm", "Are you sure that you want to delete this?").then(function () {
                        let data = { workflowId: workflow.adhoc._selected.workflowId }
                        workflow.adhoc.services.deleteWfBox(data)
                            .done(function(response) {
                                if (response.Success) {
                                    workflow.generateUI(workflow.adhoc._selected.entityId)
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
                        let data = { workflowEntityId: workflow.adhoc._selected.entityId, sortIndex: sortIndex };
                        workflow.adhoc.services.deleteAllWfBoxes(data)
                            .done(function(response) {
                                if (response.Success) {
                                    workflow.generateUI(workflow.adhoc._selected.entityId)
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
                confirm("Confirmation", "Are you sure you want to save?").done(function () {
                    let id = $("#WFBlockId").val();
                    let name = $("#WFBlockName").val();

                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id,
                        name: name
                    }

                    workflow.adhoc.services.saveWfBlock(data)
                     .done(function (response) {
                         if (response.Success) {
                             $("#modalBlockWorkflow").modal("hide");
                             alert("Workflow block saved.");
                             workflow.adhoc.common.loadWorkflowEntities();
                         } else {
                             alert(response.Message);
                         }
                     }).fail(workflow.common.catchError);

                    $("#WFBlockId").val("");
                    $("#WFBlockName").val("");
                })
            },
            onOpenModalWfBlock: function (id, title) {

                var entityId = id === null || id === "" || id === undefined ? 0 : id;

                if (entityId === 0)
                    $("#modalBlockWorkflowTitle").html("New Workflow Block")
                else
                    $("#modalBlockWorkflowTitle").html("Update Block")

                $("#WFBlockId").val(entityId);
                $("#WFBlockName").val(title);
                $("#modalBlockWorkflow").modal("show");

            },
            onDeleteWfBlock: function (id) {
                confirm("Confirmation", "Are you sure you want to delete?").done(function () {
                    let data = {
                        id: id === null || id === "" || id === undefined ? 0 : id
                    };

                    workflow.adhoc.services.deleteWfBlock(data)
                     .done(function (response) {
                         if (response.Success) {

                             alert("Workflow block deleted.");
                             workflow.adhoc.common.loadWorkflowEntities();
                         } else {
                             alert(response.Message);
                         }
                     }).fail(workflow.common.catchError);
                })
            },
            roleOnChange: function (e) {
                $('#ApprovalWorkflow_UserDetail').data("kendoComboBox").dataSource.read();
            },
            approvalTypeOnChange: function (e) {
                commonHelper.getConfigValue("APPROVALTYPE_EXCLUSIVE")
                    .done(function(response) {
                        if (e.sender.value() === response.Value)
                            $(".userdiv").fadeIn(500);
                        else {
                            $(".userdiv").hide();
                            $('#ApprovalWorkflow_UserDetail').data("kendoComboBox").value(null);
                        }
                        $('#ApprovalWorkflow_UserDetail').data("kendoComboBox").dataSource.read();
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
                    
                    workflow.adhoc.services.changeParallelType(model)
                        .done(function (response) {
                            if (response.Success) {
                                // workflow.generateUI(workflow.adhoc._selected.entityId);
                                element.attr("origValue", selectedValue)
                                kendo.ui.progress(element.closest("div.parHolder"), false);
                            } else {
                                alert(response.Message);
                                kendo.ui.progress(element.closest("div.parHolder"), false);
                                comboBox.value(origValue);
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
            }

        },
        filters: {
            searchKeyApprovalType: function () {
                let combobox = $('#ApprovalWorkflow_ApprovalType').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyRole: function () {
                let combobox = $('#ApprovalWorkflow_RoleDetail').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyUser: function () {
                let combobox = $('#ApprovalWorkflow_UserDetail').data("kendoComboBox");
                let roleId = $('#ApprovalWorkflow_RoleDetail').data("kendoComboBox").value();
                return { searchKey: combobox.text(), roleId: roleId };
            },
            searchKeyAfterApproved: function () {
                let combobox = $('#ApprovalWorkflow_StatusAfterApproved').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyAfterRejected: function () {
                let combobox = $('#ApprovalWorkflow_StatusAfterRejected').data("kendoComboBox");
                return { searchKey: combobox.text() };
            },
            searchKeyParallel: function (elem) {
                let combobox = $(elem).data("kendoComboBox");
                return { searchKey: combobox.text() };
            }
        },
        services: {
            saveWfBox: function(data) {
                let token = $(workflow._options.adhoc.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "ApprovalAdhoc/SaveWorkflow";
                return $.post(url, data);
            },
            getWfBox: function(data) {
                let url = window.rootUrl + "ApprovalAdhoc/GetWorkflowById";
                return $.getJSON(url, data);
            },
            deleteWfBox: function(data) {
                let token = $(workflow._options.adhoc.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "ApprovalAdhoc/DeleteWorkflow";
                return $.post(url, data);
            },
            deleteAllWfBoxes: function(data){
                let token = $(workflow._options.adhoc.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "ApprovalAdhoc/DeleteWorkflowBySortIndex";
                return $.post(url, data);
            },
            changeParallelType: function(data){
                let token = $(workflow._options.adhoc.form).find('input[name="__RequestVerificationToken"]').val();
                $.extend(data, { __RequestVerificationToken: token });

                let url = window.rootUrl + "ApprovalAdhoc/UpdateParallelTypeBySortIndex";
                return $.post(url, data);
            }
        },
        validators: {
            wfBoxModal: function(){
                return $(workflow._options.adhoc.form).kendoValidator({
                    rules: {
                        requiredFields: function (input) {
                            let isNullOrWhiteSpace = commonHelper.isNullUndefinedOrWhiteSpace;
                            let wfEntryModal = $(workflow._options.adhoc.modal);
                            
                            if(input.is("[id=ApprovalWorkflow_ApprovalType]")) {
                                let comboBox = wfEntryModal.find('#ApprovalWorkflow_ApprovalType').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=ApprovalWorkflow_RoleDetail]")) {
                                let comboBox = wfEntryModal.find('#ApprovalWorkflow_RoleDetail').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=Label]")) {
                                let label = wfEntryModal.find('#Label');
                                return !isNullOrWhiteSpace(label.val())
                            }

                            if(input.is("[id=ApprovalWorkflow_StatusAfterApproved]")) {
                                let comboBox = wfEntryModal.find('#ApprovalWorkflow_StatusAfterApproved').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=ApprovalWorkflow_StatusAfterRejected]")) {
                                let comboBox = wfEntryModal.find('#ApprovalWorkflow_StatusAfterRejected').data('kendoComboBox');
                                return comboBox.selectedIndex !== -1;
                            }

                            if(input.is("[id=ApprovalWorkflow_UserDetail]")) {
                                var exclusive = $("#APPROVALTYPE_EXCLUSIVE").val()
                                let comboBox = wfEntryModal.find('#ApprovalWorkflow_UserDetail').data('kendoComboBox');
                                let approvalType = wfEntryModal.find('#ApprovalWorkflow_ApprovalType').data('kendoComboBox');
                                
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
