$(document).ready(function() {
    $("body").on("focusout, blur", "#supplier_id", function(e) {
        viewsShared.common.changeValidationMessage(this.id);
    })
})

viewsShared = {
    editorTemplates: {
        suppliers: {
            
        } 
    },
    common: {
        changeValidationMessage: function(id) {
            $('div[data-valmsg-for="'+id+'"]').find("span.k-tooltip-content").html("This field is required.");
        }
    }
}

