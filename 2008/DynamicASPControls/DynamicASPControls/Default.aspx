<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="Default.aspx.cs" Inherits="DynamicASPControls._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/ScriptService.asmx" />
        </Services>
    </asp:ScriptManager>

    <button type="button" onclick="generateUserControl()">Generate UserControl!</button>
    <div id="result">
       
    </div>
    </form>
    
        <script type="text/javascript">       
        function generateUserControl()
        {
            document.getElementById("result").innerHTML = "Load...";
            DynamicASPControls.ScriptService.GetControlHtml("~/SampleUserControl.ascx", generateUserControlCompleted);
        }
    
        function ready(result)
        {
            alert(result);
        }
    
        function generateUserControlCompleted(result)
        {
            document.getElementById("result").innerHTML = result;
        }
        
        function failed(error)
        {
            alert(error);
        }
        
    </script>
</body>
</html>
