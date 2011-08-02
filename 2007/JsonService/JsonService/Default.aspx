<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JsonService._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Json Service</title>
    <script src="ContactJsonService.ashx" type="text/javascript" language="javascript"></script>
    <script type="text/javascript" language="javascript">
    
    function create()
    {
        for(i=0; i<ContactList.length; i++)
        {
        var newLi = document.createElement("li");
        newLi.innerHTML = ContactList[i].Name;
        $get("list").appendChild(newLi);
        }
    }
    </script>
</head>
<body onload="create()">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">

        </asp:ScriptManager>
    <ul id="list">
    
    </ul>
    </form>
</body>
</html>
