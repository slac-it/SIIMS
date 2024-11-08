<%@ Page Title="Change Issue Status" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="issue_ChStatus.aspx.cs" Inherits="SIIMS.admin.issue_ChStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div  class="pageHeader"> Change Issue Status</div>
Please note that to change the staus of an issue, the actions associated with the issue have to be either Closed or Deleted. <br />
    Or please change the associted actions first before changing the status of an issue.

 <div>
      
      <br />

       <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                        Please enter the Issue ID: </asp:label>
                <br />
                  <asp:textbox id="txtOwner"  runat="server" Width="152px" MaxLength="7"  onfocus="this.scrollIntoView()" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click" ValidationGroup="IssueID"  Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
                <br /> 
     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="txtOwner"  ValidationGroup="IssueID" ForeColor="Red" ErrorMessage="Issue ID is required!"></asp:RequiredFieldValidator>
        <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic" Operator="DataTypeCheck" Type="Integer"  ValidationGroup="IssueID" ControlToValidate="txtOwner"
         ForeColor="Red" ErrorMessage="Issue ID must be integer!"></asp:CompareValidator>
                <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica"></asp:Label>
           
      <br />
      <asp:Panel ID="Panel1" Visible="False" runat="server">
          <asp:HiddenField ID="HiddenIssueID" runat="server" />
       Issue title: <asp:Label ID="lblTitle" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label> <br />
     Issue Status: <asp:Label ID="lblStatus" runat="server"  ForeColor="Black" Font-Bold="true"></asp:Label>  
    
  <table class="viewTable"  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:15px; ">
 <tr style="background-color:#E5E5FE">
   <th style="text-align:center;  padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
  Pending Requests:
     </th>
  </tr>
<tr > <td align="center" style="padding:0px; margin:0;">
  <asp:GridView ID="gvwRequests" runat="server" AutoGenerateColumns="false"  CellPadding="5" 
          BackColor="#f1f1f1"  width="90%"  GridLines="None"
       EmptyDataText="No pending requests found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center">
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="25px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <Columns>
                <asp:BoundField HeaderText="Request" DataField="status"  ItemStyle-HorizontalAlign="left" ItemStyle-Width="50%"   />
             <asp:BoundField HeaderText="Requested By" DataField="name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%" />
             <asp:BoundField HeaderText="Requested On" DataField="request_on" ItemStyle-HorizontalAlign="Left"  />
              
            </Columns>
                
 
    </asp:GridView>
        
</td> </tr>
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center;  padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      Actions for this Issue
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GVActions"  BackColor="#f1f1f1"  width="100%" CellSpacing="5"  AutoGenerateColumns="false"  DataKeyNames="ACTION_ID" EmptyDataText="No Actions"
           Font-Size="Large"  runat="server"   GridLines="None"  >
             <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="25px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
              
                <asp:TemplateField HeaderText="Action ID" SortExpression="ACTION_ID">
                    <ItemTemplate>
                        <asp:Label ID="lblActionID" Text='<%# Eval("action_id2") %>' runat="server"></asp:Label>
                    </ItemTemplate>               
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Action Title" ItemStyle-HorizontalAlign="Left" SortExpression="ATitle">
                    <ItemTemplate>
                  
                       <%# Eval("ATitle") %>
                      </ItemTemplate>          
                </asp:TemplateField>

                            <asp:TemplateField HeaderText="Owner" SortExpression="owner" ItemStyle-HorizontalAlign="Left"  ItemStyle-Width="15%">
            <ItemTemplate><%# Eval("owner")%></ItemTemplate>         
        </asp:TemplateField>

           <asp:TemplateField HeaderText="Status"  ItemStyle-HorizontalAlign="Left"     ItemStyle-Width="15%">
                 <ItemTemplate>
                 <%# Eval("status") %>    
                </ItemTemplate>      
        </asp:TemplateField>                                      
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>
  <div style="text-align:center; margin-top:10px;">
         <asp:Panel ID="PanelWarning" Visible="false" runat="server">
                    <div style="text-align:left; color:red; ">
             There are either pending requests for this issue or there are open or waiting for acceptance actions for this issue.
             If you want to proceed, the pending requests will be cleared and the open actions will be closed/deleted.

                   </div>     
       </asp:Panel>
      <asp:Panel ID="PanelOK" Visible="false" runat="server">
          <br />
                 Change Status to:   <asp:dropdownlist id="ddlStatuslist" Font-Bold="true" runat="server"  TabIndex="3"></asp:dropdownlist>
             <br /><br />
               <asp:Button ID="btnChange" runat="server" Text="Change Status" Font-Bold="True"   Font-Size="Larger" OnClick="btnChange_Click"  />
                 &nbsp; &nbsp; &nbsp; &nbsp;
                 <asp:Button ID="btnClear" runat="server" Text="Clear" Font-Bold="True"   Font-Size="Larger" onclick="btnClear_Click" />
     </asp:Panel>
 </div>
     
      </asp:Panel>

</asp:Content>
