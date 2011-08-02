<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Form
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Forms</h2>
    <p style="display: none" id="Loading">
        Load
    </p>
    <p>
        <%using (Ajax.BeginForm("ItemEdit",
                                "Ajax",
                                new AjaxOptions() { HttpMethod = "POSt",
                                                    InsertionMode = InsertionMode.InsertBefore,
                                                    UpdateTargetId = "AjaxResult" }))
          {%> <br />
            <%=Html.TextBox("Input") %>
            <button type="submit">Ok</button>
        <%} %>
    </p>
    <p id="AjaxResult">
    </p>
</asp:Content>
