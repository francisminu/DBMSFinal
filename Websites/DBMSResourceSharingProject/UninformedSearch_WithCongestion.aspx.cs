/*
 * FileName: UninformedSearch_WithCongestion.aspx.cs
 * This page contains the Algorithm Implementation for Uninformed Search With Congestion
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using DBMSResourceSharingProjectBLL;
using System.Threading;
using System.Net;
using System.Xml;
using System.IO;
using System.Data.OleDb;
using System.Diagnostics;

public partial class UninformedSearch_WithCongestion : System.Web.UI.Page
{
    public string source = ""; 
    public string destination = "";
    ResourceSharingBLL bllObject = new ResourceSharingBLL();


    protected void Page_Load(object sender, EventArgs e)
    {
        //lblStartTime.Text = DateTime.Now.ToString();

    }
    int timeVal = 0;

    public void runTestCases(string location)
    {
        
        hfSource.Value = "";
        getValues();
    }

    DateTime dateTime1;

    /*
     * Function to find the available parking
     * */
    public void getValues()
    {
        decimal sourceIntersectionLat, sourceIntersectionLng, destIntersectionLat, destIntersectionLng;
        ArrayList intersectionPoints = new ArrayList();
        DataSet ds = new DataSet();
        int flag = 0;
        ds = bllObject.getIntersections(flag, 0, 0);
        bllObject.deleteSourceIntersectionDistances();
        double sourceLat;
        double sourceLng;

        /* The source location obtained from map click is stored in the hidden field - hfSource */

        if (hfSource.Value != "")
        {
            if (hfTime.Value != "")
            {
                string a = hfSource.Value.Split(',')[0];
                string b = hfSource.Value.Split(',')[1];

                sourceLat = Convert.ToDouble(a);
                sourceLng = Convert.ToDouble(b);
                source = a + "," + b; 

            }
            else
            {
                // The below 2 steps remove the '(' and ')' brackets from hfSource.value

                string a = hfSource.Value.Split(',')[0].Substring(1, hfSource.Value.Split(',')[0].Length - 1);
                string b = hfSource.Value.Split(',')[1].Substring(0, hfSource.Value.Split(',')[1].Length - 1);

                sourceLat = Convert.ToDouble(a);
                sourceLng = Convert.ToDouble(b);
                source = a + "," + b; 

            }
        }
        else
        {
            hfSource.Value = "37.793555,-122.421438";
            sourceLat = 37.793555;
            sourceLng = -122.421438;
        }

        int countOfRows = 0;

        bllObject.deleteSourceIntersectionDistances();

        /* This step computes the distance from the Initial source location to the 40 intersections 
         and stores the computed distance in the database */

        while (countOfRows < ds.Tables[0].Rows.Count)
        {
            double dist = distance(sourceLat, sourceLng, Convert.ToDouble(ds.Tables[0].Rows[countOfRows][1]), Convert.ToDouble(ds.Tables[0].Rows[countOfRows][2]), 'N');
            string name = ds.Tables[0].Rows[countOfRows][3].ToString();
            bllObject.insertDistance(Convert.ToDecimal(ds.Tables[0].Rows[countOfRows][1]), Convert.ToDecimal(ds.Tables[0].Rows[countOfRows][2]), dist, name + ",San Francisco,CA", 0);
            countOfRows++;

        }



        countOfRows = 0;
        //source = hfSource.Value;
        DataSet dsIntersections = new DataSet();
        dsIntersections = bllObject.getIntersectionsWithDistances();

        flag = 1;

        // Since the user is not currently at an intersection, the source of user is set as the nearest intersection
        hfSource.Value = dsIntersections.Tables[0].Rows[0][0] + "," + dsIntersections.Tables[0].Rows[0][1];
        double initialLat = Convert.ToDouble(dsIntersections.Tables[0].Rows[0][0]);
        double initialLng = Convert.ToDouble(dsIntersections.Tables[0].Rows[0][1]);

        ArrayList visitedNeighbors = new ArrayList();
        string datetime = "";
        if (hfTime.Value == "")
        {
            dateTime1 = Convert.ToDateTime("2012-04-08 17:38:00"); // between 7001 and 7002
        }
        else
        {
            dateTime1 = Convert.ToDateTime(hfTime.Value);
        }

        int firstRow = 0;
        string previousNeighbor = "";
        lblStartTime.Text = dateTime1.ToString();
        int countOfSlots = 0;
        int count = 0;

        /* The below loop executes until the user finds a parking */

        while (true)
        {
            /*if(firstRow == 0)
            {
                time = 0;
                firstRow++;
            }
            else
            {
                time = Convert.ToInt32(hfTime.Value);
            }
            */
            dateTime1 = dateTime1.AddMinutes(timeVal);

            int neighborCount = 0;

            DateTime addedTimeTemp = dateTime1.AddMinutes(15);
            string addedTime = string.Format("{0:u}", addedTimeTemp);
            addedTime = addedTime.Substring(0, addedTime.Length - 1);

            datetime = string.Format("{0:u}", dateTime1);
            datetime = datetime.Substring(0, datetime.Length - 1);


            DataSet dsNeighbors = new DataSet();
            dsNeighbors = bllObject.getNeighbors(Convert.ToDecimal(hfSource.Value.Split(',')[0]), Convert.ToDecimal(hfSource.Value.Split(',')[1]));
            bllObject.deleteSourceIntersectionDistances();
            if (dsNeighbors.Tables[0].Rows.Count > 0)
            {

                while (neighborCount < dsNeighbors.Tables[0].Rows.Count)
                {
                    double dist = distance(sourceLat, sourceLng, Convert.ToDouble(dsNeighbors.Tables[0].Rows[neighborCount][1]), Convert.ToDouble(dsNeighbors.Tables[0].Rows[neighborCount][2]), 'N');
                    string name = "";
                    int nodeId = Convert.ToInt32(dsNeighbors.Tables[0].Rows[neighborCount][0]);
                    bllObject.insertDistance(Convert.ToDecimal(dsNeighbors.Tables[0].Rows[neighborCount][1]), Convert.ToDecimal(dsNeighbors.Tables[0].Rows[neighborCount][2]), dist, name, nodeId);
                    neighborCount++;
                }

                DataSet dsDistances = new DataSet();
                dsDistances = bllObject.getIntersectionsWithDistances();
                neighborCount = 0;
                if (dsDistances.Tables[0].Rows.Count > 0)
                {

                    while (neighborCount < dsDistances.Tables[0].Rows.Count)
                    {
                        string latlng = dsDistances.Tables[0].Rows[neighborCount][0] + "," + dsDistances.Tables[0].Rows[neighborCount][1];
                        if (!visitedNeighbors.Contains(dsDistances.Tables[0].Rows[neighborCount][4]) && latlng != previousNeighbor)
                        {
                            hfDestination.Value = dsDistances.Tables[0].Rows[neighborCount][0].ToString() + "," + dsDistances.Tables[0].Rows[neighborCount][1].ToString();
                            visitedNeighbors.Add(dsDistances.Tables[0].Rows[neighborCount][4]);
                            break;
                        }
                        neighborCount++;
                    }
                }


                sourceIntersectionLat = Convert.ToDecimal(hfSource.Value.Split(',')[0]);
                sourceIntersectionLng = Convert.ToDecimal(hfSource.Value.Split(',')[1]);

                destIntersectionLat = Convert.ToDecimal(hfDestination.Value.Split(',')[0]);
                destIntersectionLng = Convert.ToDecimal(hfDestination.Value.Split(',')[1]);
                int congestionPercent = 0;
                if (hfCongestionPercent.Value == "")
                {
                    congestionPercent = 0;
                }
                else
                {
                    congestionPercent = Convert.ToInt32(hfCongestionPercent.Value);
                }

                /* The below step checks if there are any operational blocks between the intersections under consideration
                 * for various levels of congestion
                * If parking is available, the method returns the block id of the operational block, else it returns -1 */
                
                string returnValue = bllObject.checkOperationalWithCongestion(sourceIntersectionLat, sourceIntersectionLng, destIntersectionLat, destIntersectionLng, datetime, addedTime,congestionPercent);
                

                if (Convert.ToInt32(returnValue) > 0)
                {
                    hfReturnValue.Value = "1";
                    DataSet dsBlockLocation = new DataSet();
                    dsBlockLocation = bllObject.findBlockStartLocation(Convert.ToInt32(returnValue));
                    if (dsBlockLocation.Tables[0].Rows.Count > 0)
                    {
                        hfDestination.Value = dsBlockLocation.Tables[0].Rows[0][0] + "," + dsBlockLocation.Tables[0].Rows[0][1];
                    }
                    hfSource.Value = source;
                    // The below call to javascript function displays teh final route taken by the user
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction" + count, "calcRoute('" + hfSource.Value + "','" + hfDestination.Value + "')", true);
                    count++;
                    lblEndTime.Text = dateTime1.ToString();
                    break;
                }
                else
                {
                    if (countOfSlots == 15)
                    {
                        hfSource.Value = source;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction" + count, "calcRoute('" + hfSource.Value + "','" + hfDestination.Value + "')", true);
                        count++;
                        lblEndTime.Text = dateTime1.ToString();
                        break;
                    }
                    else
                    {
                        double dist = distance(Convert.ToDouble(hfSource.Value.Split(',')[0].Substring(0, hfSource.Value.Split(',')[0].Length)), Convert.ToDouble(hfSource.Value.Split(',')[1].Substring(0, hfSource.Value.Split(',')[1].Length)), Convert.ToDouble(hfDestination.Value.Split(',')[0].Substring(0, hfDestination.Value.Split(',')[0].Length)), Convert.ToDouble(hfDestination.Value.Split(',')[1].Substring(0, hfDestination.Value.Split(',')[1].Length)), 'N');
                        int speed = 20;
                        timeVal = Convert.ToInt32(Math.Ceiling((dist / speed) * 60));

                        /* The visitedNeighbors arraylist is cleared each time the user has crossed 5 intersections so that 
                        * the user is given the chance to visit the same nodes in case he/she doesn't find a parking */

                        if (visitedNeighbors.Count > 5)
                        {
                            visitedNeighbors.Clear();
                        }
                        previousNeighbor = hfSource.Value;
                        hfSource.Value = hfDestination.Value;
                    }
                    
                }
            }
            else
            {
            }
            countOfSlots++;
        }


    }

    /* The below method gets the distance between two locations from the Google Directions api */
    public static int GetDrivingDistanceInMiles(string origin, string destination)
    {

        string url = @"https://maps.googleapis.com/maps/api/directions/xml?origin=" +
          origin + "&destination=" + destination +
          "&mode=driving&sensor=false&language=en-EN&key=AIzaSyCyeTlQl9EPLPhd-yxioswbnGoM9uh-jQ4";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        WebResponse response = request.GetResponse();
        Stream dataStream = response.GetResponseStream();
        StreamReader sreader = new StreamReader(dataStream);
        string responsereader = sreader.ReadToEnd();
        response.Close();

        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(responsereader);

        if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
        {
            XmlNodeList time = xmldoc.GetElementsByTagName("duration");
            return Convert.ToInt32(time[0].ChildNodes[1].InnerXml.Replace(" min", "")); //
            //return Convert.ToInt32(time[0].ChildNodes[1].InnerXml);
        }
        else if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OVER_QUERY_LIMIT")
        {
            Thread.Sleep(250);
            GetDrivingDistanceInMiles(origin, destination);
        }

        return 0;
    }


    /*
     * This method is used to display any messages
     */
    private void MessageBox(string msg)
    {
        Label lbl = new Label();
        lbl.Text = "<script language='javascript'>" + Environment.NewLine + "window.alert('" + msg + "')</script>";
        Page.Controls.Add(lbl);
    }

    private double deg2rad(double deg)
    {
        return (deg * Math.PI / 180.0);
    }


    /* This function converts radians to decimal degrees */

    private double rad2deg(double rad)
    {
        return (rad / Math.PI * 180.0);
    }

    /*
     * This method calculates the distance between a source and destination
     * 
     */
    private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
        dist = Math.Acos(dist);
        dist = rad2deg(dist);
        dist = dist * 60 * 1.1515;
        if (unit == 'K')
        {
            dist = dist * 1.609344;
        }
        else if (unit == 'N')
        {
            dist = dist * 0.8684;
        }
        return (dist);

    }
    protected void btnSample_Click(object sender, EventArgs e)
    {
        getValues();
    }
    protected void btnSample_Click1(object sender, EventArgs e)
    {
        getValues();
    }
    protected void btnTestData_Click(object sender, EventArgs e)
    {
        getDataFromExcel();
        Response.Redirect("TestResultsUninformedSearchWithCongestion.aspx");
    }

    /* The below code is for TEST EXECUTION */

    void getDataFromExcel()
    {
        // Test file is TestData.xls
        string excel = Server.MapPath("~/TestData.xls;");
        string con = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excel +
                     @"Extended Properties='Excel 8.0;HDR=Yes;'";
        bllObject.deleteTestData("WITH");
        int countv = 0;
        using (OleDbConnection connection = new OleDbConnection(con))
        {
            connection.Open();
            OleDbCommand command = new OleDbCommand("select * from [TestDataWithCongestion$]", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    string sourceLocation = dr[1].ToString() + "," + dr[2].ToString();
                    hfSource.Value = sourceLocation;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    hfTime.Value = dr[3].ToString();
                    hfCongestionPercent.Value = dr[4].ToString();
                    getValues();
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    lblAlgoTime.Text = ts.Minutes.ToString();
                    // Format and display the TimeSpan value.
                    string time = dateTime1.ToString();
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    double travelDist = distance(Convert.ToDouble(dr[1]), Convert.ToDouble(dr[2]), Convert.ToDouble(hfDestination.Value.Split(',')[0]), Convert.ToDouble(hfDestination.Value.Split(',')[1]), 'N');
                    string travel = travelDist.ToString("0.00");
                    travelDist = Convert.ToDouble(travel);
                    double travelTime = Convert.ToDouble(((travelDist / 3) * 60).ToString("0.00"));
                    bllObject.insertTestDataUninformedWithCongestion(Convert.ToDecimal(dr[1]), Convert.ToDecimal(dr[2]), Convert.ToDecimal(hfDestination.Value.Split(',')[0]), Convert.ToDecimal(hfDestination.Value.Split(',')[1]), dr[3].ToString(), time, Convert.ToInt32(hfCongestionPercent.Value),travelDist,travelTime);
                    countv++;
                    
                }
            }
        }
    }
}