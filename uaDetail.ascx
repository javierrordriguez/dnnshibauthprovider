<%@ control AutoEventWireup="false" CodeBehind="uaDetail.ascx.vb" Inherits="UF.Research.Authentication.Shibboleth.uaDetail" Language="vb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<style type="text/css">
    .style1
    {
        height: 28px;
    }
</style>

<asp:TextBox ID="TextBox1" runat="server" 
    Text='<%# DataBinder.Eval( Container, "DataItem.DNNProperty" ) %>' 
    Visible="false" Width="50%">
                        </asp:TextBox>
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
    <ajaxsettings>
        <telerik:AjaxSetting AjaxControlID="amProxy">
            <updatedcontrols>
                <telerik:AjaxUpdatedControl ControlID="Container" />
            </updatedcontrols>
        </telerik:AjaxSetting>
    </ajaxsettings>
</telerik:RadAjaxManagerProxy>
<asp:Panel ID="Panel1" runat="server">
    <table ID="Table2" border="1" cellpadding="1" cellspacing="2" 
        CssClass="RadGrid" rules="none" style="BORDER-COLLAPSE: collapse" width="100%">
        <tr>
            <td>
                <table ID="Table3" border="0" cellpadding="1" cellspacing="1" width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="lblRMID" runat="server" Text="Row ID: " 
                                Visible="<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>" 
                                width="120px"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="labelRMID" runat="server" 
                                Text='<%# DataBinder.Eval( Container, "DataItem.RMID" )%>' 
                                Visible="<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>" 
                                Width="50%">
                        %&gt;
                        </asp:Label>
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="style1">
                            <asp:Label ID="lblType" runat="server" Text="User Property Type: " 
                                width="120px"></asp:Label>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="ddlType" runat="server" onclick="ddlType_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td class="style1">
                            <asp:TextBox ID="txtType" runat="server" 
                                Text='<%# DataBinder.Eval( Container, "DataItem.Type" ) %>' 
                                Visible="false" Width="50%">
                        </asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                        <td>
                            <asp:Label ID="lblProperty" runat="server" Text="DNN Property: " width="120px"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlDNNProperty" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDNNProperty" runat="server" 
                                Text='<%# DataBinder.Eval( Container, "DataItem.DNNProperty" ) %>' 
                                Visible="false" Width="50%">
                        </asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lblSource" runat="server" Text="Shibboleth Source : " 
                                width="120px"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSource" runat="server" 
                                Text='<%# DataBinder.Eval( Container, "DataItem.Source" ) %>' 
                                Visible="true" Width="50%">
                            </asp:TextBox>
                        </td>
                    </tr>
                    
                    <tr>
                    <td>
                        <asp:Label ID="lblOverwrite" runat="server" Text="Overwrite: " width="120px"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkOverwrite" runat="server" />
                    </td>
                    </tr>
                    
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text=" " width="5%"></asp:Label>
                        </td>
                        <td>
                            <asp:Button ID="btnUpdate" runat="server" CommandName="Update" text="Update" 
                                Visible="<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>" />
                            <asp:Button ID="btnInsert" runat="server" CommandName="PerformInsert" 
                                text="Insert" 
                                Visible="<%# (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>" />
                            &nbsp;
                            <asp:Button ID="btnCancel" runat="server" causesvalidation="False" 
                                commandname="Cancel" text="Cancel" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text=" " width="50%"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
