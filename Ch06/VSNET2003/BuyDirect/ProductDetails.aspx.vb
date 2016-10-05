Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Configuration
Public Class ProductDetails
    Inherits System.Web.UI.Page
    Public productId As String
    Public productImage As Image
    Public name As Label
    Public description As Label
    Public price As Label
    Public addToCart As HyperLink

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
        productId = Request.Params("productId")
        If productId Is Nothing Then
            Response.Redirect("Default.aspx")
        Else
            DisplayDetails()
            addToCart.NavigateUrl = "AddToCart.aspx?ProductID=" & productId
        End If
    End Sub

    Sub DisplayDetails()
        Dim dbo As DbObject = DbObject.GetDbObject()
        Dim item As ShoppingItem = dbo.GetShoppingItem(productId)
        productImage.ImageUrl = "images/" & productId & ".gif"
        name.Text = item.name
        description.Text = item.description
        price.Text = item.price
    End Sub

End Class
