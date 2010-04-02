/// Author:				Joe Audette
/// Created:			2004-09-12
/// Last Modified:	    2009-12-18

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cynthia.Business;
using Cynthia.Business.WebHelpers;
using Cynthia.Web.Framework;
using Cynthia.Web.UI;
using Resources;

namespace Cynthia.Web.ForumUI
{
    public partial class ForumView : CBasePage
	{

        protected string EditContentImage = WebConfigSettings.EditContentImage;
        protected string RSSImageFileName = WebConfigSettings.RSSImageFileName;
        protected string ThreadImage = WebConfigSettings.ForumThreadImage;
        private string NewThreadImage = WebConfigSettings.NewThreadImage;
        protected int PageId = -1;
        protected int ModuleId = -1;
        protected int ItemId = -1;
		private int pageNumber = 1;
        protected bool EnableRssAtThreadLevel = false;
        private Hashtable moduleSettings;
        protected Double TimeOffset = 0;
        protected string notificationUrl = string.Empty;
        protected bool isSubscribedToForum = false;
        private SiteUser currentUser = null;
        

        #region OnInit

        protected override void OnPreInit(EventArgs e)
        {
            AllowSkinOverride = true;
            base.OnPreInit(e);
        }

        override protected void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion


        private void Page_Load(object sender, EventArgs e)
		{
            if (Page.IsPostBack) return;

            if ((siteSettings != null)&&(CurrentPage != null))
            {
                if ((SiteUtils.SslIsAvailable())
                    &&((siteSettings.UseSslOnAllPages)||(CurrentPage.RequireSsl))
                    )
                {
                    SiteUtils.ForceSsl();
                }
                else
                {
                    SiteUtils.ClearSsl();
                }

            }

            LoadParams();

            if (!UserCanViewPage(ModuleId))
            {
                if (!Request.IsAuthenticated)
                {
                    SiteUtils.RedirectToLoginPage(this);
                    return;
                }
                else
                {
                    SiteUtils.RedirectToAccessDeniedPage(this);
                    return;
                }
                
            }

            SetupCss();
            PopulateLabels();
            
            GetModuleSettings();
#if MONO
            this.rptForums.DataBind();
#else
            this.DataBind();
#endif
			PopulateControls();

		}

		

		private void PopulateControls()
		{
			Forum forum = new Forum(ItemId);

            Title = SiteUtils.FormatPageTitle(siteSettings, forum.Title);

            litForumTitle.Text = forum.Title;
			litForumDescription.Text = forum.Description;

            MetaDescription = string.Format(CultureInfo.InvariantCulture, ForumResources.ForumMetaDescriptionFormat, forum.Title);

            string pageUrl = siteSettings.SiteRoot 
				+ "/Forums/ForumView.aspx?"
				+ "ItemID=" + forum.ItemId.ToInvariantString()
                + "&amp;mid=" + ModuleId.ToInvariantString()
                + "&amp;pageid=" + PageId.ToInvariantString()
				+ "&amp;pagenumber={0}";

            pgrTop.PageURLFormat = pageUrl;
            pgrTop.ShowFirstLast = true;
            pgrTop.CurrentIndex = pageNumber;
            pgrTop.PageSize = forum.ThreadsPerPage;
            pgrTop.PageCount = forum.TotalPages;
            pgrTop.Visible = (pgrTop.PageCount > 1);

            pgrBottom.PageURLFormat = pageUrl;
            pgrBottom.ShowFirstLast = true;
            pgrBottom.CurrentIndex = pageNumber;
            pgrBottom.PageSize = forum.ThreadsPerPage;
            pgrBottom.PageCount = forum.TotalPages;
            pgrBottom.Visible = (pgrBottom.PageCount > 1);

			if((Request.IsAuthenticated)||(forum.AllowAnonymousPosts))
			{
                lnkNewThread.InnerHtml = "<img alt='' src='" + siteSettings.SiteRoot + "/Data/SiteImages/" + NewThreadImage + "'  />&nbsp;"
                    + ForumResources.ForumViewNewThreadLabel;
                lnkNewThread.HRef = siteSettings.SiteRoot
                    + "/Forums/EditPost.aspx?forumid=" + ItemId.ToString(CultureInfo.InvariantCulture)
                    + "&amp;pageid=" + PageId.ToString(CultureInfo.InvariantCulture);

                lnkNewThreadBottom.InnerHtml = "<img alt='' src='" + siteSettings.SiteRoot + "/Data/SiteImages/" + NewThreadImage + "'  />&nbsp;"
                    + ForumResources.ForumViewNewThreadLabel;

                lnkNewThreadBottom.HRef = siteSettings.SiteRoot
                    + "/Forums/EditPost.aspx?forumid=" + ItemId.ToString(CultureInfo.InvariantCulture)
                    + "&amp;pageid=" + PageId.ToString(CultureInfo.InvariantCulture);
                
                lnkLogin.Visible = false;

			}

            lnkLogin.NavigateUrl = SiteRoot + "/Secure/Login.aspx";
            lnkLogin.Text = ForumResources.ForumsLoginRequiredLink;

            if (Page.Header != null)
            {

                Literal link = new Literal();
                link.ID = "forumurl";

                string canonicalUrl = siteSettings.SiteRoot
                    + "/Forums/ForumView.aspx?"
                    + "ItemID=" + forum.ItemId.ToInvariantString()
                    + "&amp;mid=" + ModuleId.ToInvariantString()
                    + "&amp;pageid=" + PageId.ToInvariantString()
                    + "&amp;pagenumber=" + pageNumber.ToInvariantString();

                link.Text = "\n<link rel='canonical' href='" + canonicalUrl + "' />";

                Page.Header.Controls.Add(link);
            }

            using (IDataReader reader = forum.GetThreads(pageNumber))
            {
                rptForums.DataSource = reader;
                rptForums.DataBind();
            }

		}


