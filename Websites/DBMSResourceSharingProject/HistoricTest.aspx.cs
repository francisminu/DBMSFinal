using System;
using System.Collections;
using System.Web.UI.WebControls;

public partial class HistoricTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ArrayList totalTime = (ArrayList)Session["totalTimes"];
        ArrayList srcLat = (ArrayList)Session["srcLat"];
        ArrayList srcLong = (ArrayList)Session["srcLong"];
        ArrayList destLats = (ArrayList)Session["destLats"];
        ArrayList destLongs = (ArrayList)Session["destLongs"];
        ArrayList reqTime = (ArrayList)Session["reqTime"];
        ArrayList congestionLevel = (ArrayList)Session["congestionLevel"];
        ArrayList walkTime = (ArrayList)Session["WalkT"];
       // ArrayList walkD =(ArrayList)Session["WalkD"];
        TableRow trh = new TableRow();

        TableCell cell0 = new TableCell();
        cell0.Text = "Id";
        trh.Cells.Add(cell0);

        TableCell cell1 = new TableCell();
        cell1.Text =  "UserLocationLatitude";
        trh.Cells.Add(cell1);

        TableCell cell2 = new TableCell();
        cell2.Text = "UserLocationLongitude";
        trh.Cells.Add(cell2);


       // TableCell cell3 = new TableCell();
        //cell3.Text = "Destination Latitude";
        //trh.Cells.Add(cell3);

        //TableCell cell4 = new TableCell();
        //cell4.Text = "Destination Longitude";
        //trh.Cells.Add(cell4);


        TableCell cell41 = new TableCell();
        cell41.Text = "CongestionPercent";
        trh.Cells.Add(cell41);

        TableCell cell5 = new TableCell();
        cell5.Text = "StartTime";
        trh.Cells.Add(cell5);

        TableCell cell6 = new TableCell();
        cell6.Text = "Parking_Time";
        trh.Cells.Add(cell6);

      //  TableCell cell8 = new TableCell();
        //cell8.Text = "WalkingDistance";
        //trh.Cells.Add(cell8);

        TableCell cell7 = new TableCell();
        cell7.Text = "Walking_Time";
        trh.Cells.Add(cell7);
           

        ResultsTab.Rows.Add(trh);

        for (int j = 0;j<reqTime.Count;j++)
        {
            TableRow tr = new TableRow();
            TableCell no = new TableCell();
            TableCell tc = new TableCell();
            TableCell tc1 = new TableCell();
            TableCell tc2 = new TableCell();
            TableCell tc3 = new TableCell();
            TableCell tc4 = new TableCell();
            TableCell tc5 = new TableCell();
            TableCell tc6 = new TableCell();
            TableCell tc7 = new TableCell();
            TableCell tc8 = new TableCell();

            //Add Arraylist item into TableCell to display it
            no.Text = ((j+1)+"");
            tr.Cells.Add(no);
            tc.Text = srcLat[j].ToString();
            tr.Cells.Add(tc);

            tc1.Text = srcLong[j].ToString();
            tr.Cells.Add(tc1);

            //tc2.Text = destLats[j].ToString();
            //tr.Cells.Add(tc2);

            //tc3.Text = destLongs[j].ToString();
          //  tr.Cells.Add(tc3);

            tc6.Text = congestionLevel[j].ToString();
            tr.Cells.Add(tc6);

            tc4.Text = reqTime[j].ToString();
            tr.Cells.Add(tc4);

            tc5.Text = totalTime[j].ToString();
            tr.Cells.Add(tc5);

          //  tc8.Text = totalTime[j].ToString();
            //tr.Cells.Add(tc8);

            tc7.Text = walkTime[j].ToString();
            tr.Cells.Add(tc7);
            ResultsTab.Rows.Add(tr);


        }
       

    }





}