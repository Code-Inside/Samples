<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>MVC Binding</h2>
    <ul>
        <li><%=Html.ActionLink("FormCollection", "FormCollection") %></li>
        <li><%=Html.ActionLink("ParameterMatching", "ParameterMatching")%></li>
        <li><%=Html.ActionLink("DefaultBinding", "DefaultBinding")%></li>
        <li><%=Html.ActionLink("DefaultBindingWithInclude", "DefaultBindingWithInclude")%></li>
        <li><%=Html.ActionLink("DefaultBindingWithExclude", "DefaultBindingWithExclude")%></li>
        <li><%=Html.ActionLink("PersonModelBinder", "PersonModelBinder")%></li>
    </ul>

</asp:Content>
