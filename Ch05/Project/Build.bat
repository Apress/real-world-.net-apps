vbc /t:library /r:System.Windows.Forms.dll Helper.vb
vbc /t:library /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll LoginForm.vb
vbc /t:library /r:System.dll FTP.vb
vbc /t:winexe /r:System.dll,System.Windows.Forms.dll,System.Drawing.dll,Helper.dll,LoginForm.dll,FTP.dll Form1.vb
