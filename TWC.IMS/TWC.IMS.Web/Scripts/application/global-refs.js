// used in scripts/application/form-change-listener.js
// need in global for event removing purposes [window.removeEventListener("beforeunload", beforeUnloadEvent)]
beforeUnloadEvent = function (e) {
    if (_formSubmitting) {
        return undefined;
    }

    var confirmationMessage = "You have unsaved changes.";
    (e || window.event).returnValue = confirmationMessage;	// for Gecko + IE
    return confirmationMessage;								// for Gecko + Webkit, Safari, Chrome, etc
}