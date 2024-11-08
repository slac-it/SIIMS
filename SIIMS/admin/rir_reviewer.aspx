<%@ Page Title="RIR Reviewer" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_reviewer.aspx.cs" Inherits="SIIMS.admin.rir_reviewer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       <div>
    <div  class="pageHeader">Edit RIR Reviewers and RIR Coordinator  </div>
        <%--   <div>If you want to replace anyone, please disable the person first. Then you can add a new person.</div>--%>
          
 </div>

     <asp:Panel ID="PnlPopup" runat="server">
            <div style="text-align:left;">
                 <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                       First, please enter the first few characters of the new person's  last name: </asp:label>
                <br />
                  <asp:textbox id="txtOwner"  runat="server" Width="152px" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click"  Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
                <br /> 
     <asp:Panel ID="PanelFound"  Visible="false" runat="server">  
                 <table width="100%" id="editTable" style="margin-top:30px;">
         <tr>

        <td style="text-align:left;">
            <div><span  class="fieldHeader">New Person Name:</span>
             <asp:dropdownlist id="ddlEmplist" runat="server" TabIndex="3"></asp:dropdownlist>
           </div>
              <asp:RequiredFieldValidator InitialValue="-1" ID="Req_Org" ValidationGroup="newMember" runat="server" Display="Dynamic" ControlToValidate="ddlEmplist"  CssClass="errorText" 
                          ErrorMessage="Error: New Member Name is required!"></asp:RequiredFieldValidator>
        </td>
        </tr>
            <tr style="height:15px;"><td></td></tr>       
         <tr>
        <td style="text-align:left;">
            <div><span  class="fieldHeader">Select the reviewer/approver to be replaced:</span>
             <asp:DropDownList ID="drwReviewer" AutoPostBack="false" DataSourceID="ds_Reviewer"
                   DataTextField="person" DataValueField="reviewer_id"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList>    </div>
              <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ValidationGroup="newMember" ControlToValidate="drwReviewer"  CssClass="errorText" 
                          ErrorMessage="Reviewer/Approver is required!"></asp:RequiredFieldValidator>
           
        </td>
        </tr>
                     <tr style="height:15px;"><td></td></tr>
                <tr>
        <td style="text-align:left;">   
                      <asp:button id="cmdOk" runat="server"  Text="Replace"  Font-Bold="true" Font-Size="Large" ValidationGroup="newMember"
                            TabIndex="4" onclick="cmdOk_Click"></asp:button>    
                    <br />
              </td></tr>
                      <tr style="height:15px;"><td></td></tr>
                 </table>
             </asp:Panel>  
                   <br />
                   <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica">Label</asp:Label> 
     </div>
        </asp:Panel>  
    <div class="fieldHeader" style="text-align:center;">Current Reviewer/Approver
     <asp:GridView ID="gvwReviewerList" runat="server" AutoGenerateColumns="false"  CellPadding="5" AllowSorting="true"  
          BackColor="#f1f1f1"  width="80%"  GridLines="None"
       EmptyDataText="No people found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center">
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="25px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText="&nbsp;SLAC ID" DataField="Key" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%"   />
             <asp:BoundField HeaderText="&nbsp;Name" DataField="name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="45%" />
             <asp:BoundField HeaderText="&nbsp;Title" DataField="TITLE" ItemStyle-HorizontalAlign="Left"  />
              
            </Columns>
                
 
    </asp:GridView>
        </div>
       <div style="height:15px"></div>
     <asp:SqlDataSource ID="ds_Reviewer" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="SELECT vw.reviewer_id, p.key,p.name || ' - ' || vw.title as person  from SIIMS_RIR_REVIEWER vw
 join PERSONS.PERSON p on p.key = vw.reviewer_sid
 where  vw.IS_ACTIVE = 'Y' order by p.name ">
    </asp:SqlDataSource>     
</asp:Content>
