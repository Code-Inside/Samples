<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Mvc2.Views.Login.Index" %>
<%@ Import Namespace="Mvc2.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>
        Login
    </h2>
    <%using (Html.Form("Login", "Login"))
      { %>
    
        <table>
            <tr>
                <td>Name:</td>
                <td><input type="text" name="Login" /></td>
            </tr>
            <tr>
                <td>Password:</td>
                <td><input type="password" name="Password"</td>
            </tr>
            <tr>
                <td colspan="2">
                <button>Login</button>
                </td>
            </tr>
            <% if (ViewData["Failur"] != null)
               {%>
            <tr>
                <td colspan="2">Failed to login!</td>
            </tr>      
               <%} %>
            <tr>
                <td colspan="2"><%= Html.ActionLink<LoginController>(link => link.PasswordRecovery(), "Password Recovery")%></td>
            </tr>   
        </table>
    
    <%} %>
</asp:Content>
