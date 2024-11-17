<%@ Page Title="SIIMS Home" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SIIMS._default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <script  type="text/javascript">
      $(window).bind("pageshow", function () {
          // update hidden input field 
          $('#form1')[0].reset();
      });

    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            img.src = "img/minus.png";        
        }
        else
        {
            div.style.display = "none";
            img.src = "img/plus.png";
          
        }
    }

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

<table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      My Open Issues  &nbsp; &nbsp; &nbsp; <asp:HyperLink Visible="false" Font-Size="20px" NavigateUrl="~/team_issue.aspx" ID="linkMyTeamIssue" runat="server">My Team Open Issues</asp:HyperLink>
    
   </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GridView1" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="10"
            AutoGenerateColumns="false" DataSourceID="ds_OpenIssues" DataKeyNames="ISSUE_ID"
            runat="server"  OnRowDataBound="GridView1_RowDataBound" 
             GridLines="None" AllowSorting="true" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <a href="javascript:expandcollapse('divi<%# Eval("ISSUE_ID") %>', 'one');">
                            <img id="imgdivi<%# Eval("ISSUE_ID") %>" alt="Click to show/hide Orders for Issue <%# Eval("ISSUE_ID") %>"  width="9px" border="0" src="img/plus.png"/>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Issue ID" ItemStyle-Width="10%"  SortExpression="ISSUE_ID">
                    <ItemTemplate>
                        <asp:Label ID="lblIssueID" Text='<%# Eval("ISSUE_ID") %>' runat="server"></asp:Label>
                         <asp:Label ID="lblIS_RIR" Visible="false" Text='<%# Eval("is_rir") %>' runat="server"></asp:Label>
                         <asp:Label ID="lblSub115" Visible="false" Text='<%# Eval("sub115") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                 
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="Title">
                    <ItemTemplate>
                         <a href='issue_view.aspx?iid=<%# Eval("ISSUE_ID") %>'><%# Eval("Title") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
               
             
                
                 <asp:TemplateField HeaderText="Level" ItemStyle-Width="5%"  SortExpression="sig_level">
                    <ItemTemplate><%# Eval("sig_level") %> 
                    </ItemTemplate>
                </asp:TemplateField>

                
                <asp:TemplateField HeaderText="Source" ItemStyle-HorizontalAlign="Left" SortExpression="stitle">
                    <ItemTemplate><%# Eval("stitle")%></ItemTemplate>                 
                </asp:TemplateField>


                   <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%"  HeaderText="Action">
                    <ItemTemplate>
                        <asp:DropDownList ID="drwIssueSIIMS"  DataTextField="type" DataValueField="id" Visible="false" OnSelectedIndexChanged="ddlDropDownList_SelectedIndexChanged"
                             AppendDataBoundItems="true" DataSourceID="ds_IssuesSIIMSDrpDown" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                         <asp:DropDownList ID="drwIssueRIR"  DataTextField="type" DataValueField="id" Visible="false" OnSelectedIndexChanged="ddlDropDownList_SelectedIndexChanged"
                             AppendDataBoundItems="true" DataSourceID="ds_IssuesRIRDrpDown" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                          <asp:DropDownList ID="drwIssueP1Closure"  Visible="false" OnSelectedIndexChanged="ddlDropDownList_SelectedIndexChanged"
                              AutoPostBack="true" runat="server">
                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                              <asp:ListItem Text="View" Value="115" ></asp:ListItem>
                        </asp:DropDownList>
                       
                    </ItemTemplate>
                   
                </asp:TemplateField>
			    
			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>
                            <td colspan="100%">
                             
                                <div id="divi<%# Eval("issue_id") %>" style="display:none;position:relative;left:10px;OVERFLOW: auto;WIDTH:99%; margin-bottom:20px;" >
                                     <div style="margin-left:40px; margin-bottom:20px;">
                                         <%# Eval("desc2") %>
                                    </div>
                     <div style="margin-left:40px; ">
                                 <asp:GridView ID="GridView2" AllowPaging="False"  CellSpacing="5" AllowSorting="false" BackColor="White" Width="100%"
                                        AutoGenerateColumns="false"  runat="server" DataKeyNames="issue_id" 
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
                                                     <asp:Label ID="lblIS_SIIMS" Visible="false" Text='<%# Eval("is_SIIMS") %>' runat="server"></asp:Label>
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
                                          <asp:DropDownList ID="drwActionInIssueOpenRIR" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownOpenRIR" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="drwActionInIssueDraft" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownDraft" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>

                                        <asp:DropDownList ID="drwActionInIssuePending" Visible="false"  DataTextField="type" DataValueField="id" OnSelectedIndexChanged="ddlActionInIssue_SelectedIndexChanged"
                                             AppendDataBoundItems="true" DataSourceID="ds_ActionDrpdownPending" AutoPostBack="true" runat="server">
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
 
 <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      My Open Actions  &nbsp; &nbsp; &nbsp;  
       <asp:HyperLink Visible="false" Font-Size="20px" NavigateUrl="~/team_action.aspx" ID="linkMyTeamAction" runat="server">My Team Open Actions</asp:HyperLink>
    
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GVActions" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="10"
            AutoGenerateColumns="false" DataSourceID="ds_ACTIONS" DataKeyNames="ACTION_ID"
           Font-Size="Large"  runat="server"  OnRowDataBound="GVActions_RowDataBound" 
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
                <asp:TemplateField HeaderText="Action ID" ItemStyle-Width="10%"  SortExpression="ACTION_ID">
                    <ItemTemplate>
                        <asp:Label ID="lblActionID" Text='<%# Eval("action_id2") %>' runat="server"></asp:Label>
                         <asp:Label ID="lblSIIMS" Visible="false" Text='<%# Eval("is_SIIMS") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                 
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="ATitle">
                    <ItemTemplate>
                         <a href='action_view.aspx?aid=<%# Eval("ACTION_ID") %>'><%# Eval("ATitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
               
               <asp:TemplateField HeaderText="Issue" ItemStyle-HorizontalAlign="Left" SortExpression="ITitle">
                    <ItemTemplate>
                         <a href='issue_view.aspx?iid=<%# Eval("ISSUE_ID") %>'><%# Eval("ITitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
                
                 <asp:TemplateField HeaderText="Due Date"  SortExpression="due_date">
                    <ItemTemplate>
                      <%#FormatDueDate( Eval("due_date").ToString()) %>  
                    </ItemTemplate>
                  
                </asp:TemplateField>

               

                   <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%"  HeaderText="Action">
                    <ItemTemplate>
                        <asp:DropDownList ID="drwActionSIIMSOpen"  DataTextField="type" Visible="false" DataValueField="id" OnSelectedIndexChanged="ddlActions_SelectedIndexChanged"
                             AppendDataBoundItems="true" DataSourceID="ds_ActionSIIMSDrpdown" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="drwActionRIROpen"  DataTextField="type" Visible="false" DataValueField="id" OnSelectedIndexChanged="ddlActions_SelectedIndexChanged"
                             AppendDataBoundItems="true" DataSourceID="ds_ActionRIRDrpdown" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="-- Please Select --" Value="-1" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate> 
                </asp:TemplateField>

			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>
                            <td colspan="100%">
                             
                                <div id="diva<%# Eval("action_id") %>" style="display:none;position:relative;left:10px;OVERFLOW: auto;WIDTH:99%; margin-bottom:20px;" >
                                     <div style="margin-left:40px; margin-bottom:10px;">
                                        
                                         <%# Eval("desc2") %>
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


    <asp:Repeater ID="Repeater1" runat="server" DataSourceID="ds_Orgs">
        <ItemTemplate>
            <div style="margin-top:10px; margin-left:30px;">
                  <%# GetOrgLink(Eval("org_id").ToString(),Eval("org_name").ToString()) %>
               </div>
        </ItemTemplate>
    </asp:Repeater>

