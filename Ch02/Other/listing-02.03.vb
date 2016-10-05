Imports System.Xml
Dim xmlWriter As XmlTextWriter
Try
  xmlWriter = New XmlTextWriter("test2.xml", Nothing)
  xmlWriter.WriteStartDocument()
  xmlWriter.Formatting = Formatting.Indented
  xmlWriter.Indentation = 4
  xmlWriter.WriteStartElement("books")
  xmlWriter.WriteStartElement("book")
  xmlWriter.WriteAttributeString("id", "1")
  xmlWriter.WriteElementString("author", "Holzner, Charles")
  xmlWriter.WriteElementString("title", _
    "English-Japanese Dictionary")
  xmlWriter.WriteElementString("price", "59.95")
  xmlWriter.WriteEndElement()
  xmlWriter.WriteEndElement()
  xmlWriter.WriteEndDocument()
Catch ex As Exception
Finally
  xmlWriter.Close()
End Try
