<%@ Page Title="Issues by Manager" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_deptIssue_listing.aspx.cs" Inherits="SIIMS.rep_deptIssue_listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

  <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Issues by Manager  &nbsp; &nbsp; &nbsp; &nbsp; <asp:HyperLink ID="lnkDownload" Font-Size="Medium" runat="server">Export to Excel</asp:HyperLink> 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_ownerIssues" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_ownerIssues" DataKeyNames="issue_id" EmptyDataText="No Issues Found!"
           Font-Size="Large"  runat="server" 
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                <asp:TemplateField HeaderText="Manager"   ItemStyle-Width="10%"  >
                    <ItemTemplate>
                       <%#Eval("mgr_name") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

             
                <asp:TemplateField HeaderText="Issue ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Issue_ID"  ItemStyle-Width="10%" >
                    <ItemTemplate>
                       <%#Eval("Issue_ID") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>

                 <asp:HyperLinkField DataNavigateUrlFields="issue_id" DataTextField="Title" HeaderText="Title"  
                     ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"  
                     SortExpression="title"
                    DataNavigateUrlFormatString="../issue_view.aspx?iid={0}" />

                  <asp:TemplateField HeaderText="Organization" SortExpression="org">
                    <ItemTemplate>
                       <%# Eval("org") %>
                    </ItemTemplate>       
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

              

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

               <%--  <asp:TemplateField HeaderText="Source Type" ItemStyle-HorizontalAlign="Left" SortExpression="type" >
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

      <asp:SqlDataSource ID="ds_ownerIssues" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.issue_id,mn.name as owner, issue.owner_sid, mn.deptname as dept, sup.name as mgr_name, issue.title,issue.sig_level
    ,issue.stype_id,src.type, source.title as stitle, sta.status, org.name as org
from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path 
 FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP'and level> 1  
 START WITH emp.key = :SID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID 
 join siims_issue issue on issue.owner_sid=mn.key and issue.IS_ACTIVE='Y' and issue.status_id <> 10
 join siims_status sta on issue.status_id=sta.status_id
 left join siims_org org on issue.org_id=org.org_id
 join SIIMS_SOURCE source on issue.issue_id=source.issue_id and source.is_active='Y'
 left join siims_source_type src on src.stype_id=issue.stype_id
          left join siims_source s on issue.issue_id=s.issue_id order by 1"   >
 <SelectParameters>
     <asp:QueryStringParameter DefaultValue="-1" DbType="Int32" QueryStringField="sid" Name="SID" />
 </SelectParameters>
    </asp:SqlDataSource>   
</asp:Content>
