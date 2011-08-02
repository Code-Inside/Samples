<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script language="javascript" type="text/javascript">
    function loadHelloWorld()
    {
    PageMethods.HelloWorld("Robert", onComplete);
    }
    
    function onComplete(result)
    {
        alert(result);
    }
    </script>
</head>
<body onload="loadHelloWorld()">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
        <div>
        </div>
    </form>
</body>
</html>
