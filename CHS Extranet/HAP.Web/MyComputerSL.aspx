<%@ Page Title="Crickhowell High School - Home Access Plus+ - My Computer Silverlight (BETA)" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="MyComputerSL.aspx.cs" Inherits="HAP.Web.MyComputerSL" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link href="mycomputer.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
		function onSilverlightError(sender, args) {
			var appSource = "";
			if (sender != null && sender != 0) {
				appSource = sender.getHost().Source;
			}

			var errorType = args.ErrorType;
			var iErrorCode = args.ErrorCode;

			if (errorType == "ImageError" || errorType == "MediaError") {
				return;
			}

			var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

			errMsg += "Code: " + iErrorCode + "    \n";
			errMsg += "Category: " + errorType + "       \n";
			errMsg += "Message: " + args.ErrorMessage + "     \n";

			if (errorType == "ParserError") {
				errMsg += "File: " + args.xamlFile + "     \n";
				errMsg += "Line: " + args.lineNumber + "     \n";
				errMsg += "Position: " + args.charPosition + "     \n";
			}
			else if (errorType == "RuntimeError") {
				if (args.lineNumber != 0) {
					errMsg += "Line: " + args.lineNumber + "     \n";
					errMsg += "Position: " + args.charPosition + "     \n";
				}
				errMsg += "MethodName: " + args.methodName + "     \n";
			}
			alert(errMsg);
			throw new Error(errMsg);
		}
		function OnSourceDownloadProgressChanged(sender, args) {
			sender.findName("ProgressText").Text = "Loading... " + Math.round((args.progress * 1000)) / 10 + "%";
		}
	</script>
	<style type="text/css">
		#silverlightControlHost { height: 640px; border: solid 1px #A0AFC3; }
		.max #navigation { display: none; }
		.max #silverlightControlHost { position: fixed; top: 0; left: 0; bottom: 0; border: 0; right: 0; padding: 0; margin: 0; height: auto; }
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div id="silverlightControlHost">
		<object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		  <param name="source" value="clientbin/HAP.Silverlight.Browser.xap"/>
		  <param name="splashscreensource" value="MyComputerSLSplash.xaml"/>
		  <param name="onSourceDownloadProgressChanged" value="OnSourceDownloadProgressChanged" />
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="white" />
		  <param name="minRuntimeVersion" value="4.0.50401.0" />
		  <param name="autoUpgrade" value="true" />
		  <a href="http://www.microsoft.com/getsilverlight/" target="_blank" style="text-decoration: none;"><img src="images/Silverlight-Prompt.png" alt="Get Microsoft Silverlight" /></a>
		</object>
		<iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>
	</div>
</asp:Content>
