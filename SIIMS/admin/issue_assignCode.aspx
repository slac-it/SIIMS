<%@ Page Title="Assign Codes to Issues" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="issue_assignCode.aspx.cs" Inherits="SIIMS.admin.issue_assignCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
         .drw_Margin {
            margin: 10px 5px 5px 5px;
        }

    </style>
    <script  type="text/javascript">
  

    function expandcollapse2(obj, row) {
        var div = document.getElementById(obj);
        var img = document.getElementById('act' + obj);

        if (div.style.display == "none") {
            div.style.display = "block";
            img.src = "../img/minus.png";         
        }
        else {
            div.style.display = "none";
            img.src = "../img/plus.png";
            img.alt = "Expand to show Orders";
        }
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div  class="pageHeader"> Assign / Edit Trending Codes for Issue</div>
<div style="text-align:left; font-size:1em; margin-bottom:15px;">
<span style="font-weight:bold;">Source:</span> <asp:Label ID="lblSourceTitle" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Source Type:</span> <asp:Label ID="lblSType" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Source Date:</span> <asp:Label ID="lblSourceFY" runat="server" Text=""></asp:Label>-Q<asp:Label ID="lblSourceQtr" runat="server" Text=""></asp:Label> <br />

</div>

 <table id="editTable" cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: 1.1em; ">
  <tr>
  <td style="width:70%">
    <table width="100%" class="viewTable">    
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;">Issue ID: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblIssueID" runat="server" Text=""></asp:Label>
           
        </td>
        </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr>
        <td style=" text-align:right; font-weight:bold;">Title: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblTitle" runat="server" Text="Label"></asp:Label>
           
        </td>
        </tr>
        <tr >
        <td style="text-align:right;vertical-align:top; font-weight:bold;">Description:</td>
        <td style=" text-align:left;">
              <asp:Label ID="lblDesc" runat="server" Text=""></asp:Label>
        
        </td>
        </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr >
        <td style="text-align:right; font-weight:bold;">Organization:  </td>
        <td style="text-align:left;">
              <asp:Label ID="lblOrg" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        
          <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;">Owner:  </td>
        <td style=" text-align:left;">
           <asp:Label ID="lblOwner" runat="server" Text=""></asp:Label>

        </td>
        </tr>
        
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr >
        <td style=" text-align:right; font-weight:bold;">Level: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblLevel" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 

          <tr >
        <td style=" text-align:right; font-weight:bold;">Status: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 
         

         
        <%--    <tr >
       <td colspan="2"  style="text-align:center;">
             <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />
            </td></tr>    --%>
</table>
</td>
 <td  style="width:25%; text-align:left;vertical-align:top; " >
     Attachments:<br />
     <asp:Label ID="lblMsg" Visible="false" runat="server" Text=""></asp:Label>
  <ul>
  <asp:ListView ID="lv_Files" runat="server" OnItemCommand="CommandList"  OnItemDataBound="DataBoundList">
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
<ItemTemplate>
    <li><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ISATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton></li>
</ItemTemplate>

    </asp:ListView>
</ul>
     <br />
  <div style="font-weight:bold;">Work Type: </div>
     <asp:DropDownList  CssClass="drw_Margin" ID="drw_WorkType" DataSourceID="ds_WorkType" DataValueField="ICODE_ID" DataTextField="WorkType" runat="server"></asp:DropDownList>
     <br />
<div style="font-weight:bold;">Functional Area: </div>
       <asp:DropDownList CssClass="drw_Margin" ID="drw_Functional1"  DataSourceID="ds_FunctionalArea" DataValueField="ICODE_ID" DataTextField="Functional" runat="server"></asp:DropDownList> <br />
          <asp:DropDownList CssClass="drw_Margin" ID="drw_Functional2"  DataSourceID="ds_FunctionalArea" DataValueField="ICODE_ID" DataTextField="Functional" runat="server"></asp:DropDownList> <br />
          <asp:DropDownList CssClass="drw_Margin" ID="drw_Functional3" DataSourceID="ds_FunctionalArea" DataValueField="ICODE_ID" DataTextField="Functional" runat="server"></asp:DropDownList>
      <asp:Label ID="lblError" Visible="false" runat="server" ForeColor="Red" Text="Duplicate Functional Area not allowed - please reassign."></asp:Label>
          <br />
      <asp:Button ID="btnSave" runat="server" Text="Save" Font-Bold="true" Font-Size="X-Large" OnClick="btnSave_Click"  />
      </td>
  </tr>
</table>

  <table class="viewTable"  cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: large; margin-top:35px; ">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:left;  padding:4px; font-family:Arial; font-weight:bold;color:#494949; " >
      Actions 
     </th>
  </tr>
<tr > <td style="padding:0px; margin:0;">
 <asp:GridView ID="GVActions" AllowPaging="True" BackColor="#f1f1f1"  width="100%" CellSpacing="5" PageSize="10"
            AutoGenerateColumns="false" DataSourceID="ds_ACTIONS" DataKeyNames="ACTION_ID" EmptyDataText="No Actions"
           Font-Size="Large"  runat="server"  OnRowDataBound="GVActions_RowDataBound" 
             GridLines="None" AllowSorting="true" >
            <RowStyle BackColor="White" Height="20px"  Font-Bold="true" Font-Names="Arial" Font-Size="12px"   />
            <AlternatingRowStyle BackColor="#f7f5f5" Height="20px"  Font-Bold="true" Font-Names="Arial" Font-Size="12px"/>
            <HeaderStyle BackColor="#DEFECE" Height="30px" Font-Bold="true" Font-Names="Arial" Font-Size="12px" ForeColor="Black"/>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%">
                    <ItemTemplate>
                        <a href="javascript:expandcollapse2('diva<%# Eval("ACTION_ID") %>', 'one');">
                            <img id="actdiva<%# Eval("ACTION_ID") %>" alt="Click to show/hide Orders for Actions <%# Eval("ACTION_ID") %>"  width="9px" border="0" src="../img/plus.png"/>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action ID" SortExpression="ACTION_ID">
                    <ItemTemplate>
                        <asp:Label ID="lblActionID" Text='<%# Eval("action_id2") %>' runat="server"></asp:Label>
                    </ItemTemplate>               
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Action Title" ItemStyle-HorizontalAlign="Left" SortExpression="ATitle">
                    <ItemTemplate>
                         <a href='../action_view.aspx?aid=<%# Eval("ACTION_ID") %>'><%# Eval("ATitle") %></a>   
                      </ItemTemplate>          
                </asp:TemplateField>

           <asp:TemplateField HeaderText="Status/Due"  ItemStyle-HorizontalAlign="Left"
                ItemStyle-Width="15%">
            <ItemTemplate><%# FORMATACTIONStatus(Eval("status").ToString(), Eval("due_date").ToString()) %></ItemTemplate>                                                        
        </asp:TemplateField>
                                        
                                          

            <asp:TemplateField HeaderText="Owner" SortExpression="owner" ItemStyle-HorizontalAlign="Left"  ItemStyle-Width="15%">
            <ItemTemplate><%# Eval("owner")%></ItemTemplate>         
        </asp:TemplateField>

			    <asp:TemplateField>
			        <ItemTemplate>
			            <tr>
                            <td colspan="100%">                             
                              <div id="diva<%# Eval("action_id") %>" style="display:none;position:relative;left:10px;OVERFLOW: auto;WIDTH:99%; margin-bottom:10px;" >
                                     <div style="margin-left:30px; margin-bottom:5px;">                                      
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

  <div style="text-align:center; margin-top:10px;">
       <asp:Button ID="btnCancel" runat="server" Text="Back" Font-Bold="true" Font-Size="Larger"  OnClientClick="JavaScript: window.history.back(1); return false;"  />
  </div>

    
   <asp:SqlDataSource ID="ds_ACTIONS" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand=" SELECT action.issue_id,action_id, 'A' || action.action_id  as action_id2 , action.title as ATitle,  action.description as desc2,
       to_char( action.due_date, 'MM/DD/YYYY') due_date, sta.status_id, sta.status, p.name as owner 
 from siims_action action join siims_status sta on action.status_id=sta.status_id and object_id=2 left join persons.person p 
on action.owner_sid=p.key where  action.is_active='Y' and action.issue_id=:issueID order by action.created_on desc"  >
            <SelectParameters>
   <asp:QueryStringParameter QueryStringField="iid" Name="issueID" DbType="Int32" />
</SelectParameters>
    </asp:SqlDataSource>   

    <asp:SqlDataSource ID="ds_WorkType" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select 0 as ICode_ID, '- Select Work Type -' as WORKTYPE from dual
union select ICODE_ID, WORKTYPE from SIIMS_ITRENDING_CODE
where IS_WORKTYPE='Y' and IS_ACTIVE='Y' order by 2"> 
        </asp:SqlDataSource>

 <asp:SqlDataSource ID="ds_FunctionalArea" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select 0 as ICode_ID, '- Select Functional Area -' as Functional from dual
union
select ICODE_ID, Category || '-' || Sub from SIIMS_ITRENDING_CODE
where IS_WORKTYPE='N' and IS_ACTIVE='Y'
order by 2"> 
        </asp:SqlDataSource>

</asp:Content>
