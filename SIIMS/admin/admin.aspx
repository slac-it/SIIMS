<%@ Page Title="" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="SIIMS.admin.admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">




    <br />
    Trending Code for Issues: <a href="issue_code.aspx">ASSIGN</a> &nbsp; &nbsp; <a href="issue_editcode.aspx">EDIT</a>&nbsp; &nbsp; <a href="issue_exportcode.aspx?isfull=0">EXPORT</a>
    <br /> <br />
    Action Type for Actions: <a href="action_code.aspx">ASSIGN</a> &nbsp; &nbsp; <a href="action_editcode.aspx">EDIT</a>&nbsp; &nbsp; <a href="action_exportcode.aspx?isfull=0">EXPORT</a>
    <br /> <br />
 <%--   Export Data:  <a href="issue_exportcode.aspx?isfull=1">ISSUES</a>  &nbsp; &nbsp; <a href="action_exportcode.aspx?isfull=1">ACTIONS</a>
    <br /><br />--%>
    Edit SMT Org, Manager, POC, and Delegates:  <a href="org_list.aspx">SMT Org</a> 
      <br /><br />
   
    Change  <a href="action_ChStatus.aspx">Action Status</a>;   &nbsp; &nbsp;&nbsp; &nbsp;  Change  <a href="action_ChOwners.aspx">Action Owner</a>  
    <br /><br />
    Change <a href="issue_ChStatus.aspx"> Issue Status </a>  &nbsp; &nbsp;&nbsp; &nbsp; 
</asp:Content>
