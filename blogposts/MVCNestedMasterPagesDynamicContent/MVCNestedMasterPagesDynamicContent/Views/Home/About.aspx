<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="MVCNestedMasterPagesDynamicContent.Views.Home.About" %>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData.Model.Text) %></h2>
    <p>
        TODO: Put <em>about</em> content here.
    </p>
</asp:Content>
