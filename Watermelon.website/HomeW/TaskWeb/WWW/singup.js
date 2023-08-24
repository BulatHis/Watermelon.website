var resultMessage = $("#resultMessage");
var userForm = $("#userForm")
var sendForm = $("#sendForm")
sendForm.click(function () {

    var email = $("#email").val().trim();
    var mobile = $("#mobile").val().trim();
    var password = $("#password").val().trim();
    if (email === "" || mobile === "" || password === "") {
        
        resultMessage.text("Ошибка ввода");
        
        return false;
    }
    resultMessage.text("")
    $.ajax({
        url: '/registration',
        type: 'POST',
        data: JSON.stringify({ 'Email': email, 'Mobile': mobile, 'Password': password}),
        contentType: 'application/json',
        beforeSend: function (){
            sendForm.prop('disabled',true);
        } ,
        success: function (data) {

            resultMessage.text("Данные о пользователе сохранены. Его идентификатор: " + data);
            userForm.trigger("reset");

            sendForm.prop('disabled',false);
        },
        error: function (jqXHR, exception) {
            if(jqXHR.status === 400){
                resultMessage.text("Данные о пользователе не сохранены. Информация об ошибках: " + jqXHR.responseText);
            }
            else{
                resultMessage.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled',false);
        }
    });
});

