<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MvcLocalization"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Test</h2>
    <p>
		Global resource : <%=Html.Resource("Strings, GlobalResourceKey") %><br />
		Local resource : <%=Html.Resource("LocalKey")%><br />
    </p>
</asp:Content>
