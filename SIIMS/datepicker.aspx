<%@ Page Title="Date Picker Demo" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="datepicker.aspx.cs" Inherits="SIIMS.datepicker" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <%--  <script type="text/javascript" src="Scripts/jquery-3.1.1.min.js"></script>--%>
    <script type="text/javascript" src="Scripts/jquery-ui-1.12.1.min.js"></script>
<%--     <link href="Styles/jquery-ui.min.css" rel="Stylesheet" type="text/css" />--%>
    <script src="css/bootstrap-datepicker.js"></script>
    <link href="css/bootstrap-datepicker.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            $(".datepicker").datepicker({
                format: 'dd/mm/yyyy',
                autoclose: true,
                changeMonth: true,
                changeYear: true,
                todayBtn: 'linked'
            })


        });
    </script>
  

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h3>BootStrap Datetime Picker Example</h3>
    <hr />
    <div>Click textbox to open datetime picker: <input type="text" id="fromDate" class="datepicker" /></div>
    <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"></asp:DropDownList>
</asp:Content>
