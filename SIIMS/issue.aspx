<%@ Page Title="Create/Complete Issue" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="issue.aspx.cs" Inherits="SIIMS.issue" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%-- <script type="text/javascript" src="Scripts/jquery-3.1.1.min.js"></script>--%>
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
                 $(id).css('top', maskHeight - $(id).height() * 2);
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
      <div  class="pageHeader">Create / Complete Issue </div>
    <table width="100%" id="editTable">
         <tr>
        <td style="width:15%; text-align:right; font-weight:bold;font-size:1.2em;">Title: <%--<span  class ="spanred">*</span> --%></td>
        <td style="text-align:left;">
             <asp:HiddenField ID="HiddenField_ATTSESSIONID" Value="-1" runat="server" />
             <asp:TextBox ID="txtTitle" runat="server" onkeypress="return this.value.length<=100" onpaste="return this.value.length<=100"
                        TextMode="SingleLine" MaxLength="100" Width="90%"></asp:TextBox>
           <br />
           <asp:RequiredFieldValidator ID="RequiredTitle" ControlToValidate="txtTitle"  ValidationGroup="titleOnly" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Title is required."></asp:RequiredFieldValidator>
          <asp:RequiredFieldValidator ID="RequiredTitle2" ControlToValidate="txtTitle"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Title is required."></asp:RequiredFieldValidator>
     
        </td>
        </tr>
      
        <tr>
        <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Description:  </td>
        <td style="text-align:left;">
             <asp:TextBox ID="txtDesc" runat="server" Height="80px" onkeypress="return this.value.length<=3800" onpaste="return this.value.length<=3800"
                        TextMode="MultiLine"  Width="90%"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtDesc" ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Description is required."></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtDesc"  ValidationGroup="allFields"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
        
        </td>
        </tr>
       
          <tr>
        <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Organization:  </td>
        <td style=" text-align:left;">
              <asp:DropDownList ID="drwOrg" AutoPostBack="false"  DataSourceID="ds_Org"
                   DataTextField="name" DataValueField="org_id" AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">--Please Select Organization --</asp:ListItem>
              </asp:DropDownList>
            <br />
            <asp:RequiredFieldValidator InitialValue="-1" ID="Req_Org" Display="Dynamic"   ValidationGroup="allFields" runat="server" ControlToValidate="drwOrg"
                         ForeColor="Red"  ErrorMessage="Error: Organization is required!"></asp:RequiredFieldValidator>
           </td>
        </tr> 
        
        
         <tr>
        <td style=" text-align:right; vertical-align:top; font-weight:bold;font-size:1.2em;">Owner: <%-- <span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">
            <asp:TextBox ID="txtOwner_Name" ReadOnly="true" Visible="true" Text="" runat="server"></asp:TextBox>   &nbsp; &nbsp;
             <asp:button id="btnChangeOwner" runat="server" Width="100px" Text="Change" Visible="False"   Font-Bold="true" Font-Size="Large"
                            TabIndex="4" onclick="btnChangeOwner_Click"></asp:button>
             <asp:HiddenField ID="txtOwner_SID" runat="server" Value="-1" />
         <%--   <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="txtOwner_Name" ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Issue Owner is required."></asp:RequiredFieldValidator>--%>

        </td></tr>
         <asp:Panel ID="PnlPopup" runat="server">
            <tr><td></td><td>
             <div style="text-align:left;">
                 <asp:label id="lblMessage" runat="server" Width="90%" Font-Size="1.0em" Font-Names="Arial,Verdana,Helvetica">
                        Please enter the first few characters of the owner's last name: </asp:label>
                <br />
            
                  <asp:textbox id="txtOwner"  runat="server" Width="152px" onkeydown="if (event.keyCode == 13) document.getElementByID('cmdFind').click()"></asp:textbox>
		               &nbsp;  &nbsp;<asp:Button ID="cmdFind" runat="server" onclick="cmdFind_Click"  Font-Bold="true" Font-Size="Large"  TabIndex="1" Text="Find" />
          
                <asp:Label id="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red" Font-Size="10pt" Font-Names="Arial,Verdana,Helvetica">Label</asp:Label>
             &nbsp; &nbsp; &nbsp; 
                 <asp:dropdownlist id="ddlEmplist" runat="server" Visible="False" TabIndex="3"></asp:dropdownlist>
              &nbsp; &nbsp;
                 <asp:button id="cmdOk" runat="server" Width="72px" Text="OK" Visible="False"   Font-Bold="true" Font-Size="Large"
                            TabIndex="4" onclick="cmdOk_Click"></asp:button>                    
             
            </div>  
               </td> </tr>
           
           </asp:Panel>
           
       
      
          <tr>
        <td style="text-align:right; font-weight:bold;font-size:1.2em;">Level:  </td>
        <td style=" text-align:left;">
              <asp:DropDownList ID="drwLevel" runat="server">
                   <asp:ListItem Value="-1">-- Please Select Level --</asp:ListItem>
                  <asp:ListItem Value="P1">P1</asp:ListItem>
                  <asp:ListItem Value="P2">P2</asp:ListItem>
                  <asp:ListItem Value="P3">P3</asp:ListItem>
                  <asp:ListItem Value="P4">P4</asp:ListItem>
              </asp:DropDownList>
            <br />
              <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator3" Display="Dynamic"   ValidationGroup="allFields" runat="server" ControlToValidate="drwLevel"
                         ForeColor="Red"  ErrorMessage="Error: Level is required!"></asp:RequiredFieldValidator>
           </td>
          
        </tr> 
        <tr><td style="height:15px;" colspan="2"></td></tr>  
    <asp:Panel ID="PanelSourceType"  runat="server">
       <tr>
        <td style="text-align:right; font-weight:bold; vertical-align:top; font-size:1.2em;">Source: <%--<span  class ="spanred">*</span>--%> </td>
        <td style=" text-align:left;">
              <asp:DropDownList ID="drwSType" AutoPostBack="true" 
                   DataSourceID="ds_SType" DataTextField="type" DataValueField="STYPE_ID" runat="server" AppendDataBoundItems="true">
                       <asp:ListItem Selected="True" Value="-1">--Please Select Source --</asp:ListItem>
              </asp:DropDownList>
         
            <br />
            <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" Display="Dynamic"   ValidationGroup="allFields" runat="server" ControlToValidate="drwSType"
                         ForeColor="Red"  ErrorMessage="Error: Source is required!"></asp:RequiredFieldValidator>
      </td></tr> 
     </asp:Panel>
    
       <!--  Assessment  source input --->
      <asp:Panel ID="PanelAssessment" Visible="false" runat="server">
            <tr> <td ></td> <td>
             <div style="text-align:left;margin-top:10px;">
                <div style="font-weight:bold; margin-bottom:10px;">Please select an Assessment:</div>
                      <asp:dropdownlist id="drwAssessmentTitle" runat="server" AutoPostBack="false"  
                      DataSourceID="ds_Assessment" DataTextField="Title" DataValueField="ASSESSMENT_ID"  TabIndex="3">
                      </asp:dropdownlist>  
                      <asp:Button ID="btnAssessmentSelection" runat="server"  ValidationGroup="assSource"  Font-Bold="true" Font-Size="Large"  
                          TabIndex="1" Text=" OK " OnClick="btnAssessmentSelection_Click" /><br />

                 <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator12" Display="Dynamic"   ValidationGroup="assSource" runat="server" 
                     ControlToValidate="drwAssessmentTitle"
                         ForeColor="Red"  ErrorMessage="Error: Assessment Source Title is required!"></asp:RequiredFieldValidator>

                   <div style="font-weight:bold; margin-bottom:5px; margin-top:15px;" >Filter By:</div>             
                 <asp:dropdownlist id="drwAssessmentFY" runat="server" AutoPostBack="true"  DataSourceID="ds_AssessmentFY" 
                     DataTextField="FY" DataValueField="FY_DUE" TabIndex="3"></asp:dropdownlist>   
                  <br />  
                  <asp:dropdownlist id="drwAssessmentQtr" runat="server" AutoPostBack="true"  DataSourceID="ds_AssessmentQtr" 
                     DataTextField="QTR" DataValueField="QID" TabIndex="4"></asp:dropdownlist>      &nbsp;  &nbsp;
              
                 </div>  
          </td></tr>
       </asp:Panel>
    
        <!--  incident source input --->
      <asp:Panel ID="PanelIncident" Visible="false" runat="server">
            <tr> <td ></td> <td>
             <div style="text-align:left;margin-top:20px;">
                 <div style="font-weight:bold; margin-bottom:10px;">Please select an Incident:</div>
                 <asp:dropdownlist id="drwIncidentTitle" runat="server" AutoPostBack="false"  
                      DataSourceID="ds_Incident" DataTextField="Title1" DataValueField="inc_id" TabIndex="3">
                 </asp:dropdownlist>  
                  <asp:Button ID="btnIncidentSelection" runat="server"  ValidationGroup="incidentSource"  Font-Bold="true" Font-Size="Large" 
                       TabIndex="1" Text=" OK " OnClick="btnIncidentSelection_Click" />        <br />
                 <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator10" Display="Dynamic"   ValidationGroup="incidentSource" runat="server" 
                     ControlToValidate="drwIncidentTitle"
                         ForeColor="Red"  ErrorMessage="Error: Incident Source Title is required!"></asp:RequiredFieldValidator>
                 <div style="font-weight:bold; margin-bottom:5px; margin-top:15px;" >Filter By:</div>      
                 <asp:dropdownlist id="drwIncidentFY" runat="server" AutoPostBack="true"  DataSourceID="ds_IncidentFY" 
                     DataTextField="FY" DataValueField="Year" TabIndex="3"></asp:dropdownlist>   
                  <br />
                  <asp:dropdownlist id="drwIncidentQtr" runat="server" AutoPostBack="true"  DataSourceID="ds_IncidentQtr" 
                     DataTextField="QTR" DataValueField="QID" TabIndex="4"></asp:dropdownlist>       <br />
                  <asp:dropdownlist id="drwIncidentSMT" runat="server" AutoPostBack="true"  DataSourceID="ds_IncidentSMT" 
                     DataTextField="smt_org" DataValueField="org" TabIndex="5"></asp:dropdownlist>        <br />
              
                </div>  
          </td>   </tr>    
       </asp:Panel>

        <!--  Other source input --->
      <asp:Panel ID="PanelOtherInput" Visible="false" runat="server">
            <tr>  <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Source Title:  </td>
        <td style=" text-align:left;">
                           <asp:textbox id="txtSourceOtherTitle" MaxLength="150" 
                       onkeypress="return this.value.length<=150" onpaste="return this.value.length<=150"  runat="server" Width="300px"></asp:textbox> 
                 <asp:Button ID="btnOtherInput" runat="server" onclick="btnOtherInput_Click" ValidationGroup="otherSource"  
                            Font-Bold="true" Font-Size="Large"  TabIndex="1" Text=" OK " /> <br />
		            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtSourceOtherTitle"  ValidationGroup="otherSource" runat="server" ForeColor="Red" Display="Dynamic" ErrorMessage="Error: Source Title is required."></asp:RequiredFieldValidator>
            </td> </tr>

         <tr>  <td style=" text-align:right; font-weight:bold;font-size:1.2em;">FY:  </td>
               <td style=" text-align:left;">  
                  <asp:dropdownlist id="drw_OtherFY" runat="server"   TabIndex="3" >
                         <asp:ListItem Value="-1">-- Please Select FY --</asp:ListItem>
                    </asp:dropdownlist>
                      &nbsp;  &nbsp; <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator5" Display="Dynamic"   ValidationGroup="otherSource" runat="server" ControlToValidate="drw_OtherFY"
                         ForeColor="Red"  ErrorMessage="Error: Source FY is required!"></asp:RequiredFieldValidator>
       </td> </tr>
         <tr>  <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Quarter:  </td>
               <td style=" text-align:left;">   <asp:dropdownlist id="drw_OtherQtr" runat="server"   TabIndex="3" >
                         <asp:ListItem Value="-1">-- Please Select Quarter --</asp:ListItem>
                          <asp:ListItem Value="1">Oct. - Dec.</asp:ListItem>
                          <asp:ListItem Value="2">Jan. - Mar.</asp:ListItem>
                          <asp:ListItem Value="3">Apr. - Jun.</asp:ListItem>
                          <asp:ListItem Value="4">Jul. - Sep. </asp:ListItem>
                    </asp:dropdownlist>   &nbsp;  &nbsp;
                      <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator6" Display="Dynamic"   ValidationGroup="otherSource" 
                          runat="server" ControlToValidate="drw_OtherQtr"
                         ForeColor="Red"  ErrorMessage="Error: Source Quarter is required!"></asp:RequiredFieldValidator>
        </td>   </tr>    
     </asp:Panel>
      
     <asp:Panel ID="PanelOtherSelection" Visible="false" runat="server">    
          <tr> <td ></td> <td>
                  <div style="text-align:left;margin-top:20px;">
                 <div style="font-weight:bold; margin-bottom:10px;">Please select an existing Source, or create a new Source:</div>
                     
                 <asp:dropdownlist id="drw_OtherSelection" DataSourceID="ds_OtherTitle" DataTextField="title1" DataValueField="source_id" runat="server" 
                   Visible="true" AutoPostBack="false" TabIndex="3">
                 </asp:dropdownlist>
                    <asp:Button ID="btnOtherSelection" runat="server" onclick="btnOtherSelection_Click" ValidationGroup="otherDropSelection"  Font-Bold="true" Font-Size="Large" 
                          TabIndex="1" Text=" OK " />&nbsp;&nbsp; &nbsp;&nbsp;
                      <asp:Button ID="btnCreateOther" runat="server"  Font-Bold="true" Font-Size="Large"  
                               onclick="btnCreateOther_Click" Text="Create New Source" />  <br />
                
                       <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator11" Display="Dynamic"   ValidationGroup="otherDropSelection" runat="server" 
                           ControlToValidate="drw_OtherSelection"   ForeColor="Red"  ErrorMessage="Error: Source Title is required!"></asp:RequiredFieldValidator>
                  <div style="font-weight:bold; margin-bottom:5px; margin-top:15px;" >Filter By:</div> 
                 <asp:dropdownlist id="drwOtherYear" runat="server" AutoPostBack="true"  DataSourceID="ds_OtherYear" 
                     DataTextField="Year" DataValueField="YID" TabIndex="3"></asp:dropdownlist>   
                         <br />    
                 <asp:dropdownlist id="drwOtherQuarter" runat="server" AutoPostBack="true"  DataSourceID="ds_OtherQuarter" 
                     DataTextField="QTR" DataValueField="QID" TabIndex="3"></asp:dropdownlist>   
       
            </div>  
           </td>   </tr>    
     </asp:Panel>

   <asp:Panel ID="PanelSourceTitle" Visible="false" runat="server">
           
          <tr>
         <td style=" text-align:right; font-weight:bold;font-size:1.2em;">Source Title: <%--<span  class ="spanred">*</span>--%> </td>
               <td style=" text-align:left;">
                   <asp:TextBox ID="txtSourceTitle" ReadOnly="true"  Width="80%" MaxLength="150" runat="server"></asp:TextBox> 
                    <asp:Button ID="btnTitleChange" runat="server" Text="Change"  Font-Bold="true" Font-Size="Large"  OnClick="btnTitleChange_Click" />
                   <br />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ControlToValidate="txtSourceTitle"  ValidationGroup="allFields" runat="server" ForeColor="Red" Display="Dynamic" 
                         ErrorMessage="Error: Source Title is required."></asp:RequiredFieldValidator>
                  
         </td> </tr>
               
            <asp:HiddenField ID="HiddenField_ALINK" Value="-1" runat="server" />
             <asp:HiddenField ID="HiddenField_FY" Value="-1" runat="server" />
              <asp:HiddenField ID="HiddenField_Qtr" Value="-1" runat="server" />
    </asp:Panel>

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
                    <asp:Button runat="server" id="btnUpload" ValidationGroup="requiredFile" Font-Bold="true" text="Upload" onclick="btnUpload_Click" />
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
      <td><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ISATT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> </td>
      <td>  <asp:ImageButton ID="ImgBtnDelete" runat="server" ImageUrl="~/img/deleteicon.gif"  CommandArgument='<%# Eval("ISATT_ID") %>' CommandName="delete" OnClientClick = "return confirm('Warning! Are you sure you want to delete this attachment?');"/></td> 
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
        
       
        <tr><td colspan="2"  style="text-align:center;">
             <asp:Label ID="lblMsg" CssClass="errorText" Visible="false" runat="server" Text=""></asp:Label> <br />  
             <asp:Button ID="btnSave" runat="server" Text="Save as Draft" Font-Bold="true" Font-Size="X-Large" ValidationGroup="titleOnly" OnClick="btnSave_Click" /> &nbsp;&nbsp;
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" Font-Bold="true" Font-Size="X-Large" ValidationGroup="allFields" OnClick="btnSubmit_Click" /> &nbsp;&nbsp;
     <asp:Button ID="btnDelete" runat="server" Text="Delete" Font-Bold="true" Font-Size="X-Large" OnClick="btnDelete_Click" 
           onclientclick="return confirm(&quot; Are you sure you want to delete this issue?&quot;);" /> &nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click" 
          onclientclick="return confirm(&quot;Please make sure you save the form before leaving. Are you sure you want to cancel now?&quot;);" />

            </td></tr>    
    </table>                                
                
         <asp:SqlDataSource ID="ds_Org" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select org_id, name from SIIMS_ORG where is_active='Y' 
             union select org.org_id , org.name from SIIMS_ORG org
