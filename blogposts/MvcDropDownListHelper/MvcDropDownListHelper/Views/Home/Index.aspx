<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcDropDownListHelper.Models.FooViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>DropDownList</h2>
    <p>
    <% using (Html.BeginForm()) { %>
    <%= Html.DropDownListFor()
        x => x.TimeZone, 
        Model.TimeZones) %>
    <input type="submit" value="Select timezone" />
    <% } %>

    <div><%= Html.Encode(Model.TimeZone) %></div>
    </p>
</asp:Content>
