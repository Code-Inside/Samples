<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	jQuery
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
	﻿function ajaxCall() {
	    $.ajax(
	        {
	            type: "POST",
	            url: "<%=Url.Action("ItemData","Ajax") %>",
	            success: function(result) {
	                $("#AjaxResult").html(result);
	            }
	        });
	    } 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>jQuery</h2>
    <p>
        <button type="button" onclick="ajaxCall()">Ok</button>
    </p>
    <p id="AjaxResult">
    </p>
</asp:Content>
