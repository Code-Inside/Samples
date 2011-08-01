<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="addHead" ContentPlaceHolderID="AdditionalHeadContent" runat="server">
<script language="javascript" type="text/javascript">

    $().ready(function() {
        $("#ContactForm").validate({
            rules: {
                Email: { required: true, email: true },
                Feedback: "required"
            }
        });
    });

</script>
</asp:Content>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <%= Html.ValidationSummary() %>

    <form method="post" action="<%=Url.Action("Index", "Home") %>" id="ContactForm">
        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Email") %>
                <%= Html.ValidationMessage("Email",
                                            new
                                            {
                                                @class = "error",
                                                generated = "true",
                                                @for = "Email"
                                            })%>
            </p>
            <p>
                <label for="Description">Feedback:</label>
                <%= Html.TextArea("Feedback") %>
                <%= Html.ValidationMessage("Feedback",
                                            new
                                            {
                                                @class = "error",
                                                generated = "true",
                                                @for = "Feedback"
                                            })%>
            </p>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    </form>

</asp:Content>
