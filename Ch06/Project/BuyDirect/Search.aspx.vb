Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data

Namespace BuyDirect

Public Class Search
    Inherits System.Web.UI.Page

    Public searchKey As String
    Public message As Label
    Public SearchResultGrid As DataGrid

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
            searchKey = Request.Params("searchKey")
            If Not searchKey Is Nothing Then
                If searchKey.Trim().Equals("") Then
                    message.Text = "Please enter a search key."
                    SearchResultGrid.DataSource = Nothing
                Else
                    ViewState.Add("searchKey", searchKey)
                    BindGrid()
                End If
            Else
                ' request did not come from the Search form
                Server.Transfer("Default.aspx")
            End If
        End If
    End Sub

    Sub SearchResultGrid_PageIndexChanged(ByVal sender As Object, _
    ByVal e As DataGridPageChangedEventArgs)
        searchKey = ViewState("searchKey").ToString()
        SearchResultGrid.CurrentPageIndex = e.NewPageIndex
        BindGrid()
    End Sub

    Sub BindGrid()
        Dim dbo As DbObject = DbObject.GetDbObject()
        Dim ds As DataSet = dbo.GetSearchResult(searchKey)
        SearchResultGrid.DataSource = ds.Tables("Products").DefaultView
        SearchResultGrid.DataBind()
    End Sub

    
End Class

End Namespace