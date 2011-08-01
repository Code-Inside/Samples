<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <ul>
    <li><%=Ajax.ActionLink("Hier klicken für Tab1", "Index", "Home", new { id = "Tab1" },new AjaxOptions()
                                                                   {
                                                                       HttpMethod = "GET",
                                                                       UpdateTargetId = "resultPanel",
                                                                       InsertionMode = InsertionMode.Replace,
                                                                   })%>	</li>		
    <li><%=Ajax.ActionLink("Hier klicken für Tab2", "Index", "Home", new { id = "Tab2" }, new AjaxOptions()
                                                                {
                                                                   HttpMethod = "GET",
                                                                   UpdateTargetId = "resultPanel",
                                                                   InsertionMode = InsertionMode.Replace
                                                               })%></li>
   <li><%=Ajax.ActionLink("Hier klicken für Tab3", "Index", "Home", new { id = "Tab3" }, new AjaxOptions()
                                                                {
                                                                   HttpMethod = "GET",
                                                                   UpdateTargetId = "resultPanel",
                                                                   InsertionMode = InsertionMode.Replace
                                                               })%></li>                                                           
    </ul>
    <div id="resultPanel">
    
    <%if(ViewData["ActivTab"].ToString() == "Tab1") Html.RenderPartial("Tab1"); %>
    <%if(ViewData["ActivTab"].ToString() == "Tab2") Html.RenderPartial("Tab2"); %>
    <%if(ViewData["ActivTab"].ToString() == "Tab3") Html.RenderPartial("Tab3"); %>      
    </div>
</asp:Content>
