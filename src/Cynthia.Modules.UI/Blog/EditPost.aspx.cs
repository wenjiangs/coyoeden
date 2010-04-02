/// Author:                     Joe Audette
/// Created:                    2004-08-14
///	Last Modified:              2010-02-18
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cynthia.Business;
using Cynthia.Business.WebHelpers;
using Cynthia.Web.Editor;
using Cynthia.Web.Framework;
using Cynthia.Web.UI;
using Resources;
using log4net;

namespace Cynthia.Web.BlogUI
{
    public partial class BlogEdit : CBasePage
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(BlogEdit));

		protected int moduleId = -1;
		protected int itemId = -1;
        protected int pageId = -1;
        protected String cacheDependencyKey;
        protected string virtualRoot;
        protected Double timeOffset = 0;
        protected bool showCategories = false;
        protected string OdiogoFeedIDSetting = string.Empty;
        protected Hashtable moduleSettings;
        private bool useExcerpt = false;
        private int pageNumber = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private Guid restoreGuid = Guid.Empty;
        private Blog blog = null;
        private bool BlogEnableVersioningSetting = false;
        private bool enableContentVersioning = false;
        protected bool isAdmin = false;
        private string defaultCommentDaysAllowed = "90";
        ContentMetaRespository metaRepository = new ContentMetaRespository();


        #region OnInit

        protected override void OnPreInit(EventArgs e)
        {
            AllowSkinOverride = true;
            base.OnPreInit(e);
            SiteUtils.SetupEditor(edContent);
            SiteUtils.SetupEditor(edExcerpt);
        }

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);

            ScriptConfig.IncludeYuiTabs = true;
            IncludeYuiTabsCss = true;

            if (this.btnAddCategory == null)
            {
                log.Error("btnAddCategory was null");

                this.btnAddCategory = (CButton)UpdatePanel1.FindControl("btnAddCategory");
            }

            this.btnAddCategory.Click += new EventHandler(this.btnAddCategory_Click);


            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnUpdate2.Click += new EventHandler(this.btnUpdate_Click);
            this.btnUpdate3.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnDelete2.Click += new EventHandler(this.btnDelete_Click);
            this.btnDelete3.Click += new EventHandler(this.btnDelete_Click);

            btnSaveAndPreview.Click += new EventHandler(btnSaveAndPreview_Click);

            grdHistory.RowCommand += new GridViewCommandEventHandler(grdHistory_RowCommand);
            grdHistory.RowDataBound += new GridViewRowEventHandler(grdHistory_RowDataBound);
            pgrHistory.Command += new CommandEventHandler(pgrHistory_Command);
            btnRestoreFromGreyBox.Click += new System.Web.UI.ImageClickEventHandler(btnRestoreFromGreyBox_Click);
            btnDeleteHistory.Click += new EventHandler(btnDeleteHistory_Click);

            grdContentMeta.RowCommand += new GridViewCommandEventHandler(grdContentMeta_RowCommand);
            grdContentMeta.RowEditing += new GridViewEditEventHandler(grdContentMeta_RowEditing);
            grdContentMeta.RowUpdating += new GridViewUpdateEventHandler(grdContentMeta_RowUpdating);
            grdContentMeta.RowCancelingEdit += new GridViewCancelEditEventHandler(grdContentMeta_RowCancelingEdit);
            grdContentMeta.RowDeleting += new GridViewDeleteEventHandler(grdContentMeta_RowDeleting);
            grdContentMeta.RowDataBound += new GridViewRowEventHandler(grdContentMeta_RowDataBound);
            btnAddMeta.Click += new EventHandler(btnAddMeta_Click);

            grdMetaLinks.RowCommand += new GridViewCommandEventHandler(grdMetaLinks_RowCommand);
            grdMetaLinks.RowEditing += new GridViewEditEventHandler(grdMetaLinks_RowEditing);
            grdMetaLinks.RowUpdating += new GridViewUpdateEventHandler(grdMetaLinks_RowUpdating);
            grdMetaLinks.RowCancelingEdit += new GridViewCancelEditEventHandler(grdMetaLinks_RowCancelingEdit);
            grdMetaLinks.RowDeleting += new GridViewDeleteEventHandler(grdMetaLinks_RowDeleting);
            grdMetaLinks.RowDataBound += new GridViewRowEventHandler(grdMetaLinks_RowDataBound);
            btnAddMetaLink.Click += new EventHandler(btnAddMetaLink_Click);
            
        }

        

        #endregion

        private void Page_Load(object sender, EventArgs e)
		{
            if (!Request.IsAuthenticated)
            {
                SiteUtils.RedirectToLoginPage(this);
                return;
            }

            SecurityHelper.DisableBrowserCache();

            LoadParams();

            if (!UserCanEditModule(moduleId))
            {
                SiteUtils.RedirectToEditAccessDeniedPage();
                return;
            }

            if (ScriptController != null)
            {
                ScriptController.RegisterAsyncPostBackControl(btnAddCategory);
            }
            else
            {
                log.Error("ScriptController was null");
            }

            LoadSettings();
			PopulateLabels();
            SetupScripts();



            if ((!Page.IsPostBack) && (!Page.IsCallback))
            {
                if ((Request.UrlReferrer != null) && (hdnReturnUrl.Value.Length == 0))
                {
                    hdnReturnUrl.Value = Request.UrlReferrer.ToString();
                    lnkCancel.NavigateUrl = Request.UrlReferrer.ToString();
                    lnkCancel2.NavigateUrl = lnkCancel.NavigateUrl;
                    lnkCancel3.NavigateUrl = lnkCancel.NavigateUrl;

                }

				PopulateControls();
				PopulateCategories();
                BindMeta();
                BindMetaLinks();
                
			}

            

		}

		protected virtual void PopulateControls()
		{
            if (blog != null)
            {
                dpBeginDate.ShowTime = true;
                dpBeginDate.Text = DateTimeHelper.LocalizeToCalendar(blog.StartDate.AddHours(timeOffset).ToString());
                txtTitle.Text = blog.Title;
                txtItemUrl.Text = blog.ItemUrl;
                txtLocation.Text = blog.Location;
                edContent.Text = blog.Description;
                edExcerpt.Text = blog.Excerpt;
                txtMetaDescription.Text = blog.MetaDescription;
                txtMetaKeywords.Text = blog.MetaKeywords;
                this.chkIncludeInFeed.Checked = blog.IncludeInFeed;
                chkIsPublished.Checked = blog.IsPublished;
                ListItem item 
                    = ddCommentAllowedForDays.Items.FindByValue(blog.AllowCommentsForDays.ToString(CultureInfo.InvariantCulture));
                if (item != null)
                {
                    ddCommentAllowedForDays.ClearSelection();
                    item.Selected = true;
                }

                if (restoreGuid != Guid.Empty)
                {
                    ContentHistory rHistory = new ContentHistory(restoreGuid);
                    if (rHistory.ContentGuid == blog.BlogGuid)
                    {
                        edContent.Text = rHistory.ContentText;
                    }

                }
                // show preview button for saved drafts
                if ((!blog.IsPublished) || (blog.StartDate > DateTime.UtcNow)) { btnSaveAndPreview.Visible = true; }

                BindHistory();
            }
            else
            {
                chkIncludeInFeed.Checked = true;
                dpBeginDate.Text = DateTimeHelper.LocalizeToCalendar(DateTime.UtcNow.AddHours(timeOffset).ToString());
                this.btnDelete.Visible = false;
                pnlHistory.Visible = false;
            }

            if ((txtItemUrl.Text.Length == 0)&&(txtTitle.Text.Length > 0))
            {
                String friendlyUrl;

                if (WebConfigSettings.AppendDateToBlogUrls)
                {
                    friendlyUrl = SiteUtils.SuggestFriendlyUrl(txtTitle.Text + "-" + DateTime.UtcNow.AddHours(timeOffset).ToString("yyyy-MM-dd"), siteSettings);
                }
                else
                {
                    friendlyUrl = SiteUtils.SuggestFriendlyUrl(txtTitle.Text, siteSettings);
                }

                txtItemUrl.Text = "~/" + friendlyUrl;
            }

            if (blog != null) 
            {
                hdnTitle.Value = txtTitle.Text; 
            }
        
		}

		private void PopulateCategories()
		{
            // Mono doesn't see this in update panel
            // so help find it
            if (chkCategories == null)
            {
                log.Error("chkCategories was null");

                chkCategories = (CheckBoxList)UpdatePanel1.FindControl("chkCategories");
            }
            
            if (showCategories)
            {
                chkCategories.Items.Clear();
                IDataReader reader;
                using (reader = Blog.GetCategoriesList(this.moduleId))
                {
                    while (reader.Read())
                    {
                        ListItem listItem = new ListItem();
                        listItem.Text = reader["Category"].ToString();
                        listItem.Value = reader["CategoryID"].ToString();
                        chkCategories.Items.Add(listItem);
                    }
                }

                if (this.itemId > -1)
                {
                    using (reader = Blog.GetItemCategories(this.itemId))
                    {
                        while (reader.Read())
                        {
                            ListItem item = chkCategories.Items.FindByValue(reader["CategoryID"].ToString());
                            if (item != null)
                            {
                                item.Selected = true;
                            }
                        }
                    }
                }

            }
            
		}

		protected void btnAddCategory_Click(object sender, EventArgs e)
		{
            
            if (this.txtCategory.Text.Length > 0)
            {
                int newCategoryId = Blog.AddBlogCategory(this.moduleId, this.txtCategory.Text);
                if (this.itemId > -1)
                {
                    Blog.AddItemCategory(this.itemId, newCategoryId);
                }

                PopulateCategories();
                ListItem item = chkCategories.Items.FindByValue(newCategoryId.ToString(CultureInfo.InvariantCulture));
                if (item != null)
                {
                    item.Selected = true;
                }

                this.txtCategory.Text = string.Empty;
                UpdatePanel1.Update();
                
            }
            
		}

        protected virtual void btnUpdate_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (Page.IsValid) 
			{
                Save();

                if (hdnReturnUrl.Value.Length > 0)
                {
                    WebUtils.SetupRedirect(this, hdnReturnUrl.Value);
                    return;
                }

                WebUtils.SetupRedirect(this, SiteUtils.GetCurrentPageUrl());
			}

		}

        void btnSaveAndPreview_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                Save();

        
                WebUtils.SetupRedirect(this, SiteRoot + blog.ItemUrl.Replace("~/","/"));
            }

        }

        private void Save()
        {
            if (blog == null) { blog = new Blog(itemId); }
            Module module = new Module(moduleId);
            SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
            blog.UserGuid = siteUser.UserGuid;
            blog.LastModUserGuid = siteUser.UserGuid;
            blog.ContentChanged += new ContentChangedEventHandler(blog_ContentChanged);

            blog.ModuleId = moduleId;
            blog.ModuleGuid = module.ModuleGuid;
            blog.StartDate = DateTime.Parse(dpBeginDate.Text).AddHours(-timeOffset);
            blog.Title = txtTitle.Text;
            blog.Location = txtLocation.Text;
            blog.Description = edContent.Text;
            blog.Excerpt = edExcerpt.Text;
            blog.UserName = Context.User.Identity.Name;
            blog.IncludeInFeed = this.chkIncludeInFeed.Checked;
            blog.IsPublished = chkIsPublished.Checked;
            int allowComentsForDays = -1;
            int.TryParse(ddCommentAllowedForDays.SelectedValue, out allowComentsForDays);
            blog.AllowCommentsForDays = allowComentsForDays;
            blog.MetaDescription = txtMetaDescription.Text;
            blog.MetaKeywords = txtMetaKeywords.Text;


            String friendlyUrlString = txtItemUrl.Text.Replace("~/", String.Empty);
            FriendlyUrl friendlyUrl = new FriendlyUrl(siteSettings.SiteId, friendlyUrlString);

            if (
                ((friendlyUrl.FoundFriendlyUrl) && (friendlyUrl.PageGuid != blog.BlogGuid))
                && (blog.ItemUrl != txtItemUrl.Text)
                )
            {
                lblError.Text = BlogResources.PageUrlInUseBlogErrorMessage;
                return;
            }

            if (!friendlyUrl.FoundFriendlyUrl)
            {
                if (WebPageInfo.IsPhysicalWebPage("~/" + friendlyUrlString))
                {
                    lblError.Text = BlogResources.PageUrlInUseBlogErrorMessage;
                    return;
                }
            }

            string oldUrl = blog.ItemUrl.Replace("~/", string.Empty);
            string newUrl = txtItemUrl.Text.Replace("~/", string.Empty);

            blog.ItemUrl = txtItemUrl.Text;
            if (enableContentVersioning)
            {
                blog.CreateHistory(siteSettings.SiteGuid);
            }
            blog.Save();

            if (!friendlyUrl.FoundFriendlyUrl)
            {
                if ((friendlyUrlString.Length > 0)&&(!WebPageInfo.IsPhysicalWebPage("~/" + friendlyUrlString)))
                {
                    FriendlyUrl newFriendlyUrl = new FriendlyUrl();
                    newFriendlyUrl.SiteId = siteSettings.SiteId;
                    newFriendlyUrl.SiteGuid = siteSettings.SiteGuid;
                    newFriendlyUrl.PageGuid = blog.BlogGuid;
                    newFriendlyUrl.Url = friendlyUrlString;
                    newFriendlyUrl.RealUrl = "~/Blog/ViewPost.aspx?pageid="
                        + pageId.ToString(CultureInfo.InvariantCulture)
                        + "&mid=" + blog.ModuleId.ToString(CultureInfo.InvariantCulture)
                        + "&ItemID=" + blog.ItemId.ToString(CultureInfo.InvariantCulture);

                    newFriendlyUrl.Save();
                }

                //if post was renamed url will change, if url changes we need to redirect from the old url to the new with 301
                if ((oldUrl.Length > 0) && (newUrl.Length > 0) && (!SiteUtils.UrlsMatch(oldUrl, newUrl)))
                {
                    //worry about the risk of a redirect loop if the page is restored to the old url again
                    // don't create it if a redirect for the new url exists
                    if (
                        (!RedirectInfo.Exists(siteSettings.SiteId, oldUrl))
                        && (!RedirectInfo.Exists(siteSettings.SiteId, newUrl))
                        )
                    {
                        RedirectInfo redirect = new RedirectInfo();
                        redirect.SiteGuid = siteSettings.SiteGuid;
                        redirect.SiteId = siteSettings.SiteId;
                        redirect.OldUrl = oldUrl;
                        redirect.NewUrl = newUrl;
                        redirect.Save();
                    }
                    // since we have created a redirect we don't need the old friendly url
                    FriendlyUrl oldFriendlyUrl = new FriendlyUrl(siteSettings.SiteId, oldUrl);
                    if ((oldFriendlyUrl.FoundFriendlyUrl) && (oldFriendlyUrl.PageGuid == blog.BlogGuid))
                    {
                        FriendlyUrl.DeleteUrl(oldFriendlyUrl.UrlId);
                    }

                }
            }

            // new item posted so ping services
            if ((itemId == -1) && (blog.IsPublished) && (blog.StartDate <= DateTime.UtcNow))
            {
                QueuePings();
            }

            CurrentPage.UpdateLastModifiedTime();

            String blogFriendlyUrl = "blog" + blog.ModuleId.ToInvariantString() + "rss.aspx";
            if (!FriendlyUrl.Exists(siteSettings.SiteId, blogFriendlyUrl))
            {
                FriendlyUrl rssUrl = new FriendlyUrl();
                rssUrl.SiteId = siteSettings.SiteId;
                rssUrl.SiteGuid = siteSettings.SiteGuid;
                rssUrl.PageGuid = blog.ModuleGuid;
                rssUrl.Url = blogFriendlyUrl;
                rssUrl.RealUrl = "~/Blog/RSS.aspx?pageid=" + pageId.ToInvariantString()
                    + "&mid=" + blog.ModuleId.ToInvariantString();
                rssUrl.Save();
            }

            Blog.DeleteItemCategories(blog.ItemId);

            // Mono doesn't see this in update panel
            // so help find it
            if (chkCategories == null)
            {
                log.Error("chkCategories was null");

                chkCategories = (CheckBoxList)UpdatePanel1.FindControl("chkCategories");
            }

            foreach (ListItem listItem in this.chkCategories.Items)
            {
                if (listItem.Selected)
                {
                    Int32 categoryId;
                    if (Int32.TryParse(listItem.Value, out categoryId))
                    {
                        Blog.AddItemCategory(blog.ItemId, categoryId);
                    }
                }

            }

            CacheHelper.TouchCacheDependencyFile(cacheDependencyKey);
            SiteUtils.QueueIndexing();

        }

        void blog_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            IndexBuilderProvider indexBuilder = IndexBuilderManager.Providers["BlogIndexBuilderProvider"];
            if (indexBuilder != null)
            {
                indexBuilder.ContentChangedHandler(sender, e);
            }
        }

        #region Meta Data

        private void BindMeta()
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            List<ContentMeta> meta = metaRepository.FetchByContent(blog.BlogGuid);
            grdContentMeta.DataSource = meta;
            grdContentMeta.DataBind();

            btnAddMeta.Visible = true;
        }

        void grdContentMeta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            GridView grid = (GridView)sender;
            string sGuid = e.CommandArgument.ToString();
            if (sGuid.Length != 36) { return; }

            Guid guid = new Guid(sGuid);
            ContentMeta meta = metaRepository.Fetch(guid);
            if (meta == null) { return; }

            switch (e.CommandName)
            {
                case "MoveUp":
                    meta.SortRank -= 3;
                    break;

                case "MoveDown":
                    meta.SortRank += 3;
                    break;
            }

            metaRepository.Save(meta);
            List<ContentMeta> metaList = metaRepository.FetchByContent(blog.BlogGuid);
            metaRepository.ResortMeta(metaList);

            blog.CompiledMeta = metaRepository.GetMetaString(blog.BlogGuid);
            blog.Save();

            BindMeta();
            upMeta.Update();


        }

        void grdContentMeta_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            GridView grid = (GridView)sender;
            Guid guid = new Guid(grid.DataKeys[e.RowIndex].Value.ToString());
            metaRepository.Delete(guid);

            blog.CompiledMeta = metaRepository.GetMetaString(blog.BlogGuid);
            blog.Save();
            grdContentMeta.Columns[2].Visible = true;
            BindMeta();
            upMeta.Update();
        }

        void grdContentMeta_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView grid = (GridView)sender;
            grid.EditIndex = e.NewEditIndex;

            BindMeta();

            Button btnDeleteMeta = (Button)grid.Rows[e.NewEditIndex].Cells[1].FindControl("btnDeleteMeta");
            if (btnDeleteMeta != null)
            {
                btnDelete.Attributes.Add("OnClick", "return confirm('"
                    + BlogResources.ContentMetaDeleteWarning + "');");
            }

            upMeta.Update();
        }

        void grdContentMeta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grid = (GridView)sender;
            if (grid.EditIndex > -1)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList ddDirection = (DropDownList)e.Row.Cells[1].FindControl("ddDirection");
                    if (ddDirection != null)
                    {
                        if (e.Row.DataItem is ContentMeta)
                        {
                            ListItem item = ddDirection.Items.FindByValue(((ContentMeta)e.Row.DataItem).Dir);
                            if (item != null)
                            {
                                ddDirection.ClearSelection();
                                item.Selected = true;
                            }
                        }
                    }

                    if (!(e.Row.DataItem is ContentMeta))
                    {
                        //the add button was clicked so hide the delete button
                        Button btnDeleteMeta = (Button)e.Row.Cells[1].FindControl("btnDeleteMeta");
                        if (btnDeleteMeta != null) { btnDeleteMeta.Visible = false; }
                    }
                }
            }
        }

        void grdContentMeta_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            GridView grid = (GridView)sender;

            Guid guid = new Guid(grid.DataKeys[e.RowIndex].Value.ToString());
            TextBox txtName = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtName");
            TextBox txtScheme = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtScheme");
            TextBox txtLangCode = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtLangCode");
            DropDownList ddDirection = (DropDownList)grid.Rows[e.RowIndex].Cells[1].FindControl("ddDirection");
            TextBox txtMetaContent = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtMetaContent");
            SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
            ContentMeta meta = null;
            if (guid != Guid.Empty)
            {
                meta = metaRepository.Fetch(guid);
            }
            else
            {
                meta = new ContentMeta();
                Module module = new Module(moduleId);
                meta.ModuleGuid = module.ModuleGuid;
                if (siteUser != null) { meta.CreatedBy = siteUser.UserGuid; }
                meta.SortRank = metaRepository.GetNextSortRank(blog.BlogGuid);
            }

            if (meta != null)
            {
                meta.SiteGuid = siteSettings.SiteGuid;
                meta.ContentGuid = blog.BlogGuid;
                meta.Dir = ddDirection.SelectedValue;
                meta.LangCode = txtLangCode.Text;
                meta.MetaContent = txtMetaContent.Text;
                meta.Name = txtName.Text;
                meta.Scheme = txtScheme.Text;
                if (siteUser != null) { meta.LastModBy = siteUser.UserGuid; }
                metaRepository.Save(meta);

                blog.CompiledMeta = metaRepository.GetMetaString(blog.BlogGuid);
                blog.Save();

            }

            grid.EditIndex = -1;
            grdContentMeta.Columns[2].Visible = true;
            BindMeta();
            upMeta.Update();

        }

        void grdContentMeta_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdContentMeta.EditIndex = -1;
            grdContentMeta.Columns[2].Visible = true;
            BindMeta();
            upMeta.Update();
        }

        void btnAddMeta_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Guid", typeof(Guid));
            dataTable.Columns.Add("SiteGuid", typeof(Guid));
            dataTable.Columns.Add("ModuleGuid", typeof(Guid));
            dataTable.Columns.Add("ContentGuid", typeof(Guid));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Scheme", typeof(string));
            dataTable.Columns.Add("LangCode", typeof(string));
            dataTable.Columns.Add("Dir", typeof(string));
            dataTable.Columns.Add("MetaContent", typeof(string));
            dataTable.Columns.Add("SortRank", typeof(int));

            DataRow row = dataTable.NewRow();
            row["Guid"] = Guid.Empty;
            row["SiteGuid"] = siteSettings.SiteGuid;
            row["ModuleGuid"] = Guid.Empty;
            row["ContentGuid"] = Guid.Empty;
            row["Name"] = string.Empty;
            row["Scheme"] = string.Empty;
            row["LangCode"] = string.Empty;
            row["Dir"] = string.Empty;
            row["MetaContent"] = string.Empty;
            row["SortRank"] = 3;

            dataTable.Rows.Add(row);

            grdContentMeta.EditIndex = 0;
            grdContentMeta.DataSource = dataTable.DefaultView;
            grdContentMeta.DataBind();
            grdContentMeta.Columns[2].Visible = false;
            btnAddMeta.Visible = false;

            upMeta.Update();

        }

        private void BindMetaLinks()
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            List<ContentMetaLink> meta = metaRepository.FetchLinksByContent(blog.BlogGuid);

            grdMetaLinks.DataSource = meta;
            grdMetaLinks.DataBind();

            btnAddMetaLink.Visible = true;
        }

        void btnAddMetaLink_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Guid", typeof(Guid));
            dataTable.Columns.Add("SiteGuid", typeof(Guid));
            dataTable.Columns.Add("ModuleGuid", typeof(Guid));
            dataTable.Columns.Add("ContentGuid", typeof(Guid));
            dataTable.Columns.Add("Rel", typeof(string));
            dataTable.Columns.Add("Href", typeof(string));
            dataTable.Columns.Add("HrefLang", typeof(string));
            dataTable.Columns.Add("SortRank", typeof(int));

            DataRow row = dataTable.NewRow();
            row["Guid"] = Guid.Empty;
            row["SiteGuid"] = siteSettings.SiteGuid;
            row["ModuleGuid"] = Guid.Empty;
            row["ContentGuid"] = Guid.Empty;
            row["Rel"] = string.Empty;
            row["Href"] = string.Empty;
            row["HrefLang"] = string.Empty;
            row["SortRank"] = 3;

            dataTable.Rows.Add(row);

            grdMetaLinks.Columns[2].Visible = false;
            grdMetaLinks.EditIndex = 0;
            grdMetaLinks.DataSource = dataTable.DefaultView;
            grdMetaLinks.DataBind();
            btnAddMetaLink.Visible = false;

            updMetaLinks.Update();
        }

        void grdMetaLinks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grid = (GridView)sender;
            if (grid.EditIndex > -1)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (!(e.Row.DataItem is ContentMetaLink))
                    {
                        //the add button was clicked so hide the delete button
                        Button btnDeleteMetaLink = (Button)e.Row.Cells[1].FindControl("btnDeleteMetaLink");
                        if (btnDeleteMetaLink != null) { btnDeleteMetaLink.Visible = false; }

                    }

                }

            }
        }

        void grdMetaLinks_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            GridView grid = (GridView)sender;
            Guid guid = new Guid(grid.DataKeys[e.RowIndex].Value.ToString());
            metaRepository.DeleteLink(guid);

            blog.CompiledMeta = metaRepository.GetMetaString(blog.BlogGuid);
            blog.Save();

            grid.Columns[2].Visible = true;
            BindMetaLinks();

            updMetaLinks.Update();
        }

        void grdMetaLinks_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdMetaLinks.EditIndex = -1;
            grdMetaLinks.Columns[2].Visible = true;
            BindMetaLinks();
            updMetaLinks.Update();
        }

        void grdMetaLinks_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            GridView grid = (GridView)sender;

            Guid guid = new Guid(grid.DataKeys[e.RowIndex].Value.ToString());
            TextBox txtRel = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtRel");
            TextBox txtHref = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtHref");
            TextBox txtHrefLang = (TextBox)grid.Rows[e.RowIndex].Cells[1].FindControl("txtHrefLang");
            SiteUser currentUser = SiteUtils.GetCurrentSiteUser();
            ContentMetaLink meta = null;
            if (guid != Guid.Empty)
            {
                meta = metaRepository.FetchLink(guid);
            }
            else
            {
                meta = new ContentMetaLink();
                Module module = new Module(moduleId);
                meta.ModuleGuid = module.ModuleGuid;
                if (currentUser != null) { meta.CreatedBy = currentUser.UserGuid; }
                meta.SortRank = metaRepository.GetNextLinkSortRank(blog.BlogGuid);
            }

            if (meta != null)
            {
                meta.SiteGuid = siteSettings.SiteGuid;
                meta.ContentGuid = blog.BlogGuid;
                meta.Rel = txtRel.Text;
                meta.Href = txtHref.Text;
                meta.HrefLang = txtHrefLang.Text;

                if (currentUser != null) { meta.LastModBy = currentUser.UserGuid; }
                metaRepository.Save(meta);

                blog.CompiledMeta = metaRepository.GetMetaString(blog.BlogGuid);
                blog.Save();

            }

            grid.EditIndex = -1;
            grdMetaLinks.Columns[2].Visible = true;
            BindMetaLinks();
            updMetaLinks.Update();
        }

        void grdMetaLinks_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView grid = (GridView)sender;
            grid.EditIndex = e.NewEditIndex;

            BindMetaLinks();

            Guid guid = new Guid(grid.DataKeys[grid.EditIndex].Value.ToString());

            Button btnDelete = (Button)grid.Rows[e.NewEditIndex].Cells[1].FindControl("btnDeleteMetaLink");
            if (btnDelete != null)
            {
                btnDelete.Attributes.Add("OnClick", "return confirm('"
                    + BlogResources.ContentMetaLinkDeleteWarning + "');");

                if (guid == Guid.Empty) { btnDelete.Visible = false; }
            }

            updMetaLinks.Update();
        }

        void grdMetaLinks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (blog == null) { return; }
            if (blog.BlogGuid == Guid.Empty) { return; }

            GridView grid = (GridView)sender;
            string sGuid = e.CommandArgument.ToString();
            if (sGuid.Length != 36) { return; }

            Guid guid = new Guid(sGuid);
            ContentMetaLink meta = metaRepository.FetchLink(guid);
            if (meta == null) { return; }

            switch (e.CommandName)
            {
                case "MoveUp":
                    meta.SortRank -= 3;
                    break;

                case "MoveDown":
                    meta.SortRank += 3;
                    break;

            }

            metaRepository.Save(meta);
            List<ContentMetaLink> metaList = metaRepository.FetchLinksByContent(blog.BlogGuid);
            metaRepository.ResortMeta(metaList);

            blog.CompiledMeta = metaRepository.GetMetaString(blog.BlogGuid);
            blog.Save();

            BindMetaLinks();
            updMetaLinks.Update();
        }


        #endregion

        #region History

        private void BindHistory()
        {
            if (!enableContentVersioning) { return; }

            if ((blog == null)||(blog.ItemId == -1))
            {
                pnlHistory.Visible = false;
                return;
            }

            List<ContentHistory> history = ContentHistory.GetPage(blog.BlogGuid, pageNumber, pageSize, out totalPages);

            pgrHistory.ShowFirstLast = true;
            pgrHistory.PageSize = pageSize;
            pgrHistory.PageCount = totalPages;
            pgrHistory.Visible = (this.totalPages > 1);
           
            grdHistory.DataSource = history;
            grdHistory.DataBind();

            btnDeleteHistory.Visible = (grdHistory.Rows.Count > 0);
            grdHistory.Visible = (grdHistory.Rows.Count > 0);

        }

        void pgrHistory_Command(object sender, CommandEventArgs e)
        {
            pageNumber = Convert.ToInt32(e.CommandArgument);
            pgrHistory.CurrentIndex = pageNumber;
            BindHistory();
        }

        void grdHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string g = e.CommandArgument.ToString();
            if (g.Length != 36) { return; }
            Guid historyGuid = new Guid(g);

            switch (e.CommandName)
            {
                case "RestoreToEditor":
                    ContentHistory history = new ContentHistory(historyGuid);
                    if (history.Guid == Guid.Empty) { return; }

                    edContent.Text = history.ContentText;
                    BindHistory();
                    break;

                case "DeleteHistory":
                    ContentHistory.Delete(historyGuid);
                    BindHistory();
                    break;

                default:

                    break;
            }
        }

        void grdHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            Button btnDelete = (Button)e.Row.Cells[0].FindControl("btnDelete");
            if (btnDelete != null)
            {
                btnDelete.Attributes.Add("OnClick", "return confirm('"
                    + BlogResources.DeleteHistoryItemWarning + "');");
            }

        }

        void btnRestoreFromGreyBox_Click(object sender, ImageClickEventArgs e)
        {
            if (hdnHxToRestore.Value.Length != 36)
            {
                BindHistory();
                return;
            }

            Guid h = new Guid(hdnHxToRestore.Value);

            ContentHistory history = new ContentHistory(h);
            if (history.Guid == Guid.Empty) { return; }

            edContent.Text = history.ContentText;
            BindHistory();

        }

        void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            if (blog == null) { return; }

            ContentHistory.DeleteByContent(blog.BlogGuid);
            BindHistory();

        }

        #endregion

        private void DoPings(object pingersList)
        {

            if (!(pingersList is List<ServicePinger>)) return;

            List<ServicePinger> pingers = pingersList as List<ServicePinger>;
            foreach (ServicePinger pinger in pingers)
            {
                pinger.Ping();
            }

        }

        protected virtual void QueuePings()
        {
            // TODO: implement more generic support with lookup of pingable services

            if (OdiogoFeedIDSetting.Length == 0) return;

            string odogioRpcUrl = "http://rpc.odiogo.com/ping/";
            ServicePinger pinger = new ServicePinger(
                siteSettings.SiteName,
                SiteRoot,
                odogioRpcUrl);

            List<ServicePinger> pingers = new List<ServicePinger>();
            pingers.Add(pinger);
            

            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(DoPings), pingers))
            {
                throw new Exception("Couldn't queue the DoPings on a new thread.");
            }


        }



        protected void btnCancel_Click(object sender, EventArgs e)
		{
            if (hdnReturnUrl.Value.Length > 0)
            {
                WebUtils.SetupRedirect(this, hdnReturnUrl.Value);
                return;
            }

            WebUtils.SetupRedirect(this, SiteUtils.GetCurrentPageUrl());

            return;

		}

        protected void btnDelete_Click(object sender, EventArgs e)
		{
			if(blog != null)
			{
                //Blog blog = new Blog(itemId);
                blog.ContentChanged += new ContentChangedEventHandler(blog_ContentChanged);
                blog.Delete();
                FriendlyUrl.DeleteByPageGuid(blog.BlogGuid);
                CurrentPage.UpdateLastModifiedTime();
                SiteUtils.QueueIndexing();
			}

            if (hdnReturnUrl.Value.Length > 0)
            {
                WebUtils.SetupRedirect(this, hdnReturnUrl.Value);
                return;
            }

            WebUtils.SetupRedirect(this, SiteUtils.GetCurrentPageUrl());

            return;
		}

        private void PopulateCommentDaysDropdown()
        {
            ListItem item = ddCommentAllowedForDays.Items.FindByValue(defaultCommentDaysAllowed);
            if (item != null)
            {
                ddCommentAllowedForDays.ClearSelection();
                item.Selected = true;
            }  
        }

        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, BlogResources.EditPostPageTitle);

            litContentTab.Text = BlogResources.ContentTab;
            litExcerptTab.Text = BlogResources.ExcerptTab;
            litMetaTab.Text = BlogResources.MetaTab;

            lnkExcerpt.HRef = "#" + tabExcerpt.ClientID;

            liExcerpt.Visible = useExcerpt;
            tabExcerpt.Visible = useExcerpt;

            if (!Page.IsPostBack)
            {
                PopulateCommentDaysDropdown();
            }
            
            edContent.WebEditor.ToolBar = ToolBar.FullWithTemplates;
            edExcerpt.WebEditor.ToolBar = ToolBar.Full;
           
            this.lnkEditCategories.NavigateUrl = SiteRoot + "/Blog/EditCategory.aspx?pageid=" + CurrentPage.PageId.ToString()
                + "&mid=" + this.moduleId.ToString();

            this.lnkEditCategories.Text = BlogResources.BlogEditCategoriesLabel;
            
            if (moduleSettings.Contains("BlogEditorHeightSetting"))
            {
                edContent.WebEditor.Height = Unit.Parse(moduleSettings["BlogEditorHeightSetting"].ToString());
                edExcerpt.WebEditor.Height = Unit.Parse(moduleSettings["BlogEditorHeightSetting"].ToString());
            }

            btnUpdate.Text = BlogResources.BlogEditUpdateButton;
            SiteUtils.SetButtonAccessKey(btnUpdate, BlogResources.BlogEditUpdateButtonAccessKey);
            btnUpdate2.Text = BlogResources.BlogEditUpdateButton;
            btnUpdate3.Text = BlogResources.BlogEditUpdateButton;
            btnSaveAndPreview.Text = BlogResources.SaveAndPreviewButton;

            UIHelper.DisableButtonAfterClick(
                btnUpdate,
                BlogResources.ButtonDisabledPleaseWait,
                Page.ClientScript.GetPostBackEventReference(this.btnUpdate, string.Empty)
                );

            UIHelper.DisableButtonAfterClick(
                btnUpdate2,
                BlogResources.ButtonDisabledPleaseWait,
                Page.ClientScript.GetPostBackEventReference(this.btnUpdate2, string.Empty)
                );

            UIHelper.DisableButtonAfterClick(
                btnUpdate3,
                BlogResources.ButtonDisabledPleaseWait,
                Page.ClientScript.GetPostBackEventReference(this.btnUpdate3, string.Empty)
                );

            UIHelper.DisableButtonAfterClick(
                btnSaveAndPreview,
                BlogResources.ButtonDisabledPleaseWait,
                Page.ClientScript.GetPostBackEventReference(btnSaveAndPreview, string.Empty)
                );

            lnkCancel.Text = BlogResources.BlogEditCancelButton;
            lnkCancel2.Text = BlogResources.BlogEditCancelButton;
            lnkCancel3.Text = BlogResources.BlogEditCancelButton;
            btnDelete.Text = BlogResources.BlogEditDeleteButton;
            btnDelete2.Text = BlogResources.BlogEditDeleteButton;
            btnDelete3.Text = BlogResources.BlogEditDeleteButton;
            SiteUtils.SetButtonAccessKey(btnDelete, BlogResources.BlogEditDeleteButtonAccessKey);
            UIHelper.AddConfirmationDialog(btnDelete, BlogResources.BlogDeletePostWarning);
            UIHelper.AddConfirmationDialog(btnDelete2, BlogResources.BlogDeletePostWarning);
            UIHelper.AddConfirmationDialog(btnDelete3, BlogResources.BlogDeletePostWarning);

            btnAddCategory.Text = BlogResources.BlogAddCategoryButton;
            SiteUtils.SetButtonAccessKey(btnAddCategory, BlogResources.BlogAddCategoryButtonAccessKey);

            reqTitle.ErrorMessage = BlogResources.TitleRequiredWarning;
            reqStartDate.ErrorMessage = BlogResources.BlogBeginDateRequiredHelp;
            this.dpBeginDate.ClockHours = ConfigurationManager.AppSettings["ClockHours"];
            regexUrl.ErrorMessage = BlogResources.FriendlyUrlRegexWarning;

            //if (!showCategories)
            //{
            //    pnlCategories.Visible = false;
            //}

            litDays.Text = BlogResources.BlogEditCommentsDaysLabel;

            grdHistory.Columns[0].HeaderText = BlogResources.CreatedDateGridHeader;
            grdHistory.Columns[1].HeaderText = BlogResources.ArchiveDateGridHeader;

            btnRestoreFromGreyBox.ImageUrl = Page.ResolveUrl("~/Data/SiteImages/1x1.gif");
            btnRestoreFromGreyBox.AlternateText = " ";

            btnDeleteHistory.Text = BlogResources.DeleteAllHistoryButton;
            UIHelper.AddConfirmationDialog(btnDeleteHistory, BlogResources.DeleteAllHistoryWarning);

            btnAddMeta.Text = BlogResources.AddMetaButton;
            grdContentMeta.Columns[0].HeaderText = string.Empty;
            grdContentMeta.Columns[1].HeaderText = BlogResources.ContentMetaNameLabel;
            grdContentMeta.Columns[2].HeaderText = BlogResources.ContentMetaMetaContentLabel;

            btnAddMetaLink.Text = BlogResources.AddMetaLinkButton;

            grdMetaLinks.Columns[0].HeaderText = string.Empty;
            grdMetaLinks.Columns[1].HeaderText = BlogResources.ContentMetaRelLabel;
            grdMetaLinks.Columns[2].HeaderText = BlogResources.ContentMetaMetaHrefLabel;
            
        }

        private void LoadSettings()
        {
            if ((WebUser.IsAdminOrContentAdmin) || (SiteUtils.UserIsSiteEditor())) { isAdmin = true; }

            Hashtable moduleSettings = ModuleSettings.GetModuleSettings(moduleId);

            lnkCancel.NavigateUrl = SiteUtils.GetCurrentPageUrl();

            showCategories = WebUtils.ParseBoolFromHashtable(
                moduleSettings, "BlogShowCategoriesSetting", false);

            useExcerpt = WebUtils.ParseBoolFromHashtable(
                moduleSettings, "BlogUseExcerptSetting", useExcerpt);

            if (moduleSettings.Contains("BlogCommentForDaysDefault"))
            {
                defaultCommentDaysAllowed = moduleSettings["BlogCommentForDaysDefault"].ToString();
            }

            pageSize = WebUtils.ParseInt32FromHashtable(moduleSettings, "BlogVersionPageSizeSetting", pageSize);
            enableContentVersioning = WebUtils.ParseBoolFromHashtable(moduleSettings, "BlogEnableVersioningSetting", BlogEnableVersioningSetting);

            if ((siteSettings.ForceContentVersioning) || (WebConfigSettings.EnforceContentVersioningGlobally))
            {
                enableContentVersioning = true;
            }

            if (itemId > -1)
            {
                blog = new Blog(itemId);
                if (blog.ModuleId != moduleId) { blog = null; }
            }


            pnlMetaData.Visible = (blog != null);
            

            divHistoryDelete.Visible = (enableContentVersioning && isAdmin);

            pnlHistory.Visible = enableContentVersioning;

            if (enableContentVersioning)
            {
                SetupHistoryRestoreScript();
            }

            try
            {
                // this keeps the action from changing during ajax postback in folder based sites
                SiteUtils.SetFormAction(Page, Request.RawUrl);
            }
            catch (MissingMethodException)
            { 
                //this method was introduced in .NET 3.5 SP1
            }
        }

        private void SetupHistoryRestoreScript()
        {
            StringBuilder script = new StringBuilder();

            script.Append("\n<script type='text/javascript'>");
            script.Append("function LoadHistoryInEditor(hxGuid) {");

            script.Append("GB_hide();");
            //script.Append("alert(hxGuid);");

            script.Append("var hdn = document.getElementById('" + this.hdnHxToRestore.ClientID + "'); ");
            script.Append("hdn.value = hxGuid; ");
            script.Append("var btn = document.getElementById('" + this.btnRestoreFromGreyBox.ClientID + "');  ");
            script.Append("btn.click(); ");
            script.Append("}");
            script.Append("</script>");


            Page.ClientScript.RegisterStartupScript(typeof(Page), "gbHandler", script.ToString());

        }

        private void LoadParams()
        {
            timeOffset = SiteUtils.GetUserTimeOffset();
            pageId = WebUtils.ParseInt32FromQueryString("pageid", -1);
            moduleId = WebUtils.ParseInt32FromQueryString("mid", -1);
            itemId = WebUtils.ParseInt32FromQueryString("ItemID", -1);
            restoreGuid = WebUtils.ParseGuidFromQueryString("r", restoreGuid);
            cacheDependencyKey = "Module-" + moduleId.ToString(CultureInfo.InvariantCulture);
            virtualRoot = WebUtils.GetApplicationRoot();

            if (moduleId > -1)
            {
                moduleSettings = ModuleSettings.GetModuleSettings(moduleId);

                if (moduleSettings.Contains("OdiogoFeedIDSetting"))
                    OdiogoFeedIDSetting = moduleSettings["OdiogoFeedIDSetting"].ToString();
            }


        }

        private void SetupScripts()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("sarissa"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sarissa", "<script src=\""
                    + ResolveUrl("~/ClientScript/sarissa/sarissa.js") + "\" type=\"text/javascript\"></script>");
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("sarissa_ieemu_xpath"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sarissa_ieemu_xpath", "<script src=\""
                    + ResolveUrl("~/ClientScript/sarissa/sarissa_ieemu_xpath.js") + "\" type=\"text/javascript\"></script>");
            }

            
            if (!Page.ClientScript.IsClientScriptBlockRegistered("friendlyurlsuggest"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "friendlyurlsuggest", "<script src=\""
                    + ResolveUrl("~/ClientScript/friendlyurlsuggest_v2.js") + "\" type=\"text/javascript\"></script>");
            }

            string focusScript = string.Empty;
            if (itemId == -1) { focusScript = "document.getElementById('" + this.txtTitle.ClientID + "').focus();"; }

            string hookupInputScript = "<script type=\"text/javascript\">"
                + "new UrlHelper( "
                + "document.getElementById('" + this.txtTitle.ClientID + "'),  "
                + "document.getElementById('" + this.txtItemUrl.ClientID + "'), "
                + "document.getElementById('" + this.hdnTitle.ClientID + "'), "
                + "document.getElementById('" + this.spnUrlWarning.ClientID + "'), "
                + "\"" + SiteRoot + "/Blog/BlogUrlSuggestService.ashx" + "\""
                + "); " + focusScript + "</script>";

            if (!Page.ClientScript.IsStartupScriptRegistered(this.UniqueID + "urlscript"))
            {
                this.Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    this.UniqueID + "urlscript", hookupInputScript);
            }
            

        }

		
	}
}
