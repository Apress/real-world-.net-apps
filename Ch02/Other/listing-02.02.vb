Imports System.Xml
Dim xmlReader As XmlTextReader
Dim xmlValReader As XmlValidatingReader
Dim output As New StringBuilder(1024)
Try
  xmlReader = New XmlTextReader("test.xml")
  xmlValReader = New XmlValidatingReader(xmlReader)
  While xmlValReader.Read()
  End While
  System.Console.WriteLine("document validated")
Catch ex As Exception
    System.Console.WriteLine(ex.ToString())
End Try
If Not xmlValReader Is Nothing Then
  xmlValReader.Close()
End If
