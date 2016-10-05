<%@ Page Language="vb" AutoEventWireup="false" Src="ProductDetails.aspx.vb" Inherits="BuyDirect.ProductDetails"%>
<%@ OutputCache Duration="14400" VaryByParam="productId" %>
<%@ Register TagPrefix="BuyDirect" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Footer" Src="Footer.ascx" %>
 
<html>
<head>
<title>Product Details</title>
<link rel=stylesheet type="text/css" href=Styles.css> 

<script language="VB" runat="server">

</script>
</head>

<body bgcolor="<%=ConfigurationSettings.AppSettings("pageBackgroundColor")%>">
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
    <%-- The Product Details --%>
    <font class="PageHeader">Product Details</font>
    <p> 
    <form runat=server ID="Form2">
    <table border="0"
    <tr>
      <td>
        <asp:Image id="productImage" 
          runat="server" 
          width="200"
          height="150">
        </asp:Image>
      </td>
      <td>
    <asp:Label id="name" runat="server"/>
    <p>
    <asp:Label id="description" runat="server"/>
    <p>
    <asp:Label id="price" runat="server"/>
    <p>
    <asp:Hyperlink id="addToCart" 
      Text="Buy"
      runat="server"/>
      </td>
    </tr>
    </table>

    </form>
    <br>
    <br>
  </td>
</tr>
</table> 
</body>
</html>
