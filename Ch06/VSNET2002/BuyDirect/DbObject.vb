Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Text
Imports System.Collections
Imports Microsoft.VisualBasic


Public MustInherit Class DbObject

    Private Shared db As String = ConfigurationSettings.AppSettings("db")
    Protected connectionString As String = ConfigurationSettings.AppSettings("connectionString")

    Public Shared Function GetDbObject() As DbObject
        If db.ToUpper().Equals("ACCESS") Then
            Return (New AccessDbObject())
        ElseIf db.ToUpper().Equals("MSSQL") Then
            'Return (New MsSqlDbObject())
        End If
    End Function

    Public Shared Function FixFieldValue(ByVal value As String) As String
        If value Is Nothing Then
            Return Nothing
        Else
            Return value.Replace("'", "''")
        End If
    End Function

    Public Shared Function GetConnectionString() As String
        Return ConfigurationSettings.AppSettings("connectionString")
    End Function

    Protected Function GetOrderId() As String
        ' Generate a unique order id from the DateTime structure's Now method
        Return DateTime.Now.Ticks.ToString()
    End Function

    Public MustOverride Function GetCategories() As DataSet
    Public MustOverride Function GetSearchResult(ByVal searchKey As String) As DataSet
    Public MustOverride Function GetBrowseResult(ByVal categoryId As String) As DataSet
    ' pass a product id, return a ShoppingItem object populated with the product details
    Public MustOverride Function GetShoppingItem(ByVal productId As String) As ShoppingItem
    Public MustOverride Function ProcessPurchase( _
      ByVal contactName As String, _
      ByVal deliveryAddress As String, _
      ByVal ccName As String, _
      ByVal ccNumber As String, _
      ByVal ccExpiryDate As String, _
      ByVal cart As Hashtable) As Boolean

    Public MustOverride Function GetAllProducts() As DataSet
    Public MustOverride Function GetAllCategoriesAsString() As String
    Public MustOverride Sub InsertProduct( _
      ByVal categoryId As String, _
      ByVal newProductName As String, _
      ByVal newProductDescription As String, _
      ByVal newProductPrice As String)
    Public MustOverride Sub EditProduct( _
      ByVal productId As String, _
      ByVal productName As String, _
      ByVal productDescription As String, _
      ByVal productPrice As String)
    Public MustOverride Sub DeleteProduct(ByVal productId As String)

    Public MustOverride Function GetAllCategories() As DataSet
    Public MustOverride Sub InsertCategory(ByVal category As String)
    Public MustOverride Sub EditCategory( _
      ByVal categoryId As String, _
      ByVal category As String)
    Public MustOverride Sub DeleteCategory(ByVal categoryId As String)

End Class

