Imports System.Xml
Dim xmlReader As XmlTextReader
Try
  xmlReader = New XmlTextReader("test.xml")
  While xmlReader.Read()
    If xmlReader.NodeType = XmlNodeType.Element Then
      System.Console.WriteLine(xmlReader.Name & " : ")
    End If
    If xmlReader.NodeType = XmlNodeType.Text Then
      System.Console.WriteLine(xmlReader.Value)
    End If
  End While
Catch ex As Exception
End Try
If Not xmlReader Is Nothing Then
  xmlReader.Close()
End If
