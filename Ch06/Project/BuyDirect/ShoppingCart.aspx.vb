Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Collections

Namespace BuyDirect

Public Class ShoppingCart
    Inherits System.Web.UI.Page

    Public dataTable As New DataTable
    Public CartView As DataView
    Public MyDataGrid As DataGrid
    Public totalLabel As Label

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        If Not Session("cart") Is Nothing Then
            Dim cart As Hashtable = CType(Session("cart"), Hashtable)

            dataTable.Columns.Add( _
              New DataColumn("Name", GetType(String)))
            dataTable.Columns.Add( _
              New DataColumn("Description", GetType(String)))
            dataTable.Columns.Add( _
              New DataColumn("Quantity", GetType(String)))
            dataTable.Columns.Add( _
              New DataColumn("Price", GetType(String)))
            dataTable.Columns.Add( _
              New DataColumn("Subtotal", GetType(String)))
            dataTable.Columns.Add( _
              New DataColumn("ProductId", GetType(String)))


            ' Make some rows and put some sample data in
            Dim enumerator As IEnumerator = cart.GetEnumerator()
            While (enumerator.MoveNext())

                'enumerator.Current returns a DictionaryEntry object, so use
                'its Value property to obtain the ShoppingItem
                Dim shoppingItem As ShoppingItem = _
                  CType(enumerator.Current.Value, ShoppingItem)
                Dim dr As DataRow = CreateNewDataRow(shoppingItem)
                dataTable.Rows.Add(dr)

            End While

            CartView = New DataView(dataTable)
            CartView.Sort = "Name"
            If Not IsPostBack Then
                BindGrid()
            End If
        End If
        UpdateTotal()
    End Sub 'Page_Load


    Sub UpdateTotal()
        totalLabel.Text = "Total: " & GetTotal().ToString("C")
    End Sub

    Sub MyDataGrid_Edit(ByVal sender As Object, _
    ByVal e As DataGridCommandEventArgs)
        MyDataGrid.EditItemIndex = e.Item.ItemIndex
        BindGrid()
    End Sub 'MyDataGrid_Edit


    Sub MyDataGrid_Cancel(ByVal sender As Object, _
    ByVal e As DataGridCommandEventArgs)
        MyDataGrid.EditItemIndex = -1
        BindGrid()
    End Sub 'MyDataGrid_Cancel

    Function CreateNewDataRow(ByVal ShoppingItem As ShoppingItem) As DataRow

        Dim dr As DataRow = dataTable.NewRow()
        dr(0) = shoppingItem.Name
        dr(1) = shoppingItem.Description
        dr(2) = shoppingItem.Quantity
        dr(3) = CDec(shoppingItem.Price).ToString("C")

        'Calculating subtotal
        Dim subtotal As Decimal
        Try
            subtotal = CInt(shoppingItem.quantity) * _
              CDec(shoppingItem.price)
        Catch ex As InvalidCastException
            ' do nothing, but subtotal will be 0
        End Try
        dr(4) = subtotal.ToString("C")
        dr(5) = shoppingItem.ProductId
        Return dr

    End Function

    Function GetTotal() As Decimal
        Dim total As Decimal

        If Not Session("cart") Is Nothing Then
            Dim cart As Hashtable = CType(Session("cart"), Hashtable)

            ' Make some rows and put some sample data in
            Dim enumerator As IEnumerator = cart.GetEnumerator()
            While (enumerator.MoveNext())

                'enumerator.Current returns a DictionaryEntry object, so use
                'its Value property to obtain the ShoppingItem
                Dim shoppingItem As ShoppingItem = _
                  CType(enumerator.Current.Value, ShoppingItem)
                total = total + CDec((CDbl(shoppingItem.Price) * CDbl(shoppingItem.Quantity)))

            End While
        End If

        Return total
    End Function


    Sub MyDataGrid_Update(ByVal sender As Object, _
    ByVal e As DataGridCommandEventArgs)
        ' For bound columns the edited value is stored in a textbox.
        ' The textbox is the 0th element in the column's cell.


        Dim qtyText As TextBox = CType(e.Item.Cells(4).Controls(0), _
          TextBox)
        Dim productId As String = e.Item.Cells(7).Text
        Dim quantity As String = qtyText.Text ' this is the new quantity
        Dim shoppingItem As ShoppingItem

        ' Update the shopping item in the Session object
        Dim cart As Hashtable = CType(Session("cart"), Hashtable)

        If Not cart Is Nothing Then

            shoppingItem = CType(cart.Item(productId), ShoppingItem)
            If Not shoppingItem Is Nothing Then
                shoppingItem.Quantity = quantity
            End If
        End If


        ' Now update the DataTable      
        ' We'll delete the old row and replace it with a new one.
        If Not shoppingItem Is Nothing Then
            ' Remove old entry.
            CartView.RowFilter = "ProductId='" & _
              shoppingItem.ProductId & "'"
            If CartView.Count > 0 Then
                CartView.Delete(0)
            End If
            CartView.RowFilter = ""

            ' Add new entry.
            Dim dr As DataRow = CreateNewDataRow(shoppingItem)
            dataTable.Rows.Add(dr)

        End If

        MyDataGrid.EditItemIndex = -1
        BindGrid()
        UpdateTotal()
    End Sub 'MyDataGrid_Update


    Sub MyDataGrid_Delete(ByVal sender As Object, _
    ByVal e As DataGridCommandEventArgs)

        Dim productId As String = e.Item.Cells(7).Text

        ' Update the shopping item in the Session object
        Dim cart As Hashtable = CType(Session("cart"), Hashtable)

        If Not cart Is Nothing Then
            cart.Remove(productId)
        End If


        ' Now delete the row from DataTable      
        CartView.RowFilter = "ProductId='" & productId & "'"
        If CartView.Count > 0 Then
            CartView.Delete(0)
        End If
        CartView.RowFilter = ""

        MyDataGrid.EditItemIndex = -1
        BindGrid()
        UpdateTotal()
    End Sub 'MyDataGrid_Delete


    Sub BindGrid()
        MyDataGrid.DataSource = CartView
        MyDataGrid.DataBind()
    End Sub 'BindGrid

End Class

End Namespace