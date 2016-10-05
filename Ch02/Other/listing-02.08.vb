  Private Sub filePrintMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
    Dim dlg As New PrintDialog()
    dlg.Document = printDoc
    If (dlg.ShowDialog = DialogResult.OK) Then
      printDoc.Print()
    End If
  End Sub
