<%@ Page Title="Immediate Actions" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="action1.aspx.cs" Inherits="SIIMS.RIR.action1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <script type="text/javascript" src="../Scripts/jquery-ui-1.12.1.min.js"></script>
     <link href="../Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
     <script type="text/javascript">    

       
         var currentYear = new Date().getFullYear();
         var twoYears = currentYear + 2;
         var dateToday = new Date();
         $(function () {
             $("[id$=txtDueDate]").datepicker({
                 showOn: 'button',
                 buttonImageOnly: true,
                 minDate: dateToday,
                 //changeMonth: true,
                 //changeYear: true,
                 //yearRange: currentYear + ":" + twoYears ,
                 buttonImage: '../img/calendar.png'
             });
         });
       
         $(document).ready(function () {
             $('#btnUpload').on('click', function () {
                 $(this).val('Please wait ...');
                 $(this).toggleClass('greyOutButton');
                 $('#btnUpload').submit();
             });


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
                 $(id).css('top', maskHeight /2- $(id).height() );
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
          function disableButton(btnName, vgroup) {
              if (Page_ClientValidate(vgroup)) {
                  document.getElementById(btnName.id).disabled = true;
                <%-- document.getElementById('<%=btnSubmit.ClientID %>').disabled = true;--%>
                  return true;
              }

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

     .greyOutButton {
             background: #F5F5F5;
             color : #C5C5C3;
        }
        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div  class="pageHeader">
     <asp:Literal ID="litHeader"  runat="server">Immediate Corrective Action</asp:Literal>
 </div>

<div style="text-align:left; font-size:1em; margin-bottom:15px;">
<div><span style="font-weight:bold;">Report ID: </span>  <asp:Label ID="lblIssueID" runat="server" Text=""></asp:Label>  </div>
  <div>  <span style="font-weight:bold;">Title:</span> <asp:Label ID="lblITitle" runat="server" Text=""></asp:Label> <br />
   <div> <span style="font-weight:bold;">Initiator: </span> <asp:Label ID="lblInitiator" runat="server" Text=""></asp:Label> </div>
      <div> <span style="font-weight:bold;">Date Initiated: </span> <asp:Label ID="lblDateInit" runat="server" Text=""></asp:Label> </div>
  
<div> <span style="font-weight:bold;">Affected Organization: </span>  <asp:Label ID="lblOrg" runat="server" Text=""></asp:Label> </div>
<div><span style="font-weight:bold;">Department/Group: </span>  <asp:Label ID="lblDept" runat="server" Text=""></asp:Label> </div>
       <div> <span style="font-weight:bold;">Point of Contact: </span> <asp:Label ID="lblPOC" runat="server" Text=""></asp:Label>  </div>
 <div> <span style="font-weight:bold;">Date Discovered: </span> <asp:Label ID="lblDate_Disc" runat="server" Text=""></asp:Label>  </div>
<br />

    <br />
</div>
       <table width="100%" id="editTable">
              <tr>
        <td style="text-align:right; font-weight:bold;font-size:1.2em; vertical-align:top;">Description: <%--<span  class ="spanred">*</span>--%> 
                  <asp:HiddenField ID="HiddenField_ATTSESSIONID" Value="-1" runat="server" /></td>
            <td style=" text-align:left;">
             <asp:TextBox ID="txtDesc" runat="server" Height="80px" onkeypress="return this.value.length<=3800" onpaste="return this.value.length<=3800"
                        TextMode="MultiLine"  Width="95%"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtDesc" ValidationGroup="allFields" runat="server"  Display="Dynamic"  CssClass="errorText"  ErrorMessage="Error: Description is required."></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtDesc"  ValidationGroup="allFields"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
        
        </td>   </tr>
  <asp:Panel ID="PanelType2" Visible="false" runat="server">
          <tr>   <td style=" text-align:right; vertical-align:middle; font-weight:bold;font-size:1.2em;">Action Owner:  <%--<span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">                 
        
                             <asp:HiddenField ID="txtPOC_SID" runat="server" Value="-1" />           
                               <asp:Panel ID="PnlNamePOC" runat="server" Visible="false">
               <asp:TextBox ID="txtPOCName" ReadOnly="true" Visible="true"  Text="" runat="server"></asp:TextBox>&nbsp;&nbsp;
                <asp:button id="BtnChangePOC" CssClass="nounload btnfont" runat="server" Width="100px" Text="Change"  Visible="false"      TabIndex="4"  OnClick="BtnChangePOC_Click"></asp:button>  
            </asp:Panel>     
            <div id="divtrPOC" runat="server"  visible="false">   
            </div>             
            <asp:Panel ID="PnlPopupPOC" Visible="true" runat="server">   
                <div style="text-align:left; margin-bottom:0px;">        
                    <asp:label id="LblMsgLdr" runat="server" Width="100%" Font-Size="0.9em" Font-Names="Arial,Verdana,Helvetica">
                  <span class="txtmedias">   Please enter the first few characters of the action Owner's last name:</span> </asp:label><br />
                  <asp:textbox id="txtPOC"  runat="server"  ClientIDMode="Static"></asp:textbox>&nbsp;&nbsp;
                  <asp:Button ID="CmdFindPOC" CssClass="nounload btnfont" runat="server"  OnClick="CmdFindPOC_Click"   TabIndex="1" Text="Find" ClientIDMode="Static" />
          
                  <asp:Button ID="CmdCancelPOC" CssClass="nounload" runat="server" OnClick="CmdCancelldr_Click" Font-Size="Medium" Text="Cancel" Visible="false" />                   
                  <asp:Label id="LblerrorPOC" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica">Label</asp:Label> &nbsp; &nbsp; &nbsp; 
                  <asp:dropdownlist id="ddlEmplistPOC" runat="server" Visible="False" TabIndex="3"></asp:dropdownlist>&nbsp; &nbsp;
                  <asp:button id="cmdokPOC" CssClass="nounload btnfont" runat="server" Width="72px" Text="OK" Visible="False"   TabIndex="4"  OnClick="cmdokPOC_Click"></asp:button>         
                          
                </div>  
             </asp:Panel> 
         
              
          </td>   </tr>
             <tr>
     <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Due Date:<%-- <span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">
             <asp:TextBox ID="txtDueDate" runat="server" style="margin-right:10px"  Width="15%"></asp:TextBox> </div>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtDueDate" Display="Dynamic"  ValidationGroup="allFields_2" runat="server" CssClass="errorText" ErrorMessage="Error: Due Date  is required."></asp:RequiredFieldValidator>
        </td>     </tr>
      
           </asp:Panel>
           

      <tr><td style="height:15px;" colspan="2"></td></tr> <tr>
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
                    <asp:Button runat="server" id="btnUpload" ValidationGroup="requiredFile" ClientIDMode="Static" Font-Bold="true" text="Upload" onclick="btnUpload_Click" />
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
      <td><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> &nbsp; &nbsp;</td>
      <td>  <asp:ImageButton ID="ImgBtnDelete" runat="server" ImageUrl="~/img/deleteicon.gif"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="delete" OnClientClick = "return confirm('Warning! Are you sure you want to delete this attachment?');"/></td> 
  </tr>
</ItemTemplate>

    </asp:ListView>
 
        </table>
      
     </div>  
      <div style="display:inline-block; margin-top: 0px; margin-left:15px; float:left;">
                <asp:Button  ID="imgResume" runat="server" Font-Bold="true"  Text="Upload Attachment"/>
      </div>          
       </td>   </tr>
             <tr><td   style="text-align:center; height:15px;" colspan="2"></td></tr>
                    <tr><td   style="text-align:center;" colspan="2">
       
             <asp:Button ID="btnSave" runat="server" Text="Save" Font-Bold="true" Font-Size="X-Large" ValidationGroup="allFields" 
                  UseSubmitBehavior="false" OnClientClick="disableButton(this, 'allFields');" 
                 OnClick="btnSave_Click" /> 
   &nbsp;&nbsp;&nbsp;&nbsp;
     <asp:Button ID="btnDelete" runat="server" Text="Delete" Font-Bold="true" Font-Size="X-Large" OnClick="btnDelete_Click" Visible="false"
           onclientclick="return confirm(&quot; Are you sure you want to delete this action?&quot;);" /> &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />
                    <br />          <asp:Label ID="lblMsg" CssClass="errorText" Visible="false" runat="server" Text=""></asp:Label>
            </td></tr>    
                  <tr style="height:15px;"><td colspan="2"></td></tr>
    </table>
    	<script src='../scripts/autosize.js'></script>
     <script type="text/javascript">
         autosize(document.getElementById('<%=txtDesc.ClientID%>'));
</script>
</asp:Content>
