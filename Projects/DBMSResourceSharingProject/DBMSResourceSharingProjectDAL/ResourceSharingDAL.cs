using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;


namespace DBMSResourceSharingProjectDAL
{
    public class ResourceSharingDAL
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());//new SqlConnection(ConfigurationManager.AppSettings.Get("ConnectionString"));
        DataSet ds = new DataSet();
        public DataSet getIntersections(int flag,decimal latitude, decimal longitude)
        {
            
            try
            {
                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                string sqlQuery = "";
                if(flag == 0)
                    sqlQuery = "SELECT * FROM INTERSECTIONS";
                else
                    sqlQuery = "SELECT * FROM INTERSECTIONS WHERE LATITUDE != " + latitude + "AND LONGITUDE != " + longitude;

                cmd.CommandText = sqlQuery;
                da.SelectCommand = cmd;
                
                conn.Open();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch(Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
                return null;
            }
        }

        public void deleteSourceIntersectionDistances()
        {
            try
            {
              
                SqlCommand sqlcmd = new SqlCommand("DELETE FROM SourceIntersectionDistance", conn);
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
            catch(Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
            }
            
        }

        public string getNodeId(decimal lat, decimal lng)
        {
            ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            string sqlQuery = "";
            sqlQuery = "SELECT NodeId FROM INTERSECTIONS WHERE LATITUDE = " + lat + "AND LONGITUDE = " + lng;
            cmd.CommandText = sqlQuery;
            da.SelectCommand = cmd;

            conn.Open();
            da.Fill(ds);
            conn.Close();
            string NodeId = "";
            if(ds.Tables[0].Rows.Count > 0)
            {
                NodeId = ds.Tables[0].Rows[0][0].ToString();
            }
            return NodeId;
        }

        public string getParkingLocationFromNodeId(string nodeId)
        {
            ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            string sqlQuery = "";
            sqlQuery = "SELECT Latitude,Longitude FROM INTERSECTIONS WHERE NodeId = " + nodeId;
            cmd.CommandText = sqlQuery;
            da.SelectCommand = cmd;

            conn.Open();
            da.Fill(ds);
            conn.Close();
            string destination = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                destination = ds.Tables[0].Rows[0][0].ToString() + "," + ds.Tables[0].Rows[0][1].ToString();
            }
            return destination;
        }

        public void insertDistance(decimal latitude, decimal longitude, double distance, string name, int nodeId)
        {
            try
            {
                
                SqlCommand sqlcmd = new SqlCommand("INSERT INTO SourceIntersectionDistance(Latitude,Longitude,Distance,IntersectionName,NodeId) values(@Latitude,@Longitude,@Distance,@Name,@NodeId)", conn);
                sqlcmd.Parameters.AddWithValue("@Latitude", latitude);
                sqlcmd.Parameters.AddWithValue("@Longitude", longitude);
                sqlcmd.Parameters.AddWithValue("@Distance", distance);
                sqlcmd.Parameters.AddWithValue("@Name", name + ",San Francisco,CA");
                sqlcmd.Parameters.AddWithValue("@NodeId", nodeId);
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
            catch(Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
            }
            
        }

        public DataSet getIntersectionsWithDistances()
        {
            try
            {
               
                SqlDataAdapter daIntersections = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                string sqlQueryIntersections = "SELECT * FROM SourceIntersectionDistance ORDER BY DISTANCE";
                cmd.CommandText = sqlQueryIntersections;
                daIntersections.SelectCommand = cmd;
                DataSet dsIntersections = new DataSet();
                conn.Open();
                daIntersections.Fill(dsIntersections);
                conn.Close();
                return dsIntersections;
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
                return null;
            }
            
        }

        public string checkOperationalWithCongestion(decimal sourceIntersectionLat, decimal sourceIntersectionLng, decimal destIntersectionLat, decimal destIntersectionLng, string datetime, string addedTime,int congestionPercent)
        {
            
            SqlCommand cmdCheckOperational = new SqlCommand();
            Object returnValue;

            cmdCheckOperational.CommandText = "CHECKOPERATIONALINTERSECTIONSWITHCONGESTION";
            
            cmdCheckOperational.CommandType = CommandType.StoredProcedure;
            cmdCheckOperational.Connection = conn;
            int congestionCount = 0;
            if (congestionPercent == 0)
            {
                congestionCount = 0;
            }
            else if (congestionPercent == 10)
            {
                congestionCount = 2;
            }
            if (congestionPercent == 20)
            {
                congestionCount = 4;
            }
            if (congestionPercent == 30)
            {
                congestionCount = 6;
            }
            if (congestionPercent == 40)
            {
                congestionCount = 8;
            }
            if (congestionPercent == 50)
            {
                congestionCount = 10;
            }

            cmdCheckOperational.Parameters.AddWithValue("@SourceLatitude", sourceIntersectionLat);
            cmdCheckOperational.Parameters.AddWithValue("@SourceLongitude", sourceIntersectionLng);
            cmdCheckOperational.Parameters.AddWithValue("@DestinationLatitude", destIntersectionLat);
            cmdCheckOperational.Parameters.AddWithValue("@DestinationLongitude", destIntersectionLng);
            cmdCheckOperational.Parameters.AddWithValue("@TimeStamp", datetime);
            cmdCheckOperational.Parameters.AddWithValue("@TimeStamp1", addedTime);
            cmdCheckOperational.Parameters.AddWithValue("@CongestionCount", congestionCount);


            conn.Open();
            returnValue = cmdCheckOperational.ExecuteScalar();
            conn.Close();

            return Convert.ToString(returnValue);
        }

        public string checkOperationalWithoutCongestion(decimal sourceIntersectionLat, decimal sourceIntersectionLng, decimal destIntersectionLat, decimal destIntersectionLng,string datetime, string addedTime)
        {
            
            SqlCommand cmdCheckOperational = new SqlCommand();
            Object returnValue;

            cmdCheckOperational.CommandText = "CHECKOPERATIONALINTERSECTIONS";
            cmdCheckOperational.CommandType = CommandType.StoredProcedure;
            cmdCheckOperational.Connection = conn;

            //string datetime = "2012-04-06 00:08:00";
            //2012-04-06 00:02:00

            cmdCheckOperational.Parameters.AddWithValue("@SourceLatitude", sourceIntersectionLat);
            cmdCheckOperational.Parameters.AddWithValue("@SourceLongitude", sourceIntersectionLng);
            cmdCheckOperational.Parameters.AddWithValue("@DestinationLatitude", destIntersectionLat);
            cmdCheckOperational.Parameters.AddWithValue("@DestinationLongitude", destIntersectionLng);
            cmdCheckOperational.Parameters.AddWithValue("@TimeStamp", datetime);
            cmdCheckOperational.Parameters.AddWithValue("@TimeStamp1", addedTime);

            conn.Open();
            returnValue = cmdCheckOperational.ExecuteScalar();
            conn.Close();

            return Convert.ToString(returnValue);
        }

        public DataSet findBlockStartLocation(int block_id)
        {
            ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            string sqlQuery = "SELECT LATITUDE_1,LONGITUDE_1 FROM sfpark_edges WHERE BLOCK_ID=" + block_id;
            cmd.CommandText = sqlQuery;
            da.SelectCommand = cmd;

            conn.Open();
            da.Fill(ds);
            conn.Close();
            return ds;
        }

        public DataSet getNeighbors(decimal lat,decimal lng)
        {
            try
            {

                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GET_NEIGHBOURS";
                cmd.Connection = conn;
                da.SelectCommand = cmd;
                cmd.Parameters.AddWithValue("@Latitude", lat);
                cmd.Parameters.AddWithValue("@Longitude", lng);
                
                conn.Open();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
                return null;
            }

        }

        public void deleteTestData(string status)
        {
            try
            {
                SqlCommand sqlcmd;
                if(status == "WITH")
                {
                    sqlcmd = new SqlCommand("DELETE FROM Test_Data_UninformedSearchWithCongestion", conn);
                }
                else
                {
                    sqlcmd = new SqlCommand("DELETE FROM Test_Data_UninformedSearchWithoutCongestion", conn);
                }

                
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
            }

        }

        public void insertTestData(decimal latitude, decimal longitude, decimal destLat, decimal destLng, string startTime, string endTime, double travelDist, double travelTime)
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand("INSERT INTO Test_Data_UninformedSearchWithoutCongestion(UserLocationLatitude,UserLocationLongitude,ParkingSlotLatitude,ParkingSlotLongitude,StartTime,EndTime,WalkingDistance,WalkingTime) values(@Latitude,@Longitude,@DestLat,@DestLng,@StartTime,@EndTime,@WalkingDist,@WalkingTime)", conn);
                sqlcmd.Parameters.AddWithValue("@Latitude", latitude);
                sqlcmd.Parameters.AddWithValue("@Longitude", longitude);
                sqlcmd.Parameters.AddWithValue("@DestLat", destLat);
                sqlcmd.Parameters.AddWithValue("@DestLng", destLng);
                sqlcmd.Parameters.AddWithValue("@StartTime", startTime);
                sqlcmd.Parameters.AddWithValue("@EndTime", endTime);
                sqlcmd.Parameters.AddWithValue("@WalkingDist", travelDist);
                sqlcmd.Parameters.AddWithValue("@WalkingTime", travelTime);
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                conn.Close();
                conn.Open();
                sqlcmd = new SqlCommand("UPDATE Test_Data_UninformedSearchWithoutCongestion SET Parking_Time = datediff(MINUTE,startTime,endTime) ", conn);
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
            }
        }

        public void insertTestDataUninformedWithCongestion(decimal latitude, decimal longitude, decimal destLat, decimal destLng, string startTime, string endTime, int congestionPercent, double travelDist, double travelTime)
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand("INSERT INTO Test_Data_UninformedSearchWithCongestion(UserLocationLatitude,UserLocationLongitude,ParkingSlotLatitude,ParkingSlotLongitude,CongestionPercent,StartTime,EndTime,WalkingDistance,WalkingTIme) values(@Latitude,@Longitude,@DestLat,@DestLng,@CongestionPercent,@StartTime,@EndTime,@WalkDist,@WalkTime)", conn);
                sqlcmd.Parameters.AddWithValue("@Latitude", latitude);
                sqlcmd.Parameters.AddWithValue("@Longitude", longitude);
                sqlcmd.Parameters.AddWithValue("@DestLat", destLat);
                sqlcmd.Parameters.AddWithValue("@DestLng", destLng);
                sqlcmd.Parameters.AddWithValue("@CongestionPercent", congestionPercent);
                sqlcmd.Parameters.AddWithValue("@StartTime", startTime);
                sqlcmd.Parameters.AddWithValue("@EndTime", endTime);
                sqlcmd.Parameters.AddWithValue("@WalkDist", travelDist);
                sqlcmd.Parameters.AddWithValue("@WalkTime", travelTime);
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                conn.Close();
                conn.Open();
                sqlcmd = new SqlCommand("UPDATE Test_Data_UninformedSearchWithCongestion SET Parking_Time = datediff(MINUTE,startTime,endTime) ", conn);
                sqlcmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
            }
        }

        public DataSet getResultsUninformedSearchWithoutCongestion()
        {
            try
            {
                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                string sqlQuery = "";
                sqlQuery = "SELECT * FROM Test_Data_UninformedSearchWithoutCongestion";

                cmd.CommandText = sqlQuery;
                da.SelectCommand = cmd;

                conn.Open();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
                return null;
            }
        }

        public DataSet getResultsUninformedSearchWithCongestion()
        {
            try
            {
                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                string sqlQuery = "";
                sqlQuery = "SELECT * FROM Test_Data_UninformedSearchWithCongestion";

                cmd.CommandText = sqlQuery;
                da.SelectCommand = cmd;

                conn.Open();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "DBMSResourceSharingProjectDataAccessLayer";
                appLog.WriteEntry(e.Message);
                return null;
            }
        }
    }
}
