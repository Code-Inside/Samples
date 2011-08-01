<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DllMembership.Web.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Name: <br />
        <asp:TextBox ID="Username" runat="server" />
        
        <br/ />Password: <br />
        <asp:TextBox ID="Password" TextMode="Password" runat="server" />
        <asp:Button OnClick="DoLogin" ID="LoginButton" runat="server" Text="Login" />
        <asp:Label ID="Error" runat="server" Visible="false" Text="Error" />
    </form>
</body>
</html>
