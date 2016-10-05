Imports System.Xml
Dim xmlDocument As New XmlDocument()
Try
  xmlDocument.Load("test.xml")
  DrawNode(xmlDocument.DocumentElement, 1)
Catch ex As Exception
End Try
.
.
.
Sub DrawNode(ByVal node As XmlNode, ByVal level As Integer)
  'Draw only if node.NodeType is Element or Text
  'Draw the line
  System.Console.Write(New String("-"c, level * 2))
  'If node.NodeType = Text, we don't want to display
  'the node.Name
  If node.NodeType = XmlNodeType.Element Then
    System.Console.Write(node.Name)
  End If
  System.Console.WriteLine(node.Value)
  If (node.HasChildNodes) Then
    node = node.FirstChild
    While Not IsNothing(node)
      If node.NodeType = XmlNodeType.Element Or _
        node.NodeType = XmlNodeType.Text Then
        DrawNode(node, level + 1)
      End If
      node = node.NextSibling
    End While
  End If
End Sub
