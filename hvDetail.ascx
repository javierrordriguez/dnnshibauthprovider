<%@ Control Language="vb"
 AutoEventWireup="false"
  CodeBehind="hvDetail.ascx.vb"
  Inherits="UF.Research.Authentication.Shibboleth.hvDetail" %>

                

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
                        
                        
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
  <AjaxSettings>
    <telerik:AjaxSetting AjaxControlID="amProxy">
      <UpdatedControls>
        <telerik:AjaxUpdatedControl ControlID="Container" />
      </UpdatedControls>
    </telerik:AjaxSetting>
  </AjaxSettings>
</telerik:RadAjaxManagerProxy>                        

<asp:Panel ID="Panel2" runat="server">

<table id="Table2" cellspacing="2" cellpadding="1" width="100%" border="1" rules="none"
    style="BORDER-COLLAPSE: collapse" CssClass="RadGrid">
    
    <tr >
    
        <td>
            <table id="Table3" cellspacing="1" cellpadding="1" width="100%" border="0">
               
                <tr>
                    <td>
                        <asp:Label ID="lblRMID" runat="server" Text="Row ID: " width="120px"  Visible="false" ></asp:Label>
                    </td>
                                                                              
                    <td> 
                        <asp:Label id="labelRMID" runat="server" Width="50%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.RMID" )%>' Visible="false">
                        %>
                        </asp:Label>
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblShibHdrVarName" runat="server" Text="Shibboleth Header Variable Name: " width="120px"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox id="txtSHIBHdrVarName" runat="server" Width="50%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.ShibHdrVarName" ) %>' Visible="true">
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSHIBHdrVarDelim" runat="server" Text="Shibboleth Header Variable Delimiter: " width="120px"></asp:Label>
                    </td> 
                    <td> 
                        <asp:TextBox id="txtSHIBHdrVarDelim" runat="server" Width="10%"
                        Text='<%# DataBinder.Eval( Container, "DataItem.ShibHdrVarDelim" ) %>' 
                            Visible="true" MaxLength="1"></asp:textbox>
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
        
    </table>

</asp:Panel> 

                  
   
                  
