<%@ Page Title="Issues by Source" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_sourceIssue.aspx.cs" Inherits="SIIMS.rep_sourceIssue" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       <div>
    <div  class="pageHeader">Issue Report by Source</div>

 </div>
     <div >
        <span style="font-weight:bold;"> Please select Source:  </span> &nbsp;
           <asp:DropDownList ID="drwSType" AutoPostBack="true" 
                   DataSourceID="ds_SType" DataTextField="type" DataValueField="STYPE_ID" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="drwSType_SelectedIndexChanged">
                       <asp:ListItem Selected="True" Value="-1">--Please Select Source --</asp:ListItem>
              </asp:DropDownList>   
     </div>
        <asp:Panel ID="PanelTitle" Visible="false" runat="server">
             <div style="text-align:left;margin-top:20px;">
                 <div style="font-weight:bold;"> Please select Source Title:</div> 
                 <asp:dropdownlist id="drwTitle" runat="server" AutoPostBack="false"  
                      DataSourceID="ds_SourceTitle" DataTextField="Title" DataValueField="title_id"  >
                 </asp:dropdownlist> &nbsp; &nbsp;
                 <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator3" Display="Dynamic"   
                     ValidationGroup="allFields" runat="server" ControlToValidate="drwTitle"
                         ForeColor="Red"  ErrorMessage="Error: Title is required!"></asp:RequiredFieldValidator>
                  <br />   <br /> Narrow the source title list by:  
                   <div style="margin-top:10px;">     
                 <asp:dropdownlist id="drwSourceFY" runat="server" AutoPostBack="true"  DataSourceID="ds_SourceFY" 
                     DataTextField="FY" DataValueField="Year" ></asp:dropdownlist>   
                &nbsp; &nbsp; &nbsp;
                  <asp:dropdownlist id="drwSourceQtr" runat="server" AutoPostBack="true"  DataSourceID="ds_SourceQtr" 
                     DataTextField="QTR" DataValueField="QID"></asp:dropdownlist>       <br />
                 </div>
                </div>  

        </asp:Panel>               
            
              

         <br /><br />
       <asp:Button ID="btnView" runat="server" Text=" Submit " Font-Bold="True" Visible="false" ValidationGroup="allFields"
               Font-Size="Larger" onclick="btnView_Click" /> &nbsp; &nbsp; &nbsp; &nbsp;
         <asp:Button ID="btnClear" runat="server" Text="Clear" Font-Bold="True" 
               Font-Size="Larger" onclick="btnClear_Click" />



     <asp:SqlDataSource ID="ds_SType" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select STYPE_ID,type from siims_source_type where is_active='Y' 
          order by TYPE ">
    </asp:SqlDataSource>   

     <asp:SqlDataSource ID="ds_SourceTitle" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as title_id ,'-- Please Select Title  --' as Title from dual 
union select distinct src.title, src.title from siims_source src
join SIIMS_ISSUE issue on issue.issue_id=src.ISSUE_ID and issue.is_active='Y' and issue.STYPE_ID=:STYPEID and issue.status_id <> 10 
where src.is_active='Y'  and (:FYear='-1' or FY=:FYear) and (:Qtr='-1' or Quarter=:Qtr) 
 order by 2"   >
            <SelectParameters>
                 <asp:ControlParameter ControlID="drwSType" Name="STYPEID" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                <asp:ControlParameter ControlID="drwSourceFY" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwSourceFY" Name="FYear" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwSourceQtr" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
                  <asp:ControlParameter ControlID="drwSourceQtr" Name="Qtr" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />
      </SelectParameters>

    </asp:SqlDataSource>   
      <asp:SqlDataSource ID="ds_SourceFY" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as Year ,'-- Filter by FY  --' as FY from dual
union select distinct src.FY as Year, src.FY 
from siims_source src
join SIIMS_ISSUE issue on issue.issue_id=src.ISSUE_ID and issue.is_active='Y' and issue.STYPE_ID=:STYPEID and issue.status_id <> 10 
where src.is_active='Y' ">
          <SelectParameters>
                 <asp:ControlParameter ControlID="drwSType" Name="STYPEID" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />

      </SelectParameters>
    </asp:SqlDataSource>  

      <asp:SqlDataSource ID="ds_SourceQtr" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as qid ,'-- Filter by Quarter  --' as qtr from dual union select distinct src.quarter as qid, src.quarter as qtr 
from siims_source src
join SIIMS_ISSUE issue on issue.issue_id=src.ISSUE_ID and issue.is_active='Y' and issue.STYPE_ID=:STYPEID and issue.status_id <> 10
where src.is_active='Y'">
 <SelectParameters>
                 <asp:ControlParameter ControlID="drwSType" Name="STYPEID" DefaultValue="-1" PropertyName="SelectedValue"  Type="String" />

      </SelectParameters>
    </asp:SqlDataSource>   
</asp:Content>
