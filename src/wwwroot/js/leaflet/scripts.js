// check if a map is present on the page
let mapElement = document.querySelector('#map');
if (mapElement) {

  let longitudeInput = document.querySelector('#longitude');
  let latitudeInput = document.querySelector('#latitude');
  let radiusInput = document.querySelector('#radius');

  // initialize leaflet
  let map = L.map('map');

  // add the tile layer
  L.tileLayer('https://tile.openstreetmap.de/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
  }).addTo(map);

  // add the fullscreen button
  let fullscreen = new L.Control.Fullscreen();
  map.addControl(fullscreen);

  let suppliedLongitude = mapElement.dataset.longitude;
  let suppliedLatitude = mapElement.dataset.latitude;
  let suppliedRadius = mapElement.dataset.radius;
  let suppliedZoom = mapElement.dataset.zoom;

  if (suppliedLongitude && suppliedLatitude && suppliedRadius) {
    // initialize the circle if a radius should be shown
    let circle = L.circle([suppliedLatitude, suppliedLongitude], {
      radius: suppliedRadius * 1000,
      color: 'var(--green)'
    });
    circle.addTo(map);

    // set the view
    let zoom = Math.max(9 - suppliedRadius * 0.015, 3);
    map.setView([suppliedLatitude, suppliedLongitude], zoom);

    // handle clicking on the map
    function onMapClick(e) {
      longitudeInput.value = e.latlng.lng;
      latitudeInput.value = e.latlng.lat;
      circle.setLatLng(e.latlng);
    }

    // make circle position editable
    if (longitudeInput && latitudeInput) {
      map.on('click', onMapClick);
    }

    // make circle radius editable
    radiusInput.addEventListener('change', (event) => {
      circle.setRadius(event.target.value * 1000);
    });
  }
  else {
    // initialize the marker with the icon
    let icon = L.icon({
      iconUrl: '/img/marker-fill.svg',
      iconSize: [70, 70],
      iconAnchor: [35, 60],
      popupAnchor: [0, 0]
    });
    let marker = L.marker([0, 0], { icon: icon });
    marker.addTo(map);

    // handle clicking on the map
    function onMapClick(e) {
      longitudeInput.value = e.latlng.lng;
      latitudeInput.value = e.latlng.lat;
      marker.setLatLng(e.latlng);
    }

    // set the view and marker
    if (suppliedLongitude && suppliedLatitude) {
      let zoom = suppliedZoom ? suppliedZoom : 5;
      map.setView([suppliedLatitude, suppliedLongitude], zoom);
      marker.setLatLng([suppliedLatitude, suppliedLongitude]);
    }

    // make marker editable
    if (longitudeInput && latitudeInput) {
      map.on('click', onMapClick);
    }
  }
}
