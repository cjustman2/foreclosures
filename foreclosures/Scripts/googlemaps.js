var geocoder;
var map;
var markers = [];
function initialize(address) {

    geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {

            map.setCenter(results[0].geometry.location);
        }

    });
    var myOptions = {
        zoom: 10

    }

    map = new google.maps.Map(document.getElementById('inDIV'), myOptions);
}





function addMarker(result, pin) {

    var icon;

    if (result.IsNew)
        icon = 'http://www.google.com/mapfiles/markerN.png';
    else
        icon = 'http://maps.google.com/mapfiles/ms/icons/green-dot.png';

    marker = new google.maps.Marker({
        position: pin,
        map: map,
        animation: google.maps.Animation.DROP,
        title: result.ListingAddress,
        icon: icon

    });

    markers.push(marker);

    if (result.PDFLink != null) {
        var infowindow = new google.maps.InfoWindow();
        google.maps.event.addListener(marker, 'click', (function (marker) {
            return function () {
                var content = '<p>' + result.Price + '<div style="background-color: #ffffff;border-color: #3f89d8;border-radius: 3px;border-style: solid;border-width: 1px;clear: both;cursor: default;padding: 10px;position: relative;min-width:300px"></p><a href="' + result.PDFLink + '" target="_blank">' + result.ListingAddress + '</a><br><a href="' + result.ScopeOfWork + '" target="_blank">Scope of Work</a><br><img src="' + result.Image + '"/></div>';
                infowindow.setContent(content);
                infowindow.open(map, marker);
            }
        })(marker));
    }
}





function MapTINS() {

    // Define the LatLng coordinates for the polygon's path.
    var BORCHERTFIELD = [
      new google.maps.LatLng(43.081512, -87.921085),
   new google.maps.LatLng(43.081712, -87.928585),
   new google.maps.LatLng(43.073112, -87.929185),
    new google.maps.LatLng(43.072912, -87.921385)

    ];
    borchertField = new google.maps.Polygon({
        paths: BORCHERTFIELD,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: '#FF0000',
        fillOpacity: 0.35
    });

    borchertField.setMap(map);


    var CENTURYCITY = [
      new google.maps.LatLng(43.089711, -87.947086),
   new google.maps.LatLng(43.089811, -87.958386),
   new google.maps.LatLng(43.075212, -87.958386),
    new google.maps.LatLng(43.075212, -87.947286)

    ];
    centuryCity = new google.maps.Polygon({
        paths: CENTURYCITY,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: '#FF0000',
        fillOpacity: 0.35
    });

    centuryCity.setMap(map);

    var STJOSEPHS = [
     new google.maps.LatLng(43.071712, -87.967586),
   new google.maps.LatLng(43.071712, -87.978587),
   new google.maps.LatLng(43.075212, -87.978487),
    new google.maps.LatLng(43.075112, -87.967286)
    ]

    stJoesephs = new google.maps.Polygon({
        paths: STJOSEPHS,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: '#FF0000',
        fillOpacity: 0.35
    });

    stJoesephs.setMap(map);

    // var BURNHAMLAYTON = [
    //new google.maps.LatLng(43.010314, -87.948085),


    // ];

    //burnhamLayton = new google.maps.Polygon({
    //    paths: BORCHERTFIELD,
    //    strokeColor: '#FF0000',
    //    strokeOpacity: 0.8,
    //    strokeWeight: 2,
    //    fillColor: '#FF0000',
    //    fillOpacity: 0.35
    //});



    // burnhamLayton.setMap(map);


}

function ClearMap() {

    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);

    }
}