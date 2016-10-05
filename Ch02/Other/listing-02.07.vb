Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.IO

Public Class Form1 : Inherits Form

  Private printDoc As New PrintDocument()

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
  Private Sub filePrintMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
    printDoc.Print()
  End Sub

  Private Sub filePrintPreviewMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
  End Sub

  Private Sub filePageSetupMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
  End Sub

  Private Sub printDoc_PrintPage(ByVal sender As Object, ByVal e As PrintPageEventArgs)
    Dim textToPrint = ".NET Printing is easy"
    Dim printFont As New Font("Courier New", 12)
    e.Graphics.DrawString(textToPrint, printFont, Brushes.Black, 0, 0)
  End Sub
  '----------- end of event handlers ------------------------

  <STAThread()> Shared Sub Main()
    Application.Run(New Form1())
  End Sub

End Class
