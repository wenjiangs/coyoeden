using System;
#if !MONO
using System.Web.UI.WebControls.WebParts;
#endif
using System.Web.UI;
using Cynthia.Business;
using Cynthia.Web.UI;

namespace Cynthia.Web
{
    public partial class ModuleWrapper : SiteModuleControl
    {
        private bool moduleLoaded = false;
        private int configureModuleID = -1;
#if !MONO
        [Personalizable(PersonalizationScope.User)]
        [WebBrowsable]
        [WebDisplayName("Enter a module id")]
        [WebDescription("Enter a module id")]
#endif
        public int ConfigureModuleId
        {
            get { return this.configureModuleID;}
            set 
            {
                // this can throw an error when set during page pre-init
                try
                {
                    this.configureModuleID = value;
                    LoadModule();
                }
                catch (NullReferenceException) { }
            }
        }
        

        protected void Page_Load(object sender, EventArgs e)
        {
            // don't render on admin or edit pages
            if(!(this.Page is CmsPage)) { this.Visible = false; return; }

            Description = "Module Wrapper";
            if (ConfigureModuleId > -1)
            {
                if (!moduleLoaded)
                {
                    LoadModule();
                }

            }

        }

        protected void LoadModule()
        {
            if (configureModuleID > -1)
            {
                this.Controls.Clear();
                Module module = new Module(ConfigureModuleId);

                //SiteModuleControl siteModule =
                //    Page.LoadControl("~/" + module.ControlSource) as SiteModuleControl;

                Control c = Page.LoadControl("~/" + module.ControlSource);
                if (c == null) 
                {
                    throw new ArgumentException("Unable to load control from ~/" + module.ControlSource);
                }

                if (c is SiteModuleControl)
                {
                    SiteModuleControl siteModule = (SiteModuleControl)c;
                    siteModule.SiteId = siteSettings.SiteId;
                    siteModule.ModuleConfiguration = module;
                    this.Title = module.ModuleTitle;
                    
                }

                this.Controls.Add(c);
                moduleLoaded = true;
            }

        }

    }
}
