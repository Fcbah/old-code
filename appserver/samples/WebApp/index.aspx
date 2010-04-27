<%@ Page language="C#" %>
<%@ Import namespace="System.IO" %>
<html>
	<head>
		<title>Welcome to Mono XSP!</title>
	</head>
	<body>
		<h1>Welcome to Mono XSP!</h1>
		<a href="http://www.go-mono.com"><img src="mono.png" alt="http://www.go-mono.com"></a>
		<p>Here are some ASP.NET examples:</p>
		<%
DirectoryInfo dir = new DirectoryInfo (Server.MapPath("."));
Response.Write(dir.FullName);
FileInfo[] files = dir.GetFiles ();
for (int i=0; i < files.Length; i++) {
	string FileName = Path.GetFileName(files[i].FullName);
	if (files[i].Extension == ".aspx" || files[i].Extension == ".asmx")
	FileList.Text += "<li><a href=\"" + FileName + "\">" + FileName + "</a></li>\n";
}
%>
		<ul>
			<asp:Label id="FileList" runat="server" />
		</ul>
		<hr />
		<small>Generated:
			<%= DateTime.Now %>
		</small>
</html>
