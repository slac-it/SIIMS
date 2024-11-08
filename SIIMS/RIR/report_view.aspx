<%@ Page Title="RIR Report" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="report_view.aspx.cs" Inherits="SIIMS.RIR.report_view" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <script type="text/javascript" src="../Scripts/jquery-ui-1.12.1.min.js"></script>
    <style type="text/css">
           .displayData {
                  text-align: left;
                    font-weight: normal;
                    font-size: 1.0em;
                    margin-top: 5px;
                    margin-bottom: 0px;
            }
    </style>
      
    
      <script type="text/javascript">  
          function disableButton(btnName, vgroup) {
              if (Page_ClientValidate(vgroup)) {
                  document.getElementById(btnName.id).disabled = true;
                <%-- document.getElementById('<%=btnSubmit.ClientID %>').disabled = true;--%>
                  return true;
              }

          }
      </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <div  class="pageHeader">View RIR Report </div>
    <table width="100%" id="editTable">
         <tr>
        <td style="text-align:left;">
            <div> <span  class="fieldHeader">Title of Report:</span>
                   <asp:Label ID="lblTitle" runat="server" CssClass="displayData" Text="">             </asp:Label>
            </div> 
        </td> </tr>
     <%--   </tr>
               <tr><td style="text-align:left;">
             <div class="fieldHeader">Official RIR #:      </div>
           <asp:Label ID="lblSource" runat="server" ></asp:Label>  
     
 
        </td>
        </tr>--%>
        <tr>
        <td style="text-align:left;">
              <div> <span  class="fieldHeader">Report Initiator:</span>
                  <asp:Label ID="lblInitiator" runat="server" CssClass="displayData"></asp:Label>  
                  </div>
        </td>     </tr>

      
           <tr>
        <td style="text-align:left;">
               <div> <span  class="fieldHeader">Date Initiated:</span>
                  <asp:Label ID="lblDateCreated" runat="server" CssClass="displayData"></asp:Label>  
                </div>
        </td>     </tr>
     <tr>
        <td style=" text-align:left;">
             <div> <span  class="fieldHeader">Affected Organization:</span>
                   <asp:Label ID="lblOrg" runat="server" CssClass="displayData"> </asp:Label>  
     </div>
           </td>
        </tr> 
   <tr> <td style=" text-align:left;">
            <div> <span  class="fieldHeader">Department/Group:  </span>
            <asp:Label ID="lblDept" runat="server" CssClass="displayData"></asp:Label>           </div>   
           </td>
        </tr> 
           <tr>
        <td style=" text-align:left;">
            <div> <span  class="fieldHeader">Point of Contact:   </span>
            <asp:Label ID="lblPOC" runat="server" CssClass="displayData"> </asp:Label>   </div>
        </td></tr>
                <tr>
        <td style="text-align:left;">
             <div> <span  class="fieldHeader">Date Discovered:    </span>
                  <asp:Label ID="lblDateDisc" runat="server" CssClass="displayData"></asp:Label>   </div>
     
        </td>     </tr>
         <tr>
        <td style=" text-align:left;">
              <div> <span  class="fieldHeader">Location of Problem:    </span>
                      <asp:Label ID="lblLocation" runat="server" CssClass="displayData"> </asp:Label>   </div>
           </td>   </tr> 
             <tr>
        <td style=" text-align:left;">
            <div> <span  class="fieldHeader">Initial Radiological Posting(s) or Conditions (as appropriate):    </span>
                  <asp:Label ID="lblCondition" runat="server" CssClass="displayData"> </asp:Label>   </div>
           </td>             
        </tr> 
          
         <asp:Panel ID="PnlLevel" Visible="false" runat="server">
              <tr>
        <td style=" text-align:left;">
            <div> <span  class="fieldHeader">Significance Level:    </span>
                  <asp:Label ID="lblLevel" runat="server" CssClass="displayData"> </asp:Label>   </div>
           </td>             
        </tr> <tr>
        <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Tracking/Trending Codes:    </span>
                  <asp:Label ID="lblCode" runat="server" CssClass="displayData"></asp:Label>   </div>
           </td>             
        </tr>  
      </asp:Panel>
        <asp:Panel ID="PnlIssueDate" Visible="false" runat="server">
             <tr>
          <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Issued Date:    </span>
                  <asp:Label ID="lblIssueDate" runat="server" CssClass="displayData"></asp:Label>   </div>
           </td>             
        </tr> 
        </asp:Panel>
        <tr>
        <td style="text-align:left;"> 

              <div class="fieldHeader">Statement of Issue, Error Precursor, or Improvement Opportunity:       </div>
              <div style="margin-left:20px; border-style:solid; border-width:1px; padding:2px;">
                        <asp:Label ID="lblStatement" runat="server" CssClass="displayData" Text=""></asp:Label>
                   </div>
           
        </td>
        </tr>
           
      <tr><td style="height:15px;"  ></td></tr>
            
     <tr>    <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Attachments for RIR Report:   </span></div>
      
       
         <asp:Label ID="lblMsg" CssClass="errorText" Visible="false" runat="server" Text=""></asp:Label>
  <div style="margin-left:0em;  margin-top: 0px; text-align:left;display:inline-block; float:left;">
         <table id="viewTable"  cellspacing="2" cellpadding="4" align="left" rules="all"  style=" min-width:250px; color:#333333;border-collapse: separate;border-spacing:0;">
   
  <asp:ListView ID="lv_File" runat="server"  OnItemCommand="CommandList" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
