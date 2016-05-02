using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBMSResourceSharingProjectDAL;
using System.Data;

namespace DBMSResourceSharingProjectBLL
{
    
    public class ResourceSharingBLL
    {
        ResourceSharingDAL dalObject = new ResourceSharingDAL();
        public DataSet getIntersections(int flag, decimal latitude, decimal longitude)
        {
            return dalObject.getIntersections(flag,latitude,longitude);
        }

        public string getParkingLocationFromNodeId(string nodeId)
        {
            return dalObject.getParkingLocationFromNodeId(nodeId);
        }

        public string getNodeId(decimal lat,decimal lng)
        {
            return dalObject.getNodeId(lat,lng);
        }
        public void deleteSourceIntersectionDistances()
        {
            dalObject.deleteSourceIntersectionDistances();
        }

        public void insertDistance(decimal latitude, decimal longitude, double distance, string name, int nodeId)
        {
            dalObject.insertDistance(latitude, longitude, distance, name, nodeId);
        }

        public DataSet getIntersectionsWithDistances()
        {
            return dalObject.getIntersectionsWithDistances();
        }

        public string checkOperationalWithCongestion(decimal sourceIntersectionLat, decimal sourceIntersectionLng, decimal destIntersectionLat, decimal destIntersectionLng, string datetime, string addedTime, int congestionPercent)
        {
            return dalObject.checkOperationalWithCongestion(sourceIntersectionLat, sourceIntersectionLng, destIntersectionLat, destIntersectionLng, datetime,addedTime,congestionPercent);
        }

        public string checkOperationalWithoutCongestion(decimal sourceIntersectionLat, decimal sourceIntersectionLng, decimal destIntersectionLat, decimal destIntersectionLng,string datetime, string addedTime)
        {
            return dalObject.checkOperationalWithoutCongestion(sourceIntersectionLat, sourceIntersectionLng, destIntersectionLat, destIntersectionLng,datetime,addedTime);
        }

        public DataSet findBlockStartLocation(int block_id)
        {
            return dalObject.findBlockStartLocation(block_id);
        }

        public DataSet getNeighbors(decimal lat,decimal lng)
        {
            return dalObject.getNeighbors(lat,lng);
        }

        public void insertTestData(decimal latitude, decimal longitude, decimal destLat, decimal destLng,string startTime, string endTime,double travelDist,double travelTime)
        {
            dalObject.insertTestData(latitude, longitude, destLat, destLng, startTime, endTime, travelDist, travelTime);
        }

        public void deleteTestData(string status)
        {
            dalObject.deleteTestData(status);
        }

        public DataSet getResultsUninformedSearchWithoutCongestion()
        {
            return dalObject.getResultsUninformedSearchWithoutCongestion();
        }

        public DataSet getResultsUninformedSearchWithCongestion()
        {
            return dalObject.getResultsUninformedSearchWithCongestion();
        }

        public void insertTestDataUninformedWithCongestion(decimal latitude, decimal longitude, decimal destLat, decimal destLng, string startTime, string endTime, int congestionPercent, double travelDist, double travelTime)
        {
            dalObject.insertTestDataUninformedWithCongestion(latitude, longitude, destLat, destLng, startTime, endTime, congestionPercent, travelDist,travelTime);
        }
        
    }
}
