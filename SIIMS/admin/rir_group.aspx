<%@ Page Title="Edit Group" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_group.aspx.cs" Inherits="SIIMS.admin.rir_group" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       <div>
    <div  class="pageHeader">Edit Added Groups </div>

 </div>
     <br />
     <asp:ValidationSummary 
            ID="ValidationSummary1" 
            runat="server" 
            HeaderText="Following error occurs....." 
            ShowMessageBox="false" 
            DisplayMode="BulletList" 
            ShowSummary="true"
            BackColor="Snow"
            Width="90%"
            ForeColor="Red"
            Font-Size="X-Large"
            Font-Italic="true"
            />
    <br /><br />
      <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="98%" border="1" style="color:#333333;font-size: large">
            <tr>
                    <td colspan="3" align="center" style="padding:10px;">
                        <asp:LinkButton Text="+ Add New" ID="lnkNew" runat="server" Font-Bold="true" OnClick="NewGroup"></asp:LinkButton>
                    </td>
                </tr>
  <tr style="background-color:#E5E5FE">
      <th align="left" width="25%">Organization</th>   
   <th align="left" width="55%">Group</th>          
      <th >To Do</th>
  </tr>
           
  <asp:ListView ID="lv_group" runat="server" DataKeyNames="DEPT_ID"    OnItemCommand="CommandList" 
            OnItemDataBound="DataBoundList"
            OnItemEditing="EditList"
            OnItemUpdating="UpdateList"
            OnItemCanceling="CancelList"
            OnItemDeleting="DeleteList"
            OnItemInserting="InsertList" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
     
 
<ItemTemplate>
  <tr style=" color:#333333;">
        <td style="text-align:left; vertical-align:top;">
         <%# Eval("org").ToString() %> </td>
       <asp:label runat="server" ID="lblOrgID" Visible="false" Text ='<%# Eval("org_id")%>'></asp:label> </td>
    <td style="text-align:left; vertical-align:top;">
         <%# Eval("dept").ToString() %> </td>
     
       <td  style="text-align:left; vertical-align:top; "> <asp:LinkButton ID="lnkEdit" CommandName="Edit" runat="server" Text="Edit"></asp:LinkButton> &nbsp;&nbsp;
                        <asp:LinkButton ID="lnkDelete" CommandName="Delete" runat="server" Text="Delete"></asp:LinkButton>  </td>
     
  </tr>
</ItemTemplate>
    <EditItemTemplate>
        <tr  style="background-color:lightblue;">
            <td style="text-align:left; vertical-align:top;"> 
            
                <asp:DropDownList ID="drwOrg" AutoPostBack="false" DataSourceID="ds_Org"
                   DataTextField="name" DataValueField="org_id"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList>    </div>
              <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ControlToValidate="drwOrg"  CssClass="errorText" 
                          ErrorMessage="Organization is required!"></asp:RequiredFieldValidator>

            </td>            
  <td style="text-align:left; vertical-align:top;"> 
            
                <asp:TextBox ID="txtDesc" runat="server" Text='<%#Eval("dept")%>'  MaxLength="195" Width="99%"></asp:TextBox>

                <br />
    <asp:RequiredFieldValidator ID="RequiredDesc" ControlToValidate="txtDesc" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Tracking/Trending Code is required."></asp:RequiredFieldValidator>
 
            </td>            
 
           
            <td  style="text-align:center; vertical-align:top; ">
                <asp:LinkButton ID="btnUpdate" CommandName="Update" runat="server" Text="SAVE" CssClass="emphRedText"></asp:LinkButton> &nbsp;  &nbsp; &nbsp;
              <asp:LinkButton ID="btnCancel"  CausesValidation="false" CommandName="Cancel" runat="server" Text="Cancel"  OnClientClick="javascript:return confirm('Are you sure you want to Cancel?');" ></asp:LinkButton>
           
            </td>
       
        </tr>    
        
            
    </EditItemTemplate>
       <InsertItemTemplate>
                <tr style="background-color:lightblue;">
                      <td> 
                      
                <asp:DropDownList ID="drwOrg" AutoPostBack="false" DataSourceID="ds_Org"
                   DataTextField="name" DataValueField="org_id"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList>    </div>
              <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ControlToValidate="drwOrg"  CssClass="errorText" 
                          ErrorMessage="Organization is required!"></asp:RequiredFieldValidator>

                      </td>
                    <td > <asp:TextBox ID="txtDesc" runat="server" 
                       MaxLength="195" Width="99%"></asp:TextBox>

                        <br />
            <asp:RequiredFieldValidator ID="RequiredDesc" ControlToValidate="txtDesc" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Tracking/Trending Code is required."></asp:RequiredFieldValidator>

                    </td>
                    
                   
                    <td>
                        <asp:LinkButton ID="lnkSave" CommandName="Save" runat="server" Text="SAVE" CssClass="emphRedText"></asp:LinkButton> &nbsp;  &nbsp; &nbsp;
                        <asp:LinkButton ID="lnkCancel" CommandName="Cancel" runat="server"  OnClientClick="javascript:return confirm('Are you sure you want to Cancel?');" Text="Cancel" CausesValidation="false"></asp:LinkButton>
                    </td>
                    
                </tr>                
            </InsertItemTemplate>
   </asp:ListView>
      
        </table>
            <asp:SqlDataSource ID="ds_Org" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select org_id, name from SIIMS_ORG where is_active='Y' 
             order by name">
    </asp:SqlDataSource>      
</asp:Content>
