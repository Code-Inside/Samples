<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="script" ContentPlaceHolderID="ScriptContent" runat="server">
<script language="javascript" type="text/javascript">
    $(document).ready(function() {
        $("#search").autocomplete('<%=Url.Action("Search") %>', {
                                    width: 300,
                                    multiple: true,
                                    matchContains: true,
                                    dataType: 'json',
                                    parse: function(data) {
                                          var rows = new Array();
                                          for(var i=0; i<data.length; i++){
                                              rows[i] = { data:data[i], value:data[i], result:data[i] };
                                          }
                                          return rows;
                                      },
                                    formatItem: function(row, i, n) {
                                          return row;
                                      }
                                });

    });
</script>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        To learn more about ASP.NET MVC visit <a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a>.
    </p>
    <p>
        <input type="text" id="search" />
    </p>
</asp:Content>
