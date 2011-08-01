<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Microsoft.Web.Mvc"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    
    <p>Ohne Parameter: </p>
     <% Html.RenderAction("News", "Home"); %>
    
    <p>
        Mit Parameter:
    </p>
    
    <% Html.RenderAction("News", "Home", new { count = 15 }); %>
</asp:Content>
