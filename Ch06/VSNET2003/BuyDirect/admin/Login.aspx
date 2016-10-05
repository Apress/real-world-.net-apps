<html>
<head>
<title>Login Form</title>
<script language="VB" runat="server">
Sub Login(sender As Object, e As EventArgs)
  If FormsAuthentication.Authenticate( _
    userName.Text, password.Text) Then
    
    FormsAuthentication.RedirectFromLoginPage( _
      userName.Text, False)
  Else
    message.Text = "Login failed. Please try again."
  End If
End Sub 

</script>
</head>
<body>
<form runat="server">
<asp:Label runat="server"
  id="message"
/>

<hr>

<h2>BuyDirect Administrator Login Form</h2>
<table>
<tr>
  <td>Username:</td>
  <td><asp:TextBox runat="server" id="userName"/></td>
</tr>
<tr>
  <td>Password:</td>
  <td>
    <asp:TextBox runat="server" 
      TextMode="password" id="password"
    />
  </td>
</tr>
<tr>
  <td align="right" colspan="2">
    <asp:Button runat="server"
      Text="Login"
      OnClick="Login"
    />
  </td>
</tr>
</table>  
</form>
</body>
</html>