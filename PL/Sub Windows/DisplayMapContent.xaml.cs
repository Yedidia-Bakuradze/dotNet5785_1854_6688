using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
namespace PL.Sub_Windows;
using System;
using System.Windows;

public partial class DisplayMapContent : UserControl
{
    private const string GOOGLE_MAPS_API_KEY = "AIzaSyDhFsDBvWYHUmKJ-aenR3jXGOV2USDKteU";
    public int RangeHtml { get; set; }
    public string MapHTML { get; set; }
    public (double, double) Source { get; set; }
    public (double, double) Dest { get; set; }
    public List<(double, double)> ListOfPoints { get; set; }
    public string LoadedFunction { get; set; }
    public TypeOfMap Type { get; set; }
    public DisplayMapContent(TypeOfMap type, BO.TypeOfRange range, IEnumerable<(double, double)> listOfPoints)
    {
        ListOfPoints = listOfPoints.ToList();

        LoadedFunction = type switch
        {
            TypeOfMap.Pin => "ShowPinLocations",
            TypeOfMap.Route => "ShowRoute",
            TypeOfMap.MultipleTypeOfRoutes => "ShowMultipleRoutes",
            _ => "ShowMultipleRoutes",
        };
        RangeHtml = range.GetHashCode();
        Source = listOfPoints.FirstOrDefault();
        Type = type;
        MapHTML = GetHtmlContent();
        InitializeComponent();
    }

    private string GetHtmlCordinatesList() => string.Join(",", ListOfPoints.Select(point => $"{{lat: {point.Item1}, lng: {point.Item2}}}"));

    private string GetHtmlContent() =>
        @"<!DOCTYPE html>
<html>
  <head>
    <meta charset='utf-8' />
    <title>Google Maps</title>
    <style>
      html,
      body {
        height: 100%;
        margin: 0;
        padding: 0;
      }

