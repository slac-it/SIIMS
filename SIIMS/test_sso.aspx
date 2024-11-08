<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test_sso.aspx.cs" Inherits="SIIMS.test_sso" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Hello SIIMS <br />
            <br />
  <%--          Request ServerVariables: <br />
             <asp:Label ID="lblRequest" runat="server" Text=""></asp:Label>
            <br />--%>

            Are you authenticated? <br />
            <asp:Label ID="lblSSO" runat="server" Text=""></asp:Label>
        </div>
    </form>
</body>
</html>
