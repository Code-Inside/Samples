<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CrossDomainAjax._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<script src="http://localhost:56761/CrossDomainScript.js" type="text/javascript"></script>
<script src="jquery-1.3.2.min.js" type="text/javascript"></script>
    <title>Untitled Page</title>
 
</head>
<body>
    <form id="form1" runat="server">
    <div id="test">
        ZielSeite    
    </div>
    </form>
       <script type="text/javascript">
      $.ajax({
                dataType: 'jsonp',
                jsonp: 'jsonp_callback',
                url: 'http://localhost:56761/TestService.ashx',
                success: function (j) {
                    alert(j.response);
                },
            });     
    </script>
</body>
</html>
