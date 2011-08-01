<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VirtualEarthDemo._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Virtual Earth Demo</title>
    <script language="javascript" type="text/javascript" src="http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=5"></script>
    <script language="javascript" type="text/javascript">
             var map = null;
             var shape = null;
             
             function GetMap()
             {
             map = new VEMap('<%=PanelMap.ClientID%>'); // ZielDiv
             map.LoadMap();                             // Karte initialisieren
             map.SetZoomLevel(14);                      // Zoomlevel
             map.HideDashboard();                       // Auswahloptionen verstecken
             map.SetMapStyle(VEMapStyle.Aerial);        // Auf Arealsicht stellen
             map.SetScaleBarDistanceUnit(VEDistanceUnit.Kilometers); // Von Meilen auf Kilometer umstellen
             
             map.Find('','<%=Request.QueryString["destination"] %>, Germany',null,null,0,1,false,false,false,true, onComplete); // Suchen
             }
             
             function onComplete(shape, FindResult, VEPlace, HasMoreFlag)
             {
             shape = new VEShape(VEShapeType.Pushpin, VEPlace[0].LatLong);
             shape.SetTitle(VEPlace[0].Name);
             map.AddShape(shape);
             }
             
    </script>
</head>
<body onload="GetMap();">
    <form id="form1" enableviewstate="false" runat="server">

    <asp:Panel ID="PanelMap" style="position:relative; width:400px; height:400px;" runat="server">
    
    </asp:Panel>
    </form>
</body>
</html>
