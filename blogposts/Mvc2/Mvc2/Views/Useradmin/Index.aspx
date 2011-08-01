<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Mvc2.Views.Useradmin.Index" %>
<%@ Import Namespace="Mvc2.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>
        Useradministration
    </h2>
    <table>
        <tr>
            <td>Name</td>
            <td>Email</td>
            <td>Created</td>
            <td>Last Login</td>
            <td>Is online?</td>
            <td>Options</td>
        </tr>
        <% foreach(MembershipUser user in ViewData) 
           { %>
        <tr>
            <td><%=user.UserName %></td>
            <td><%=user.Email %></td>
            <td><%=user.CreationDate.ToShortDateString() %></td>
            <td><%=user.LastLoginDate.ToShortDateString() %></td>
            <td><%=user.IsOnline.ToString() %></td>
            <%if (Membership.GetUser().UserName != user.UserName)
              {%>
                <td><%=Html.ActionLink<UseradminController>(link => link.Delete(user.UserName), "Delete")%></td>
            <%} %>
        </tr>      
          <%} %>
    </table>
</asp:Content>
