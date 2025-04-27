/*
*Created By     : Seb
*Creation Date  : 02-22-2016

*Modified By    : Seb
*Mod Date       : 01-20-2022
*Description    : Replaced "var" with "let" keyword
*/

function markAsRequired() {
    // FOR ORDINARY
    let input_elements = $("input[type=text].form-control,select.form-control,textarea.form-control,input[type=email].form-control,input[type=password].form-control");
    $.each(input_elements, function (i, el) {
        let e = $(el);
        let k_attr = e.attr('class');
        let e_parent = e.parent().parent();
        let e_id = e.attr('id');
        let label = e_parent.find('label[for=' + e_id + ']');
        //
        let attr = $(e).attr('data-val-required');
        if (typeof attr !== typeof undefined && attr !== false)
            label.addClass('required');
        else
            label.removeClass('required');
    });
    // FOR KENDO UI ELEMENTS
    let kendo_input_elements = $("input.k-input");
    $.each(kendo_input_elements, function (i, el) {
        let e = $(el);
        let e_parent = e.parent().parent().parent().parent();
        let e_id = e.attr('id');
        let label = e_parent.find('label[for=' + e_id + ']');
        //
        let attr = $(e).attr('data-val-required');
        if (typeof attr !== typeof undefined && attr !== false)
            label.addClass('required');
        else {
            // kendo combobox support
            let e2 = $(e).parent().siblings('[data-val-required]');
            let e_id2 = e2.attr('id');
            let label2 = $(e2).parent().parent().parent().find('label[for=' + e_id2 + ']');
            if (e2.length > 0) {
                label2.addClass('required');
            }
            else {
                label.removeClass('required');
                label2.removeClass('required');
            }
        }
    });
}