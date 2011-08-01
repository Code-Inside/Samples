<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WCF & ASMX JSON Webservices</title>
    <script type="text/javascript">
    
    /*
     * WCF
     *
     */
    function onDateTimeLoad()
    {
        ServiceNamespace.Service.GetDateTime(onDateTimeLoadCompleted);
    }
    
    function onDateTimeLoadCompleted(result)
    {
        $get('result').innerHTML = result;
    }

    function onComplexOneLoad()
    {
        ServiceNamespace.Service.GetComplexOne(onComplexOneLoadCompleted);
    }
    
    function onComplexOneLoadCompleted(result)
    {
        $get('result').innerHTML = result;
    }

    function onComplexListLoad()
    {
        ServiceNamespace.Service.GetComplexList(onComplexListLoadCompleted);
    }
    
    function onComplexListLoadCompleted(result)
    {
        $get('result').innerHTML = result;
    }
    
    /*
     * ASMX
     *
     */   
     
    function onDateTimeLoadAsmx()
    {
        WebServiceNamespace.WebService.GetDateTime(onDateTimeLoadAsmxCompleted)
    }
    
    function onDateTimeLoadAsmxCompleted(result)
    {
        $get('result').innerHTML = result;
    }    
    
    function onComplexOneLoadAsmx()
    {
        WebServiceNamespace.WebService.GetComplexOne(onComplexOneLoadAsmxCompleted);
    }
    
    function onComplexOneLoadAsmxCompleted(result)
    {
        $get('result').innerHTML = result;
    }

    function onComplexListLoadAsmx()
    {
        WebServiceNamespace.WebService.GetComplexList(onComplexListLoadAsmxCompleted);
    }
    
    function onComplexListLoadAsmxCompleted(result)
    {
        $get('result').innerHTML = result;
    }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/Service.svc" />
            <asp:ServiceReference Path="~/WebService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div>
        <table>
            <tr>
                <td>WCF</td>
                <td>ASMX</td>
            </tr>
            <tr>
                <td><button type="button" onclick="onDateTimeLoad()">DateTime</button></td>
                <td><button type="button" onclick="onDateTimeLoadAsmx()">DateTime</button></td>
            </tr>
            <tr>
                <td><button type="button" onclick="onComplexOneLoad()">ComplexOne</button></td>
                <td><button type="button" onclick="onComplexOneLoadAsmx()">ComplexOne</button></td>
            </tr>
            <tr>
                <td><button type="button" onclick="onComplexListLoad()">ComplexList</button></td>
                <td><button type="button" onclick="onComplexListLoadAsmx()">ComplexList</button></td>
            </tr>
        </table>

        <div id="result" style="border: solid 1px black;"></div>
    </div>
    </form>
</body>
</html>
