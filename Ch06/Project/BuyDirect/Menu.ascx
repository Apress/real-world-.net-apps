<%@ Control Language="vb" AutoEventWireup="false" Src="Menu.ascx.vb" Inherits="BuyDirect.Menu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ OutputCache Duration="86400" VaryByParam="None" %>

<!-- the main table containing two other tables: Search table and Browse table -->
<table border="0" cellpadding="0" cellspacing="0"
  width="<%=ConfigurationSettings.AppSettings("menuWidth")%>">
<tr>
<td>

<!-- the Search table -->
<table cellspacing="0" cellpadding="1"
  width="100%"
  border="0" class="OuterTable"
>
<tr>
  <td>
    <table cellspacing="0" cellpadding="5" 
      width="100%" border="0" 
      class="InnerTable">
    <tr>
      <td class="MenuHeader">Search</td>
    </tr>
    <tr valign="middle">
      <td rowspan="2">
        <form method="post" action="Search.aspx">
        <input type="text" name="searchKey" size="13">
        <input type="submit" value="Go">
        </form>
      </td>
    </tr>
    </table>
  </td>
</tr>
</table>

</td>
</tr>

<!-- space between the Search table and Browse table -->
<tr>
<td height="7"></td>
</tr>

<!-- the Browse table -->
<tr>
<td>
<table cellspacing="0" cellpadding="1"
  width="100%" border="0" 
  class="OuterTable">
<tr>
  <td>

    <table cellspacing="0" cellpadding="5" width="100%" border="0" class="InnerTable">
    <tr>
      <td class="MenuHeader">Browse</td>
    </tr>
    <tr valign="top">
      <td>
        <asp:DataList id="categoryList" runat="server" 
          cellpadding="3" cellspacing="0" width="145" 
          SelectedItemStyle-BackColor="dimgray" 
          EnableViewState="false">
        <ItemTemplate>
          <asp:HyperLink runat="server" 
            Font-Name="Verdana" 
            CssClass="MenuItem"
            Text='<%# DataBinder.Eval(Container.DataItem, "Category") %>' 
            ID = "Hyperlink1"
            NavigateUrl='<%# "Browse.aspx?CategoryId=" & _
            DataBinder.Eval(Container.DataItem, "CategoryID") %>'
          />
        </ItemTemplate>
        </asp:DataList>
      </td>
    </tr>
    </table>
  </td>
</tr>
</table>

</td>
</tr>
</table>