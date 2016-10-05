Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Printing
Imports System.IO


Public Class TextPrinter
    Private Shared prtDialog As New PrintDialog
    Private printDoc As New PrintDocument
    Private Shared pgSettings As New PageSettings
    Private Shared prtSettings As PrinterSettings
    Public Text As String
    Private textToPrint As StringReader
    Private lineCounter As Integer

    'indicates that this is the beginning of the printing process. 
    'Useful to tell printDoc_PrintPage to initialize values when
    'this boolean is True
    Private startPrinting As Boolean
    Private startPrintedLine As Integer
    Private endPrintedLine As Integer
    Private printFont As New Font("Courier New", 10)
    Private printedPageLineCount As Integer

    Public Sub New()
        AddHandler printDoc.PrintPage, _
          New PrintPageEventHandler(AddressOf printDoc_PrintPage)
        AddHandler printDoc.BeginPrint, _
          New PrintEventHandler(AddressOf printDoc_BeginPrint)
    End Sub

    'Event fired before printing
    Private Sub printDoc_BeginPrint(ByVal sender As Object, _
      ByVal e As PrintEventArgs)
        startPrinting = True
        textToPrint = New StringReader(Text)
    End Sub

    'Event fired for each page to print
    Private Sub printDoc_PrintPage(ByVal sender As Object, _
      ByVal e As PrintPageEventArgs)
        If startPrinting Then
            'calculate values. This If block will only execute once for
            'every printing process
            printedPageLineCount = CInt(Math.Floor(e.MarginBounds.Height / _
              printFont.GetHeight(e.Graphics)))
            If prtDialog.PrinterSettings.PrintRange = PrintRange.SomePages Then
                If prtDialog.PrinterSettings.ToPage < _
                  prtDialog.PrinterSettings.FromPage Then
                    e.Cancel = True
                    Return
                End If
                Dim fromPage As Integer = Math.Max(1, prtDialog.PrinterSettings.FromPage)
                Dim toPage As Integer = Math.Max(1, prtDialog.PrinterSettings.ToPage)
                startPrintedLine = (fromPage - 1) * printedPageLineCount + 1
                endPrintedLine = toPage * printedPageLineCount
                lineCounter = startPrintedLine
                If endPrintedLine < startPrintedLine Then
                    e.Cancel = True
                    Return
                End If
                Dim i As Integer
                For i = 1 To startPrintedLine - 1
                    textToPrint.ReadLine()
                Next
            End If

            startPrinting = False
        End If

        e.PageSettings.PrinterSettings = prtSettings

        Dim yPos As Single
        Dim leftMargin As Single = e.MarginBounds.Left
        Dim topMargin As Single = e.MarginBounds.Top
        Dim line As String = ""

        Dim counter As Integer = 1

        If prtDialog.PrinterSettings.PrintRange = PrintRange.SomePages Then

            While (counter <= printedPageLineCount And _
              lineCounter <= endPrintedLine And Not line Is Nothing)
                line = textToPrint.ReadLine()
                If Not line Is Nothing Then
                    yPos = topMargin + ((counter - 1) * printFont.GetHeight(e.Graphics))
                    e.Graphics.DrawString(line, printFont, Brushes.Black, _
                      leftMargin, yPos, New StringFormat)
                End If
                counter += 1
                lineCounter += 1
            End While
            If (lineCounter < endPrintedLine And Not line Is Nothing) Then
                e.HasMorePages = True
            Else
                e.HasMorePages = False
            End If
        ElseIf prtDialog.PrinterSettings.PrintRange = PrintRange.AllPages Then
            While (counter <= printedPageLineCount And Not line Is Nothing)
                line = textToPrint.ReadLine()
                If Not line Is Nothing Then
                    yPos = topMargin + ((counter - 1) * printFont.GetHeight(e.Graphics))
                    e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, _
                      yPos, New StringFormat)
                End If
                counter += 1
                lineCounter += 1
            End While
            If line Is Nothing Then
                e.HasMorePages = False
            Else
                e.HasMorePages = True
            End If
        End If

    End Sub

    Public Sub Print()
        Try
            'prtDialog.AllowSelection = False
            prtDialog.Document = printDoc
            'prtDialog.AllowSomePages = True
            'printDoc.DefaultPageSettings = pgSettings
            'prtDialog.PrinterSettings.FromPage = 1
            'prtDialog.PrinterSettings.ToPage = 1
            If (prtDialog.ShowDialog() = DialogResult.OK) Then
                printDoc.Print()
            End If
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

    Public Sub PrintPreview()
        Try
            Dim dlg As New PrintPreviewDialog
            dlg.Document = printDoc

            dlg.ShowDialog()
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

    Public Sub SetupPage()
        Try
            Dim pageSetupDialog As New PageSetupDialog
            pageSetupDialog.Document = printDoc
            pageSetupDialog.PageSettings = pgSettings
            pageSetupDialog.PrinterSettings = prtSettings
            pageSetupDialog.AllowOrientation = True
            pageSetupDialog.AllowMargins = True
            pageSetupDialog.ShowDialog()
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

End Class
