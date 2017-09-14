<%@ Control Language="vb" AutoEventWireup="false"
 CodeBehind="Settings.ascx.vb" 
 Inherits="UF.Research.Authentication.Shibboleth.Settings" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls"
    TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
 
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%> 
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<asp:Panel ID="pnlError" runat="server" Visible="false">
    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/red-error.gif" ImageAlign="Left"  /><asp:Label ID="lblError" runat="server" CssClass="Normal" />
</asp:Panel>
<table id="tblSettings" runat="server" cellspacing="2" cellpadding="2" width="500" summary="Authentication Settings Design Table" border="0">
    <tr valign="top" height="*">
        <td id="MessageCell" align="center" runat="server"></td>
    </tr>    
    <tr>
        <td valign="top" width="500">
            <table id="tblADSettings" cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td width="200"><dnn:label id="plEnabled" runat="server" controlname="chkEnabled" text="Shibboleth Provider Enabled?" /></td>
                    <td valign="top"><asp:CheckBox ID="chkEnabled" runat="server" CssClass="NormalTextBox"></asp:CheckBox></td>
                    <td></td>
                </tr>
                <tr>
                    <td width="200"><dnn:label id="plAutoCreateUsers" runat="server" controlname="chkAutoCreateUsers" text="AutoCreate Users?" /></td>
                    <td valign="top"><asp:CheckBox ID="chkAutoCreateUsers" runat="server" CssClass="NormalTextBox"></asp:CheckBox></td>
                    <td></td>
                </tr>
                <tr>
                    <td width="200"><dnn:label id="plSynchronizeRoles" runat="server" controlname="chkSynchronizeRoles" text="Synchronize Roles?" /></td>
                    <td valign="top"><asp:CheckBox ID="chkSynchronizeRoles" runat="server" CssClass="NormalTextBox"></asp:CheckBox></td>
                    <td></td>
                </tr>
                
                <tr>
                    <td width="200"><dnn:label id="plLogoutPage" runat="server" controlname="txtLogoutPage" text="Logout Page" /></td>
                    <td width="200"><asp:DropDownList ID="ddlLogoutPage" runat="server"></asp:DropDownList></td>
                    <td width="300"><asp:Label ID="lblLogoutPageError" runat="server" Text="" Backcolor="Red" Visible="false"></asp:Label></td>
                    </tr>
                <tr>
                    <td width="200"><dnn:label id="plLoginPage" runat="server" controlname="txtLoginPage" text="Login Page" /></td>
                    <td width="200"><asp:DropDownList ID="ddlLoginPage" runat="server"></asp:DropDownList></td>
                    <td width="300"><asp:Label ID="lblLoginPageError" runat="server" Text="" BackColor="Red" Visible="false"></asp:Label></td>
                </tr>

                <tr>
                    <td width="200"><dnn:label id="plSimulateLogin" runat="server" controlname="txtSimulateLogin" text="Simulate Login" /></td>
                    <td width="200">
                        <asp:CheckBox ID="chkSimulateLogin" runat="server" />
                    </td>
                    <td>
                    <asp:TextBox ID="txtSimLoginTextFile" runat="server" Visible="false" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td width="200"><dnn:label id="plDelimiter" runat="server" controlname="txtDelimiter" text="Delimiter" Enabled="false" visible="false" /></td>
                    <td valign="top"><asp:Textbox  ID="txtDelimiter" runat="server" CssClass="NormalTextBox" Width="10" BorderStyle="Double" Enabled="false" visible="false"  ></asp:TextBox></td>
                </tr>
                
            </table>
        </td>
    </tr>
</table>


