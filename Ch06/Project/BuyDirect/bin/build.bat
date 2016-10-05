vbc /target:library ShoppingItem.vb
vbc /target:library /r:System.dll,System.Data.Dll,System.Xml.dll,ShoppingItem.dll DbObject.vb
