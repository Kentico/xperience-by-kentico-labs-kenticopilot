using Microsoft.AspNetCore.Html;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// View model for the Endorsement widget.
    /// </summary>
    public class EndorsementWidgetViewModel
    {
        /// <summary>
        /// The selected layout variant (V1 ExpertSpotlight, V2 ProductFeatureCard, V3 MagazinePullQuote).
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// The endorsement quote text.
        /// </summary>
        public string EndorsementQuote { get; set; }

        /// <summary>
        /// Star rating 1–5; 0 means not set.
        /// </summary>
        public int EndorsementRating { get; set; }

        /// <summary>
        /// Whether to render the star rating.
        /// </summary>
        public bool ShowRating { get; set; }

        /// <summary>
        /// Endorsement date.
        /// </summary>
        public DateTime EndorsementDate { get; set; }

        /// <summary>
        /// Whether to render the endorsement date.
        /// </summary>
        public bool ShowDate { get; set; }

        /// <summary>
        /// Expert full name.
        /// </summary>
        public string ExpertName { get; set; }

        /// <summary>
        /// Expert professional title.
        /// </summary>
        public string ExpertTitle { get; set; }

        /// <summary>
        /// URL of the expert headshot; null if not available.
        /// </summary>
        public string ExpertPhotoUrl { get; set; }

        /// <summary>
        /// Whether to render the expert photo.
        /// </summary>
        public bool ShowExpertPhoto { get; set; }

        /// <summary>
        /// Expert bio text (may contain HTML).
        /// </summary>
        public string ExpertBio { get; set; }

        /// <summary>
        /// Whether to render the expert bio.
        /// </summary>
        public bool ShowExpertBio { get; set; }

        /// <summary>
        /// Expert website URL.
        /// </summary>
        public string ExpertWebUrl { get; set; }

        /// <summary>
        /// Whether to render the expert website URL link.
        /// </summary>
        public bool ShowExpertWebUrl { get; set; }

        /// <summary>
        /// Name of the endorsed product.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product description text (HTML).
        /// </summary>
        public HtmlString ProductDescription { get; set; }

        /// <summary>
        /// Whether to render the product description.
        /// </summary>
        public bool ShowProductDescription { get; set; }

        /// <summary>
        /// Product image URL; null if not available.
        /// </summary>
        public string ProductImageUrl { get; set; }

        /// <summary>
        /// Relative URL of the product page for the CTA button; null if not found.
        /// </summary>
        public string ProductPageUrl { get; set; }
    }
}
