
function register_form_ajax_submission(form_query) {
    jQuery(form_query).submit(submit_edit_event_form);
}

function submit_edit_event_form(event) {
    event.preventDefault();
    let form = jQuery(this);
    __submit_form(form);
}

function __submit_form(form) {
    disable_form(form);
    form.ajaxSubmit({
        success: function(response) {
            console.log(response);
            alert(response);
            location.reload();
        },
        error: function(response) {
            let text = response.responseText;
            alert(text);
            console.error(text);
            enable_form(form);
        },
    });
}

function disable_form(form) {
    form.find('button').attr('disabled', '');
}

function enable_form(form) {
    form.find('button').removeAttr('disabled');
}