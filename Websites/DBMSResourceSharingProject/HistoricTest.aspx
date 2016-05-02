<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HistoricTest.aspx.cs" Inherits="HistoricTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="js/GetData.js"></script>
    <title>Historic Data Testing Results</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1><b>Test Results</b></h1>
    <div>
  </div>
        <asp:Table ID="ResultsTab"  BorderStyle="Double" CellPadding="2" align="Center" GridLines="Both" runat="server">
        </asp:Table>
    </form>
</body>
</html>