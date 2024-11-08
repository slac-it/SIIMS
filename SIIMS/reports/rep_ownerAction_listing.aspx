<%@ Page Title="Actions by Owner" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_ownerAction_listing.aspx.cs" Inherits="SIIMS.rep_ownerAction_listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
          
    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Actions by Owner    &nbsp; &nbsp; &nbsp; &nbsp; <asp:HyperLink ID="lnkDownload" Font-Size="Medium" runat="server">Export to Excel</asp:HyperLink> 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_ownerActions" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_ownerActions" DataKeyNames="action_id"
           Font-Size="Large"  runat="server" EmptyDataText="No Actions Found!"
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                  <asp:TemplateField HeaderText="Owner" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                  <asp:TemplateField HeaderText="Action ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Action_ID" >
                    <ItemTemplate>
                       <%#Eval("Action_AID") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                   		    
			    <asp:HyperLinkField DataNavigateUrlFields="action_id" 
                    ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"  
                     DataTextField="Title" HeaderText="Action Title"  SortExpression="title"
                    DataNavigateUrlFormatString="../action_view.aspx?aid={0}" />                            

               <%-- <asp:TemplateField HeaderText="Level" ItemStyle-HorizontalAlign="Left"  SortExpression="sig_level">
                    <ItemTemplate>
                       <%#Eval("sig_level") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>--%>

			   <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Left" SortExpression="status" >
                    <ItemTemplate>
                       <%#Eval("status") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Due Date" ItemStyle-HorizontalAlign="Left" SortExpression="due_date" >
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

      <asp:SqlDataSource ID="ds_ownerActions" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.issue_id,action.action_id,  'A' || action.action_id as action_aid, action.title, action.owner_sid, p.name as owner, issue.SIG_LEVEL, 
action.status_id, sta.status,  to_char( action.due_date,'MM/DD/YYYY') as duedate,action.due_date
from siims_action action 
join siims_issue issue on action.issue_id=issue.issue_id
join siims_status sta on action.status_id=sta.status_id
join persons.person p on p.key=action.owner_sid
where action.is_active='Y' and action.owner_sid=:SID and action.status_id <> 20
order by 1, 2"   >
 <SelectParameters>
     <asp:QueryStringParameter DefaultValue="-1" DbType="int32" QueryStringField="sid" Name="SID" />
 </SelectParameters>
    </asp:SqlDataSource>   

</asp:Content>
