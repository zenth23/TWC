// see $(document).ready() function

requests = {
    initialize: function() {
        let approvalEntityId = $("#ApprovalEntityId").val();
        if(commonHelper.hasValue(approvalEntityId)) {
            $("#btnGenerateWf").hide();

            
            workflow.generateUI(approvalEntityId)
                    .done(function() {
                        $("#btnGroupApproval").find("input[type='button']").attr("disabled", false);
                    });
            
        }
    },
    events: {
        onSubmit: function (elem, e) {
            e.preventDefault();

            let form = $(elem).closest("form");
            let isValid = form.valid();

            form.validate();
            if(isValid) {
                confirm("Confirmation", "Are you sure you want to submit?")
                .done(function () {
                    $('#systemSpinner').show();
                    let model = {
                        Id: form.find("#Id").val(),
                        Name: form.find("#Name").val(),
                        Description: form.find("#Description").val()
                    }
                    let data = {
                        model: model,
                        __RequestVerificationToken: form.find('input[name="__RequestVerificationToken"]').val()
                    }
                    requests.services
                            .submit(data)
                            .done(function (response) {
                                $('#systemSpinner').hide();

                                if (response.Success) {
                                    alert("Request submitted");

                                     // see scripts/application/form-change-listener.js
                                     window.removeEventListener("beforeunload", beforeUnloadEvent);
                                     window.location = window.rootUrl + "requests";

                                } else {
                                    alert(response.Message);
                                }

                            })
                            .fail(requests.common.catchError);
                })
            }
        },
        onGenerateWorkflow: function (elem, e) {
            e.preventDefault();
            workflow.generateUI(0);
        },
        onOpenResponseModal: function(elem, e, clsResponse) {
            let modal = $("#responseModal")

            // modal.find(".modal-footer").find("button").hide();
            $(".clsReject").hide();
            $(".clsWfa").hide();

            $(clsResponse).show();

            modal.modal("show");
        },
        onApprove: function(elem, e, wfActionId) {
            confirm("Confirmation", "Are you sure you want to approve?")
                .done(function() {
                    $('#systemSpinner').show();

                    let requestId =  $("#Id").val();
                    let wfApprovalId = $("#UserPendingApprovalId").val();
                    let remarks = $("#ResponseRemarks").val();
                    remarks = commonHelper.hasValue(remarks) ? remarks.trim() : remarks;

                    let form = $(elem).closest("form");
                    let model = {
                        WorkflowActionId: wfActionId,
                        WorkflowApprovalId: wfApprovalId,
                        RequestId: requestId,
                        Remarks: remarks
                    }

                    let data = {
                        model: model,
                        __RequestVerificationToken: form.find('input[name="__RequestVerificationToken"]').val()
                    }

                    requests.services
                            .respond(data)
                            .done(function (response) {
                                $('#systemSpinner').hide();

                                if (response.Success) {
                                    alert("Approval successful.");

                                    // see scripts/application/form-change-listener.js
                                    window.removeEventListener("beforeunload", beforeUnloadEvent);
                                    window.location = window.rootUrl + "requests";

                                } else {
                                    alert(response.Message);
                                }

                            })
                            .fail(requests.common.catchError);
                })
        },
        onReject: function(elem, e, wfActionId) {

            var validator = requests.validators.responseModal();
            var isValid = validator.validate();
            if(isValid) {

                confirm("Confirmation", "Are you sure you want to reject?")
                .done(function() {
                    
                    $('#systemSpinner').show();

                    let requestId =  $("#Id").val();
                    let wfApprovalId = $("#UserPendingApprovalId").val();
                    let remarks = $("#ResponseRemarks").val();
                    remarks = commonHelper.hasValue(remarks) ? remarks.trim() : remarks;

                    let returnTo = $("#ReturnTo").data("kendoComboBox").value();
                    let returnWfaSortIndex = $("#ReturnWorkflowApproval").data("kendoComboBox").value();

                    let form = $(elem).closest("form");
                    let model = {
                        WorkflowActionId: wfActionId,
                        WorkflowApprovalId: wfApprovalId,
                        RequestId: requestId,
                        Remarks: remarks,
                        ReturnTo: returnTo,
                        ReturnWfaSortIndex: returnWfaSortIndex
                    }

                    let data = {
                        model: model,
                        __RequestVerificationToken: form.find('input[name="__RequestVerificationToken"]').val()
                    }

                    requests.services
                            .respond(data)
                            .done(function (response) {
                                $('#systemSpinner').hide();

                                if (response.Success) {
                                    alert("Rejection successful.");

                                     // see scripts/application/form-change-listener.js
                                     window.removeEventListener("beforeunload", beforeUnloadEvent);
                                     window.location = window.rootUrl + "requests";

                                } else {
                                    alert(response.Message);
                                }

                            })
                            .fail(requests.common.catchError);
                })
            }
        },
        returnToOnChange: function(e) {
            var comboBox = e.sender;
            if(comboBox.selectedIndex !== -1) {
                if(comboBox.value() == "2") {
                    $(".clsWfa").fadeIn(500);
                } else {
                    $(".clsWfa").hide();
                }
            }
        }
    },
    services: {
        submit: function (data) {
            let url = window.rootUrl + "requests/submit"
            return $.post(url, data);
        },
        respond: function(data) {
            let url = window.rootUrl + "requests/respond"
            return $.post(url, data);
        }
    },
    common: {
        catchError: function (x, t, e) {
            $('#systemSpinner').hide();
            console.log('error');
            console.log(e);
            let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
            window.alert(msg, t);
        }
    },
    validators: {
        responseModal: function() {
            return $("#responseModalValidator").kendoValidator({
                rules: {
                    requiredFields: function (input) {
                        let hasValue = commonHelper.hasValue;

                        if(input.is("[id=ResponseRemarks]")) {
                            let value = $("#ResponseRemarks").val();
                            return $("#btnApprove").is(":visible") || ( hasValue(value) && $("#btnReject").is(":visible") )
                        }

                        if(input.is("[id=ReturnTo]")) {
                            let returnTo = $("#ReturnTo").data("kendoComboBox");
                            return  returnTo.selectedIndex !== -1;
                        }

                        if(input.is("[id=ReturnWorkflowApproval]")) {
                            let returnTo = $("#ReturnTo").data("kendoComboBox");
                            let specWfa = $("#ReturnWorkflowApproval").data("kendoComboBox");

                            if(returnTo.value() == "2" && specWfa.selectedIndex === -1)
                                return false;
                            
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
    },
    filters: {
        returnWfa: function() {
            var requestId = $("#ReturnWorkflowApproval_RequestId").val();
            var requestName = $("#ReturnWorkflowApproval_RequestName").val();
            var searchKey = $("#ReturnWorkflowApproval").data("kendoComboBox").text();
            var userPendingApprovalId = $("#ReturnWorkflowApproval_UserPendingApprovalId").val();

            return {
                requestId: requestId,
                requestName: requestName,
                searchKey: searchKey,
                userPendingApprovalId: userPendingApprovalId
            }
        }
    }
};

$(document).ready(function () {
    let workflowOptions = {
        wfBlockHolder: "#workflowBlockHolder", // holder of workflows GUI
        wfBlockHolderParent: "#workflowBlockHolderParent", // just holder of circle loader
        getWorkflowsUrl: "requests/getworkflows", // will be called everytime workflow.generateUI() was called
        adhoc: {
            modal: "#modalWorkflow", // modal for crud 
            form: "#requestForm",
            counter: 0, // flagging for highlighting workflow box/step, no need to change
        }
    };

    workflow.initialize(workflowOptions);
    requests.initialize();
});

