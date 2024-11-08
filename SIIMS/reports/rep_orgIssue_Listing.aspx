<%@ Page Title="Organization Issues" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_orgIssue_Listing.aspx.cs" Inherits="SIIMS.rep_orgIssue_Listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
     Organization Issues &nbsp; &nbsp; &nbsp; &nbsp; <asp:HyperLink ID="lnkDownload" Font-Size="Medium" runat="server">Export to Excel</asp:HyperLink> 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_OrgIssues" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_orgIssues" DataKeyNames="issue_id"
           Font-Size="Large"  runat="server" 
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
               
                <asp:TemplateField HeaderText="Organization" HeaderStyle-CssClass ="paddingCell"
                   ItemStyle-CssClass="paddingCell" ItemStyle-HorizontalAlign="Left"  ItemStyle-Width="10%">
                    <ItemTemplate>
                       <%# Eval("org") %>
                    </ItemTemplate>       
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Issue ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Issue_ID"  ItemStyle-Width="10%">
                    <ItemTemplate>
                       <%#Eval("Issue_ID") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
                 		    
			    <asp:HyperLinkField DataNavigateUrlFields="issue_id" 
                    ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"  DataTextField="Title" HeaderText="Title"  SortExpression="title"
                    DataNavigateUrlFormatString="../issue_view.aspx?iid={0}" />
               
               

                 <asp:TemplateField HeaderText="Owner" ItemStyle-HorizontalAlign="Left" SortExpression="Owner" >
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Level" ItemStyle-HorizontalAlign="Left" SortExpression="sig_level" >
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

<%--                 <asp:TemplateField HeaderText="Source Type" ItemStyle-HorizontalAlign="Left" SortExpression="type" >
                    <ItemTemplate>
                       <%#Eval("type") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>--%>

                <asp:TemplateField HeaderText="Source Title" ItemStyle-HorizontalAlign="Left" SortExpression="stitle" >
                    <ItemTemplate>
                       <%#Eval("stitle") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

      <asp:SqlDataSource ID="ds_orgIssues" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.issue_id, issue.title, issue.owner_sid, p.name as owner, issue.SIG_LEVEL, 
issue.status_id, sta.status,
issue.stype_id,src.type,  s.title as stitle, org.name as org
from siims_issue issue
join siims_org org on issue.org_id=org.org_id
join siims_status sta on issue.status_id=sta.status_id
left join persons.person p on p.key=issue.owner_sid
left join siims_source_type src on src.stype_id=issue.stype_id
          left join siims_source s on issue.issue_id=s.issue_id
where issue.is_active='Y' and issue.org_id=:ORGID and issue.status_id <> 10
order by 1"   >
 <SelectParameters>
     <asp:QueryStringParameter DefaultValue="-1" DbType="Int32" QueryStringField="org_id" Name="ORGID" />
 </SelectParameters>
    </asp:SqlDataSource>   


</asp:Content>
