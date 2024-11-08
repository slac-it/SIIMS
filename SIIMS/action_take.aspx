<%@ Page Title="Accept Action Owner Assignment" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="action_take.aspx.cs" Inherits="SIIMS.action_take" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <div  class="pageHeader">Accept or Reject Action Owner Assignment</div>
      
    <div style="text-align:left; margin-bottom:15px;">
        You are assigned an action. Once you accept, you will become the action owner.
You can approve or reject the request.  If the assignment is rejected, the requestor will be notified. 
</div>
<table id="editTable" cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: 1.1em; ">
  <tr>
  <td style="width:70%">
    <table width="100%" class="viewTable">    
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;">Action ID: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblAID2" runat="server" Text=""></asp:Label>
           
        </td>
        </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr>
        <td style=" text-align:right; font-weight:bold;">Title: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblATitle" runat="server" Text="Label"></asp:Label>
           
        </td>
        </tr>
        <tr >
        <td style="text-align:right;vertical-align:top; font-weight:bold;">Description:</td>
        <td style=" text-align:left;">
              <asp:Label ID="lblADesc" runat="server" Text=""></asp:Label>
        
        </td>
        </tr>
   
      
         <tr >
        <td style=" text-align:right; font-weight:bold;">Due Date: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblDue" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 

         <tr><td style="height:15px;" colspan="2"></td></tr>
          <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;">Requestor:  </td>
        <td style=" text-align:left;">
           <asp:Label ID="lblAOwner" runat="server" Text=""></asp:Label>

        </td>
        </tr>
        
</table>
</td>
 <td  style="width:25%; text-align:left;vertical-align:top; " >
     Attachments:<br />
     <asp:Label ID="lblMsg" Visible="false" runat="server" Text=""></asp:Label>
  <ul>
  <asp:ListView ID="lv_Files" runat="server" OnItemCommand="CommandList"  OnItemDataBound="DataBoundList">
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
<ItemTemplate>
    <li><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton></li>
</ItemTemplate>

    </asp:ListView>
</ul>
      </td>
  </tr>


</table>

 <div style="text-align:center; margin-top:15px;">
     <table width="98%">
         <tr>
         <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Note: </td>
                 <td style=" text-align:left;">
               <asp:TextBox ID="txtNote"  Width="95%" Height="80px" TextMode="MultiLine" runat="server"></asp:TextBox> <br />
              <asp:RequiredFieldValidator ID="RequiredNote" ControlToValidate="txtNote"  ValidationGroup="rejectionValidation" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Note is required."></asp:RequiredFieldValidator>
                       <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtNote"  ValidationGroup="noteValidation"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
              <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server" Display="Dynamic"
                                        ControlToValidate="txtNote"  ValidationGroup="rejectionValidation"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
         </td> </tr>
          <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr><td colspan="2" style="text-align:center;">
    <asp:Button ID="btnApprove" runat="server" Text="Accept Assignment" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="noteValidation"
          onclientclick="if(! confirm(&quot; Are you sure you want to accept the request?&quot;)) return false;" OnClick="btnApprove_Click"       /> &nbsp;&nbsp;
      <asp:Button ID="btnReject" runat="server" Text="Reject Assignment" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="rejectionValidation"
           onclientclick="if(! confirm(&quot; Are you sure you want to reject the request?&quot;)) return false;"  OnClick="btnReject_Click"       /> &nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot; Are you sure you want to cancel now?&quot;);" />
             </td></tr>

     </table>
   </div>       
              
</asp:Content>
