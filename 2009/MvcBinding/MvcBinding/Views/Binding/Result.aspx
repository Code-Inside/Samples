<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcBinding.Models.Person>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Result
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Result</h2>
    <p><%=Html.ActionLink("Back to Index", "Index") %></p>
    <fieldset>
        <legend>Fields</legend>
        <p>
            Id:
            <%= Html.Encode(Model.Id) %>
        </p>
        <p>
            Prename:
            <%= Html.Encode(Model.Prename) %>
        </p>
        <p>
            Surname:
            <%= Html.Encode(Model.Surname) %>
        </p>
        <p>
            Age:
            <%= Html.Encode(Model.Age) %>
        </p>
    </fieldset>
    <p><%=Html.ActionLink("Back to Index", "Index") %></p>

</asp:Content>

