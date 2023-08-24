var resultMessage = $("#resultMessage")
var userForm = $("#userForm")
var sendForm = $("#sendForm")


sendForm.click(function () {
    var sale
    if (sessionStorage.getItem('min') < 5) {
        sale = 'True'
    }
    if (sessionStorage.getItem('min') >= 5) {
        sale = 'False'
    }
    var email = localStorage.getItem('email');
    var address = $("#address").val().trim();
    var countOfWatermelon = $("#countOfWatermelon").val().trim();
    var watermelonName = $("#watermelonName").val().trim();

    if (address === "" || countOfWatermelon === "" || watermelonName === "") {

        resultMessage.text("Ошибка ввода или 0 арбузов ");

        return false;
    }
    resultMessage.text("")
    $.ajax({
        url: '/buyRequest',
        type: 'POST',
        data: JSON.stringify({
            'Address': address,
            'CountOfWatermelon': countOfWatermelon,
            'watermelonName': watermelonName,
            'Email': email,
            'Sale': sale
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
                resultMessage.text("Вход не выполнен. Информация об ошибках: " + jqXHR.responseText);
            } else {
                resultMessage.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled', false);
        }
    });
});

