<%@ Control Language="c#"  AutoEventWireup="false" Codebehind="BlogModule.ascx.cs" Inherits="Cynthia.Web.BlogUI.BlogModule" %>
<%@ Register TagPrefix="blog" TagName="TagList" Src="~/Blog/Controls/CategoryListControl.ascx" %>
<%@ Register TagPrefix="blog" TagName="Archives" Src="~/Blog/Controls/ArchiveListControl.ascx" %>
<%@ Register TagPrefix="blog" TagName="FeedLinks" Src="~/Blog/Controls/FeedLinksControl.ascx" %>
<%@ Register TagPrefix="blog" TagName="StatsControl" Src="~/Blog/Controls/StatsControl.ascx" %>

<portal:ModulePanel ID="pnlContainer" runat="server">
<portal:CPanel ID="mp1" runat="server" ArtisteerCssClass="art-Post" RenderArtisteerBlockContentDivs="true">
<cy:CornerRounderTop id="ctop1" runat="server" />
<asp:Panel ID="pnlWrapper" runat="server" cssclass="art-Post-inner panelwrapper blogmodule">
<portal:ModuleTitleControl id="Title1" runat="server" />
<portal:CPanel ID="CynPanel1" runat="server" ArtisteerCssClass="art-PostContent">
<asp:UpdatePanel ID="updBlog" UpdateMode="Conditional" runat="server">
<ContentTemplate>
<div class="modulecontent">
<div class="blogwrapper">
    <asp:Panel id="divNav" runat="server" cssclass="blognavright" SkinID="plain">
        <asp:calendar id="calBlogNav" runat="server" EnableViewState="false"
         CaptionAlign="Top"
         CssClass="aspcalendarmain"
         DayHeaderStyle-CssClass="aspcalendardayheader"
         DayNameFormat="FirstLetter"
         DayStyle-CssClass="aspcalendarday"
         FirstDayOfWeek="sunday"
         NextMonthText="+"
         NextPrevFormat="CustomText"
         NextPrevStyle-CssClass="aspcalendarnextprevious"
         OtherMonthDayStyle-CssClass="aspcalendarothermonth"
         PrevMonthText="-"
         SelectedDayStyle-CssClass="aspcalendarselectedday"
         SelectorStyle-CssClass="aspcalendarselector"
         ShowDayHeader="true"
         ShowGridLines="false"
         ShowNextPrevMonth="true"
         ShowTitle="true"
         TitleFormat="MonthYear"
         TitleStyle-CssClass="aspcalendartitle"
         TodayDayStyle-CssClass="aspcalendartoday"
         WeekendDayStyle-CssClass="aspcalendarweekendday"
        ></asp:calendar><br />
        <blog:FeedLinks id="Feeds" runat="server" />
        <asp:Panel ID="pnlStatistics" runat="server">
        <blog:StatsControl id="stats" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlCategories" Runat="server" SkinID="plain">
	       <blog:TagList id="tags" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlArchives" Runat="server" SkinID="plain">
	        <blog:Archives id="archive" runat="server" />
	        <br class="clear" />
        </asp:Panel>
        		
    </asp:Panel>
    <asp:Panel id="divblog" runat="server" cssclass="blogcenter-rightnav" SkinID="plain">
        <asp:repeater id="rptBlogs" runat="server"  SkinID="Blog" EnableViewState="False" >
	        <ItemTemplate>
	            <h3 class="blogtitle">
		        <asp:HyperLink SkinID="BlogTitle" id="lnkTitle" runat="server" 
		            EnableViewState="false"
		            Text='<%# DataBinder.Eval(Container.DataItem,"Heading") %>' 
		            Visible='<%# BlogUseLinkForHeading %>'  
                    NavigateUrl='<%# FormatBlogTitleUrl(DataBinder.Eval(Container.DataItem,"ItemUrl").ToString(), Convert.ToInt32(DataBinder.Eval(Container.DataItem,"ItemID")))  %>'>
		        </asp:HyperLink><asp:Literal ID="litTitle" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Heading") %>' Visible='<%#(!BlogUseLinkForHeading) %>' />&nbsp;
		        <asp:HyperLink id="editLink"  runat="server" EnableViewState="false"
		            Text="<%# EditLinkText %>" 
				    Tooltip="<%# EditLinkTooltip %>"  
		            ImageUrl='<%# EditLinkImageUrl %>' 
		            NavigateUrl='<%# this.SiteRoot + "/Blog/EditPost.aspx?pageid=" + PageId.ToString() + "&amp;ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleId.ToString() %>' 
		            Visible="<%# IsEditable %>" CssClass="ModuleEditLink" /></h3>
		        <div class="blogdate">
		            <%# FormatPostAuthor(DataBinder.Eval(Container.DataItem, "Name").ToString())%>
			        <%# DateTimeHelper.GetTimeZoneAdjustedDateTimeString(((System.Data.Common.DbDataRecord)Container.DataItem),"StartDate", TimeOffset, BlogDateTimeFormat) %>
		        </div>
		        <asp:Panel ID="pnlPost" runat="server" Visible='<%# !TitleOnly %>'>
		        <portal:CRating runat="server" ID="Rating" Enabled='<%# EnableContentRatingSetting %>' ContentGuid='<%# new Guid(Eval("BlogGuid").ToString()) %>' AllowFeedback='false' />
		        <cy:OdiogoItem id="od1" runat="server" OdiogoFeedId='<%# OdiogoFeedIDSetting %>' ItemId='<%# DataBinder.Eval(Container.DataItem,"ItemID") %>' ItemTitle = '<%# Eval("Heading") %>' />
		        <div class="blogtext"><%# FormatBlogEntry(DataBinder.Eval(Container.DataItem, "Description").ToString(), DataBinder.Eval(Container.DataItem, "Abstract").ToString(), DataBinder.Eval(Container.DataItem, "ItemUrl").ToString(), Convert.ToInt32(Eval("ItemID")))%></div>
		        <goog:LocationMap ID="gmap" runat="server" 
		        Visible='<%# ((Eval("Location").ToString().Length > 0)&&(ShowGoogleMap)) %>' 
		        Location='<%# Eval("Location") %>' 
		        GMapApiKey='<%# GmapApiKey %>' 
		        EnableMapType='<%# GoogleMapEnableMapTypeSetting %>'
		        EnableZoom='<%# GoogleMapEnableZoomSetting %>' 
		        ShowInfoWindow='<%# GoogleMapShowInfoWindowSetting %>' 
		        EnableLocalSearch='<%# GoogleMapEnableLocalSearchSetting %>' 
		        EnableDrivingDirections='<%# GoogleMapEnableDirectionsSetting %>'
		        GmapType='<%# mapType %>' 
		        ZoomLevel='<%# GoogleMapInitialZoomSetting %>'
		        MapHeight='<%# GoogleMapHeightSetting %>'
		        MapWidth='<%# GoogleMapWidthSetting %>'
		        ></goog:LocationMap>
		        
		        <cy:AddThisButton ID="addThis1" runat="server"
		         AccountId='<%# addThisAccountId %>' 
		         Visible='<%# (!HideAddThisButton) %>'
		         UseMouseOverWidget='<%# useAddThisMouseOverWidget %>' 
		         Text='<%# Resources.BlogResources.AddThisButtonAltText %>'
		         TitleOfUrlToShare='<%# DataBinder.Eval(Container.DataItem,"Heading") %>' 
		         CustomBrand='<%# addThisCustomBrand %>'
		         CustomOptions='<%# addThisCustomOptions %>'
		         CustomLogoUrl='<%# addThisCustomLogoUrl %>'
		         CustomLogoBackgroundColor='<%# addThisCustomLogoBackColor %>'
		         CustomLogoColor='<%# addThisCustomLogoForeColor %>'
		         ButtonImageUrl='<%# addThisButtonImageUrl %>'
		         UrlToShare='<%# this.SiteRoot + DataBinder.Eval(Container.DataItem,"ItemUrl").ToString().Replace("~", string.Empty)  %>'
		        />
		        
		        <div  class="blogcommentlink">
			        <asp:HyperLink id="Hyperlink2" runat="server" EnableViewState="false" 
			            Text='<%# FeedBackLabel + "(" + DataBinder.Eval(Container.DataItem,"CommentCount") + ")" %>' 
			            Visible='<%# AllowComments && ShowCommentCounts %>' 
			            NavigateUrl='<%# FormatBlogUrl(DataBinder.Eval(Container.DataItem,"ItemUrl").ToString(), Convert.ToInt32(DataBinder.Eval(Container.DataItem,"ItemID")))  %>' 
			            CssClass="blogcommentlink"></asp:HyperLink>
			            <asp:HyperLink id="Hyperlink1" runat="server" EnableViewState="false" 
			            Text='<%# FeedBackLabel %>' 
			            Visible='<%# AllowComments && !ShowCommentCounts %>' 
			            NavigateUrl='<%# FormatBlogUrl(DataBinder.Eval(Container.DataItem,"ItemUrl").ToString(), Convert.ToInt32(DataBinder.Eval(Container.DataItem,"ItemID")))  %>' 
			            CssClass="blogcommentlink"></asp:HyperLink>&#160;
			        
		        </div>
		        </asp:Panel>
	        </ItemTemplate>
        </asp:repeater>
        <portal:CCutePager ID="pgr" runat="server" />
    </asp:Panel>
    <div class="blogcopyright">
    <asp:label id="lblCopyright" Runat="server" />
    </div>
    <portal:DisqusWidget ID="disqus" runat="server" />
</div>
<div class="modulefooter"></div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</portal:CPanel>
<div class="cleared"></div>
</asp:Panel>
<cy:CornerRounderBottom id="cbottom1" runat="server" />
</portal:CPanel>
</portal:ModulePanel>
