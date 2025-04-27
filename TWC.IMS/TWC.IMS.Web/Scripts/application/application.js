/*
************************************************************************************************
JS Application Interface v1.1.0
Purpose             : To standardize and centralize all javascript calls within an application                    
Author              : Seb
Date                : 10/7/2019
------------------------------------------------------------------------------------------------
Last Modified By    : seb
Last Modified Date  : 01/20/2022
Change Description  : Added common kendo combobox refresh method
                    : Replaced "var" with "let" for performance purposes
------------------------------------------------------------------------------------------------
Last Modified By    : seb
Last Modified Date  : 03/10/2022
Change Description  : Added Kendo DropdownList control support for the required field functionality
------------------------------------------------------------------------------------------------
Last Modified By    : seb
Last Modified Date  : 07/13/2022
Change Description  : Added system-wide spinner when the form was submitted
************************************************************************************************
*/

$(function () {
    // execute this code immediately after jQuery has loaded
    void function () {
        // disable cache globally
        $.ajaxSetup({ cache: false });
        console.log('$.ajax cache disabled');

        // this will include kendo ui controls to be validated by jquery as well
        let validator = $.validator;
        if (typeof validator !== 'undefined') {
            validator.setDefaults({ ignore: '' });
            console.log('$.validator for kendo controls enabled');
        }
    }();

    // bind onClick event to elements with btnReset as id
    $(document).on('click', '#btnReset', function () { application.UI.resetForm(); });

    // triggered every time form is submitted
    $(document).on('submit', 'form', function () {

        let $alert = $(this).find('div.alert');
        let $input = $(this).find('input');
        let $button1 = $(this).find('input[type="submit"]');
        let $button2 = $(this).find('button');
        let $button3 = $(this).find('a.btn'); // button links
        let $textarea = $(this).find('textarea');
        setTimeout(function () {
            $alert.hide();
            $input.prop('disabled', true);
            $button1.val('Please wait...');
            $button2.prop('disabled', true);
            $button3.addClass('disabled');
            $textarea.prop('disabled', true);

            $('#systemSpinner').show();
        }, 1);
    });

    $(document).on('invalid-form.validate', 'form', function () {

        let $input = $(this).find('input');
        let $button1 = $(this).find('input[type="submit"]');
        let $button2 = $(this).find('button');
        let $button3 = $(this).find('a.btn'); // button links
        let $textarea = $(this).find('textarea');
        setTimeout(function () {
            $input.prop('disabled', false);
            $button1.prop('disabled', false);
            $button2.prop('disabled', false);
            $button3.removeClass('disabled');
            $textarea.prop('disabled', false);

            $('#systemSpinner').hide();
        }, 1);
    });
});

function alert(content, title) {
    title = (typeof title === 'undefined') ? "Message" : title;
    $("<div></div>").kendoAlert({
        title: title,
        content: content,
        minWidth: 250,
        maxWidth: 500
    }).data("kendoAlert").open();
}

function confirm(title, content) {
    return $("<div></div>").kendoConfirm({
        title: title,
        content: content,
        icon: "k-ext-information",
        minWidth: 250,
        maxWidth: 500
    }).data("kendoConfirm").open().result;
}

function confirmDelete(content) {
    return $("<div></div>").kendoConfirm({
        title: "Confirm Delete",
        content: content,
        icon: "k-ext-information",
        minWidth: 250,
        maxWidth: 500
    }).data("kendoConfirm").open().result;
}

