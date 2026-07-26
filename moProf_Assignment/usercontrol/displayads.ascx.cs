using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;

namespace moProf_Assignment.usercontrol
{
    public partial class displayads : UserControl
    {
        private class Ad
        {
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public string LinkUrl { get; set; }
        }

        // How many ads to show at once
        private const int AdsToShow = 2;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAds();
            }
        }

        private void LoadAds()
        {
            var allAds = LoadAdsFromXml();

            if (allAds.Count == 0)
            {
                adsRepeater.Visible = false;
                return;
            }

            var rnd = new Random();
            var randomAds = allAds
                .OrderBy(a => rnd.Next())
                .Take(AdsToShow)
                .ToList();

            adsRepeater.DataSource = randomAds;
            adsRepeater.DataBind();
        }

        private List<Ad> LoadAdsFromXml()
        {
            var ads = new List<Ad>();

            try
            {
                string xmlPath = Server.MapPath("~/App_Data/ads.xml");
                XDocument doc = XDocument.Load(xmlPath);

                ads = doc.Descendants("Ad")
                    .Select(x => new Ad
                    {
                        Title = (string)x.Element("Title"),
                        ImageUrl = (string)x.Element("ImageUrl"),
                        LinkUrl = (string)x.Element("LinkUrl")
                    })
                    .Where(a => !string.IsNullOrEmpty(a.ImageUrl))
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading ads.xml: " + ex.Message);
            }

            return ads;
        }
    }
}