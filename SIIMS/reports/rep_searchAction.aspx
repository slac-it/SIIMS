<%@ Page Title="" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_searchAction.aspx.cs" Inherits="SIIMS.reports.rep_searchAction" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    &nbsp;&nbsp; &nbsp;&nbsp;Enter keyword to search Actions: <asp:TextBox ID="TxtKeyword" runat="server"  onkeydown="if (event.keyCode == 13) document.getElementByID('BtnGo').click()"></asp:TextBox> 
    <asp:Button ID="BtnGo" runat="server" Text="Go" TabIndex="1"  Font-Bold="true" Font-Size="Large" OnClick="BtnGo_Click" />
    &nbsp;&nbsp; <span style="font-size:small;color:darkblue;">Note: This search is performed on Title and Description </span>
   
    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Actions by Keyword &nbsp; &nbsp; &nbsp; &nbsp;
       <asp:LinkButton ID="LnkExcel" runat="server" OnClick="LnkExcel_Click" Font-Size="Medium" Text="Export to Excel" TabIndex="100"></asp:LinkButton>
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_Action" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_Actions" DataKeyNames="action_id"
           Font-Size="Large"  runat="server" GridLines="None" AllowSorting="True"  EmptyDataText="No Actions found">
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
    

                  <asp:TemplateField HeaderText="Action ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Action_ID" >
                    <ItemTemplate>
                       <%#Eval("Action_AID") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
                
           <asp:TemplateField HeaderText="Issue ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Issue_ID" >
                    <ItemTemplate>
                       <%#Eval("Issue_ID") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>
               
                <asp:HyperLinkField DataNavigateUrlFields="action_id" DataTextField="Title" 
                    ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"  
                    HeaderText="Action Title"  SortExpression="title"
                    DataNavigateUrlFormatString="../action_view.aspx?aid={0}" />

                 <asp:TemplateField HeaderText="Owner" ItemStyle-HorizontalAlign="Left"  SortExpression="Owner">
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>


			   <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Left" SortExpression="status" >
                    <ItemTemplate>
                       <%#Eval("status") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Due Date" ItemStyle-HorizontalAlign="Left" SortExpression="duedate" >
                    <ItemTemplate>
                       <%#Eval("duedate") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

      <asp:SqlDataSource ID="ds_Actions" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.issue_id, action.action_id,'A' || action.action_id as action_aid, action.title, action.description, action.owner_sid, p.name as owner, issue.SIG_LEVEL, 
action.status_id, sta.status,  to_char( action.due_date,'MM/DD/YYYY') as duedate
from siims_action action join siims_issue issue on action.issue_id=issue.issue_id
join siims_status sta on action.status_id=sta.status_id
left join persons.person p on p.key=action.owner_sid
where action.is_active='Y'  and action.status_id <> 20 order by 1, 2"  FilterExpression="title LIKE '%{0}%' OR description LIKE '%{0}%'"  >
  <FilterParameters>
              <asp:ControlParameter ControlID="TxtKeyword" PropertyName="Text" />
  </FilterParameters>
</asp:SqlDataSource>   
     <asp:HiddenField ID="hdnkeyword" runat="server" />
</asp:Content>
