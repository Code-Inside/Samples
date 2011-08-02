<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HomeViewModel>" %>
<%@ Import Namespace="MvcCascadingDropdownJson.Models"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Cascading DropDown with AJAX</h2>
    <% using(Html.BeginForm()) { %>
    <div>
        <fieldset>
            <legend>Country/FederalState/City/Street Selection</legend>
            <p>
                <%=Html.ValidationMessage("Message") %>
            </p>
            <p>
                <lable>Country</lable>
                <%=Html.DropDownList("Country",Model.Countries, new { id = "CountrySelection", onchange="changeCountry()" }) %> 
            </p>
            <p>
                <lable>Federal States</lable>
                <%=Html.DropDownList("FederalState", Model.FederalStates, new { id = "FedStateSelection", onchange = "changeFederalState()" })%> 
            </p>
            <p>
                <lable>City</lable>
                <%=Html.DropDownList("City", Model.Cities, new { id = "CitySelection", onchange = "changeCity()" })%> 
            </p>
            <p>
                <lable>Street</lable>
                <%=Html.DropDownList("Street", Model.Streets, new { id = "StreetSelection" })%> 
            </p>
            <button type="submit">Submit!</button>
        </fieldset>
    </div>
    <%} %>
</asp:Content>
