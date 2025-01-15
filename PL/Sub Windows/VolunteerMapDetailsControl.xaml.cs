using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace PL.Sub_Windows;
public partial class VolunteerMapDetailsControl : Window
{
    private const string GOOGLE_MAPS_API_KEY = "AIzaSyAzbe6J2zmObUxADHW0yfmaJ-9thYaD_mE"; // Replace with your API key
    private readonly double sourceLat = 40.7128; // New York City coordinates
    private readonly double sourceLog = 40.7128; // New York City coordinates
    private readonly double desLat = -74.0060;
    private readonly double desLog = -74.0060;
    private readonly int defaultZoom = 12;

    public VolunteerMapDetailsControl(double _lat,double _log,double? t_lat,double? t_log)
    {
        sourceLat = _lat;
        sourceLog = _log;
        if(t_lat !=null && t_log != null)
        {
            desLat = (double)t_lat!;
            desLat = (double)t_log!;

        }
        else
        {

        }
        InitializeComponent();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async();
        LoadMap();
    }

    private void LoadMap()
    {
        string html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Google Maps</title>
                    <style>
                        html, body, #map {{
                            height: 100%;
                            margin: 0;
                            padding: 0;
                        }}
                    </style>
                    <script src='https://maps.googleapis.com/maps/api/js?key={GOOGLE_MAPS_API_KEY}'></script>
                    <script>
                        function initMap() {{
                            var location = {{ lat: {sourceLat}, lng: {sourceLog} }};
                            var map = new google.maps.Map(document.getElementById('map'), {{
                                zoom: {defaultZoom},
                                center: location
                            }});
                            var marker = new google.maps.Marker({{
                                position: location,
                                map: map
                            }});
                        }}
                    </script>
                </head>
                <body onload='initMap()'>
                    <div id='map'></div>
                </body>
                </html>";

        webView.NavigateToString(html);
    }

    public void UpdateMapLocation(double latitude, double longitude, int zoom = 12)
    {
        string script = $@"
                var location = new google.maps.LatLng({latitude}, {longitude});
                map.setCenter(location);
                map.setZoom({zoom});
                marker.setPosition(location);";

        webView.CoreWebView2.ExecuteScriptAsync(script);
    }
}