<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" Title="Bookstorage | Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" Runat="Server">
<form id="searchform" action="/Intro/Search/Results" method="post">
    <span>Search:</span>
    <input type="text" name="query" />
    <button type="submit">Suchen</button>
</form>

</asp:Content>

