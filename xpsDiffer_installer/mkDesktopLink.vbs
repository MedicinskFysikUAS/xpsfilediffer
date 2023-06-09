Set oWS = WScript.CreateObject("WScript.Shell")
Set args = Wscript.Arguments 
linkName = args.Item(0) 
targetPath = args.Item(1) 
iconPath = args.Item(2) 
workingDir = args.Item(3) 
userProfilePath = args.Item(4) 
sLinkFile = ""&userProfilePath&"\Desktop\"&linkName&".lnk" 
Set oLink = oWS.CreateShortcut(sLinkFile) 
oLink.TargetPath = ""&targetPath&"" 
oLink.IconLocation = ""&iconPath&"" 
oLink.HotKey = "ALT+CTRL+K"   
oLink.WorkingDirectory = ""&workingDir&""  
oLink.Save 
