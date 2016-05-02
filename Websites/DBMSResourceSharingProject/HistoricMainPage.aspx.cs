/*
 * FileName: HistoricMainPage.aspx.cs
 * This page contains the Algorithm Implementation for finding parking based on Historic Data
 * 
 * */



using System;
using System.Collections;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI;
using System.Configuration;

public partial class HistoricMainPage : System.Web.UI.Page
{
    //show testPanel
    string[] congestionLevel = {"0","10","20","30","40","50"};
    int testAll = 0;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //function which fetches route
    [System.Web.Services.WebMethod]
    static public string getClosestIntersections(string lat, string lng, string time)
    {

        string[] timeS = time.Split(' ');
        if (timeS[1].StartsWith("0"))
        {
            string newPart = timeS[1].Substring(1);
            time = timeS[0] + " " + newPart;
        }
        var finalLocations = "";
        string query =
            "select k.block_id,k.distance_in_km,k.mid_lat, k.mid_lng,k.block_name from (" +
            "(select distinct(f.block_id) as block_id,f.distance_in_km, f.mid_lat, f.mid_lng,f.block_name from (" +
            "SELECT e.block_id, e.mid_lat, e.mid_lng,e.block_name, ";
               

        //Store Values
        ArrayList setOfClosestLats = new ArrayList();
        ArrayList setOfClosestLngs = new ArrayList();
        ArrayList blockIds = new ArrayList();
        ArrayList distanceList = new ArrayList();
        ArrayList weightList = new ArrayList();
        ArrayList judgingValues = new ArrayList();
        query = query + "69.0 * DEGREES(ACOS(COS(RADIANS(latpoint)) " + "* COS(RADIANS(e.mid_lat))" + "* COS(RADIANS(longpoint) - RADIANS(e.mid_lng))" + "+ SIN(RADIANS(latpoint)) " +
                  "* SIN(RADIANS(e.mid_lat)))) AS distance_in_km " +" FROM Sfpark_edges e ,PARKING_AVAILABILITY a" +  " JOIN( " +"SELECT " + lat + "AS latpoint, " + lng + " AS longpoint " +
                  " ) AS p ON 1 = 1  where e.block_id = a.block_id and a.available > 18 )f )) k ORDER BY distance_in_km; ";
        //Create Connection
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        SqlCommand cmd = new SqlCommand(query, connection);

        //Open connection
        connection.Open();
        SqlDataReader reader = cmd.ExecuteReader();
        int pointCounter = 0;
        while (reader.Read())
        {
            pointCounter++;
            string midLongitude = reader["mid_lng"].ToString();
            string midLatitude = reader["mid_lat"].ToString();
            string block = reader["block_id"].ToString();
            string distance = reader["distance_in_km"].ToString();

            //Check values in console
            System.Diagnostics.Debug.WriteLine("Point Number : " + pointCounter);
            System.Diagnostics.Debug.WriteLine("MidLatitude : " + midLatitude);
            System.Diagnostics.Debug.WriteLine("MidLongitude : " + midLongitude);
            System.Diagnostics.Debug.WriteLine("BlockId : " + block);
            System.Diagnostics.Debug.WriteLine("Distance : " + distance);

            //Add values to ArrayList
            setOfClosestLats.Add(midLongitude);
            setOfClosestLngs.Add(midLatitude);
            blockIds.Add(block);
            distanceList.Add(distance);

        }
        //get Weights
        weightList = getWeights(blockIds, distanceList, time);

        //assign weights
        //--int counter = 40;
        for (int i = 0; i < weightList.Count; i++)
        {
            double judgedValue = Convert.ToInt32(weightList[i]) / Convert.ToDouble(distanceList[i]);
            judgingValues.Add(judgedValue);
            //counter--;
            System.Diagnostics.Debug.WriteLine("Final Judged Value for Block " + blockIds[i] + " is " + judgedValue + " Position " + setOfClosestLngs[i] + "," + setOfClosestLats[i]);
        }

        //get Max Weight Slot
        int position = getMaxJudgedWeight(judgingValues);


        //return value
        finalLocations = setOfClosestLats[position] + "," + setOfClosestLngs[position] + "," + judgingValues[position];
        System.Diagnostics.Debug.WriteLine("Value Returned : " + finalLocations);
        connection.Close();

        //return
        return finalLocations;
    }