<asp:SqlDataSource ID="ds_Orgs" runat="server"    ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select org_id,org_name from vw_siims_org_stakeholders where SLACID=:UserID order by 2 ">
    <SelectParameters>
    <asp:SessionParameter Name="UserID" SessionField="LoginSID" />
</SelectParameters>
   </asp:SqlDataSource>

<asp:SqlDataSource ID="ds_OpenIssues" runat="server"    ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select issue.issue_id, issue.title, issue.sig_level, source.title as stitle, issue.description
    , replace(issue.description, chr(10),'<br />') as desc2, NVL(issue.is_rir,'N') as is_rir,decode(issue.sub_status_id,'115','Y','N') as SUB115
from vw_siims_issue_VIEW issue
join SIIMS_SOURCE source on issue.issue_id=source.issue_id and source.is_active='Y'
where issue.owner_sid=:UserID  and issue.status_id=11
order by 1 desc">
    <SelectParameters>
    <asp:SessionParameter Name="UserID" SessionField="LoginSID" />
</SelectParameters>
        </asp:SqlDataSource>

    
       <asp:SqlDataSource ID="ds_ACTIONS" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand=" select action.action_id, 'A' || action.action_id  as action_id2 , action.title as atitle,to_char(action.due_date, 'MM/DD/YYYY') due_date
           , action.description, replace(action.description, chr(10),'<br />') as desc2, issue.issue_id,issue.title as ititle, issue.sig_level, NVL(action.is_SIIMS,'Y') as is_siims 
