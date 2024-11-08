<%@ Page Title="Reports" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="reports.aspx.cs" Inherits="SIIMS.areports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <style type="text/css">
    li{
  margin: 15px 0;
}
   </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <div  class="pageHeader"> Issue and Action Reports</div>
      
    <div style="text-align:left; margin-left:20px; font-weight:bold;">

   <ol >
       <li> <a href="reports/rep_orgIssue.aspx">Issue Report by Organization</a> </li>
       <li> <a href="reports/rep_orgAction.aspx">Action Report by Organization </a> </li>
       <li> <a href="reports/rep_deptIssue.aspx">Issue Report by Manager</a> </li>
       <li> <a href="reports/rep_deptAction.aspx">Action Report by Manager </a> </li>
               <li> <a href="reports/rep_ownerIssue.aspx">Issue Report by Owner </a> </li>
       <li> <a href="reports/rep_ownerAction.aspx">Action Report by Owner </a> </li>
       <li>  <a href="reports/rep_sourceIssue.aspx"> Issue Report by Source </a> </li>
               <li>  <a href="reports/rep_levelIssue.aspx"> Issue Report by Level </a> </li>

       <li><a href="rep_issueCodes.aspx">Trending Report</a></li>
         <li><a href="admin/issue_exportcode.aspx?isfull=1">Export Issue Data</a></li>
         <li><a href="admin/action_exportcode.aspx?isfull=1">Export Action Data</a></li>
        <li><a href="reports/rep_searchIssue.aspx">Issue Report by Keyword Search</a></li>
        <li><a href="reports/rep_searchAction.aspx">Action Report by Keyword Search</a></li>

   </ol>
</div>

</asp:Content>
