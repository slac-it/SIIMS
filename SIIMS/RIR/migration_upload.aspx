<%@ Page Title="Upload Attachment for Migration" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="migration_upload.aspx.cs" Inherits="SIIMS.RIR.migration_upload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

       <div  class="pageHeader">Upload PDF for Migrated Report </div>
      <div>
             This is for the migration report:<br />
           <div><span  class="fieldHeader">Report #: </span> <asp:Label ID="lblIssueID" runat="server" Text=""></asp:Label> </div>
        <div><span  class="fieldHeader">Title: </span> <asp:Label ID="lblTitle" runat="server" Text=""></asp:Label> </div>
      </div>

         <br />
              
     

             <div style="width:100%; margin-left:30px; margin-top:5px;">
              Upload PDF <span style="font-style:italic;">
                       (Note: only files with  .pdf with maximum of 30MB are supported.)</span>: 
                </div>  <div style="width:100%; margin-left:30px; margin-top:5px;">
                <asp:FileUpload ID="oFilePDF" runat="server" />
           </div>  <div style="width:100%; margin-left:30px; margin-top:5px;">
        <asp:button id="btnUploadDesc" type="submit" text="Upload PDF" Font-Bold="true" UseSubmitBehavior="false"
            ClientIDMode="Static"  runat="server" OnClick="btnUploadPDF_Click"></asp:button> 
              </div>  <div style="width:100%; margin-left:30px; margin-top:5px;">
                   <asp:Label ID="lblUploadPDF" Visible="false" ForeColor="Red" Font-Bold="true" runat="server" Text=""></asp:Label>
                 </div>

</asp:Content>
