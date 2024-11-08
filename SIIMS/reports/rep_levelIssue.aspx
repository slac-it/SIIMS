<%@ Page Title="Issues by Level" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_levelIssue.aspx.cs" Inherits="SIIMS.rep_levelIssue" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     
    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Issue Report by Level
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_OrgIssues" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_srcIssues" DataKeyNames="sig_level"
           Font-Size="Large"  runat="server" 
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
               
                <asp:TemplateField HeaderText="Level" HeaderStyle-CssClass ="paddingCell"
                   ItemStyle-CssClass="paddingCell" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                       <%# Eval("sig_level") %>
                    </ItemTemplate>       
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

<%--                <asp:TemplateField HeaderText="Number of Draft Issues" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NDraft") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>--%>
               
               <asp:TemplateField HeaderText="Open Issues" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NOpen") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Closed Issues" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NClose") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>

			  		    
			    <asp:HyperLinkField DataNavigateUrlFields="sig_level" DataTextField="total" HeaderText="Total" DataNavigateUrlFormatString="rep_levelIssue_Listing.aspx?level={0}" />
               
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

      <asp:SqlDataSource ID="ds_srcIssues" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.sig_level,count(case when issue.status_id = 10 then 1 end) as NDraft,
 count(case when issue.status_id = 11 then 1 end) as NOpen,
  count(case when issue.status_id = 12 then 1 end) as NClose,
count(case when issue.status_id <> 10 then 1 end) as Total
from siims_issue issue
where issue.is_active='Y' and issue.sig_level is not null
group by issue.sig_level
order by 1"   >
           
    </asp:SqlDataSource>   

</asp:Content>
