<%@ Import Namespace="MvcRenderToString.Models"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<ExcelData>>" %>

<table>
    <tr>
        <% foreach(ExcelData excel in Model) {%>
        <td><%=excel.Foobar %></td>
        <% } %>
    </tr>
</table>