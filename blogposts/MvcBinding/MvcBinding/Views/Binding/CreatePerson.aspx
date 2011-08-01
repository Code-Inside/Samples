<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcBinding.Models.Person>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	DefaultBinding
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create Person</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Id">Id:</label>
                <%= Html.TextBox("Id") %>
                <%= Html.ValidationMessage("Id", "*") %>
            </p>
            <p>
                <label for="Prename">Prename:</label>
                <%= Html.TextBox("Prename") %>
                <%= Html.ValidationMessage("Prename", "*") %>
            </p>
            <p>
                <label for="Surname">Surname:</label>
                <%= Html.TextBox("Surname") %>
                <%= Html.ValidationMessage("Surname", "*") %>
            </p>
            <p>
                <label for="Age">Age:</label>
                <%= Html.TextBox("Age") %>
                <%= Html.ValidationMessage("Age", "*") %>
            </p>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

