<%@ Page Title="Close Action" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="action_close.aspx.cs" Inherits="SIIMS.action_close" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <%--<script type="text/javascript" src="Scripts/jquery-3.1.1.min.js"></script>--%>
    <script type="text/javascript">    

        $(document).ready(function () {

            //        if ($('#lnkResume').attr('class') == null) {
            //            showUploadResume();
            //        };

            $('#<%= imgResume.ClientID %>').click(function (e) {
                //Cancel the link behavior
                e.preventDefault();
                //Get the A tag
                var id = $('#resume');

                //Get the screen height and width
                var maskHeight = $(document).height();
                var maskWidth = $(window).width();

                //Set height and width to mask to fill up the whole screen
                $('#mask').css({ 'width': maskWidth, 'height': maskHeight });

                //transition effect		
                $('#mask').fadeIn(1000);
                $('#mask').fadeTo("slow", 0.8);

                //Get the window height and width
                var winH = $(window).height();
                var winW = $(window).width();

                //Set the popup window to center
                //$(id).css('top', winH / 2 - $(id).height() / 2);
                $(id).css('top', maskHeight - $(id).height() *3);
                $(id).css('left', winW / 2 - $(id).width() / 2);

                //transition effect
                $(id).fadeIn(2000);

            });



            //if close button is clicked
            $('.window .close').click(function (e) {
                //Cancel the link behavior
                e.preventDefault();

                $('#mask').hide();
                $('.window').hide();
            });

            //if mask is clicked
            $('#mask').click(function () {
                $(this).hide();
                $('.window').hide();
            });
        });

        function showFull() {
            var divFull = document.getElementById('<%=lblIDesc.ClientID %>');
            divFull.style.display = "block";
            var divShort = document.getElementById('<%=lblIDescShort.ClientID %>');
            divShort.style.display = "none";
        }

    </script>

  <style type="text/css">
    #mask{position:absolute;left:0;top:0;
          z-index:9000;
          background-color:#000;
          opacity: 0.8;
          display:none
      ;}

     .window{position:absolute;left:0;top:0
             ;width:400px;
             height:200px;
             display:none;
             z-index:9999;
             padding:20px;
       }
   
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
          <div  class="pageHeader">Close Action</div>
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
        
       <%--  <tr><td style="height:15px;" colspan="2"></td></tr>--%>
         <tr >
        <td style=" text-align:right; font-weight:bold;">Due Date: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblDue" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 

        <%--  <tr >
        <td style=" text-align:right; font-weight:bold;">Status: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> --%>
         
</table>
</td>
 <td  style="width:25%; text-align:left;vertical-align:top; " >
     Attachments:<br />
     <asp:Label ID="lblMsg" Visible="false" runat="server" Text=""></asp:Label>
    <div style="margin-left:0em;  margin-top: 0px; text-align:left;display:inline-block; float:left;">
         <table id="viewTable"  cellspacing="2" cellpadding="4" align="left" rules="all"  style=" min-width:250px; color:#333333;font-size: large;border-collapse: separate;border-spacing:0;">
   
  <asp:ListView ID="lv_Files" runat="server" OnItemCommand="CommandList"  OnItemDataBound="DataBoundList"  OnItemDeleting="DeleteList">
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
<ItemTemplate>
  <tr style="background-color:#F7F6F3; color:#333333;">
      <td><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> &nbsp;</td>
      <td> &nbsp; <asp:ImageButton ID="ImgBtnDelete" runat="server" ImageUrl="~/img/deleteicon.gif"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="delete" OnClientClick = "return confirm('Warning! Are you sure you want to delete this attachment?');"/></td> 
  </tr>
</ItemTemplate>

    </asp:ListView>
 
        </table>
      
     </div>  
     <div id="boxes">
      <!--- Resume Upload --->
             <div id="resume" class="window" style="background-color: white;">
                <div style="padding:10px 0px 2px 5px;" class="proTitle">
                    Please Upload Your File: <span style="font-size: 0.8em; "> <br />(Note: only files with extension doc, docx, xls,xlsx, ppt,pptx, txt, jpg, or .pdf with maximum of 30MB are supported.) </span><br /><br />
                    <asp:FileUpload ID="FileUploadControl" Font-Bold="true" runat="server" />
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="FileUploadControl" 
                        ForeColor="Red" Display="Dynamic"  ValidationGroup="requiredFile"  ErrorMessage="Error: a file is required!">
                         </asp:RequiredFieldValidator>
                   <div style="margin-top:20px;">
                    <asp:Button runat="server" id="btnUpload" Font-Bold="true"  ValidationGroup="requiredFile" text="Upload" onclick="btnUpload_Click" />
                      &nbsp;  <asp:Button ID="Button2" class="close" runat="server"  OnClientClick="javascript:return confirm('Are you sure you want to Cancel?');" Text="Cancel" />
                       </div>
                 </div>
    
             </div>
        <div id="mask"></div>
    </div>
     <div style="display:inline-block; margin-top: 5px; margin-left:15px; float:left;">
                <asp:Button  ID="imgResume" runat="server" Font-Bold="true"  Text="Upload Attachment"/>
      </div>   
      </td>
  </tr>
</table>
<div style="text-align:center; margin-top:20px;">
    <asp:Label ID="lblLockout" Visible="false" CssClass="errorText" runat="server" Text=""></asp:Label><br />
      <asp:Button ID="btnClose" runat="server" Text="Close" Font-Bold="true" Font-Size="X-Large" 
           onclientclick="return confirm(&quot; Are you sure you want to close the action?&quot;);" OnClick="btnClose_Click" /> &nbsp;&nbsp;
    
   
             <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />
</div>
</asp:Content>

