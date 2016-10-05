<%@ Register TagPrefix="BuyDirect" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Footer" Src="Footer.ascx" %>

<html>
<head>
<title>Welcome to BuyDirect</title>
<link rel=stylesheet type="text/css" href=Styles.css> 
</head>

<body>
<table border="0" cellspacing="4" cellpadding="0"
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
    <%-- Welcome page --%>
    <br>
    <div class="NormalLarge">Welcome to Buy Direct</div>
    <br> 
    <div class="NormalSmall">Try our lowest prices today!</div>
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
