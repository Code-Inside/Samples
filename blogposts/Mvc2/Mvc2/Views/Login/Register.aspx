<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Mvc2.Views.Login.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
     <h2>
        Register
    </h2>
    <%using (Html.Form("Login", "Register"))
      { %>
    
        <table>
            <tr>
                <td>Name:</td>
                <td><input type="text" name="Login" /></td>
            </tr>
            <tr>
                <td>Email:</td>
                <td><input type="text" name="Email"</td>
            </tr>
            <tr>
                <td>Password:</td>
                <td><input type="password" name="Password"</td>
            </tr>
            <tr>
                <td colspan="2">
                <button>Register</button>
                </td>
            </tr>
            <% if (ViewData["Failur"] != null)
               {%>
            <tr>
                <td colspan="2">Failed to login!</td>
            </tr>      
               <%} %>
        </table>
    
    <%} %>
</asp:Content>
