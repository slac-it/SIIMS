<%@ Page Title="Reports" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="rep_cy_data.aspx.cs" Inherits="SIIMS.RIR.rep_cy_data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <%--<div class="fieldHeader">
        This is what I get:
   </div>
    <br />
    Organizations: <asp:Label ID="lblOrg" runat="server" Text="Label"></asp:Label>
    <br />
     Year: <asp:Label ID="lblYear" runat="server" Text="Label"></asp:Label>
     <br />
      Quarter: <asp:Label ID="lblQuarter" runat="server" Text="Label"></asp:Label>
    <br />
    Keyword: <asp:Label ID="lblKeyword" runat="server" Text="Label"></asp:Label>
    <br />
   Final SQL query: <br />
  <asp:Label ID="lblSQL" runat="server" Text="Label"></asp:Label>--%>
      <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
  Closed Reports by Calendar Year &nbsp; &nbsp; &nbsp; &nbsp;<asp:ImageButton ID="ImgBtnExport" runat="server" 
        ImageUrl="~/img/ExportToExcel.gif"  onclick="ImgBtnExport_Click" />
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
  <asp:GridView ID="gvw1" runat="server" AutoGenerateColumns="false"  CellPadding="5" 
          BackColor="#f1f1f1"  width="100%"  GridLines="None" AllowSorting="true" OnSorting="gvw1_Sorting"
          EmptyDataText="No reports found." HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" >
          <RowStyle BackColor="White" Height="40px"  VerticalAlign="Top" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px" VerticalAlign="Top" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
         <PagerStyle  HorizontalAlign="Left" CssClass="pager"  />
         <Columns>
                <asp:BoundField HeaderText="RIR ID&nbsp;" DataField="rir_id" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%" SortExpression="rir_id"  />
             <asp:TemplateField HeaderText="Title" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%" SortExpression="Title">
                    <ItemTemplate>
                         <a href='report_view.aspx?from=d&iid=<%# Eval("issue_id") %>'><%# Eval("Title") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>
             
                <asp:BoundField HeaderText="Organization" DataField="Org_Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" SortExpression="Org_Name" />
                <asp:BoundField HeaderText="Department" DataField="DEPT_NAME" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%"   SortExpression="DEPT_NAME"/>
              <asp:BoundField HeaderText="Trending Code" DataField="Tracking_Code" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="20%"  SortExpression="Tracking_Code" />
             <asp:BoundField HeaderText="Discovered" DataField="date_discovered" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%"  SortExpression="date_discovered" />
          <asp:BoundField HeaderText="Issued Date" DataField="issued_date" ItemStyle-HorizontalAlign="Left"  SortExpression="issued_date" />
            </Columns>      
 
    </asp:GridView>
        
</td> </tr>
    </table>
</asp:Content>
