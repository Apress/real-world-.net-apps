Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections

Public Class CheckOut
    Inherits System.Web.UI.Page

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
    End Sub


    Public header1, header2 As Label
    Public myTable As Table
    Public contactName, deliveryAddress, ccName, ccNumber, ccExpiryDate As TextBox
    Public contactNameValidator As RequiredFieldValidator
    Public deliveryAddressValidator As RequiredFieldValidator
    Public ccNameValidator As RequiredFieldValidator
    Public ccNumberValidator As RequiredFieldValidator
    Public ccExpiryDateValidator1 As RequiredFieldValidator
    Public ccExpiryDateValidator2 As RegularExpressionValidator

    Sub ProcessPurchase(ByVal sender As Object, ByVal e As EventArgs)
        'Check input
        If contactNameValidator.IsValid() AndAlso _
          deliveryAddressValidator.IsValid() AndAlso _
          ccNameValidator.IsValid() AndAlso _
          ccNumberValidator.IsValid() AndAlso _
          ccExpiryDateValidator1.IsValid() AndAlso _
          ccExpiryDateValidator2.IsValid() Then

            myTable.Visible = False
            ' process purchase
            header2.Text = ""
            If ProcessPurchase() Then
                ' Empty shopping cart
                Session("cart") = Nothing
                header1.Text = "Transaction completed successfully."
            Else
                header1.Text = "Transaction failed."
            End If
        End If
    End Sub

    Public Function ProcessPurchase() As Boolean
        Dim dbo As DbObject = DbObject.GetDbObject()
        Return dbo.ProcessPurchase(contactName.Text, _
          deliveryAddress.Text, _
          ccName.Text, _
          ccNumber.Text, _
          ccExpiryDate.Text, _
          CType(Session("cart"), Hashtable))

    End Function

End Class
