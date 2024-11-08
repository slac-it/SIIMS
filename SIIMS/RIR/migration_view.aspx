<%@ Page Title="View Migration Report" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="migration_view.aspx.cs" Inherits="SIIMS.RIR.migration_view" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div  class="pageHeader">View Migrated RIR Report </div>
    <table width="100%" id="editTable">
         <tr>
        <td style="text-align:left;">
            <div> <span  class="fieldHeader">Title of Report:</span>
                   <asp:Label ID="lblTitle" runat="server" CssClass="displayData" Text="">             </asp:Label>
            </div> 
        </td> </tr>
           <tr>
        <td style="text-align:left;">
            <div> <span  class="fieldHeader">SIIMS Issue Status:</span>
                   <asp:Label ID="lblIssue_Status" runat="server" CssClass="displayData" Text="">             </asp:Label>
            </div> 
        </td> </tr>
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
             <tr>
          <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Issued Date:    </span>
                  <asp:Label ID="lblIssueDate" runat="server" CssClass="displayData"></asp:Label>   </div>
           </td>             
        </tr> 
           <tr>
          <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Source Title:    </span>
                  <asp:Label ID="lblSource" runat="server" CssClass="displayData"></asp:Label>   </div>
           </td>             
        </tr> 
           <tr>
          <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Source Calendar Year:   </span>
                  <asp:Label ID="lblCY" runat="server" CssClass="displayData"></asp:Label>   </div>
           </td>             
        </tr> 
          <tr>
          <td style=" text-align:left;">
               <div> <span  class="fieldHeader">Source Calendar Quarter:  </span>
                  <asp:Label ID="lblCQ" runat="server" CssClass="displayData"></asp:Label>   </div>
           </td>             
        </tr> 
        <tr>
        <td style="text-align:left;"> 

              <div class="fieldHeader">Statement of Issue, Error Precursor, or Improvement Opportunity:       </div>
              <div style="margin-left:20px; border-style:solid; border-width:1px; padding:2px;">
                        <asp:Label ID="lblStatement" runat="server" CssClass="displayData" Text=""></asp:Label>
                   </div>
           
        </td>
        </tr>
           
    

                  
                  <tr style="height:15px;"><td></td></tr>
     </table>


</asp:Content>
