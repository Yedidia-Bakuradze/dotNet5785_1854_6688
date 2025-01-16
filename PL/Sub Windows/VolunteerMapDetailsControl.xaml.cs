using System.Windows.Controls;
namespace PL.Sub_Windows;
using System;
using System.Windows;

public partial class VolunteerMapDetailsControl : Window
{
    private const string GOOGLE_MAPS_API_KEY = "AIzaSyAzbe6J2zmObUxADHW0yfmaJ-9thYaD_mE"; // Replace with your API key
    private readonly double sourceLat = 40.7128; // New York City coordinates
    private readonly double sourceLog = 40.7128; // New York City coordinates
    private readonly double desLat = -74.0060;
    private readonly double desLog = -74.0060;
    private readonly int defaultZoom = 12;
    public VolunteerMapDetailsControl()
    {
        if(true)
        {
            InitializeComponent();
            RouteMapInit();
        }
        else
        {
            //Show Map
            InitializeComponent();
            PinMapInit();
        }
    }

    private async void PinMapInit()
    {
        try
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.NavigateToString(GetPinMap(sourceLat,sourceLog));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"WebView2 initialization error: {ex.Message}");
        }
    }
    private async void RouteMapInit()
    {
        try
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.NavigateToString(GetRouteMap());

            // Give the map a moment to initialize before calculating the route
            await Task.Delay(1000);
            await webView.CoreWebView2.ExecuteScriptAsync("calculateRoute();");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"WebView2 initialization error: {ex.Message}");
        }
    }
    private string GetPinMap(double startLat, double startLog)
    {
        return @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <title>Google Maps</title>
            <style>
                html, body { height: 100%; margin: 0; padding: 0; }
                #map { height: 100%; }
            </style>
        </head>
        <body>
            <div id='map'></div>
            <script>
                function initMap() {
                    // Coordinates for the markers
                    const startCoord = { lat: " + startLat + ", lng: " + startLog + @" }; // Example: New York
                    const endCoord = { lat: " + 34.0522 + ", lng: " + -118.2437 + @" }; // Example: Los Angeles

                    // Initialize the map centered at the start coordinate
                    const map = new google.maps.Map(document.getElementById('map'), {
                        center: startCoord,
                        zoom: 5
                    });

                    // Add markers for the start and end coordinates
                    new google.maps.Marker({
                        position: startCoord,
                        map: map,
                        title: 'Start Point'
                    });

                    new google.maps.Marker({
                        position: endCoord,
                        map: map,
                        title: 'End Point'
                    });
                }
            </script>
            <script src='https://maps.googleapis.com/maps/api/js?key=" + GOOGLE_MAPS_API_KEY + @"&callback=initMap' async defer></script>
        </body>
        </html>";
    }

    private string GetRouteMap()
    {
        return @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Google Maps Route</title>
                    <style>
                        html, body { height: 100%; margin: 0; padding: 0; }
                        #map { height: 100%; }
                        #distance { 
                            position: absolute; 
                            top: 10px; 
                            left: 10px; 
                            background: white; 
                            padding: 10px; 
                            border-radius: 5px;
                            box-shadow: 0 2px 6px rgba(0,0,0,0.3);
                        }
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <div id='distance'></div>
                    <script>
                        let map;
                        let directionsService;
                        let directionsRenderer;

                        // Define your coordinates here
                        const startCoords = { lat: "+ 40.7128+", lng:"+ -74.0060+@" }; // New York
                        const endCoords = { lat: "+42.3601+", lng:"+ -71.0589+@" };   // Boston

                        function initMap() {
                            map = new google.maps.Map(document.getElementById('map'), {
                                center: startCoords,
                                zoom: 7
                            });

                            directionsService = new google.maps.DirectionsService();
                            directionsRenderer = new google.maps.DirectionsRenderer();
                            directionsRenderer.setMap(map);
                        }

                        function calculateRoute() {
                            const request = {
                                origin: startCoords,
                                destination: endCoords,
                                travelMode: google.maps.TravelMode.DRIVING
                            };

                            directionsService.route(request, (result, status) => {
                                if (status === 'OK') {
                                    directionsRenderer.setDirections(result);
                                    
                                    // Calculate and display the distance
                                    const route = result.routes[0];
                                    const distanceText = route.legs[0].distance.text;
                                    const durationText = route.legs[0].duration.text;
                                    document.getElementById('distance').innerHTML = 
                                        `Distance: ${distanceText}<br>Duration: ${durationText}`;
                                } else {
                                    alert('Could not calculate directions: ' + status);
                                }
                            });
                        }
                    </script>
                    <script src='https://maps.googleapis.com/maps/api/js?key=" + GOOGLE_MAPS_API_KEY + @"&callback=initMap' async defer></script>
                </ body >
                </ html > ";
    }
}
