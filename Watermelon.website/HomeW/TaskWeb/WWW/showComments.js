let resultComment = $("#resultComment")

    var page_id = $("#page_id").val();
    
    resultMessage.text("")
    $.ajax({
        url: '/showcomments',
        method: 'get',
        data: {'WatermelonName': page_id},
        success: function (data) {
            let str = data.replace(/[{]/g,'').replace(/[}]/g,'').replace(/["]/g,' ').replaceAll(', Comment ',' ').
            replaceAll('[ Email :','Комментарии к этому арбузу -=>').replaceAll('Email :','').replaceAll(']','')
            resultComment.text(str);
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 400) {
                resultComment.text("Комментарий не записан. Информация об ошибках: " + jqXHR.responseText);
            } else {
                resultComment.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled', false);
        }
    });
