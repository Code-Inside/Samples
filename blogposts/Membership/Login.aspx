<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server"> 
    <asp:Login ID="LoginControl" OnAuthenticate="Login_Authenticate" runat="server">
        <LayoutTemplate>
            Login:<br />
            <asp:TextBox ID="UserName" runat="server"></asp:TextBox><br />    
            Password:<br />
            <asp:TextBox TextMode="Password" ID="Password" runat="server"></asp:TextBox>
            <asp:Button ID="Login" runat="server" CommandName="Login" Text="Anmelden" />
        </LayoutTemplate>
    </asp:Login> 
</asp:Content>
