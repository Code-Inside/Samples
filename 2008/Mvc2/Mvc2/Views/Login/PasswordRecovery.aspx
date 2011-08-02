<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="PasswordRecovery.aspx.cs" Inherits="Mvc2.Views.Login.PasswordRecovery" %>
<%@ Import Namespace="Mvc2.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>
        Password Recovery
    </h2>
    <%using (Html.Form("Login", "PasswordRecovery"))
      { %>
    
        <table>
            <% if (ViewData["NewPassword"] == null)
               {%>
            <tr>
                <td>Name:</td>
                <td><input type="text" name="Login" /></td>
            </tr>
            <tr>
                <td colspan="2"><button>Submit</button></td>
            </tr>
            <%} %>
            <% if (ViewData["NewPassword"] != null)
               {%>
            <tr>
                <td>Generated Password:</td>
                <td><%=ViewData["NewPassword"]%></td>
            </tr>
            <tr>
                <td colspan="2"><%= Html.ActionLink<LoginController>(link => link.Index(), "Login")%></td>
            </tr>      
               <%} %>
            <% if (ViewData["Failur"] != null)
               {%>
            <tr>
                <td colspan="2">No such user!</td>
            </tr>      
               <%} %>
        </table>
    
    <%} %>
</asp:Content>
