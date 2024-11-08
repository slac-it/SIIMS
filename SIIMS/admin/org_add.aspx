<%@ Page Title="Add SMT Org" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="org_add.aspx.cs" Inherits="SIIMS.admin.org_add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div  class="pageHeader"> Add New SMT Organization</div>

    <table width="100%" id="editTable">
      
        <tr>
            <td colspan="2" style="text-align:center;"><span class="spanrequired"><span  class ="spanred">*</span> Required Fields </span></td>
        </tr>
         <tr  style="margin-bottom:15px;">
        <td style="width:20%; text-align:right; font-weight:bold;font-size:1.2em;">Org name:   <span  class ="spanred">*</span>  </td>
        <td style=" text-align:left;">
             <asp:TextBox ID="txtOrgName" runat="server" onkeypress="return this.value.length<=150" onpaste="return this.value.length<=150"
                        TextMode="SingleLine" MaxLength="150" width="70%"></asp:TextBox>
           <br />
          <asp:RequiredFieldValidator ID="RequiredFD1" ControlToValidate="txtOrgName"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Org Name is required."></asp:RequiredFieldValidator>
        </td>
        </tr>
         <tr>
            <td colspan="2" style="height:20px;"></td>
        </tr>
        <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">Manager's SLAC ID:  <span  class ="spanred">*</span> </td>
        <td style=" text-align:left;">
            <asp:TextBox ID="txtManager_SID" TextMode="SingleLine" MaxLength="12"  Width="15em" Visible="true" Text="" runat="server"></asp:TextBox>   
            &nbsp; <asp:Label ID="lblManager" runat="server" ForeColor="Red" Visible="false" Text=""></asp:Label>
            <asp:RequiredFieldValidator ID="RequiredFD2" ControlToValidate="txtManager_SID" ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Manager is required."></asp:RequiredFieldValidator>
                     <asp:CompareValidator runat="server" Operator="DataTypeCheck" Type="Integer"  ValidationGroup="allFields" ForeColor="Red" Display="Dynamic"
 ControlToValidate="txtManager_SID" ErrorMessage="Error: Manager's SLAC ID must be a whole number" /> 
        </td></tr>
        <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">Manager's Name: </td>
        <td style=" text-align:left;">
            <asp:TextBox ID="txtManager" TextMode="SingleLine"  BackColor="DimGray" ForeColor="White" Width="50%" ReadOnly="true"  Visible="true" Text="" runat="server"></asp:TextBox>         
        </td></tr>
           <tr>
            <td colspan="2" style="height:20px;"></td>
        </tr>
       <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">POC's SLAC ID:  <span  class ="spanred">*</span> </td>
        <td style=" text-align:left;">
            <asp:TextBox ID="txtPOC_SID" TextMode="SingleLine" MaxLength="12" Width="15em" Visible="true" Text="" runat="server"></asp:TextBox>  
              &nbsp; <asp:Label ID="lblPoc" runat="server" ForeColor="Red" Visible="false" Text=""></asp:Label> 
            <asp:RequiredFieldValidator ID="RequiredFD3" ControlToValidate="txtPOC_SID" ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: POC is required."></asp:RequiredFieldValidator>
                     <asp:CompareValidator runat="server" Operator="DataTypeCheck" Type="Integer" ValidationGroup="allFields" ForeColor="Red" Display="Dynamic"
 ControlToValidate="txtPOC_SID" ErrorMessage="Error: POC's SLAC ID must be a whole number" /> 
                        &nbsp;
            <asp:CompareValidator ID="CompareValidator1" ControlToValidate="txtManager_SID" ControlToCompare="txtPOC_SID" ValidationGroup="allFields" Operator="NotEqual" 
                 ForeColor="Red" Display="Dynamic" runat="server" ErrorMessage="Error: Manager and POC must be diffrent person!"></asp:CompareValidator>
        </td></tr>
         <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">POC's Name:   </td>
        <td style=" text-align:left; ">
            <asp:TextBox ID="txtPOC" BackColor="DimGray" ForeColor="White" TextMode="SingleLine" Width="50%"  ReadOnly="true" Text="" runat="server"></asp:TextBox>   
       
        </td></tr>
       

        <tr>
            <td colspan="2" style="height:40px;"></td>
        </tr>
     <tr style="margin-top:20px;">
        <td colspan="2"  style="text-align:center;">    
              <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Font-Bold="true" Font-Size="X-Large" ValidationGroup="allFields" OnClick="btnConfirm_Click" /> &nbsp;&nbsp; &nbsp;&nbsp;  
          <asp:Button ID="btnSubmit" runat="server" Text="Submit" Visible="false" Font-Bold="true" Font-Size="X-Large" ValidationGroup="allFields" OnClick="btnSubmit_Click" /> 
            &nbsp;&nbsp; &nbsp;&nbsp; 
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click"  />

       </td></tr>             
 </table>
</asp:Content>