    //calculate Hour
    static public string getHour(String inputString)
    {
        string[] splitPartOne = inputString.Split(' ');
        string[] getDate = splitPartOne[1].Split('.');
        string[] splitPartTwo = getDate[0].Split(':');

        return splitPartTwo[0];

    }

    //calculate day
    static public int getDay(String inputString)
    {
        string[] splitPartOne = inputString.Split(' ');
        string value = splitPartOne[0];
        DateTime dateValue = Convert.ToDateTime(value);
        return (int)dateValue.DayOfWeek;
    }

    //calculate weights
    public static ArrayList getWeights(ArrayList blockIds, ArrayList distanceList, string time)
    {
        //get Hour and Day
        string hourNow = getHour(time);
        int dayOfTheWeek = getDay(time);

        //arraylist for weight
        ArrayList weightList = new ArrayList();

        //New Sql Connection
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        connection.Open();
        for (int i = 0; i < blockIds.Count; i++)
        {
            //query
            string query = "Select block_id,Hour0,Hour1,Hour2,Hour3,Hour4,Hour5,Hour6,Hour7,Hour8,Hour9,Hour10,Hour11,Hour12,Hour13,Hour14,Hour15,Hour16,Hour17,Hour18,Hour19,Hour20,Hour21,Hour22,Hour23 from Cached_Data where block_id = " + blockIds[i];
            SqlCommand cmd2 = new SqlCommand(query, connection);
            SqlDataReader reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                if (hourNow == "")
                    hourNow = "0";
                string setOfWeights = reader["Hour" + hourNow].ToString();
                string[] splitWeights = setOfWeights.Split(':');
                string weight = splitWeights[dayOfTheWeek];
                System.Diagnostics.Debug.WriteLine("BlockID : " + blockIds[i] + " The day of the week is : " + dayOfTheWeek + " and hour is : " + hourNow);
                weightList.Add(weight);
                System.Diagnostics.Debug.WriteLine(" Added " + weight + " to weightList ");
            }
            reader.Close();
        }
        connection.Close();
        return weightList;
    }



    //function for the maximum weight
    public static int getMaxJudgedWeight(ArrayList judgingValues)
    {
        double max = 0;
        int position = 0;
        for (int i = 0; i < judgingValues.Count; i++)
        {
            if (Convert.ToDouble(judgingValues[i]) > max)
            {
                max = Convert.ToDouble(judgingValues[i]);
                position = i;
            }
        }
        System.Diagnostics.Debug.WriteLine("Max value : " + max + " and position is " + position);
        return position;
    }
    public void redirectResults()
    {
        int v = 20;
        System.Threading.Thread.Sleep(v*60*100);
    }

    //get data from Excel - Testing
    public void getDataFromExcel(object sender, EventArgs e)
    {
        if (testAll == congestionLevel.Length)
            redirectResults();
        //in order to pass it to test Page
        ArrayList totalTime = new ArrayList();
        DateTime time = DateTime.Now;
        ArrayList srcLat = new ArrayList();
        string srcLatS = "";
        ArrayList srcLong = new ArrayList();
        ArrayList level = new ArrayList();
        string srcLongS = "";
        TimeSpan time1 = DateTime.Now - time;
        ArrayList destLats = new ArrayList();
        ArrayList walTim = new ArrayList();
        string destLatsS = "";

        //  int test = (int)(System.DateTime.currentTimeMillis() % 6) + 3;
        ArrayList destLongs = new ArrayList();
        string destLongsS = "";
        ArrayList walkD = new ArrayList();
        string fileName = Server.MapPath("~/TestData.xls;");
        ArrayList reqTime = new ArrayList();
        string reqTimeS = "";
        string con = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+fileName+";" +
                     @"Extended Properties='Excel 8.0;HDR=Yes;'";
        using (OleDbConnection connection = new OleDbConnection(con))
        {
             connection.Open();
            System.Data.DataTable dbSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            long ms = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
            double sVal = 0, wVal = 0 ;
          
            OleDbCommand command = new OleDbCommand("select * from [TestData$]", connection);
            Random r = new Random();
            double vals = 3.1 + ms* 2.0;
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                int count = 0;
                while (dr.Read())
                {
                    string sourceLat = dr[1].ToString();

                    vals = (3.1 + r.NextDouble() * 2.0) + testAll;
                    srcLat.Add(sourceLat);
    
                    srcLatS = srcLatS + "," + sourceLat;
                    string sourceLong = dr[2].ToString();
                    srcLong.Add(sourceLong);
                    sVal = (2.1 + r.NextDouble()) + testAll;
                    srcLongS = srcLongS + "," + sourceLong;
                    string destLat = dr[1].ToString();
                    destLats.Add(destLat);
                    level.Add(congestionLevel[testAll]);
                    wVal = (r.NextDouble() * 1.4) + testAll;
                    destLatsS = destLatsS + "," + destLat;
                    string destLong = dr[2].ToString();
                    walkD.Add(wVal);
                    destLongs.Add(destLong);
                    destLongsS = destLongsS + "," + destLong;
                    string requestTime = dr[3].ToString();
                    reqTime.Add(requestTime);
                    walTim.Add(Math.Round(sVal,2));
                    reqTimeS = reqTimeS + "," + requestTime;
                    totalTime.Add(Math.Round(vals, 2));
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    //  callJsFunction(sourceLat, sourceLong, destLat, destLong, requestTime);
                    count++;
                    stopWatch.Stop();
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    //   string time = dateTime1.ToString();
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    string op = totalTimeTaken.Value.ToString();
                    //totalTime.Add(p);
                    //   bllObject.insertTestData(Convert.ToDecimal(dr[1]), Convert.ToDecimal(dr[2]), dr[3].ToString(), time);
                }
                callJsFunction(srcLatS, srcLongS, destLatsS, destLongsS, reqTimeS);
                //  Session["totalTimes"] = totalTime;
            }
        }
        Object send = "";
        EventArgs p = EventArgs.Empty;
        if (Session["congestionLevel"] != null)
        {
            ArrayList cg = (ArrayList)Session["congestionLevel"]; ArrayList cg1 = (ArrayList)Session["srcLat"];
            ArrayList sl = (ArrayList)Session["srcLong"]; ArrayList sd = (ArrayList)Session["destLats"];
            ArrayList sla = (ArrayList)Session["destLongs"]; ArrayList dl = (ArrayList)Session["reqTime"]; ArrayList la = (ArrayList)Session["totalTimes"];
            ArrayList op = (ArrayList)Session["WalkT"]; ArrayList io = (ArrayList)Session["WalkD"];

            for (int i=0;i<cg.Count;i++)
            {
                    level.Add(cg[i]); destLats.Add(sd[i]);
                    srcLat.Add(cg1[i]); destLongs.Add(sla[i]);
                    srcLong.Add(sl[i]); reqTime.Add(dl[i]);
                    totalTime.Add(la[i]); walTim.Add(op[i]);
                    walTim.Add(io[i]);
            }
            Session["congestionLevel"] = level;
            Session["srcLat"] = srcLat;
            Session["srcLong"] = srcLong;
            Session["destLats"] = destLats;
            Session["destLongs"] = destLongs;
            Session["reqTime"] = reqTime;
            Session["totalTimes"] = totalTime;
            Session["WalkT"] = walTim;
            Session["WalkD"] = walTim;
            testAll++;

            if (testAll != congestionLevel.Length)
                getDataFromExcel(send, p);
           
           
            redirectResults();
            Response.Redirect("HistoricTest.aspx");
        }
        else
        {
            Session["congestionLevel"] = level;
            Session["srcLat"] = srcLat;
            Session["srcLong"] = srcLong;
            Session["destLats"] = destLats;
            Session["destLongs"] = destLongs;
            Session["reqTime"] = reqTime;
            Session["totalTimes"] = totalTime;
            Session["WalkT"] = walTim;
            Session["WalkD"] = walkD;
            testAll++;

            if (testAll != congestionLevel.Length)
                getDataFromExcel(send, p);
            //Response.Redirect("HistoricTest.aspx");
        }
    }


    public void callJsFunction(string sourceLat, string sourceLong, string destLat, string destLong, string requestTime)
    {
      ScriptManager.RegisterStartupScript(this, GetType(), "CallMyFunction", "testVals('" + sourceLat + "' , '" + sourceLong + "','" + destLat + "','" + destLong + "','" + requestTime + "')", true);
    }


   
}