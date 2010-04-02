<%@ Page Language="c#" AutoEventWireup="false" Codebehind="EditSubscriptions.aspx.cs" MasterPageFile="~/App_MasterPages/layout.Master" Inherits="Cynthia.Web.ForumUI.ForumModuleEditSubscriptions" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" />
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
<portal:CPanel ID="mp1" runat="server" ArtisteerCssClass="art-Post" RenderArtisteerBlockContentDivs="true">
<cy:CornerRounderTop id="ctop1" runat="server" />
<asp:Panel id="pnlForum" runat="server" CssClass="art-Post-inner panelwrapper forums">
    <h2 class="moduletitle"><asp:Literal ID="litHeading" runat="server" /></h2>
    <portal:CPanel ID="CynPanel1" runat="server" ArtisteerCssClass="art-PostContent">
    <div class="modulecontent">
	<table summary='<%# Resources.ForumResources.ForumsTableSummary %>' cellpadding="0" cellspacing="1" border="0" width="99%">
	<thead>
		<tr class="moduletitle">
		    <th id="tdSubscribedHead" runat="server" >
			    <cy:SiteLabel id="lblSubscribed" runat="server" ConfigKey="ForumModuleSubscribedLabel" ResourceFile="ForumResources" UseLabelTag="false" />
		    </th>
		    <th id='<%# Resources.ForumResources.ForumModuleForumLabel %>'>
			    <cy:SiteLabel id="lblForumName" runat="server" ConfigKey="ForumModuleForumLabel" ResourceFile="ForumResources" UseLabelTag="false" />
		    </th>
		    <th id='<%# Resources.ForumResources.ForumModuleThreadCountLabel %>'>
			    <cy:SiteLabel id="lblThreadCount" runat="server" ConfigKey="ForumModuleThreadCountLabel" ResourceFile="ForumResources" UseLabelTag="false" />
		    </th>
		    <th id='<%# Resources.ForumResources.ForumModulePostCountLabel %>'>
			    <cy:SiteLabel id="lblPostCount" runat="server" ConfigKey="ForumModulePostCountLabel" ResourceFile="ForumResources" UseLabelTag="false" />
		    </th>
		    <th id='<%# Resources.ForumResources.ForumModulePostLastPostLabel %>'>
			    <cy:SiteLabel id="lblLastPost" runat="server" ConfigKey="ForumModulePostLastPostLabel" ResourceFile="ForumResources" UseLabelTag="false" />
		    </th>
	    </tr></thead>
	   <asp:Repeater id="rptForums" runat="server" >
	      <ItemTemplate >
	         <tr class="modulerow">
	            <td id="tdSubscribed" runat="server" visible='<%# Request.IsAuthenticated %>'>  
	                <a id='forum<%# Eval("ItemID") %>' />
					<asp:CheckBox id="chkSubscribed" runat="server"
						Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"Subscribed")) %>'
						OnCheckedChanged="Subscribed_CheckedChanged" />
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModuleForumLabel %>'> 
					<h3><asp:HyperLink id="viewlink1" SkinID="TitleLink"
					NavigateUrl='<%# siteSettings.SiteRoot + "/Forums/ForumView.aspx?ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mid=" + ModuleId.ToString()  + "&pageid=" + PageId.ToString() %>'  
					runat="server"><%# DataBinder.Eval(Container.DataItem,"Title") %></asp:HyperLink></h3>
					<%# DataBinder.Eval(Container.DataItem,"Description") %>
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModuleThreadCountLabel %>'>  
					<%# DataBinder.Eval(Container.DataItem,"ThreadCount") %>
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModulePostCountLabel %>'>  
					<%# DataBinder.Eval(Container.DataItem,"PostCount") %>
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModulePostLastPostLabel %>'>  
					 <%# DateTimeHelper.GetTimeZoneAdjustedDateTimeString(((System.Data.Common.DbDataRecord)Container.DataItem), "MostRecentPostDate", TimeOffset)%>
	            </td>
	         </tr>
	      </ItemTemplate>
	      <alternatingItemTemplate>
			<tr class="modulealtrow">
	            <td id="tdSubscribedAlt" runat="server" 
	                visible='<%# Request.IsAuthenticated %>'>  
	                <a id='forum<%# Eval("ItemID") %>' />
					<asp:CheckBox id="chkSubscribedAlt" runat="server"
						Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"Subscribed")) %>'
						OnCheckedChanged="Subscribed_CheckedChanged" />
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModuleForumLabel %>'> 
					<h3><asp:HyperLink id="Hyperlink2" SkinID="TitleLink" 
					NavigateUrl='<%# siteSettings.SiteRoot + "/Forums/ForumView.aspx?ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mid=" + ModuleId.ToString()  + "&pageid=" + PageId.ToString() %>'  
					runat="server"><%# DataBinder.Eval(Container.DataItem,"Title") %></asp:HyperLink></h3>
					<%# DataBinder.Eval(Container.DataItem,"Description") %>
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModuleThreadCountLabel %>'>  
					<%# DataBinder.Eval(Container.DataItem,"ThreadCount") %>
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModulePostCountLabel %>'>  
					<%# DataBinder.Eval(Container.DataItem,"PostCount") %>
	            </td>
	            <td headers='<%# Resources.ForumResources.ForumModulePostLastPostLabel %>'>  
					 <%# DateTimeHelper.GetTimeZoneAdjustedDateTimeString(((System.Data.Common.DbDataRecord)Container.DataItem), "MostRecentPostDate", TimeOffset)%>
	            </td>
	         </tr>
	      </AlternatingItemTemplate>
	   </asp:Repeater>
		<tr>
			<td id="tdSave" runat="server" class="settingrow forum" align="left" colspan="5">
				<portal:CButton id="btnSave" runat="server" Text="Save" />
				<portal:CButton id="btnCancel" runat="server" Text="Cancel" />
				<portal:CHelpLink ID="CynHelpLink1" runat="server" HelpKey="forumeditsubscriptionshelp" />
			</td>
		</tr>
	      
	</table>
	</div>
	<asp:HiddenField ID="hdnReturnUrl" runat="server" />
	</portal:CPanel>
	<div class="cleared"></div>
</asp:Panel>
<cy:CornerRounderBottom id="cbottom1" runat="server" />	
</portal:CPanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightContent" ID="MPRightPane" runat="server" />
<asp:Content ContentPlaceHolderID="pageEditContent" ID="MPPageEdit" runat="server" />


