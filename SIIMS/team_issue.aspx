<%@ Page Title="Team Issues" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="team_issue.aspx.cs" Inherits="SIIMS.team_issue" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script  type="text/javascript">
    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            img.src = "img/minus.png";
           
            img.alt = "Close to view other Customers";
        }
        else
        {
            div.style.display = "none";
            img.src = "img/plus.png";
          
            img.alt = "Expand to show Orders";
        }
    } 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
      <div  class="pageHeader"> My Team Open Issues</div>
  
    
    <div style="text-align:left;">
This lists all open issues of your department owns.
        </div>
      

    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large;margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      My Team Open Issues  &nbsp; &nbsp; &nbsp; <asp:HyperLink Visible="false" Font-Size="20px" NavigateUrl="~/team_issue.aspx" ID="linkMyTeamIssue" runat="server">My Team Open Issues</asp:HyperLink>
    
   </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GridView1" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="false" DataSourceID="ds_OpenIssues" DataKeyNames="ISSUE_ID"
            runat="server"  OnRowDataBound="GridView1_RowDataBound" 
            OnRowCommand = "GridView1_RowCommand" 
             GridLines="None" AllowSorting="true" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <a href="javascript:expandcollapse('divi<%# Eval("ISSUE_ID") %>', 'one');">
                            <img id="imgdivi<%# Eval("ISSUE_ID") %>" alt="Click to show/hide Orders for Issue <%# Eval("ISSUE_ID") %>"  width="9px" border="0" src="img/plus.png"/>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Issue ID" SortExpression="ISSUE_ID"  ItemStyle-Width="10%">
                    <ItemTemplate>
                        <asp:Label ID="lblIssueID" Text='<%# Eval("ISSUE_ID") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                 
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="Title">
                    <ItemTemplate>
                         <a href='issue_view.aspx?from=team&iid=<%# Eval("ISSUE_ID") %>'><%# Eval("Title") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
               
             
                
                 <asp:TemplateField HeaderText="Level" SortExpression="sig_level" ItemStyle-Width="10%">
                    <ItemTemplate><%# Eval("sig_level") %> 
                    </ItemTemplate>
                  
                </asp:TemplateField>

                
                <asp:TemplateField HeaderText="Source Title" ItemStyle-HorizontalAlign="Left" SortExpression="stitle">
                    <ItemTemplate><%# Eval("stitle")%></ItemTemplate>                 
                </asp:TemplateField>

                
                  <asp:TemplateField HeaderText="Owner" SortExpression="emp_name">
                    <ItemTemplate><%# Eval("emp_name") %> 
                    </ItemTemplate>       
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Department" SortExpression="dept">
                    <ItemTemplate><%# Eval("dept") %> 
                    </ItemTemplate>       
                </asp:TemplateField>

                   <asp:TemplateField HeaderText="Manager" SortExpression="mgr_name">
                    <ItemTemplate><%# Eval("mgr_name") %> 
                    </ItemTemplate>       
                </asp:TemplateField>


			    
			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>
                            <td colspan="100%">
                             
                                <div id="divi<%# Eval("issue_id") %>" style="display:none;position:relative;left:10px;OVERFLOW: auto;WIDTH:99%; margin-bottom:20px;" >
                                     <div style="margin-left:40px; margin-bottom:20px;">
                                         <%# Eval("description") %>
                                    </div>
                     <div style="margin-left:40px; ">
                                 <asp:GridView ID="GridView2" AllowPaging="False" CellSpacing="5" AllowSorting="false" BackColor="White" Width="100%"
                                        AutoGenerateColumns="false"  runat="server" DataKeyNames="issue_id" 
                                        OnRowCommand = "GridView2_RowCommand" 
                                      OnRowDataBound = "GridView2_RowDataBound"
                                     ShowFooter="false"
                                      GridLines="None">
            <RowStyle BackColor="White" Height="30px"   Font-Bold="true" Font-Names="Arial" Font-Size="12px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="30px"    Font-Bold="true" Font-Names="Arial" Font-Size="12px"/>
            <HeaderStyle BackColor="#DEFECE" Height="33px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black" />

                                        <Columns>
                                            <asp:TemplateField HeaderText="Action ID" SortExpression="action_id"  ItemStyle-Width="10%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblActionID2" Text='<%# Eval("action_id2") %>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Action Title" SortExpression="ActTitle" ItemStyle-HorizontalAlign="Left"
                                                 ItemStyle-Width="50%" >
                                                <ItemTemplate><%# Eval("ActTitle")%></ItemTemplate>
                                            </asp:TemplateField>

                                               <asp:TemplateField HeaderText="Status/Due"  ItemStyle-HorizontalAlign="Left"
                                                   ItemStyle-Width="15%">
                                                <ItemTemplate><%# FORMATACTIONStatus(Eval("status").ToString(), Eval("due_date").ToString()) %></ItemTemplate>                                                        
                                            </asp:TemplateField>
                                        
                                          

                                               <asp:TemplateField HeaderText="Owner" SortExpression="owner" ItemStyle-HorizontalAlign="Left"  ItemStyle-Width="15%">
                                                <ItemTemplate><%# Eval("owner")%></ItemTemplate>         
                                            </asp:TemplateField>
                                                                                         
                                           

                                         
                                        </Columns>
                                   </asp:GridView>
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



    
<asp:SqlDataSource ID="ds_OpenIssues" runat="server"    ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select issue.issue_id,mn.name as emp_name, mn.deptname as dept, sup.name as mgr_name, issue.title,issue.sig_level
    , source.title as stitle,  issue.description
from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path 
 FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP'and level>=1  
 START WITH emp.key = :UserID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID 
 join siims_issue issue on issue.owner_sid=mn.key and issue.IS_ACTIVE='Y' and issue.status_id=11
 join SIIMS_SOURCE source on issue.issue_id=source.issue_id and source.is_active='Y'">
    <SelectParameters>
    <asp:SessionParameter Name="UserID" SessionField="LoginSID" />
</SelectParameters>
        </asp:SqlDataSource>

</asp:Content>
