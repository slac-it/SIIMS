<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="charting_test.aspx.cs" Inherits="SIIMS.charting_test" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <asp:Chart ID="Chart1" runat="server"  Height="300px" Width="1080px" OnDataBound="Chart1_DataBound">
            <Titles><asp:Title Text="Fall Protection Issues"></asp:Title></Titles>
            <series>
                <asp:Series Name="Series1" ChartType="Line" ChartArea="ChartArea1">
                    <EmptyPointStyle IsValueShownAsLabel="false" IsVisibleInLegend="false" />
                   <%-- <Points>
                        <asp:DataPoint AxisLabel="FY15-Q3" YValues="1" />
                        <asp:DataPoint AxisLabel="FY15-Q4" YValues="0" />
                         <asp:DataPoint AxisLabel="FY16-Q1" YValues="3" />
                        <asp:DataPoint AxisLabel="FY16-Q2" YValues="2" />
                         <asp:DataPoint AxisLabel="FY16-Q3" YValues="5" />
                        <asp:DataPoint AxisLabel="FY16-Q4" YValues="4" />
                    </Points>--%>
                </asp:Series>
            </series>
            <chartareas>
                <asp:ChartArea Name="ChartArea1">
                    <AxisY Title="Number of Issues"></AxisY>
                </asp:ChartArea>
            </chartareas>
        </asp:Chart>
    </form>
</body>
</html>
