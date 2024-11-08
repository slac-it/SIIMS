<%@ Page Title="RIR Reports" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="rir_reports.aspx.cs" Inherits="SIIMS.RIR.rir_reports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    li{
  margin: 15px 0;
}
   </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <div  class="pageHeader"> RIR Reports</div>
      
    <div style="text-align:left; margin-left:20px; font-weight:bold;">

   <ol >
       <li> <a href="rep_cy.aspx">Closed Reports by Calendar Year</a> </li>
 <%--      <li> <a href="reports/rep_orgAction.aspx">Closed Reports by Fisical Year </a> </li>
       <li> <a href="reports/rep_deptIssue.aspx">Closed Reports by Issue Date</a> </li>--%>

  

   </ol>
</div>
</asp:Content>
