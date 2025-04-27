
var $form = $('form'),
    origForm = $form.serialize();

$('form :input').on('change input', function () {
	let hasChanges = $form.serialize() !== origForm;
	if (hasChanges) {
		window.addEventListener("beforeunload", beforeUnloadEvent);
	}
});

$('form').on('submit', function () {
	_formSubmitting = true;
});



