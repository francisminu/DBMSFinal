/*
 * FileName: RealTimeWithCongestion.aspx.cs
 * This page contains the Algorithm Implementation for finding parking based on Real Time Data considering Congestion
 * 
 * */


using System;
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
//using System.JSon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using System.Collections;
using DBMSResourceSharingProjectBLL;
using System.Threading;
using System.Net;
using System.Xml;
using System.IO;
using System.Data.OleDb;
using System.Diagnostics;


public partial class RealTimeWithCongestion : System.Web.UI.Page
{
     string times = "";
    Dictionary<string, string> int_dis ;
    ResourceSharingBLL bllObject = new ResourceSharingBLL();
    double parking_time = 0;
    int congestion = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
           // findParking();
        }
        
    }
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    //---- Function to find the parking using Gravitation Pull with Congestion---------------------//
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    
    public void findParking()
    {
        try
        {
            //--- Setting different congestion level
            congestion = Convert.ToInt32(hfCongestion.Value);
            if(congestion == 10)
            {
                congestion = -2;
            }
            else if (congestion == 20)
            {
                congestion = -4;
            }
            else if (congestion == 30)
            {
                congestion = -6;
            }
            else if (congestion == 40)
            {
                congestion = -8;
            }
            else if (congestion == 50)
            {
                congestion = -10;
            }
            
            //find nearest intersection

            DataSet ds = new DataSet();
            
            ds = bllObject.getIntersections(0, 0, 0);
            bllObject.deleteSourceIntersectionDistances();
            int countOfIntersection = 0;

            string aLat = hfSource.Value.Split(',')[0];
            string bLng = hfSource.Value.Split(',')[1];
            double sourceLat;
            double sourceLng;
            sourceLat = Convert.ToDouble(aLat);
            sourceLng = Convert.ToDouble(bLng);
            
            while (countOfIntersection < ds.Tables[0].Rows.Count)
            {
                double dist = distance(sourceLat, sourceLng, Convert.ToDouble(ds.Tables[0].Rows[countOfIntersection][1]), Convert.ToDouble(ds.Tables[0].Rows[countOfIntersection][2]), 'N');
                string name = ds.Tables[0].Rows[countOfIntersection][3].ToString();
                bllObject.insertDistance(Convert.ToDecimal(ds.Tables[0].Rows[countOfIntersection][1]), Convert.ToDecimal(ds.Tables[0].Rows[countOfIntersection][2]), dist, name + ",San Francisco,CA", 0);
                countOfIntersection++;

            }

            countOfIntersection = 0;
            //source = hfSource.Value;
            DataSet dsIntersections = new DataSet();
            dsIntersections = bllObject.getIntersectionsWithDistances();
            hfSource.Value = dsIntersections.Tables[0].Rows[0][0] + "," + dsIntersections.Tables[0].Rows[0][1];

            string[] ts = { "2012-04-05 10:08:00", "2012-04-05 00:14:00", "2012-04-06 00:16:00", "2012-04-06 00:18:00", "2012-04-06 00:20:00", "2012-04-06 00:22:00" };

            string source_id = bllObject.getNodeId(Convert.ToDecimal(hfSource.Value.Split(',')[0]), Convert.ToDecimal(hfSource.Value.Split(',')[1]));


            DateTime datetime2;
            datetime2 = Convert.ToDateTime(hfTime.Value); 
            
            times = string.Format("{0:u}", datetime2);
            times = times.Substring(0, times.Length - 2);
            
            

            //parking flag if parking found flag will be set to True
            bool flag = false;
            int int_trav = 1;
            int_dis = get_intersection_distance();
            int numberoftimes = 20;
            
            while (flag == false && int_trav <= numberoftimes)
            {
                ArrayList list = new ArrayList();
                list.Add(source_id);
                System.Diagnostics.Debug.Write("\n" + "start  " + "\n");

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                string sqlQuery = "SELECT * FROM neighbors_new where block_id = " + source_id;
                cmd.CommandText = sqlQuery;
                da.SelectCommand = cmd;
                ds = new DataSet();

                conn.Open();
                da.Fill(ds);

                int countOfRows = 0;
                while (countOfRows < ds.Tables[0].Rows.Count)
                {
                    //string source_id = ds.Tables[0].Rows[countOfRows][0].ToString();
                    System.Diagnostics.Debug.Write("\n" + "source id" + source_id + "\n");
                    // Console.WriteLine(source_id);
                    //System.Diagnostics.Debug.Write(source_id);
                    string val = ds.Tables[0].Rows[countOfRows][2].ToString();
                    dynamic stuff = JsonConvert.DeserializeObject(val);
                    System.Diagnostics.Debug.Write("Hello");
                    // string key1 = stuff.;
                    String val1 = "[" + val + "]";
                    string json = @val1;

                    JArray a = JArray.Parse(json);
                    System.Diagnostics.Debug.Write("array a");
                    Dictionary<string, double> dictionary = new Dictionary<string, double>();
                    foreach (JObject o in a.Children<JObject>())
                    {
                        System.Diagnostics.Debug.Write("before property ");
                        foreach (JProperty p in o.Properties())
                        {
                            System.Diagnostics.Debug.Write("before");
                            string intersection1 = p.Name;
                            double dist = (double)p.Value;
                            //  Console.WriteLine(name + " -- " + value);
                            System.Diagnostics.Debug.Write(intersection1 + " -- " + dist + "\n");

                            string s4 = source_id;
                            string current_intersection = intersection1;
                            list.Add(current_intersection);
                            int neibr = 1;
                            double avialbilty = congestion;
                            int[] blockids = get_block_id(s4, current_intersection);
                            // for first level neighbour
                            for (int j = 0; j < blockids.Length; j++)
                            {
                                string block_id = blockids[j].ToString();
                                // int dist = (int)
                                avialbilty += get_availability(block_id, times);
                            }
                            
                            hfNodeId.Value = intersection1;
                            //---- if at current position parking is available 
                            if (avialbilty > 0)
                            {
                                System.Diagnostics.Debug.Write("Parking found at  " + intersection1 + "availabilty" + avialbilty + "\n");
                                
                                String s3 = intersection1.Substring(0, 4);
                                string flev = int_dis[s3];

                                var jss = new JavaScriptSerializer();
                                var dict = jss.Deserialize<Dictionary<string, string>>(flev);

                                String s5 = source_id;
                                string travel_dist = dict[s5];
                                // System.Diagnostics.Debug.Write("distance between curent id " + source_id + " and intersection with highest gp " + travel_dist + "\n");
                                double travel_time = Convert.ToDouble(travel_dist) / 20;

                                parking_time = parking_time + travel_time;
                                parking_time = parking_time * 60;
                                System.Diagnostics.Debug.Write("total time to search a parking in minutes = " + parking_time + "\n");

                                flag = true;
                                break;
                            }
                            else
                            {
                                // gravitational pull = availabilty/ driving time + walking time
                                // average driving speed is 20 miles per hour
                                double driving_time = dist / 20;
                                // average  walking time is 3 miles per hours
                                double walking_time = dist / 3;
                                double cost = driving_time + walking_time;
                                double gp = avialbilty / Math.Pow(cost, 2);
                                double first_lev_available = avialbilty;
                                // function to get gravitation pull from all the intersection via current neighbor 
                                
                                gp_cal(current_intersection, dist, source_id, times);

                                dictionary.Add(intersection1, gp);

                            }


                        }

                    }
                    //--- If parking is not found then calculate gravitational pull again
                    if (flag == false)
                    {
                        string s2 = dictionary.FirstOrDefault(x => x.Value == dictionary.Values.Max()).Key;
                        System.Diagnostics.Debug.Write("max intersection id " + s2 + " available " + dictionary[s2] + "\n");
                        double current_available = dictionary[s2];
                        s2 = s2.Substring(0, 4);
                        // travel time to highest gp intersection

                        string flev = int_dis[s2];

                        var jss = new JavaScriptSerializer();
                        var dict = jss.Deserialize<Dictionary<string, string>>(flev);

                        String s1 = source_id;
                        string travel_dist = dict[s1];
                        // System.Diagnostics.Debug.Write("distance between curent id " + source_id + " and intersection with highest gp " + travel_dist + "\n");
                        double travel_time = Convert.ToDouble(travel_dist) / 20;
                        parking_time = parking_time + travel_time;
                        //--- source id will be replaced to current location
                        source_id = s2;

                        //---- Timestamp will be change to current updated time
                        int timeVal = Convert.ToInt32(Math.Ceiling(travel_time * 60));

                        DateTime datetime1;
                        datetime1 = Convert.ToDateTime(times); 
                        datetime1 = datetime1.AddMinutes(timeVal);

                        times = string.Format("{0:u}", datetime1);
                        times = times.Substring(0, times.Length - 1);

                    }


                    countOfRows++;

                }
                int_trav++;
                conn.Close();
            }
        }
        catch (Exception ex)
        {

        }
    }
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    //----- Funtion call the gravitation pull from all intersection recursively :::://
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    private double  gp_cal(String current_intersection,double dist,String source_id,String times)
    {
        String s4 = source_id;
        double gp = 0;
    
            string intersection2 = current_intersection.Substring(0, 4);
            string flev = int_dis[intersection2];
            if (flev.Length == 0)
            {
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<Dictionary<string, string>>(flev);
                foreach (KeyValuePair<string, string> entry in dict)
                {
                    // do something with entry.Value or entry.Key

                    System.Diagnostics.Debug.Write(entry.Key + " -- " + entry.Value + "\n");
                    s4 = intersection2;
                    current_intersection = entry.Key;
                    double dist1 = dist + Convert.ToDouble(entry.Value);
                    // dist = (double)p.Value;
                    if (current_intersection != source_id)
                    {
                        int[] blockids = get_block_id(s4, current_intersection);
                        int avialbilty = congestion;
                        for (int j = 0; j < blockids.Length; j++)
                        {
                            string block_id = blockids[j].ToString();
                            // int dist = (int)
                            avialbilty += get_availability(block_id, times);
                        }
                        //----- gravitational pull = availabilty/ driving time + walking time
                        //----- avg driving speed 20 miles per hour
                        double driving_time = dist1/20;
                        //----- avg walking time 3 miles per hours
                        double walking_time = dist1/3;
                        //----- cost is driving time of vehicle to reach that parking slot and walking time to travel back
                        double cost = driving_time + walking_time;
                        gp = gp_cal(current_intersection,dist1,source_id,times)+ (avialbilty / Math.Pow(cost, 2));
                    }
                }
            }
            return gp;
    }
     //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    //---- Get id of the blocks between given two intersection ids ::::::::::::::::://
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    private int[] get_block_id(string node1, string node2)
    {
        
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        SqlDataAdapter da1 = new SqlDataAdapter();
        SqlCommand cmd = conn.CreateCommand();
        string sqlQuery = "SELECT block_id FROM sfpark_edges where node_id_1 =" + node1 + " and node_id_2 =" + node2 ;
        cmd.CommandText = sqlQuery;
        da1.SelectCommand = cmd;
        DataSet ds1 = new DataSet();

        conn.Open();
        da1.Fill(ds1);

        int countOfRows = 0;
        int numberofblocks = ds1.Tables[0].Rows.Count;
        int []block_id = new int[numberofblocks];
        while (countOfRows < ds1.Tables[0].Rows.Count)
        {
            block_id[countOfRows] = (int)ds1.Tables[0].Rows[countOfRows][0];
            countOfRows++;

        }


        conn.Close();
        
       //--- return the list of block ids between present between two intersection
        return block_id;
    }
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    //--- get neghbors and their corresponding distance from all the neighbors using precomputed table "neighbors"//
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    private Dictionary<string, string> get_intersection_distance()
    {
        System.Diagnostics.Debug.Write("\n" + "start  " + "\n");

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmd = conn.CreateCommand();
        string sqlQuery = "SELECT * FROM neighbors_new ";
        cmd.CommandText = sqlQuery;
        da.SelectCommand = cmd;
        DataSet ds = new DataSet();

        conn.Open();
        da.Fill(ds);
        int countOfRows = 0;
        Dictionary<string, string> dict = new Dictionary<string, string>();
            
        while (countOfRows < ds.Tables[0].Rows.Count)
        {
            System.Diagnostics.Debug.Write("\n" + countOfRows + "\n");
            string iid = ds.Tables[0].Rows[countOfRows][0].ToString();
            string flevl = ds.Tables[0].Rows[countOfRows][2].ToString();
            dict.Add(iid, flevl);

            countOfRows++;

        }


        conn.Close();
        return dict;
        
    }
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    // Function to get availabilty of a particular block id at particular timestamp //
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
    private int get_availability(string block_id, string times)
    {
        System.Diagnostics.Debug.Write("\n" + "node1 and node2 " + block_id + " = " + times + "\n");

        int availabilty = congestion;
        
        System.Diagnostics.Debug.Write("\n" + " check" + block_id + "\n");

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        SqlDataAdapter da1 = new SqlDataAdapter();
        SqlCommand cmd = conn.CreateCommand();
        string sqlQuery = "SELECT available FROM PARKING_AVAILABILITY where block_id =" + block_id + " and time_stamp ='" + times + "'";
        System.Diagnostics.Debug.Write("\n" + "query" + sqlQuery+ "\n");
        cmd.CommandText = sqlQuery;
        da1.SelectCommand = cmd;
        DataSet ds1 = new DataSet();

        conn.Open();
        da1.Fill(ds1);

        int countOfRows = 0;
        while (countOfRows < ds1.Tables[0].Rows.Count)
        {
            System.Diagnostics.Debug.Write("\n" + countOfRows + "\n");
            availabilty= (int)ds1.Tables[0].Rows[countOfRows][0];
            System.Diagnostics.Debug.Write("\n" + "availability" + availabilty+ "\n");
            countOfRows++;

        }


        conn.Close();
        return availabilty;
    }

    private double deg2rad(double deg)
    {
        return (deg * Math.PI / 180.0);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts radians to decimal degrees             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double rad2deg(double rad)
    {
        return (rad / Math.PI * 180.0);
    }
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
    public class LeagueUser
    {
        public int id { get; set; }
        public string name { get; set; }
        public long revisionDate { get; set; }
    }
    class MyConverter : CustomCreationConverter<IDictionary<string, object>>
    {
        public override IDictionary<string, object> Create(Type objectType)
        {
            return new Dictionary<string, object>();
        }

        public override bool CanConvert(Type objectType)
        {
            // in addition to handling IDictionary<string, object>
            // we want to handle the deserialization of dict value
            // which is of type object
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject
                || reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);

            // if the next token is not an object
            // then fall back on standard deserializer (strings, numbers etc.)
            return serializer.Deserialize(reader);
        }
    }





    protected void btnTestData_Click(object sender, EventArgs e)
    {
        getDataFromExcel();
        Response.Redirect("TestResultsUninformedSearchWithCongestion.aspx");
    }

    void getDataFromExcel()
    {
        string excel = Server.MapPath("~/TestData.xls;");
        string con = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excel +
                     @"Extended Properties='Excel 8.0;HDR=Yes;'";
        bllObject.deleteTestData("WITH");
        int counterBreak = 0;
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
                    findParking();
                    stopWatch.Stop();
                    hfCongestion.Value = dr[4].ToString();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    //lblAlgoTime.Text = ts.Minutes.ToString();
                    // Format and display the TimeSpan value.
                    //string time = parking_time.ToString();

                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    if (hfNodeId.Value != "")
                    {
                        DateTime datetime2;
                        datetime2 = Convert.ToDateTime(dr[3].ToString()); // between 7001 and 7002

                        string startTime = string.Format("{0:u}", datetime2);
                        startTime = startTime.Substring(0, startTime.Length - 2);
                        string destination = bllObject.getParkingLocationFromNodeId(hfNodeId.Value);
                        double travelDist = distance(Convert.ToDouble(dr[1]), Convert.ToDouble(dr[2]), Convert.ToDouble(destination.Split(',')[0]), Convert.ToDouble(destination.Split(',')[1]), 'N');
                        string travel = travelDist.ToString("0.00");
                        travelDist = Convert.ToDouble(travel);
                        double travelTime = Convert.ToDouble(((travelDist / 3) * 60).ToString("0.00"));
                        bllObject.insertTestDataUninformedWithCongestion(Convert.ToDecimal(dr[1]), Convert.ToDecimal(dr[2]), Convert.ToDecimal(destination.Split(',')[0]), Convert.ToDecimal(destination.Split(',')[1]), startTime, times,Convert.ToInt32(hfCongestion.Value),travelDist,travelTime);

                    }
                    parking_time = 0;
                    hfNodeId.Value = "";

                    counterBreak++;
                    
                }
            }
        }
    }
}

