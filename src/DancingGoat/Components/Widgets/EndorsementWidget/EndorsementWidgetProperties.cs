using CMS.ContentEngine;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Properties for the Endorsement widget.
    /// </summary>
    public class EndorsementWidgetProperties : IWidgetProperties
    {
        /// <summary>Layout constant: Expert Spotlight (V1).</summary>
        public const string LAYOUT_EXPERT_SPOTLIGHT = "ExpertSpotlight";

        /// <summary>Layout constant: Product Feature Card (V2).</summary>
        public const string LAYOUT_PRODUCT_FEATURE_CARD = "ProductFeatureCard";

        /// <summary>Layout constant: Magazine Pull-Quote (V3).</summary>
        public const string LAYOUT_MAGAZINE_PULL_QUOTE = "MagazinePullQuote";


        /// <summary>
        /// The single Endorsement content item to display.
        /// </summary>
        [ContentItemSelectorComponent(
            Models.Endorsement.CONTENT_TYPE_NAME,
            Label = "Endorsement",
            MaximumItems = 1,
            Order = 1)]
        public IEnumerable<ContentItemReference> SelectedEndorsement { get; set; } = [];


        /// <summary>
        /// Selects one of the 3 design layouts.
        /// </summary>
        [DropDownComponent(
            Label = "Layout",
            DataProviderType = typeof(EndorsementLayoutOptionsProvider),
            Order = 2)]
        public string Layout { get; set; } = LAYOUT_EXPERT_SPOTLIGHT;


        /// <summary>
        /// Toggles the product description. Visible for ProductFeatureCard and MagazinePullQuote.
        /// </summary>
        [CheckBoxComponent(Label = "Show product description", Order = 10)]
        [VisibleIfEqualTo(nameof(Layout), LAYOUT_PRODUCT_FEATURE_CARD)]
        [VisibleIfEqualTo(nameof(Layout), LAYOUT_MAGAZINE_PULL_QUOTE)]
        public bool ShowProductDescription { get; set; } = true;


        /// <summary>
        /// Toggles the endorsement star rating.
        /// </summary>
        [CheckBoxComponent(Label = "Show rating", Order = 11)]
        public bool ShowRating { get; set; } = true;


        /// <summary>
        /// Toggles the endorsement date.
        /// </summary>
        [CheckBoxComponent(Label = "Show date", Order = 12)]
        public bool ShowDate { get; set; } = true;


        /// <summary>
        /// Toggles the expert photo.
        /// </summary>
        [CheckBoxComponent(Label = "Show expert photo", Order = 13)]
        public bool ShowExpertPhoto { get; set; } = true;


        /// <summary>
        /// Toggles the expert bio. Visible for ExpertSpotlight and MagazinePullQuote.
        /// </summary>
        [CheckBoxComponent(Label = "Show expert bio", Order = 14)]
        [VisibleIfEqualTo(nameof(Layout), LAYOUT_EXPERT_SPOTLIGHT)]
        [VisibleIfEqualTo(nameof(Layout), LAYOUT_MAGAZINE_PULL_QUOTE)]
        public bool ShowExpertBio { get; set; } = false;


        /// <summary>
        /// Toggles the expert website URL link. Visible for ExpertSpotlight and MagazinePullQuote.
        /// </summary>
        [CheckBoxComponent(Label = "Show expert website URL", Order = 15)]
        [VisibleIfEqualTo(nameof(Layout), LAYOUT_EXPERT_SPOTLIGHT)]
        [VisibleIfEqualTo(nameof(Layout), LAYOUT_MAGAZINE_PULL_QUOTE)]
        public bool ShowExpertWebUrl { get; set; } = false;
    }
}
