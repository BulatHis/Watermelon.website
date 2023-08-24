var resultMessage = $("#resultMessage");
var userForm = $("#userForm")
var sendForm = $("#sendForm")
sendForm.click(function () {

    var email = $("#email").val().trim();
    var phone = $("#phone").val().trim();
    var education = $("#education").val().trim()
    var experience = $("#experience").val().trim()
    var city = $("#city").val().trim()
    if (email === "" ||phone === "" || education === "" || experience === "" || city === "") {

        resultMessage.text("Ошибка ввода");

        return false;
    }
    resultMessage.text("")
    $.ajax({
        url: '/addResume',
        type: 'POST',
        data: JSON.stringify({
            'Phone': phone, 'Email': email, 'Education': education,
            'Experience': experience, 'City': city
        }),
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
                resultMessage.text(jqXHR.responseText);
            } else {
                resultMessage.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled', false);
        }
    });
});