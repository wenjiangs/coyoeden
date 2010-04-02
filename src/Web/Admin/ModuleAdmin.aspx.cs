/// Author:					Joe Audette
/// Created:				2004-07-21
/// Last Modified:			2009-06-07
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using Cynthia.Business;
using Cynthia.Business.WebHelpers;
using Cynthia.Web.Framework;
using Resources;

namespace Cynthia.Web.AdminUI
{
    public partial class ModuleAdminPage : CBasePage
    {
        //protected string EditLabel = string.Empty;
        //protected string SettingsLabel = string.Empty;

        private bool IsAdmin = false;
        protected string EditPropertiesImage = WebConfigSettings.EditPropertiesImage;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSettings();

            if (!IsAdmin)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/AccessDenied.aspx");
                return;
            }

            if (!siteSettings.IsServerAdminSite)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Admin/AdminMenu.aspx");
                return;
            }
            LoadSettings();
            PopulateLabels();
            PopulateControls();

        }

        private void PopulateControls()
        {
            if (Page.IsPostBack) return;

            using (IDataReader reader = ModuleDefinition.GetModuleDefinitions(siteSettings.SiteGuid))
            {
                defsList.DataSource = reader;
                defsList.DataBind();
            }

        }

        

        private void DefsList_ItemCommand(object sender, DataListCommandEventArgs e)
        {
            // TODO: why not make this a link instead of
            // postback then redirect? JA

            int moduleDefID = (int)defsList.DataKeys[e.Item.ItemIndex];
            string redirectUrl
                = SiteRoot + "/Admin/ModuleDefinitions.aspx?defId="
                + moduleDefID.ToString(CultureInfo.InvariantCulture);

            WebUtils.SetupRedirect(this, redirectUrl);

        }


        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, Resource.AdminMenuFeatureModulesLink);

            lnkAdminMenu.Text = Resource.AdminMenuLink;
            lnkAdminMenu.NavigateUrl = SiteRoot + "/Admin/AdminMenu.aspx";

            lnkAdvancedTools.Text = Resource.AdvancedToolsLink;
            lnkAdvancedTools.NavigateUrl = SiteRoot + "/Admin/AdvancedTools.aspx";

            lnkFeatureAdmin.Text = Resource.AdminMenuFeatureModulesLink;
            lnkFeatureAdmin.ToolTip = Resource.AdminMenuFeatureModulesLink;
            lnkFeatureAdmin.NavigateUrl = SiteRoot + "/Admin/ModuleAdmin.aspx";

            lnkNewModule.Text = Resource.ModuleDefsAddButton;
            lnkNewModule.ToolTip = Resource.ModuleDefsAddButton;
            
        }


        protected String GetEditImageUrl()
        {
            return ImageSiteRoot + "/Data/SiteImages/" + EditPropertiesImage;
        }

        private void LoadSettings()
        {
            IsAdmin = WebUser.IsAdmin;

        }


        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);
            this.defsList.ItemCommand += new DataListCommandEventHandler(this.DefsList_ItemCommand);

            SuppressMenuSelection();
            SuppressPageMenu();
            
        }

        #endregion
    }
}
