<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="ScriptContent" ID="script" runat="server">

		<script language="Javascript">

			// Remember to invoke within jQuery(window).load(...)
			// If you don't, Jcrop may not initialize properly
			jQuery(window).load(function(){

				jQuery('#cropbox').Jcrop({
					onChange: showPreview,
					onSelect: showPreview,
					aspectRatio: 1
				});

			});

			// Our simple event handler, called from onChange and onSelect
			// event handlers, as per the Jcrop invocation above
			function showPreview(coords)
			{
				if (parseInt(coords.w) > 0)
				{
					var rx = 100 / coords.w;
					var ry = 100 / coords.h;

					jQuery('#preview').css({
						width: Math.round(rx * 500) + 'px',
						height: Math.round(ry * 370) + 'px',
						marginLeft: '-' + Math.round(rx * coords.x) + 'px',
						marginTop: '-' + Math.round(ry * coords.y) + 'px'
					});
	            }

	            $('#x').val(coords.x);
	            $('#y').val(coords.y);
	            $('#w').val(coords.w);
	            $('#h').val(coords.h);
			}

		</script>

</asp:Content>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Crop</h2>
    
    <p>
        <!-- "Original" -->
        <img id="cropbox" src="<%=Url.Content("~/Content/flowers.jpg") %>" />
    </p>
    <p style="width:100px;height:100px;overflow:hidden;">
        <!-- Crop Preview -->
        <img id="preview" src="<%=Url.Content("~/Content/flowers.jpg") %>" />
    </p>
    <p>
        <!-- Form Data -->
        <form action="<%=Url.Action("PostPicture") %>" method="post">
            <input type="hidden" id="x" name="x" />
            <input type="hidden" id="y" name="y" />
            <input type="hidden" id="w" name="w" />
            <input type="hidden" id="h" name="h" />
        <button type="submit">Send</button>
        </form>
    </p>
</asp:Content>
