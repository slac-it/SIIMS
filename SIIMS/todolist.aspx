<%@ Page Title="TODO List" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="todolist.aspx.cs" Inherits="SIIMS.todolist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .paddingCell  {
            padding:5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      My To Do List 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_ToDo" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="false" DataSourceID="ds_TODO" DataKeyNames="item_id"
           Font-Size="Large"  runat="server"  OnRowDataBound="GVTodo_RowDataBound" 
            OnRowCommand = "GVTodo_RowCommand" 
             GridLines="None" AllowSorting="true" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
               
                <asp:TemplateField HeaderText="Item ID" HeaderStyle-CssClass ="paddingCell"
                   ItemStyle-CssClass="paddingCell" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                       <%# DataFormatID( Eval("type").ToString(),Eval("item_id").ToString()) %>
                    </ItemTemplate>
                 
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="Title">
                    <ItemTemplate>
                       <%#Eval("Title") %> 
                      </ItemTemplate>          
                </asp:TemplateField>
               
               <asp:TemplateField HeaderText="Requestor" ItemStyle-HorizontalAlign="Left" SortExpression="name">
                    <ItemTemplate>
                        <%#Eval("name") %>
                      </ItemTemplate>          
                </asp:TemplateField>
                
                 <asp:TemplateField HeaderText="Request date" SortExpression="created_on">
                    <ItemTemplate><%# Eval("created_on") %> 
                    </ItemTemplate>    
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Status">
                    <ItemTemplate><%#  DataFormatStatus( Eval("type").ToString(),Eval("status_id").ToString(),Eval("sub_status_id").ToString()) %>
                    </ItemTemplate>    
                </asp:TemplateField>

                  <asp:TemplateField HeaderText="To Do">
                    <ItemTemplate>
                        <%#  DataFormatAction( Eval("type").ToString(),Eval("status_id").ToString(),Eval("sub_status_id").ToString(),Eval("item_id").ToString()) %>
                    </ItemTemplate>    
                </asp:TemplateField>
               
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

  
       <asp:SqlDataSource ID="ds_TODO" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '1' as type, iss.status_id,sta.status, iss.sub_status_id, iss.issue_id as item_id, iss.title,p.name,p.key,to_char(iss.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_issue iss 
           join persons.person p on iss.created_by = p.key 
           join siims_status sta on sta.status_id=iss.status_id
           where iss.is_active='Y' and iss.created_by=:UserID and iss.status_id=10 and (iss.is_rir is null or iss.is_rir='N')
union
   select '1' as type, iss.status_id,sta.status, iss.sub_status_id, issue_id as item_id, iss.title,p.name,p.key,to_char(iss.last_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_issue iss 
           join persons.person p on iss.created_by = p.key 
            join siims_status sta on sta.status_id=iss.status_id
           where iss.is_active='Y' and  iss.status_id=11  and iss.sub_status_id in (110, 111,115) and exists (select sid from siims_manager where SID=:UserID and IS_IIP='Y'  and is_active='Y')
 union 
           select '2' as type, act.status_id,sta.status, act.sub_status_id, act.action_id as item_id, act.title,p.name,p.key,to_char(act.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_action act 
           join siims_issue iss on iss.issue_id=act.issue_id
           join persons.person p on act.created_by = p.key 
           join siims_status sta on sta.status_id=act.status_id
           where act.is_active='Y' and act.created_by=:UserID and act.status_id=20   and (iss.is_rir is null or iss.is_rir='N')
 union
           select '2' as type, act.status_id,sta.status, act.sub_status_id, act.action_id as item_id, act.title,p.name,p.key,to_char(act.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_action act 
               join siims_issue issue on issue.issue_id=act.issue_id
        join persons.person p on  p.key =issue.OWNER_SID
           join siims_status sta on sta.status_id=act.status_id
           where act.is_active='Y' and act.owner_sid=:UserID and act.status_id=21 and  act.sub_status_id is null
UNION
 select '2' as type, act.status_id,sta.status, act.sub_status_id, act.action_id as item_id, act.title,p.name,p.key,to_char(act.last_on, 'MM/DD/YYYY') CREATED_ON 
           from siims_action act 
           join siims_action_change ch on ch.action_id=act.action_id and ch.sub_status_id=210 and ch.is_active='Y' and ch.NEW_OWNER_SID=:UserID
           join persons.person p on p.key = ch.NEW_OWNER_SID
           join siims_status sta on sta.status_id=act.status_id
           where act.is_active='Y'  and act.status_id=21 and act.sub_status_id =210
union
select '2' as type, act.status_id,'Open' as status, ch.sub_status_id, act.action_id as item_id, act.title,p.name,p.key,to_char(ch.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
from siims_action_change ch
join siims_action act on ch.action_id =act.action_id and act.status_id=22
join persons.person p on p.key=ch.created_by
where ch.is_active='Y' and  ch.sub_status_id in (223, 221,222,220,224)  and (ch.IS_BY_IOWNER='Y' or ch.IS_IOWNER_APPROVED='Y')
and exists (select sid from siims_manager where SID=:UserID and IS_IIP='Y' and is_active='Y')
union
select '3' as type, act.status_id,'Open' as status, ch.sub_status_id, act.action_id as item_id, act.title,p.name,p.key,to_char(ch.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
from siims_action_change ch
join siims_action act on ch.action_id =act.action_id and act.status_id=22
join persons.person p on p.key=ch.created_by
join siims_issue issue on act.issue_id=issue.issue_id and issue.owner_sid=:UserID
where ch.is_active='Y' and  ch.sub_status_id in (221,222,220,224) and ch.IS_BY_IOWNER='N' and  (ch.IS_IOWNER_APPROVED='N' or ch.IS_IOWNER_APPROVED is null)

union
    select '11' as type, iss.status_id,sta.status, iss.sub_status_id, issue_id as item_id, iss.title,p.name,p.key,to_char(iss.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_issue iss 
           join persons.person p on iss.created_by = p.key 
            join siims_status sta on sta.status_id=iss.status_id
           where iss.is_active='Y' and  iss.status_id=11 and (select count(*) from siims_action where issue_id=iss.issue_id and is_active='Y')=0 and iss.owner_sid=:UserID
            and trunc(sysdate) - iss.CREATED_ON > 30 

     union select '20' as type, iss.status_id,sta.status, iss.sub_status_id, iss.issue_id as item_id, iss.title,p.name,p.key,to_char(iss.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_issue iss 
           join siims_rir_report rir on rir.issue_id=iss.issue_id and rir.rir_status='D'
           join persons.person p on iss.created_by = p.key 
           join siims_status sta on sta.status_id=iss.status_id
           where iss.is_active='Y' and iss.created_by=:UserID and iss.status_id=10 and  IS_RIR='Y'
    UNION select '22' as type, iss.status_id,sta.status, iss.sub_status_id, iss.issue_id as item_id, iss.title,
           (select name from persons.person where key=(select reviewer_sid from siims_rir_reviewer where IS_OWNER='Y' and IS_ACTIVE='Y')) as name,
            (select reviewer_sid from siims_rir_reviewer where IS_OWNER='Y' and IS_ACTIVE='Y') as key 
            ,to_char(iss.CREATED_ON, 'MM/DD/YYYY') CREATED_ON 
           from siims_issue iss 
           join siims_rir_report rir on rir.issue_id=iss.issue_id and rir.rir_status='R'
           join siims_status sta on sta.status_id=iss.status_id
           where iss.is_active='Y' and  iss.status_id=10 and  IS_RIR='Y'
           and EXISTS (SELECT *
              FROM SIIMS_RIR_REVIEWER
              WHERE reviewer_sid=:UserID and is_active='Y' and is_owner='N')
           order by 9 "
           >
            <SelectParameters>
    <asp:SessionParameter Name="UserID" SessionField="LoginSID" />
</SelectParameters>
    </asp:SqlDataSource>   
</asp:Content>
