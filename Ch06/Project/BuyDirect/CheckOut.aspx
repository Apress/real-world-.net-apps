<%@ Page Language="vb" AutoEventWireup="false" Src="CheckOut.aspx.vb" Inherits="BuyDirect.CheckOut"%>
<%@ Register TagPrefix="BuyDirect" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Footer" Src="Footer.ascx" %>
 
<html>
<head>
<title>Check Out</title>
<link rel=stylesheet type="text/css" href=Styles.css> 
</head>

<body>
<table border="0" cellspacing="0" cellpadding="4"
  width="<%=ConfigurationSettings.AppSettings("pageWidth")%>"> 

<tr>
  <td colspan="2">
    <%-- The header --%>
    <BuyDirect:Header id="header" runat="server"/>
  </td>
</tr>
<tr>
  <td valign="top"> 

    <%-- The menu --%>
    <BuyDirect:Menu id="menu" runat="server"/>

  </td>

  <td valign="top">
    <%-- Check Out --%>
    <asp:Label id="header1" runat="server" CssClass="PageHeader" Text="Check Out"/>
    <p> 
    <asp:Label id="header2" runat="server" CssClass="SecondPageHeader" Text="Please enter your details"/>
    <form runat="server" ID="Form1">
    <asp:Table id="myTable" runat="server">
      <asp:TableRow>
        <asp:TableCell CssClass="NormalLarge" ColumnSpan="3">Delivery Details:</asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell CssClass="NormalSmall">Contact Name:</asp:TableCell>
        <asp:TableCell><asp:TextBox runat="server" id="contactName"/></asp:TableCell>
        <asp:TableCell CssClass="ErrorMessage">
          <asp:RequiredFieldValidator id="contactNameValidator" ControlToValidate="contactName"
            EnableClientScript="False" runat="server" 
            ErrorMessage="Contact name required."
          />
        </asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell CssClass="NormalSmall">Delivery Address:</asp:TableCell>
        <asp:TableCell><asp:TextBox runat="server" id="deliveryAddress"/></asp:TableCell>
        <asp:TableCell CssClass="ErrorMessage">
          <asp:RequiredFieldValidator id="deliveryAddressValidator" ControlToValidate="deliveryAddress"
            EnableClientScript="False" runat="server" 
            ErrorMessage="Delivery address required."
          />
        </asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell CssClass="NormalLarge" ColumnSpan="3">Credit Card Details:</asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell CssClass="NormalSmall">Name on Credit Card:</asp:TableCell>
        <asp:TableCell><asp:TextBox runat="server" id="ccName"/></asp:TableCell>
        <asp:TableCell CssClass="ErrorMessage">
          <asp:RequiredFieldValidator id="ccNameValidator" ControlToValidate="ccName"
            EnableClientScript="False" runat="server" 
            ErrorMessage="Name on the credit card required."
          />
        </asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell CssClass="NormalSmall">Credit Card Number:</asp:TableCell>
        <asp:TableCell><asp:TextBox runat="server" id="ccNumber"/></asp:TableCell>
        <asp:TableCell CssClass="ErrorMessage">
          <asp:RequiredFieldValidator id="ccNumberValidator" ControlToValidate="ccNumber"
            EnableClientScript="False" runat="server" 
            ErrorMessage="Credit card number required."
          />
        </asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell CssClass="NormalSmall">Expiry Date (mm/yyyy):</asp:TableCell>
        <asp:TableCell><asp:TextBox runat="server" id="ccExpiryDate"/></asp:TableCell>
        <asp:TableCell CssClass="ErrorMessage">
          <asp:RequiredFieldValidator id="ccExpiryDateValidator1" ControlToValidate="ccExpiryDate"
            EnableClientScript="False" runat="server" 
            ErrorMessage="Expiry date required."
          />
          <asp:RegularExpressionValidator id="ccExpiryDateValidator2" runat="server" 
            ControlToValidate="ccExpiryDate"
            ErrorMessage="Invalid Expiry Date."
            ValidationExpression="^([0|1][0-9]/[0-9]{4})$"/>
        </asp:TableCell>
      </asp:TableRow>
      <asp:TableRow>
        <asp:TableCell/>
        <asp:TableCell ColumnSpan="3">
          <asp:Button runat="server" Text="Submit" OnClick="ProcessPurchase" ID="Button1" NAME="Button1"/>
        </asp:TableCell>
      </asp:TableRow>
    </asp:Table>
    </form>
  </td>
</tr>
<tr>
  <td colspan="2">
    <%-- The footer --%>
    <BuyDirect:Footer id="footer" runat="server"/>
  </td>
</tr>
</table> 
</body>
</html>
