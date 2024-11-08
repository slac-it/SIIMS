<%@ Page Title="Add a new RIR Issue" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="SIIMS.RIR_Create" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <%--        <script type="text/javascript" src="scripts/backbutton.js"></script>--%>
      <script type="text/javascript" src="../Scripts/jquery-ui-1.12.1.min.js"></script>
     <link href="../Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
     <script type="text/javascript">       
         var currentYear = new Date().getFullYear();
         var twoYears = currentYear + 2;
         var dateToday = new Date();
         $(function () {
             $("[id$=txtDateDisc]").datepicker({
                 showOn: 'button',
                 buttonImageOnly: true,
                 maxDate: dateToday,
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
      <div  class="pageHeader">Create / Edit RIR Report </div>
    <table width="100%" id="editTable">
         <tr>
        <td style="text-align:left;">
            <div><span  class="fieldHeader">Title of Report:</span>
             <asp:HiddenField ID="HiddenField_ATTSESSIONID" Value="-1" runat="server" />
             <asp:TextBox ID="txtTitle" runat="server" onkeypress="return this.value.length<=100" onpaste="return this.value.length<=100"
                        TextMode="SingleLine" MaxLength="100" Width="80%"> </asp:TextBox>
           </div>
           <asp:RequiredFieldValidator ID="RequiredTitle" ControlToValidate="txtTitle" Display="Dynamic" ValidationGroup="titleOnly" runat="server" CssClass="errorText" ErrorMessage="Error: Title is required."></asp:RequiredFieldValidator>
          <asp:RequiredFieldValidator ID="RequiredTitle2" ControlToValidate="txtTitle" Display="Dynamic" ValidationGroup="allFields" runat="server" CssClass="errorText" ErrorMessage="Error: Title is required."></asp:RequiredFieldValidator>
     
        </td>
        </tr>



       <tr>        <td style=" text-align:left;"><div><span  class="fieldHeader">Affected Organization:</span>
              <asp:DropDownList ID="drwOrg"  DataSourceID="ds_Org" AutoPostBack="true" OnSelectedIndexChanged="drwOrg_Changed"
                   DataTextField="name" DataValueField="org_id" AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select  --</asp:ListItem>
              </asp:DropDownList>
            </div>
            <asp:RequiredFieldValidator InitialValue="-1" ID="Req_Org" ValidationGroup="allFields" runat="server" Display="Dynamic" ControlToValidate="drwOrg"  CssClass="errorText" 
                          ErrorMessage="Error: Organization is required!"></asp:RequiredFieldValidator>
     </td>   </tr>
        <tr>   <td style=" text-align:left;">    <div><span  class="fieldHeader">Department/Group:</span>
              <asp:DropDownList ID="drwGroup"    runat="server">
                  
              </asp:DropDownList>
            </div>
          <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator3" ValidationGroup="allFields" runat="server" 
               Display  ="Dynamic" ControlToValidate="drwGroup"  CssClass="errorText" 
                          ErrorMessage="Error: Department/Group is required!"></asp:RequiredFieldValidator>
     </td>   </tr>
           
           <tr>        <td style=" text-align:left;">
               <table style="margin-left:-5px;">
                   <tr>
                       <td> <div class="fieldHeader">Point of Contact: </div></td>
                       <td>
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
                  <span class="txtmedias">   Please enter the first few characters of the POC's last name:</span> </asp:label><br />
                  <asp:textbox id="txtPOC"  runat="server"  ClientIDMode="Static"></asp:textbox>&nbsp;&nbsp;
                  <asp:Button ID="CmdFindPOC" CssClass="nounload btnfont" runat="server"  OnClick="CmdFindPOC_Click"   TabIndex="1" Text="Find" ClientIDMode="Static" />
          <%--        <asp:RequiredFieldValidator ID="RFVTldr"  ControlToValidate="txtPOC" ValidationGroup="allFields" runat="server"  CssClass="errorText"  Display="Dynamic" ErrorMessage="Error:Point of Contact is required!" ></asp:RequiredFieldValidator>--%>
                  <asp:Button ID="CmdCancelPOC" CssClass="nounload" runat="server" OnClick="CmdCancelldr_Click" Font-Size="Medium" Text="Cancel" Visible="false" />                   
                  <asp:Label id="LblerrorPOC" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica">Label</asp:Label> &nbsp; &nbsp; &nbsp; 
                  <asp:dropdownlist id="ddlEmplistPOC" runat="server" Visible="False" TabIndex="3"></asp:dropdownlist>&nbsp; &nbsp;
                  <asp:button id="cmdokPOC" CssClass="nounload btnfont" runat="server" Width="72px" Text="OK" Visible="False"   TabIndex="4"  OnClick="cmdokPOC_Click"></asp:button>         
                          
                </div>  
             </asp:Panel> 
                    <asp:CustomValidator runat="server" ClientValidationFunction="validateHidden" ValidationGroup="allFields" CssClass="errorText"  Display="Dynamic" ErrorMessage="Error:Point of Contact is required!"  />
                       </td>
                   </tr>
               </table>

          </td>   </tr>
             <tr>
        <td style="text-align:left;">    <div><span  class="fieldHeader">Date Discovered:</span>
             <asp:TextBox ID="txtDateDisc" runat="server" style="margin-right:10px"   Width="15%"></asp:TextBox> </div>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtDateDisc" Display="Dynamic"  ValidationGroup="allFields" runat="server" CssClass="errorText" ErrorMessage="Error: Date Discovered is required."></asp:RequiredFieldValidator>
        </td>     </tr>
        <tr>        <td style=" text-align:left;">  <div><span  class="fieldHeader">Location of Problem: </span>
              <asp:TextBox ID="txtLocation" MaxLength="100"   Width="50%" runat="server"></asp:TextBox>        </div>
               <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtLocation"  ValidationGroup="allFields" runat="server" Display="Dynamic" CssClass="errorText" ErrorMessage="Error: Location of Problem is required."></asp:RequiredFieldValidator>
           </td>
         
        </tr> 
             <tr>        <td style=" text-align:left;">  <div><span  class="fieldHeader">Initial Radiological Posting(s) or Conditions (as appropriate): </span>
               <asp:DropDownList ID="drwCondition" AutoPostBack="false" DataSourceID="ds_condition"
                   DataTextField="condition" DataValueField="condition_id"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList>    </div>
              <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" ValidationGroup="allFields" runat="server" Display="Dynamic" ControlToValidate="drwCondition"  CssClass="errorText" 
                          ErrorMessage="Error: Initial Radiological Posting(s) or Conditions is required!"></asp:RequiredFieldValidator>
          </td>   </tr>
       <asp:Panel ID="PanelLblStatement" Visible="false" runat="server">
               <tr>        <td style=" text-align:left;">
              <div class="fieldHeader">Statement of Issue, Error Precursor, or Improvement Opportunity:  &nbsp;&nbsp; 
                 <%-- <asp:HyperLink ID="lnkStatement" runat="server">Edit</asp:HyperLink>--%>
                   <asp:LinkButton ID="lnkStatement"  OnClientClick="return confirm(&quot; Please save your data prior to performing this action. Are you sure you want to continue?&quot;);"  runat="server">Edit</asp:LinkButton> 
 </div>
                   <div style="margin-left:20px; border-style:solid; border-width:1px; padding:2px;">
                        <asp:Label ID="lblStatement" runat="server"  Text=""></asp:Label>
                   </div>
                  
        </td>   </tr>
       </asp:Panel>
        <asp:Panel ID="PanelTxtStatement" Visible="false" runat="server">
           <tr>        <td style=" text-align:left;">
              <div class="fieldHeader">Statement of Issue, Error Precursor, or Improvement Opportunity:  
 </div>
             <asp:TextBox ID="txtDesc" runat="server" Height="80px"       TextMode="MultiLine"  Width="95%"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtDesc" ValidationGroup="allFields" runat="server"  Display="Dynamic"  CssClass="errorText"  ErrorMessage="Error: Statement of Issue is required."></asp:RequiredFieldValidator> 
        </td>   </tr>
            </asp:Panel>
               <tr><td style="height:10px;" ></td></tr>
     <tr> <td class="fieldHeader">Attachments for RIR Report: </td>  </tr>
        <tr>  <td style=" text-align:left;">
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
      <td><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ISATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> &nbsp; &nbsp;</td>
      <td>  <asp:ImageButton ID="ImgBtnDelete" runat="server" ImageUrl="~/img/deleteicon.gif"  CommandArgument='<%# Eval("ISATT_ID") %>' CommandName="delete" OnClientClick = "return confirm('Warning! Are you sure you want to delete this attachment?');"/></td> 
  </tr>
</ItemTemplate>

    </asp:ListView>
 
        </table>
      
     </div>  
      <div style="display:inline-block; margin-top: 0px; margin-left:15px; float:left;">
                <asp:Button  ID="imgResume" runat="server" Font-Bold="true"  Text="Upload Attachment"/>
      </div>          
       </td>   </tr>

                  
                  <tr style="height:15px;"><td></td></tr>
    </table>

        <asp:Panel ID="PanelActions" Visible="false" runat="server">

    <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="100%" border="1" style="color:#333333;font-size: large">
         
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center; " colspan="4"> <span style="font-size:1.2em;">Immediate Corrective Actions</span>  &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
     <asp:LinkButton ID="LinkAction1" OnClientClick="return confirm(&quot; Please save data or you will lose your changes. Are you sure you want to leave?&quot;);"  runat="server">Add New</asp:LinkButton>
   </th>
  </tr>

  <tr>
      <%--  <th style="width:10%;">Action ID</th>--%>
      <th style="width:5%;">No. </th>
   <th  style="width:70%;">Description</th>
      <th style="width:5%;">To Do</th>
        <th  style="width:20%;">Attachment</th>
  </tr>
         <asp:ListView ID="lv_Actions1" DataKeyNames="action_id"  OnItemDataBound="action1_DataBoundList"    runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333;">
    <td style="text-align:center; vertical-align:top;"> 
       <%# Eval("Imme_No").ToString() %>
    </td>
       <td style="text-align:left; vertical-align:top;">  
            <asp:label runat="server" ID="lblInclude" Text='<%# Eval("DESCRIPTION").ToString().Replace("\r", "<br />") %>'> </asp:label> </td>
      <td style="text-align:center; vertical-align:top;"> 
               <asp:LinkButton ID="test1" PostBackUrl='<%#"action1.aspx?type=1&from=e&aid="+Eval("action_id") %>' OnClientClick="return confirm(&quot; Please save your data prior to performing this action. Are you sure you want to continue?&quot;);"  runat="server">Edit</asp:LinkButton> 
    </td>

      <td style="text-align:left; vertical-align:top;">                 
     
    <asp:Repeater ID="Repeater1" OnItemCommand="action1_CommandList" runat="server">
        <ItemTemplate>
            <div style="margin-top:0px; margin-left:5px;">
                <asp:LinkButton ID="LnkDownAction1"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%#  Eval("FILE_NAME").ToString() %>'></asp:LinkButton>
               </div>
        </ItemTemplate>
    </asp:Repeater>
      </td>
  </tr>
</ItemTemplate>
                </asp:ListView>
  </table>
<div style="height:10px"></div>

 <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="100%" border="1" style="color:#333333;font-size: large">
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center; " colspan="6"> <span style="font-size:1.2em;">Recommended Actions</span>  &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
         <asp:LinkButton ID="LinkAction2" OnClientClick="return confirm(&quot; Please save data or you will lose your changes. Are you sure you want to leave?&quot;);"  runat="server">Add New</asp:LinkButton>
   </th>
  </tr>
  
  <tr >
  <%--    <th style="width:10%;">Action ID</th>--%>
      <th style="width:5%;">No.</th>
   <th  style="width:50%;">Description</th>
      <th  style="width:10%;">Due Date</th>
      <th  style="width:10%;">Owner</th>
       <th style="width:5%;">To Do</th>
        <th  style="width:20%;">
            Attachment</th>
  </tr>
          <asp:ListView ID="lv_Actions2" DataKeyNames="action_id"  OnItemDataBound="action2_DataBoundList"    runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333;">
    <td style="text-align:center; vertical-align:top;"> 
       <%# Eval("Imme_No").ToString() %>
    </td>
       <td style="text-align:left; vertical-align:top;">  
            <asp:label runat="server" ID="lblInclude" Text='<%# Eval("DESCRIPTION").ToString().Replace("\r", "<br />") %>'> </asp:label> </td>
      <td style="text-align:left; vertical-align:top;"><%# Eval("due_date") %>  </td>
       <td style="text-align:left; vertical-align:top;"><%# Eval("owner") %>  </td>
       <td style="text-align:center; vertical-align:top;"> 
             <asp:LinkButton ID="test2" PostBackUrl='<%#"action1.aspx?type=2&from=e&aid="+Eval("action_id") %>' OnClientClick="return confirm(&quot; Please save your data prior to performing this action. Are you sure you want to continue?&quot;);"  runat="server">Edit</asp:LinkButton>
    </td>
      <td style="text-align:left; vertical-align:top;">                 
     
    <asp:Repeater ID="Repeater2" OnItemCommand="action1_CommandList" runat="server">
        <ItemTemplate>
            <div style="margin-top:0px; margin-left:5px;">
                <asp:LinkButton ID="LnkDownAction2"  runat="server"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%#  Eval("FILE_NAME").ToString() %>'></asp:LinkButton>
               </div>
        </ItemTemplate>
    </asp:Repeater>
      </td>
  </tr>
</ItemTemplate>
                </asp:ListView>

        </table>


   
           </asp:Panel>
       
 <div style="text-align:center; margin-top:15px;">
       <asp:Button ID="btnSave" runat="server" Text="Create Actions" Font-Bold="true" Font-Size="X-Large" ValidationGroup="titleOnly" 
           UseSubmitBehavior="false" OnClientClick="disableButton(this, 'titleOnly');" 
           OnClick="btnSave_Click" /> &nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="btnKeep" runat="server" Text="Save and Keep Working" Font-Bold="true" Font-Size="X-Large" ValidationGroup="titleOnly"
                    UseSubmitBehavior="false" OnClientClick="disableButton(this, 'titleOnly');" 
                 OnClick="btnKeep_Click" /> &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnSubmit" runat="server" Text="&nbsp;Submit&nbsp;" Font-Bold="true" Font-Size="X-Large" ValidationGroup="allFields"
        UseSubmitBehavior="false" OnClientClick="disableButton(this, 'allFields');" 
        OnClick="btnSubmit_Click" /> &nbsp;&nbsp;&nbsp;&nbsp;
     <asp:Button ID="btnDelete" runat="server" Text="Delete" Font-Bold="true" Font-Size="X-Large" OnClick="btnDelete_Click" 
           onclientclick="return confirm(&quot; Are you sure you want to delete this report?&quot;);" /> &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />
                    <br />          <asp:Label ID="lblMsg" CssClass="errorText" Visible="false" runat="server" Text=""></asp:Label>
            </div>                   
                
         <asp:SqlDataSource ID="ds_Org" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select org_id, name from SIIMS_ORG where is_active='Y' 
             and org_id in (select distinct  org_id from VW_siims_rir_dept union select org_id from SIIMS_ISSUE where issue_id=:IID)
             order by name">
               <SelectParameters>
                       <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="Int32" />
              </SelectParameters>
    </asp:SqlDataSource>    



           <asp:SqlDataSource ID="ds_condition" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select condition_id, condition from SIIMS_RIR_Condition where is_active='Y' 
            union select cond.condition_id, cond.condition from SIIMS_RIR_Condition cond
 join siims_rir_report rep on rep.condition_id=cond.condition_id 
and rep.issue_id=:IID
order by 2">
                <SelectParameters>
                       <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="Int32" />
              </SelectParameters>
    </asp:SqlDataSource>    


    	<script src='../scripts/autosize.js' type="text/javascript"></script>
     <script type="text/javascript">
         autosize(document.getElementById('<%=txtDesc.ClientID%>'));

             function validateHidden(source, args) {
                 args.IsValid = document.getElementById("<%= txtPOC_SID.ClientID %>").value !="-1";
            }

</script>
</asp:Content>
