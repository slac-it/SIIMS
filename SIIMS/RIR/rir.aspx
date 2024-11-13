<%@ Page Title="" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="rir.aspx.cs" Inherits="SIIMS.RIR.rir" %>
<%@ Register Src="~/UserControl/getSSOvariables.ascx" TagName="getVar" TagPrefix="uc"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <uc:getVar ID="getVar" runat="server" />

    <div style="margin-bottom:5px;">
    <table cellspacing="2" cellpadding="4" align="left" rules="all" border="1" width="100%" style="color:#333333;font-size: large">
        <caption  class="fieldHeader">
			  RIR	 </caption>
        <tr style="color:White;background-color:#5D7B9D;font-weight:bold;">
            <th scope="col" style="width:20%;">RIR Draft</th>
			<th scope="col"  style="width:20%;">Submitted to RIR Coordinator</th>
            <th scope="col"  style="width:20%;">Submitted for Review to RPFO & DREP</th>
            <th scope="col"  style="width:20%;">Submitted for Approval <br />to RIR Coordinator</th>
             <th scope="col">RIR Completed</th>
         <%--   <th>RIR Closed</th>--%></tr>
        <asp:ListView ID="lv_Changes" DataSourceID="ds_Status"     runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333;">
       <td style="text-align:center; vertical-align:top;"> 
            <a href='rir.aspx?s=D'>  <%# Eval("No_Draft").ToString() %>  </a>   </td>
      <td style="text-align:center; vertical-align:top;"> 
            <a href='rir.aspx?s=E'>  <%# Eval("No_Edit").ToString() %>  </a>   </td>
    <td style="text-align:center; vertical-align:top;"> 
            <a href='rir.aspx?s=R'>  <%# Eval("No_Review").ToString() %>  </a>   </td>
       <td style="text-align:center; vertical-align:top;"> 
            <a href='rir.aspx?s=A'>  <%# Eval("No_APP").ToString() %>  </a>   </td>
       <td style="text-align:center; vertical-align:top;"> 
            <a href='rir.aspx?s=C'>  <%# Eval("No_Closed").ToString() %>  </a>   </td>
  </tr>
</ItemTemplate>
                </asp:ListView>
    </table>
    </div>
     <asp:Panel ID="PanelList" Visible="false" runat="server">

    <table  cellspacing="0" cellpadding="0" align="Center" width="100%" rules="all" border="1" style="color:#333333;font-size: large; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
   <asp:Literal ID="lit_Title" runat="server"></asp:Literal>
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
     <asp:GridView ID="gvw1" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="20"  CellPadding="5" AllowSorting="true"
          BackColor="#f1f1f1"  width="100%"  GridLines="None"  onrowdatabound="gvw1_RowDataBound"
         DataSourceID="ds_Report" DataKeyNames="issue_id"  EmptyDataText="No reports found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" >
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText=" ID&nbsp;" DataField="issue_id" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="7%" SortExpression="issue_id"  />
             <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="Title" ItemStyle-Width="25%">
                    <ItemTemplate>
                         <a href='report_view.aspx?from=d&iid=<%# Eval("issue_id") %>'><%# Eval("Title") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
             
                <asp:BoundField HeaderText="Organization" DataField="Org_Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" SortExpression="Org_Name" />
                <asp:BoundField HeaderText="Department" DataField="DEPT_NAME" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%" SortExpression="DEPT_NAME" />
              <asp:BoundField HeaderText="Trending Code" DataField="code" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%" SortExpression="Code" />
             <asp:BoundField HeaderText="Discovered" DataField="date_disc" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" SortExpression="date_disc"  />
           <asp:TemplateField HeaderText="To Do" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%">
                    <ItemTemplate>
                        <%# FormatAction( Eval("issue_id").ToString(),  Eval("rir_status").ToString(), Eval("date_closed").ToString()) %>
                     
                      </ItemTemplate>          
                </asp:TemplateField>
            </Columns>      
 
    </asp:GridView>
         
</td> </tr>
    </table>
             </asp:Panel>
    <div>
        &nbsp;
    </div>
 <%--   <div style="padding:20px; height:150px;">
        <a href="migration_list.aspx" style="padding-top:20px;font-weight:bold; ">List of Data Migration Reports</a>
    </div>--%>
    <br />   
      <asp:SqlDataSource ID="ds_Report" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand=" select issue.issue_id, issue.title, p.name as Initiator, to_char(rep.DATE_DISCOVERED,'MM/DD/YYYY') as date_disc, org.NAME as Org_Name,issue.DEPT_NAME, rep.rir_status
, (select listagg(code.CODE, '; ') WITHIN GROUP(ORDER BY NULL)
from SIIMS_RIR_REPORTCODE rcode join SIIMS_RIR_CODE code on code.RIRCODE_ID=rcode.RIRCODE_ID where rcode.is_active = 'Y' 
and rcode.issue_id = issue.issue_id) as Code, app.date_closed
from  siims_issue issue 
join SIIMS_RIR_REPORT rep on issue.issue_id=rep.ISSUE_ID
join persons.person p on p.key=issue.CREATED_BY
 left join (select issue_id, to_char(max(date_respond),'MM/DD/YYYY') as date_closed from siims_rir_report_approve  where is_active='Y' and IS_ACCEPTED='Y'
group by issue_id) app on app.issue_id=issue.issue_id
left join siims_org org on org.ORG_ID=issue.ORG_ID
where issue.is_rir='Y' and issue.is_active='Y' and rep.rir_status=:Status
order by issue.CREATED_ON desc"> 
           <SelectParameters>
                       <asp:QueryStringParameter Name="Status" QueryStringField="s" DefaultValue="" Type="String" />
              </SelectParameters>
        </asp:SqlDataSource>

       <asp:SqlDataSource ID="ds_Status" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select (select count(*) 
from  siims_issue issue 
join SIIMS_RIR_REPORT rep on issue.issue_id=rep.ISSUE_ID
where issue.is_rir='Y' and issue.is_active='Y' and rep.rir_status='E') as No_Edit,
          ( select count(*) 
from  siims_issue issue 
join SIIMS_RIR_REPORT rep on issue.issue_id=rep.ISSUE_ID
where issue.is_rir='Y' and issue.is_active='Y' and rep.rir_status='D') as No_Draft,
          ( select count(*) 
from  siims_issue issue 
join SIIMS_RIR_REPORT rep on issue.issue_id=rep.ISSUE_ID
where issue.is_rir='Y' and issue.is_active='Y' and rep.rir_status='C') as No_Closed,
(select count(*) 
from  siims_issue issue 
join SIIMS_RIR_REPORT rep on issue.issue_id=rep.ISSUE_ID
where issue.is_rir='Y' and issue.is_active='Y' and rep.rir_status='R') as No_Review,
(select count(*) 
from  siims_issue issue 
join SIIMS_RIR_REPORT rep on issue.issue_id=rep.ISSUE_ID
where issue.is_rir='Y' and issue.is_active='Y' and rep.rir_status='A') as No_APP
from dual"> 
        </asp:SqlDataSource>

</asp:Content>
