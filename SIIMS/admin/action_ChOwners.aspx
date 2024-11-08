<%@ Page Title="Change Action Owner" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="action_ChOwners.aspx.cs" Inherits="SIIMS.admin.action_ChOwners" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    

      <div  class="pageHeader"> Change  Action Owner </div>


 <div>
      
      <br />

       <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                        Please enter the Action ID: </asp:label>
                <br />
                  <asp:textbox id="txtOwner" MaxLength="7"  runat="server" Width="152px" placeholder="(Integer Action ID)"  onfocus="this.scrollIntoView()" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click" ValidationGroup="actionID" Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
                <br /> 
     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ValidationGroup="actionID" ControlToValidate="txtOwner" ForeColor="Red" ErrorMessage="Action ID is required!"></asp:RequiredFieldValidator>
          <asp:CompareValidator ID="CompareValidator1" runat="server" Operator="DataTypeCheck" Display="Dynamic" Type="Integer"  ValidationGroup="actionID" ControlToValidate="txtOwner"
         ForeColor="Red" ErrorMessage="Action ID must be integer!"></asp:CompareValidator>
              <br />
                <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica"></asp:Label>
             <asp:HiddenField ID="HiddenAID" runat="server" />
      <br />

     <asp:Panel ID="Panel1" Visible="False" runat="server">
    
      <br /> 
            Action title: <asp:Label ID="lblTitle" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label>
         <br />
         Action Owner: <asp:Label ID="lblOwner" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label>  <br />
      Action Status: <asp:Label ID="lblStatus" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label>

       <br />  <br /> 
       New Action Owner:  <asp:TextBox ID="txtNewOwner" placeholder="(SLAC ID)" Width="100px" runat="server"></asp:TextBox>
       
         <asp:Button ID="btnRequest" runat="server" Text="Request Change" Font-Bold="True" ValidationGroup="actionOwnerID"  Font-Size="Larger" OnClick="btnRequest_Click"  />
         &nbsp; &nbsp;<br />
           <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="actionOwnerID" ControlToValidate="txtNewOwner" ForeColor="Red" Display="Dynamic"
               ErrorMessage="New action Owner ID is required!"></asp:RequiredFieldValidator>
         <asp:CompareValidator ID="CompareValidator2" runat="server" Operator="DataTypeCheck" Display="Dynamic" Type="Integer"  ValidationGroup="actionOwnerID" 
             ControlToValidate="txtNewOwner" 
         ForeColor="Red" ErrorMessage="SLAC ID must be integer!"></asp:CompareValidator>
         <br /> 
           <asp:Label id="lblError2" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica"></asp:Label>
         <br />
         <asp:Panel ID="Panel2" Visible="false" runat="server">
        The new action owner will be: 
         <asp:Label ID="lblNewOwner" runat="server" Font-Bold="true"  Text=""></asp:Label> <br />
        Please click "Make Change" to confirm; or click "Clear" to restart.

         <br /><br />
           
       <asp:Button ID="btnChange" runat="server" Text="Make Change"  Font-Bold="True" ValidationGroup="actionOwnerID"  Font-Size="Larger" OnClick="btnChange_Click"  />
         &nbsp; &nbsp; &nbsp; &nbsp;
         <asp:Button ID="btnClear" runat="server" Text="Clear" Font-Bold="True"   Font-Size="Larger" onclick="btnClear_Click" />
             </asp:Panel>

     </asp:Panel>
   </div>

</asp:Content>
