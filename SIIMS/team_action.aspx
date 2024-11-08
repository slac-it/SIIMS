<%@ Page Title="Team Actions" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="team_action.aspx.cs" Inherits="SIIMS.team_action" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script  type="text/javascript">
        
         function expandcollapse2(obj, row) {
             var div = document.getElementById(obj);
             var img = document.getElementById('act' + obj);

             if (div.style.display == "none") {
                 div.style.display = "block";
                 img.src = "img/minus.png";
             }
             else {
                 div.style.display = "none";
                 img.src = "img/plus.png";
                 img.alt = "Expand to show Orders";
             }
         }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  

      <div  class="pageHeader"> My Team Open Actions</div>
   
    <div style="text-align:left;">
This lists all open actions of your department owns.
        </div>

<table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
     My Team Open Actions 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GridView1" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="false" DataSourceID="ds_OpenActions" DataKeyNames="ACTION_ID"
           Font-Size="Large"  runat="server"  OnRowDataBound="GridView1_RowDataBound" 
            OnRowCommand = "GridView1_RowCommand" 
             GridLines="None" AllowSorting="true" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <a href="javascript:expandcollapse2('diva<%# Eval("ACTION_ID") %>', 'one');">
                            <img id="actdiva<%# Eval("ACTION_ID") %>" alt="Click to show/hide Orders for Actions <%# Eval("ACTION_ID") %>"  width="9px" border="0" src="img/plus.png"/>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action ID" SortExpression="ACTION_ID"  ItemStyle-Width="10%">
                    <ItemTemplate>
                        <asp:Label ID="lblActionID" Text='<%# Eval("action_id2") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                 
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="ATitle">
                    <ItemTemplate>
                         <a href='action_view.aspx?from=team&aid=<%# Eval("ACTION_ID") %>'><%# Eval("ATitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
               
               <asp:TemplateField HeaderText="Issue" ItemStyle-HorizontalAlign="Left" SortExpression="ITitle">
                    <ItemTemplate>
                         <a href='issue_view.aspx?from=team&iid=<%# Eval("ISSUE_ID") %>'><%# Eval("ITitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
                
                <%-- <asp:TemplateField HeaderText="Level" SortExpression="sig_level">
                    <ItemTemplate><%# Eval("sig_level") %> 
                    </ItemTemplate>
                  
                </asp:TemplateField>--%>

                <asp:TemplateField HeaderText="Due Date" SortExpression="dueDate">
                    <ItemTemplate><%# Eval("dueDate") %> 
                    </ItemTemplate>             
                </asp:TemplateField>

                  <asp:TemplateField HeaderText="Owner" SortExpression="emp_name">
                    <ItemTemplate><%# Eval("emp_name") %> 
                    </ItemTemplate>       
                </asp:TemplateField>

                <%-- <asp:TemplateField HeaderText="Department" SortExpression="dept">
                    <ItemTemplate><%# Eval("dept") %> 
                    </ItemTemplate>       
                </asp:TemplateField>--%>

                   <asp:TemplateField HeaderText="Manager" SortExpression="mgr_name">
                    <ItemTemplate><%# Eval("mgr_name") %> 
                    </ItemTemplate>       
                </asp:TemplateField>


			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>
                            <td colspan="100%">
                             
                                <div id="diva<%# Eval("action_id") %>" style="display:none;position:relative;left:10px;OVERFLOW: auto;WIDTH:99%; margin-bottom:20px;" >
                                     <div style="margin-left:40px; margin-bottom:10px;">
                                         <%# Eval("description") %>
                                     </div>              
                                </div>
                             </td>
                        </tr>
                       
			        </ItemTemplate>			       
			    </asp:TemplateField>	
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

 
    
<asp:SqlDataSource ID="ds_OpenActions" runat="server"    ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select mn.name as emp_name, mn.deptName as dept, sup.name as mgr_name, act.action_id, 'A' || act.action_id  as action_id2 
    ,act.title as atitle, to_char(act.DUE_DATE,'MM/DD/YYYY') as dueDate,  act.description , issue.issue_id,issue.title as ititle, issue.sig_level
from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path 
FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP' and level>=1  
 START WITH emp.key = :UserID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID 
 join siims_action act on act.owner_sid=mn.key and act.IS_ACTIVE='Y' and act.status_id=22 join siims_issue issue on issue.issue_id=act.issue_id order by act.action_id desc">
    <SelectParameters>
    <asp:SessionParameter Name="UserID" SessionField="LoginSID" />
</SelectParameters>
        </asp:SqlDataSource>
</asp:Content>
