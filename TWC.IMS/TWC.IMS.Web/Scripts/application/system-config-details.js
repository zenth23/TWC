
$(function () {
    application.UI.markRequiredFields();
    setInputType();
    loadControl();

    $('input[name="inputType"]').on('change', function () {
        let isHtml = $(this).val();
        $('#txtIsHtml').val(isHtml);
        setInputType();
        loadControl();
    });

    $(document).on('click', '#btnCheckConfigs', (e)=>{
        e.preventDefault();
        $('#modalConfigsList').modal('show');
    });
});

function setInputType() {
    let isHtml = $('#txtIsHtml').val();
    if (isHtml.toLowerCase() == 'true') {
        $('#radHtml').attr('checked', 'checked');
        $('#radPlainText').removeAttr('checked');
    }
    else {
        $('#radHtml').removeAttr('checked');
        $('#radPlainText').attr('checked', 'checked');
    }
    $('#Value').focus();
}

function loadControl() {
    let value = "";
    let isHtml = $('#txtIsHtml').val();
    let $editor = $("#Value");
    if (isHtml.toLowerCase() == 'true') {
        let kEditor = $("#Value").data("kendoEditor");
        if (kEditor == null) {
            $editor.kendoEditor({
                tools: [
                        "bold",
                        "italic",
                        "underline",
                        "justifyLeft",
                        "justifyCenter",
                        "justifyRight",
                        "insertUnorderedList",
                        "createLink",
                        "unlink",
                        "insertImage",
                        "tableWizard",
                        "createTable",
                        "addRowAbove",
                        "addRowBelow",
                        "addColumnLeft",
                        "addColumnRight",
                        "deleteRow",
                        "deleteColumn",
                        "mergeCellsHorizontally",
                        "mergeCellsVertically",
                        "splitCellHorizontally",
                        "splitCellVertically",
                        "fontSize",
                        "foreColor",
                        "backColor",
                ],
                encoded: false
            });
        }
        else {
            value = kEditor.value();
            //$editor.refresh();
        }
    }
    else {
        value = $editor.val();
        $editor.remove();
        let $e = $('<textarea class="form-control" rows="25" cols="20" name="Value" id="Value" style="height:400px;">' + value + '</textarea>');
        $('#divValueContainer').html($e);
    }

}