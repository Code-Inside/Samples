<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="MVCNestedMasterPagesDynamicContent.Views.Account.ChangePassword" %>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Change Password</h2>
    <p>
        Use the form below to change your password. 
    </p>
    <p>
        New passwords are required to be a minimum of <%=Html.Encode(ViewData["PasswordLength"])%> characters in length.
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
        <div>
            <table>
                <tr>
                    <td>Current password:</td>
                    <td>
                        <%= Html.Password("currentPassword") %>
                        <%= Html.ValidationMessage("currentPassword") %>
                    </td>
                </tr>
                <tr>
                    <td>New password:</td>
                    <td>
                        <%= Html.Password("newPassword") %>
                        <%= Html.ValidationMessage("newPassword") %>
                    </td>
                </tr>
                <tr>
                    <td>Confirm new password:</td>
                    <td>
                        <%= Html.Password("confirmPassword") %>
                        <%= Html.ValidationMessage("confirmPassword") %>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Change Password" /></td>
                </tr>
            </table>
        </div>
    <% } %>
</asp:Content>
