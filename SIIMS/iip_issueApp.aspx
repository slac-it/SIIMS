<%@ Page Title="Concur Change Request" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="iip_issueApp.aspx.cs" Inherits="SIIMS.iip_issueApp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--  <script type="text/javascript" src="scripts/backbutton.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    

      <div  class="pageHeader">Concur Request  Change</div>
      
    <br />
    <br />
     <div style="clear:both;"></div>
    <div style="text-align:left;">
You can concur or reject the change request. Optionally please provide a note/reason.
      For P1 closure request, the issue owner will be notified your decision.
        For others, the requestor will be notified only when rejected. 
</div>
    <br />

    <table width="100%" id="editTable">
         <tr>
            <td colspan="2" style="text-align:center;font-weight:bold;">Current Issue Attributes</td>
        </tr>
        
      
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;font-size:1.2em;">Issue ID: </td>
        <td style=" text-align:left;">
             <asp:Label ID="lblIID" runat="server"  Width="90%"></asp:Label>    
        </td>
        </tr>
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;font-size:1.2em;">Title: </td>
        <td style=" text-align:left;">
             <asp:Label ID="lblTitle" runat="server"  Width="90%"></asp:Label>    
        </td>
        </tr>
       <tr >
        <td style="width:15%; text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">Description:</td>
        <td style=" text-align:left;">
              <asp:Label ID="lblDesc" runat="server" Text=""></asp:Label>
        
        </td>
        </tr>

        
          <tr>
        <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Organization: </td>
        <td style=" text-align:left;">
             <asp:Label ID="lblOrg" runat="server"   Width="90%"></asp:Label>   
           </td>
        </tr> 
         
         <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">Owner: <%-- <span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">
          
            <asp:Label ID="lblOwner" ReadOnly="true"  Text="" runat="server"></asp:Label>   &nbsp; &nbsp;
          
        </td>
        </tr>
      
         
          <tr>
        <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Level:  </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblLevel" ReadOnly="true"  Text="" runat="server"></asp:Label> 
          
           </td>
          
        </tr> 
          

          <tr>
        <td style="text-align:right; font-weight:bold; vertical-align:top; font-size:1.2em;">Source:  </td>
        <td style=" text-align:left;">
              <asp:Label ID="lblSType" Text="" runat="server"></asp:Label>   &nbsp; &nbsp;
         
           </td>
          
        </tr> 
        <asp:Panel ID="PanelSourceTitle" Visible="false" runat="server">
           
          <tr>
         <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Source Title: <span  class ="spanred">*</span> </td>
               <td style=" text-align:left;">
                   <asp:TextBox ID="txtSourceTitle" ReadOnly="true" runat="server"></asp:TextBox> <br />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ControlToValidate="txtSourceTitle"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Source Title is required."></asp:RequiredFieldValidator>
         </td> </tr>
            
          <tr>
         <td style="text-align:right; font-weight:bold;font-size:1.2em;">Source FY: <span  class ="spanred">*</span> </td>
                 <td style=" text-align:left;">
               <asp:TextBox ID="txtSourceFY" ReadOnly="true" runat="server"></asp:TextBox> <br />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="txtSourceFY"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Source FY is required."></asp:RequiredFieldValidator>
         </td> </tr>
            
          <tr>
         <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Source Quarter: <span  class ="spanred">*</span> </td>
              <td style="text-align:left;">
                  <asp:TextBox ID="txtSourceQtr" ReadOnly="true" runat="server"></asp:TextBox>  <br />
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator9" ControlToValidate="txtSourceQtr"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Source Quarter is required."></asp:RequiredFieldValidator>
         </td> </tr>
            
        </asp:Panel>
           
        <asp:Panel ID="PanelDeletion" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center; font-weight:bold;font-size:1.2em;">
            Change Requested: Deletion
        </td>      </tr> 
        </asp:Panel>
         <asp:Panel ID="PanelNewLevel" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center;font-weight:bold;font-size:1.2em; ">
            Change Requested: change Level to <asp:Label ID="lblNewLevel" runat="server" Text=""></asp:Label>
        </td>      </tr> 
        </asp:Panel>
         <asp:Panel ID="PanelP1Closure" Visible="false" runat="server">
             <tr>
        <td colspan="2" style=" text-align:center; font-weight:bold;font-size:1.2em;">
            Change Requested: Close P1 Issue
        </td>      </tr> 
        </asp:Panel>
          <tr>
         <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Reason: </td>
                 <td style="text-align:left;">
               <asp:TextBox ID="txtNote"  Width="95%" Height="80px" TextMode="MultiLine" runat="server"></asp:TextBox> <br />
                       <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtNote"  ValidationGroup="noteValidation"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
         </td> </tr>
       
        <tr><td colspan="2" style="text-align:center;">
    <asp:Button ID="btnApprove" runat="server" Text="Concur Request" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="noteValidation"
           onclientclick="if(! confirm(&quot; Are you sure you want to concur the change?&quot;)) return false;" OnClick="btnApprove_Click"       /> &nbsp;&nbsp;
      <asp:Button ID="btnReject" runat="server" Text="Reject Request" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="noteValidation"
           onclientclick="if(! confirm(&quot; Are you sure you want to reject the change?&quot;)) return false;" OnClick="btnReject_Click"       /> &nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot; Are you sure you want to cancel now?&quot;);" />

            </td></tr>             
 </table>
       <br /><br />
</asp:Content>
