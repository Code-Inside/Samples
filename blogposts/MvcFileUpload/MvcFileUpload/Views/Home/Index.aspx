<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%=Html.Encode(ViewData["Message"])%></h2>
    <p>
        <%using(Html.BeginForm("FileUpload", 
                               "Home", 
                               FormMethod.Post,
                               new {enctype = "multipart/form-data"})) {%>
        <input type="file" name="file" id="file" />
        <input type="submit" name="submit" value="Submit" />
        <% } %>
    </p>
</asp:Content>
