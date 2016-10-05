Imports System
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml

Public Class XmlViewer : Inherits TreeView

    Public RootImageIndex As Integer
    Public RootSelectedImageIndex As Integer
    Public LeafImageIndex As Integer
    Public LeafSelectedImageIndex As Integer
    Public BranchImageIndex As Integer
    Public BranchSelectedImageIndex As Integer

    Public Sub BuildTree(ByVal xmlText As String)
        Try
            Clear()
            Dim xmlDocument As New XmlDocument
            xmlDocument.Load(New StringReader(xmlText))

            Dim root As New TreeNode(xmlDocument.DocumentElement.Name, _
              RootImageIndex, rootselectedimageindex)
            DrawNode(xmlDocument.DocumentElement, root, 0)
            Me.Nodes.Add(root)
            Me.ExpandAll()
        Catch e As Exception
        End Try

    End Sub

    Public Sub Clear()
        Me.Nodes.Clear()
    End Sub

    Private Sub DrawNode(ByVal node As XmlNode, ByVal treeNode As TreeNode, _
      ByVal level As Integer)
        'Draw only if node.NodeType is Element or Text
        If level = 1 Then
            If node.NodeType = XmlNodeType.Element Then
                Dim newTreeNode As TreeNode = New TreeNode(node.Name, _
                  BranchImageIndex, BranchSelectedImageIndex)
                treeNode.Nodes.Add(newTreeNode)
                treeNode = newTreeNode
            ElseIf node.NodeType = XmlNodeType.Text Then
                Dim newTreeNode As TreeNode = New TreeNode(node.Value.Trim(), _
                  LeafImageIndex, LeafSelectedImageIndex)
                treeNode.Nodes.Add(newTreeNode)
                treeNode = newTreeNode
            End If
        End If
        If (node.HasChildNodes) Then
            node = node.FirstChild
            While Not node Is Nothing
                If node.NodeType = XmlNodeType.Element Or _
                  node.NodeType = XmlNodeType.Text Then
                    DrawNode(node, treeNode, 1)
                End If
                node = node.NextSibling
            End While
        End If
    End Sub

End Class