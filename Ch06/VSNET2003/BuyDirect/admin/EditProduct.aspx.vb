Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Configuration
Imports System.Text
Imports Microsoft.VisualBasic
Imports BuyDirect

Public Class EditProduct : Inherits Page

Public dataTable As New DataTable
Public dataView As DataView
Public connectionString As String = _
  ConfigurationSettings.AppSettings("connectionString")
Public categories As String 'used to populate the select box
    
Public MyDataGrid As DataGrid

Sub PopulateDataSet()
  Dim dbo As DbObject = DbObject.GetDbObject()
  Dim ds As DataSet = dbo.GetAllProducts()
  dataTable = ds.Tables("Products")
  dataView = New DataView(dataTable)
End Sub

Sub Page_Load(sender As Object, e As EventArgs)


  Dim newCategoryId As String = Request.Params("category")
  Dim newProductName As String = Request.Params("productName")
  Dim newProductDescription As String = _
    Request.Params("description")
  Dim newProductPrice As String = Request.Params("price")
  Dim dbo As DbObject = DbObject.GetDbObject()

  If Not (newProductName Is Nothing Or _
    newProductDescription Is Nothing Or _
    newProductPrice Is Nothing) Then
    If Not (newProductName.Equals("") Or _
      newProductPrice.Equals("")) Then
      dbo.InsertProduct(newCategoryId, newProductName, newProductDescription, newProductPrice)
    End If
  End If

  categories = dbo.GetAllCategoriesAsString()
      
  PopulateDataSet()
  If Not IsPostBack Then  
    BindGrid()
  End If

End Sub 'Page_Load

Sub MyDataGrid_Edit(sender As Object, _
  e As DataGridCommandEventArgs)
  MyDataGrid.EditItemIndex = e.Item.ItemIndex
  BindGrid()
End Sub 'MyDataGrid_Edit


Sub MyDataGrid_Cancel(sender As Object, _
  e As DataGridCommandEventArgs)
  MyDataGrid.EditItemIndex = - 1
  BindGrid()
End Sub 'MyDataGrid_Cancel

Sub MyDataGrid_Update(sender As Object, _
  e As DataGridCommandEventArgs)
  ' For bound columns the edited value is stored in a textbox.
  ' The textbox is the 0th element in the column's cell.

  Dim productId As String = e.Item.Cells(2).Text
  Dim productName As String = _
    CType(e.Item.Cells(3).Controls(0), TextBox).Text
  Dim productDescription As String = _
    CType(e.Item.Cells(4).Controls(0), TextBox).Text
  Dim productPrice As String = _
    CType(e.Item.Cells(5).Controls(0), TextBox).Text

  Dim dbo As DbObject = DbObject.GetDbObject()
  dbo.EditProduct(productId, productName, productDescription, productPrice)

  ' Now update the DataTable      
  Dim dr() As DataRow = _
    dataTable.Select("ProductId='" & productId & "'")
  If dr.Length > 0 Then
    Dim dataRow As DataRow = dr(0)
    dataRow.BeginEdit
    dataRow("Name") = productName
    dataRow("Description") = productDescription
    dataRow("Price") = productPrice
    dataRow.EndEdit
  End If

  MyDataGrid.EditItemIndex = - 1
  BindGrid()
End Sub 'MyDataGrid_Update


Sub MyDataGrid_Delete(sender As Object, _
  e As DataGridCommandEventArgs)

  Dim productId As String = e.Item.Cells(2).Text
  Dim dbo As DbObject = DbObject.GetDbObject()
  dbo.DeleteProduct(productId)

  ' Now delete the row from DataTable      
  dataView.RowFilter = "productId='" & productId & "'"
  If dataView.Count > 0 Then
    dataView.Delete(0)
  End If
  dataView.RowFilter = ""
  MyDataGrid.EditItemIndex = - 1
  BindGrid()
End Sub 'MyDataGrid_Delete


Sub BindGrid()

  MyDataGrid.DataSource = dataView
  MyDataGrid.DataBind()
End Sub 'BindGrid

Sub MyDataGrid_PageIndexChanged(sender As Object, _
  e As DataGridPageChangedEventArgs)
  myDataGrid.CurrentPageIndex = e.NewPageIndex
  BindGrid()
End Sub 

End Class