join siims_issue issue on org.org_id=issue.org_id and issue.issue_id=:IID
             order by name">
              <SelectParameters>
               <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="String" />
              </SelectParameters>
    </asp:SqlDataSource>    
    
      <asp:SqlDataSource ID="ds_SType" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select STYPE_ID,type from siims_source_type where is_active='Y' and Type <> 'RIR'
          union select type.STYPE_ID , type.TYPE from siims_source_type type
join siims_issue issue on type.stype_id=issue.stype_id and issue.issue_id=:IID
          order by TYPE ">
           <SelectParameters>
               <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="String" />
              </SelectParameters>
    </asp:SqlDataSource>   
    
        <asp:SqlDataSource ID="ds_Incident" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select -1 as inc_id ,'9999' as FY, '5' as Quarter, ' ' as Incident_Date, '-- Please Select Incident  --' as Title1 from dual 
union select inc_id, FY,Quarter, Incident_Date, Incident_Date || ' ' ||  title as title1 from SIIMS_Incident where is_active='Y'
             and (:FYear='-1' or FY=:FYear) and (:Qtr='-1' or Quarter=:Qtr) and (:ORG='-1' or SMT_ORG=:ORG)
               order by FY desc, Quarter desc, Incident_Date desc "   >
             <SelectParameters>
                 <asp:ControlParameter ControlID="drwIncidentFY" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwIncidentFY" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwIncidentQtr" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwIncidentQtr" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwIncidentSMT" Name="ORG" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwIncidentSMT" Name="ORG" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
              </SelectParameters>

    </asp:SqlDataSource>   
      <asp:SqlDataSource ID="ds_IncidentFY" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as Year ,'-- Filter by FY  --' as FY from dual union select distinct FY as Year, FY from SIIMS_Incident where is_active='Y'  order by FY">
    </asp:SqlDataSource>  
      <asp:SqlDataSource ID="ds_IncidentQtr" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as qid ,'-- Filter by Quarter  --' as qtr from dual union select distinct s.quarter as qid,s.quarter as qtr from siims_incident s
