using CMS.ContentEngine;
using CMS.Websites;

using DancingGoat.Models;
using DancingGoat.Widgets;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

[assembly: RegisterWidget(
    EndorsementWidgetViewComponent.IDENTIFIER,
    typeof(EndorsementWidgetViewComponent),
    "Endorsement Widget",
    typeof(EndorsementWidgetProperties),
    Description = "Displays a product endorsement with expert credentials in one of three layouts.",
    IconClass = "icon-bubble")]

namespace DancingGoat.Widgets
{
    /// <summary>
    /// View component for the Endorsement widget.
    /// </summary>
    public class EndorsementWidgetViewComponent : ViewComponent
    {
        /// <summary>Widget identifier.</summary>
        public const string IDENTIFIER = "DancingGoat.LandingPage.EndorsementWidget";

        private const string VIEW_V1 = "~/Components/Widgets/EndorsementWidget/_EndorsementWidget_V1.cshtml";
        private const string VIEW_V2 = "~/Components/Widgets/EndorsementWidget/_EndorsementWidget_V2.cshtml";
        private const string VIEW_V3 = "~/Components/Widgets/EndorsementWidget/_EndorsementWidget_V3.cshtml";

        private readonly IContentRetriever contentRetriever;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;


        /// <summary>
        /// Creates an instance of <see cref="EndorsementWidgetViewComponent"/>.
        /// </summary>
        public EndorsementWidgetViewComponent(
            IContentRetriever contentRetriever,
            IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.contentRetriever = contentRetriever;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        /// <inheritdoc />
        public async Task<ViewViewComponentResult> InvokeAsync(
            EndorsementWidgetProperties properties,
            CancellationToken cancellationToken)
        {
            string viewPath = GetViewPath(properties.Layout);
            var viewModel = await BuildViewModel(properties, cancellationToken);
            return View(viewPath, viewModel);
        }


        private static string GetViewPath(string layout) => layout switch
        {
            EndorsementWidgetProperties.LAYOUT_PRODUCT_FEATURE_CARD => VIEW_V2,
            EndorsementWidgetProperties.LAYOUT_MAGAZINE_PULL_QUOTE => VIEW_V3,
            _ => VIEW_V1
        };


        private async Task<EndorsementWidgetViewModel> BuildViewModel(
            EndorsementWidgetProperties properties,
            CancellationToken cancellationToken)
        {
            var endorsementRef = properties.SelectedEndorsement?.FirstOrDefault();
            if (endorsementRef == null)
            {
                return new EndorsementWidgetViewModel { Layout = properties.Layout };
            }

            var endorsementGuid = endorsementRef.Identifier;
            var endorsements = await contentRetriever.RetrieveContent<Endorsement>(
                new RetrieveContentParameters { LinkedItemsMaxLevel = 2 },
                query => query.Where(w => w.WhereEquals(nameof(IContentQueryDataContainer.ContentItemGUID), endorsementGuid)),
                new RetrievalCacheSettings($"Endorsement_guid_{endorsementGuid}"),
                cancellationToken);

            var endorsement = endorsements.FirstOrDefault();
            if (endorsement == null)
            {
                return new EndorsementWidgetViewModel { Layout = properties.Layout };
            }

            var expert = endorsement.EndorsementExpert?.FirstOrDefault();
            var expertPhoto = expert?.IndustryExpertPhoto?.FirstOrDefault();
            var product = endorsement.EndorsementProduct?.FirstOrDefault() as IProductFields;

            string expertPhotoUrl = expertPhoto?.ImageFile?.Url;
            string expertBio = expert?.IndustryExpertBio;
            string expertWebUrl = expert?.IndustryExpertWebsiteUrl;
            string productDescription = product?.ProductFieldDescription;
            string productImageUrl = product?.ProductFieldImage?.FirstOrDefault()?.ImageFile?.Url;

            string productPageUrl = null;
            if (endorsement.EndorsementProduct?.FirstOrDefault() is IContentItemFieldsSource productSource)
            {
                productPageUrl = await GetProductPageUrl(productSource.SystemFields.ContentItemID, cancellationToken);
            }

            return new EndorsementWidgetViewModel
            {
                Layout = properties.Layout,
                EndorsementQuote = endorsement.EndorsementQuote,
                EndorsementRating = endorsement.EndorsementRating,
                ShowRating = properties.ShowRating && endorsement.EndorsementRating > 0,
                EndorsementDate = endorsement.EndorsementDate,
                ShowDate = properties.ShowDate && endorsement.EndorsementDate != default,
                ExpertName = expert?.IndustryExpertName,
                ExpertTitle = expert?.IndustryExpertTitle,
                ExpertPhotoUrl = expertPhotoUrl,
                ShowExpertPhoto = properties.ShowExpertPhoto && expertPhotoUrl != null,
                ExpertBio = expertBio,
                ShowExpertBio = properties.ShowExpertBio && !string.IsNullOrEmpty(expertBio),
                ExpertWebUrl = expertWebUrl,
                ShowExpertWebUrl = properties.ShowExpertWebUrl && !string.IsNullOrEmpty(expertWebUrl),
                ProductName = product?.ProductFieldName,
                ProductDescription = productDescription != null ? new HtmlString(productDescription) : null,
                ShowProductDescription = properties.ShowProductDescription && !string.IsNullOrEmpty(productDescription),
                ProductImageUrl = productImageUrl,
                ProductPageUrl = productPageUrl
            };
        }


        private async Task<string> GetProductPageUrl(int productContentItemId, CancellationToken cancellationToken)
        {
            string languageName = currentLanguageRetriever.Get();
            var productPages = await contentRetriever.RetrievePages<ProductPage>(
                new RetrievePagesParameters { LanguageName = languageName },
                query => query.Linking(nameof(ProductPage.ProductPageProduct), [productContentItemId]),
                new RetrievalCacheSettings($"ProductPage_LinkedByProduct_{productContentItemId}_{languageName}"),
                cancellationToken);

            return productPages.FirstOrDefault()?.GetUrl()?.RelativePath;
        }
    }
}
