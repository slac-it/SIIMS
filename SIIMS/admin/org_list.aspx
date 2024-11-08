<%@ Page Title="Org Listing" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="org_list.aspx.cs" Inherits="SIIMS.admin.org_list" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script  type="text/javascript">
     

    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            img.src = "../img/minus.png";        
        }
        else
        {
            div.style.display = "none";
            img.src = "../img/plus.png";
          
        }
    }

   

   
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <table  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left; font-size:24px; padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      SMT Organizations  &nbsp; &nbsp; &nbsp; &nbsp;<a href="org_add.aspx">Add New SMT Org</a>
   </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GridView1" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="20"
            AutoGenerateColumns="false" DataSourceID="ds_activeOrgs" DataKeyNames="org_id"
            runat="server"  OnRowDataBound="GridView1_RowDataBound" 
             GridLines="None" AllowSorting="true" >
            <RowStyle BackColor="White" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="40px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px"/>
            <HeaderStyle HorizontalAlign="Center" BackColor="#DEFECE" Height="43px" Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black"/>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <a href="javascript:expandcollapse('divi<%# Eval("org_id") %>', 'one');">
                            <img id="imgdivi<%# Eval("org_id") %>" alt="Click to show/hide Orders for Issue <%# Eval("org_id") %>"  width="9px" border="0" src="../img/plus.png"/>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Organization" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate>
                          <a href="org_edit.aspx?orgID=<%# Eval("org_id") %>"> <%# Eval("org_name") %> </a>
                      
                      </ItemTemplate>          
                </asp:TemplateField>
               
             
                
                 <asp:TemplateField HeaderText="Manager Name" >
                    <ItemTemplate><%# Eval("manager_name") %> 
                    </ItemTemplate>
                </asp:TemplateField>

                
                <asp:TemplateField HeaderText="Poc Name" ItemStyle-HorizontalAlign="Left" >
                    <ItemTemplate><%# Eval("poc_name")%></ItemTemplate>                 
                </asp:TemplateField>


                   <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%"  HeaderText="Action">
                    <ItemTemplate>
                       <a href="delegate_edit.aspx?orgID=<%# Eval("org_id") %>">Add/Delete Delegate</a>
                       
                    </ItemTemplate>
                   
                </asp:TemplateField>
			    
			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>  <td colspan="100%">
                       <div id="divi<%# Eval("org_id") %>" style="display:none;position:relative;left:10px;OVERFLOW: auto;WIDTH:99%; margin-bottom:20px;" >
                             
                     <div style="margin-left:30%;margin-right:30%; ">
                                 <asp:GridView ID="GridView2" AllowPaging="False"  CellSpacing="5" AllowSorting="false" BackColor="White" Width="100%" 
                                        AutoGenerateColumns="false"  runat="server" DataKeyNames="org_id" 
                                      OnRowDataBound = "GridView2_RowDataBound" 
                                     ShowFooter="false"
                                      GridLines="None">
            <RowStyle BackColor="White" Height="30px"   Font-Bold="true" Font-Names="Arial" Font-Size="12px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="30px"    Font-Bold="true" Font-Names="Arial" Font-Size="12px"/>
            <HeaderStyle BackColor="#DEFECE" Height="33px"  Font-Bold="true" Font-Names="Arial" Font-Size="14px" ForeColor="Black" />

                                        <Columns>                                     

                                            <asp:TemplateField HeaderText="SMT Organization Delegates"  ItemStyle-HorizontalAlign="Left"      ItemStyle-Width="80%" >
                                                <ItemTemplate><%# Eval("delegate_name")%></ItemTemplate>
                                            </asp:TemplateField>

                                            

                                                                 
                                             
                                        </Columns>
                                   </asp:GridView>

                          </div>
                            
       </div>
                 </td>        </tr>
                       
			        </ItemTemplate>			       
			    </asp:TemplateField>			    
			</Columns>
        </asp:GridView>
        
</td> </tr>
    </table>

    <asp:SqlDataSource ID="ds_activeOrgs" runat="server"    ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select org.org_id, org.name as org_name, manager_sid, mgr.name as manager_name ,org.poc_sid, poc.name as poc_name
from siims_org org join persons.person mgr on mgr.key=org.manager_sid
join persons.person poc on poc.key=org.poc_sid 
where org.is_active='Y'
order by org.name">
        </asp:SqlDataSource>
</asp:Content>
