Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data

Namespace BuyDirect
Public Class Browse
    Inherits System.Web.UI.Page

    Public categoryId As String
    Public message As Label
    Public CatalogGrid As DataGrid

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
        If Not IsPostBack Then
            categoryId = Request.Params("CategoryId")
            If Not categoryId Is Nothing Then
                If categoryId.Trim().Equals("") Then
                    message.Text = "Please select a category from the menu."
                    CatalogGrid.DataSource = Nothing
                Else
                    ViewState.Add("categoryId", categoryId)
                    BindGrid()
                End If
            Else
                ' invalid request
                Server.Transfer("Default.aspx")
            End If
        End If
    End Sub
    Sub CatalogGrid_PageIndexChanged(ByVal sender As Object, _
    ByVal e As DataGridPageChangedEventArgs)
        categoryId = ViewState("categoryId").ToString()
        CatalogGrid.CurrentPageIndex = e.NewPageIndex
        BindGrid()
    End Sub

    Sub BindGrid()
        Dim dbo As DbObject = DbObject.GetDbObject()
        Dim ds As DataSet = dbo.GetBrowseResult(categoryId)
        CatalogGrid.DataSource = ds.Tables("Products").DefaultView
        CatalogGrid.DataBind()
    End Sub

End Class
End Namespace