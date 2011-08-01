<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    OpenID Login
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript" src="../../Scripts/jquery.openid.js"></script>
    <script type="text/javascript">    $(function () { $("form.openid:eq(0)").openid(); });</script>
</asp:Content>


<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <form class="openid" method="post" action="/Home/OpenID"> 
      <div><ul class="providers"> 
      <li class="openid" title="OpenID"><img src="../../Content/images/openidW.png" alt="icon" /> 
      <span><strong>http://{your-openid-url}</strong></span></li> 
      <li class="direct" title="Google"> 
		    <img src="../../Content/images/googleW.png" alt="icon" /><span>https://www.google.com/accounts/o8/id</span></li> 
      <li class="direct" title="Yahoo"> 
		    <img src="../../Content/images/yahooW.png" alt="icon" /><span>http://yahoo.com/</span></li> 
      <li class="username" title="AOL screen name"> 
		    <img src="../../Content/images/aolW.png" alt="icon" /><span>http://openid.aol.com/<strong>username</strong></span></li> 
      <li class="username" title="MyOpenID user name"> 
		    <img src="../../Content/images/myopenid.png" alt="icon" /><span>http://<strong>username</strong>.myopenid.com/</span></li> 
      <li class="username" title="Flickr user name"> 
		    <img src="../../Content/images/flickr.png" alt="icon" /><span>http://flickr.com/<strong>username</strong>/</span></li> 
      <li class="username" title="Technorati user name"> 
		    <img src="../../Content/images/technorati.png" alt="icon" /><span>http://technorati.com/people/technorati/<strong>username</strong>/</span></li> 
      <li class="username" title="Wordpress blog name"> 
		    <img src="../../Content/images/wordpress.png" alt="icon" /><span>http://<strong>username</strong>.wordpress.com</span></li> 
      <li class="username" title="Blogger blog name"> 
		    <img src="../../Content/images/blogger.png" alt="icon" /><span>http://<strong>username</strong>.blogspot.com/</span></li> 
      <li class="username" title="LiveJournal blog name"> 
		    <img src="../../Content/images/livejournal.png" alt="icon" /><span>http://<strong>username</strong>.livejournal.com</span></li> 
      <li class="username" title="ClaimID user name"> 
		    <img src="../../Content/images/claimid.png" alt="icon" /><span>http://claimid.com/<strong>username</strong></span></li> 
      <li class="username" title="Vidoop user name"> 
		    <img src="../../Content/images/vidoop.png" alt="icon" /><span>http://<strong>username</strong>.myvidoop.com/</span></li> 
      <li class="username" title="Verisign user name"> 
		    <img src="../../Content/images/verisign.png" alt="icon" /><span>http://<strong>username</strong>.pip.verisignlabs.com/</span></li> 
      </ul></div> 
      <fieldset> 
      <label for="openid_username">Enter your <span>Provider user name</span></label> 
      <div><span></span><input type="text" name="openid_username" /><span></span> 
      <input type="submit" value="Login" /></div> 
      </fieldset> 
      <fieldset> 
      <label for="openid_identifier">Enter your <a class="openid_logo" href="http://openid.net">OpenID</a></label> 
      <div><input type="text" name="openid_identifier" /> 
      <input type="submit" value="Login" /></div> 
      </fieldset> 
    </form>
</asp:Content>