<ItemTemplate>
  <tr style="background-color:#F7F6F3; color:#333333;">
      <td class="displayData"><asp:LinkButton ID="LnkDownload"  runat="server"  CommandArgument='<%# Eval("ISATT_ID") %>' CommandName="download"  CssClass="displayData" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton> &nbsp; &nbsp;
       
      </td>
  </tr>
</ItemTemplate>

    </asp:ListView>
 
        </table>
      
     </div>  
         
       </td>   </tr>

                  
                  <tr style="height:15px;"><td></td></tr>
     </table>

  <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="100%" border="1" style="color:#333333;">
         
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center; font-size:1.2em;" colspan="3">Immediate Corrective Actions</th>

  </tr>
  <tr style="background-color:#E5E5FE">
      <%--  <th style="width:10%;">Action ID</th>--%>
      <th style="width:5%;"  class="fieldHeader">No. </th>
   <th  style="width:75%;"  class="fieldHeader">Description</th>
        <th   class="fieldHeader">Attachment</th>
  </tr>
     <asp:ListView ID="lv_Actions1" DataKeyNames="action_id"  OnItemDataBound="action1_DataBoundList"    runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333;">
    <td style="text-align:center; vertical-align:top;"> <%# Eval("Imme_No").ToString() %>
        <%--<a href='action1.aspx?type=1&from=v&aid=<%# Eval("action_id") %>'><%# Eval("Imme_No").ToString() %></a>   --%>
    </td>
       <td style="text-align:left; vertical-align:top;">  
            <asp:label runat="server" ID="lblInclude" CssClass="displayData" Text='<%# Eval("DESCRIPTION").ToString().Replace("\r", "<br />") %>'> </asp:label> </td>
      
      <td style="text-align:left; vertical-align:top;">                 
     
    <asp:Repeater ID="Repeater1" OnItemCommand="action1_CommandList" runat="server">
        <ItemTemplate>
            <div style="margin-top:0px; margin-left:5px;">
                <asp:LinkButton ID="LnkDownAction1"  runat="server" CssClass="displayData"  Font-Size="Small"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%#  Eval("FILE_NAME").ToString() %>'></asp:LinkButton>
               </div>
        </ItemTemplate>
    </asp:Repeater>
      </td>
  </tr>
</ItemTemplate>
                </asp:ListView>

        </table>

<div style="height:10px"></div>

    <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="100%" border="1" style="color:#333333;">
         
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center; font-size:1.2em;" colspan="5">Recommended  Actions</th>
  </tr>
          <tr >
      <th style="width:5%;"  class="fieldHeader">No.</th>
   <th  style="width:55%;"  class="fieldHeader">Description</th>
      <th  style="width:10%;"  class="fieldHeader">Due Date</th>
      <th  style="width:10%;"  class="fieldHeader">Owner</th>
        <th   class="fieldHeader">
            Attachment</th>
  </tr>
     <asp:ListView ID="lv_Actions2" DataKeyNames="action_id"  OnItemDataBound="action2_DataBoundList"    runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333;">
    <td style="text-align:center; vertical-align:top;">
        <%# FormatActionEdit(Eval("action_id").ToString(), Eval("Imme_No").ToString()) %>
    </td>
       <td style="text-align:left; vertical-align:top;">  
            <asp:label runat="server" ID="lblInclude" CssClass="displayData"  Text='<%# Eval("DESCRIPTION").ToString().Replace("\r", "<br />") %>'> </asp:label> </td>
      <td style="text-align:left; vertical-align:top;" class="displayData"><%# Eval("due_date") %>  </td>
       <td style="text-align:left; vertical-align:top;"  class="displayData"><%# Eval("owner") %>  </td>
      <td style="text-align:left; vertical-align:top;">                 
     
    <asp:Repeater ID="Repeater2" OnItemCommand="action1_CommandList" runat="server">
        <ItemTemplate>
            <div style="margin-top:0px; margin-left:5px; font-size:0.9em;">
                <asp:LinkButton ID="LnkDownAction2"  runat="server" Font-Size="Small" CssClass="displayData"  CommandArgument='<%# Eval("ACATT_ID") %>' CommandName="download" Text ='<%#  Eval("FILE_NAME").ToString() %>'></asp:LinkButton>
               </div>
        </ItemTemplate>
    </asp:Repeater>
      </td>
  </tr>
