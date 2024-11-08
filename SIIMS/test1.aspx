<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test1.aspx.cs" Inherits="SIIMS.test1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <h1> Convert Office Word string to normal ASCII string</h1>
    <form id="form1" runat="server">
   <div>
       Input from Word document (or other office doc like Excel). This input data is converted and saved to DB.
       <br />
       <div style="width:200px; background-color:blue; height:15px; float:left;"></div>
   </div>
          <br />
        <asp:TextBox ID="txtInput" runat="server" Height="100px" Width="90%" TextMode="MultiLine"></asp:TextBox>

        <br />
        <br />
After clicking "Convert" button below, the text below is read from database (meaning the data below should be in correct format):
        <strong>
      
        If you find any bad charcaters below, please contact Jay.</strong>
        <br />  <br />
  <asp:TextBox ID="txtOutput" runat="server" Height="100px" Width="90%" TextMode="MultiLine"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" Text="Convert" OnClick="Button1_Click" style="font-weight: 700; font-size: large; text-align: center" />
    </form>
</body>
</html>
