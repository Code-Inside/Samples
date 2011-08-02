<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Mvc2.Views.Profile.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>
        Edit Profile
    </h2>
     <%using (Html.Form("Profile", "Edit"))
       { %>
    <table>
        <tr>
            <td>Name:</td>
            <td><%=ViewData["Login"]%></td>
        </tr>
        <tr>
            <td>Email:</td>
            <td><input type="text" name="Email" value="<%=ViewData["Email"]%>" /></td>
        </tr>
        <tr>
            <td>Old Password:</td>
            <td><input type="password" name="OldPassword" /></td>
        </tr>
        <tr>
            <td>New Password:</td>
            <td><input type="password" name="NewPassword" /></td>
        </tr>
        <tr>
            <td colspan="2"><button>Submit</button></td>
        </tr>
    </table>
     <%} %>
</asp:Content>
