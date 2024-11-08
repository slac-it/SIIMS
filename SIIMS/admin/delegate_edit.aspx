<%@ Page Title="Edit Delegates" Language="C#" MasterPageFile="~/admin/adminMaster.Master" AutoEventWireup="true" CodeBehind="delegate_edit.aspx.cs" Inherits="SIIMS.admin.delegate_edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div>
    <div  class="pageHeader">Organization Delegates for SMT Org:  <asp:Label ID="lblOrgName" Font-Bold="true" Font-Size="Larger" runat="server" Text=""></asp:Label> </div>
          
 

       <br />
        <!--  ListView or GridView with Remove and Add only. No Edit here   -->
        <asp:GridView ID="gvw_editSMT" runat="server" style="font-size: large" DataKeyNames="Delegate_ID"
                    CellPadding="4" ForeColor="#333333" CellSpacing="2" HorizontalAlign="Center" CssClass="gridviewCSS"
                    EmptyDataText="No SMT Org delegates found!" AutoGenerateColumns="False" Width="70%"  >

                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>             
                        <asp:BoundField HeaderText="Delegate_ID"  DataField="DELEGATE_ID" Visible="false"  />
                        <asp:BoundField HeaderText="SLAC ID" DataField="SID"  />
                        <asp:BoundField HeaderText="Name" DataField="NAME" />
                        <asp:BoundField HeaderText="Date Added" DataField="CREATED_ON" ItemStyle-HorizontalAlign="Center" />

                        <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" runat="server" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this person from delegate list?');"> Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

                       

                    </Columns>
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                </asp:GridView>
          <br />  <br /><br />
        <div class="addButton">
           <span style="font-weight:bold;">SLAC ID:</span>  <asp:TextBox ID="txtDelegate" MaxLength="20" runat="server"></asp:TextBox> &nbsp;&nbsp;
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtDelegate"  ForeColor="Red" Display="Dynamic" runat="server" ValidationGroup="slacid" ErrorMessage="Error: SLAC ID is required"></asp:RequiredFieldValidator> &nbsp;&nbsp;
            <asp:CompareValidator runat="server" Operator="DataTypeCheck" Type="Integer"  ForeColor="Red" Display="Dynamic"
 ControlToValidate="txtDelegate" ErrorMessage="Error: SLAC ID must be a whole number" />  &nbsp;&nbsp;
            <asp:Label ID="lblDelegateError" runat="server" ForeColor="Red" Visible="false" Text=""></asp:Label> 
            <br />
             <asp:TextBox ID="txtDelegateName" BackColor="DimGray" ForeColor="White" Visible="false" TextMode="SingleLine" Width="50%"  ReadOnly="true" Text="" runat="server"></asp:TextBox>   
            <br />
             <asp:Button ID="btnConfirm" runat="server"  Text="Confirm" Font-Bold="true" Font-Size="Large"  ValidationGroup="slacid" OnClick="btnConfirm_Click" />
            <asp:Button ID="btnAdd" runat="server"  Text="Add New Delegate" Font-Bold="true" Font-Size="Large" Visible="false"   OnClick="btnAdd_Click" />
        </div>

        <br />
            <br />
         
         <div style="text-align:center;">
              <asp:Button ID="btnCancel" runat="server" Text="Cancel" Font-Bold="true" Font-Size="X-Large"  OnClick="btnCancel_Click"  />

         </div>
     <br />
</div>
</asp:Content>
