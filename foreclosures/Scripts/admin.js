//var meter = '<div id="" class="meterHolder"><h4></h4><div class="meter nostripes"><span class="orange" style=""></span></div></div>';
//function PageScrape(id) {
//    // var target = document.getElementById('inDIV');
//    //var spinner = new Spinner().spin(target);

//   // var id = $('#Counties').val();
//    var county = $('#Counties :selected').text();
//    $.ajax({
//        url: '/Home/PageScrape',
//        type: 'Post',
//        data: {
//            countyId: id
//        },
//        success: function (result) {

//            if (result.IsStarted) {
//                $(meter).attr('id', id).appendTo('#meterHolder');
//                $('#' + id + ' h4').html(county);
 
//                GetProgress(id);
//            }


//        },
//        complete: function () {
        
//        },
//        beforeSend: function () {
//            $('#errors').empty();
//        },
//        error: function () {
           
//            alert('Error!');
//        }


//    });
//}



//function GetProgress(id) {
   
//    $.ajax({
//        url: '/Home/Progress',
//        data: { countyId: id },
//        type: 'Post',
//        success: function (result) {
//            $('#' + id + ' span').css('width', result.Complete + '%');
//            if (result.Complete < 100) {

//                setTimeout(function () {
//                    GetProgress(id);
//                }, 500);

//            } else {
//                setTimeout(function () {
//                    $('#' + id).remove();
//                }, 1000);
//            }
//            for (var i = 0; i < result.Errors.length; i++) {
//                $('<p style="color:red">' + result.Errors[i] + '</p>').appendTo('#errors');
//            }
//        }
//    });
//}