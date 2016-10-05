vbc /t:library /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,StyledTextArea.dll FindDialog.vb
vbc /t:library /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.Xml.dll XmlViewer.vb
vbc /t:library /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.Xml.dll TextPrinter.vb
vbc /t:library /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,mscorlib.dll,System.Xml.dll,StyledTextArea.dll,XmlViewer.dll,TextPrinter.dll,FindDialog.dll Document.vb
vbc /t:winexe /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,mscorlib.dll,System.Xml.dll,StyledTextArea.dll,XmlViewer.dll,TextPrinter.dll,FindDialog.dll,Document.dll form1.vb
