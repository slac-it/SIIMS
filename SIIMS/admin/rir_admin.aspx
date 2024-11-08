<%@ Page Title="RIR Admin" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_admin.aspx.cs" Inherits="SIIMS.admin.rir_admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    

    <br />
  
     <div style="font-size:1.1em; font-weight:bold;padding-bottom:10px;">
        <a href="rir_condition.aspx">Edit Initial Radiological Posting(s) or Conditions List</a> 
    </div>
      <div style="font-size:1.1em; font-weight:bold;padding-bottom:10px;">
        <a href="rir_code.aspx">Edit Tracking/Trending Codes</a> 
    </div>
      <div style="font-size:1.1em; font-weight:bold;padding-bottom:10px;">
        <a href="rir_group.aspx">Edit Added Groups</a> 
    </div>
       <div style="font-size:1.1em; font-weight:bold;padding-bottom:10px;">
        <a href="rir_dist.aspx">Edit Shared Notification Email List</a> 
    </div>
      <div style="font-size:1.1em; font-weight:bold; padding-bottom:10px;">
        <a href="rir_reviewer.aspx">Edit Reviewers/RIR Coordinator</a> 
    </div>
       <div style="font-size:1.1em; font-weight:bold; padding-bottom:10px;">
        <a href="rir_number.aspx">My Open and Closed Report</a> 
    </div>
</asp:Content>
