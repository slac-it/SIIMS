<%@ Page Title="Approve Change Request" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="action_iapp.aspx.cs" Inherits="SIIMS.action_iapp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
      <div  class="pageHeader">Approve Action Owner's Request Change</div>
      
    <div style="text-align:left; margin-bottom:15px;">
You can approve or reject the change request. Optionally please provide a note/reason.
        If the request is rejected, the requestor will be notified. 
        <asp:Panel ID="PanelP1" Visible="false" runat="server">
            Since the signicance level of the associated issue is P1, the request will sent to administrator for concur even if you approve it.
        </asp:Panel>
</div>

    <table width="100%" id="editTable">
         <tr>
            <td colspan="2" style="text-align:center;font-weight:bold;">Current Action Attributes</td>
        </tr>
       
         <tr>
        <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">Action ID: </td>
        <td style="width:75%; text-align:left;">
             <asp:Label ID="lblAID" runat="server"  Width="90%"></asp:Label>    
        </td>
        </tr>
      
         <tr>
        <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">Title: </td>
        <td style="width:75%; text-align:left;">
             <asp:Label ID="lblTitle" runat="server"  Width="90%"></asp:Label>    
        </td>
        </tr>
     
        <tr>
        <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">Description: </td>
        <td style="width:75%; text-align:left;">
             <asp:Label ID="lblDesc" runat="server"  Width="90%"></asp:Label><br />
         
        
        </td>
        </tr>
       
          <tr>
        <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">Due Date: </td>
        <td style="width:75%; text-align:left;">
             <asp:Label ID="lblDueDate" runat="server"   Width="90%"></asp:Label>   
           </td>
        </tr> 
        
        
         <tr>
        <td style="width:25%; text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">Owner: <%-- <span  class ="spanred">*</span>--%> </td>
        <td style="width:75%; text-align:left;">
          
            <asp:Label ID="lblOwner" ReadOnly="true"  Text="" runat="server"></asp:Label>   &nbsp; &nbsp;
          
        </td>
        </tr>
       
       

       <asp:Panel ID="PanelEdit" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center;font-weight:bold;font-size:1.2em; ">
            Change Requested: action editing. <br />

        </td>      </tr> 
               <tr>
        <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">New Title: </td>
        <td style="width:75%; text-align:left;">
             <asp:Label ID="lblNewTitle" runat="server"  Width="90%"></asp:Label>    
        </td>
        </tr>
       
        <tr>
        <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">New Description: </td>
        <td style="width:75%; text-align:left;">
             <asp:Label ID="lblNewDesc" runat="server"  Width="90%"></asp:Label><br />
         
        
        </td>
        </tr>
       

        </asp:Panel>

       <asp:Panel ID="PanelDueDate" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center;font-weight:bold;font-size:1.1em; ">
            Change Requested: New Due Date  at <asp:Label ID="lblNewDueDate" runat="server"></asp:Label>    

        </td> </tr> 
             
       

        </asp:Panel>

          <asp:Panel ID="PanelClose" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center;font-weight:bold;font-size:1.2em; ">
            Change Requested: action closure.
             <div style="margin-left:0em;  margin-top: 10px; text-align:center;">
         <table  cellspacing="2" cellpadding="4" align="Center" rules="all"  style=" width:70%;color:#333333;font-size: large;border-collapse: separate;border-spacing:0;">
   <tr style="background-color:#E5E5FE;">
   <th style="text-align:center; font-size:1.2em;" colspan="4">Uploaded Action Supporting Documents</th>
  </tr>
  <tr style="background-color:#E5E5FE;">
   <th align="left">File Name</th>
   <th align="left">Uploaded Date</th>
    <th>Download File</th>
  </tr>
  <asp:ListView ID="lv_File" runat="server" OnItemCommand="CommandList"  OnItemDataBound="DataBoundList" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
<ItemTemplate>
  <tr style="background-color:#F7F6F3; color:#333333;">
      <td>  <%#Eval("FILE_NAME") %> </td>
      <td>  <%#Eval("CREATED_ON") %> </td>
      <td><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> </td>
  </tr>
</ItemTemplate>

    </asp:ListView>
 
 
        </table>

     </div>
        </td>      </tr> 
        </asp:Panel>

          <asp:Panel ID="PanelDelete" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center;font-weight:bold;font-size:1.2em; ">
            Change Requested:  action deletion.
        </td>      </tr> 
        </asp:Panel>
         
         
          <tr>
         <td style="width:25%; text-align:right; font-weight:bold;font-size:1.2em;">Reason: </td>
                 <td style="width:75%; text-align:left;">
               <asp:TextBox ID="txtNote"  Width="95%" Height="80px" TextMode="MultiLine" runat="server"></asp:TextBox> <br />
                       <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtNote"  ValidationGroup="noteValidation"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
         </td> </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
        <tr style="text-align:center;"><td colspan="2">
    <asp:Button ID="btnApprove" runat="server" Text="Approve Request Change" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="noteValidation"
           onclientclick="if(! confirm(&quot; Are you sure you want to approve the change?&quot;)) return false;" OnClick="btnApprove_Click"       /> &nbsp;&nbsp;
      <asp:Button ID="btnReject" runat="server" Text="Reject Request Change" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="noteValidation"
            onclientclick="if(! confirm(&quot; Are you sure you want to reject the change?&quot;)) return false;" OnClick="btnReject_Click"       /> &nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot; Are you sure you want to cancel now?&quot;);" />

            </td></tr>             
 </table>
</asp:Content>
