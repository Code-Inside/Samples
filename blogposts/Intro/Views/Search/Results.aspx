<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" Title="Bookstorage | Searchresults" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" Runat="Server">
Search Results:

<table>
<%foreach (var returnBook in (BookCollection)ViewData["BookCollection"])  { %>
<tr>
    <td><%= returnBook.Title %></td>
    <td><%= returnBook.Description %></td>
    <td><%= returnBook.PicUrl %></td>
</tr>
<% } %>
</table>
</asp:Content>

