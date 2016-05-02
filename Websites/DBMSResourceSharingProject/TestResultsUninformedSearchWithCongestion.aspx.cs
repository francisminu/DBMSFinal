using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using DBMSResourceSharingProjectBLL;
public partial class TestResultsUninformedSearchWithCongestion : System.Web.UI.Page
{
    ResourceSharingBLL bllObject = new ResourceSharingBLL();
    protected void Page_Load(object sender, EventArgs e)
    {
        DataSet ds = new DataSet();
        ds = bllObject.getResultsUninformedSearchWithCongestion();
        grdResults.DataSource = ds;
        grdResults.DataBind();
       // grdResults.Columns[7].Visible = false;
    }
    protected void grdResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[7].Visible = false;
    }
}
