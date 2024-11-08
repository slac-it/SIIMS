<%@ Page Title="Edit Tracking Code" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_code.aspx.cs" Inherits="SIIMS.admin.rir_code" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div>
    <div  class="pageHeader">Edit Tracking/Trending Codes </div>

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
                        <asp:LinkButton Text="+ Add New" ID="lnkNew" runat="server" Font-Bold="true" OnClick="NewCondition"></asp:LinkButton>
                    </td>
                </tr>
  <tr style="background-color:#E5E5FE">
      <th align="left" width="15%">Tracking/Trending Category</th>   
   <th align="left" width="70%">Tracking/Trending Code</th>          
      <th >To Do</th>
  </tr>
           
  <asp:ListView ID="lv_condition" runat="server" DataKeyNames="RIRCODE_ID"    OnItemCommand="CommandList" 
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
         <%# Eval("CATEGORY").ToString() %> </td>
    <td style="text-align:left; vertical-align:top;">
         <%# Eval("code").ToString() %> </td>
     
       <td  style="text-align:left; vertical-align:top; "> <asp:LinkButton ID="lnkEdit" CommandName="Edit" runat="server" Text="Edit"></asp:LinkButton> &nbsp;&nbsp;
                        <asp:LinkButton ID="lnkDelete" CommandName="Delete" runat="server" Text="Delete"></asp:LinkButton>  </td>
     
  </tr>
</ItemTemplate>
    <EditItemTemplate>
        <tr  style="background-color:lightblue;">
            <td style="text-align:left; vertical-align:top;"> 
                <asp:Literal ID="ltCat" Text='<%#Eval("CATEGORY")%>'  runat="server"></asp:Literal>
            </td>            
  <td style="text-align:left; vertical-align:top;"> 
            
                <asp:TextBox ID="txtDesc" runat="server" Text='<%#Eval("code")%>'  MaxLength="195" Width="99%"></asp:TextBox>

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
                      <td> <asp:TextBox ID="txtCat" runat="server" 
                       MaxLength="49" Width="99%"></asp:TextBox>

                        <br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtCat" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: RaTracking/Trending Category is required."></asp:RequiredFieldValidator>

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
     
</asp:Content>
