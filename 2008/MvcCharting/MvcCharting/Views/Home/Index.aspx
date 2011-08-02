<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MvcCharting.Views.Home.Index" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        <asp:chart id="Chart1" runat="server" Height="296px" Width="412px" Palette="BrightPastel" imagetype="Png" BorderDashStyle="Solid" BackSecondaryColor="White" BackGradientStyle="TopBottom" BorderWidth="2" backcolor="#D3DFF0" BorderColor="26, 59, 105">
		    <Titles>
			    <asp:Title Text="With datasource in code behind" />
			</Titles>
		    <legends>
				<asp:Legend IsTextAutoFit="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold"></asp:Legend>
			</legends>
			<borderskin skinstyle="Emboss"></borderskin>
			<series>
				<asp:Series Name="Column" BorderColor="180, 26, 59, 105">

				</asp:Series>
			</series>
			<chartareas>
				<asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
					<area3dstyle Rotation="10" perspective="10" Inclination="15" IsRightAngleAxes="False" wallwidth="0" IsClustered="False"></area3dstyle>
					<axisy linecolor="64, 64, 64, 64">
						<labelstyle font="Trebuchet MS, 8.25pt, style=Bold" />
						<majorgrid linecolor="64, 64, 64, 64" />
					</axisy>
					<axisx linecolor="64, 64, 64, 64">
						<labelstyle font="Trebuchet MS, 8.25pt, style=Bold" />
						<majorgrid linecolor="64, 64, 64, 64" />
					</axisx>
				</asp:ChartArea>
			</chartareas>
		</asp:chart>
        </p>
        <p>
        <%
						System.Web.UI.DataVisualization.Charting.Chart Chart2 = new System.Web.UI.DataVisualization.Charting.Chart();
                        Chart2.Width = 412;
                        Chart2.Height = 296;
                        Chart2.RenderType = RenderType.ImageTag;

                        Chart2.Palette = ChartColorPalette.BrightPastel;
                        Title t = new Title("No Code Behind Page", Docking.Top, new System.Drawing.Font("Trebuchet MS", 14, System.Drawing.FontStyle.Bold), System.Drawing.Color.FromArgb(26, 59, 105));
                        Chart2.Titles.Add(t);
                        Chart2.ChartAreas.Add("Series 1");

						// create a couple of series
                        Chart2.Series.Add("Series 1");
                        Chart2.Series.Add("Series 2");
						
						// add points to series 1
                        foreach (int value in (List<int>)ViewData["Chart"])
                        {
                            Chart2.Series["Series 1"].Points.AddY(value); 
                        }

                        // add points to series 2
                        foreach (int value in (List<int>)ViewData["Chart"])
                        {
                            Chart2.Series["Series 2"].Points.AddY(value + 1);
                        }

                        Chart2.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
                        Chart2.BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);
                        Chart2.BorderlineDashStyle = ChartDashStyle.Solid;
                        Chart2.BorderWidth = 2;

                        Chart2.Legends.Add("Legend1");

						// Render chart control
                        Chart2.Page = this;
						HtmlTextWriter writer = new HtmlTextWriter(Page.Response.Output);
						Chart2.RenderControl(writer);
					
                     %>
        </p>
        <p>
        To learn more about ASP.NET MVC visit <a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a>.
        </p>
</asp:Content>
