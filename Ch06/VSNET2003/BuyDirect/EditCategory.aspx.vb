Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Configuration
Imports BuyDirect

Public Class EditCategory : Inherits Page

Public dataTable As New DataTable
Public dataView As DataView
Public MyDataGrid As DataGrid
Public newCategory As TextBox

Sub PopulateDataSet()
  Dim dbo As DbObject = DbObject.GetDbObject()
  Dim ds As DataSet = dbo.GetCategories()
  dataTable = ds.Tables("Categories")
  dataView = New DataView(dataTable)
End Sub

Sub Page_Load(sender As Object, e As EventArgs)
  PopulateDataSet()
  If Not IsPostBack Then  
    BindGrid()
  End If

End Sub 'Page_Load

Sub MyDataGrid_Edit(sender As Object, e As DataGridCommandEventArgs)
  MyDataGrid.EditItemIndex = e.Item.ItemIndex
  BindGrid()
End Sub 'MyDataGrid_Edit


Sub MyDataGrid_Cancel(sender As Object, e As DataGridCommandEventArgs)
  MyDataGrid.EditItemIndex = - 1
  BindGrid()
End Sub 'MyDataGrid_Cancel

Sub MyDataGrid_Update(sender As Object, e As DataGridCommandEventArgs)
  ' For bound columns the edited value is stored in a textbox.
  ' The textbox is the 0th element in the column's cell.

  Dim categoryText As TextBox = CType(e.Item.Cells(3).Controls(0), TextBox)
  Dim categoryId As String = e.Item.Cells(2).Text
  Dim category As String = categoryText.Text ' this is the updated category
  Dim dbo As DbObject = DbObject.GetDbObject()
  dbo.EditCategory(categoryId, category)

  ' Now update the DataTable      
  Dim dr() As DataRow = dataTable.Select("CategoryId='" & categoryId & "'")
  If dr.Length > 0 Then
    Dim dataRow As DataRow = dr(0)
    dataRow.BeginEdit
    dataRow("Category") = category
    dataRow.EndEdit
  End If

  MyDataGrid.EditItemIndex = - 1
  BindGrid()
End Sub 'MyDataGrid_Update


Sub MyDataGrid_Delete(sender As Object, e As DataGridCommandEventArgs)

  Dim categoryId As String = e.Item.Cells(2).Text
  Dim dbo As DbObject = DbObject.GetDbObject()
  dbo.DeleteCategory(categoryId)

  ' Now delete the row from DataTable      
  dataView.RowFilter = "categoryId='" & categoryId & "'"
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

Sub AddCategory(source As Object, e As EventArgs)
  Dim newCat As String = newCategory.Text.Trim()
  If Not newCat.Equals("") Then

    Dim dbo As DbObject = DbObject.GetDbObject()
    dbo.InsertCategory(newCat)

    newCategory.Text = ""
    PopulateDataSet()  
    BindGrid() 
  End If

End Sub

Sub MyDataGrid_PageIndexChanged(sender As Object, _
  e As DataGridPageChangedEventArgs)
  myDataGrid.CurrentPageIndex = e.NewPageIndex
  BindGrid()
End Sub 

End Class
