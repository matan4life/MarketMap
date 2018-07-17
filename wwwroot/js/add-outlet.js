var drawingManager;
var selectedShape;
var image;

$.ajax({
    type: 'get',
    url: '/api/get-icon-url',
    success: function (response) {
        image = response;
        console.log(image);
    },
    failure: function (response) {
        image = "https://httpsimage.com/v2/25319ce4-f21d-464e-a74a-f7a429c583fc.png";
        console.log('failed to load default icon');
    }
});

function clearSelection() {
    console.log('clear selection');
    if (selectedShape) {
        selectedShape.setEditable(false);
        selectedShape = null;
    }
}

function setSelection(shape) {
    console.log('set selection');
    shape.setEditable(true);
    selectedShape = shape;
}

function deleteSelectedShape() {
    console.log('delete selected shape');
    if (selectedShape) {
        selectedShape.setMap(null);
        drawingManager.setOptions({
            drawingControl: true
        });
        $("#delete-shape").addClass("hidden");
    }
}

function initMap() {

    var map = new google.maps.Map(document.getElementById('google-map'), {
        center: { lat: 50.003698, lng: 36.228516 },
        zoom: 13,
        disableDoubleClickZoom: true,
        disableDefaultUI: true,
        zoomControl: true
    });

    drawingManager = new google.maps.drawing.DrawingManager({
        map: map,
        //drawingMode: google.maps.drawing.OverlayType.POLYGON,
        drawingControlOptions: {
            drawingModes: ['polygon']
        },
        markerOptions: {
            draggable: true
        },
        polygonOptions: {
            strokeWeight: 0,
            fillOpacity: 0.45,
            editable: true
        }, 
    });

    // Create the search box and link it to the UI element.
    var input = $('input[name=address]')[0];

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

    google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {
        console.log('new shape!');

        $("#delete-shape").removeClass("hidden");

        drawingManager.setDrawingMode(null);
        drawingManager.setOptions({
            drawingControl: false
        });
        
        var newShape = e.overlay;
        newShape.type = e.type;
        setSelection(newShape);
        google.maps.event.addListener(newShape, 'click', function () {
            setSelection(newShape);
        });

        var vertices = [];
        var polygonBounds = newShape.getPath();

        polygonBounds.forEach(function (coordinate, i) {
            vertices.push({ order: i, lat: coordinate.lat(), lng: coordinate.lng() });
            console.log('Coordinate: ' + i + ', ' + coordinate.lat() + ',' + coordinate.lng());
        });

        $("#vertices").val(JSON.stringify(vertices));
    });

    google.maps.event.addListener(drawingManager, 'drawingmode_changed', clearSelection);
    google.maps.event.addListener(map, 'click', clearSelection);
    google.maps.event.addDomListener(document.getElementById('delete-shape'), 'click', deleteSelectedShape);
}

$(function () {
    initMap();

    $('[data-toggle="tooltip"]').tooltip();
});