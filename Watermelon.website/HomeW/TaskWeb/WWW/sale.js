let resultsale = $("#resultsale")

var h1 = document.getElementById('timer');
var sec = sessionStorage.getItem('sec') || 0;
var min = sessionStorage.getItem('min') || 0;
var hrs = sessionStorage.getItem('hrs') || 0;
var t;

if(min >= 5) {
    var page_id = $("#page_id").val();
    $.ajax({
        url: '/showPrice',
        method: 'get',
        data: {'watermelonName': page_id},
        success: function (data) {
            var str = data;
            str = str.replace(/[^0-9]/g, '');
            result.text('Цена:  ' + str + '   рублей');
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 400) {
                result.text("Комментарий не записан. Информация об ошибках: " + jqXHR.responseText);
            } else {
                result.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled', false);
        }
    });
}
if (min < 5) {
    var page_id = $("#page_id").val();
    $.ajax({
        url: '/showSale',
        method: 'get',
        data: {'watermelonName': page_id},
        success: function (data) {
            var str = data;
            str = str.replace(/[^0-9]/g, '');
            resultsale.text( 'Всего '+str + ' рублей!');
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 400) {
                resultsale.text("Комментарий не записан. Информация об ошибках: " + jqXHR.responseText);
            } else {
                resultsale.text("Произошла неизвестная ошибка: " + jqXHR.responseText);
            }
            sendForm.prop('disabled', false);
        }
    });
}

function tick() {
    sec++;
    if (sec >= 60) {
        sec = 0;
        min++;
        if (min >= 60) {
            min = 0;
            hrs++;
        }
    }

    sessionStorage.setItem('sec', sec);
    sessionStorage.setItem('min', min);
    sessionStorage.setItem('hrs', hrs);
}

function add() {
    tick();
    h1.textContent = (hrs > 9 ? hrs : "0" + hrs)
        + ":" + (min > 9 ? min : "0" + min)
        + ":" + (sec > 9 ? sec : "0" + sec) + ' цена на данный момент ниже, успейте купить, у вас всего 5 минут !';
    timer();
}

function timer() {
    if (min >= 5) {
        h1.textContent = 'Aкция на данный товар закончилась'
    }
    t = setTimeout(add, 1000);
}

timer();

