<%@ Page Title="RIR Admin Report" Language="C#" MasterPageFile="~/admin/RIRAdmin.Master" AutoEventWireup="true" CodeBehind="rir_number.aspx.cs" Inherits="SIIMS.admin.rir_number" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script type="text/javascript" src="../Scripts/jquery-ui-1.12.1.min.js"></script>
     <link href="../Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
     <script type="text/javascript">       
         var todayDate = new Date();
         var currentMonth = todayDate.getMonth();
         var startDate = new Date();
         startDate.setDate(1);  
         startDate.setMonth(currentMonth - 3);
         var endDate = new Date();
         endDate.setDate(0);
         endDate.setMonth(currentMonth - 1);
         $(function () {
             $("[id$=txtStartDate]").datepicker({
                 showOn: 'button',
                 buttonImageOnly: true,
                 maxDate: todayDate,
                 defaultDate: startDate,
                 //changeMonth: true,
                 //changeYear: true,
                 //yearRange: currentYear + ":" + twoYears ,
                 buttonImage: '../img/calendar.png'
             });
             $("[id$=txtEndDate]").datepicker({
                 showOn: 'button',
                 buttonImageOnly: true,
                 maxDate: todayDate,
                 defaultDate: todayDate,
                 buttonImage: '../img/calendar.png'
             });
         });
     </script>
     <style type="text/css">
        .paddingCell {
            padding:5px;
        }

        .link-btn {
    color: black;
    background: white;
    padding: 5px 10px;
    font-weight:bold;
    font-size: larger;
    border: 2px #555555 solid;
    height:50px;
}

          .link-btn2 {
    color: black;
    background: white;
    padding: 0px 10px;
    font-weight:bold;
    font-size: larger;
    border: 2px #555555 solid;
    height:32px;
}

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div  class="pageHeader">Open and Closed Actions Report</div>

<%--    <asp:Panel ID="PanelSelection" runat="server">--%>
         <div style="font-weight:bold; margin-bottom:10px;"> Please select action start date and end date:  </div> 
        <div style="font-weight:bold;"> Start Date:  
 <asp:TextBox ID="txtStartDate" runat="server" style="margin-right:10px"   Width="15%"></asp:TextBox> </div>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtStartDate" Display="Dynamic"  ValidationGroup="g1" runat="server" 
                 CssClass="errorText" ErrorMessage="Error: Start Date is required."></asp:RequiredFieldValidator>


         <div style="margin-top:20px;">
          <div style="font-weight:bold; margin-top:20px;">  End Date: 
               <asp:TextBox ID="txtEndDate" runat="server" style="margin-right:10px"   Width="15%"></asp:TextBox> </div>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtEndDate" Display="Dynamic"  ValidationGroup="g1" runat="server" 
                 CssClass="errorText" ErrorMessage="Error: End Date is required."></asp:RequiredFieldValidator>
          </div> &nbsp;
          
               <br />
               <asp:Label ID="lblErrMsg" Visible="false" ForeColor="Red" Font-Bold="true" Font-Size="Large" runat="server" Text=""></asp:Label>
           

<div style="margin:10px 0 30px 100px;">

   <asp:Button ID="btnGo"  CssClass="link-btn2" runat="server" Text="Submit"  ValidationGroup="g1" OnClick="btnGo_Click" />
    &nbsp; &nbsp; &nbsp; &nbsp;
        <%-- <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="link-btn2" onclick="btnClear_Click" />--%>
      <asp:Button ID="btnCancel" runat="server" Text="Cancel"  CssClass="link-btn2" OnClick="btnCancel_Click"  />
</div>
 
  <%--      </asp:Panel>--%>
  

          <asp:Panel ID="PanelReport" Visible="false" runat="server">

    <div style="font-size:1.2em;">

        <div style="margin:10px 0 15px 0;">
   Number of actions <span style="font-weight:bold;font-size:1.1em;">Opened</span> between the start and end dates  (with you as issue owner): <asp:Label ID="lblOpenIssues" Font-Bold="true" Font-Size="Large" runat="server" Text="">10</asp:Label>
    &nbsp; &nbsp; <asp:Button ID="btnType1" runat="server" Font-Bold="true" Font-Size="Medium" PostBackUrl="~/admin/rir_numberlisting.aspx?type=1" Text="View Detail" />
</div>
                 <div style="margin:10px 0 15px 0;">         
                Number of actions <span style="font-weight:bold;font-size:1.1em;">Closed </span>between the start and end dates (with you as issue owner):  <asp:Label ID="lblClosedIssues"  Font-Size="Large" Font-Bold="true" runat="server">5</asp:Label> 
                       &nbsp; &nbsp; <asp:Button ID="btnType2" runat="server" Font-Bold="true" Font-Size="Medium" PostBackUrl="~/admin/rir_numberlisting.aspx?type=2" Text="View Detail" />
              </div>

  
      </div>
              </asp:Panel>


</asp:Content>
