Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.IO

Public Class Form1 : Inherits Form

  Private printDoc As New PrintDocument()
  Private pgSettings As New PageSettings()

  Public Sub New()
    Me.Menu = New MainMenu()
    Dim fileMenuItem As New MenuItem("&File")
    Dim filePageSetupMenuItem As New MenuItem("Page Set&up...", _
      New EventHandler(AddressOf filePageSetupMenuItem_Click))
    Dim filePrintPreviewMenuItem As New MenuItem("Print Pre&view", _
      New EventHandler(AddressOf filePrintPreviewMenuItem_Click))
    Dim filePrintMenuItem As New MenuItem("&Print...", _
      New EventHandler(AddressOf filePrintMenuItem_Click), Shortcut.CtrlP)

    fileMenuItem.MenuItems.Add(filePageSetupMenuItem)
    fileMenuItem.MenuItems.Add(filePrintPreviewMenuItem)
    fileMenuItem.MenuItems.Add(filePrintMenuItem)

    Me.Menu.MenuItems.Add(fileMenuItem)

    AddHandler printDoc.PrintPage, AddressOf printDoc_PrintPage
  End Sub

  '----------- event handlers ------------------------------
  Private Sub filePrintMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    printDoc.DefaultPageSettings = pgSettings

    Dim dlg As New PrintDialog()
    dlg.Document = printDoc
    If (dlg.ShowDialog = DialogResult.OK) Then
      printDoc.Print()
    End If
  End Sub

  Private Sub filePrintPreviewMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
  End Sub

  Private Sub filePageSetupMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    Dim pageSetupDialog As New PageSetupDialog()
    pageSetupDialog.PageSettings = pgSettings
    pageSetupDialog.AllowOrientation = True
    pageSetupDialog.AllowMargins = True
    pageSetupDialog.ShowDialog()
  End Sub

  Private Sub printDoc_PrintPage(ByVal sender As Object, _
    ByVal e As PrintPageEventArgs)
    Dim textToPrint = ".NET Printing is easy"
    Dim printFont As New Font("Courier New", 12)
    Dim leftMargin As Integer = e.MarginBounds.Left
    Dim topMargin As Integer = e.MarginBounds.Top
    e.Graphics.DrawString(textToPrint, printFont, Brushes.Black, _
      leftMargin, topMargin)
  End Sub
  '----------- end of event handlers ------------------------

  <STAThread()> Shared Sub Main()
    Application.Run(New Form1())
  End Sub

End Class