		public bool GetPermission(object startedByUser)
		{
			if (WebUser.IsAdmin || WebUser.IsContentAdmin) return true;
			return false;
		}


        private void GetModuleSettings()
        {
            if (ModuleId > -1)
            {
                moduleSettings = ModuleSettings.GetModuleSettings(ModuleId);
                EnableRssAtThreadLevel = WebUtils.ParseBoolFromHashtable(moduleSettings, "ForumEnableRSSAtThreadLevel", false);

            }

        }

        

        private void PopulateLabels()
        {
            lnkPageCrumb.Text = CurrentPage.PageName;
            lnkPageCrumb.NavigateUrl = SiteUtils.GetCurrentPageUrl();
            //EditAltText = Resource.EditImageAltText;
            pgrTop.NavigateToPageText = ForumResources.CutePagerNavigateToPageText;
            pgrTop.BackToFirstClause = ForumResources.CutePagerBackToFirstClause;
            pgrTop.GoToLastClause = ForumResources.CutePagerGoToLastClause;
            pgrTop.BackToPageClause = ForumResources.CutePagerBackToPageClause;
            pgrTop.NextToPageClause = ForumResources.CutePagerNextToPageClause;
            pgrTop.PageClause = ForumResources.CutePagerPageClause;
            pgrTop.OfClause = ForumResources.CutePagerOfClause;

            pgrBottom.NavigateToPageText = ForumResources.CutePagerNavigateToPageText;
            pgrBottom.BackToFirstClause = ForumResources.CutePagerBackToFirstClause;
            pgrBottom.GoToLastClause = ForumResources.CutePagerGoToLastClause;
            pgrBottom.BackToPageClause = ForumResources.CutePagerBackToPageClause;
            pgrBottom.NextToPageClause = ForumResources.CutePagerNextToPageClause;
            pgrBottom.PageClause = ForumResources.CutePagerPageClause;
            pgrBottom.OfClause = ForumResources.CutePagerOfClause;

            

        }

        private void SetupCss()
        {
            // older skins have this
            StyleSheet stylesheet = (StyleSheet)Page.Master.FindControl("StyleSheet");
            if (stylesheet != null)
            {
                if (stylesheet.FindControl("forumcss") == null)
                {
                    Literal cssLink = new Literal();
                    cssLink.ID = "forumcss";
                    cssLink.Text = "\n<link href='"
                    + SiteUtils.GetSkinBaseUrl()
                    + "forummodule.css' type='text/css' rel='stylesheet' media='screen' />";

                    stylesheet.Controls.Add(cssLink);
                }
            }
            
        }

        private void LoadParams()
        {
            PageId = WebUtils.ParseInt32FromQueryString("pageid", -1);
            ModuleId = WebUtils.ParseInt32FromQueryString("mid", -1);
            ItemId = WebUtils.ParseInt32FromQueryString("ItemID", -1);
            pageNumber = WebUtils.ParseInt32FromQueryString("pagenumber", 1);
            TimeOffset = SiteUtils.GetUserTimeOffset();

            notificationUrl = SiteRoot + "/Forums/EditSubscriptions.aspx?mid="
                + ModuleId.ToInvariantString()
                + "&pageid=" + PageId.ToInvariantString() +"#forum" + ItemId.ToInvariantString();

            if (Request.IsAuthenticated)
            {
                currentUser = SiteUtils.GetCurrentSiteUser();
                if ((currentUser != null)&&(ItemId > -1))
                {
                    isSubscribedToForum = Forum.IsSubscribed(ItemId, currentUser.UserId);
                }

                if (!isSubscribedToForum) { pnlNotify.Visible = true; }
                
            }

        }

       
	}
}
