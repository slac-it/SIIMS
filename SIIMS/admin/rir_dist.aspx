<%@ Page Title="Shared Notification List" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_dist.aspx.cs" Inherits="SIIMS.admin.rir_dist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div  class="pageHeader"> Notification email Recipients List</div>

     <asp:Panel ID="PnlPopup" runat="server">
            <div style="text-align:left;">
                 <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                     To add more people to the shared notification recipient list, 
                        please enter the first few characters of the member's last name: </asp:label>
                <br />
                  <asp:textbox id="txtOwner"  runat="server" Width="152px" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click"  Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
                <br /> 
     <asp:Panel ID="PanelFound"  Visible="false" runat="server">  
                 <table width="100%" id="editTable">
         <tr>
        <td style="text-align:left;">
            <div><span  class="fieldHeader">New Member Name:</span>
             <asp:dropdownlist id="ddlEmplist" runat="server" TabIndex="3"></asp:dropdownlist>
           </div>
              <asp:RequiredFieldValidator InitialValue="-1" ID="Req_Org" ValidationGroup="newMember" runat="server" Display="Dynamic" ControlToValidate="ddlEmplist"  CssClass="errorText" 
                          ErrorMessage="Error: New Member Name is required!"></asp:RequiredFieldValidator>
        </td>
        </tr>
                   
         <tr>
        <td style="text-align:left;">
            <div><span  class="fieldHeader">Title:</span>
             <asp:TextBox ID="txtTitle" runat="server" onkeypress="return this.value.length<=100" onpaste="return this.value.length<=100"
                        TextMode="SingleLine" MaxLength="100" Width="80%"> </asp:TextBox>
           </div>
        </td>
        </tr>
                <tr>
        <td style="text-align:left;">   
                      <asp:button id="cmdOk" runat="server"  Text="Add to List"   Font-Bold="true" Font-Size="Large" ValidationGroup="newMember"
                            TabIndex="4" onclick="cmdOk_Click"></asp:button>    
                    <br />
              </td></tr></table>
             </asp:Panel>  
                   <br />
                   <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica">Label</asp:Label> 
     </div>
        </asp:Panel>  
   <div style="text-align:center;">
     <asp:GridView ID="gvwDistList" runat="server" AutoGenerateColumns="false"  CellPadding="5" AllowSorting="true"  DataKeyNames="dist_id" 
          BackColor="#f1f1f1"  width="80%"  GridLines="None"
       EmptyDataText="No people found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center">
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="25px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText="&nbsp;SLAC ID" DataField="SID" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%"   />
             <asp:BoundField HeaderText="&nbsp;Name" DataField="viewer" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="25%" />
             <asp:BoundField HeaderText="&nbsp;Title" DataField="DIST_TITLE" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="25%"  />
                 <asp:BoundField HeaderText="&nbsp;Department" DataField="dept_name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%"  />
               <asp:TemplateField HeaderText="To Do">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" runat="server" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this person from the shared notification list?');"> Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            </Columns>
                
 
    </asp:GridView>
   </div>
     <div style="height:15px"></div>
     <div style="text-align:center;">
          <%--<asp:Button ID="btnEmail" runat="server" Text="Create List"  Font-Bold="true" Font-Size="X-Large" 
             onclientclick="return confirm(&quot;Are you sure you want to send email now?&quot;);" 
          OnClick="btnEmail_Click" /> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
    <asp:Button ID="btnCancel" runat="server" Text="Back" Font-Bold="true" Font-Size="X-Large" OnClick="btnCancel_Click" />
         </div>
</asp:Content>
