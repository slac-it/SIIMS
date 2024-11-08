<%@ Page Title="Assign/Edit Action Type" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="action_assignCode.aspx.cs" Inherits="SIIMS.admin.action_assignCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
         .drw_Margin {
            margin: 10px 5px 5px 5px;
        }

    </style>

    <script type="text/javascript">
        function showFull() {
            var divFull = document.getElementById('<%=lblIDesc.ClientID %>');
            divFull.style.display = "block";
            var divShort = document.getElementById('<%=lblIDescShort.ClientID %>');
            divShort.style.display = "none";
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <div  class="pageHeader"> Assign/Edit Action Type</div>
<div style="text-align:left; font-size:1em; margin-bottom:15px;">
<span style="font-weight:bold;">Source:</span> <asp:Label ID="lblSourceTitle" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Source Type:</span> <asp:Label ID="lblSType" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Source Date:</span> <asp:Label ID="lblSourceFY" runat="server" Text=""></asp:Label>-Q<asp:Label ID="lblSourceQtr" runat="server" Text=""></asp:Label> <br />
</div>
<div style="text-align:left; font-size:1em; margin-bottom:15px;">
<span style="font-weight:bold;">Issue ID: </span>
    <asp:Label ID="lblIssueID" runat="server" Text=""></asp:Label>

<span style="font-weight:bold; padding-left:20px;">Level: </span>
    <asp:Label ID="lblLevel" runat="server" Text=""></asp:Label>

<span style="font-weight:bold;padding-left:20px;">Organization: </span>
    <asp:Label ID="lblOrg" runat="server" Text=""></asp:Label>

<span style="font-weight:bold;padding-left:20px;">Owner: </span>
    <asp:Label ID="lblIOwner" runat="server" Text=""></asp:Label>

<br />
<span style="font-weight:bold;">Issue Title:</span> <asp:Label ID="lblITitle" runat="server" Text=""></asp:Label> <br />
<span style="font-weight:bold;">Issue Description:</span>  
<div style=" border: solid black 0px; margin-left:20px;">
     <asp:Label ID="lblIDescShort" Visible="false" runat="server" Text=""></asp:Label> 
    <asp:Label ID="lblIDesc" CssClass="" runat="server" Text=""></asp:Label> 
</div>
</div>
<table id="editTable" cellspacing="0" cellpadding="0" align="Center" width="98%" rules="all" border="1" style="color:#333333;font-size: 1.1em; ">
  <tr>
  <td style="width:70%">
    <table width="100%" class="viewTable">    
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;">Action ID: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblAID2" runat="server" Text=""></asp:Label>
           
        </td>
        </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr>
        <td style=" text-align:right; font-weight:bold;">Title: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblATitle" runat="server" Text="Label"></asp:Label>
           
        </td>
        </tr>
        <tr >
        <td style="text-align:right;vertical-align:top; font-weight:bold;">Description:</td>
        <td style=" text-align:left;">
              <asp:Label ID="lblADesc" runat="server" Text=""></asp:Label>
        
        </td>
        </tr>
   
        <tr><td style="height:15px;" colspan="2"></td></tr>
          <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;">Owner:  </td>
        <td style=" text-align:left;">
           <asp:Label ID="lblAOwner" runat="server" Text=""></asp:Label>

        </td>
        </tr>
        
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr >
        <td style=" text-align:right; font-weight:bold;">Due Date: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblDue" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 

          <tr >
        <td style=" text-align:right; font-weight:bold;">Status: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 
         
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
    <li><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton></li>
</ItemTemplate>

    </asp:ListView>
</ul>

       <br />
  <div style="font-weight:bold;">Category: </div>
     <asp:DropDownList  CssClass="drw_Margin" ID="drw_cat" DataSourceID="ds_cat" DataValueField="ACODE_ID" DataTextField="ACODE" runat="server"></asp:DropDownList>
     <br />
<div style="font-weight:bold;">Subcategory: </div>
       <asp:DropDownList CssClass="drw_Margin" ID="drw_sub"  DataSourceID="ds_sub" DataValueField="ACODE_ID" DataTextField="ACODE" runat="server"></asp:DropDownList> 
       
          <br />
      <asp:Button ID="btnSave" runat="server" Text="Save" Font-Bold="true" Font-Size="X-Large" OnClick="btnSave_Click"  />
      </td>
  </tr>
</table>

     <div style="text-align:center; margin-top:10px;">
       <asp:Button ID="btnCancel" runat="server" Text="Back" Font-Bold="true" Font-Size="Large" OnClick="btnCancel_Click"  />
  </div>

     <asp:SqlDataSource ID="ds_cat" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select 0 as ACode_ID, '- Select Category -' as ACODE from dual
union select ACODE_ID,ACODE from SIIMS_ATRENDING_CODE
where IS_CATEGORY='Y' and IS_ACTIVE='Y' 
order by 2"> 
        </asp:SqlDataSource>

 <asp:SqlDataSource ID="ds_sub" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="select 0 as ACode_ID, '- Select Subcategory -' as ACODE from dual
union select ACODE_ID,ACODE from SIIMS_ATRENDING_CODE
where IS_CATEGORY='N' and IS_ACTIVE='Y' 
order by 2"> 
        </asp:SqlDataSource>

</asp:Content>
