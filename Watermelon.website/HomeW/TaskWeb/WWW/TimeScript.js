function zero_first_format(value) {
    if (value < 10) {
        value = '0' + value;
    }
    return value;
}

function date_time() {
    var current_datetime = new Date();
    var day = current_datetime.getDate();
    var month = zero_first_format(current_datetime.getMonth() + 1);
    var year = current_datetime.getFullYear();
    var hours = zero_first_format(current_datetime.getHours() + 9);
    var minutes = zero_first_format(current_datetime.getMinutes());
    var seconds = zero_first_format(current_datetime.getSeconds());

    if (hours >= 24) {
        hours -= 24;
        day += 1;
        return hours + ":" + minutes + ":" + seconds + " " +  day + "." + month + "." + year ;
    }
    return hours + ":" + minutes + ":" + seconds + " " +  day + "." + month + "." + year ;
}

setInterval(function () {
    document.getElementById('current_date_time_block2').innerHTML = date_time();
}, 1000);