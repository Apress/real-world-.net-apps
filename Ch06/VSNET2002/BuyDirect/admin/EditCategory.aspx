<%@ Page Inherits="EditCategory" src="EditCategory.aspx.vb" %>
<html>
<head>
<title>Edit Category</title>

<script language="VB" runat="server">

</script>
</head>


<%-- User Interface --%>
<body>
<h3><font face="verdana">Managing Product Categories</font></h3>
<br>
<hr>
<h4>Add Category</h4>
<form runat="server">
  <asp:TextBox id="newCategory" runat="server" />
  <asp:Button runat="server" onClick="AddCategory"
    Text="Add Category"/>
 
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
          HeaderText="Edit Category">
          <ItemStyle Wrap="false"/>
          <HeaderStyle Wrap="false"/>
        </asp:EditCommandColumn>
 
        <asp:ButtonColumn
          HeaderText="Delete Category"
          ButtonType="LinkButton"
          Text="Delete"
          CommandName="Delete"
        />
        <asp:BoundColumn HeaderText="Category Id" 
          ReadOnly="True" 
          DataField="CategoryId"/>

        <asp:BoundColumn HeaderText="Category" 
          DataField="Category"/>

      </Columns>

    </asp:DataGrid>
    <p>
  </form>
<hr>
<br>
<a href="Admin.aspx">Admin menu</a>
 
</body>
</html>
