//global
var newLatitude = "";
var newLongitude = "";
var latitudeSrc = "";
var longitudeSrc = "";
var myMap = "";
var stepsLeft = 3;
var latitudeDest = "";
var longitudeDest = "";
var requestTime = "";
var instructCount = 0;
var weightAssigned = "";

//testing Global Variables
var startTime = "";
var endTime = "";
var startRequestTime = new Date();
var firstRequest = 1;
var addTime = "";
var totalTimes = [];
var algoTimes = [];
var globalCounter = 0;
var inTest = 0;

//Show Test Panel

function showTestPanel()
{
    document.getElementById('testPanel').style.visible = true;
  
}

//Initial Loading Map
function initialize() {
    newLatitude = "";
    newLongitude = "";
    latitudeSrc = "";
    longitudeSrc = "";
    myMap = "";
    stepsLeft = 3;
    latitudeDest = "";
    longitudeDest = "";
    requestTime = "";
    weightAssigned = ""
   
    var mapOptions =
        {
            center: new google.maps.LatLng(37.8039, -122.4268),
            zoom: 15,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        myMap = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
}


function recurse(requestTime)
{
    //--console.log('recurse ' + requestTime);
    lookForParking(latitudeSrc, longitudeSrc, latitudeDest, longitudeDest, requestTime);
}

//look for Parking
function callLookForParking()
{
    addTime = 0;
    //start Timer
    startTime = Date.now();
    //--console.log('The start time set is '+startTime);

    //get input
    latitudeSrc = document.getElementById('latitudeSrc').value;
    longitudeSrc = document.getElementById('longitudeSrc').value;
    latitudeDest = document.getElementById('latitudeSrc').value;
    longitudeDest = document.getElementById('longitudeSrc').value;
    var reqDate = document.getElementById('requestDate').value;
    var reqTime = document.getElementById('requestTime').value;
    requestTime = reqDate + " " + reqTime + ":00.00";


    //call search function
    
    lookForParking(latitudeSrc, longitudeSrc, latitudeDest, longitudeDest, requestTime);
}

function testVals(latitudeSrc, longitudeSrc, latitudeDest, longitudeDest, requestTime)
{
    inTest = 1;
    debugger;
    initialize();
    if (globalCounter == 0) {
        alert('called');
        // var json = new JavaScriptSerializer().Serialize(latitudeSrc);
        //  console.log(json);
        var latitudeSrcVals = latitudeSrc.split(",");
        var longitudeSrcVals = longitudeSrc.split(",");
        var latitudeDestVals = latitudeDest.split(",");
        var longitudeDestVals = longitudeDest.split(",");
        var requestTimeVals = requestTime.split(",");
        console.log(latitudeSrcVals.length + ' latitudeSrcVals');
        console.log(longitudeSrcVals.length + ' longitudeSrcVals');

        document.onload = function () {
            console.log(latitudeSrcVals[1] + ' latitudeSrcVals[i]');
            lookForParking(latitudeSrcVals[1], longitudeSrcVals[1], latitudeDestVals[1], longitudeDestVals[1], requestTimeVals[1]);
        }
    }
    else {
        console.log(latitudeSrcVals[globalCounter] + ' latitudeSrcVals[i]');
        lookForParking(latitudeSrcVals[globalCounter], longitudeSrcVals[globalCounter], latitudeDestVals[globalCounter], longitudeDestVals[globalCounter], requestTimeVals[globalCounter]);
        if (globalCounter == 49)
            inTest = 0;
    }
}

function lookForParking(latitudeSrc,longitudeSrc,latitudeDest,longitudeDest,requestTime)
{
  
    //--console.log('Values fetched from callLookForParking : ' +latitudeSrc+' '+longitudeSrc+' '+latitudeDest+' '+longitudeDest+' '+requestTime)
    PageMethods.getClosestIntersections(latitudeDest, longitudeDest, requestTime, returnedLocationVals);    
}


function returnedLocationVals(location) {
    console.log('The value returned by the code behind is : ' + location);
    newLatitude = location.split(",")[1];
    newLongitude = location.split(",")[0];
    weightAssigned = location.split(",")[2];
    console.log('After Split' + newLatitude + ' and ' + newLongitude + ' and weight is ' + weightAssigned);
    plotOnMap(newLatitude, newLongitude,weightAssigned);
}


function plotOnMap(newLatitude, newLongitude, weightAssigned)
{
    var startPoint = new google.maps.LatLng(latitudeSrc, longitudeSrc);
    console.log('The values of start point are ' + latitudeSrc + ',' + longitudeSrc);
    var endPoint = new google.maps.LatLng(newLatitude, newLongitude);
    console.log('The values of the end point are ' + newLatitude + ',' + newLongitude);
    var directionsService = new google.maps.DirectionsService;
    var directionsDisplay = new google.maps.DirectionsRenderer({ map: myMap });
    var stepDisplay = new google.maps.InfoWindow;
    directionsDisplay.setMap(myMap);
    //directionsDisplay.setPanel(document.getElementById('right-panel'));
    var markerArray = [];

    //plot route
    for (var i = 0; i < markerArray.length; i++)
    {
        markerArray[i].setMap(null);
    }
    directionsService.route({
        origin: startPoint,
        destination: endPoint,
        travelMode: google.maps.TravelMode.DRIVING
    }, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
            instructCount++;
            document.getElementById('right-panel').innerHTML += "<br/><b>" + instructCount + " <b>." + response.routes[0].legs[0].steps[0].instructions + "</b></br></ br></ br>";
            console.log('The Instruction Text : ' + response.routes[0].legs[0].steps[0].instructions);
            var myRoute = response.routes[0].legs[0];
            console.log('Number of steps in the myRoute Var : ' + myRoute.steps.length);
            for (var i = 0; i < myRoute.steps.length; i++) {
                var marker = markerArray[i] = markerArray[i] || new google.maps.Marker;
                marker.setMap(myMap);
                marker.setPosition(myRoute.steps[i].start_location);
                console.log('The start position ' + myRoute.steps[i].start_location);
                console.log('The end position ' + myRoute.steps[i].end_location);
                google.maps.event.addListener(marker, 'click', function () {
                   // stepDisplay.setContent(myRoute.steps[i].instructions + weightAssigned);
                    console.log('weight in marker ' + weightAssigned);
                    stepDisplay.open(myMap, marker);
                });
            }


            //get start location
            if (myRoute.steps.length > 0) {
                //since we have a start_location here
                if (myRoute.steps.length > 1) {
                    latitudeSrc = myRoute.steps[1].start_location.lat();
                    longitudeSrc = myRoute.steps[1].start_location.lng();
                    console.log('New Start Location ' + latitudeSrc + ',' + longitudeSrc);
                    stepsLeft = myRoute.steps.length - 1;

                    //get time details
                    var timeTakenToReach = myRoute.steps[1].duration.value;
                    addTime = addTime + timeTakenToReach;
                    console.log('Time Taken to Reach : ' + addTime);
                    var getDateFromDate = requestTime.split(' ')[0];
                    var getOnlyDate = getDateFromDate.split('-');
                    var getTimeFromDate = requestTime.split(' ')[1];
                    var getTime = getTimeFromDate.split('.')[0];
                    var getOnlyTime = getTime.split(':');
                    var formattedDate = new Date(getOnlyDate[0], getOnlyDate[1], getOnlyDate[2], getOnlyTime[0], getOnlyTime[1], getOnlyTime[2]);
                    //--console.log('The date is ' + formattedDate);

                    //check if first request
                    if (firstRequest === 1)
                    {
                        startRequestTime = formattedDate;
                        firstRequest = 0;
                    }

                    //add the new time
                    formattedDate.setSeconds(formattedDate.getSeconds() + timeTakenToReach);
                    //--console.log('After adding ' + timeTakenToReach + ' seconds, the time now is' + formattedDate);

                    //reformat date
                    requestTime = formattedDate.getFullYear() + "-" + "0" + (formattedDate.getMonth()) + "-" + formattedDate.getDate() + " " + formattedDate.getHours() + ":" + formattedDate.getMinutes() + ":" + formattedDate.getSeconds() + ".00";
                    //--console.log('Now request time is : ' + requestTime);

                    //recurse
                    recurse(requestTime);
                }

                //since we don't have a start_location here
                else
                {
                    latitudeSrc = myRoute.steps[0].start_location.lat();
                    longitudeSrc = myRoute.steps[0].start_location.lng();
                    console.log('New Start Location in 0 ' + latitudeSrc + ',' + longitudeSrc);
                    stepsLeft = myRoute.steps.length - 1;

                    //get time details
                    var timeTakenToReach = myRoute.steps[0].duration.value;
                    var startPoint = new google.maps.LatLng(document.getElementById('latitudeSrc').value, document.getElementById('longitudeSrc').value);
                    var endPoint = myRoute.steps[0].end_location;
                    addTime = addTime + timeTakenToReach;
                    console.log('Final Time ' + addTime);
                    //copying to Array
                    totalTimes.push(addTime);
                    console.log('TotalTimes is ' + totalTimes);
                    document.getElementById('totalTimeTaken').value = addTime;
                    console.log('Hidden field set');
                    //--alert(document.getElementById('totalTimeTaken').value);
                    //--console.log('Time Taken to Reach : ' + timeTakenToReach);
                    var getDateFromDate = requestTime.split(' ')[0];
                    var getOnlyDate = getDateFromDate.split('-');
                    var getTimeFromDate = requestTime.split(' ')[1];
                    var getTime = getTimeFromDate.split('.')[0];
                    var getOnlyTime = getTime.split(':');
                    var formattedDate = new Date(getOnlyDate[0], getOnlyDate[1], getOnlyDate[2], getOnlyTime[0], getOnlyTime[1], getOnlyTime[2]);
                    //--console.log('The date is ' + formattedDate);

                    //add the new time
                    formattedDate.setSeconds(formattedDate.getSeconds() + timeTakenToReach);
                    //--console.log('After adding ' + timeTakenToReach + ' seconds, the time now is' + formattedDate);

                    //reformat date
                    requestTime = formattedDate.getFullYear() + "-" + "0" + (formattedDate.getMonth()) + "-0" + formattedDate.getDate() + " " + formattedDate.getHours() + ":" + formattedDate.getMinutes() + ":" + formattedDate.getSeconds() + ".00";
                    //--console.log('Now request time is : ' + requestTime);
                    // writeToExcel();
                    if (inTest == 1)
                    {
                        testVals();
                    }
                }
            }

            //measuring time taken
            endTime = Date.now();
            var finalTime = endTime - startTime;
            //--console.log('The start Time is ' + startTime + ' the end time is ' + endTime);
            document.getElementById('timeTaken').value = finalTime;
            //--console.log('The final time is : ' + finalTime)

            console.log('Start Time is ' + startRequestTime);
            console.log('End Time is ' + formattedDate);
            var ttTime = formattedDate.getSeconds() - startRequestTime.getSeconds();
            console.log('The total Time Taken is  : ' + ttTime);
            algoTimes.push(ttTime);
           
        }
        else
        {
            window.alert('No Parking Slot Available. Please Try again after sometime.');
        }
    });
    
}

function writeToExcel()
{
    var finalVal = '';
    console.log('in write to excel')
    for (var i = 0; i < totalTimes.length; i++) {
        var value = totalTimes[i];
        console.log('value is '+value);
        for (var j = 0; j < value.length; j++) {
            var innerValue = value[j];
            console.log('inner value is '+innerValue);
            var result = innerValue.replace(/"/g, '""');
            if (result.search(/("|,|\n)/g) >= 0)
                result = '"' + result + '"';
            console.log('result is'+result);
            if (j > 0)
                finalVal += ',';
            finalVal += result;
        }

        finalVal += '\n';
    }

    var pom = document.createElement('a');
    pom.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(finalVal));
    pom.setAttribute('download', 'test.csv');
    pom.click();

}