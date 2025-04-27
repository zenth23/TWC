

$(function () {
    $(document).on('invalid-form.validate', 'form', function () {
        var $text = $(this).find('input[type="text"]');
        var $text2 = $(this).find('input[type="password"]');
        var $button = $(this).find('input[type="submit"]');
        setTimeout(function () {
            $button.prop('disabled', false);
            $text.prop('disabled', false);
            $text2.prop('disabled', false);
        }, 1);
    });

    $(document).on('submit', 'form', function () {
        var $text = $(this).find('input[type="text"]');
        var $text2 = $(this).find('input[type="password"]');
        var $button = $(this).find('input[type="submit"]');
        setTimeout(function () {
            $button.prop('disabled', true).val('Logging in...');
            $text.prop('disabled', true);
            $text2.prop('disabled', true);
        }, 1);
    });
});