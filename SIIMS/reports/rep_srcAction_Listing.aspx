<%@ Page Title="Actions by Source" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_srcAction_Listing.aspx.cs" Inherits="SIIMS.rep_srcAction_Listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      
    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Actions by Source
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_srcIssues" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_srcActions" DataKeyNames="action_id"
           Font-Size="Large"  runat="server" 
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
               
                <asp:TemplateField HeaderText="Source Type"  ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                       <%# Eval("type") %>
                    </ItemTemplate>       
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

              <%--  <asp:TemplateField HeaderText="Issue ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Issue_ID" >
                    <ItemTemplate>
                       <%#Eval("Issue_ID") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>--%>

                  <asp:TemplateField HeaderText="Action ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Action_ID" >
                    <ItemTemplate>
                       <%#Eval("Action_AID") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
               
                 <asp:HyperLinkField DataNavigateUrlFields="action_id" DataTextField="Title" 
                     ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"  HeaderText="Action Title"  SortExpression="title"
                    DataNavigateUrlFormatString="action_view.aspx?aid={0}" />

                 <asp:TemplateField HeaderText="Owner" ItemStyle-HorizontalAlign="Left"  SortExpression="Owner">
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Level" ItemStyle-HorizontalAlign="Left"  SortExpression="sig_level">
                    <ItemTemplate>
                       <%#Eval("sig_level") %> 
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

      <asp:SqlDataSource ID="ds_srcActions" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.issue_id, action.action_id, 'A' || action.action_id as action_aid, action.title, action.owner_sid, p.name as owner, issue.SIG_LEVEL, 
action.status_id, sta.status, org.name as org, src.type,to_char( action.due_date,'MM/DD/YYYY') as duedate
from siims_action action 
join siims_issue issue on action.issue_id=issue.issue_id
left join siims_org org on issue.org_id=org.org_id
join siims_status sta on action.status_id=sta.status_id
join SIIMS_SOURCE_TYPE src on issue.stype_id=src.stype_id
left join persons.person p on p.key=action.owner_sid
where action.is_active='Y' and issue.stype_id=:STYPE and action.status_id <> 20
order by 1, 2"   >
 <SelectParameters>
     <asp:QueryStringParameter DefaultValue="-1" DbType="String" QueryStringField="stype" Name="STYPE" />
 </SelectParameters>
    </asp:SqlDataSource>   


</asp:Content>
