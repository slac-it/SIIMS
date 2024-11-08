<%@ Page Title="RIR Statement" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="report_statement.aspx.cs" Inherits="SIIMS.rir_statement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
			textarea {
				padding: 5px;
				vertical-align: top;
                text-align:left;
				width: 900px;
			}
			textarea:focus {
				outline-style: solid;
				outline-width: 2px;
			}
			  
		</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="fieldHeader">Statement of Issue, Error Precursor, or Improvement Opportunity: </div>
    
        <asp:TextBox  ID="txtImage1" Name="txtImage1" runat="server"  TextMode="MultiLine"   >
 </asp:TextBox>  <br />  
     <asp:RequiredFieldValidator ID="RequiredTitle" ControlToValidate="txtImage1" Display="Dynamic" ValidationGroup="titleOnly" runat="server" CssClass="errorText" ErrorMessage="Error: Statement of Issue is required."></asp:RequiredFieldValidator>
   
    <div style="margin-top:10px; margin-bottom:20px; margin-left:30px;">
               <asp:Button ID="btnSubmit" runat="server" Text="&nbsp;Save&nbsp;" Font-Bold="true" ValidationGroup="titleOnly"  Font-Size="X-Large"   OnClick="btnSubmit_Click" /> &nbsp;  &nbsp; &nbsp;
       <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />
    </div>
    <div  style="height:10px; display:block;">
    <table cellspacing="2" cellpadding="4" align="left" rules="all" border="1" width="80%" style="color:#333333;font-size: large">
        <caption  class="fieldHeader">
			Change History of Statement of Issue, Error Precursor, or Improvement Opportunity </caption>
        <tr style="color:White;background-color:#5D7B9D;font-weight:bold;">
			<th scope="col">Date</th><th scope="col">Changed By</th><th scope="col">View</th></tr>
  <asp:ListView ID="lv_Changes" DataSourceID="ds_Statement" DataKeyNames="HISTORY_ID"     runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333;">
      <td style="text-align:left; vertical-align:top;">  <%# Eval("Name").ToString() %> </td>
       <td style="text-align:left; vertical-align:top;">  
            <%# Eval("Change_Date").ToString() %> </td>
        <td style="text-align:center; vertical-align:top;"> 
        <a href='report_statement.aspx?iid=<%# Eval("issue_id") %>&hid=<%# Eval("HISTORY_ID") %>'>View</a>   
    </td>
  </tr>
</ItemTemplate>
                </asp:ListView>
        <asp:Panel ID="PanelHistory" Visible="false" runat="server">
            <tr><td colspan="3" style="text-align:center; font-size:1.2em; font-weight:bold;">Statement by <asp:Label ID="lblPerson" runat="server" Text=""></asp:Label> 
                on <asp:Label ID="lblDate" runat="server" Text=""></asp:Label></td></tr>
            <tr><td colspan="3">
                <asp:Label ID="lblOldStatement" runat="server" Text=""></asp:Label>
                </td></tr>
        </asp:Panel>
    </table>
  </div>
   <div style="height:50px; display:block;">&nbsp;</div>
        
       <div style=" font-size:1.2em; margin-top:10px; display:block;">&nbsp; <br /><br />
    </div>

<asp:SqlDataSource ID="ds_Statement" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select history_id,p.name, to_char(sh.created_on,'MM/DD/YYYY') as change_date , issue_id
from SIIMS_RIR_STATEMENT_HISTORY sh
join persons.person p on p.key=sh.created_by
where issue_id=:IID
order by created_on desc">
              <SelectParameters>
                       <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="Int32" />
              </SelectParameters>
    </asp:SqlDataSource>    
		<script src='../scripts/autosize.js'></script>
     <script type="text/javascript">
         autosize(document.getElementById('<%=txtImage1.ClientID%>'));
     </script>
</asp:Content>
