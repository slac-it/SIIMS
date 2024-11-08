﻿<%@ Page Title="Action by Organization" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_orgAction.aspx.cs" Inherits="SIIMS.rep_orgAction" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Action Report by Organization
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_OrgIssues" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_orgActions" DataKeyNames="org_id"
           Font-Size="Large"  runat="server" 
             GridLines="None" AllowSorting="True" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
               
                <asp:TemplateField HeaderText="Organization" HeaderStyle-CssClass ="paddingCell"
                   ItemStyle-CssClass="paddingCell" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                       <%# Eval("name") %>
                    </ItemTemplate>       
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

<%--                <asp:TemplateField HeaderText="Number of Draft Actions" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NDraft") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>--%>

                 <asp:TemplateField HeaderText="Waiting for Acceptance" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NWaiting") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>
               
               <asp:TemplateField HeaderText="Open" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NOpen") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Closed" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("NClose") %> 
                      </ItemTemplate>          
<ItemStyle HorizontalAlign="Left"></ItemStyle>
                </asp:TemplateField>

			  		    
			    <asp:HyperLinkField DataNavigateUrlFields="org_id" DataTextField="total" HeaderText="Total" DataNavigateUrlFormatString="rep_orgAction_Listing.aspx?org_id={0}" />
               
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

      <asp:SqlDataSource ID="ds_orgActions" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.org_id,org.NAME,  
count(case when action.status_id = 20 then 1 end) as NDraft,
count(case when action.status_id = 21 then 1 end) as NWaiting,
count(case when action.status_id = 22 then 1 end) as NOpen,
count(case when action.status_id = 23 then 1 end) as NClose,
count(case when action.status_id != 20 then 1 end) as Total
from siims_action action
join siims_issue issue on action.issue_id=issue.issue_id
join siims_org org on issue.org_id=org.org_id
where action.is_active='Y' and issue.org_id is not null
group by issue.org_id, org.NAME
order by 2"   >
           
    </asp:SqlDataSource>   

</asp:Content>