where is_active='Y'">
    </asp:SqlDataSource>   
    <asp:SqlDataSource ID="ds_IncidentSMT" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as org ,'-- Filter by SMT Org  --' as smt_org from dual union 
               select distinct SMT_ORG as org, SMT_ORG as smt_org from siims_incident where is_active='Y'">
    </asp:SqlDataSource>  

      <asp:SqlDataSource ID="ds_OtherTitle" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select -1 as source_id ,'9999' as fy, '5' as quarter, ' ' as title , '-- Please Select Existing  --' as Title1 from dual union  
select min(s.source_id) as source_id, s.fy, s.Quarter,
s.title,  REGEXP_REPLACE (s.fy,  '20','FY', 1, 1, 'i') || '-Q' || s.Quarter || ' ' ||  s.title as title1
from siims_issue iss 
          join siims_source s on iss.issue_id=s.issue_id  and  (:FYear='-1' or s.fy = :FYear) and (:Qtr='-1' or s.Quarter=:Qtr)
            where iss.stype_id='O' 
            group by  s.fy,s.Quarter, s.title 
  order by fy desc, quarter desc, title">
           <SelectParameters>
                <asp:ControlParameter ControlID="drwOtherYear" Direction="Input" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" /> 
               <asp:ControlParameter ControlID="drwOtherYear" Direction="Input" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" /> 
               <asp:ControlParameter ControlID="drwOtherQuarter" Direction="Input" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />   
               <asp:ControlParameter ControlID="drwOtherQuarter" Direction="Input" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />   
           </SelectParameters>
    </asp:SqlDataSource>   
    
  

      <asp:SqlDataSource ID="ds_OtherYear" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as yid ,'-- Filter by Year  --' as year from dual union select distinct s.fy as yid,s.fy as year from siims_issue iss join siims_source s on iss.issue_id=s.issue_id 
