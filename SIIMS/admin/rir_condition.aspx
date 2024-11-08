<%@ Page Title="Edit RIR Conditions" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_condition.aspx.cs" Inherits="SIIMS.admin.rir_condition" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
         <div>
    <div  class="pageHeader">Edit Radiological Posting or Condition List  </div>

 </div>
    <br />
       
      <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="98%" border="1" style="color:#333333;font-size: large">
            <tr>
                    <td colspan="2" align="center" style="padding:10px;">
                        <asp:LinkButton Text="+ Add New" ID="lnkNew" runat="server" Font-Bold="true" OnClick="NewCondition"></asp:LinkButton>
                    </td>
                </tr>
  <tr style="background-color:#E5E5FE">
   <th align="left" width="80%">Radiological Posting or Condition</th>          
      <th >To Do</th>
  </tr>
           
  <asp:ListView ID="lv_condition" runat="server" DataKeyNames="condition_ID"    OnItemCommand="CommandList" 
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
    <td style="width:80%;text-align:left; vertical-align:top;">
         <%# Eval("condition").ToString() %> </td>
     
       <td  style="text-align:left; vertical-align:top; "> <asp:LinkButton ID="lnkEdit" CommandName="Edit" runat="server" Text="Edit"></asp:LinkButton> &nbsp;&nbsp;
                        <asp:LinkButton ID="lnkDelete" CommandName="Delete" runat="server" Text="Delete"></asp:LinkButton>  </td>
     
  </tr>
</ItemTemplate>
    <EditItemTemplate>
        <tr  style="background-color:lightblue;">
  <td style="width:80%;text-align:left; vertical-align:top;"> 
            
                <asp:TextBox ID="txtDesc" runat="server" Text='<%#Eval("condition")%>'  MaxLength="150" Width="95%"></asp:TextBox>

                <br />
    <asp:RequiredFieldValidator ID="RequiredDesc" ControlToValidate="txtDesc" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Radiological Posting or Condition is required."></asp:RequiredFieldValidator>
 
            </td>            
 
           
            <td  style="text-align:center; vertical-align:top; ">
                <asp:LinkButton ID="btnUpdate" CommandName="Update" runat="server" Text="SAVE" CssClass="emphRedText"></asp:LinkButton> &nbsp;  &nbsp; &nbsp;
              <asp:LinkButton ID="btnCancel"  CausesValidation="false" CommandName="Cancel" runat="server" Text="Cancel"  OnClientClick="javascript:return confirm('Are you sure you want to Cancel?');" ></asp:LinkButton>
           
            </td>
       
        </tr>    
        
            
    </EditItemTemplate>
       <InsertItemTemplate>
                <tr style="background-color:lightblue;">
                    <td style="width:80%;"> <asp:TextBox ID="txtDesc" runat="server" 
                       MaxLength="150" Width="95%"></asp:TextBox>

                        <br />
            <asp:RequiredFieldValidator ID="RequiredDesc" ControlToValidate="txtDesc" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Radiological Posting or Condition is required."></asp:RequiredFieldValidator>

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
