<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Mvc2.Views.Profile.Index" %>
<%@ Import Namespace="Mvc2.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>
        Profile
    </h2>
    <table>
        <tr>
            <td>Name:</td>
            <td><%=ViewData["Login"] %></td>
        </tr>
        <tr>
            <td>Email:</td>
            <td><%=ViewData["Email"] %></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink<ProfileController>(link => link.Edit(), "Edit")%></td>
        </tr>   
    </table>
</asp:Content>
