
		var
		mapObject,
		markers = [],
		markersData = {
			'Marker': [
			{
				type_point: 'Historic',
				name: 'Open Bus',
				location_latitude: 48.865633, 
				location_longitude: 2.321236,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Open Bus',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Walking',
				name: 'Senna River Tour',
				location_latitude: 48.854183,
				location_longitude: 2.354808,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Senna River Tour',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Museum',
				name: 'Louvre',
				location_latitude: 48.863893, 
				location_longitude: 2.342348,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Louvre',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Museum',
				name: 'Pompidou ',
				location_latitude: 48.860642,
				location_longitude: 2.352245,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Pompidou',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Walking',
				name: 'Tour Eiffel',
				location_latitude: 48.858370, 
				location_longitude: 2.294481,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Tour Eiffel',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Walking',
				name: 'Montparnasse',
				location_latitude: 48.837273,
				location_longitude: 2.335387,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Montparnasse',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Museum',
				name: 'Beaubourg',
				location_latitude: 48.860819, 
				location_longitude: 2.354507,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Beaubourg',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Walking',
				name: 'St. Germain des Prés',
				location_latitude: 48.853798,
				location_longitude: 2.333328,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'St. Germain des Prés',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Walking',
				name: 'Trocadero',
				location_latitude: 48.862880, 
				location_longitude: 2.287205,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Trocadero',
				get_directions_start_address: '',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Walking',
				name: 'Champs-Élysées',
				location_latitude: 48.865784,
				location_longitude: 2.307314,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Champs-Élysées',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Historic',
				name: 'Notre Dame',
				location_latitude: 48.852729, 
				location_longitude: 2.350564,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Notre Dame',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			},
			{
				type_point: 'Historic',
				name: 'Madeleine',
				location_latitude: 48.870587, 
				location_longitude: 2.318943,
				map_image_url: 'img/thumb_map_single_tour.jpg',
				rate: 'Superb | 7.5',
				name_point: 'Madeleine',
				get_directions_start_address: '',
				phone: '+3934245255',
				url_point: 'tour-detail.html'
			}
			]

		};

			var mapOptions = {
				zoom: 14,
				center: new google.maps.LatLng(48.865633, 2.321236),
				mapTypeId: google.maps.MapTypeId.ROADMAP,

				mapTypeControl: false,
				mapTypeControlOptions: {
					style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
					position: google.maps.ControlPosition.LEFT_CENTER
				},
				panControl: false,
				panControlOptions: {
					position: google.maps.ControlPosition.TOP_RIGHT
				},
				zoomControl: true,
				zoomControlOptions: {
					position: google.maps.ControlPosition.RIGHT_BOTTOM
				},
				fullscreenControl: true,
				fullscreenControlOptions: {
					position: google.maps.ControlPosition.LEFT_BOTTOM
				},
				scrollwheel: false,
				scaleControl: false,
				scaleControlOptions: {
					position: google.maps.ControlPosition.TOP_LEFT
				},
				streetViewControl: true,
				streetViewControlOptions: {
					position: google.maps.ControlPosition.LEFT_TOP
				},
				styles: [
					{
						"featureType": "administrative.country",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "administrative.province",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "administrative.locality",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "administrative.neighborhood",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "administrative.land_parcel",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "landscape.man_made",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "simplified"
							}
						]
					},
					{
						"featureType": "landscape.natural.landcover",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "on"
							}
						]
					},
					{
						"featureType": "landscape.natural.terrain",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.attraction",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.business",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.government",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.medical",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.park",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "on"
							}
						]
					},
					{
						"featureType": "poi.park",
						"elementType": "labels",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.place_of_worship",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.school",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "poi.sports_complex",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "road.highway",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "road.highway",
						"elementType": "labels",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "road.highway.controlled_access",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "road.arterial",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "simplified"
							}
						]
					},
					{
						"featureType": "road.local",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "simplified"
							}
						]
					},
					{
						"featureType": "transit.line",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "transit.station.airport",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "transit.station.bus",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "transit.station.rail",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					},
					{
						"featureType": "water",
						"elementType": "all",
						"stylers": [
							{
								"visibility": "on"
							}
						]
					},
					{
						"featureType": "water",
						"elementType": "labels",
						"stylers": [
							{
								"visibility": "off"
							}
						]
					}
				]
			};
			var
			marker;
			mapObject = new google.maps.Map(document.getElementById('map'), mapOptions);
			for (var key in markersData)
				markersData[key].forEach(function (item) {
					marker = new google.maps.Marker({
						position: new google.maps.LatLng(item.location_latitude, item.location_longitude),
						map: mapObject,
						icon: '/Content/img/pins/' + key + '.png',
					});

					if ('undefined' === typeof markers[key])
						markers[key] = [];
					markers[key].push(marker);
					google.maps.event.addListener(marker, 'click', (function () {
				  closeInfoBox();
				  getInfoBox(item).open(mapObject, this);
				  mapObject.setCenter(new google.maps.LatLng(item.location_latitude, item.location_longitude));
				 }));

	});
	
		new MarkerClusterer(mapObject, markers[key]);
	
		function hideAllMarkers () {
			for (var key in markers)
				markers[key].forEach(function (marker) {
					marker.setMap(null);
				});
		};

		function closeInfoBox() {
			$('div.infoBox').remove();
		};

		function getInfoBox(item) {
			return new InfoBox({
				content:
				'<div class="marker_info" id="marker_info">' +
				'<img src="' + item.map_image_url + '" alt=""/>' +
				'<span>'+ 
					'<span class="infobox_rate">'+ item.rate +'</span>' +
					'<h3>'+ item.name_point +'</h3>' +
				'<em>'+ item.type_point +'</em>' +
				'<strong>'+ item.description_point +'</strong>' +
				'<a href="'+ item.url_point + '" class="btn_infobox_detail"></a>' +
				'<form action="https://maps.google.com/maps" method="get" target="_blank"><input name="saddr" value="'+ item.get_directions_start_address +'" type="hidden"><input type="hidden" name="daddr" value="'+ item.location_latitude +',' +item.location_longitude +'"><button type="submit" value="Get directions" class="btn_infobox_get_directions">Get directions</button></form>' +
					'<a href="tel://' + item.phone +'" target="_blank" class="btn_infobox_phone">'+ item.phone +'</a>' +
					'</span>' +
				'</div>',
				disableAutoPan: false,
				maxWidth: 0,
				pixelOffset: new google.maps.Size(10, 92),
				closeBoxMargin: '',
				closeBoxURL: "/Content/img/close_infobox.png",
				isHidden: false,
				alignBottom: true,
				pane: 'floatPane',
				enableEventPropagation: true
			});
		};
		
function onHtmlClick(location_type, key){
     google.maps.event.trigger(markers[location_type][key], "click");
}
