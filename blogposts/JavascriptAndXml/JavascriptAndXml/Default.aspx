<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JavascriptAndXml._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Xml mit Javascript parsen</title>
    <script language="javascript" type="text/javascript">
        
        var Global_Example = 1;
        
        function reset()
        {
        document.getElementById("Description").innerHTML = "";
        document.getElementById("Result").innerHTML = "";
        }
    
        function loadExample(id)
        {
            reset();
            Global_Example = id;
            JavascriptAndXml.ExampleService.LoadExample(Global_Example, loadExampleCompleted);
        }
        
        function loadExampleCompleted(result)
        {
            if(Global_Example == 1)
                {
                document.getElementById("Description").innerHTML = "Den Inhalt des root Elements ausgeben: ";
                var myResult = result.getElementsByTagName("root")[0].firstChild.nodeValue;
                document.getElementById("Result").innerHTML = myResult;
                }
            else if(Global_Example == 2)
                {
                document.getElementById("Description").innerHTML = "Wir möchten alle Items ausgeben: ";
                var myResult = "";
                for(i = 0; i < result.getElementsByTagName("item").length; i++)
                    {
                    myResult += result.getElementsByTagName("item")[i].firstChild.nodeValue + "<br/>";
                    }   
                             
                document.getElementById("Result").innerHTML = myResult;   
                }
            else
                {
                document.getElementById("Description").innerHTML = "Wir möchten alle Daten (samt Attribute) ausgeben lassen: ";
                document.getElementById("Result").innerHTML = "Kategorie: " + result.getElementsByTagName("itemCollection")[0].getAttribute("category") + "<br/>";
                
                var myResult = "";
                for(i = 0; i < result.getElementsByTagName("item").length; i++)
                    {
                    var oneItem = result.getElementsByTagName("item")[i];
                    
                    myResult += oneItem.childNodes[0].firstChild.nodeValue + " - " + oneItem.childNodes[1].firstChild.nodeValue + " (" + oneItem.getAttribute("id") + ")" + "<br/>";
                    }            
                    document.getElementById("Result").innerHTML += myResult;   
                }

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="ExampleService.asmx" />
            </Services>
        </asp:ScriptManager>
    <h1>Xml mit Javascript parsen</h1>
    <button onclick="loadExample(1)" type="button">Example 1 laden</button>
    <button onclick="loadExample(2)" type="button">Example 2 laden</button>
    <button onclick="loadExample(3)" type="button">Example 3 laden</button>
    
    <div id="XmlDiv">
        <span id="Description"></span>
        <div id="Result">
        
        </div>
    </div>
    </form>
</body>
</html>
