<%@ Page Title="Trending Report" Language="C#" MasterPageFile="~/user.Master" AutoEventWireup="true" CodeBehind="rep_issueCodes.aspx.cs" Inherits="SIIMS.rep_issueCodes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .paddingCell {
            padding:5px;
        }

        .link-btn {
    color: black;
    background: white;
    padding: 5px 10px;
    font-weight:bold;
    font-size: larger;
    border: 2px #555555 solid;
    height:50px;
}

          .link-btn2 {
    color: black;
    background: white;
    padding: 0px 10px;
    font-weight:bold;
    font-size: larger;
    border: 2px #555555 solid;
    height:32px;
}

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div  class="pageHeader">Trending Report</div>

    <asp:Panel ID="PanelSelection" runat="server">
         <div style="font-weight:bold; margin-bottom:10px;"> Please select a start and end date for your trending report:  </div> 
        <span style="font-weight:bold;"> Start Date  </span> &nbsp;
           <div style="text-align:left;margin-top:10px;">
                
                   <div style="margin-top:10px; margin-left:30px;"> Year:  
                 <asp:dropdownlist id="drwFY_start" runat="server"  DataSourceID="ds_SourceFY" 
                     DataTextField="FY" DataValueField="Year" ></asp:dropdownlist>   
                       <asp:RequiredFieldValidator InitialValue="-1" ID="Req_ID" Display="Dynamic" 
    ValidationGroup="g1" runat="server" ControlToValidate="drwFY_start" 
    Text="* Please select Start Year!" ForeColor="Red" Font-Bold="true" Font-Size="Large" ErrorMessage="Please select Start Year!"></asp:RequiredFieldValidator>
                &nbsp; &nbsp; &nbsp; Quarter:
                  <asp:dropdownlist id="drwQtr_start" runat="server">
                        <asp:ListItem Selected="True" Value="-1">-- Select Start Quarter --</asp:ListItem>
                      <asp:ListItem>1</asp:ListItem>
                      <asp:ListItem>2</asp:ListItem>
                      <asp:ListItem>3</asp:ListItem>
                      <asp:ListItem>4</asp:ListItem>
                    
                       </asp:dropdownlist>   
                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator2" Display="Dynamic" 
    ValidationGroup="g1" runat="server" ControlToValidate="drwQtr_start" 
    Text="* Please select Start Quarter!" ForeColor="Red" Font-Bold="true" Font-Size="Large" ErrorMessage="Please select Start Quarter!"></asp:RequiredFieldValidator>   
                 </div>
        </div>
         <div style="margin-top:20px;">
          <span style="font-weight:bold; margin-top:20px;">  End Date  </span> &nbsp;
           <div style="text-align:left;margin-top:10px;">
                   <div style="margin-top:10px;margin-left:30px;">Year:  
                 <asp:dropdownlist id="drwFY_end" runat="server"  DataSourceID="ds_SourceFY2" 
                     DataTextField="FY" DataValueField="Year" ></asp:dropdownlist>   
                         <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator1" Display="Dynamic" 
    ValidationGroup="g1" runat="server" ControlToValidate="drwFY_end" 
    Text="* Please select End Year!" ForeColor="Red" Font-Bold="true" Font-Size="Large" ErrorMessage="Please select End Year!"></asp:RequiredFieldValidator>
                &nbsp; &nbsp; &nbsp; Quarter:
                  <asp:dropdownlist id="drwQtr_end" runat="server">
                        <asp:ListItem Selected="True" Value="-1">-- Select End Quarter --</asp:ListItem>
                      <asp:ListItem>1</asp:ListItem>
                      <asp:ListItem>2</asp:ListItem>
                      <asp:ListItem>3</asp:ListItem>
                      <asp:ListItem>4</asp:ListItem>
                    
                       </asp:dropdownlist>      
                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator3" Display="Dynamic" 
    ValidationGroup="g1" runat="server" ControlToValidate="drwQtr_end" 
    Text="* Please select End Quarter!" ForeColor="Red" Font-Bold="true" Font-Size="Large" ErrorMessage="Please select End Quarter!"></asp:RequiredFieldValidator>   
                 </div>
               <br />
               <asp:Label ID="lblErrMsg" Visible="false" ForeColor="Red" Font-Bold="true" Font-Size="Large" runat="server" Text=""></asp:Label>
              <%-- <asp:CompareValidator id="Comparedates" runat="server" ControlToValidate="drwFY_start" 
                    ForeColor="Red" Font-Bold="true" Font-Size="Large"
                   ControlToCompare="drwFY_end" Type="Integer" Operator="LessThanEqual" ErrorMessage="Start Year must be less than or equal to End Year!" />--%>
       </div>
 

  </div>

