<%@ Page Title="" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="issue_code.aspx.cs" Inherits="SIIMS.admin.issue_code" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <style type="text/css">

         .grid td, .grid th {
             text-align: center;
         }


        .pager td
{
	padding-left: 4px;
	padding-right: 4px;
	padding-top: 1px;
	padding-bottom: 2px;
}

    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div  class="pageHeader">Assign Trending Codes to Issues</div>
   <div style="text-align:center;">
     <asp:GridView ID="gvw1" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="20"  CellPadding="5" AllowSorting="true"
          BackColor="#f1f1f1"  width="100%"  GridLines="None"
         DataSourceID="ds_AllIssues" DataKeyNames="Issue_ID"  EmptyDataText="No issues found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center"
         OnPageIndexChanging="gvw1_PageIndexChanging">
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText="Issue ID&nbsp;" DataField="Issue_ID" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="7%" SortExpression="issue_id"  />
             <asp:TemplateField HeaderText="Issue Title" ItemStyle-HorizontalAlign="Left" SortExpression="iTitle" ItemStyle-Width="35%">
                    <ItemTemplate>
                         <a href='issue_assignCode.aspx?from=a&iid=<%# Eval("ISSUE_ID") %>'><%# Eval("iTitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
             <asp:BoundField HeaderText="Level&nbsp;" DataField="sig_level" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="6%" SortExpression="sig_level" />
             <asp:BoundField HeaderText="&nbsp;Organization" DataField="org" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="9%" SortExpression="org"  />
              <asp:BoundField HeaderText="&nbsp;Source Title" DataField="stitle" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="33%" SortExpression="stitle" />
                 <asp:BoundField HeaderText="&nbsp;Owner" DataField="owner" ItemStyle-HorizontalAlign="Left" SortExpression="owner" />
            </Columns>
                
 
    </asp:GridView></div>


    <asp:SqlDataSource ID="ds_AllIssues" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select issue.issue_id, issue.title as ititle, issue.sig_level, org.NAME as org, p.name as owner, s.title as stitle
from siims_issue issue
join siims_org org on org.org_id=issue.org_id
join persons.person p on issue.owner_sid=p.key
join siims_source s on s.issue_id=issue.issue_id
where  issue.is_active='Y' and issue.status_id in (11,12) 
        and ( not exists (select * from siims_issue_code where issue_id=issue.issue_id and is_worktype='Y')
OR not exists (select * from siims_issue_code where issue_id=issue.issue_id and is_worktype='N'))
order by 1"> 
        </asp:SqlDataSource>

  

</asp:Content>
