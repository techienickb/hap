<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="HAP.Web.Setup" %>
<%@ Register Assembly="HAP.Web" Namespace="HAP.Web.Controls" TagPrefix="hap" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Home Access Plus+ - Setup</title>
		<link rel="shortcut icon" href="~/favicon.ico" />
		<hap:jQuery runat="server" />
		<script src="Scripts/jquery.dynatree.js" type="text/javascript"></script>
		<script src="Scripts/jquery-Gtimepicker.js" type="text/javascript"></script>
		<link href="~/style/jquery-ui.css" rel="stylesheet" type="text/css" />
		<link href="~/style/basestyle.css" rel="stylesheet" type="text/css" />
		<link href="style/setup.css" rel="stylesheet" type="text/css" />
		<link href="style/ui.dynatree.css" rel="stylesheet" type="text/css" />
		<link rel="icon" type="image/vnd.microsoft.icon" href="~/favicon.ico" />
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
		<hap:HAPTag runat="server" />
	</head>
	<body>
		<form runat="server">
			<div id="hapContent">
				<asp:ToolkitScriptManager runat="server" EnablePartialRendering="true" />
				<asp:LoginView runat="server">
					<LoggedInTemplate>
						<div id="usertop">Logged in as <asp:LoginName runat="server" /> | <asp:LoginStatus runat="server" /></div>
					</LoggedInTemplate>
				</asp:LoginView>
				<div id="baseContent">
					<script type="text/javascript">
						function toggleGroup(e) {
							e.className = e.className == "" ? "collapsed" : "";
							var nb = e.parentNode.parentNode.parentNode.parentNode.nextSibling;
							if (nb.tagName != "div") nb = nb.nextSibling;
							runEffect(nb);
						}
						function runEffect(e) {
							$(e).toggle( "blind", { to: { height: 0} }, 100);
						};
						function OnUpdateError(xhr, ajaxOptions, thrownError) {
							alert(thrownError);
						}
						var root = "<%=ResolveUrl("~/") %>";
						var reqtype = "group"
						var obj = null;
						var obj2 = null;
					</script>
					<div id="adbrowserwrapper" title="Active Directory Browser"><div id="treeprogress"></div><div id="treecontainer"><ul id="tree"></ul></div></div>
					<% if (Request.QueryString.Count == 1 && Request.QueryString["saved"] == "1")
					   { %>
					   <p class="ui-state-highlight ui-corner-all" style=" padding: 5px 10px"><span class="ui-icon ui-icon-circle-check" style="float: left; margin-right: 5px;"></span>Config Saved! <span style="display: block; text-align: center;"><button style="font-size: 20px;" onclick="location.href='./'; return false;">Start Using HAP+</button></span></p> 
					   <div class="ui-state-error ui-corner-all" style=" padding: 5px 10px"><span class="ui-icon ui-icon-alert" style="float: left; margin-right: 5px;"></span>If you have not already done so, please edit ~/web.config with notepad, etc..., changing:<br />
							&lt;location path="setup.aspx"&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&lt;system.web&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;authorization&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;allow users="*" /&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/authorization&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&lt;/system.web&gt;<br />
							&lt;/location&gt;<br />
						to<br />
							&lt;location path="setup.aspx"&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&lt;system.web&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;authorization&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;allow roles="Domain Admins" /&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;deny users="*" /&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/authorization&gt;<br />
							&nbsp;&nbsp;&nbsp;&nbsp;&lt;/system.web&gt;<br />
							&lt;/location&gt;<br />
						</code>
					   </div> 
					   <p class="ui-state-highlight ui-corner-all" style=" padding: 5px 10px"><span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>You may also need to edit the other web.config files to change the permissions on those folders <br />(i.e. allow staff to get onto the help desk/booking system)</p>
					<% }
					   else
					   { %>
					<p class="ui-state-highlight ui-corner-all" style=" padding: 5px 10px"><span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>If you do not want to use a feature, you don't need to complete the section</p>
					<%} %>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>General <asp:Image ID="generalstate" runat="server" ImageUrl="~/images/setup/266.png" /></b></span></span></h2>
					<div class="group">
						<div>
							<asp:Label ID="Label1" runat="server" Text="School Name: " AssociatedControlID="name" />
							<asp:TextBox runat="server" ID="name" Text="" onkeypress="generalchange();" onchange="generalchange();" />
						</div>
						<div>
							<asp:Label ID="Label2" runat="server" Text="School URL: " AssociatedControlID="schoolurl" />
							<asp:TextBox runat="server" ID="schoolurl" Text="" onkeypress="generalchange();" onchange="generalchange();" />
						</div>
					</div>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>Active Directory Connection <asp:Image ID="adstate" runat="server" ImageUrl="~/images/setup/266.png" /></b></span></span></h2>
					<div class="group">
						<div class="error"><asp:Literal runat="server" ID="adresponse" /></div>
						<div>
							<asp:Label runat="server" Text="Domain UPN: " AssociatedControlID="upn" />
							<asp:TextBox runat="server" ID="upn" Text="" onchange="checkad();" />
						</div>
						<div>
							<asp:Label runat="server" Text="User with Delegate Access (Admin): " AssociatedControlID="un" />
							<asp:TextBox runat="server" ID="un" Text="" onchange="checkad();" />
						</div>
						<div>
							<asp:Label runat="server" Text="User Password: " AssociatedControlID="up" />
							<asp:TextBox runat="server" ID="up" Text="" TextMode="Password" onchange="checkad();" />
						</div>

					</div>
					<asp:UpdatePanel runat="server">
						<ContentTemplate>
							<h2><span><span><b><i onclick="toggleGroup(this)"></i>Active Directory Base Settings <asp:Image ID="adbasestate" runat="server" ImageUrl="~/images/setup/266.png" /></b></span></span></h2>
							<div class="group">
								<div>
									<asp:Label runat="server" Text="Students Group: " AssociatedControlID="sg" />
									<asp:TextBox runat="server" ID="sg" Text="" AutoPostBack="true" ontextchanged="sg_TextChanged" onclick="showadbrowser(this, 'group');" />
								</div>
								<div>
									<h3>AD Organizational Units</h3>
									<p>For user selection, each OU unit will be processed in the Booking System/Help Desk Admin User Drop Down Selectors</p>
									<asp:Repeater runat="server" ID="adorgs" onitemcommand="adorgs_ItemCommand">
										<ItemTemplate>
											<div>
												Name: <%#Eval("Name") %><br />
												Ignore: <%#Eval("Ignore") %> <asp:Button runat="server" Text="Delete" CommandName="Delete" CommandArgument='<%#Eval("Name") %>' />
												<pre><%#Eval("Path") %></pre>
											</div>
										</ItemTemplate>
									</asp:Repeater>
									<h4>New</h4>
									<div>
										<asp:Label runat="server" Text="Name: " AssociatedControlID="ouname" />
										<asp:TextBox ID="ouname" runat="server" />
									</div>
									<div>
										<asp:CheckBox runat="server" ID="ouignore" Text="Ignore: " TextAlign="Left" />
									</div>
									<div>
										<asp:Label runat="server" Text="OU Path: " AssociatedControlID="oupath" />
										<asp:TextBox runat="server" ID="oupath" onclick="showadbrowser(this, 'organizationalUnit');" />
									</div>
									<asp:Button runat="server" ID="newou" Text="Add" onclick="newou_Click" style="width: 37px; margin-left: 220px;" />
								</div>
							</div>
						</ContentTemplate>
					</asp:UpdatePanel>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>Web Proxy Server <asp:Image ID="proxystate" runat="server" ImageUrl="~/images/setup/267.png" /></b></span></span></h2>
					<div class="group">
						<div>
							<asp:CheckBox Text="Enabled: " id="proxyenabled" TextAlign="Left" runat="server" onclick="checkweb();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Address: " AssociatedControlID="proxyaddress" />
							<asp:TextBox runat="server" ID="proxyaddress" Text="" onkeypress="checkweb();" onchange="checkweb();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Port: " AssociatedControlID="proxyport" />
							<asp:TextBox runat="server" ID="proxyport" Text="" onkeypress="checkweb();" onchange="checkweb();" />
						</div>
					</div>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>SMTP Server <asp:Image ID="smtpstate" runat="server" ImageUrl="~/images/setup/267.png" /></b></span></span></h2>
					<div class="group">
						<div>
							<asp:CheckBox Text="Enabled: " id="smtpenabled" TextAlign="Left" runat="server" onclick="checksmtp();" onchange="checksmtp();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Address: " AssociatedControlID="smtpaddress" />
							<asp:TextBox runat="server" ID="smtpaddress" Text="" onkeypress="checksmtp();" onchange="checksmtp();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Port: " AssociatedControlID="smtpport" />
							<asp:TextBox runat="server" ID="smtpport" Text="25" onkeypress="checksmtp();" onchange="checksmtp();" />
						</div>
						<div>
							<asp:CheckBox Text="SSL: " id="smtpssl" TextAlign="Left" runat="server" />
						</div>
						<div>
							<asp:Label runat="server" Text="SMTP User: " AssociatedControlID="smtpuser" />
							<asp:TextBox runat="server" ID="smtpuser" Text="" />
						</div>
						<div>
							<asp:Label runat="server" Text="SMTP Password: " AssociatedControlID="smtppassword" />
							<asp:TextBox runat="server" ID="smtppassword" TextMode="Password" Text="" />
						</div>
						<div>
							<asp:Label runat="server" Text="From (Name): " AssociatedControlID="smtpfromname" />
							<asp:TextBox runat="server" ID="smtpfromname" Text="" onkeypress="checksmtp();" onchange="checksmtp();" />
						</div>
						<div>
							<asp:Label runat="server" Text="From (Email): " AssociatedControlID="smtpfromemail" />
							<asp:TextBox runat="server" ID="smtpfromemail" Text="" onkeypress="checksmtp();" onchange="checksmtp();" />
						</div>
					</div>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>Logon Tracker <asp:Image ID="trackerstate" runat="server" ImageUrl="~/images/setup/266.png" /></b></span></span></h2>
					<div class="group">
						<div>
							<asp:Label runat="server" Text="Provider (Default XML): " AssociatedControlID="trackerprovider" />
							<asp:TextBox runat="server" ID="trackerprovider" Text="XML" onkeypress="checktracker();" onchange="checktracker();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Override Code: " AssociatedControlID="trackercode" />
							<asp:TextBox runat="server" ID="trackercode" Columns="5" onkeypress="checktracker();" onchange="checktracker();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Max Student Logons: " AssociatedControlID="trackerstudentlogs" />
							<asp:TextBox runat="server" ID="trackerstudentlogs" Text="1" Columns="2" onkeypress="checktracker();" onchange="checktracker();" />
						</div>
						<div>
							<asp:Label runat="server" Text="Max Staff Logons: " AssociatedControlID="trackerstafflogs" />
							<asp:TextBox runat="server" ID="trackerstafflogs" Text="4" Columns="2" onkeypress="checktracker();" onchange="checktracker();" />
						</div>
					</div>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>Home Page</b></span></span></h2>
					<div class="group">

						<div>
							<h3>Announcement Box</h3>
							<div>
								<asp:Label runat="server" AssociatedControlID="hp_ab_st" Text="Show To: " /><asp:TextBox runat="server" ID="hp_ab_st" onclick="showadbuilder(this, false);" />
							</div>
							<div>
								<asp:Label runat="server" AssociatedControlID="hp_ab_adt" Text="Allow Edit To: " /><asp:TextBox runat="server" ID="hp_ab_adt" onclick="showadbuilder(this, false);" />
							</div>
						</div>
						<div>
							<h3>Tabs</h3>
							<div id="homepagetabs">
							<ul>
								<asp:Repeater ID="homepageTabs" runat="server"><ItemTemplate>
								<li class="ui-state-default" id="id_<%# Eval("Type") %>"><a href="#homepagetabs-<%#Eval("Type") %>"><%# Eval("Type")%></a></li>
								</ItemTemplate></asp:Repeater>
							</ul>
							<asp:Repeater ID="homepageTabs2" runat="server"><ItemTemplate>
								<div id="homepagetabs-<%#Eval("Type") %>">
									<div style="overflow: hidden; clear: both;">
									<label for="homepagetabs-<%#Eval("Type") %>-Name">Name: </label>
									<input type="text" id="homepagetabs-<%#Eval("Type") %>-Name" value="<%#Eval("Name") %>" onchange="updatetab('<%#Eval("Type") %>');" />
									</div>
									<div style="overflow: hidden; clear: both;">
									<label for="homepagetabs-<%#Eval("Type") %>-ShowTo">Show To: </label>
									<input type="text" id="homepagetabs-<%#Eval("Type") %>-ShowTo" value="<%#Eval("ShowTo") %>" onclick="showadbuilder(this, false);"  onchange="updatetab('<%#Eval("Type") %>');" />
									</div>
									<%#((HAP.Web.Configuration.TabType)Eval("Type")) == HAP.Web.Configuration.TabType.Me ? "<div style=\"overflow: hidden; clear: both;\"><label for=\"homepagetabs-" + Eval("Type").ToString() + "-allowupdateto\">Allow Update To: </label><input type=\"text\" id=\"homepagetabs-" + Eval("Type").ToString() + "-allowupdateto\" value=\"" + Eval("AllowUpdateTo").ToString() + "\" onclick=\"showadbuilder(this, false);\" onchange=\"updatetab('" + Eval("Type").ToString() + "');\" /></div>" : "" %>
									<%#((HAP.Web.Configuration.TabType)Eval("Type")) == HAP.Web.Configuration.TabType.Me ? "<div style=\"overflow: hidden; clear: both;\"><label for=\"homepagetabs-" + Eval("Type").ToString() + "-showspace\">Show Space: </label><input type=\"checkbox\" id=\"homepagetabs-" + Eval("Type").ToString() + "-showspace\"" + (((Nullable<bool>)Eval("ShowSpace")).Value ? " checked=\"checked\"" : "") + " onchange=\"updatetab('" + Eval("Type").ToString() + "');\" /></div>" : ""%>
								</div>
							</ItemTemplate></asp:Repeater>
							</div>
						</div>
						<div>
							<h3>Links <button class="addbutton" onclick="return addgroup();">Add Group</button></h3>
							<div class="sortablegroup">
							<asp:Repeater runat="server" ID="homepageLinkGroups">
								<ItemTemplate>
									<div id="linkgroup<%#Eval("Name").ToString().Replace(' ', '_') %>" class="linkgroup"><h4><span class="lgName"><%#Eval("Name") %></span> (<span class="lgST"><%#Eval("ShowTo") %></span>) <div class="cbuttonset" style="display: inline;"><button class="edit" onclick="return editgroup('<%#Eval("Name").ToString().Replace(' ', '_') %>');">Edit</button><button class="minusbutton" onclick="return removegroup('linkgroup<%#Eval("Name").ToString().Replace(' ', '_') %>');">Delete</button><button class="addbutton" onclick="return addlink('linkgroup<%#Eval("Name").ToString().Replace(' ', '_') %>');">Add Link</button></div></h4>
									<p><%#Eval("SubTitle") %></p>
									<div class="sortable">
									<asp:Repeater ID="Repeater1" runat="server" DataSource='<%#Container.DataItem %>'>
										<ItemTemplate>
											<div class="homepagelink" id="link<%#Eval("Name").ToString().Replace(' ', '_') %>">
											<button title="Remove" onclick="return removelink(this);">X</button>
											<a href="#link" title="Edit" onclick="return editlink(this);">
												<img src="<%#ResolveUrl(Eval("Icon").ToString()) %>" alt="" />
												<b><%#Eval("Name") %></b>
												<i><%#Eval("Description") %></i>
												<span><%#Eval("Target") %></span>
												<u><%#Eval("Url") %></u>
												<dd><%#Eval("ShowTo") %></dd>
											</a>
											</div>
										</ItemTemplate>
									</asp:Repeater>
									</div>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							</div>
						</div>
					</div>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>Booking System</b></span></span></h2>
					<div class="group">
						<div>
							<asp:Label runat="server" Text="Max Bookings Per Week: " AssociatedControlID="bsmax" />
							<asp:TextBox runat="server" ID="bsmax" Text="1" Columns="2" />
						</div>
						<div>
							<asp:Label runat="server" Text="Max Days Ahead: " AssociatedControlID="bsdays" />
							<asp:TextBox runat="server" ID="bsdays" Text="4" Columns="2" />
						</div>
						<div>
							<asp:Label runat="server" Text="Two Week Timetable: " AssociatedControlID="bstwoweek" />
							<asp:CheckBox runat="server" ID="bstwoweek" ToolTip="Two Week Timetable?" />
						</div>
						<div>
							<asp:Label runat="server" Text="Keep XML Clear: " AssociatedControlID="bsclean" />
							<asp:CheckBox runat="server" ID="bsclean" ToolTip="Keep the XML Database Clear of Old Bookings?" />
						</div>
						<div>
							<asp:Label runat="server" Text="Admin Groups: " AssociatedControlID="bsadmins" />
							<asp:TextBox runat="server" ID="bsadmins" Text="" onclick="showadbuilder(this, false, false);" /> (domain admins are already allocated)
						</div>
						<div>
							<h3>Lessons <button onclick="return addlesson();" class="addbutton">Add</button></h3>
							<div id="lessons">
								<asp:Repeater runat="server" ID="bslessons">
									<ItemTemplate>
										<div>
											<div class="lesson" style="float: left;">
												<span><%#Eval("Name") %></span>
												(<i><%#Eval("Type") %></i>)<br />
												<b class="starttime"><%#((DateTime)Eval("StartTime")).ToString("hh:mm tt") %></b> - 
												<b class="endtime"><%#((DateTime)Eval("EndTime")).ToString("hh:mm tt")%></b>
											</div>
											<div class="cbuttonset"><button onclick="return editlesson(this);" title="Edit">Edit</button><button onclick="return removelesson(this);" title="Delete">Delete</button></div>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
						<div>
							<h3>Resources <button onclick="return addres();" class="addbutton">Add</button></h3>
							<div id="resources">
								<asp:Repeater runat="server" ID="bsresources">
									<ItemTemplate>
										<div>
											<div class="resource" style="float: left;">
												<span><%#Eval("Name") %></span>
												(<i><%#Eval("Type") %></i>) [<b class="enabled"><%#((bool)Eval("Enabled")) ? "Enabled" : "Disabled" %></b>|<b class="charging"><%#((bool)Eval("EnableCharging")) ? "Charging" : "N"%></b>|<b class="email"><%#((bool)Eval("EmailAdmins")) ? "Email Admins" : "N" %></b>]<br />
												Admins: <b class="admins"><%#Eval("Admins") %></b>
											</div>
											<div class="cbuttonset"><button onclick="return editres(this);" title="Edit">Edit</button><button onclick="return removeres(this);" title="Delete">Delete</button></div>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
						<div>
							<h3>Subjects <button onclick="return addsubject();" class="addbutton">Add</button></h3>
							<div id="subjects">
								<asp:Repeater runat="server" ID="bssubjects">
									<ItemTemplate>
										<div style="overflow: hidden;">
											<span style="float: left;"><%#Container.DataItem%></span>
											<div class="cbuttonset"><button onclick="return editsubject(this);" title="Edit">Edit</button><button onclick="return removesubject(this);" title="Delete">Delete</button></div>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
					</div>
					<h2><span><span><b><i onclick="toggleGroup(this)"></i>My School Computer Browser</b></span></span></h2>
					<div class="group">
						<div>
							<asp:Label runat="server" Text="Hidden Extensions: " AssociatedControlID="mscbExt" />
							<asp:TextBox runat="server" ID="mscbExt" />
						</div>
						<div>
							<h3>Mappings <button onclick="return addmapping();" class="addbutton">Add</button></h3>
							<div id="mappings">
								<asp:Repeater runat="server" ID="mscbMappings">
									<ItemTemplate>
										<div class="homepagelink">
											<button title="Remove" onclick="return removemapping(this);">X</button>
											<a href="#mapping" class="mapping" title="Edit" onclick="return editmapping(this);">
												<img src="<%#ResolveUrl("~/images/icons/netdrive.png") %>" alt="" />
												<b><%#Eval("Drive") %></b><i><%#Eval("UNC") %></i>
												<span class="name"><%#Eval("Name") %></span>
												<span class="ert"><%#Eval("EnableReadTo") %></span>
												<span class="ewt"><%#Eval("EnableWriteTo") %></span>
												<span class="em"><%#Eval("EnableMove") %></span>
												<dd><%#Eval("UsageMode") %></dd>
											</a>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
						<div>
							<h3>Filters <button onclick="return addfilter();" class="addbutton">Add</button></h3>
							<div id="filters">
								<asp:Repeater runat="server" ID="mscbFilters">
									<ItemTemplate>
										<div class="homepagelink">
											<button title="Remove" onclick="return removefilter(this);">X</button>
											<a href="#filter" class="filter" title="Edit" onclick="return editfilter(this);">
												<b><%#Eval("Name") %></b>
												<i><%#Eval("Expression") %></i>
												<span><%#Eval("EnableFor") %></span>
											</a>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
						<div>
							<h3>Quota Servers <button onclick="return addqserver();" class="addbutton">Add</button></h3>
							<div id="qservers">
								<asp:Repeater runat="server" ID="mscbQuotaServers">
									<ItemTemplate>
										<div class="homepagelink">
											<button title="Remove" onclick="return removeqserver(this);">X</button>
											<a href="#qserver" class="qserver" title="Edit" onclick="return editqserver(this);">
												<i><%#Eval("Expression") %></i>
												<b><%#Eval("Server") %></b>
												<span><%#Eval("Drive") %></span>
											</a>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
					</div>
					<div id="mappingEditor" title="Mapping Editor">
						<p class="ui-state-highlight ui-corner-all" style=" padding: 5px 10px"><span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>%homedir% = User's Home Directory, %username% = username</p>
						<div>
							<label for="mappingDrive" style="width: 100px;">Drive: </label><input type="text" id="mappingDrive" maxlength="1" style="width: 20px" />
						</div>
						<div>
							<label for="mappingName" style="width: 100px;">Name: </label><input type="text" id="mappingName" />
						</div>
						<div>
							<label for="mappingUNC" style="width: 100px;">UNC: </label><input type="text" id="mappingUNC" />
						</div>
						<div>
							<label for="mappingEnableMove" style="width: 100px;">Enable Move: </label><input type="checkbox" id="mappingEnableMove" />
						</div>
						<div>
							<label for="mappingEnableReadTo" style="width: 100px;">Enable Read To: </label><input type="text" id="mappingEnableReadTo" onclick="showadbuilder(this, false);" />
						</div>
						<div>
							<label for="mappingEnableWriteTo" style="width: 100px;">Enable Write To: </label><input type="text" id="mappingEnableWriteTo" onclick="showadbuilder(this, false);" />
						</div>
						<div>
							<label for="mappingEnableReadTo" style="width: 100px;">Usage Mode: </label>
							<div id="mappingUsageMode">
								<input type="radio" id="mappingUsageModeDriveSpace" name="mappingUsageMode" checked="checked" value="Drive Space" /><label for="mappingUsageModeDriveSpace" style="width: auto;">Drive Space</label>
								<input type="radio" id="mappingUsageModeQuotaServer" name="mappingUsageMode" value="Quota Server" /><label for="mappingUsageModeQuotaServer" style="width: auto;">Quota Server</label>
							</div>
						</div>
					</div>
					<div id="filterEditor" title="Filter Editor">
						<div>
							<label for="filterName" style="width: 100px;">Name: </label><input type="text" id="filterName" />
						</div>
						<div>
							<label for="filterExpression" style="width: 100px;">Expression: </label><input type="text" id="filterExpression" />
						</div>
						<div>
							<label for="filterEnableFor" style="width: 100px;">Enable For: </label><input type="text" id="filterEnableFor" onclick="showadbuilder(this, false);" />
						</div>
					</div>
					<div id="linkgroupEditor" title="Link Group Editor">
						<div>
							<label for="groupName" style="width: 100px;">Name: </label><input type="text" id="groupName" />
						</div>
						<div>
							<label for="groupShowTo" style="width: 100px;">Show To: </label><input type="text" id="groupShowTo" onclick="showadbuilder(this, false);" />
						</div>
						<div>
							<label for="groupSubTitle" style="width: 100px;">SubTitle: </label><input type="text" id="groupSubTitle" />
						</div>
					</div>
					<div id="linkEditor" title="Link Editor">
						<div>
							<label for="linkName" style="width: 100px;">Name: </label><input type="text" id="linkName" />
						</div>
						<div>
							<label for="linkDesc" style="width: 100px;">Description: </label><input type="text" id="linkDesc" />
						</div>
						<div>
							<label for="linkUrl" style="width: 100px;">Url: </label><input type="text" id="linkUrl" />
						</div>
						<div>
							<label for="linkTarget" style="width: 100px;">Target: </label><input type="text" id="linkTarget" />
						</div>
						<div>
							<label for="linkIcon" style="width: 100px;">Icon: </label><input type="text" id="linkIcon" /> (~/ is the HAP+ root folder)
						</div>
						<div>
							<label for="linkShowTo" style="width: 100px;">Show To: </label><input type="text" id="linkShowTo" onclick="showadbuilder(this, true);" />
						</div>
					</div>
					<div id="addsub" title="Add/Edit Subject">
						<div>
							<input type="text" id="subject" />
						</div>
					</div>
					<div id="addlesson" title="Add/Edit Lesson">
						<div>
							<label for="lessonName" style="width: 100px;">Name: </label><input type="text" id="lessonName" />
						</div>
						<div>
							<label for="lessonType" style="width: 100px;">Type: </label><select id="lessonType"><option>Lesson</option><option>Break</option><option>Lunch</option><option>Before School</option><option>After School</option></select>
						</div>
						<div>
							<label for="lessonStart" style="width: 100px;">Start Time: </label><input type="text" id="lessonStart" />
						</div>
						<div>
							<label for="lessonEnd" style="width: 100px;">End Time: </label><input type="text" id="lessonEnd" />
						</div>
					</div>
					<div id="addres" title="Add/Edit a Resource">
						<div>
							<label for="resName">Name: </label><input type="text" id="resName" />
						</div>
						<div>
							<label for="resType">Type: </label><select id="resType"><option>Room</option><option>Laptops</option><option>Equipment</option><option>Other</option></select>
						</div>
						<div>
							<label for="resEnabled">Enabled: </label><input type="checkbox" id="resEnabled" />
						</div>
						<div>
							<label for="resCharging">Charging Enabled: </label><input type="checkbox" id="resCharging" />
						</div>
						<div>
							<label for="resAdmins">Additional Admins: </label><input type="text" id="resAdmins" /><br />(Comma Seperated Usernames)
						</div>
						<div>
							<label for="resEmail">Email Admins: </label><input type="checkbox" id="resEmail" />
						</div>
					</div>
					<div id="qserverEditor" title="Quota Server Editor">
						<p class="ui-state-highlight ui-corner-all" style=" padding: 5px 10px"><span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>%username% = username</p>
						<div>
							<label for="qserver" style="width: 100px;">Server: </label><input type="text" id="qserver" />
						</div>
						<div>
							<label for="qdrive" style="width: 100px;">Drive: </label><input type="text" id="qdrive" maxlength="1" style="width: 20px;" />
						</div>
						<div>
							<label for="qexpression" style="width: 100px;">Expression: </label><input type="text" id="qexpression" />
						</div>
					</div>
					<asp:Button runat="server" UseSubmitBehavior="true" Text="Save" Font-Size="XX-Large" CssClass="checkbutton" ID="Save" onclick="Save_Click" />
					<div id="adgroups" title="Group Builder">
						<div id="adgroups-mode">
							<input type="radio" id="adgroups-mode-all" name="adgroups-m" checked="checked" value="All" /><label for="adgroups-mode-all">All</label>
							<input type="radio" id="adgroups-mode-inherit" name="adgroups-m" value="Inherit" /><label for="adgroups-mode-inherit" id="adgroups-mode-inherit-label">Inerhit</label>
							<input type="radio" id="adgroups-mode-custom" name="adgroups-m" value="Custom" /><label for="adgroups-mode-custom">Custom</label>
						</div>
						<div id="adgroup-custom" style="display: none;">
							<input type="text" id="customgroup" /><button onclick="showadbrowser(document.getElementById('customgroup'), 'group'); return false;" class="searchbutton" title="Search for an AD Object">...</button>
							<div style="display: inline;" class="cbuttonset">
							<button class="addbutton" onclick="add();">Add</button><button class="minusbutton" onclick="$('#adgroups-custom option:selected').remove(); return false;">Remove</button>
							</div>
							<br />
							<select size="4" id="adgroups-custom" style="width: 100%;" />
						</div>
					</div>
					<script type="text/javascript">
						var tempe = null;
						function addmapping() {
							$("#mappingDrive").val("");
							$("#mappingUNC").val("");
							$("#mappingName").val("");
							$("#mappingEnableMove").removeAttr("checked");
							$("#mappingEnableReadTo").val("");
							$("#mappingEnableWriteTo").val("");
							$("#mappingUsageModeDriveSpace").attr("checked", "checked");
							$("#mappingUsageModeQuotaServer").removeAttr("checked");
							$("#mappingEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Add": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddMapping',
											data: '{ "drive": "' + $("#mappingDrive").val() + '", "name": "' + $("#mappingName").val() + '", "unc": "' + $("#mappingUNC").val().replace(/\\/g, "/") + '", "enablemove": ' + ($("#mappingEnableMove").attr("checked") ? "true" : "false") + ', "enablereadto": "' + $("#mappingEnableReadTo").val() + '", "enablewriteto": "' + $("#mappingEnableWriteTo").val() + '", "usagemode": "' + ($("mappingUsageModeDriveSpace").attr("checked") ? "DriveSpace" : "Quota") + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnMappingAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnMappingAddSuccess(response) {
							if (response != null && response.GetAddMappingResult != null) {
								var data = response.AddMappingResult;
								if (data != 0) alert(data);
								else {
									$("#mappings").append('<div class="homepagelink"><button title="Remove" onclick="return removemapping(this);">X</button><a href="#mapping" class="mapping" title="Edit" onclick="return editmapping(this);"><img src="<%#ResolveUrl("~/images/icons/netdrive.png") %>" alt="" /><b>' + $("#mappingDrive").val() + '</b><i>' + $("#mappingUNC").val() + '</i><span class="name">' + $("#mappingName").val() + '</span><span class="ert">' + $("#mappingEnableReadTo").val() + '</span><span class="ewt">' + $("#mappingEnableWriteTo").val() + '</span><span class="em">' + ($("mappingEnableMove").attr("checked") ? "true" : "false") + '</span><dd>' + ($("mappingUsageModeDriveSpace").attr("checked") ? "DriveSpace" : "Quota") + '</dd></a></div>');
									resetButtons();
								}
							}
						}
						function editmapping(e) {
							tempe = $(e).parent();
							$("#mappingDrive").val(tempe.children("a").children("b").html());
							$("#mappingName").val(tempe.children("a").children(".name").html());
							$("#mappingUNC").val(tempe.children("a").children("i").html());
							if (tempe.children("a").children(".em").html() == "True")
								$("#mappingEnableMove").attr("checked", "checked");
							else $("#mappingEnableMove").removeAttr("checked");
							$("#mappingEnableReadTo").val($.trim(tempe.children("a").children(".ert").html()));
							$("#mappingEnableWriteTo").val($.trim(tempe.children("a").children(".ewt").html()));
							if (tempe.children("a").children("dd").html() == "DriveSpace") {
								$("#mappingUsageModeDriveSpace").attr("checked", "checked");
								$("#mappingUsageModeQuotaServer").removeAttr("checked");
							} else {
								$("#mappingUsageModeQuotaServer").attr("checked", "checked");
								$("#mappingUsageModeDriveSpace").removeAttr("checked");
							}
							$("#mappingEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Save": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateMapping',
											data: '{ "origdrive": "' + tempe.children("a").children("b").html() + '", "drive": "' + $("#mappingDrive").val() + '", "name": "' + $("#mappingName").val() + '", "unc": "' + $("#mappingUNC").val().replace(/\\/g, "/") + '", "enablemove": ' + ($("#mappingEnableMove").attr("checked") ? "true" : "false") + ', "enablereadto": "' + $("#mappingEnableReadTo").val() + '", "enablewriteto": "' + $("#mappingEnableWriteTo").val() + '", "usagemode": "' + ($("mappingUsageModeDriveSpace").attr("checked") ? "DriveSpace" : "Quota") + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnMappingUpdateSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnMappingUpdateSuccess(response) {
							if (response != null && response.GetUpdateMappingResult != null) {
								var data = response.UpdateMappingResult;
								if (data != 0) alert(data);
								else {
									tempe.children("a").children("b").html($("#mappingDrive").val());
									tempe.children("a").children("i").html($("#mappingUNC").val());
									tempe.children("a").children(".name").html($("#mappingName").val());
									if ($("#mappingEnableMove").attr("checked")) tempe.children("a").children(".em").html() == "True";
									else tempe.children("a").children(".em").html() == "False"
									tempe.children("a").children(".ert").html($("#mappingEnableReadTo").val());
									tempe.children("a").children(".ewt").html($("#mappingEnableWriteTo").val());
									if ($("#mappingUsageModeDriveSpace").attr("checked")) tempe.children("a").children("dd").html("DriveSpace");
									else tempe.children("a").children("dd").html("Quota");
									tempe = null;
								}
							}
						}
						function removemapping(e) {
							tempe = $(e).parent();
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveMapping',
								data: '{ "drive": "' + tempe.children("a").children("b").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnMappingRemoveSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnFilterRemoveSuccess(response) {
							if (response != null && response.GetRemoveMappingResult != null) {
								var data = response.RemoveMappingResult;
								if (data != 0) alert(data);
								else { tempe.remove(); tempe = null; }
							}
						}
						function editfilter(e) {
							tempe = $(e).parent();
							$("#filterName").val(tempe.children("a").children("b").html());
							$("#filterExpression").val(tempe.children("a").children("i").html());
							$("#filterEnableFor").val(tempe.children("a").children("span").html());
							$("#filterEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Save": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateFilter',
											data: '{ "origname": "' + tempe.children("a").children("b").html() + '", "origexpression": "' + tempe.children("a").children("i").html() + '", "name": "' + $("#filterName").val() + '", "expression": "' + $("#filterExpression").val() + '", "enablefor": "' + $("#filterEnableFor").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnFilterUpdateSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnFilterUpdateSuccess(response) {
							if (response != null && response.GetUpdateFilterResult != null) {
								var data = response.UpdateFilterResult;
								if (data != 0) alert(data);
								else {
									tempe.children("a").children("b").html($("#filterName").val());
									tempe.children("a").children("i").html($("#filterExpression").val());
									tempe.children("a").children("span").html($("#filterEnableFor").val());
									tempe = null;
								}
							}
						}
						function removefilter(e) {
							tempe = $(e).parent();
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveFilter',
								data: '{ "name": "' + tempe.children("a").children("b").html() + '", "expression": "' + tempe.children("a").children("i").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnFilterRemoveSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnFilterRemoveSuccess(response) {
							if (response != null && response.RemoveFilterResult != null) {
								var data = response.RemoveFilterResult;
								if (data != 0) alert(data);
								else { tempe.remove(); tempe = null; }
							}
						}
						function addfilter() {
							$("#filterName").val("");
							$("#filterExpression").val("");
							$("#filterEnableFor").val("");
							$("#filterEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Add": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddFilter',
											data: '{ "name": "' + $("#filterName").val() + '", "expression": "' + $("#filterExpression").val() + '", "enablefor": "' + $("#filterEnableFor").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnFilterAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnFilterAddSuccess(response) {
							if (response != null && response.AddFilterResult != null) {
								var data = response.AddFilterResult;
								if (data != 0) alert(data);
								else {
									$("#filters").append('<div class="homepagelink"><button title="Remove" onclick="return removefilter(this);">X</button><a href="#filter" class="filter" title="Edit" onclick="return editfilter(this);"><i>' + $("#filterName").val() + '</i><b>' + $("#filterExpression").val() + '</b><span>' + $("#filterEnableFor").val() + '</a></div>');
									resetButtons();
								}
							}
						}
						function editqserver(e) {
							tempe = $(e).parent();
							$("#qserver").val(tempe.children("a").children("b").html());
							$("#qexpression").val(tempe.children("a").children("i").html());
							$("#qdrive").val(tempe.children("a").children("span").html());
							$("#qserverEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Save": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateQServer',
											data: '{ "origserver": "' + tempe.children("a").children("b").html() + '", "origexpression": "' + tempe.children("a").children("i").html().replace(/\\/g, "/") + '", "server": "' + $("#qserver").val() + '", "expression": "' + $("#qexpression").val().replace(/\\/g, "/") + ', "drive": "' + $("#qdrive").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnQServerUpdateSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnQServerUpdateSuccess(response) {
							if (response != null && response.UpdateQServerResult != null) {
								var data = response.UpdateQServerResult;
								if (data != 0) alert(data);
								else {
									tempe.children("a").children("b").html($("#qserver").val());
									tempe.children("a").children("i").html($("#qexpression").val());
									tempe.children("a").children("span").html($("#qdrive").val());
									tempe = nul;
								}
							}
						}
						function removeqserver(e) {
							tempe = $(e).parent();
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveQServer',
								data: '{ "server": "' + tempe.children("a").children("b").html() + '", "expression": "' + tempe.children("a").children("i").html().replace(/\\/g, "/") + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnQServerRemoveSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnQServerRemoveSuccess(response) {
							if (response != null && response.RemoveQServerResult != null) {
								var data = response.RemoveQServerResult;
								if (data != 0) alert(data);
								else { tempe.remove(); tempe = null; }
							}
						}
						function addqserver() {
							$("#qserver").val("");
							$("#qexpression").val("");
							$("#qserverEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Add": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddQServer',
											data: '{ "server": "' + $("#qserver").val() + '", "expression": "' + $("#qexpression").val().replace(/\\/g, "/") + '", "drive": "' + $("#qdrive").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnQServerAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnQServerAddSuccess(response) {
							if (response != null && response.AddQServerResult != null) {
								var data = response.AddQServerResult;
								if (data != 0) alert(data);
								else {
									$("#qservers").append('<div class="homepagelink"><button title="Remove" onclick="return removeqserver(this);">X</button><a href="#qserver" class="qserver" title="Edit" onclick="return editqserver(this);"><i>' + $("#qexpression").val() + '</i><b>' + $("#qserver").val() + '</b><span>' + $("#qdrive").val() + '</span></a></div>');
									resetButtons();
								}
							}
						}
						function addres() {
							$("#resName").val("");
							$("#resType option:first-child").attr("selected", "selected");
							$("#resEnabled").attr("checked", "checked");
							$("#resCharging").removeAttr("checked");
							$("#resAdmins").val("");
							$("#resEmail").attr("checked", "checked");
							$("#addres").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Add": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddResource',
											data: '{ "name": "' + $("#resName").val() + '", "type": "' + $("#resType").val() + '", "enabled": ' + ($("#resEnabled").attr("checked") ? 'true' : 'false') + ', "charging": ' + ($("#resCharging").attr("checked") ? 'true' : 'false') + ', "admins": "' + $("#resAdmins").val() + '", "emailadmins": ' + ($("#resEmail").attr("checked") ? 'true' : 'false') + ' }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnResourceAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnResourceAddSuccess(response) {
							if (response != null && response.AddResourceResult != null) {
								var data = response.AddResourceResult;
								if (data != 0) alert(data);
								else {
									$("#resources").append('<div><div class="resource" style="float: left;"><span>' + $("#resName").val() + '</span> (<i>' + $("#resType").val() + '</i>) [<b class="enabled">' + ($("#resEnabled").attr("checked") ? "Enabled" : "Disabled") + '</b>|<b class="charging">' + ($("#resCharging").attr("checked") ? "Charging" : "N") + '</b>|<b class="email">' + ($("#resEmail").attr("checked") ? "Email Admins" : "N") + '</b>]<br />Admins: <b class="admins">' + $("#resAdmins").val() + '</b></div><div class="cbuttonset"><button onclick="return editres(this);" title="Edit">Edit</button><button onclick="return removeres(this);" title="Delete">Delete</button></div></div>');
									resetButtons();
								}
							}
						}
						function editres(b) {
							tempe = $(b).parent().parent();
							$("#resName").val(tempe.children(".resource").children("span").html());
							$("#resType option:first-child").attr("selected", "selected");
							if (tempe.children(".resource").children(".enabled").html() == "Enabled") $("#resEnabled").attr("checked", "checked");
							else $("#resEnabled").removeAttr("checked");
							if (tempe.children(".resource").children(".charging").html() == "N") $("#resCharging").removeAttr("checked");
							else $("#resCharging").attr("checked", "checked");
							if (tempe.children(".resource").children(".email").html() == "N") $("#resEmail").removeAttr("checked");
							else $("#resEmail").attr("checked", "checked");
							$("#resAdmins").val(tempe.children(".resource").children(".admins").html());
							$("#addres").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Save": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateResource',
											data: '{ "origname": "' + tempe.children(".resource").children("span").html() + '", "name": "' + $("#resName").val() + '", "type": "' + $("#resType").val() + '", "enabled": ' + ($("#resEnabled").attr("checked") == "checked" ? 'true' : 'false') + ', "charging": ' + ($("#resCharging").attr("checked") ? 'true' : 'false') + ', "admins": "' + $("#resAdmins").val() + '", "emailadmins": ' + ($("#resEmail").attr("checked") ? 'true' : 'false') + ' }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnResourceUpdateSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnResourceUpdateSuccess(response) {
							if (response != null && response.UpdateResourceResult != null) {
								var data = response.UpdateResourceResult;
								if (data != 0) alert(data);
								else {
									$(tempe).children(".resource").replaceWith('<div class="resource" style="float: left;"><span>' + $("#resName").val() + '</span> (<i>' + $("#resType").val() + '</i>) [<b class="enabled">' + ($("#resEnabled").attr("checked") ? "Enabled" : "Disabled") + '</b>|<b class="charging">' + ($("#resCharging").attr("checked") ? "Charging" : "N") + '</b>|<b class="email">' + ($("#resEmail").attr("checked") ? "Email Admins" : "N") + '</b>]<br />Admins: <b class="admins">' + $("#resAdmins").val() + '</b></div>');
									tempe = null;
								}
							}
						}
						function removeres(b) {
							tempe = $(b).parent().parent();
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveResource',
								data: '{ name: "' + tempe.children(".resource").children("span").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnResourceRemoveSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnResourceRemoveSuccess(response) {
							if (response != null && response.RemoveResourceResult != null) {
								var data = response.RemoveResourceResult;
								if (data != 0) alert(data);
								else {
									tempe.remove();
									tempe = null;
								}
							}
						}
						function addlesson() {
							$("#lessonName").val("");
							$("#lessonType option:first-child").attr("selected", "selected");
							$("#lessonStart").val("00:00 AM");
							$("#lessonEnd").val("00:00 AM");
							$("#addlesson").dialog({
								autoOpen: true,
								width: 400,
								buttons: {
									"Add": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddLesson',
											data: '{ "name": "' + $("#lessonName").val() + '", "type": "' + $("#lessonType").val() + '", "start": "' + $("#lessonStart").val() + '", "end": "' + $("#lessonEnd").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnLessonAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnLessonAddSuccess(response) {
							if (response != null && response.AddLessonResult != null) {
								var data = response.AddLessonResult;
								if (data != 0) alert(data);
								else {
									$("#lessons").append('<div><div class="lesson"><span>' + $("#lessonName").val() + '</span> (<i>' + $("#lessonType").val() + '</i>)<br /><b class="starttime">' + $("#lessonStart").val() + '</b> - <b class="endtime">' + $("#lessonEnd").val() + '</b></div><div class="cbuttonset"><button onclick="return editlesson(this);" title="Edit">Edit</button><button onclick="return removelesson(this);" title="Delete">Delete</button></div></div>');
									resetButtons();
								}
							}
						}
						function editlesson(b) {
							tempe = b;
							$("#lessonName").val($(tempe).parent().parent().children(".lesson").children("span").html());
							$("#lessonType option:first-child").attr("selected", "selected");
							$("#lessonStart").val($(tempe).parent().parent().children(".lesson").children(".starttime").html());
							$("#lessonEnd").val($(tempe).parent().parent().children(".lesson").children(".endtime").html());
							$("#addlesson").dialog({
								autoOpen: true,
								width: 400,
								buttons: {
									"Update": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/EditLesson',
											data: '{ "origname": "' + $(tempe).parent().parent().children(".lesson").children("span").html() + '", "name": "' + $("#lessonName").val() + '", "type": "' + $("#lessonType").val() + '", "start": "' + $("#lessonStart").val() + '", "end": "' + $("#lessonEnd").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnLessonEditSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnLessonEditSuccess(response) {
							if (response != null && response.EditLessonResult != null) {
								var data = response.EditLessonResult;
								if (data != 0) alert(data);
								else {
									$(tempe).parent().parent().children(".lesson").children("span").html($("#lessonName").val());
									$(tempe).parent().parent().children(".lesson").children("i").html($("#lessonType").val());
									$(tempe).parent().parent().children(".lesson").children(".starttime").html($("#lessonStart").val());
									$(tempe).parent().parent().children(".lesson").children(".endtime").html($("#lessonEnd").val());
									tempe = null;
									resetButtons();
								}
							}
						}
						function removelesson(b) {
							tempe = b;
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveLesson',
								data: '{ "name": "' + $(b).parent().parent().children(".lesson").children("span").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnLessonRemoveSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnLessonRemoveSuccess(response) {
							if (response != null && response.RemoveLessonResult != null) {
								var data = response.RemoveLessonResult;
								if (data != 0) alert(data);
								else {
									$(tempe).parent().parent().remove();
									tempe = null;
								}
							}
						}
						function addsubject() {
							$("#subject").val("");
							$("#addsub").dialog({
								autoOpen: true,
								width: 400,
								buttons: {
									"Add": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddSubject',
											data: '{ "subject": "' + $("#subject").val() + '"   }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnSubjectAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnSubjectAddSuccess(response) {
							if (response != null && response.AddSubjectResult != null) {
								var data = response.AddSubjectResult;
								if (data != 0) alert(data);
								else {
									$("#subjects").append('<div style="overflow: hidden;"><span style="float: left;">' + $("#subject").val() + '</span><div class="cbuttonset"><button onclick="return editsubject(this);" title="Edit">Edit</button><button onclick="return removesubject(this);" title="Delete">Delete</button></div></div>');
									resetButtons();
								}
							}
						}
						function editsubject(b) {
							tempe = b;
							$("#subject").val($(tempe).parent().parent().children("span").html());
							$("#addsub").dialog({
								autoOpen: true,
								width: 400,
								buttons: {
									"Update": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateSubject',
											data: '{ "origsubject": "' + $(tempe).parent().parent().children("span").html() + '", "subject": "' + $("#subject").val() + '"   }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnSubjectEditSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnSubjectEditSuccess(response) {
							if (response != null && response.UpdateSubjectResult != null) {
								var data = response.UpdateSubjectResult;
								if (data != 0) alert(data);
								else {
									$(tempe).parent().parent().children("span").html($("#subject").val());
									tempe = null;
								}
							}
						}
						function removesubject(b) {
							tempe = b;
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveSubject',
								data: '{ "subject": "' + $(tempe).parent().parent().children("span").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnRemoveSubjectSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnRemoveSubjectSuccess(response) {
							if (response != null && response.RemoveSubjectResult != null) {
								var data = response.RemoveSubjectResult;
								if (data != 0) alert(data);
								else {
									$(tempe).parent().parent().remove();
									tempe = null;
								}
							}
						}
						function removelink(b) {
							tempe = b;
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveLink',
								data: '{ "group": "' + $("#" + $(tempe).parent().parent().parent().attr("id") + " > h4 > .lgName").html() + '", "name": "' + $("#" + $(tempe).parent().attr("id") + " > a > b").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnLinkDeleteSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnLinkDeleteSuccess(response) {
							if (response != null && response.RemoveLinkResult != null) {
								var data = response.RemoveLinkResult;
								if (data != 0) alert(data);
								else {
									$(tempe).parent().remove();
									tempe = null;
								}
							}
						}
						function addlink(e) {
							$("#linkName").val("");
							$("#linkIcon").val("");
							$("#linkDesc").val("");
							$("#linkUrl").val("");
							$("#linkTarget").val("");
							$("#linkShowTo").val("");
							tempe = e;
							$("#linkEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Update": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddLink',
											data: '{ "group": "' + $("#" + tempe + " > h4 > .lgName").html() + '", "name": "' + $("#linkName").val() + '", "desc": "' + $("#linkDesc").val() + '", "icon": "' + $("#linkIcon").val() + '", "url": "' + $("#linkUrl").val() + '", "target": "' + $("#linkTarget").val() + '", "showto": "' + $("#linkShowTo").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnLinkAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnLinkAddSuccess(response) {
							if (response != null && response.AddLinkResult != null) {
								var data = response.AddLinkResult;
								if (data != 0) alert(data);
								else {
									$("#" + tempe + " > .sortable").append('<div class="homepagelink" id="link' + $("#linkName").val().replace(' ', '-') + '"><button title="Remove" onclick="return removelink(this);">X</button><a href="#link" title="Edit" onclick="return editlink(\'link' + $("#linkName").val().replace(' ', '-') + '\');"><img src="' + $("#linkIcon").val().replace("~/", root) + '" alt="" /><b>' + $("#linkName").val() + '</b><i>' + $("#linkDesc").val() + '</i><span>' + $("#linkTarget").val() + '</span><u>' + $("#linkUrl").val() + '</u><dd>' + $("#linkShowTo").val() + '</dd></a></div>');
									resetButtons();
									tempe = null;
								}
							}
						}
						function editlink(e) {
							$("#linkName").val($(e).children("b").html());
							$("#linkIcon").val($(e).children("img").attr("src").replace(root, "~/"));
							$("#linkDesc").val($(e).children("i").html());
							$("#linkUrl").val($(e).children("u").html());
							$("#linkTarget").val($(e).children("span").html());
							$("#linkShowTo").val($(e).children("dd").html());
							tempe = e;
							$("#linkEditor").dialog({
								autoOpen: true,
								width: 500,
								buttons: {
									"Update": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateLink',
											data: '{ "group": "' + $("#" + $(tempe).parent().parent().parent().attr("id") + " > h4 > .lgName").html() + '", "origname": "' + $(tempe).children("b").html() + '", "name": "' + $("#linkName").val() + '", "desc": "' + $("#linkDesc").val() + '", "icon": "' + $("#linkIcon").val() + '", "url": "' + $("#linkUrl").val() + '", "target": "' + $("#linkTarget").val() + '", "showto": "' + $("#linkShowTo").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnLinkUpdateSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnLinkUpdateSuccess(response) {
							if (response != null && response.UpdateLinkResult != null) {
								var data = response.UpdateLinkResult;
								if (data != 0) alert(data);
								else {
									$(tempe).children("b").html($("#linkName").val());
									$(tempe).children("img").attr("src", $("#linkIcon").val().replace("~/", root));
									$(tempe).children("i").html($("#linkDesc").val());
									$(tempe).children("u").html($("#linkUrl").val());
									$(tempe).children("span").html($("#linkTarget").val());
									$(tempe).children("dd").html($("#linkShowTo").val());
									$("#" + tempe).attr("id", "link" + $("#linkName").val().replace(' ', '_'));
									tempe = null;
								}
							}
						}
						function removegroup(e) {
							tempe = e;
							$.ajax({
								type: 'POST',
								url: 'API/Setup/RemoveLinkGroup',
								data: '{ "name": "' + $("#" + tempe + " > h4 > .lgName").html() + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnLinkGroupDeleteSuccess,
								error: OnUpdateError
							});
							return false;
						}
						function OnLinkGroupDeleteSuccess(response) {
							if (response != null && response.RemoveLinkGroupResult != null) {
								var data = response.RemoveLinkGroupResult;
								if (data != 0) alert(data);
								else {
									$("#" + tempe).remove();
									tempe = null;
								}
							}
						}
						function editgroup(e) {
							$("#groupName").val($("#linkgroup" + e + " > h4 > .lgName").html());
							$("#groupShowTo").val($("#linkgroup" + e + " > h4 > .lgST").html());
							$("#groupSubTitle").valueOf($("#linkgroup" + e + " > p").html());
							tempe = e;
							$("#linkgroupEditor").dialog({
								autoOpen: true,
								width: 350,
								buttons: {
									"Update": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/UpdateLinkGroup',
											data: '{ "origname": "' + $("#linkgroup" + tempe + " > h4 > .lgName").html() + '", "name": "' + $("#groupName").val() + '", "showto": "' + $("#groupShowTo").val() + '", "subtitle": "' + $("#groupSubTitle").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnLinkGroupUpdateSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										tempe = null;
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnLinkGroupUpdateSuccess(response) {
							if (response != null && response.UpdateLinkGroupResult != null) {
								var data = response.UpdateLinkGroupResult;
								if (data != 0) alert(data);
								else {
									$("#linkgroup" + tempe + " > h4 > .lgName").html($("#groupName").val());
									$("#linkgroup" + tempe + " > h4 > .lgST").html($("#groupShowTo").val());
									$("#linkgroup" + tempe + " > p").html($("#groupSubTitle").val());
									$("#linkgroup" + tempe).attr("id", "linkgroup" + $("#groupName").val().replace(' ', '_'));
									tempe = null;
								}
							}
						}
						function addgroup() {
							$("#groupName").val("");
							$("#groupShowTo").val("");
							$("#groupSubTitle").val("");
							$("#linkgroupEditor").dialog({
								autoOpen: true,
								width: 350,
								buttons: {
									"Update": function () {
										$.ajax({
											type: 'POST',
											url: 'API/Setup/AddLinkGroup',
											data: '{ "name": "' + $("#groupName").val() + '", "showto": "' + $("#groupShowTo").val() + '", "subtitle": "' + $("#groupSubTitle").val() + '" }',
											contentType: 'application/json; charset=utf-8',
											dataType: 'json',
											success: OnLinkGroupAddSuccess,
											error: OnUpdateError
										});
										$(this).dialog("close");
									},
									"Close": function () {
										$(this).dialog("close");
									}
								}
							});
							return false;
						}
						function OnLinkGroupAddSuccess(response) {
							if (response != null && response.AddLinkGroupResult != null) {
								var data = response.AddLinkGroupResult;
								if (data != 0) alert(data);
								else {
									$(".sortablegroup").append('<div id="linkgroup' + $("#groupName").val().replace(' ', '_') + '" class="linkgroup"><h4><span class="lgName">' + $("#groupName").val() + '</span> (<span class="lgST">' + $("#groupShowTo").val() + '</span>) <div class="cbuttonset" style="display: inline;"><button class="edit" onclick="return editgroup(\'' + $("#groupName").val().replace(' ', '_') + '\');">Edit</button><button class="minusbutton" onclick="return removegroup(\'linkgroup' + $("#groupName").val().replace(' ', '_') + '\');">Delete</button><button class="addbutton" onclick="return addlink(\'linkgroup' + $("#groupName").val().replace(' ', '_') + '\');">Add Link</button></div></h4><p>' + $("#groupSubTitle").val() + '</p><div class="sortable"></div></div>');
									resetButtons();
								}
							}
						}
						function add() {
							if ($("#customgroup").val().length < 2) return false;
							$("#adgroups-custom").append('<option value="' + $("#customgroup").val() + '">' + $("#customgroup").val() + '</option>');
							$("#customgroup").val('');
							return false;
						}
						function checktracker() {
							if ($("#<%=trackerprovider.ClientID %>").val().length > 2 && $("#<%=trackercode.ClientID %>").val().length > 0 && $("#<%=trackerstudentlogs.ClientID %>").val().match(/\d/g) && $("#<%=trackerstafflogs.ClientID %>").val().match(/\d/g))
								$("#<%=trackerstate.ClientID %>").attr("src", root + "images/setup/267.png");
							else $("#<%=trackerstate.ClientID %>").attr("src", root + "images/setup/266.png");
						}
						function checksmtp() {
							if ($("#<%=smtpenabled.ClientID %>").attr("checked") && $("#<%=smtpenabled.ClientID %>").attr("checked") == "checked") {
								if ($("#<%=smtpaddress.ClientID %>").val().length > 2 && $("#<%=smtpport.ClientID %>").val().length > 1 && $("#<%=smtpfromname.ClientID %>").val().length > 2 && $("#<%=smtpfromemail.ClientID %>").val().length > 2)
									$("#<%=smtpstate.ClientID %>").attr("src", root + "images/setup/267.png");
								else $("#<%=smtpstate.ClientID %>").attr("src", root + "images/setup/266.png");
							} else $("#<%=smtpstate.ClientID %>").attr("src", root + "images/setup/267.png");
						}
						function checkweb() {
							if ($("#<%=proxyenabled.ClientID %>").attr("checked")) {
								if ($("#<%=proxyenabled.ClientID %>").attr("checked") == "checked" && $("#<%=proxyaddress.ClientID %>").val().length > 2 && $("#<%=proxyport.ClientID%>").val().length > 1)
									$("#<%=proxystate.ClientID %>").attr("src", root + "images/setup/267.png");
								else $("#<%=proxystate.ClientID %>").attr("src", root + "images/setup/266.png");
							} else $("#<%=proxystate.ClientID %>").attr("src", root + "images/setup/267.png");
						}
						function updatetab(e) {
							var d = e + "@" + $("#homepagetabs-" + e + "-Name").val() + "@" + $("#homepagetabs-" + e + "-ShowTo").val();
							if (e == "Me") d += "@" + $("#homepagetabs-" + e + "-allowupdateto").val() + "@" + (($("#homepagetabs-" + e + "-showspace").attr("checked") && $("#homepagetabs-" + e + "-showspace").attr("checked") == "checked") ? "true" : "false");
							$.ajax({
								type: 'POST',
								url: 'API/Setup/UpdateTab',
								data: '{ "tab": "' + d + '" }',
								contentType: 'application/json; charset=utf-8',
								dataType: 'json',
								success: OnTabUpdateSuccess,
								error: OnUpdateError
							});
						}
						function OnTabUpdateSuccess(response) {
							if (response != null && response.UpdateTabResult != null) {
								var data = response.UpdateTabResult;
								if (data != 0) alert(data);
							}
						}
						function OnGroupOrderUpdateSuccess(response) {
							if (response != null) {
								if (response.UpdateLinkGroupOrderResult != null) {
									var data = response.UpdateLinkGroupOrderResult;
									if (data != 0) alert(data);
								} else if (response.UpdateLinkOrderResult != null) {
									var data = response.UpdateLinkOrderResult;
									if (data != 0) alert(data);
								}
							}
						}
						function checkad() {
							$("#treeprogress").progressbar({ value: 0 });
							$("#treeprogress").show();
							document.getElementById("tree").innerHTML = "";
							if ($("#<%=un.ClientID %>").val().length > 2 && $("#<%=up.ClientID %>").val().length > 2 && $("#<%=upn.ClientID %>").val().length > 2) {
								$.ajax({
									type: 'POST',
									url: 'api/setup/GetADTree',
									data: '{"username": "' + $("#<%=un.ClientID %>").val() + '", "password": "' + $("#<%=up.ClientID %>").val() + '", "domain": "' + $("#<%=upn.ClientID %>").val() + '"}',
									contentType: 'application/json; charset=utf-8',
									dataType: 'json',
									success: OnADUpdateSuccess,
									error: OnUpdateError
								});
								$("#treeprogress").progressbar({ value: 10 });
							}
						}
						function OnADUpdateSuccess(response) {
							if (response != null && response.GetADTreeResult != null) {
								var data = response.GetADTreeResult;
								$("#treeprogress").progressbar({ value: 90 });
								var t = $(processTreeNode(data)).appendTo("#tree");
								$("#treeprogress").progressbar({ value: 100 });
								$("#<%=adstate.ClientID %>").attr("src", root + "images/setup/267.png");
								$("#treecontainer").dynatree({ imagePath: root + "images/setup/", selectMode: 1, noLink: true });
								$("#treeprogress").hide();
								console.info(data);
							} else $("#<%=adstate.ClientID %>").attr("src", root + "images/setup/266.png");
						}
						function processTreeNode(o) {
							var s = "<li data=\"icon: '" + o.Icon.replace(/~\//g, root) + "'\">" + (o.Url == null ? "" : "<a href=\"" + o.Url + "\">") + o.Name + (o.Url == null ? "" : "</a>");
							if (o.Items.length > 0) {
								s += "<ul>";
								for (var i = 0; i < o.Items.length; i++) s += processTreeNode(o.Items[i]);
								s += "</ul>";
							}
							s += "</li>";
							return s;
						}
						function selectad(path, type) {
							if (reqtype == type) {
								obj.value = path;
								if (obj.onchange != null) obj.onchange();
								$("#adbrowserwrapper").dialog("close");
							}
						}
						function showadbrowser(e, type) {
							reqtype = type;
							obj = e;
							$("#adgroups-all").attr("checked", "checked");
							$("#adbrowserwrapper").dialog({
								autoOpen: true,
								width: 400,
								height: 600,
								buttons: {
									"Close": function () {
										$(this).dialog("close");
									}
								}
							});
						}
						function showadbuilder(e, inherit, noall) {
							obj2 = e;
							if (noall) {
								$("#adgroups-mode").attr("style", "display: none");
								document.getElementById("adgroup-custom").style.display = "block";
								$("#adgroups-mode-custom").attr("checked", "checked");
								$("#adgroups-mode-all").removeAttr("checked");
								$("#adgroups-mode-inherit").removeAttr("checked");
							}
							else {
								$("#adgroups-mode").removeAttr("style");
								document.getElementById("adgroup-custom").style.display = "none";
								$("#adgroups-mode-custom").removeAttr("checked");
								$("#adgroups-mode-all").attr("checked", "checked");
								$("#adgroups-mode-inherit").removeAttr("checked");
							}
							if (inherit) $("#adgroups-mode-inherit-label").show();
							else $("#adgroups-mode-inherit-label").hide();
							$('#adgroups-custom option').remove();
							if ($(obj2).val() == "All") {
								$("#adgroups-mode-custom").removeAttr("checked");
								$("#adgroups-mode-all").attr("checked", "checked");
								$("#adgroups-mode-inherit").removeAttr("checked");
							}
							else if ($(obj2).val() == "Inherit") {
								$("#adgroups-mode-custom").removeAttr("checked");
								$("#adgroups-mode-all").removeAttr("checked");
								$("#adgroups-mode-inherit").attr("checked", "checked");
							}
							else {
								$("#adgroups-mode-custom").attr("checked", "checked");
								$("#adgroups-mode-all").removeAttr("checked");
								$("#adgroups-mode-inherit").removeAttr("checked");
								document.getElementById("adgroup-custom").style.display = "block";
								for (var x = 0; x < $(obj2).val().split(', ').length; x++) {
									$("#adgroups-custom").append('<option value="' + $(obj2).val().split(', ')[x] + '">' + $(obj2).val().split(', ')[x] + '</option>');
								}
							}
							$("#adgroups").dialog({
								autoOpen: true, width: 400, buttons: {
									"OK": function () {
										if ($("#adgroups-mode-all").attr("checked")) obj2.value = "All";
										else if ($("#adgroups-mode-inherit").attr("checked")) obj2.value = "Inherit";
										else { var a = []; for (var x = 0; x < document.getElementById("adgroups-custom").childNodes.length; x++) if (document.getElementById("adgroups-custom").childNodes[x].tagName) a.push(document.getElementById("adgroups-custom").childNodes[x].value); obj2.value = a.join(", "); }
										$(obj2).trigger('change');
										$(this).dialog("close");
									},
									"Cancel": function () {
										$(this).dialog("close");
									}
								}
							});
						}
						Sys.Application.add_load(resetButtons);
						function resetButtons(sender, args) {
							$("input:submit,input:button,a.button,button").button();
							$(".checkbutton").button({ icons: { primary: "ui-icon-check"} });
							$(".addbutton").button({ icons: { primary: "ui-icon-plus"} });
							$(".minusbutton").button({ icons: { primary: "ui-icon-minus"} });
							$(".edit").button({ icons: { primary: "ui-icon-wrench"} });
							$("#adgroups-mode").buttonset();
							$("#mappingUsageMode").buttonset();
							$(".cbuttonset").buttonset();
							$(".searchbutton").button({ icons: { primary: "ui-icon-search" }, text: false });
							$("#lessonStart").timePkr();
							$("#lessonEnd").timePkr();
						}
						function generalchange() {
							var i = "266.png";
							if ($("#<%=name.ClientID %>").val().length > 2 && $("#<%=schoolurl.ClientID %>").val().length > 2) i = "267.png";
							$("#<%=generalstate.ClientID %>").attr("src", root + "images/setup/" + i);
						}
						$(function () {
							$('#homepagetabs').tabs();
							$(".sortablegroup").sortable({
								placeholder: 'homepagegroup',
								axis: 'y',
								update: function (e, u) {
									$.ajax({
										type: 'POST',
										url: 'API/Setup/UpdateLinkGroupOrder',
										data: '{"groups": "' + $(this).sortable('toArray').toString() + '"}',
										contentType: 'application/json; charset=utf-8',
										dataType: 'json',
										success: OnGroupOrderUpdateSuccess,
										error: OnUpdateError
									});
								}
							});
							$(".sortablegroup").disableSelection();
							$(".sortable").sortable({
								placeholder: 'homepagelink',
								update: function (e, u) {
									$.ajax({
										type: 'POST',
										url: 'API/Setup/UpdateLinkOrder',
										data: '{"group": "' + $(this).parent(".linkgroup").attr("id") + '", "links": "' + $(this).sortable('toArray').toString() + '"}',
										contentType: 'application/json; charset=utf-8',
										dataType: 'json',
										success: OnGroupOrderUpdateSuccess,
										error: OnUpdateError
									});
								}
							});
							$(".sortable").disableSelection();
							$("#adgroups-mode-custom").click(function () {
								document.getElementById("adgroup-custom").style.display = "block";
							});
							$("#adgroups-mode-all").click(function () {
								document.getElementById("adgroup-custom").style.display = "none";
							});
							$("#adgroups-mode-inherit").click(function () {
								document.getElementById("adgroup-custom").style.display = "none";
							});
							$("#test").click(function () {
								checkad();
								$("#adbrowserwrapper").dialog({
									autoOpen: true,
									width: 400,
									height: 600,
									buttons: {
										"Close": function () {
											$(this).dialog("close");
										}
									}
								});
								return false;
							});
							generalchange();
							checkweb();
							checksmtp();
							checktracker();
							$("#adgroups").dialog({ autoOpen: false });
							$("#linkgroupEditor").dialog({ autoOpen: false });
							$("#linkEditor").dialog({ autoOpen: false });
							$("#addsub").dialog({ autoOpen: false });
							$("#addres").dialog({ autoOpen: false });
							$("#addlesson").dialog({ autoOpen: false });
							$("#adbrowserwrapper").dialog({ autoOpen: false });
							$("#qserverEditor").dialog({ autoOpen: false });
							$("#filterEditor").dialog({ autoOpen: false });
							$("#mappingEditor").dialog({ autoOpen: false });
						});
						checkad();
					</script>
				</div>
			</div>
		</form>
	</body>
</html>