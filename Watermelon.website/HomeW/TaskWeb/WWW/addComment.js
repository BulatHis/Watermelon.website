let resultMessage = $("#resultMessage")
let userForm = $("#userForm")
let sendForm = $("#sendForm")
sendForm.click(function () {
    let email = localStorage.getItem('email');
    let comment = $("#comment").val().trim();
    let page_id = $("#page_id").val().trim();

    if ( comment === "" || page_id === "") {
        resultMessage.text("Ошибка ввода");

        return false;
    }
    resultMessage.text("")
    $.ajax({
        url: '/addcomment',
        type: 'POST',
        data: JSON.stringify({'Comment': comment, 'WatermelonName': page_id, 'Email': email}),
        contentType: 'application/json',
        beforeSend: function () {
            sendForm.prop('disabled', true);
        },
        success: function (data) {
            
            resultMessage.text(data);
            userForm.trigger("reset");
            sendForm.prop('disabled', false);
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 400) {
                resultMessage.text("Комментарий не записан. Информация об ошибках: " + jqXHR.responseText);
            } else {
                resultMessage.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled', false);
        }
    });
});
