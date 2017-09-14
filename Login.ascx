<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Login.ascx.vb" Inherits="UF.Research.Authentication.Shibboleth.Login" %>

<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

<asp:Label ID="lblShibEnabled" runat="server" Text="Shibboleth Logon" resourceKey="lblShibEnabled"> </asp:Label>
<asp:Label ID="lblShibNotEnabled" runat="server" Text="Shibboleth is not Enabled" resourceKey="lblShibNotEnabled"></asp:Label>
<br />
<asp:button id="cmdLogin" resourcekey="cmdLogin"  text="ShibLogin" runat="server" />











