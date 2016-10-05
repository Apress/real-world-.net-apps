Imports System
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

<Serializable()> Class SerializableClass
  Private value As Integer
  Public Function GetValue() As Integer
    GetValue = value
  End Function
  
  Public Sub SetValue(ByVal value As Integer)
    Me.value = value
  End Sub
End Class

Public Module modMain
Public Sub Serialize()
  Dim myObject As SerializableClass = _
    New SerializableClass()
  myObject.SetValue(888)

  Dim stream As Stream = File.Create("C:\MyObject.bin")
  Dim formatter As IFormatter
  formatter = CType(New BinaryFormatter(), IFormatter)
  formatter.Serialize(stream, myObject)
  stream.Close()

End Sub

Public Sub Deserialize()
  Dim stream As Stream = File.OpenRead("C:\MyObject.bin")
  Dim formatter As IFormatter
  formatter = CType(New BinaryFormatter(), IFormatter)
  Dim myObject As SerializableClass
  myObject = CType(formatter.Deserialize(stream), SerializableClass)

  stream.Close()
  Console.WriteLine(myObject.GetValue)
End Sub

Public Sub Main()
   Serialize
   Deserialize
End Sub

End Module
