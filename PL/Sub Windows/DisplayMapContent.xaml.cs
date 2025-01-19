using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
namespace PL.Sub_Windows;
using System;
using System.Windows;

public partial class DisplayMapContent : UserControl
{
    private const string GOOGLE_MAPS_API_KEY = "AIzaSyAzbe6J2zmObUxADHW0yfmaJ-9thYaD_mE";
    public int RangeHtml { get; set; }
    public (double, double) Source { get; set; }
    public (double, double) Dest { get; set; }
    public List<(double, double)> ListOfPoints { get; set; }
    public string LoadedFunction { get; set; }
    public TypeOfMap Type { get; set; }
    public DisplayMapContent(TypeOfMap type, BO.TypeOfRange range, IEnumerable<(double, double)> listOfPoints)
    {
        ListOfPoints = listOfPoints.ToList();

        switch (type)
        {
            case TypeOfMap.Pin:
                LoadedFunction = "ShowPinLocations";
                break;
            case TypeOfMap.Route:
                LoadedFunction = "ShowRoute";
                break;
            case TypeOfMap.MultipleTypeOfRoutes:
                LoadedFunction = "ShowMultipleRoutes";
                break;
            default:
                LoadedFunction = "ShowMultipleRoutes";
                break;
        }
        RangeHtml = range.GetHashCode();
        Source = listOfPoints.FirstOrDefault();
        Type = type;
        InitializeComponent();
        InitializeWebBrowser();
    }

    private string GetHtmlCordinatesList() => string.Join(",", ListOfPoints.Select(point => $"{{lat: {point.Item1}, lng: {point.Item2}}}"));

    private async void InitializeWebBrowser()
    {
        try
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.NavigateToString(LoadHtmlForPins());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"WebView2 initialization error: {ex.Message}");
        }
    }
    private string LoadHtmlForPins() =>
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
    <div id='infoPanel'>
    </div>
    <div id='map'></div>

    <!--Route Calculation-->
    <script>
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

        function ShowRoute(mode = "+RangeHtml+@") {
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
        const locations = ["+GetHtmlCordinatesList()+@"];

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

        // Colors for different modes
        const routeColors = {
          air: ""#4169E1"", // Royal Blue
          driving: ""#228B22"", // Forest Green
          walking: ""#8B4513"", // Saddle Brown
        };
        // Calculate air distance
        const R = 6371; // Earth's radius in km
        const lat1 = (locations[0].lat * Math.PI) / 180;
        const lat2 = (locations[1].lat * Math.PI) / 180;
        const dLat = ((locations[1].lat - locations[0].lat) * Math.PI) / 180;
        const dLon = ((locations[1].lng - locations[0].lng) * Math.PI) / 180;

        const a =
          Math.sin(dLat / 2) * Math.sin(dLat / 2) +
          Math.cos(lat1) *
            Math.cos(lat2) *
            Math.sin(dLon / 2) *
            Math.sin(dLon / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        const airDistance = R * c;

        // Draw air route
        const flightPath = new google.maps.Polyline({
          path: [locations[0], locations[1]],
          geodesic: true,
          strokeColor: routeColors.air,
          strokeOpacity: 1.0,
          strokeWeight: 2,
        });
        flightPath.setMap(map);

        // Add markers
        locations.forEach((location, index) => {
          new google.maps.Marker({
            position: location,
            map: map,
            label: index === 0 ? ""S"" : ""D"",
          });
        });

        // Calculate driving and walking routes
        directionsService = new google.maps.DirectionsService();

        // Driving route
        const drivingRenderer = new google.maps.DirectionsRenderer({
          map: map,
          suppressMarkers: true,
          polylineOptions: {
            strokeColor: routeColors.driving,
            strokeWeight: 4,
          },
        });

        // Walking route
        const walkingRenderer = new google.maps.DirectionsRenderer({
          map: map,
          suppressMarkers: true,
          polylineOptions: {
            strokeColor: routeColors.walking,
            strokeWeight: 4,
          },
        });

        // Request driving route
        directionsService.route(
          {
            origin: locations[0],
            destination: locations[1],
            travelMode: google.maps.TravelMode.DRIVING,
          },
          (result, status) => {
            if (status === ""OK"") {
              drivingRenderer.setDirections(result);
              const drivingDistance = result.routes[0].legs[0].distance.text;
              const drivingDuration = result.routes[0].legs[0].duration.text;

              document.getElementById(
                ""infoPanel""
              ).innerHTML += `<div style=""color:${routeColors.driving}"">
          Driving Distance: ${drivingDistance}<br>
          Duration: ${drivingDuration}
         </div><br>`;
            }
          }
        );

        // Request walking route
        directionsService.route(
          {
            origin: locations[0],
            destination: locations[1],
            travelMode: google.maps.TravelMode.WALKING,
          },
          (result, status) => {
            if (status === ""OK"") {
              walkingRenderer.setDirections(result);
              const walkingDistance = result.routes[0].legs[0].distance.text;
              const walkingDuration = result.routes[0].legs[0].duration.text;

              document.getElementById(
                ""infoPanel""
              ).innerHTML += `<div style=""color:${routeColors.walking}"">
          Walking Distance: ${walkingDistance}<br>
          Duration: ${walkingDuration}
         </div><br>`;
            }
          }
        );

        // Add air distance to info panel
        document.getElementById(""infoPanel"").innerHTML = `<div style=""color:${
          routeColors.air
        }"">
      Air Distance: ${airDistance.toFixed(2)} km
     </div><br>`;
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