<%@ Page Title="Actions by Owner" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_ownerAction.aspx.cs" Inherits="SIIMS.rep_ownerAction" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
     <div>
       <asp:Label ID="lblMsg" runat="server" Visible="False" ForeColor="Red"></asp:Label>
      <br /><br />

       <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                        Please enter the first few characters of the employee's last name: </asp:label>
                <br />  <br />
                  <asp:textbox id="txtOwner"  runat="server" Width="152px"  onfocus="this.scrollIntoView()" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click"  Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
                <br /> 
                <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica"></asp:Label>
                <br />
                 <asp:dropdownlist id="ddlEmplist" runat="server" Visible="False" TabIndex="3"></asp:dropdownlist>

 

         <br /><br />
       <asp:Button ID="btnView" runat="server" Text=" Submit " Font-Bold="True" Visible="false" 
               Font-Size="Larger" onclick="btnView_Click" /> &nbsp; &nbsp; &nbsp; &nbsp;
         <asp:Button ID="btnClear" runat="server" Text="Clear" Font-Bold="True" Visible="false" 
               Font-Size="Larger" onclick="btnClear_Click" />
   </div>
</asp:Content>
