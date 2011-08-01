<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Simple Silverlight Videoplayer</title>
    <script src="Silverlight.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" type="text/javascript">


    function createSilverlight()
    {
    Silverlight.createObjectEx({
		source: "Videoplayer.xaml",
		parentElement: document.getElementById("SilverlightVideoHost"),
		id: "SilverlightControl",
		properties: {
			width: "100%",
			height: "100%",
			version: "1.0"
		},
		events: {
         onError: null,
         onLoad: null
      }

	});
	}
	
	function play()
	{
	var control = document.getElementById("SilverlightControl");
	control.Content.FindName("Player").play();	
	}
	
	function stop()
	{
	var control = document.getElementById("SilverlightControl");
	control.Content.FindName("Player").stop();
	}
	
	function pause()
	{
	var control = document.getElementById("SilverlightControl");
	control.Content.FindName("Player").pause();	
	}
    </script>
</head>
<body onload="createSilverlight()">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div>
            <button onclick="play()">Play</button>
            <button onclick="stop()">Stop</button>
            <button onclick="pause()">Pause</button>
        </div>
        <div id="SilverlightVideoHost">
        </div>
    </form>
</body>
</html>
