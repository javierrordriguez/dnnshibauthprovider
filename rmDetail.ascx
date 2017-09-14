<%@ Control Language="vb"
 AutoEventWireup="false"
  CodeBehind="rmDetail.ascx.vb"
   Inherits="UF.Research.Authentication.Shibboleth.rmDetail" %>
   

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
                         <asp:TextBox id="TextBox1" runat="server" Width="50%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.DNNRoleName" ) %>' Visible="false">
                        </asp:TextBox>
                        
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
  <AjaxSettings>
    <telerik:AjaxSetting AjaxControlID="amProxy">
      <UpdatedControls>
        <telerik:AjaxUpdatedControl ControlID="Container" />
      </UpdatedControls>
    </telerik:AjaxSetting>
  </AjaxSettings>
</telerik:RadAjaxManagerProxy>                        

<asp:Panel ID="Panel1" runat="server">

<table id="Table2" cellspacing="2" cellpadding="1" width="100%" border="1" rules="none"
    style="BORDER-COLLAPSE: collapse" CssClass="RadGrid">
    
    <tr >
    
        <td>
            <table id="Table3" cellspacing="1" cellpadding="1" width="100%" border="0">
               
                <tr>
                    <td>
                        <asp:Label ID="lblRMID" runat="server" Text="Row ID: " width="120px"  Visible='<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>' ></asp:Label>
                    </td>
                                                                              
                    <td> 
                        <asp:Label id="labelRMID" runat="server" Width="50%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.RMID" )%>' Visible='<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>'>
                        %>
                        </asp:Label>
                    </td>
                </tr>


                <tr>
                    <td>
                        <asp:Label ID="lblDNNRole" runat="server" Text="DNN Role: " width="120px"></asp:Label>
                    </td>
                    
                    <td>
                         <asp:DropDownList id="ddlDNNRoles" runat="server"></asp:DropDownList>
                    </td>
                   
                    <td> 
                        <asp:TextBox id="txtDNNRoleName" runat="server" Width="50%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.DNNRoleName" ) %>' Visible="false">
                        </asp:textbox>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Label ID="lblShibType" runat="server" Text="Shibboleth Role Type: " width="120px"></asp:Label>
                    </td>
                    <td>
                         <asp:DropDownList id="ddlSHIBType" runat="server"></asp:DropDownList>
                    </td>
                    <td> 
                        <asp:TextBox id="txtShibRoleType" runat="server" Width="50%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.ShibRoleType" ) %>' Visible="false">
                        </asp:textbox>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Label ID="lblShibRoleName" runat="server" Text="Shibboleth Role Name: " width="120px"></asp:Label>
                    </td>
                    <td>
                          <asp:TextBox id="txtSHIBRoleName" runat="server" TextMode="MultiLine" width="500px"
                        Text='<%# DataBinder.Eval( Container, "DataItem.ShibRoleName" ) %>' Visible="true">
                        </asp:textbox>
                    </td>
                    
                    <td> 
                        <asp:TextBox id="txtSHIBRName" runat="server" Width="50%" 
                        Text='<%# DataBinder.Eval( Container, "DataItem.ShibRoleName" ) %>' Visible="false">
                        </asp:textbox>
                    </td>
                </tr>
               
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text=" " width="5%"></asp:Label>
                   
                    </td>
                    <td>
                        <asp:button id="btnUpdate" text="Update"  runat="server" CommandName="Update" Visible='<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>'></asp:button>
                   
                        <asp:button id="btnInsert" text="Insert" runat="server" CommandName="PerformInsert" Visible='<%# (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>'></asp:button>&nbsp;
                  
                        <asp:button id="btnCancel" text="Cancel" runat="server" causesvalidation="False" commandname="Cancel"></asp:button>
                    </td>
                     <td>
                        <asp:Label ID="Label1" runat="server" Text=" " width="50%"></asp:Label>
                   
                    </td>
                </tr>
              
            </table>
        
    <tr>

    </tr>
</table>

</asp:Panel> 
   
                  