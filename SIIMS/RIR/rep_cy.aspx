<%@ Page Title="Calendar Year Report" Language="C#" MasterPageFile="~/RIR/RIR.Master" AutoEventWireup="true" CodeBehind="rep_cy.aspx.cs" Inherits="SIIMS.RIR.rep_cy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <asp:Panel ID="PanelTitle" runat="server">
   <div >
        <span style="font-weight:bold;font-size:1.1em;"> Please select Organization:  </span> &nbsp;
           <asp:DropDownList ID="drwOrg" AutoPostBack="true" 
                   DataSourceID="ds_Org" DataTextField="name" DataValueField="org_ID" runat="server" AppendDataBoundItems="true">
                       <asp:ListItem Selected="True" Value="-1">-- All Organizations --</asp:ListItem>
              </asp:DropDownList>   
     </div>
      
             <div style="text-align:left;margin-top:20px;">
                 <div style="font-weight:bold;font-size:1.1em;"> Please select Departments/Groups:</div> 
                 <asp:ListBox id="drwDept" runat="server" AutoPostBack="false" SelectionMode="Multiple" Rows="8"
                      DataSourceID="ds_Dept" DataTextField="display_name" DataValueField="dept_id"  >
                 </asp:ListBox> &nbsp; &nbsp;
               
                  <br />   
               <div style="text-align:left;margin-top:20px;">
                 <div style="font-weight:bold;font-size:1.1em;"> Please select Trending/Tracking Codes:</div> 
                 <asp:ListBox id="drwCode" runat="server" AutoPostBack="false" SelectionMode="Multiple" Rows="8"
                      DataSourceID="ds_code" DataTextField="code" DataValueField="RIRCODE_ID"  >
                 </asp:ListBox> &nbsp; &nbsp;
               
                  <br />   
                   <div style="font-weight:bold; font-size:1.1em; margin-top:10px;"> You can filter by  Calendar Year and Quarter: </div> 
                
                   <div style="margin-top:5px;">     
                 <asp:dropdownlist id="drwCY" runat="server" AutoPostBack="true"  DataSourceID="ds_SourceCY" 
                     DataTextField="CY" DataValueField="Year" ></asp:dropdownlist>   
                &nbsp; &nbsp; &nbsp;
                  <asp:dropdownlist id="drwCQ" runat="server" AutoPostBack="true">
                      <asp:ListItem Value="-1" Selected="True">-- All Quarters --</asp:ListItem>
                       <asp:ListItem Value="1">First Quarter</asp:ListItem>
                       <asp:ListItem Value="2" >Second Quarter</asp:ListItem>
                       <asp:ListItem Value="3" >Third Quarter</asp:ListItem>
                       <asp:ListItem Value="4">Fourth Quarter</asp:ListItem>
                  </asp:dropdownlist>       <br />
                 </div>
                    <div style="font-weight:bold; font-size:1.1em; margin-top:10px;"> You can further filter by keyword (in title and statement): </div> 
                   <asp:TextBox ID="txtKeyword" MaxLength="100"  runat="server"></asp:TextBox>
                  
                 </div>

                </div>  

         <br />
       <asp:Button ID="btnView" runat="server" Text=" Submit " Font-Bold="True"  Font-Size="Larger" PostBackUrl="~/RIR/rep_cy_data.aspx"  /> 
          &nbsp; &nbsp; &nbsp; &nbsp;
         <asp:Button ID="btnClear" runat="server" Text="Clear" Font-Bold="True" 
               Font-Size="Larger" onclick="btnClear_Click" />
            </asp:Panel>     


     <asp:SqlDataSource ID="ds_Org" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select distinct st.org_id, st.name from siims_issue issue join siims_org st on st.org_id=issue.org_id
where issue.is_active='Y' and issue.is_RIR='Y' and issue.status_id <> 10 order by name ">
    </asp:SqlDataSource>   

       <asp:SqlDataSource ID="ds_Dept" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as dept_id , '----' as dept_name, '-- All Groups/Departments --' as display_name from dual
union select distinct dept_id, dept_name,  display_name
from vw_siims_rir_dept
where (:ORG_ID = -1 or org_id=:ORG_ID) order by 2">
          <SelectParameters>
                 <asp:ControlParameter ControlID="drwOrg" Name="ORG_ID" DefaultValue="-1" PropertyName="SelectedValue"  Type="Int32" />

      </SelectParameters>
    </asp:SqlDataSource>  

        <asp:SqlDataSource ID="ds_Code" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand=" select  -1 as RIRCODE_ID, 'A' as CATEGORY, '-- All Trending/Tracking Codes --' as code from dual 
            union select RIRCODE_ID, CATEGORY, CATEGORY || '-' || Code as code from SIIMS_RIR_CODE
where is_active='Y' order by 2 ">
    </asp:SqlDataSource>   
  
      <asp:SqlDataSource ID="ds_SourceCY" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '-1' as Year ,'--  All Years --' as CY from dual
union select distinct to_char(src.CY) as Year, to_char(src.CY) as CY
from siims_source src
join SIIMS_ISSUE issue on issue.issue_id=src.ISSUE_ID and  issue.is_active='Y' and issue.is_RIR='Y' and issue.status_id <> 10 
where src.is_active='Y'  ">
    </asp:SqlDataSource>  
 
</asp:Content>
