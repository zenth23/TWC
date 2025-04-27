workflow = {
    _options: {
        wfBlockHolder: ".ContentDiv", // div where workflow ui will be placed
        wfApprovalBox: '#parentApprovalDiv > .approvalCardDiv', // div for every approval box/step
        entityTitle: "#TitleSpan", // just title of current block
        wfBlockHolderParent: ".ParentContainer", // parent of wfBlockHolder, just for showing progress bars
        getWorkflowsUrl: "Workflow/GetWorkflows", // url for getting workflow data
        jsPlumbInstance: null, // just for storing object globally, no need to change
        editor: {
            modal: "#modalWorkflow", // modal for crud 
            form: "#wfBoxForm",
            counter: 0, // flagging for highlighting workflow box/step, no need to change
        },
        adhoc: {
            modal: "#modalWorkflow", // modal for crud 
            form: "#wfBoxForm",
            counter: 0, // flagging for highlighting workflow box/step, no need to change
        }
    },
    initialize: function (options) {
        options = options === undefined ? {} : options;

        this.setOptions(options);
        this.createInstance(); // this.generateUI();

        if (this.editor !== undefined) {
            this.editor.initialize();
        }
    },
    setOptions: function(options) {

        if (options.editor === undefined) {
            $.extend(options, { editor: {} })
        }

        if (options !== null || options !== undefined) {

            let isNullUndefinedOrWhiteSpace = commonHelper.isNullUndefinedOrWhiteSpace;

            if (!isNullUndefinedOrWhiteSpace(options.wfBlockHolder)) workflow._options.wfBlockHolder = options.wfBlockHolder;
            if (!isNullUndefinedOrWhiteSpace(options.wfApprovalBox)) workflow._options.wfApprovalBox = options.wfApprovalBox;
            if (!isNullUndefinedOrWhiteSpace(options.entityTitle)) workflow._options.entityTitle = options.entityTitle;
            if (!isNullUndefinedOrWhiteSpace(options.wfBlockHolderParent)) workflow._options.wfBlockHolderParent = options.wfBlockHolderParent;
            if (!isNullUndefinedOrWhiteSpace(options.getWorkflowsUrl)) workflow._options.getWorkflowsUrl = options.getWorkflowsUrl;

            if(options.adhoc !== undefined && options.adhoc !== null ) {
                workflow._options.adhoc = options.adhoc;
            }
        }
    },
    createInstance: function () {


        if (this._options.jsPlumbInstance !== null)
            this._options.jsPlumbInstance.reset();

        this._options.jsPlumbInstance = jsPlumb.getInstance()

        let strDivIds = "approval_start";
        let prevDivIds = "";
        let mainDivs = $(this._options.wfApprovalBox);
        let mainDivLength = mainDivs.length;
        let instance = this._options.jsPlumbInstance;

        instance.importDefaults({
            ConnectionsDetachable: false
        });
        instance.deleteEveryConnection();

        $.each(mainDivs, function (index, value) {
            let cards = $(value).find('.card');
            if (cards.length > 0) {
                $.each(cards, function (cardIndex, cardValue) {
                    let cardId = $(cardValue).attr("id");
                    let cardapprovalType = $(cardValue).attr("approvalType");
                    if (prevDivIds != "") {
                        let res = prevDivIds.split(",");
                        $.each(res, function (prevIndex, prevValue) {
                            targetEndpoint = "Blank";
                            if (cardapprovalType == "any") {
                                targetEndpoint = "Rectangle";
                            }
                            else if (cardapprovalType == "all") {
                                targetEndpoint = "Dot";
                            }
                            instance.connect({
                                source: prevValue,
                                target: cardId,
                                anchors: ["RightMiddle", "LeftMiddle"],
                                connector: ["Flowchart"],
                                endpoints: ["Blank", [targetEndpoint, { radius: 4, width: 8, height: 8 }]]
                            });
                        })
                    }
                    if (strDivIds != "") {
                        strDivIds = strDivIds + ",";
                    }
                    strDivIds = strDivIds + cardId;
                })
            }
            else {
                if (prevDivIds != "" && (mainDivLength - 1) == index) {
                    let res = prevDivIds.split(",");
                    $.each(res, function (prevIndex, prevValue) {
                        instance.connect({
                            source: prevValue,
                            target: "approval_end",
                            anchors: ["RightMiddle", "LeftMiddle"],
                            connector: ["Flowchart"],
                            endpoint: "Blank"
                        });
                    })
                }
            }
            prevDivIds = strDivIds;
            strDivIds = "";
        });
    },
    generateUI: function (entityId) {
        
        if (entityId !== undefined) {


            let wfInstance = this;
            let options = wfInstance._options;

            let titleSpan = $(options.entityTitle);
            let wfHolderParent = $(options.wfBlockHolderParent);
            let wfHolder = $(options.wfBlockHolder);

            titleSpan.html($('#tab-' + entityId).html()); //Change Title of container

            let url = window.rootUrl + options.getWorkflowsUrl;
            let ajax = $.ajax({
                url: url,
                type: 'POST',
                cache: false,
                beforeSend: function () {
                    kendo.ui.progress(wfHolderParent, true);
                },
                data: { workflowEntityId: entityId }
            })

            return ajax.done(function (result) {
                        wfHolder.html(result);

                        kendo.ui.progress(wfHolderParent, false);
                        wfInstance.createInstance();
                    }).fail(function (e) {

                        wfHolder.html("Error loading content.");
                        kendo.ui.progress(wfHolderParent, false);
                    });
        }
    },
    clear: function() {
        let wfInstance = this;
        let options = wfInstance._options;

        $(options.wfBlockHolder).empty();
        $(options.entityTitle).empty();
    },
    common: {
        catchError: function (x, t, e) {
            $('#systemSpinner').hide();
            console.log('error');
            console.log(e);
            let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
            window.alert(msg, t);
        }
    }
}