      #map {
        height: 100%;
        width: 100%;
        position: absolute;
        top: 0;
        left: 0;
      }

      #infoPanel {
        position: absolute;
        top: 10px;
        left: 10px;
        background-color: white;
        padding: 10px;
        border-radius: 5px;
        box-shadow: 0 2px 6px rgba(0, 0, 0, 0.3);
        z-index: 1; /* Lower z-index to ensure map controls are above */
        min-width: 200px;
        pointer-events: none; /* Allow clicking through the panel */
      }

      #distance {
        pointer-events: auto; /* Re-enable interactions for the content */
      }
    </style>
  </head>
  <body>
    <div id=""infoPanel"" class=""collapsed""></div>
    <button id=""toggleButton"" class=""collapsed"" onclick=""toggleInfoPanel()"">→</button>
    <div id=""map""></div>

    <!--Route Calculation-->
    <script>
       
          // Add this new function at the start of your script section
      function toggleInfoPanel() {
        const infoPanel = document.getElementById('infoPanel');
        const toggleButton = document.getElementById('toggleButton');
        
        infoPanel.classList.toggle('collapsed');
        toggleButton.classList.toggle('collapsed');
        
        // Update button text
        if (toggleButton.textContent === '←') {
          toggleButton.textContent = '→';
        } else {
          toggleButton.textContent = '←';
        }
      }
    

        let map;
      let directionsService;
      let directionsRenderer;

      function generateColors(count) {
        const colors = [];
        for (let i = 0; i < count; i++) {
          // Use HSL to generate evenly distributed colors
          const hue = (i * 360) / count;
          colors.push(`hsl(${hue}, 100%, 50%)`);
        }
        return colors;
      }

        function ShowRoute(mode = " + RangeHtml+@") {
        // Define multiple coordinates with names
        const locations = ["+GetHtmlCordinatesList()+@"]

        // Define colors for different routes
        const routeColors = generateColors(locations.length - 1);

        // Initialize map
        map = new google.maps.Map(document.getElementById('map'), {
          center: locations[0],
          zoom: 12,
          disableDefaultUI: true, // Removes all default UI elements
          zoomControl: false, // Explicitly disable zoom controls
          mapTypeControl: false, // Disable map type selector
          scaleControl: false, // Disable scale bar
          streetViewControl: false, // Disable Street View
          rotateControl: false, // Disable rotate control
          fullscreenControl: false, // Disable fullscreen control
        });
        directionsService = new google.maps.DirectionsService();

        // Add markers for each location
        locations.forEach((location, index) => {
          new google.maps.Marker({
            position: location,
            map: map,
            label: index === 0 ? 'S' : `D${index}`,
            title: location.name,
          });
        });
        if (mode === 0)
        {
            // Clear existing routes
            if (directionsRenderer)
            {
                directionsRenderer.setMap(null);
            }

            // Calculate and draw air routes to each destination
            for (let i = 1; i < locations.length; i++)
            {
                // Calculate air distance using Haversine formula
                const R = 6371; // Earth's radius in km
                const lat1 = (locations[0].lat * Math.PI) / 180;
                const lat2 = (locations[i].lat * Math.PI) / 180;
                const dLat =
                  ((locations[i].lat - locations[0].lat) * Math.PI) / 180;
                const dLon =
                  ((locations[i].lng - locations[0].lng) * Math.PI) / 180;

                const a =
                  Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                  Math.cos(lat1) *
                    Math.cos(lat2) *
                    Math.sin(dLon / 2) *
                    Math.sin(dLon / 2);
                const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                const distance = R * c;

                // Draw straight line
                const flightPath = new google.maps.Polyline({
                      path: [locations[0], locations[i]],
                      geodesic: true,
                      strokeColor: routeColors[i - 1],
                      strokeOpacity: 1.0,
                      strokeWeight: 2,
                    });

            flightPath.setMap(map);
        }
            return;
        }

        // Calculate routes from start to each destination
        for (let i = 1; i < locations.length; i++)
{
    const directionsRenderer = new google.maps.DirectionsRenderer({
            map: map,
            suppressMarkers: true,
            polylineOptions:
    {
    strokeColor: routeColors[i - 1],
              strokeWeight: 4,
            },
          });

const request = {
            origin: locations[0],
            destination: locations[i],
            travelMode:
mode == 1
  ? google.maps.TravelMode.DRIVING
  : google.maps.TravelMode.WALKING,
          };

directionsService.route(request, (result, status) => {
    if (status === 'OK')
    {
        directionsRenderer.setDirections(result);
        const route = result.routes[0];
        const distance = route.legs[0].distance.text;
        const duration = route.legs[0].duration.text;
    }
});
        }
      }
      //Show Pin Location
      function ShowPinLocations() {
        // Array of coordinates
        const locations = [" + GetHtmlCordinatesList() + @"];

        // Initialize map centered at first location
        const map = new google.maps.Map(document.getElementById('map'), {
          center: locations[0],
          zoom: 12,
        });

        // Add markers for each location
        locations.forEach((location, index) => {
          new google.maps.Marker({
            position: location,
            map: map,
            label: String.fromCharCode(65 + index),
          });
        });
      }


        function ShowMultipleRoutes() {
        const locations = [
"+GetHtmlCordinatesList()+@"
        ];

        // Initialize map
        map = new google.maps.Map(document.getElementById(""map""), {
          center: locations[0],
          zoom: 4,
          disableDefaultUI: true,
          zoomControl: false,
          mapTypeControl: false,
          scaleControl: false,
          streetViewControl: false,
          rotateControl: false,
          fullscreenControl: false,
        });

        directionsService = new google.maps.DirectionsService();

        // Generate unique colors for each destination
        const baseColors = generateColors(locations.length - 1);

        // Add markers
        locations.forEach((location, index) => {
          new google.maps.Marker({
            position: location,
            map: map,
            label: index === 0 ? ""S"" : `D${index}`,
          });
        });

        // Process each destination
        for (let i = 1; i < locations.length; i++) {
          const currentDestination = locations[i];
          const currentColor = baseColors[i - 1];

          // Create route renderers for this destination
          const routeRenderers = {
            air: new google.maps.Polyline({
              geodesic: true,
              strokeColor: currentColor,
              strokeOpacity: 1.0,
              strokeWeight: 2,
              map: map,
            }),
            driving: new google.maps.DirectionsRenderer({
              suppressMarkers: true,
              polylineOptions: {
                strokeColor: adjustColor(currentColor, -20),
                strokeWeight: 4,
              },
              map: map,
            }),
            walking: new google.maps.DirectionsRenderer({
              suppressMarkers: true,
              polylineOptions: {
                strokeColor: adjustColor(currentColor, 20),
                strokeWeight: 4,
              },
              map: map,
            }),
          };

          // Draw air route
          const R = 6371; // Earth's radius in km
          const lat1 = (locations[0].lat * Math.PI) / 180;
          const lat2 = (currentDestination.lat * Math.PI) / 180;
          const dLat =
            ((currentDestination.lat - locations[0].lat) * Math.PI) / 180;
          const dLon =
            ((currentDestination.lng - locations[0].lng) * Math.PI) / 180;

          const a =
            Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(lat1) *
              Math.cos(lat2) *
              Math.sin(dLon / 2) *
              Math.sin(dLon / 2);
          const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
          const airDistance = R * c;

          // Set air route path
          routeRenderers.air.setPath([locations[0], currentDestination]);

          // Calculate driving route
          calculateRoute(
            locations[0],
            currentDestination,
            ""DRIVING"",
            routeRenderers.driving,
            adjustColor(currentColor, -20)
          );

          // Calculate walking route
          calculateRoute(
            locations[0],
            currentDestination,
            ""WALKING"",
            routeRenderers.walking,
            adjustColor(currentColor, 20)
          );
        }
      }

      function calculateRoute(origin, destination, mode, renderer, color) {
        directionsService.route(
          {
            origin: origin,
            destination: destination,
            travelMode: google.maps.TravelMode[mode],
          },
          (result, status) => {
            if (status === ""OK"") {
              renderer.setDirections(result);
              const distance = result.routes[0].legs[0].distance.text;
              const duration = result.routes[0].legs[0].duration.text;
            }
          }
        );
      }

      function adjustColor(hslColor, amount) {
        const match = hslColor.match(/hsl\((\d+),\s*(\d+)%,\s*(\d+)%\)/);
        if (match) {
          const h = match[1];
          const s = match[2];
          const l = Math.min(100, Math.max(0, parseInt(match[3]) + amount));
          return `hsl(${h}, ${s}%, ${l}%)`;
        }
        return hslColor;
      }

      function appendRouteInfo(destIndex, mode, color, distance, duration) {
        const targetDiv =
          document.getElementById(""infoPanel"").children[destIndex - 1];
        if (targetDiv) {
          targetDiv.innerHTML += `
            <div style=""color:${color}"">
              ${
                mode.charAt(0).toUpperCase() + mode.slice(1)
              } Distance: ${distance}<br>
              Duration: ${duration}
            </div>`;
        }
      }
    </script>
    <script
      src='https://maps.googleapis.com/maps/api/js?key=" + GOOGLE_MAPS_API_KEY + @"&callback=" + LoadedFunction + @"'
      async
      defer
    ></script>
  </body>
</html>";
}