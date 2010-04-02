/// Author:					Joe Audette
/// Created:				2007-03-18
/// Last Modified:			2010-03-15
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cynthia.Business;
using Cynthia.Business.Commerce;
using Cynthia.Business.WebHelpers;
using Cynthia.Web;
using Cynthia.Web.Controls.google;
using Cynthia.Web.Framework;
using Cynthia.Web.UI;
using WebStore.Business;
using Resources;
using WebStore.Helpers;
using log4net;

namespace WebStore.UI
{
    public partial class OrderDetailPage : CBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderDetailPage));
        protected int pageId = -1;
        protected int moduleId = -1;
        protected Store store = null;
        protected Guid orderGuid = Guid.Empty;
        protected Order order = null;
        protected SiteUser siteUser = null;
        protected Double timeOffset = 0;
        private Collection<FullfillDownloadTicket> downloadTickets;
        protected CultureInfo currencyCulture;
        private DataSet dsOffers = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SiteUtils.SslIsAvailable()) { SiteUtils.ForceSsl(); }

            LoadParams();

            if (!UserCanViewPage(moduleId))
            {
                SiteUtils.RedirectToAccessDeniedPage();
                return;
            }

            LoadSettings();
           
            if (order == null)
            {
                WebUtils.SetupRedirect(this, SiteUtils.GetCurrentPageUrl());
                return;
            }

            if ((siteUser != null) && (order.UserGuid != siteUser.UserGuid))
            {
                SiteUtils.RedirectToAccessDeniedPage();
                return;
            }

            SetupCss();
            PopulateLabels();
            PopulateControls();

        }

        private void PopulateControls()
        {
            if (order == null) { return; }

            Title = SiteUtils.FormatPageTitle(siteSettings, " - " + WebStoreResources.OrderDetailHeader + " - " + order.OrderGuid.ToString());
            lblOrderId.Text = order.OrderGuid.ToString();

            litOrderDate.Text = order.Completed.AddHours(timeOffset).ToShortDateString();
            litSubTotal.Text = order.SubTotal.ToString("c", currencyCulture);
            litDiscount.Text = order.Discount.ToString("c", currencyCulture);
            litShippingTotal.Text = order.ShippingTotal.ToString("c", currencyCulture);
            litTaxTotal.Text = order.TaxTotal.ToString("c", currencyCulture);
            litOrderTotal.Text = order.OrderTotal.ToString("c", currencyCulture);

            pnlDiscount.Visible = (order.Discount > 0);
            pnlShippingTotal.Visible = (order.ShippingTotal > 0);
            pnlTaxTotal.Visible = (order.TaxTotal > 0);

            if ((order.ShippingTotal == 0) && (order.TaxTotal == 0) && (order.Discount == 0))
            {
                pnlSubTotal.Visible = false;
            }

            //using (IDataReader reader = order.GetProducts())
            //{
            //    rptOrderItems.DataSource = reader;
            //    rptOrderItems.DataBind();
            //}

            dsOffers = Order.GetOrderOffersAndProducts(store.Guid, orderGuid);
            rptOffers.DataSource = dsOffers;
            rptOffers.DataBind();
            // once payment has cleared, status will be fullfillable or fullfilled
            // pending payments are common for echeck or overseas accounts
            // lets not give them the ability to download until payment has cleared
            if (order.StatusGuid == OrderStatus.OrderStatusReceivedGuid)
            {
                litPaymentPending.Text = WebStoreResources.PaymentPendingMessage;
            }
            else
            {
                downloadTickets = order.GetDownloadTickets();
                if (downloadTickets.Count > 0)
                {
                    if ((siteUser != null) && (order.UserGuid == siteUser.UserGuid))
                    {
                        pnlDownloadItems.Visible = true;
                        rptDownloadItems.DataSource = downloadTickets;
                        rptDownloadItems.DataBind();
                    }
                    else
                    {
                        if (siteUser == null)
                        {
                            lblMustSignInToDownload.Visible = true;
                        }
                    }
                }
                
            }

            PopulateCustomerInfo();
            DoGoogleAnalyticsTracking();

        }

        private void PopulateCustomerInfo()
        {
            if (order == null) { return; }
            if (siteUser == null) { return; }
            if (siteUser.UserGuid != order.UserGuid) { return; }
            // don't show customer information in the page ifnot using ssl
            if (!Request.IsSecureConnection) { return; }

            pnlCustomer.Visible = true;
            
            litBillingName.Text = order.BillingFirstName + " " + order.BillingLastName + "<br />";

            if (order.BillingCompany.Length > 0)
            {
                litBillingCompany.Text = order.BillingCompany + "<br />";
            }

            litBillingAddress1.Text = order.BillingAddress1 + "<br />";

            if (order.BillingAddress2.Length > 0)
            {
                litBillingAddress2.Text = order.BillingAddress2 + "<br />";
            }

            if (order.BillingSuburb.Length > 0)
            {
                litBillingSuburb.Text = order.BillingSuburb + "<br />";
            }

            litBillingCity.Text = order.BillingCity + ",&nbsp;";
            litBillingState.Text = order.BillingState + "&nbsp;&nbsp;";
            litBillingPostalCode.Text = order.BillingPostalCode + "<br />";
            litBillingCountry.Text = order.BillingCountry + "<br />";

            if (order.HasShippingProducts())
            {
                pnlShippingAddress.Visible = true;

                litShippingName.Text = order.DeliveryFirstName + " " + order.DeliveryLastName + "<br />";

                if (order.DeliveryCompany.Length > 0)
                {
                    litShippingCompany.Text = order.DeliveryCompany + "<br />";
                }

                litShippingAddress1.Text = order.DeliveryAddress1 + "<br />";

                if (order.DeliveryAddress2.Length > 0)
                {
                    litShippingAddress2.Text = order.DeliveryAddress2 + "<br />";
                }

                if (order.DeliverySuburb.Length > 0)
                {
                    litShippingSuburb.Text = order.DeliverySuburb + "<br />";
                }

                litShippingCity.Text = order.DeliveryCity + ",&nbsp;";
                litShippingState.Text = order.DeliveryState + "&nbsp;&nbsp;";
                litShippingPostalCode.Text = order.DeliveryPostalCode + "<br />";
                litShippingCountry.Text = order.DeliveryCountry + "<br />";

            }

            

        }

        void rptOffers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (dsOffers == null) { return; }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string offerGuid = ((DataRowView)e.Item.DataItem).Row.ItemArray[0].ToString();
                Repeater rptProducts = (Repeater)e.Item.FindControl("rptProducts");

                if (rptProducts == null) { return; }

                string whereClause = string.Format("OfferGuid = '{0}'", offerGuid);
                DataView dv = new DataView(dsOffers.Tables["Products"], whereClause, "", DataViewRowState.CurrentRows);
                if (dv.Count > 1)
                {
                    rptProducts.DataSource = dv;
                    rptProducts.DataBind();
                }


            }
        }

        private void DoGoogleAnalyticsTracking()
        {
            if (order == null) { return; }
            if (order.AnalyticsTracked) { return; }
            if (store == null) { return; }

            try
            {
                CGoogleAnalyticsScript analytics = Page.Master.FindControl("CGoogleAnalyticsScript1") as CGoogleAnalyticsScript;
                if (analytics == null) { DoAsyncTracking(); return; }

                AnalyticsTransaction transaction = new AnalyticsTransaction();
                transaction.OrderId = order.OrderGuid.ToString();
                transaction.City = order.CustomerCity;
                transaction.Country = order.CustomerCountry;
                transaction.State = order.CustomerState;
                transaction.StoreName = siteSettings.SiteName + " - " + store.Name;
                transaction.Tax = order.TaxTotal.ToString(CultureInfo.InvariantCulture);
                transaction.Total = order.OrderTotal.ToString(CultureInfo.InvariantCulture);

                foreach (OrderOffer offer in order.OrderOffers)
                {
                    AnalyticsTransactionItem item = new AnalyticsTransactionItem();
                    item.Category = string.Empty;
                    item.OrderId = order.OrderGuid.ToString();
                    item.Price = offer.OfferPrice.ToString(CultureInfo.InvariantCulture);
                    item.ProductName = offer.Name;
                    item.Quantity = offer.Quantity.ToString(CultureInfo.InvariantCulture);
                    item.Sku = offer.OfferGuid.ToString();
                    transaction.Items.Add(item);

                }

                if (transaction.IsValid())
                {
                    analytics.Transactions.Add(transaction);
                    Order.TrackAnalytics(order.OrderGuid);
                }

            }
            catch(Exception ex)
            {
                log.Error("error tracking order in google analytics.", ex);
            }

        }

        private void DoAsyncTracking()
        {
            AnalyticsAsyncTopScript analytics = Page.Master.FindControl("analyticsTop") as AnalyticsAsyncTopScript;
            if (analytics == null) { return; }

            AnalyticsTransaction transaction = new AnalyticsTransaction();
            transaction.OrderId = order.OrderGuid.ToString();
            transaction.City = order.CustomerCity;
            transaction.Country = order.CustomerCountry;
            transaction.State = order.CustomerState;
            transaction.StoreName = siteSettings.SiteName + " - " + store.Name;
            transaction.Tax = order.TaxTotal.ToString(CultureInfo.InvariantCulture);
            transaction.Total = order.OrderTotal.ToString(CultureInfo.InvariantCulture);

            foreach (OrderOffer offer in order.OrderOffers)
            {
                AnalyticsTransactionItem item = new AnalyticsTransactionItem();
                item.Category = string.Empty;
                item.OrderId = order.OrderGuid.ToString();
                item.Price = offer.OfferPrice.ToString(CultureInfo.InvariantCulture);
                item.ProductName = offer.Name;
                item.Quantity = offer.Quantity.ToString(CultureInfo.InvariantCulture);
                item.Sku = offer.OfferGuid.ToString();
                transaction.Items.Add(item);

            }

            if (transaction.IsValid())
            {
                analytics.Transactions.Add(transaction);
                Order.TrackAnalytics(order.OrderGuid);
            }

        }


        private void PopulateLabels()
        {
            Control c = Master.FindControl("Breadcrumbs");
            if (c != null)
            {
                BreadcrumbsControl crumbs = (BreadcrumbsControl)c;
                crumbs.ForceShowBreadcrumbs = true;
            }

            Title = SiteUtils.FormatPageTitle(siteSettings, WebStoreResources.OrderDetailHeader);

            litOrderHeader.Text = WebStoreResources.OrderDetailHeader;
            litItemsHeader.Text = WebStoreResources.OrderDetailItemsHeader;
            litDownloadItemsHeader.Text = WebStoreResources.OrderDetailDownloadItemsHeader;
            litBillingHeader.Text = WebStoreResources.CheckoutBillingInfoHeader;
            litShippingHeader.Text = WebStoreResources.CheckoutShippingInfoHeader;


        }

        private void LoadParams()
        {
            pageId = WebUtils.ParseInt32FromQueryString("pageid", pageId);
            moduleId = WebUtils.ParseInt32FromQueryString("mid", moduleId);

        }

        private void LoadSettings()
        {
            timeOffset = SiteUtils.GetUserTimeOffset();
            currencyCulture = ResourceHelper.GetCurrencyCulture(siteSettings.GetCurrency().Code);
            if (Request.IsAuthenticated)
            {
                siteUser = SiteUtils.GetCurrentSiteUser();
            }

            if (CurrentPage.ContainsModule(moduleId))
            {
                store = StoreHelper.GetStore();
            }

            if (store == null) { return; }

            orderGuid = WebUtils.ParseGuidFromQueryString("orderid", orderGuid);
            if (orderGuid == Guid.Empty)
            {
                orderGuid = WebUtils.ParseGuidFromQueryString("order", orderGuid);
                
            }

            if (orderGuid != Guid.Empty)
            {
                order = new Order(orderGuid);
                if (order.StoreGuid != store.Guid) { order = null; }

                if ((order.StatusGuid == OrderStatus.OrderStatusCancelledGuid)
                    ||(order.StatusGuid == OrderStatus.OrderStatusNoneGuid))
                { order = null; }

            }

           

        }

        protected virtual void SetupCss()
        {
            // older skins have this
            StyleSheet stylesheet = (StyleSheet)Page.Master.FindControl("StyleSheet");
            if (stylesheet != null)
            {
                if (stylesheet.FindControl("stylewebstore") == null)
                {
                    Literal cssLink = new Literal();
                    cssLink.ID = "stylewebstore";
                    cssLink.Text = "\n<link href='"
                    + SiteUtils.GetSkinBaseUrl()
                    + "stylewebstore.css' type='text/css' rel='stylesheet' media='screen' />";

                    stylesheet.Controls.Add(cssLink);
                }
            }
            
        }


        #region OnInit

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);
            rptOffers.ItemDataBound += new RepeaterItemEventHandler(rptOffers_ItemDataBound);

            SuppressPageMenu();

        }

        

        #endregion
    }
}
