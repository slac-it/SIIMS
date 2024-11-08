<%@ Page Title="Issues for My Org" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="issue_org.aspx.cs" Inherits="SIIMS.issue_org" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


      <script  type="text/javascript">
    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            img.src = "img/minus.gif";
           
            img.alt = "Close to view other Customers";
        }
        else
        {
            div.style.display = "none";
            img.src = "img/plus.gif";
 
            img.alt = "Expand to show Orders";
        }
    } 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    
  <asp:Label ID="lblTitle" runat="server" Text="  My Organization Open Issues  "></asp:Label>
   </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GridView1" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="false" DataSourceID="ds_OpenIssues" DataKeyNames="ISSUE_ID" CssClass="gridviewCSS"
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
                <asp:TemplateField HeaderText="Issue ID" SortExpression="ISSUE_ID" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <asp:Label ID="lblIssueID" Text='<%# Eval("ISSUE_ID") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                 
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="Title">
                    <ItemTemplate>
                         <a href='issue_view.aspx?from=org&iid=<%# Eval("ISSUE_ID") %>'><%# Eval("Title") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
               
             
                
                 <asp:TemplateField HeaderText="Level" SortExpression="sig_level">
                    <ItemTemplate><%# Eval("sig_level") %> 
                    </ItemTemplate>
                  
                </asp:TemplateField>

                
                <asp:TemplateField HeaderText="Source" ItemStyle-HorizontalAlign="Left" SortExpression="stitle">
                    <ItemTemplate><%# Eval("stitle")%></ItemTemplate>                 
                </asp:TemplateField>


                   <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%"  HeaderText="Action">
                    <ItemTemplate>
                        <asp:DropDownList ID="drwIssueActions"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlDropDownList_SelectedIndexChanged"
                             AppendDataBoundItems="true" DataSourceID="ds_IssuesDrpDown" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                       
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
                                                     <asp:Label ID="lblStatusID" Visible="false" Text='<%# Eval("status_id") %>' runat="server"></asp:Label>
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
                                                                                         

                                     <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%"  HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="drwActionInIssueOpen" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownOpen" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="drwActionInIssueDraft" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownDraft" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>

                                        <asp:DropDownList ID="drwActionInIssuePending" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownpending" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>

                                          <asp:DropDownList ID="drwActionInIssueClosed" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownClosed" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                       
                                    </ItemTemplate> 
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
            SelectCommand="select issue.issue_id, issue.title, issue.sig_level,  NVL(issue.sub_status_id,'') as sub_status_id,org.name,
     source.title as stitle, issue.description
from siims_issue issue
join SIIMS_SOURCE source on issue.issue_id=source.issue_id and source.is_active='Y'
join siims_status status on  issue.status_id=status.STATUS_ID and  status.status='Open' and status.is_active='Y' and status.object_id=1
join SIIMS_ORG org on org.org_id=issue.org_id
where  issue.is_active='Y' and issue.org_id = :OID
order by issue.created_on desc">
    <SelectParameters>
    <asp:QueryStringParameter QueryStringField="orgID" DbType="Int32" Name="OID" />
</SelectParameters>
        </asp:SqlDataSource>
    
     <asp:SqlDataSource ID="ds_IssuesDrpDown" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Edit' as type, '3' as ID from dual
            UNION  select 'Close' as type, '2' as ID from dual   UNION  select 'Change Level' as type, '4' as ID from dual
           UNION  select 'Delete' as type, '5' as ID from dual UNION  select 'Create Action' as type, '1' as ID from dual
           order by 2
          "  >         
    </asp:SqlDataSource>   
    
        <asp:SqlDataSource ID="ds_ActionDrpdownOpen" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Close' as type, '1' as ID from dual union select 'Edit' as type, '2' as ID from dual union select 'Change Owner' as type, '3' as ID from dual 
            UNION  select 'Change Due Date' as type, '4' as ID from dual    UNION  select 'Delete' as type, '5' as ID from dual 
           order by 2
          "  >    
    </asp:SqlDataSource>   

      <asp:SqlDataSource ID="ds_ActionDrpdownDraft" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Complete' as type, '12' as ID from dual  UNION  select 'Delete' as type, '15' as ID from dual 
           order by 2
          "  >    
    </asp:SqlDataSource>   

      <asp:SqlDataSource ID="ds_ActionDrpdownPending" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand=" select 'Edit' as type, '12' as ID from dual UNION  select 'Delete' as type, '5' as ID from dual 
           order by 2
          "  >    
    </asp:SqlDataSource>   

     <asp:SqlDataSource ID="ds_ActionDrpdownClosed" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand=" select 'View' as type, '22' as ID from dual 
           order by 2
          "  >    
    </asp:SqlDataSource>  

</asp:Content>