application = {
    UI: {
        markRequiredFields: function () {
            // FOR ORDINARY
            let input_elements = $("input[type=text].form-control,select.form-control,textarea.form-control,input[type=email].form-control,input[type=password].form-control");
            input_elements.each(function () {
                let e = $(this);
                let e_id = e.attr('id');
                let label = $('label[for="' + e_id + '"]');
                let attr = e.data('val-required');
                if (typeof attr !== typeof undefined && attr !== false) {
                    label.addClass('required');
                }
                else {
                    label.removeClass('required');
                }
            });
            // FOR KENDO UI ELEMENTS
            let kendo_input_elements = $("input.k-input,input.k-textbox,span.k-input");
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
        },
        showValidationSummary: function () {
            // triggered every page load
            // checks the alert div if any entry is present
            // then show alert
            // alert is hidden by default using the CSS above for faster execution (no delay)
            let $alertSummary = $('div.validation-summary-errors');
            if ($alertSummary.children('ul').length > 0) {
                $('div.alert').show();
            }
        },
        resetForm: function () {
            this.clearValidationMessages();
            let $form = $('form');
            $form.find('input[type="text"]').each(function () {
                $(this).val('');
            });
            $form.find('input[type="password"]').each(function () {
                $(this).val('');
            });
            $form.find('input[type="email"]').each(function () {
                $(this).val('');
            });
            $form.find('input[type="tel"]').each(function () {
                $(this).val('');
            });
            $form.find('input[type="number"]').each(function () {
                $(this).val('');
            });
            $form.find('textarea').each(function () {
                $(this).val('');
            });
            $form.find('select').each(function () {
                $(this).val(0);
            });
            $form.find('input[type="checkbox"]').each(function () {
                $(this).prop('checked', false);
            });
            $form.find('input[type="radio"]').each(function () {
                $(this).prop('checked', false);
            });
        },
        clearValidationMessages: function () {
            let $form = $('form');
            $form.find('span.text-danger.field-validation-valid').children('span').text('');  // manually clear error messages on the form
            $form.find('span.text-danger.field-validation-error').children('span').text('');
        },
        isDateRangeValid: function (startDate, endDate) {
            try {
                if (startDate.trim() == '' && endDate.trim() == '')
                    return true;

                let start = new Date(startDate);
                let end = new Date(endDate);
                if (start <= end) {
                    return true;
                }
            } catch (e) {
                console.log('isDateRangeValid error: ');
                console.log(e);
            }
            return false;
        },
        isNumberRangeValid: function (from, to) {
            try {
                if (from.trim() == '' && to.trim() == '')
                    return true;
                if (from.trim() == '' || to.trim() == '')
                    return false;

                let f = parseFloat(from);
                let t = parseFloat(to);
                if (f > t || t < f)
                    return false;
                else
                    return true;
            } catch (e) {
                console.log('isNumberRangeValid error: ');
                console.log(e);
            }
            return false;
        },
        formatCurrency: function (value) {
            let k = 1.0e+3, // 1.0e+3 = 1,000
                m = 1.0e+6, // 1.0e+6 = 1,000,000
                b = 1.0e+9; // 1.0e+9 = 1,000,000,000

            let decimalFormat = (Math.sign(value) * Math.abs(value));
            let thousandFormat = Math.sign(value) * (Math.abs(value) / k);
            let millionFormat = Math.sign(value) * (Math.abs(value) / m);
            let billionFormat = Math.sign(value) * (Math.abs(value) / b);

            return value >= b ? billionFormat.toFixed(2) + 'B'
                 : value >= m ? millionFormat.toFixed(2) + 'M'
                 : value >= k ? thousandFormat.toFixed(1) + 'K'
                 : decimalFormat.toFixed(2);
        }
    },
    grid: {
        refreshGrid: function (gridObject) {
            let grid = gridObject.data("kendoGrid");
            grid.dataSource.read();
        },
        editItem: function (e) {
            e.preventDefault();
            let $obj = $(e.currentTarget);
            let controllerName = $obj.attr('data-cname');
            let route = $obj.attr('data-route');
            let propName = $obj.attr('data-propname');

            let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            let name = dataItem[propName];
            let url = '';
            if (route == '' || typeof route == 'undefined')
                url = window.rootUrl + controllerName + '/details/' + name;
            else
                url = window.rootUrl + controllerName + '/details/' + route + '/' + name;

            console.log(url);

            window.location = url;
        },
        deleteItem: function (e) {
            e.preventDefault();
            let $obj = $(e.currentTarget);
            let $parent = $obj.parents('div.k-grid');
            let controllerName = $obj.attr('data-cname');
            let actionName = $obj.attr('data-aname');
            let propId = $obj.attr('data-propid');
            let propName = $obj.attr('data-propname');
            let gridName = $parent.attr('id');

            let dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            let key = dataItem[propId];
            let name = dataItem[propName];
            let reqToken = $('input[name="__RequestVerificationToken"]').val();

            window.confirmDelete("Are you sure that you want to delete '" + name + "'?")
                  .then(function () {
                      actionName = (typeof actionName == 'undefined' || actionName == '') ? 'delete' : actionName;
                      let url = window.rootUrl + controllerName + '/' + actionName;
                      $.ajax({
                          url: url,
                          data: { key: key, __RequestVerificationToken: reqToken },
                          type: 'POST',
                          dataType: 'json',
                          beforeSend: function () { },
                          success: function (response) {
                              application.grid.refreshGrid($('#' + gridName));
                              let r = response;
                              window.alert(r.Message, r.Status);
                          },
                          error: function (x, t, e) {
                              console.log('error');
                              console.log(e);
                              let msg = 'ERROR ' + e.number + ': ' + e.name + '. ' + e.message;
                              window.alert(msg, t);
                          }
                      });
                  }, function () {
                      //kendo.alert("You chose to Cancel action.");
                  });
        }
    },
    combobox: {
        refresh: function (cboObject) {
            let cbo = cboObject.data("kendoComboBox");
            cbo.dataSource.read();
        }
    },
    crypto: {
        encrypt: {
            toBase64: function (string) {
                let encryptedString = btoa(string);
                return encryptedString;
            }
        },
        decrypt: {
            fromBase64: function (string) {
                let decodedString = atob(string);
                return decodedString;
            }
        }
    },
    activateTabViaUrl: function (url) {
        let a = url.split('#');
        if (a.length > 1 && a[1].trim() != '') {
            let activeTab = a[1];
            $(".tab-pane").removeClass("active in");
            $("#" + activeTab).addClass("active in");
            $('a[href="#' + activeTab + '"]').tab('show');
        }
    },
    chart: {
        refreshChart: function (chartObject) {
            let c = chartObject.data("kendoChart");
            c.dataSource.read();
        }
    }
}