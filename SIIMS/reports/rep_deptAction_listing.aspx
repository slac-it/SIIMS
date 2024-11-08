<%@ Page Title="Actions by Manager" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_deptAction_listing.aspx.cs" Inherits="SIIMS.rep_deptAction_listing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

 <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Actions by Manager  &nbsp; &nbsp; &nbsp; &nbsp; <asp:HyperLink ID="lnkDownload" Font-Size="Medium" runat="server">Export to Excel</asp:HyperLink> 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_ownerActions" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_ownerActions" DataKeyNames="action_id"
           Font-Size="Large"  runat="server"  EmptyDataText="No Actions Found!"
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                  <asp:TemplateField HeaderText="Manager" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("mgr_name") %> 
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

                   		    
			    <asp:HyperLinkField DataNavigateUrlFields="action_id" DataTextField="ATitle" 
                    ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"   HeaderText="Action Title"  SortExpression="Atitle"
                    DataNavigateUrlFormatString="../action_view.aspx?aid={0}" />              
           
                   <asp:TemplateField HeaderText="Action Owner" ItemStyle-HorizontalAlign="Left" SortExpression="Owner" >
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
               

<%--                <asp:TemplateField HeaderText="Level" ItemStyle-HorizontalAlign="Left"  SortExpression="sig_level">
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
             SelectCommand="select mn.name as owner, mn.deptName as dept, sup.name as mgr_name, act.action_id, 'A' || act.action_id  as action_aid
    ,act.title as atitle, to_char(act.DUE_DATE,'MM/DD/YYYY') as dueDate, act.DUE_DATE, issue.issue_id,issue.title as ititle, issue.sig_level,
    act.status_id, sta.status
from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path 
FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP' and level > 1  
 START WITH emp.key = :SID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID 
 join siims_action act on act.owner_sid=mn.key and act.IS_ACTIVE='Y' and act.status_id<> 20 
 join siims_issue issue on issue.issue_id=act.issue_id 
 join siims_status sta on act.status_id=sta.status_id
 order by act.action_id desc"   >
 <SelectParameters>
     <asp:QueryStringParameter DefaultValue="-1" DbType="int32" QueryStringField="sid" Name="SID" />
 </SelectParameters>
    </asp:SqlDataSource>   
</asp:Content>
