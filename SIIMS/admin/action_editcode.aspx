<%@ Page Title="Edit Action Type" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="action_editcode.aspx.cs" Inherits="SIIMS.admin.action_editcode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <style type="text/css">

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

      <div  class="pageHeader">Edit Action Types</div>
   <div style="text-align:center;">
     <asp:GridView ID="gvw1" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="20"  CellPadding="5" AllowSorting="true"
          BackColor="#f1f1f1"  width="100%"  GridLines="None"
         DataSourceID="ds_AllActions" DataKeyNames="aid"  EmptyDataText="No issues found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center"
         OnPageIndexChanging="gvw1_PageIndexChanging">
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText="Action ID&nbsp;" DataField="ACTION_ID" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="7%" SortExpression="aid"  />
             <asp:TemplateField HeaderText="Action Title" ItemStyle-HorizontalAlign="Left" SortExpression="sTitle" ItemStyle-Width="30%">
                    <ItemTemplate>
                         <a href='action_assignCode.aspx?from=e&aid=<%# Eval("aid") %>'><%# Eval("aTitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
              <asp:BoundField HeaderText="Owner" DataField="owner" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="12%" SortExpression="owner" />
            <%-- <asp:BoundField HeaderText="Level&nbsp;" DataField="sig_level" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="6%" SortExpression="sig_level" />--%>
             <asp:BoundField HeaderText="Issue Title" DataField="ititle" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%" SortExpression="ititle"  />
              <asp:BoundField HeaderText="Source Title" DataField="stitle" ItemStyle-HorizontalAlign="Left"  SortExpression="stitle" />
                
            </Columns>
                
 
    </asp:GridView></div>


    <asp:SqlDataSource ID="ds_AllActions" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select ac.action_id as aid,'A' || ac.action_id as action_id, ac.title as atitle, issue.title as ititle, issue.sig_level, p.name as owner, s.title as stitle
from siims_action ac 
join siims_issue issue on ac.issue_id = issue.issue_id
join persons.person p on ac.owner_sid=p.key
join siims_source s on s.issue_id=issue.issue_id
where  ac.is_active='Y' and ac.status_id in (22,23) 
AND ac.CAT_ACODE_ID is not null AND ac.SUB_ACODE_ID is not null
order by 1"> 
        </asp:SqlDataSource>

</asp:Content>
