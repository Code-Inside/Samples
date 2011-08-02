<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%
  List<string> result = (List<string>)ViewData["News"];
      
  foreach (string item in result)
  {%>
    <%=item %>
<%} %>
