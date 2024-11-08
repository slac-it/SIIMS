<%@ Page Title="Edit Action" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="action_edit.aspx.cs" Inherits="SIIMS.action_edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <%-- <script type="text/javascript" src="Scripts/jquery-3.1.1.min.js"></script>--%>
         <script type="text/javascript" src="Scripts/jquery-ui-1.12.1.min.js"></script>
     <link href="Styles/jquery-ui.min.css" rel="Stylesheet" type="text/css" />
     <script type="text/javascript" src="scripts/backbutton.js"></script>
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
                 var winH = $(window).height() ;
                 var winW = $(window).width();

                 //Set the popup window to center
                 //$(id).css('top', winH / 2 - $(id).height() / 2);
                 $(id).css('top', maskHeight - $(id).height() *4);
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

        </script>


      <script type="text/javascript">
          var currentYear = new Date().getFullYear();
          var twoYears = currentYear + 2;

          $(function () {
              $("[id$=txtDueDate]").datepicker({
                  showOn: 'button',
                  buttonImageOnly: true,
                  changeMonth: true,
                  changeYear: true,
                  yearRange: currentYear + ":" + twoYears,
                  buttonImage: 'img/calendar.png'
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

 <div  class="pageHeader"> Edit Action</div>
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

    <table width="100%" id="editTable">
        <tr>
        <td style="width:15%; text-align:right; font-weight:bold;">Action ID: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblAID2" runat="server" Text=""></asp:Label>
           
        </td>
        </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;font-size:1.2em;">Title:<%-- <span  class ="spanred">*</span>--%>
               <asp:HiddenField ID="HiddenField_ATTSESSIONID" Value="-1" runat="server" />
        </td>
        <td style=" text-align:left;">
             <asp:TextBox ID="txtTitle" runat="server" onkeypress="return this.value.length<=100" onpaste="return this.value.length<=100"
                        TextMode="SingleLine" MaxLength="100" Width="90%"></asp:TextBox>
           <br />
           <asp:RequiredFieldValidator ID="RequiredTitle" ControlToValidate="txtTitle"  ValidationGroup="titleOnly" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Title is required."></asp:RequiredFieldValidator>
          <asp:RequiredFieldValidator ID="RequiredTitle2" ControlToValidate="txtTitle"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Title is required."></asp:RequiredFieldValidator>
     
        </td>
        </tr>
       
        <tr>
        <td style="text-align:right; font-weight:bold;font-size:1.2em; vertical-align:top;">Description: <%--<span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">
             <asp:TextBox ID="txtDesc" runat="server" Height="80px" onkeypress="return this.value.length<=3800" onpaste="return this.value.length<=3800"
                        TextMode="MultiLine"  Width="90%"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtDesc" ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Description is required."></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtDesc"  ValidationGroup="allFields"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
        
        </td>
        </tr>
         <tr><td style="height:15px;" colspan="2"></td></tr>
            
        
          <tr><td style="height:15px;" colspan="2"></td></tr>
          <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;">Owner:  </td>
        <td style=" text-align:left;">
           <asp:Label ID="lblAOwner" runat="server" Text=""></asp:Label>

        </td>
        </tr>
       
         <tr >
        <td style=" text-align:right; font-weight:bold;">Due Date: </td>
        <td style=" text-align:left;">
            <asp:Label ID="lblDue" runat="server" Text=""></asp:Label>
           </td>
          
        </tr> 

      <tr><td style="height:15px;" colspan="2"></td></tr>
        <td style=" text-align:right; vertical-align:top;font-weight:bold;font-size:1.2em;">Attachments: </td>
        <td style=" text-align:left;">
 <div id="boxes">
      <!--- Resume Upload --->
             <div id="resume" class="window" style="background-color: white;">
                <div style="padding:10px 0px 2px 5px;" class="proTitle">
                    Please Upload Your File: <span style="font-size: 0.8em; "> <br />(Note: only files with extension doc, docx, xls,xlsx, ppt,pptx, txt, jpg, or .pdf with maximum of 30MB are supported.) </span><br /><br />
                    <asp:FileUpload ID="FileUploadControl" Font-Bold="true" runat="server" />
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="FileUploadControl" 
                        ForeColor="Red" Display="Dynamic"  ValidationGroup="requiredFile"  ErrorMessage="Error: a file is required!">
                         </asp:RequiredFieldValidator>
                    <br /> <br />
                    <asp:Button runat="server" id="btnUpload" Font-Bold="true"  ValidationGroup="requiredFile"  text="Upload" onclick="btnUpload_Click" />
                      &nbsp;  <asp:Button ID="Button2" class="close" runat="server"  OnClientClick="javascript:return confirm('Are you sure you want to Cancel?');" Text="Cancel" />
                 </div>
    
             </div>
        <div id="mask"></div>
    </div>
    <div style="margin-left:0em;  margin-top: 0px; text-align:left;display:inline-block; float:left;">
         <table id="viewTable"  cellspacing="2" cellpadding="4" align="left" rules="all"  style=" min-width:250px; color:#333333;font-size: large;border-collapse: separate;border-spacing:0;">
   
  <asp:ListView ID="lv_File" runat="server" OnItemCommand="CommandList"  OnItemDataBound="DataBoundList"  OnItemDeleting="DeleteList">
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
<ItemTemplate>
  <tr style="background-color:#F7F6F3; color:#333333;">
      <td><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> </td>
      <td> &nbsp; <asp:ImageButton ID="ImgBtnDelete" runat="server" ImageUrl="~/img/deleteicon.gif"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="delete" OnClientClick = "return confirm('Warning! Are you sure you want to delete this attachment?');"/> &nbsp; </td> 
  </tr>
</ItemTemplate>

    </asp:ListView>
 
        </table>
      
     </div>  
      <div style="display:inline-block; margin-top: 0px; margin-left:15px; float:left;">
                <asp:Button  ID="imgResume" runat="server" Font-Bold="true"  Text="Upload Attachment"/>
      </div>          
        </td>
        </tr>
        
        <tr><td colspan="2" style="text-align:center;">
             <asp:Label ID="lblMsg" CssClass="errorText" Visible="false" runat="server" Text=""></asp:Label> <br />   
            
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" Font-Bold="true" Font-Size="X-Large" ValidationGroup="allFields" OnClick="btnSubmit_Click" /> &nbsp;&nbsp;
    
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />

            </td></tr> 
                    
 </table>
     


<%--    <asp:Panel ID="PanelOwnerNote" Visible="false" runat="server">
     <div style="margin-left:0em;  margin-top: 20px; text-align:center;">
 <table width="100%">
         <tr>
            <td colspan="2" style="text-align:center;"><span style="font-weight:bold;">Action Owner's Progress Note: </span> 
            </td>
        </tr>
        
       
        <tr style="margin-top:10px;">
        <td style="width:75%; text-align:left;" colspan="2">
             <asp:TextBox ID="txtOwnerNote" runat="server" Height="80px" onkeypress="return this.value.length<=3800" onpaste="return this.value.length<=3800"
                        TextMode="MultiLine"  Width="90%"></asp:TextBox><br />
        
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic"
                                        ControlToValidate="txtDesc"  ValidationGroup="CheckNote"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
        
        </td>
        </tr>
       <tr style="margin-top:20px; margin-bottom:20px;">
        <td colspan="2">
    <asp:Button ID="btnNote" runat="server" Text="Update" Font-Bold="true"  ValidationGroup="CheckNote" Font-Size="X-Large" OnClick="btnNote_Click"  /> 

            </td></tr>  
 

</table>
     </div>
     </asp:Panel>--%>
                         


</asp:Content>
