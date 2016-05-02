<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestResultsUninformedSearch.aspx.cs" Inherits="TestUninformedSearch" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="div1" align="center">
        <asp:Label ID="lblHeading" align="center" runat="server" Text="TEST RESULTS WITHOUT CONGESTION"></asp:Label>
        <br /><br />
        <asp:GridView ID="grdResults" runat="server" align="center" OnRowDataBound="grdResults_RowDataBound">
        </asp:GridView>
    </div>
        
    </form>
</body>
</html>
