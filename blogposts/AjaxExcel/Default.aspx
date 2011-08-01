<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript">
    var DataExchangeHttpRequest;

    function createXlsData()
    {
    var data = "<table>";
            data += "<tr>";
                data += "<td>Test 1</td>";
                data += "<td>Test 2</td>";
             data += "</tr>"
        
    for(i=0; i < 10; i++)
    {
        var singleLine = "<tr>";
                singleLine += "<td> Test Data1: " + i + "</td>";
                singleLine += "<td> Test Data2: "  + i + "</td>";
        singleLine += "</tr>";
        data += singleLine;   
    }
        data += "</table>";
    sendXlsData(data);
    }

    function sendXlsData(data)
    {
    if (window.XMLHttpRequest) // Mozilla, Safari, Opera, IE7
        {
        DataExchangeHttpRequest = new XMLHttpRequest();
        }
    else if (window.ActiveXObject) // IE6, IE5
        {
        DataExchangeHttpRequest = new ActiveXObject("Microsoft.XMLHTTP");
        }

    DataExchangeHttpRequest.onreadystatechange = openXlsPage;
    DataExchangeHttpRequest.open('POST', 'ExcelHandler.ashx', true);
    DataExchangeHttpRequest.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    DataExchangeHttpRequest.setRequestHeader('Content-Length',data.length);

    DataExchangeHttpRequest.send(data);
    }
    function openXlsPage()
    {
        if (DataExchangeHttpRequest.readyState == 4 && DataExchangeHttpRequest.status == 200)
        {
        window.open("ExcelHandler.ashx?openXls=true","xls");
        }
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <button type="button" onclick="createXlsData()">Create XLS</button>
    </div>
    </form>
</body>
</html>
