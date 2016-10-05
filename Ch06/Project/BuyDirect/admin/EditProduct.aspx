<%@ Page Inherits="EditProduct" src="EditProduct.aspx.vb" %>

<html>
<head>
<title>Edit Product</title>
</head>


<%-- User Interface --%>
<body>
  <h3><font face="verdana">Products Administration</font></h3>
<hr>
<form method="post">
<h4>Add Product</h4>
<table>
<tr>
  <td>Category:</td>
  <td><select name="category"><%=categories%></select></td>
</tr>
<tr>
  <td>Product Name:</td>
  <td><input type=text name=productName></td>
</tr>
<tr>
  <td>Description:</td>
  <td>
    <textarea cols=25 rows=4 name=description></textarea>
  </td>
</tr>
<tr>
  <td>Price:</td>
  <td><input type=text name=price></td>
</tr>
<tr>
  <td colspan=2><input type=submit value="Add Product"></td>
</tr>
</table>
</form>
<hr>
<p>
        
<form runat="server">
  <asp:DataGrid id="MyDataGrid" runat="server"
    BorderColor="black"
    BorderWidth="1"
    CellPadding="3"
    Font-Name="Verdana"
    Font-Size="8pt"
    OnEditCommand="MyDataGrid_Edit"
    OnCancelCommand="MyDataGrid_Cancel"
    OnUpdateCommand="MyDataGrid_Update"
    OnDeleteCommand="MyDataGrid_Delete"
    OnPageIndexChanged="myDataGrid_PageIndexChanged"      
    AllowSorting="True"
    AllowPaging="True"
    PageSize="8"
    AutoGenerateColumns="False">  
    <PagerStyle Mode="NumericPages" HorizontalAlign="Right"/>
    <HeaderStyle BackColor="#aaaadd" ForeColor="white"/>
    <EditItemStyle BackColor="yellow"/>

    <Columns>
      <asp:EditCommandColumn
        EditText="Edit"
        CancelText="Cancel"
        UpdateText="Save"
        HeaderText="Edit Product">
        <ItemStyle Wrap="false"/>
        <HeaderStyle Wrap="false"/>
      </asp:EditCommandColumn>

      <asp:ButtonColumn
        HeaderText="Delete Product"
        ButtonType="LinkButton"
        Text="Delete"
        CommandName="Delete"
      />
      <asp:BoundColumn HeaderText="Product Id" 
        ReadOnly="True" 
        DataField="ProductId"/>

      <asp:BoundColumn HeaderText="ProductName" 
        DataField="Name"/>

      <asp:BoundColumn HeaderText="Description" 
        DataField="Description"/>

      <asp:BoundColumn HeaderText="Price" 
        DataField="Price"/>

    </Columns>

  </asp:DataGrid>
</form>
<br>
<hr>
<br>
<a href="Admin.aspx">Admin menu</a>
 
</body>
</html>
