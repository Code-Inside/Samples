<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Ajax.ActionLink</h2>
    <p style="display: none" id="Loading">
        Load
    </p>
    <p>
        <%=Ajax.ActionLink("Ajax.ActionLink + Replace",
                           "ItemData",
                           "Ajax",
                           new AjaxOptions() { HttpMethod = "GET", 
                                               InsertionMode = InsertionMode.Replace,
                                               UpdateTargetId = "AjaxResult" })%> <br />
        <%=Ajax.ActionLink("Ajax.ActionLink + InsertAfter",
                           "ItemData",
                           "Ajax",
                           new AjaxOptions() { HttpMethod = "GET",
                                               InsertionMode = InsertionMode.InsertAfter,
                                               UpdateTargetId = "AjaxResult"}) %> <br />
        <%=Ajax.ActionLink("Ajax.ActionLink + InsertAfter + Loading",
                           "ItemData",
                           "Ajax",
                           new AjaxOptions() { HttpMethod = "GET",
                                               InsertionMode = InsertionMode.InsertAfter,
                                               UpdateTargetId = "AjaxResult",
                                               LoadingElementId = "Loading" })%> <br />
    </p>
    <p id="AjaxResult">
    </p>
</asp:Content>