from siims_action action join siims_issue issue on issue.issue_id=action.issue_id
where action.owner_sid=:UserID and action.status_id=22 and action.is_active='Y' 
           and action.action_id not in (select action_id from SIIMS_ACTION_CHANGE where SUB_STATUS_ID in (221,222) and is_active='Y')
           order by 1 desc "  >
            <SelectParameters>
    <asp:SessionParameter Name="UserID" SessionField="LoginSID" />
</SelectParameters>
    </asp:SqlDataSource>    
    
     <asp:SqlDataSource ID="ds_IssuesSIIMSDrpDown" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Edit' as type, '3' as ID from dual
            UNION  select 'Close' as type, '2' as ID from dual   UNION  select 'Change Level' as type, '4' as ID from dual
           UNION  select 'Delete' as type, '5' as ID from dual UNION  select 'Create Action' as type, '1' as ID from dual
           order by 2
          "  >         
    </asp:SqlDataSource>   

      
     <asp:SqlDataSource ID="ds_IssuesRIRDrpDown" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Close' as type, '2' as ID from dual   UNION  select 'Change Level' as type, '4' as ID from dual
           UNION  select 'Create Action' as type, '1' as ID from dual
           order by 2
          "  >         
    </asp:SqlDataSource>   

     <asp:SqlDataSource ID="ds_ActionSIIMSDrpdown" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Close' as type, '1' as ID from dual union select 'Edit' as type, '2' as ID from dual 
            UNION  select 'Change Due Date' as type, '3' as ID from dual    UNION  select 'Delete' as type, '4' as ID from dual 
           order by 2
          "  >  
     </asp:SqlDataSource>   

        <asp:SqlDataSource ID="ds_ActionRIRDrpdown" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Upload/Close' as type, '1' as ID from dual UNION  select 'Change Due Date' as type, '3' as ID from dual     order by 2
          "  >  
     </asp:SqlDataSource>  

        <asp:SqlDataSource ID="ds_ActionDrpdownOpen" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Close' as type, '1' as ID from dual union select 'Edit' as type, '2' as ID from dual union select 'Change Owner' as type, '3' as ID from dual 
            UNION  select 'Change Due Date' as type, '4' as ID from dual    UNION  select 'Delete' as type, '5' as ID from dual 
           order by 2
          "  >    
    </asp:SqlDataSource>   

      <asp:SqlDataSource ID="ds_ActionDrpdownOpenRIR" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select 'Close' as type, '1' as ID from dual  union select 'Change Owner' as type, '3' as ID from dual 
            UNION  select 'Change Due Date' as type, '4' as ID from dual   order by 2
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
