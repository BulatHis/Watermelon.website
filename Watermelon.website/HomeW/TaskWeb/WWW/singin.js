var resultMessage = $("#resultMessage")
var userForm = $("#userForm")
var sendForm = $("#sendForm")
sendForm.click(function () {
    localStorage.removeItem('email');
    var email = $("#email").val().trim();
    var password = $("#password").val().trim();
    
    if(email === "" || password === ""){

        resultMessage.text("Ошибка ввода"); 

        return false;
    }
    localStorage.setItem('email',email);
    resultMessage.text("")
    $.ajax({
        url: '/login',
        type: 'POST',
        data: JSON.stringify({'Email': email, 'Password': password}),
        contentType: 'application/json',
        beforeSend: function (){
            sendForm.prop('disabled',true);
        } ,
        success: function (data) {
            resultMessage.text(data);
            userForm.trigger("reset");

            sendForm.prop('disabled',false);
        },
        error: function (jqXHR, exception) {
            if(jqXHR.status === 400){
                resultMessage.text("Вход не выполнен. Информация об ошибках: " + jqXHR.responseText);
            }
            else{
                resultMessage.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled',false);
        }
    });
});