</ItemTemplate>
                </asp:ListView>
        </table>


<div style="height:10px"></div>
    <asp:Panel ID="Panelpprover" Visible="false"  runat="server">
            <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="100%" border="1" style="color:#333333;">
         
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center; font-size:1.2em;" colspan="6">Review/Approval Status</th>

  </tr>
  <tr style="background-color:#E5E5FE">
      <th style="width:15%;" class="fieldHeader">Reviewer Type </th>
   <th  style="width:20%;"  class="fieldHeader">Reviewer Name</th>
        <th  style="width:10%;"  class="fieldHeader">Date Requested</th>
         <th  style="width:10%;"  class="fieldHeader">Date Responded</th>
       <th  style="width:35%;"  class="fieldHeader">Comment</th>
       <th   class="fieldHeader">Approval Status</th>
  </tr>
  
   <asp:ListView ID="ListView1" DataKeyNames="REVIEWER_SID"  DataSourceID="ds_approver"   runat="server" >
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
               
            </LayoutTemplate>
             <ItemTemplate>
  <tr style=" color:#333333; font-size:0.9em;">
    <td style="text-align:center; vertical-align:top;"> 
        <%# Eval("title") %> 
    </td>
       <td style="text-align:left; vertical-align:top;">     <%# Eval("name") %> 
             </td>
      <td style="text-align:left; vertical-align:top;"><%# Eval("Date_Sent") %>  </td>
       <td style="text-align:left; vertical-align:top;"><%# Eval("Date_Response") %>  </td>
      <td style="text-align:left; vertical-align:top;">                 
      <%# Eval("note").ToString().Replace("\r","<br />") %> 
      </td>
         <td style="text-align:left; vertical-align:top;">                 
      <%# Eval("IS_ACCEPTED").ToString() %> 
      </td>
  </tr>
</ItemTemplate>
                </asp:ListView>

            </table>

    <div style="height:10px"></div>
 </asp:Panel>
    <asp:Panel ID="PanelDist" Visible="false" runat="server">
            <table  cellspacing="2" cellpadding="4" align="Center" rules="all" width="100%" border="1" style="color:#333333;">
         
   <tr style="background-color:#E5E5FE">
   <th style="text-align:center; font-size:1.2em;" >Recipients List:  &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <asp:HyperLink ID="linkDist" Visible="false" Font-Bold="true" runat="server">Edit </asp:HyperLink></th>
  </tr>
<tr>
    <td>
         <ul style="line-height:20px;">  
        <asp:Repeater ID="RepeaterDistList" DataSourceID="ds_DistList" runat="server">
         <ItemTemplate>
           ​<li  style=" margin: -7px 0; font-size:1.0em;">
                  <%# Eval("name_title").ToString() %>
            ​</li>
        </ItemTemplate>
            </asp:Repeater>
     </ul>
    </td>