where iss.stype_id='O' 
order by year">
    </asp:SqlDataSource>  

      <asp:SqlDataSource ID="ds_OtherQuarter" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as qid ,'-- Filter by Quarter  --' as qtr from dual union select distinct s.quarter as qid,s.quarter as qtr from siims_issue iss join siims_source s on iss.issue_id=s.issue_id 
where iss.stype_id='O' 
order by qtr">
    </asp:SqlDataSource>  

    
    <asp:SqlDataSource ID="ds_Assessment" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select -1 as ASSESSMENT_ID ,'FY99' as FY_DUE,5 as QTR_DUE , '-- Please Select Assessment --' as Title from dual union 
select ASSESS.ASSESSMENT_ID,ASSESS.FY_DUE,  ASSESS.QTR_DUE, ASSESS.FY_DUE || '-Q' || ASSESS.QTR_DUE || ' ' || ASSESS.TITLE 
FROM IAS_ASSESSMENT ASSESS 
INNER JOIN IAS_ASSESSMENT_STATUS_HISTORY HIS ON ASSESS.ASSESSMENT_ID = HIS.ASSESSMENT_ID
AND ASSESS.IS_ACTIVE='Y' AND HIS.IS_ACTIVE='Y' AND HIS.STATUS_ID=2
         and (:FYear ='-1' or ASSESS.FY_DUE=:FYear) and (:Qtr='-1' or ASSESS.QTR_DUE=:Qtr)
