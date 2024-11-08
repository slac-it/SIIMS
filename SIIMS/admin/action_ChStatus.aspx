<%@ Page Title="Change Action Status" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="action_ChStatus.aspx.cs" Inherits="SIIMS.admin.action_ChStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


      <div  class="pageHeader"> Change  Action Status</div>


 <div>
      
      <br />

       <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                        Please enter the Action ID: </asp:label>
                <br />
                  <asp:textbox id="txtOwner" MaxLength="7"  runat="server" Width="152px"  onfocus="this.scrollIntoView()" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click" ValidationGroup="actionID" Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
                <br /> 
     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="Dynamic" ValidationGroup="actionID" runat="server" ControlToValidate="txtOwner" ForeColor="Red" ErrorMessage="Action ID is required!"></asp:RequiredFieldValidator>
     <asp:CompareValidator ID="CompareValidator1" runat="server" Operator="DataTypeCheck" Display="Dynamic" Type="Integer"  ValidationGroup="actionID" ControlToValidate="txtOwner"
         ForeColor="Red" ErrorMessage="Action ID must be integer!"></asp:CompareValidator>
              <br />
                <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica"></asp:Label>
           
      <br />
 <asp:Panel ID="PanelOK" Visible="False" runat="server">
     <div style="font-size:1.1em; font-weight:bold">
           Action Status: <asp:Label ID="lblAStatus" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label>
         </div>
     Action title: <asp:Label ID="lblATitle" runat="server" Font-Bold="true" Text=""></asp:Label> <br />
   <div style="font-size:1.0em; font-weight:bold">
      Issue Status: <asp:Label ID="lblIStatus" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label>
          </div>
       Issue title: <asp:Label ID="lblITitle" runat="server" Font-Bold="true" Text=""></asp:Label> <br />
    
     <asp:HiddenField ID="HiddenAID" runat="server" />
     <div style="height:10px"></div>
    <div class="fieldHeader" style="text-align:center;">Pending Requests:
     <asp:GridView ID="gvwRequests" runat="server" AutoGenerateColumns="false"  CellPadding="5" 
          BackColor="#f1f1f1"  width="80%"  GridLines="None"
       EmptyDataText="No pending requests found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center">
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="25px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <Columns>
                <asp:BoundField HeaderText="Request" DataField="status" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50%"   />
             <asp:BoundField HeaderText="Requested By" DataField="name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%" />
             <asp:BoundField HeaderText="Requested On" DataField="request_on" ItemStyle-HorizontalAlign="Left"  />
              
            </Columns>
                
 
    </asp:GridView>
        </div>

      <div style="text-align:center; margin-top:10px;">
         <asp:Panel ID="PanelWarning" Visible="false" runat="server">
                    <div style="text-align:left; color:red; ">
             There are either pending requests for this action. If you want to proceed, the pending requests will be cleared.

                   </div>   
    </asp:Panel>
 </asp:Panel>  
       <br /> 
     <asp:Panel ID="Panel1" Visible="False" runat="server">
    
      <br /> 
         Change Status to:   <asp:dropdownlist id="ddlStatuslist" runat="server"  TabIndex="3">
                             </asp:dropdownlist>
     <br /><br />
       <asp:Button ID="btnChange" runat="server" Text="Change Status" Font-Bold="True"   Font-Size="Larger" OnClick="btnChange_Click"  />
         &nbsp; &nbsp; &nbsp; &nbsp;
         <asp:Button ID="btnClear" runat="server" Text="Clear" Font-Bold="True"   Font-Size="Larger" onclick="btnClear_Click" />

     </asp:Panel>
   </div>

</asp:Content>
