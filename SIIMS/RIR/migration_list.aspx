<%@ Page Title="List of Data Migration Reports" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="migration_list.aspx.cs" Inherits="SIIMS.RIR.migration_list" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <table  cellspacing="0" cellpadding="0" align="Center" width="100%" rules="all" border="1" style="color:#333333;font-size: large; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
  List of Reports from Data Migration File
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
     <asp:GridView ID="gvw1" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="30"  CellPadding="5" AllowSorting="true"
          BackColor="#f1f1f1"  width="100%"  GridLines="None"  
         DataSourceID="ds_Migration" DataKeyNames="issue_id"  EmptyDataText="No reports found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" >
          <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText=" ID&nbsp;" DataField="issue_id" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="5%" SortExpression="issue_id"  />
             <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" SortExpression="Title" ItemStyle-Width="30%">
                    <ItemTemplate>
                         <a href='migration_view.aspx?iid=<%# Eval("issue_id") %>'><%# Eval("Title") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
             
                <asp:BoundField HeaderText="Organization" DataField="Org_Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" SortExpression="Org_Name" />
            <%--    <asp:BoundField HeaderText="Department" DataField="DEPT_NAME" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%" SortExpression="DEPT_NAME" />--%>
              <asp:BoundField HeaderText="Trending Code" DataField="code" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="25%" SortExpression="Code" />
             <asp:BoundField HeaderText="Discovered" DataField="date_disc" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" SortExpression="date_disc"  />
           <asp:TemplateField HeaderText="Edit Statement" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                         <ItemTemplate>
                         <a href='migration_edit.aspx?iid=<%# Eval("issue_id") %>'>Edit Statement</a>    
                      </ItemTemplate>  
                </asp:TemplateField>
              <asp:TemplateField HeaderText="Attachment" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                   <ItemTemplate>
                        <%# FormatAction(Eval("issue_id").ToString(), Eval("IS_PDF").ToString()) %>                   
                      </ItemTemplate>   
    
                </asp:TemplateField>
            </Columns>      
 
    </asp:GridView>
         
</td> </tr>
    </table>
     <asp:SqlDataSource ID="ds_Migration" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand=" select issue_id,title, INITIATOR_NAME_ORIG as Initiator, to_char(DATE_DISCOVERED_GENERATED,'MM/DD/YYYY') as date_disc,
SMT_ORG_ORIG as Org_Name, RIR_CODE_GENERATED as code,NVL(PDF_FILENAME,'') as IS_PDF
from APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION 
order by issue_id"> 
        </asp:SqlDataSource>
</asp:Content>
