let resultDelete = $("#resultdelete")
var deleteForm = $("#deleteForm")
var sendFormDel = $("#sendFormDel")
sendFormDel.click(function () {

    var emailDel = $("#emailDel").val().trim();

    if (emailDel === "") {
        resultDelete.text("Ошибка ввода");
        
        return false;
    }
    resultDelete.text("")
    $.ajax({
        url: '/deleteResume',
        method: 'get',
        data: {'email': emailDel},
        beforeSend: function () {
            deleteForm.prop('disabled', true);
        },
        success: function (data) {
            resultDelete.text(data);
            deleteForm.trigger("reset");

            sendFormDel.prop('disabled', false);
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 400) {
                resultDelete.text("Заявка не отправлена. Информация об ошибках: " + jqXHR.responseText);
            } else {
                resultDelete.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendFormDel.prop('disabled', false);
        }
    });
})