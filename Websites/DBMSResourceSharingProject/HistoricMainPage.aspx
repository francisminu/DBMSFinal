<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HistoricMainPage.aspx.cs" Inherits="HistoricMainPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Find Parking</title>
    <!--CSS Files -->
    <link href="css/reset.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <!--Javascript File -->
    <script src="js/GetData.js"></script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDjPuHirjPSHYrPS4SbpwwoKCaIkOzK7fY"></script>
</head>
<body onload="initialize()">
	<div id="control">
    	<h2><b>Looking for a Parking spot</b></h2>
		<p>Let us help you find one!</p>
		<form runat="server">
            <asp:HiddenField ID ="totalTimeTaken" runat="server"/>
            <asp:HiddenField ID ="queryTime" runat="server" />
            <asp:ScriptManager ID="forPageMethods" runat="server" EnablePageMethods="true"/>
            <input id="latitudeSrc" type="text"  value="37.800113" placeholder="Enter Source Latitude" /><br /><br />
            <input id="longitudeSrc" type="text" value="-122.420867" placeholder="Enter Source Longitude" /><br /><br />
       <!--       <input id="latitudeDest" type="text" value="37.802326" placeholder="Enter Destination Latitude" /><br /><br />  -->
      <!--      <input id="longitudeDest" type="text" value="-122.424665" placeholder="Enter Destination Longitude" /><br /><br /> -->
            

            <p>Enter Request Time</p>
            <input id="requestDate" type="date"  style="width: 100%;height: 40px;font-size: 24px;background-color: #6B7283;border: 1px solid #BE757E;" /> <br /><br />
            <input id="requestTime" type="time" style="width: 100%;height: 40px;font-size: 24px;background-color: #6B7283;border: 1px solid #BE757E;" /><br /><br />
		<!--	<input id="timeTaken"  type="text" placeholder="Time Taken to Run" disabled="disabled" /> -->
            <button type="button" id="searchParking" onclick="callLookForParking()" class="learnButton">Look for Parking</button>
		    
            <asp:button type="button" runAt="server" OnClick="getDataFromExcel" 
                style="	float: left;margin-bottom: 20px;background: #BE757E;border: 0;color: #D8D8DD;
		        height: 37px;line-height: 37px;	width: 100%;text-align: center;" text="Show Results" class="LearnButton"></asp:button>  
        </form>
	</div>
   
    <div id="right-panel" class="control" visible="false">
        <h2>Instructions</h2>
  
    </div>
	<!--Map Canvas -->
	<div id="map-canvas"></div>
  </body>
</html>