union   select A.ASSESSMENT_ID,A.FY_DUE,  A.QTR_DUE, A.FY_DUE || '-Q' || A.QTR_DUE || ' ' || A.TITLE as Title  FROM IASR_ASSESSMENT A INNER JOIN IASR_STATUS_HISTORY HIS ON A.ASSESSMENT_ID = HIS.ASSESSMENT_ID
        AND A.IS_ACTIVE='Y' AND HIS.IS_ACTIVE='Y' AND HIS.PROCESS_STATUS_ID=24  and (:FYear ='-1' or A.FY_DUE=:FYear) and (:Qtr='-1' or A.QTR_DUE=:Qtr)
order by 2 DESC,3 DESC, 4"   >
             <SelectParameters>
                 <asp:ControlParameter ControlID="drwAssessmentFY" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwAssessmentFY" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwAssessmentQtr" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwAssessmentQtr" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
              </SelectParameters>
    </asp:SqlDataSource>   

     <asp:SqlDataSource ID="ds_AssessmentFY" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="  select '-- Filter by FY  --' as FY,'-1' as FY_DUE
from dual union select distinct  REPLACE(ASSESS.FY_DUE, 'FY', '20') as FY, ASSESS.FY_DUE
FROM IAS_ASSESSMENT ASSESS 
INNER JOIN IAS_ASSESSMENT_STATUS_HISTORY HIS ON ASSESS.ASSESSMENT_ID = HIS.ASSESSMENT_ID
AND ASSESS.IS_ACTIVE='Y' AND HIS.IS_ACTIVE='Y' AND HIS.STATUS_ID=2 
union select distinct  REPLACE(A.FY_DUE, 'FY', '20') as FY, A.FY_DUE
FROM IASR_ASSESSMENT A
INNER JOIN IASR_STATUS_HISTORY H ON A.ASSESSMENT_ID = H.ASSESSMENT_ID
AND A.IS_ACTIVE='Y' AND H.IS_ACTIVE='Y' AND H.PROCESS_STATUS_ID=24 
order by FY_DUE ">
    </asp:SqlDataSource>  

      <asp:SqlDataSource ID="ds_AssessmentQtr" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand=" select -1 as qid ,'-- Filter by Quarter  --' as qtr from dual union select distinct QTR_DUE as qid, QTR_DUE || ' ' as qtr from IAS_ASSESSMENT
where is_active='Y' ">
    </asp:SqlDataSource>  
   
</asp:Content>
