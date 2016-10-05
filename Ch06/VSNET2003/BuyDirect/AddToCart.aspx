<%@ Page Language="VB" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="BuyDirect" %>

<script language="VB" runat="server">

Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

  Dim productId As String = Request.Params("productId") 
  If productId Is Nothing Then
    Server.Transfer("Default.aspx")
  Else

    Dim dbo As DbObject = DbObject.GetDbObject()
    Dim shoppingItem As ShoppingItem = dbo.GetShoppingItem(productId)

    Dim cart As Hashtable = CType(Session("cart"), Hashtable)

    If cart Is Nothing Then
      cart = new Hashtable()
    End If

    ' Before adding an item, check if the key (productId) exists
    Dim obj As Object = cart.Item(productId)
    If Not obj Is Nothing Then
      cart.Remove(productId)
    End If
    cart.Add(productId, shoppingItem)
    Session.Add("cart", cart)
    Server.Transfer("ShoppingCart.aspx")
  End If 
End Sub 

</script>