Public Class AccessDbObject : Inherits DbObject

    Public Overrides Function GetCategories() As DataSet
        Dim connection As New OleDbConnection(connectionString)
        Dim sql As String = "SELECT CategoryId, Category From Categories"
        Dim command As New OleDbDataAdapter(sql, connection)
        Dim dataSet As New DataSet()
        command.Fill(dataSet, "Categories")
        connection.Close()
        Return dataSet
    End Function

    Public Overrides Function GetSearchResult(ByVal searchKey As String) As DataSet
        Dim con As New OleDbConnection(connectionString)

        searchKey = FixFieldValue(searchKey)

        Dim sql As New StringBuilder(512)
        sql.Append("SELECT '<img src=images/thumbnail/' & ProductId")
        sql.Append(" & '.gif width=40 height=40>' As [&nbsp;],")
        sql.Append(" Name, Price,")
        sql.Append(" '<a href=ProductDetails.aspx?productId=' " & _
          "& ProductId & '>Details</a>' As Details")
        sql.Append(" FROM Products")
        sql.Append(" WHERE Name LIKE '%" & searchKey & "%'")
        sql.Append(" OR Description LIKE '%" & searchKey & "%'")
        Dim cmd As New OleDbDataAdapter(sql.ToString(), con)
        con.Close()
        Dim ds As New DataSet()
        cmd.Fill(ds, "Products")
        Return ds

    End Function

    Public Overrides Function GetBrowseResult(ByVal categoryId As String) As DataSet
        Dim con As New OleDbConnection(connectionString)

        Dim sql As New StringBuilder(512)
        sql.Append("SELECT '<img src=images/thumbnail/' & ProductId")
        sql.Append(" & '.gif width=40 height=40>' As [&nbsp;],")
        sql.Append(" Name, Price,")
        sql.Append(" '<a href=ProductDetails.aspx?productId=' &" & _
          " ProductId & '>Details</a>' As Details")
        sql.Append(" FROM Products")
        sql.Append(" WHERE CategoryId=")
        sql.Append(categoryId)
        Dim cmd As New OleDbDataAdapter(sql.ToString(), con)
        Dim ds As New DataSet()
        cmd.Fill(ds, "Products")
        Return ds
    End Function

    Public Overrides Function GetShoppingItem(ByVal productId As String) As ShoppingItem
        Dim shoppingItem As New ShoppingItem()
        Dim con As New OleDbConnection(connectionString)

        Dim sql As String = "SELECT ProductId, Name, Description, Price" & _
          " FROM Products" & _
          " WHERE ProductId=" & productId
        Dim cmd As New OleDbCommand(sql, con)
        Dim dataReader As OleDbDataReader
        con.Open()
        dataReader = cmd.ExecuteReader()

        If dataReader.Read() Then
            shoppingItem.ProductId = dataReader.GetInt32(0).ToString()
            shoppingItem.Name = dataReader.GetString(1)
            ' somehow using GetString(2) throws an exception
            shoppingItem.Description = dataReader.GetValue(2).ToString()
            shoppingItem.Price = dataReader.GetDecimal(3).ToString()
            shoppingItem.Quantity = "1"
        End If
        con.Close()
        Return shoppingItem
    End Function

    Public Overrides Function ProcessPurchase( _
      ByVal contactName As String, _
      ByVal deliveryAddress As String, _
      ByVal ccName As String, _
      ByVal ccNumber As String, _
      ByVal ccExpiryDate As String, _
      ByVal cart As Hashtable) As Boolean

        Dim returnValue As Boolean
        Dim orderId As String = GetOrderId()

        Dim sql As New StringBuilder(512)
        sql.Append("INSERT INTO Orders")
        sql.Append(" (OrderId, ContactName, DeliveryAddress, CCName, CCNumber, CCExpiryDate)")
        sql.Append(" VALUES (")
        sql.Append(orderId)
        sql.Append(",")
        sql.Append("'").Append(FixFieldValue(contactName)).Append("',")
        sql.Append("'").Append(FixFieldValue(deliveryAddress)).Append("',")
        sql.Append("'").Append(FixFieldValue(ccName)).Append("',")
        sql.Append("'").Append(ccNumber).Append("',")
        sql.Append("'").Append(ccExpiryDate).Append("')")

        Dim con As New OleDbConnection(connectionString)
        con.Open()
        Dim transaction As OleDbTransaction = con.BeginTransaction()

        Try
            ' Create a Command object
            Dim command As New OleDbCommand(sql.ToString(), con)
            ' Execute the SQL statement
            command.Transaction = transaction
            Dim recordsAffected As Integer = command.ExecuteNonQuery()

            Dim enumerator As IEnumerator = cart.GetEnumerator()
            While (enumerator.MoveNext())

                'enumerator.Current returns a DictionaryEntry object, so use
                'its Value property to obtain the ShoppingItem
                Dim shoppingItem As ShoppingItem = _
                  CType(enumerator.Current.Value, ShoppingItem)
                Dim productId As String = shoppingItem.ProductId
                Dim price As String = shoppingItem.Price
                Dim quantity As String = CStr(shoppingItem.Quantity)
                sql = New StringBuilder(512)
                sql.Append("INSERT INTO OrderDetails (")
                sql.Append("OrderId, ProductId, Quantity, Price) VALUES (")
                sql.Append(orderId).Append(",")
                sql.Append(productId).Append(",")
                sql.Append(quantity).Append(",")
                sql.Append(price).Append(")")
                command.CommandText = sql.ToString()
                command.ExecuteNonQuery()

            End While

            transaction.Commit()
            returnValue = True
        Catch ex As Exception
            transaction.Rollback()
            returnValue = False
        Finally
            con.Close()
        End Try

        Return returnValue
    End Function

    Public Overrides Function GetAllProducts() As DataSet
        Dim con As New OleDbConnection(connectionString)
        Dim sql As String = _
          "SELECT ProductId, Name, Description, Price" & _
          " FROM Products"
        Dim cmd As New OleDbDataAdapter(sql, con)
        Dim ds As New DataSet()
        cmd.Fill(ds, "Products")
        con.Close()
        Return ds
    End Function

    Public Overrides Function GetAllCategoriesAsString() As String
        Dim sql As String = "SELECT CategoryId, Category FROM Categories"

        Dim connection As New OleDbConnection(connectionString)
        connection.Open()
        Dim command As New OleDbCommand(sql, connection)

        ' Instantiate a DataReader object 
        ' using the OleDbCommand class's ExecuteReader method
        Dim dataReader As OleDbDataReader = command.ExecuteReader()

        ' Loop through the DataReader 
        Dim s As New StringBuilder(1024)
        Do While dataReader.Read()
            s.Append("<option value=")
            s.Append(dataReader.GetInt32(0))
            s.Append(">").Append(dataReader.GetString(1))
            s.Append("</option>").Append(vbCr)
        Loop
        connection.Close()

        Return s.ToString()
    End Function

    Public Overrides Sub InsertProduct( _
      ByVal newCategoryId As String, _
      ByVal newProductName As String, _
      ByVal newProductDescription As String, _
      ByVal newProductPrice As String)

        Dim sql As String = "INSERT INTO Products (CategoryId, Name, Description, Price)" & _
          " VALUES (" & newCategoryId & "," & _
          " '" & newProductName.Trim() & "'," & _
          " '" & newProductDescription.Trim() & "'," & _
          newProductPrice.Trim() & ")"

        Dim connection As New OleDbConnection(connectionString)
        connection.Open()
        ' Create a Command object
        Dim command As New OleDbCommand(sql, connection)
        ' Execute the SQL statement
        Dim recordsAffected As Integer = _
          command.ExecuteNonQuery()
        connection.Close()
    End Sub

    Public Overrides Sub EditProduct( _
      ByVal productId As String, _
      ByVal productName As String, _
      ByVal productDescription As String, _
      ByVal productPrice As String)

        Dim sql As String = _
          "Update Products SET Name='" & FixFieldValue(productName) & "'," & _
          "Description='" & FixFieldValue(productDescription) & "'," & _
          "Price=" & productPrice & _
          " WHERE ProductId=" & productId

        Dim connection As New _
          OleDbConnection(connectionString)

        connection.Open()
        ' Create a Command object
        Dim command As New OleDbCommand(sql, connection)
        ' Execute the SQL statement
        Dim recordsAffected As Integer = command.ExecuteNonQuery()
        connection.Close()

    End Sub

    Public Overrides Sub DeleteProduct(ByVal productId As String)
        Dim sql As String = _
          "DELETE FROM Products WHERE ProductId=" & productId
        Dim connection As New _
          OleDbConnection(connectionString)
        connection.Open()
        ' Create a Command object
        Dim command As New OleDbCommand(sql, connection)
        ' Execute the SQL statement
        Dim recordsAffected As Integer = command.ExecuteNonQuery()
        connection.Close()
    End Sub

    Public Overrides Function GetAllCategories() As DataSet
        Dim con As New OleDbConnection(connectionString)
        Dim sql As String = "SELECT CategoryId, Category FROM Categories"
        Dim cmd As New OleDbDataAdapter(sql, con)
        Dim ds As New DataSet()
        cmd.Fill(ds, "Categories")
        con.Close()
        Return ds
    End Function

    Public Overrides Sub InsertCategory(ByVal category As String)
        Dim sql As String = "INSERT INTO Categories (Category)" & _
          " VALUES ('" & FixFieldValue(category) & "')"
        Dim connection As New OleDbConnection(connectionString)
        connection.Open()
        ' Create a Command object
        Dim command As New OleDbCommand(sql, connection)
        ' Execute the SQL statement
        Dim recordsAffected As Integer = command.ExecuteNonQuery()
        connection.Close()
    End Sub

    Public Overrides Sub EditCategory( _
      ByVal categoryId As String, _
      ByVal category As String)

        Dim sql As String = "Update Categories SET Category='" & FixFieldValue(category) & "'" & _
          " WHERE CategoryId=" & categoryId

        Dim connection As New OleDbConnection(connectionString)

        connection.Open()
        ' Create a Command object
        Dim command As New OleDbCommand(sql, connection)
        ' Execute the SQL statement
        Dim recordsAffected As Integer = command.ExecuteNonQuery()
        connection.Close()
    End Sub

    Public Overrides Sub DeleteCategory(ByVal categoryId As String)
        Dim sql As String = "DELETE FROM Categories WHERE CategoryId=" & categoryId
        Dim connection As New OleDbConnection(connectionString)
        connection.Open()
        ' Create a Command object
        Dim command As New OleDbCommand(sql, connection)
        ' Execute the SQL statement
        Dim recordsAffected As Integer = command.ExecuteNonQuery()
        connection.Close()
    End Sub
End Class

