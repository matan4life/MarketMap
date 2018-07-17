var image;
var data;
var allCategories = false;
var markers = [];
var infoWindows = [];
var polygons = [];

function polygonCenter(poly) {
    var lowx,
        highx,
        lowy,
        highy,
        lats = [],
        lngs = [],
        vertices = poly.getPath();

    for (var i = 0; i < vertices.length; i++) {
        lngs.push(vertices.getAt(i).lng());
        lats.push(vertices.getAt(i).lat());
    }

    lats.sort();
    lngs.sort();
    lowx = lats[0];
    highx = lats[vertices.length - 1];
    lowy = lngs[0];
    highy = lngs[vertices.length - 1];
    center_x = lowx + ((highx - lowx) / 2);
    center_y = lowy + ((highy - lowy) / 2);

    return (new google.maps.LatLng(center_x, center_y));
}

$(function () {

    initMap();

    $('.label').click(function () {
        $(this).prev().toggleClass('fa-check');
    });

    window.FontAwesomeConfig = {
        searchPseudoElements: true
    }

    var contentWidth = $('#info-panel').outerWidth();

    var trigger = true;

    $('#trigger').click(function () {
        if (trigger = !trigger) {
            $('#info-content').fadeIn(500);
            $('#trigger i').removeClass('fa-angle-right').addClass('fa-angle-left');
            $('#info-panel').animate({
                width: contentWidth + 'px'
            });
        } else {
            $('#info-content').fadeOut(500);
            $('#trigger i').removeClass('fa-angle-left').addClass('fa-angle-right');
            $('#info-panel').animate({
                width: '30px'
            });
        }
    });

    $('#category-all').click(function () {
        if (allCategories = !allCategories)
            $('.fa-ul i').removeClass('fa-check');
        else
            $('.fa-ul i').addClass('fa-check');
    });
});

function initMap() {

    var getDetails = function (outlet) {
        var str = "<h1>" + outlet.Name +"</h1>" +
            "<p><strong>Address: </strong>" + outlet.Address + "</p>" +
            "<p><strong>Working hours: </strong>" + outlet.WorkingHours + "</p>" +
            "<p><strong>Product categories: </strong>";

        outlet.OutletCategories.forEach(function (oc) {
            str += oc.CategoryName + ", ";
        });

        str = str.substr(0, str.length - 2) + "</p>";
        return str;
    }

    var hexcolor = function (R, G, B) {
        return "#" + R.toString(16) + G.toString(16) + B.toString(16);
    }

    var getColor = function (outlet) {
        console.log('------------');
        if (outlet.OutletCategories.length == 1) {
            console.log('OutletCategories == 1');
            console.log(outlet.Id);
            console.log(outlet.OutletCategories[0].CategoryName);
            var color = outlet.OutletCategories[0].Category.Color;
            var o_hexcolor = hexcolor(color.R, color.G, color.B);
            console.log(o_hexcolor);
            return o_hexcolor;
        } else {
            console.log('OutletCategories > 1');
            return "#8c8c8c";
        }
    }

    console.log('init map');

    var map = new google.maps.Map(document.getElementById('google-map'), {
        center: { lat: 50.003698, lng: 36.228516 },
        zoom: 13,
        disableDoubleClickZoom: true,
        disableDefaultUI: false,
        zoomControl: true
    });

    $.ajax({
        type: 'get',
        url: '/api/get-icon-url',
        success: function (response) {
            image = response;
        },
        failure: function (response) {
            image = "https://httpsimage.com/v2/3839ca17-09fd-43e7-91d6-0efdb51bb477.png";
            console.log('failed to load default icon');
        }
    }).then(function () {
        console.log(image);
    });

    $.ajax({
        type: 'get',
        url: '/api/index-data',
        success: function (response) {

            data = JSON.parse(response);
            console.log(response);

            data.forEach(function (outlet) {

                var vertices = [];

                outlet.Points.forEach(function (p) {
                    vertices.push({ i: p.Order, lat: p.Latitude, lng: p.Longtitude });
                });

                vertices.sort(function (a, b) {
                    return a.i - b.i;
                });

                console.log(vertices);

                var poly = new google.maps.Polygon({
                    map: map,
                    paths: vertices,
                    strokeColor: '#333333',
                    strokeOpacity: 0.8,
                    strokeWeight: 2,
                    fillColor: getColor(outlet),
                    fillOpacity: 0.5
                });

                polygons.push(poly);

                // get polygon center
                var center = polygonCenter(poly);
                console.log(center.lat() + " + " + center.lng());

                var marker = new google.maps.Marker({
                    map: map,
                    icon: image,
                    position: center,
                    size: new google.maps.Size(150, 150),
                    scaledSize: new google.maps.Size(35, 35),
                    anchor: new google.maps.Point(17, 34),
                    anchorPoint: new google.maps.Point(0, -29),
                    animation: google.maps.Animation.DROP,
                });

                markers.push(marker);

                var infowindow = new google.maps.InfoWindow({
                    content: getDetails(outlet)
                });

                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });

            });
        },
        failure: function (response) {
            console.log('failed to load data');
        }
    });

    var input = $("input[name=search-place]")[0];

    var autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', map);

    var infowindow = new google.maps.InfoWindow();
    var marker = new google.maps.Marker({
        map: map,
        anchorPoint: new google.maps.Point(0, -29),
        animation: google.maps.Animation.DROP
    });

    autocomplete.addListener('place_changed', function () {
        infowindow.close();
        marker.setVisible(false);
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            window.alert("No details available for input: '" + place.name + "'");
            return;
        }

        // If the place has a geometry, then present it on a map.
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);  // Why 17? Because it looks good.
        }

        marker.setIcon(/** @type {google.maps.Icon} */({
            //url: place.icon,
            url: image,
            size: new google.maps.Size(150, 150),
            //origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
        }));

        marker.setPosition(place.geometry.location);
        marker.setVisible(true);

        var address = '';
        if (place.address_components) {
            address = [
                (place.address_components[0] && place.address_components[0].short_name || ''),
                (place.address_components[1] && place.address_components[1].short_name || ''),
                (place.address_components[2] && place.address_components[2].short_name || '')
            ].join(' ');
        }

        infowindow.setContent('<div><strong>' + place.name + '</strong><br>' + address);
        infowindow.open(map, marker);
    });
}