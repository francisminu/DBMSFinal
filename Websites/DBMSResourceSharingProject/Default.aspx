﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblHeading" runat="server" Text="TEST RESULTS - UNINFORMED SEARCH WITHOUT CONGESTION"></asp:Label>
    </div>
        <asp:GridView ID="grdResults" runat="server">
        </asp:GridView>
    </form>
</body>
</html>
