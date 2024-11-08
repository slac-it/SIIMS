<%@ Page Title="Change Action Owner" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="action_changeOwner.aspx.cs" Inherits="SIIMS.action_changeOwner" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <div  class="pageHeader"> Change Action Owner</div>
<div style="text-align:left; font-size:1em; margin-bottom:15px;">
<span style="font-weight:bold;">Source:</span> <asp:Label ID="lblSourceTitle" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Source Type:</span> <asp:Label ID="lblSType" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Source Date:</span> <asp:Label ID="lblSourceFY" runat="server" Text=""></asp:Label>-Q<asp:Label ID="lblSourceQtr" runat="server" Text=""></asp:Label> <br />
</div>
<div style="text-align:left; font-size:1em; margin-bottom:15px;">
<span style="font-weight:bold;">Issue ID: </span>
    <asp:Label ID="lblIssueID" runat="server" Text=""></asp:Label>

<span style="font-weight:bold; padding-left:20px;">Level: </span>
    <asp:Label ID="lblLevel" runat="server" Text=""></asp:Label>

<span style="font-weight:bold;padding-left:20px;">Organization: </span>
    <asp:Label ID="lblOrg" runat="server" Text=""></asp:Label>

<span style="font-weight:bold;padding-left:20px;">Owner: </span>
    <asp:Label ID="lblIOwner" runat="server" Text=""></asp:Label>

<br />
<span style="font-weight:bold;">Issue Title:</span> <asp:Label ID="lblITitle" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Issue Description:</span>  
<div style=" border: solid black 0px; margin-left:20px;">
     <asp:Label ID="lblIDescShort" Visible="false" runat="server" Text=""></asp:Label> 
    <asp:Label ID="lblIDesc" CssClass="" runat="server" Text=""></asp:Label> 
</div>
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
   
        <tr><td style="height:15px;" colspan="2"></td></tr>
          <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;">Owner:  </td>
        <td style=" text-align:left;">
           <asp:Label ID="lblAOwner" runat="server" Text=""></asp:Label>

        </td>
        </tr>
         <tr >
        <td style=" text-align:right; font-weight:bold;">Due Date: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblDue" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 

        <%--  <tr >
        <td style=" text-align:right; font-weight:bold;">Status: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> --%>
         <tr><td style="height:15px;" colspan="2"></td></tr>
             
      <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;">New Owner: <%-- <span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">
             <asp:Panel ID="PnlPopup" Visible="true" runat="server">
             <div style="text-align:left; margin-bottom:10px;">
                 <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                        Please enter the first few characters of the owner's last name: </asp:label>
                <br />
                  <asp:textbox id="txtOwner"  runat="server" Width="200px" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click"  Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
          
                <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica">Label</asp:Label>
             &nbsp; &nbsp; &nbsp; 
                 <asp:dropdownlist id="ddlEmplist" runat="server" Visible="False" TabIndex="3"></asp:dropdownlist>
              &nbsp; &nbsp;
                 <asp:button id="cmdOk" runat="server" Width="72px" Text="OK" Visible="False"   Font-Bold="true" Font-Size="Large"
                            TabIndex="4" onclick="cmdOk_Click"></asp:button>                    
             
            </div>  
           </asp:Panel> 
            <asp:TextBox ID="txtOwner_Name" ReadOnly="true" Visible="false"  Text="" runat="server"></asp:TextBox>   &nbsp; &nbsp;
              <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtOwner_Name" ValidationGroup="allFields"
                   runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Please find and select a new action owner."></asp:RequiredFieldValidator>
            <asp:HiddenField ID="txtOwner_SID" runat="server" Value="-1" />
           </td></tr>   
        

        <tr><td colspan="2"  style="text-align:center;">
            <asp:Label ID="lblLockout" CssClass="errorText" Visible="false" runat="server" Text=""></asp:Label> <br />  
    <asp:Button ID="btnChange" runat="server" Text="Submit" Font-Bold="true" Font-Size="X-Large"  ValidationGroup="allFields"
           onclientclick="return confirm(&quot; Are you sure you want to make the change?&quot;);" OnClick="btnChange_Click"    /> &nbsp;&nbsp;
    
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot; Are you sure you want to cancel now?&quot;);" />

            </td></tr> 
         
</table>
</td>
 <td  style="width:25%; text-align:left;vertical-align:top; " >
     Attachments:<br />
     <asp:Label ID="Label3" Visible="false" runat="server" Text=""></asp:Label>
  <ul>
  <asp:ListView ID="lv_Files" runat="server" OnItemCommand="CommandList"  >
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


   <tr>
       

           

        </td>
        </tr>
        <tr>
           <td colspan="2" style="height:10px;">&nbsp;

           
           </td>
        </tr>  

        <tr><td colspan="2"  style="text-align:center;">
             

            </td></tr>             
 </table>
</asp:Content>
