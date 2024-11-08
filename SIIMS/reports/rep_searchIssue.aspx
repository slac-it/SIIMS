<%@ Page Title="" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_searchIssue.aspx.cs" Inherits="SIIMS.reports.rep_searchIssue" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

 &nbsp;&nbsp; &nbsp;&nbsp;Enter keyword to search Issues: <asp:TextBox ID="TxtKeyword" runat="server"  onkeydown="if (event.keyCode == 13) document.getElementByID('BtnGo').click()"></asp:TextBox> 
    <asp:Button ID="BtnGo" runat="server" Text="Go" TabIndex="1"  Font-Bold="true" Font-Size="Large" OnClick="BtnGo_Click" />
    &nbsp;&nbsp; <span style="font-size:small;color:darkblue;">Note: This search is performed on Title and Description </span>

      <table  cellspacing="0" cellpadding="0" align="center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
     Issues by Keyword   &nbsp; &nbsp; &nbsp; &nbsp;
       <asp:LinkButton ID="LnkExcel" runat="server" OnClick="LnkExcel_Click" Font-Size="Medium" Text="Export to Excel" TabIndex="100"></asp:LinkButton>
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_Issues" AllowPaging="True" BackColor="#F1F1F1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="False" DataSourceID="ds_Issues" DataKeyNames="issue_id"  CssClass="gridviewCSS"
           Font-Size="Large"  runat="server" 
             GridLines="None" AllowSorting="True" EmptyDataText="No Issues found" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>

              

                <asp:TemplateField HeaderText="Issue ID" ItemStyle-HorizontalAlign="Left"  SortExpression="Issue_ID" ItemStyle-Width="6%">
                    <ItemTemplate>
                       <%#Eval("Issue_ID") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
               
                <asp:HyperLinkField DataNavigateUrlFields="issue_id" DataTextField="Title" 
                    ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"               
                     HeaderText="Title"  SortExpression="title"
                    DataNavigateUrlFormatString="../issue_view.aspx?iid={0}" />

                 
                <asp:TemplateField HeaderText="Organization" ItemStyle-HorizontalAlign="Left" SortExpression="org">
                    <ItemTemplate>
                       <%# Eval("org") %>
                    </ItemTemplate>       
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Owner" ItemStyle-HorizontalAlign="Left" SortExpression="Owner" >
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Source Title" ItemStyle-HorizontalAlign="Left" SortExpression="stitle" >
                    <ItemTemplate>
                       <%#Eval("stitle") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
                
                
                <asp:TemplateField HeaderText="Level" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="5%" SortExpression="sig_level">
                    <ItemTemplate>
                       <%#Eval("sig_level") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>

                </asp:TemplateField>


                  <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Left" SortExpression="status" >
                    <ItemTemplate>
                       <%#Eval("status") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>
			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

      <asp:SqlDataSource ID="ds_Issues" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select issue.issue_id, issue.title, issue.description, issue.owner_sid, p.name as owner, issue.SIG_LEVEL, issue.status_id, sta.status,issue.stype_id,src.type,  s.title as stitle, org.name as org
from siims_issue issue left join siims_org org on issue.org_id=org.org_id join siims_status sta on issue.status_id=sta.status_id left join persons.person p on p.key=issue.owner_sid left join siims_source_type src on src.stype_id=issue.stype_id left join siims_source s on issue.issue_id=s.issue_id where issue.is_active='Y'  and issue.status_id in (11,12)
order by 1"  FilterExpression="title LIKE '%{0}%' OR description LIKE '%{0}%'"  >
          <FilterParameters>
              <asp:ControlParameter ControlID="TxtKeyword" PropertyName="Text" />
          </FilterParameters>
    </asp:SqlDataSource> 

    <asp:HiddenField ID="hdnkeyword" runat="server" />
</asp:Content>