</tr>
 </table>
        </asp:Panel>
    <asp:Panel ID="PanelApproveNote" Visible="false" runat="server">
           <div class="fieldHeader">Reviewer/Approver Comments: </div>
    
        <asp:TextBox  ID="txtImage1" Name="txtImage1" runat="server"  Width="100%" TextMode="MultiLine"   >
        </asp:TextBox>
         <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtImage1" ValidationGroup="Val_Comment" runat="server"  Display="Dynamic"  CssClass="errorText"  ErrorMessage="Error: Comments is required."></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" Display="Dynamic"
                                        ControlToValidate="txtImage1"  ValidationGroup="Val_CommentLength"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>  
         <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic"
                                        ControlToValidate="txtImage1"  ValidationGroup="Val_Comment"
                                        ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}" 
                                        ErrorMessage="Error: input text exceeded 3800 characters" ForeColor="Red" ></asp:RegularExpressionValidator>
               <br />
     <asp:Label ID="lblWarning" runat="server" ForeColor="Red" Font-Size="Large" Font-Bold="true" Visible="false" Font-Italic="true" Text=""></asp:Label>
    </asp:Panel>
    <asp:Panel ID="Panel2Reviewer" Visible="false" runat="server">
           <table width="100%" id="editTable2">
         <tr>        <td style=" text-align:right;">  <span  class="fieldHeader">Tracking/Trending Code (select up to 3): </span> </td><td>
               <asp:DropDownList ID="drwCode1" AutoPostBack="false" DataSourceID="ds_code"
                   DataTextField="code" DataValueField="RIRCODE_ID"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList>    <br />
                <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" ValidationGroup="trending" runat="server" Display="Dynamic" ControlToValidate="drwCode1"  CssClass="errorText" 
                          ErrorMessage="Error: First Tracking/Trending Code is required!"></asp:RequiredFieldValidator>
                 <asp:DropDownList ID="drwCode2" AutoPostBack="false" DataSourceID="ds_code"
                   DataTextField="code" DataValueField="RIRCODE_ID"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList> <br />
                 <asp:DropDownList ID="drwCode3" AutoPostBack="false" DataSourceID="ds_code"
                   DataTextField="code" DataValueField="RIRCODE_ID"  AppendDataBoundItems="true"  runat="server">
                  <asp:ListItem Selected="True" Value="-1">-- Please Select --</asp:ListItem>
              </asp:DropDownList> 
           
          </td>   </tr>
         <tr>        <td style=" text-align:right;">  <span  class="fieldHeader">Significance Level: </span> </td><td>
                <asp:DropDownList ID="drwLevel" runat="server">
                  <asp:ListItem Value="P1">P1</asp:ListItem>
                  <asp:ListItem Value="P2">P2</asp:ListItem>
                  <asp:ListItem Value="P3">P3</asp:ListItem>
                  <asp:ListItem Value="P4" Selected="True">P4</asp:ListItem>
              </asp:DropDownList>
              <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator1" ValidationGroup="trending" runat="server" Display="Dynamic" ControlToValidate="drwLevel"  CssClass="errorText" 
                          ErrorMessage="Error: Significance Level is required!"></asp:RequiredFieldValidator>
          </td>   </tr>
               </table>
    </asp:Panel>
        <div style="height:15px"></div>
      <asp:Button ID="btnReview" runat="server" Text="Request for Reviewers to Approve" Visible="false" Font-Bold="true" Font-Size="X-Large"  
          ValidationGroup="trending"   UseSubmitBehavior="false" OnClientClick="disableButton(this, 'trending');" 
          OnClick="btnReview_Click" /> &nbsp;&nbsp;&nbsp;
       <asp:Button ID="btnApprove" runat="server" Text="Approve" Visible="false" Font-Bold="true" Font-Size="X-Large" ValidationGroup="Val_CommentLength" 
          OnClick="btnApprove_Click" /> &nbsp;&nbsp;&nbsp;
       <asp:Button ID="btnReject" runat="server" Text="Reject" Visible="false" Font-Bold="true" Font-Size="X-Large" ValidationGroup="Val_Comment" 
          OnClick="btnReject_Click" /> &nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnCancel" runat="server" Text="Back to Dashboard" Font-Bold="true" Font-Size="X-Large" OnClick="btnCancel_Click" />

      <asp:SqlDataSource ID="ds_approver" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand=" select rev.REVIEWER_SID,title,p.name,  to_char(Date_Emailed,'MM/DD/YYYY') as Date_Sent, to_char(Date_Respond,'MM/DD/YYYY') as Date_Response, 
          NOTE,  decode(app.IS_ACCEPTED,'Y','Approved','N','Rejected') as IS_ACCEPTED
from SIIMS_RIR_REPORT_APPROVE app
join APPS_ADMIN.VW_SIIMS_RIR_REVIEWER rev on app.REVIEWER_SID=rev.REVIEWER_SID and NVL(app.is_owner,'N')=rev.is_owner
join persons.person p on p.key=app.REVIEWER_SID
where app.IS_RESPOND='Y' and app.issue_id=:IID
order by Date_Emailed desc"> 
              <SelectParameters>
                       <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="Int32" />
              </SelectParameters>
        </asp:SqlDataSource>
      <asp:SqlDataSource ID="ds_code" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand=" select RIRCODE_ID, CATEGORY || '-' || Code as code from SIIMS_RIR_CODE
where is_active='Y' order by category"> 
        </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="ds_DistList" runat="server"   
         ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
            SelectCommand="SELECT  p.name  || ' - ' || vw.VIEWER_TITLE as name_title
from SIIMS_RIR_VIEW vw
join PERSONS.PERSON p on p.key=vw.VIEWER_SID
where ISSUE_ID=:IID and vw.IS_ACTIVE='Y'
order by name "> 
         <SelectParameters>
                       <asp:QueryStringParameter Name="IID" QueryStringField="iid" DefaultValue="-1" Type="Int32" />
              </SelectParameters>
        </asp:SqlDataSource>
    	<script src='../scripts/autosize.js'></script>
     <script type="text/javascript">
         autosize(document.getElementById('<%=txtImage1.ClientID%>'));
     </script>
</asp:Content>
