<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RealTime.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no">
    <meta charset="utf-8">


    <link rel="stylesheet" type="text/css" href="style.css" />
     
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false&key=AIzaSyCyeTlQl9EPLPhd-yxioswbnGoM9uh-jQ4">
    </script>
    <style>
    body {
        color: black;
        font-family: arial,sans-serif;
        font-size: 13px;
    }
    </style>
    <script type="text/javascript">
        var map;
        var directionsDisplay;
        var directionsService = new google.maps.DirectionsService();

        $(document).ready(function () {

            //draw a map centered at Empire State Building Newyork
            //var latlng = new google.maps.LatLng(40.748492, -73.985496); 
            var latlng = new google.maps.LatLng(37.79466064876699, -122.42395877838135);
            var myOptions = {
                zoom: 14,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

            directionsDisplay = new google.maps.DirectionsRenderer();
            directionsDisplay.setMap(map);

            var start = "36.845267,-122.53635464";
            var end = "36.3434545,-122.67456655";
            calcRoute(start, end);

            //directionsDisplay.setPanel(document.getElementById("divDirections"));
            /*if ($("#hfReturnValue").val() > '-1')
                calcRoute($("#hfSource").val(), $("#hfDestination").val()); */
            /*
            map.addListener('click', function (e) {
                var sourceLocation = document.getElementById('hfSource');
                sourceLocation.value = e.latLng;
                placeMarkerAndPanTo(e.latLng, map);
            });
            */
        });

        /*function placeMarkerAndPanTo(latLng, map) {
            var marker = new google.maps.Marker({
                position: latLng,
                map: map,
                clickable: true
            });

            // hfMapLocation.value = latLng;
            var infowindow = new google.maps.InfoWindow({
                content: '<p>Marker Location:' + marker.getPosition() + '</p>'
            });

            google.maps.event.addListener(marker, 'click', function () {
                infowindow.open(map, marker);
            });
            var buttonName = document.getElementById("btnSample");
            buttonName.click();
        }*/


        function calcRoute(start, end) {

            var request = {
                origin: start,
                destination: end,
                travelMode: google.maps.TravelMode.DRIVING,
                provideRouteAlternatives: false
            };
            directionsService.route(request, function (result, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(result);
                    displayDirections(result);
                }
            });


        }

        function displayDirections(result) {
            var html = '<div style="margin:5px;padding:5px;background-color:#EBF2FC;border-left: 1px solid #EBEFF9;border-right: 1px solid #EBEFF9;text-align:right;width:500px;">';
            html = html + '<span><strong>' + $.trim(result.routes[0].legs[0].distance.text.replace(/"/g, '')) + ', ' + $.trim(result.routes[0].legs[0].duration.text.replace(/"/g, '')) + '</strong></span></div>';
            $("#divDirections").html(html);

            //Display Steps
            var steps = result.routes[0].legs[0].steps;
            for (i = 0; i < steps.length; i++) {
                var instructions = JSON.stringify(steps[i].instructions);
                var distance = JSON.stringify(steps[i].distance.text);
                var time = JSON.stringify(steps[i].duration.text);
                $("#divDirections").append(getEmbedHTML(i + 1, instructions, distance, time));
            }
        }

        function getEmbedHTML(seqno, instructions, distance, duration) {
            var strhtml = '<div class="row">';
            strhtml = strhtml + '<span>' + seqno + '</span>&nbsp;' + $.trim(instructions.replace(/"/g, '')) + '<br/>'
            strhtml = strhtml + '<div style="text-align:right;"><span>' + $.trim(distance.replace(/"/g, '')) + ' <span></div>'
            strhtml = strhtml + '</div><div class="separator"></div>';

            return strhtml;
        }
    </script>
    </head>
    
<body>

    <div id="container" class="shadow">
        <div id="map_canvas"></div>
        <div id="sidebar">
            <div class="row" style="background:#E3EDFA">
               
                <form id="Form2" runat="server">
                  <asp:Button ID="btnTestData" runat="server" Text="Show Results" OnClick="btnTestData_Click" />
        <asp:HiddenField ID="hfSource" runat="server" />
        <asp:HiddenField ID="hfTime" runat="server" />
        <asp:HiddenField ID="hfNodeId" runat="server" />
                    </form>
 
            </div>
            <div class="separator"></div>
            <div id="divDirections">
 
            </div>
           
        </div>
    </div>

</body>
</html>
    
