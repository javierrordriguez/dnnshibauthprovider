<%@ Control Language="vb"
    AutoEventWireup="false"
    CodeBehind="ViewRoleMappings.ascx.vb"
    Explicit="true"
    Inherits="UF.Research.Authentication.Shibboleth.ViewRoleMappings" %>
 

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<div class="center3">
    
    
<asp:panel id="pnlAddShibHdrVar" runat="server">
 
    <p>
    <asp:CheckBox ID="chkShibEnabled" runat="server" Text="Shibboleth Enabled?" enabled="false"/>
    <asp:CheckBox ID="chkShibSimulation" runat="server" Text="Shibboleth Simulation?" enabled="false"/>
    </p>
    
    <p>
        <asp:Label ID="lblShibUserName" runat="server" Text="Shibboleth UserName Variable: "></asp:Label>
        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
        <br /> <br />
        <asp:Button ID="btnUpdateUserName" runat="server" Text="Update Shibboleth UserName Variable" />
    </p>
    
    <p></p>
    
    <telerik:RadGrid ID="RadGrid30" runat="server" GridLines="None" AllowPaging="false"
        AllowSorting="True" AutoGenerateColumns="False" ShowStatusBar="true" CssClass="RadGrid">
        <MasterTableView Width="100%" CommandItemDisplay="Top" DataKeyNames="RMID" EditMode="EditForms" >
             
            <Columns>
                        
                <Telerik:GridEditCommandColumn UniqueName="EditCommandColumn"  ItemStyle-Width="30px"></Telerik:GridEditCommandColumn>
                <Telerik:GridButtonColumn UniqueName="DeleteColumn" Text="Delete" CommandName="Delete" ItemStyle-Width="30px"/>

                <telerik:GridBoundColumn UniqueName="RMID" DataField="RMID" visible="false">
                    <HeaderStyle Width="10px"></HeaderStyle>
                </telerik:GridBoundColumn>
                            
                <telerik:GridBoundColumn UniqueName="SHIBHdrVarName"  DataField="SHIBHdrVarName">
                    <HeaderStyle Width="30px"></HeaderStyle>
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn UniqueName="SHIBHdrVarDelim"  DataField="SHIBHdrVarDelim">
                    <HeaderStyle Width="30px"></HeaderStyle>
                </telerik:GridBoundColumn> 
                     
            </Columns>

                <EditFormSettings UserControlName="hvDetail.ascx" EditFormType="WebUserControl">
                    <EditColumn UniqueName="EditCommandColumn1">
                    </EditColumn>
                    
                </EditFormSettings>
            <ExpandCollapseColumn ButtonType="ImageButton" Visible="False" UniqueName="ExpandColumn">
                <HeaderStyle Width="19px"></HeaderStyle>
            </ExpandCollapseColumn>
 
        </MasterTableView>

    </telerik:RadGrid><br />


    <br /> 

    <asp:Button ID="btnUpdateShibHdrVar" runat="server" Text="Update Shibboleth Header Variable" />
    <br />
    <br />   
   </asp:panel>
      
</div>

<div class="center">
    
