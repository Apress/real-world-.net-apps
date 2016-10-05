<%@ Page Language="vb" AutoEventWireup="false" Src="ShoppingCart.aspx.vb" Inherits="BuyDirect.ShoppingCart"%>
<%@ Register TagPrefix="BuyDirect" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="BuyDirect" TagName="Footer" Src="Footer.ascx" %>
 
<html>
<head>
<title>Shopping Cart</title>
<link rel=stylesheet type="text/css" href=Styles.css> 
</head>

<%-- User Interface --%>
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
    <%-- The Shopping Cart --%>
    <div class="PageHeader">Shopping Cart</div>
    <p> 
    <form runat="server" ID="Form1">
 
    <asp:DataGrid id="MyDataGrid" runat="server"
      BorderColor="black"
      BorderWidth="1"
      CellPadding="3"
      CssClass="TableItem"
      OnEditCommand="MyDataGrid_Edit"
      OnCancelCommand="MyDataGrid_Cancel"
      OnUpdateCommand="MyDataGrid_Update"
      OnDeleteCommand="MyDataGrid_Delete"
      AllowSorting="True"
      AutoGenerateColumns="false">

      <HeaderStyle BackColor="#CCCCCC" CssClass="TableHeader"/>

      <EditItemStyle BackColor="#EDEDED"/>
 
      <Columns>
        <asp:EditCommandColumn
          EditText="Edit"
          CancelText="Cancel"
          UpdateText="Save"
          HeaderText="Edit">
          <ItemStyle Wrap="false"/>
          <HeaderStyle Wrap="false"/>
        </asp:EditCommandColumn>
 
        <asp:ButtonColumn
          HeaderText="Delete"
          ButtonType="LinkButton"
          Text="Delete"
          CommandName="Delete"
        />
        <asp:BoundColumn HeaderText="Name" 
          ReadOnly="True" 
          DataField="Name"/>
        <asp:BoundColumn HeaderText="Description" 
          ReadOnly="True" 
          DataField="Description"/>

        <asp:BoundColumn HeaderText="Quantity" 
          DataField="Quantity">
          <ItemStyle HorizontalAlign="Right"/>
        </asp:BoundColumn>

        <asp:BoundColumn HeaderText="Price" 
          ReadOnly="True" 
          DataField="Price">
          <ItemStyle HorizontalAlign="Right"/>
        </asp:BoundColumn>
 
        <asp:BoundColumn HeaderText="Subtotal" 
          ReadOnly="True" 
          DataField="Subtotal">
          <ItemStyle HorizontalAlign="Right"/>
        </asp:BoundColumn>

        <asp:BoundColumn HeaderText="ProductId" 
          ReadOnly="True" 
          Visible="False"
          DataField="ProductId"/>

      </Columns>

    </asp:DataGrid>
    <p>
    <asp:Label id="totalLabel" runat="server"/> 
  </form>
  <a href="Default.aspx">Main page</a>
  <a href="CheckOut.aspx">Check Out</a> 
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
