<%@ Page Title="Edit Migrated Report" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="migration_edit.aspx.cs" Inherits="SIIMS.RIR.migration_edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="pageHeader">Edit Statement of Migrated Report </div>
     <div>
             This is for the migration report:<br />
           <div><span  class="fieldHeader">Report #: </span> <asp:Label ID="lblIssueID" runat="server" Text=""></asp:Label> </div>
        <div><span  class="fieldHeader">Title: </span> <asp:Label ID="lblTitle" runat="server" Text=""></asp:Label> </div>
      </div>

         <br />
        <asp:TextBox  ID="txtImage1" Name="txtImage1" runat="server" Width="90%"  TextMode="MultiLine"   >
 </asp:TextBox>  <br />  
     <asp:RequiredFieldValidator ID="RequiredTitle" ControlToValidate="txtImage1" Display="Dynamic" ValidationGroup="titleOnly" runat="server" CssClass="errorText" ErrorMessage="Error: Statement is required."></asp:RequiredFieldValidator>
   
    <div style="margin-top:10px; margin-bottom:20px; margin-left:30px;">
               <asp:Button ID="btnSubmit" runat="server" Text="&nbsp;Save&nbsp;" Font-Bold="true" ValidationGroup="titleOnly"  Font-Size="X-Large"   OnClick="btnSubmit_Click" /> &nbsp;  &nbsp; &nbsp;
       <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />
    </div>
    	<script src='../scripts/autosize.js'></script>
     <script type="text/javascript">
         autosize(document.getElementById('<%=txtImage1.ClientID%>'));
     </script>
</asp:Content>
