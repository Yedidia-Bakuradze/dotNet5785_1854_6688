using System.Windows.Controls;
namespace PL.Sub_Windows;
using System;
using System.Windows;

public partial class VolunteerMapDetailsControl : UserControl
{
    private const string GOOGLE_MAPS_API_KEY = "AIzaSyAzbe6J2zmObUxADHW0yfmaJ-9thYaD_mE";
    public int RangeHtml { get; set; }
    public (double, double) Source { get; set; }
    public (double, double) Dest { get; set; }
    public List<(double,double)> ListOfPoints { get; set; }
    public string LoadedFunction { get; set; }
    public TypeOfMap Type { get; set; }
    public VolunteerMapDetailsControl(TypeOfMap type,BO.TypeOfRange range,IEnumerable<(double,double)>listOfPoints)
    {
        ListOfPoints = listOfPoints.ToList();
        LoadedFunction = type == TypeOfMap.Pin ? "ShowPinLocations" : "ShowRoute";
        RangeHtml = range.GetHashCode();
        Source = listOfPoints.FirstOrDefault();
        Type = type;
        InitializeComponent();
        InitializeWebBrowser();
    }

    private string GetHtmlCordinatesList()
    {
        return string.Join(",", ListOfPoints.Select(point => $"{{lat: {point.Item1}, lng: {point.Item2}}}"));
    }

    private async void InitializeWebBrowser()
    {
        try
        {
            await webView.EnsureCoreWebView2Async();
            switch (Type)
            {
                case TypeOfMap.Pin:
                    {
                        webView.CoreWebView2.NavigateToString(LoadHtmlForPins());
                        break;
                    }
                case TypeOfMap.Route:
                    {
                        Dest = ListOfPoints[1];
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"WebView2 initialization error: {ex.Message}");
        }
    }
    private string LoadHtmlForPins()
    {
        return @"<!DOCTYPE html>
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
      <div id='distance'>Select travel mode...</div>
    </div>
    <div id='map'></div>

    <!--Route Calculation-->
    <script>
      let map;
      let directionsService;
      let directionsRenderer;

      // Define your coordinates here
      const startCoords = { lat:" + Source.Item1 + ", lng: " + Source.Item2 + @" }; // New York
      const endCoords =   { lat:" + Dest.Item1 + ", lng: " + Dest.Item2 + @" }; // Boston

      function ShowRoute() {
        map = new google.maps.Map(document.getElementById('map'), {
          center: startCoords,
          zoom: 12,
        });

        directionsService = new google.maps.DirectionsService();
        directionsRenderer = new google.maps.DirectionsRenderer();
        directionsRenderer.setMap(map);

        calculateRoute(" + RangeHtml + @");
      }

      function calculateRoute(mode) {
        // Clear previous highlighting
        document.getElementById('distance').innerHTML = '';

        let travelMode;
        switch (mode) {
          case 0:
            // Calculate air distance using Haversine formula
            const R = 6371; // Earth's radius in km
            const lat1 = (startCoords.lat * Math.PI) / 180;
            const lat2 = (endCoords.lat * Math.PI) / 180;
            const dLat = ((endCoords.lat - startCoords.lat) * Math.PI) / 180;
            const dLon = ((endCoords.lng - startCoords.lng) * Math.PI) / 180;

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
              path: [startCoords, endCoords],
              geodesic: true,
              strokeColor: '#FF0000',
              strokeOpacity: 1.0,
              strokeWeight: 2,
            });

            flightPath.setMap(map);

            document.getElementById(
              'distance'
            ).innerHTML = `<div class='selected-mode'>Air Travel:</div>
         <div>Distance: ${distance.toFixed(2)} km</div>`;
            return;
          case 1:
            travelMode = google.maps.TravelMode.DRIVING;
            break;
          case 2:
            travelMode = google.maps.TravelMode.WALKING;
            break;
          default:
            alert('Invalid mode');
            return;
        }

        const request = {
          origin: startCoords,
          destination: endCoords,
          travelMode: travelMode,
        };

        directionsService.route(request, (result, status) => {
          if (status === 'OK') {
            directionsRenderer.setDirections(result);
            const route = result.routes[0];
            const distanceText = route.legs[0].distance.text;
            const durationText = route.legs[0].duration.text;

            document.getElementById(
              'distance'
            ).innerHTML = `<div class='selected-mode'>${
              travelMode === google.maps.TravelMode.DRIVING
                ? 'Driving:'
                : 'Walking:'
            }</div>
             <div>Distance: ${distanceText}</div>
             <div>Duration: ${durationText}</div>`;
          } else {
            alert('Directions request failed');
          }
        });
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
    </script>
    <script
      src='https://maps.googleapis.com/maps/api/js?key=" + GOOGLE_MAPS_API_KEY + @"&callback=" + LoadedFunction + @"'
      async
      defer
    ></script>
  </body>
</html>";
    }
}