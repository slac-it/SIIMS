<%@ Page Title="RIR Admin Report" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_numberlisting.aspx.cs" Inherits="SIIMS.admin.rir_numberlisting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div  class="pageHeader">
         <asp:Literal ID="LitTitle" runat="server">              
        Open and Closed Actions Report
         </asp:Literal>
    </div>
<table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
    Actions where you are the Issue owner  &nbsp; &nbsp; &nbsp; &nbsp; 
       <asp:ImageButton ID="ImgBtnExport" runat="server" 
        ImageUrl="~/img/ExportToExcel.gif"  onclick="ImgBtnExport_Click" />
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GV_Actions"  BackColor="#F1F1F1"  width="100%" CellSpacing="5" 
            AutoGenerateColumns="False"  DataKeyNames="action_id"
           Font-Size="Large"  runat="server"  EmptyDataText="No Actions Found!"
             GridLines="None" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
               <%--   <asp:TemplateField HeaderText="Manager" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("mgr_name") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>--%>


                  <asp:TemplateField HeaderText="Action ID" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("Action_AID") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                   		    
			    <asp:HyperLinkField DataNavigateUrlFields="action_id" DataTextField="ATitle" 
                    ItemStyle-CssClass="paddingCell" HeaderStyle-CssClass="paddingCell"   HeaderText="Action Title"  
                    DataNavigateUrlFormatString="../action_view.aspx?aid={0}" />              
           
                   <asp:TemplateField HeaderText="Action Owner" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("Owner") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

			   <asp:TemplateField HeaderText="Current Status" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("status") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Due Date" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                       <%#Eval("duedate") %> 
                      </ItemTemplate>          
<HeaderStyle CssClass="paddingCell"></HeaderStyle>
<ItemStyle HorizontalAlign="Left" CssClass="paddingCell"></ItemStyle>
                </asp:TemplateField>

			  		    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>
</asp:Content>