<asp:panel id="pnlAddTask" runat="server">

    <telerik:RadGrid ID="RadGrid10" runat="server" GridLines="None" AllowPaging="false"
        AllowSorting="True" AutoGenerateColumns="False" ShowStatusBar="true" CssClass="RadGrid">
        <MasterTableView Width="100%" CommandItemDisplay="Top" DataKeyNames="RMID" EditMode="EditForms" >
             
            <Columns>
                        
                <Telerik:GridEditCommandColumn UniqueName="EditCommandColumn"  ItemStyle-Width="30px"></Telerik:GridEditCommandColumn>
                <Telerik:GridButtonColumn UniqueName="DeleteColumn" Text="Delete" CommandName="Delete" ItemStyle-Width="30px"/>

                <telerik:GridBoundColumn UniqueName="RMID" DataField="RMID" visible="false">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn>
                            
                <telerik:GridBoundColumn UniqueName="DNNRoleName"  DataField="DNNRoleName">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn UniqueName="SHIBRoleType"  DataField="SHIBRoleType">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn> 

                     
            <telerik:GridTemplateColumn DataField="ShibRoleName" HeaderText="ShibRoleName" 
                  UniqueName="ShibRoleName">
                 <ItemTemplate>
                 <asp:TextBox ID="ShibRoleName" runat="server" Text='<%# Eval("ShibRoleName") %>'
                 TextMode="MultiLine" width="500px" readonly="true"></asp:TextBox>
                 </ItemTemplate>
            </telerik:GridTemplateColumn>           
                              
            </Columns>

                <EditFormSettings UserControlName="rmDetail.ascx" EditFormType="WebUserControl">
                    <EditColumn UniqueName="EditCommandColumn1">
                    </EditColumn>
                    
                </EditFormSettings>
            <ExpandCollapseColumn ButtonType="ImageButton" Visible="False" UniqueName="ExpandColumn">
                <HeaderStyle Width="19px"></HeaderStyle>
            </ExpandCollapseColumn>
 
        </MasterTableView>

    </telerik:RadGrid><br />


    <br /> 

    <asp:Button ID="btnUpdateRoleMappings" runat="server" Text="Update Role Mappings" />
    <br />
       
   </asp:panel>
      
</div>


<div class="center2">
    
<br />

<asp:panel id="pnlAddUserAttribute" runat="server">
    
    <telerik:RadGrid ID="RadGrid20" runat="server" GridLines="None" AllowPaging="false"
        AllowSorting="True" AutoGenerateColumns="False" ShowStatusBar="true" CssClass="RadGrid">
        <MasterTableView Width="100%" CommandItemDisplay="Top" DataKeyNames="RMID" EditMode="EditForms" >
             
            <Columns>
                        
                <Telerik:GridEditCommandColumn UniqueName="EditCommandColumn"  ItemStyle-Width="30px"></Telerik:GridEditCommandColumn>
                <Telerik:GridButtonColumn UniqueName="DeleteColumn" Text="Delete" CommandName="Delete" ItemStyle-Width="30px"/>

                <telerik:GridBoundColumn UniqueName="RMID"  DataField="RMID" visible="false">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn>
                            
                <telerik:GridBoundColumn UniqueName="Type"  DataField="Type">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn UniqueName="DNNProperty"  DataField="DNNProperty">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn> 

                <telerik:GridBoundColumn UniqueName="Source"  DataField="Source" >
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn>        
                     
                             
                <telerik:GridTemplateColumn  DataField="uaOverwrite"  HeaderText="Overwrite" UniqueName="uaOverwrite">  
                        <HeaderStyle Width="20px"></HeaderStyle>
                        <ItemTemplate >  
                            <asp:CheckBox ID="uaChkOverwrite" runat="server" enabled="false"/>  
                        </ItemTemplate>  
                </telerik:GridTemplateColumn>  
                 
                
                <telerik:GridBoundColumn UniqueName="Overwrite"  DataField="Overwrite" visible=false>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </telerik:GridBoundColumn> 
                           
                                       
            </Columns>

                <EditFormSettings UserControlName="uaDetail.ascx" EditFormType="WebUserControl">
                    <EditColumn UniqueName="EditCommandColumn1">
                    </EditColumn>
                    
                </EditFormSettings>
            <ExpandCollapseColumn ButtonType="ImageButton" Visible="False" UniqueName="ExpandColumn">
                <HeaderStyle Width="19px"></HeaderStyle>
            </ExpandCollapseColumn>
 
        </MasterTableView>

    </telerik:RadGrid><br />
    
    <br /> 
    
    <asp:Button ID="btnUpdateAttributes" runat="server" Text="Update User Attributes" />
    <br />
       
</asp:panel>

<br />

</div>