<div style="margin:30px 0 20px 50px;">

   <asp:Button ID="btnGo"  CssClass="link-btn2" runat="server" Text="Submit"  ValidationGroup="g1" OnClick="btnGo_Click" />
    &nbsp; &nbsp; &nbsp; &nbsp;
        <%-- <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="link-btn2" onclick="btnClear_Click" />--%>
      <asp:Button ID="btnCancel" runat="server" Text="Cancel"  CssClass="link-btn2" OnClick="btnCancel_Click"  />
</div>
 
        </asp:Panel>
  
    <table width="100%">
       
      <asp:Panel ID="PanelReport" Visible="false" runat="server">
           <tr>
            <td colspan="3" style="text-align:center; font-weight:bold;">
                <asp:Label ID="lblRange" runat="server" Text=""></asp:Label></td>
        </tr>
          <tr ><td  colspan="3" style="height:10px;"></td></tr>
        <asp:HiddenField ID="MAXWORKTYPE" Value="1" runat="server" />
         <asp:HiddenField ID="MAXFUNCTIONAL"  Value="1" runat="server" />
            <tr>
                <th style="width:48%; text-align:center;font-size:1.1em;">Issues by Work Type</th>
                <td style="width:1%;"></td>
                <th style="text-align:center;font-size:1.1em;">Issues by Functional Area</th>
            </tr>
            <tr>
                <td style="vertical-align:top;">
                    <asp:GridView ID="grw_workType" runat="server"  ShowHeader="False" DataKeyNames="icode_id"   OnRowCommand = "wt_OnRowCommand" EmptyDataText="No Issues found in the selected date range!"
                         AutoGenerateColumns="False">
                        <Columns>
                             <%--  <asp:ButtonField Text="Select" CommandName="Select" ItemStyle-Width="50" />--%>
                           

                            <asp:BoundField HeaderText="Work Type" DataField="worktype"  ItemStyle-CssClass="paddingCell" >
                                <ItemStyle CssClass="paddingCell" HorizontalAlign="Right" Width="150px" />
                            </asp:BoundField>

                             <asp:ButtonField CommandName = "ButtonField" HeaderText="#" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="paddingCell"
                                  DataTextField = "NCount" ItemStyle-ForeColor="Green"  ButtonType = "Button"/>
              
                            
                            <asp:TemplateField HeaderText="" ItemStyle-Width="310px" ItemStyle-HorizontalAlign="Left">         
                               <ItemTemplate>
                                <%# FormatWorkType(300,Eval("NCount").ToString()) %>                         
                               </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" Width="310px" />
                             </asp:TemplateField>    
                            
                        </Columns>                      
                         <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White"  Font-Size="1.1em" />
                         <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Size="1.0em" Height="1.5em" />
                         <AlternatingRowStyle BackColor="White" ForeColor="#284775" Font-Size="1.0em" Height="1.5em" />
                    </asp:GridView>

                </td>
                <td></td>
                <td style="vertical-align:top;">
                     <asp:GridView ID="grw_functional" runat="server"  ShowHeader="False" DataKeyNames="icode_id"   OnRowCommand = "fn_OnRowCommand" EmptyDataText="No Issues found in the selected date range!"
                         AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField HeaderText="Functional Area" DataField="functional" ItemStyle-Width="220px" ItemStyle-CssClass="paddingCell"
                                ItemStyle-HorizontalAlign="Right" />
                              <asp:ButtonField CommandName = "ButtonField" HeaderText="#" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="paddingCell"
                                  DataTextField = "NCount" ItemStyle-ForeColor="Green"  ButtonType = "Button"/>

                          <%--  <asp:BoundField HeaderText="#" DataField="NCount" ItemStyle-Width="30px" 
                                ItemStyle-CssClass="paddingCell" ItemStyle-HorizontalAlign="Right" />--%>

                            <asp:TemplateField HeaderText="" ItemStyle-Width="310px" ItemStyle-HorizontalAlign="Left">      
                            <ItemTemplate>
                                 <%# FormatFunctional(300,Eval("NCount").ToString()) %>
                            </ItemTemplate>
                </asp:TemplateField>    
                        </Columns>                      
                         <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White"  Font-Size="1.1em" />
                         <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Size="1.0em" Height="1.5em" />
                         <AlternatingRowStyle BackColor="White" ForeColor="#284775" Font-Size="1.0em" Height="1.5em" />
                    </asp:GridView>
                   
                </td>
            </tr>
           <tr><td colspan="3" style=" height:20px;"> </td></tr>
           <%-- <tr><td colspan="3" style="text-align:left; margin-left:50px; height:50px;"> 
                 <asp:HyperLink ID="lnkexpCode" Font-Bold="true" Font-Size="Larger" Target="_blank" runat="server">Export Issues to Excel</asp:HyperLink>
                </td></tr>--%>

           <tr><td colspan="3" style="height:25px;"></td></tr>
                <tr><td colspan="3" style="text-align:center;">
                   <%-- <asp:LinkButton ID="btnExpCode" CssClass="link-btn" PostBackUrl="~/rep_expIssueCode.aspx"   runat="server">Export Issues to Excel</asp:LinkButton>--%>
                     <asp:HyperLink ID="lnkexpCode" CssClass="link-btn" NavigateUrl="~/rep_expIssueCode.aspx" Target="_blank" runat="server">Export Issues to Excel</asp:HyperLink>
                     &nbsp;&nbsp; &nbsp;&nbsp;
                     <asp:Button ID="btnBack"  OnClick="btnBack2Selection_Click" runat="server" CssClass="link-btn2"  Text="Back to Date Selection" />
                    </td></tr>
       </asp:Panel>

        <asp:Panel ID="PanelCharting" Visible="false" runat="server">
             <tr>
            <td colspan="3" style="text-align:center; font-weight:bold;">
                <asp:Label ID="lblDataRange2" runat="server" Text=""></asp:Label></td>
        </tr>
          <tr ><td  colspan="3" style="height:10px;"></td></tr>
                <asp:HiddenField ID="MAXISSUECODE" Value="1" runat="server" />
                <asp:HiddenField ID="MAXACTIONTYPE" Value="1" runat="server" />
            <tr><td colspan="3" style="text-align:center;">
                 <asp:Chart ID="Chart1" runat="server" Height="300px" Width="1080px">
            <Titles><asp:Title Font="Arial, 16pt, style=Regular"  Text=""></asp:Title></Titles>
            <series>
                <asp:Series Name="Series1" ChartType="Line" ChartArea="ChartArea1">
                   
                </asp:Series>
            </series>
            <chartareas>
                <asp:ChartArea Name="ChartArea1">
                    <AxisY Title="Number of Issues"></AxisY>
                </asp:ChartArea>
            </chartareas>
        </asp:Chart>
                </td></tr>
      <tr><td colspan="3" style="height:15px;"></td></tr>
                <tr>
                <th style="width:51%; text-align:left;font-size:1.1em;">
                    <asp:Label ID="lblIssueTitle" runat="server" Text="Issues by Functional Area"></asp:Label></th>
                <td style="width:1%;"></td>
                <th style="text-align:center; font-size:1.1em;">Action Types</th>
            </tr>
            <tr>
                <td style="vertical-align:top;">
                    <asp:GridView ID="grw_IssueCodes" runat="server"  ShowHeader="False" EmptyDataText="No Issues Found!"
                         AutoGenerateColumns="False">
                        <Columns>
                           
                            <asp:BoundField DataField="icode"  ItemStyle-CssClass="paddingCell" >
                                <ItemStyle CssClass="paddingCell" HorizontalAlign="Right" Width="210px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="NCount"  ItemStyle-CssClass="paddingCell">
                                <ItemStyle CssClass="paddingCell" HorizontalAlign="Right" Width="30px" />
                            </asp:BoundField>
                
                            
                            <asp:TemplateField>         
                               <ItemTemplate>
                                <%# FormatIssueCode(300,Eval("NCount").ToString()) %>                         
                               </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" Width="310px" />
                             </asp:TemplateField>    
                            
                        </Columns>                      
                         <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Size="1.0em" Height="1.5em" />
                         <AlternatingRowStyle BackColor="White" ForeColor="#284775" Font-Size="1.0em" Height="1.5em" />
                    </asp:GridView>
                    <br />
                   
                </td>
                <td></td>
                <td style="vertical-align:top;">
                     <asp:GridView ID="grw_ActionCodes" runat="server"  ShowHeader="False" EmptyDataText="No Actions Found!"
                         AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField  DataField="acode" ItemStyle-Width="130px" ItemStyle-CssClass="paddingCell"
                                ItemStyle-HorizontalAlign="Right" />
                         <asp:BoundField  DataField="NCOUNT" ItemStyle-Width="30px" ItemStyle-CssClass="paddingCell"
                                ItemStyle-HorizontalAlign="Right" />

                            <asp:TemplateField  ItemStyle-Width="310px" ItemStyle-HorizontalAlign="Left">      
                            <ItemTemplate>
                                 <%# FormatActionType(300,Eval("NCount").ToString()) %>
                            </ItemTemplate>
                </asp:TemplateField>    
                        </Columns>                      
                         <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Size="1.0em" Height="1.5em" />
                         <AlternatingRowStyle BackColor="White" ForeColor="#284775" Font-Size="1.0em" Height="1.5em" />
                    </asp:GridView>
                    <br />
                     
                   
                </td>
            </tr>

            <tr><td colspan="3" style="height:25px;"></td></tr>
                <tr><td colspan="3" style="text-align:center;">
                  
                    
                    <asp:Button ID="btnBack2Graph" OnClick="btnBack2Graph_Click" runat="server"  CssClass="link-btn2" Text="Back to Previous Graphs" />
                    &nbsp;&nbsp;
                     <asp:Button ID="btnBack2Selection"  OnClick="btnBack2Selection_Click" runat="server"   CssClass="link-btn2" Text="Back to Date Selection" />
                     &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
                     <asp:HyperLink ID="lnkExpIssues" CssClass="link-btn" Target="_blank" runat="server">Export Issues to Excel</asp:HyperLink>
                     &nbsp;&nbsp; 
                       <asp:HyperLink ID="lnkExpActions" CssClass="link-btn" Target="_blank" runat="server">Export Actions to Excel</asp:HyperLink>
                    </td></tr>

             </asp:Panel>
        </table>


     <asp:SqlDataSource ID="ds_SourceFY" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '1' as order_id, '-1' as Year ,'-- Select Start Year  --' as FY from dual
union select  distinct '2' as order_id, src.FY as Year, src.FY 
from siims_source src
where src.is_active='Y' order by 1, 2 desc ">
         
    </asp:SqlDataSource>  

     <asp:SqlDataSource ID="ds_SourceFY2" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SLAC_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:SLAC_WEB.ProviderName %>"
             SelectCommand="select '1' as order_id, '-1' as Year ,'-- Select End Year  --' as FY from dual
union select  distinct '2' as order_id, src.FY as Year, src.FY 
from siims_source src
where src.is_active='Y' order by 1, 2 desc ">
         
    </asp:SqlDataSource>  

   

  
</asp:Content>
