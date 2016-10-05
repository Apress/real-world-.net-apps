<%@ Page Language="vb" AutoEventWireup="false" Src="Browse.aspx.vb" Inherits="BuyDirect.Browse"%>
<%@ Register TagPrefix="BuyDirect" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Footer" Src="Footer.ascx" %>
 
<html>
<head>
<title>Browse Products By Category</title>
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
    <%-- Browse Products By Category --%>
    <div class="PageHeader">Products By Category</div>
    <asp:Label id="message" runat="server"/>
    <p>
    <form runat=server ID="Form2">
    <asp:DataGrid id="CatalogGrid" runat="server"
      AllowPaging="True"
      PageSize="8"
      OnPageIndexChanged="CatalogGrid_PageIndexChanged"
      BorderColor="black"
      BorderWidth="0"
      CellPadding="3"
      CellSpacing="0"
      CssClass="TableItem"
      Width="550"
      GridLines="None">
  
      <PagerStyle Mode="NumericPages" HorizontalAlign="Right"/>
      <HeaderStyle BackColor="#CCCCCC" CssClass="TableHeader"/>
      <AlternatingItemStyle CssClass="AlternatingTableItem" BackColor="#EEEEEE"/>

    </asp:DataGrid>
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
