# Page Builder Widget Requirements

## Widget Overview

### Widget Identification

- **Widget Name**: Endorsement Widget
- **Widget Identifier**: `DancingGoat.LandingPage.EndorsementWidget`
  - _Format: `CompanyName.WidgetName` (e.g., `DancingGoat.CardWidget`)_
- **Description**: Displays a single product endorsement with expert credentials in one of three configurable visual layouts.
- **Icon Class**: `icon-bubble`

### Purpose

Allows content editors to select a single `Endorsement` content item from the Content Hub and display it on a landing page using one of three design layouts: Expert Spotlight, Product Feature Card, or Magazine Pull-Quote. Each layout emphasises a different aspect of the endorsement (expert identity vs. product vs. editorial quote). A CTA button always links to the product page of the endorsed product.

### Core Functionality

- Single `Endorsement` content item selection via the combined content selector
- Layout selection from 3 options: `ExpertSpotlight` (V1), `ProductFeatureCard` (V2), `MagazinePullQuote` (V3)
- Per-layout rendering using a dedicated `.cshtml` partial view for each layout
- Configurable visibility of: product description, endorsement rating, endorsement date, expert photo, expert website URL, expert bio
- Use `VisibleIfEqualTo` / `VisibleIfNotEqualTo` widget property visibility conditions to hide properties irrelevant to the selected layout
- Load the `Endorsement` content item with `LinkedItemsMaxLevel = 2` to eagerly load the nested `IndustryExpert` and its `IndustryExpertPhoto` (an `Image`)
- Retrieve the `ProductPage` that links to the endorsed product using `IContentRetriever.RetrievePages<ProductPage>()` with `.Linking()` filtered by the endorsed product's content item GUID
- Render CTA button linking to the product page's relative URL obtained via `GetUrl().RelativePath`
- Use `IPreferredLanguageRetriever` to get the current language for page retrieval
- Reuse existing CSS from `Site.css` and `landing-page.css` (palette tokens, `.btn-cta`, `.star-rating`, etc.); add new scoped CSS custom properties and classes for the endorsement layouts only where needed
- Gracefully handle missing data: null endorsement selection, missing expert, missing photo, missing product, missing product page URL, missing optional fields

## Widget Properties

| Property Name | Type | Form Component | Required | Default Value | Description |
| --- | --- | --- | --- | --- | --- |
| `SelectedEndorsement` | `IEnumerable<ContentItemReference>` | `ContentItemSelectorComponent(Endorsement.CONTENT_TYPE_NAME, MaximumItems = 1, Order = 1)` | Yes | `[]` | The single Endorsement content item to display |
| `Layout` | `string` | `DropDownComponent` with `DataProviderType = typeof(EndorsementLayoutOptionsProvider)`, Order = 2 | Yes | `"ExpertSpotlight"` | Selects one of the 3 design layouts |
| `ShowProductDescription` | `bool` | `CheckBoxComponent`, Order = 10 | No | `true` | Toggles product description text. Visible only for `ProductFeatureCard` and `MagazinePullQuote` layouts (hide when Layout = `ExpertSpotlight`) |
| `ShowRating` | `bool` | `CheckBoxComponent`, Order = 11 | No | `true` | Toggles the endorsement star rating display |
| `ShowDate` | `bool` | `CheckBoxComponent`, Order = 12 | No | `true` | Toggles the endorsement date display |
| `ShowExpertPhoto` | `bool` | `CheckBoxComponent`, Order = 13 | No | `true` | Toggles the expert photo. Visible for all layouts. |
| `ShowExpertBio` | `bool` | `CheckBoxComponent`, Order = 14 | No | `false` | Toggles the expert bio text. Visible only for `ExpertSpotlight` and `MagazinePullQuote` layouts (hide when Layout = `ProductFeatureCard`) |
| `ShowExpertWebUrl` | `bool` | `CheckBoxComponent`, Order = 15 | No | `false` | Toggles the expert website URL link. Visible only for `ExpertSpotlight` and `MagazinePullQuote` layouts (hide when Layout = `ProductFeatureCard`) |

**Visibility condition notes:**
- Use `[VisibleIfEqualTo(nameof(Layout), EndorsementWidgetProperties.LAYOUT_EXPERT_SPOTLIGHT)]` and `[VisibleIfEqualTo(nameof(Layout), EndorsementWidgetProperties.LAYOUT_MAGAZINE_PULL_QUOTE)]` stacked on `ShowExpertBio` and `ShowExpertWebUrl` to show them only for V1 and V3 (stacked attributes use OR logic).
- Use `[VisibleIfEqualTo(nameof(Layout), EndorsementWidgetProperties.LAYOUT_PRODUCT_FEATURE_CARD)]` and `[VisibleIfEqualTo(nameof(Layout), EndorsementWidgetProperties.LAYOUT_MAGAZINE_PULL_QUOTE)]` stacked on `ShowProductDescription` to show it only for V2 and V3.
- Layout constant strings — define as `public const string` fields on the properties class:
  - `LAYOUT_EXPERT_SPOTLIGHT = "ExpertSpotlight"`
  - `LAYOUT_PRODUCT_FEATURE_CARD = "ProductFeatureCard"`
  - `LAYOUT_MAGAZINE_PULL_QUOTE = "MagazinePullQuote"`
- Implement `EndorsementLayoutOptionsProvider : IDropDownOptionsProvider` returning the 3 layout options with user-friendly labels ("Expert Spotlight", "Product Feature Card", "Magazine Pull-Quote").

## Data Requirements

### External Data Sources

| Content Type Name | Property Name | Description |
| --- | --- | --- |
| `DancingGoat.Endorsement` | `SelectedEndorsement` | The endorsement to display, selected by the editor. Loaded with `LinkedItemsMaxLevel = 2` to include the nested `IndustryExpert` and its `IndustryExpertPhoto`. |
| `DancingGoat.ProductPage` (web page) | _(derived)_ | The product page linking to the endorsed product. Retrieved programmatically using the endorsed product's content item GUID via `Linking()`. |

### Data Retrieval Logic

1. **Retrieve the Endorsement:**
   - Get the selected endorsement GUID from `properties.SelectedEndorsement.FirstOrDefault()?.Identifier`.
   - If null, return a view with a null/empty view model (no selection made).
   - Call `contentRetriever.RetrieveContentByGuids<Endorsement>([guid], new RetrieveContentParameters { LinkedItemsMaxLevel = 2 }, cancellationToken)`.
   - Take `FirstOrDefault()`.

2. **Extract linked data from the Endorsement:**
   - `IndustryExpert expert = endorsement.EndorsementExpert.FirstOrDefault()` — may be null.
   - `Image expertPhoto = expert?.IndustryExpertPhoto.FirstOrDefault()` — may be null.
   - `IProductFields product = endorsement.EndorsementProduct.FirstOrDefault() as IProductFields` — may be null.
   - Product name: `product?.ProductFieldName`
   - Product description: `product?.ProductFieldDescription`
   - Product image URL: `product?.ProductFieldImage.FirstOrDefault()?.ImageFile.Url`

3. **Retrieve the ProductPage for the CTA link:**
   - Need the endorsed product's content item GUID. Cast `endorsement.EndorsementProduct.FirstOrDefault()` to `IContentItemFieldsSource` and get `SystemFields.ContentItemGUID`.
   - Get language via `currentLanguageRetriever.Get()`.
   - Call `contentRetriever.RetrievePages<ProductPage>(new RetrievePagesParameters { LanguageName = languageName }, query => query.Linking(nameof(ProductPage.ProductPageProduct), new[] { productGuid }), new RetrievalCacheSettings($"ProductPage_LinkedByProduct_{productGuid}"), cancellationToken)`.
   - Take `FirstOrDefault()`.
   - CTA URL: `productPage?.GetUrl()?.RelativePath` — may be null (handle gracefully, e.g., omit or disable the CTA button).

4. **Build the view model** from all retrieved data, respecting the property visibility flags from widget properties.

### Dependencies

- `IContentRetriever` — for retrieving the Endorsement and ProductPage content.
- `IPreferredLanguageRetriever` — for obtaining the current language name when retrieving pages.

## View/Presentation

### View Model Structure

| Property Name | Type | Source | Description |
| --- | --- | --- | --- |
| `Layout` | `string` | `properties.Layout` | Which layout partial to render |
| `EndorsementQuote` | `string` | `endorsement.EndorsementQuote` | The expert's endorsement quote text |
| `EndorsementRating` | `int` | `endorsement.EndorsementRating` | Star rating 1–5; 0 means not set |
| `ShowRating` | `bool` | `properties.ShowRating && endorsement.EndorsementRating > 0` | Whether to show rating (also check data presence) |
| `EndorsementDate` | `DateTime` | `endorsement.EndorsementDate` | Endorsement date |
| `ShowDate` | `bool` | `properties.ShowDate && endorsement.EndorsementDate != default` | Whether to show date |
| `ExpertName` | `string` | `expert.IndustryExpertName` | Expert's full name |
| `ExpertTitle` | `string` | `expert.IndustryExpertTitle` | Expert's professional title |
| `ExpertPhotoUrl` | `string` | `expertPhoto.ImageFile.Url` | URL of expert headshot; null if not set |
| `ShowExpertPhoto` | `bool` | `properties.ShowExpertPhoto && ExpertPhotoUrl != null` | Whether to show expert photo |
| `ExpertBio` | `string` | `expert.IndustryExpertBio` | Expert bio text (HTML) |
| `ShowExpertBio` | `bool` | `properties.ShowExpertBio && !string.IsNullOrEmpty(ExpertBio)` | Whether to show expert bio |
| `ExpertWebUrl` | `string` | `expert.IndustryExpertWebsiteUrl` | Expert website URL |
| `ShowExpertWebUrl` | `bool` | `properties.ShowExpertWebUrl && !string.IsNullOrEmpty(ExpertWebUrl)` | Whether to show expert web URL |
| `ProductName` | `string` | `product.ProductFieldName` | Name of endorsed product |
| `ProductDescription` | `string` | `product.ProductFieldDescription` | Product description text |
| `ShowProductDescription` | `bool` | `properties.ShowProductDescription && !string.IsNullOrEmpty(ProductDescription)` | Whether to show product description |
| `ProductImageUrl` | `string` | `product.ProductFieldImage.FirstOrDefault()?.ImageFile.Url` | Product image URL; null if not set |
| `ProductPageUrl` | `string` | `productPage?.GetUrl()?.RelativePath` | Relative URL of the product page for CTA; null if not found |

### HTML Structure

Each layout has its own `.cshtml` partial view. The `EndorsementWidgetViewComponent` selects the correct view path based on `properties.Layout` and passes the `EndorsementWidgetViewModel`.

**Layout V1 – Expert Spotlight** (`_EndorsementWidget_V1.cshtml`):
- Outer `div.v1-endorsement` (flex, dark espresso bg, decorative `::before` quote mark)
- Left column `div.v1-expert`: circular expert photo (if shown), expert name, expert title, expert bio (if shown), expert web URL link (if shown)
- Right column `div.v1-content`: large italic quote, product row (`div.v1-product`) with product thumbnail + product name + CTA button, footer row with star rating (if shown) and date (if shown)

**Layout V2 – Product Feature Card** (`_EndorsementWidget_V2.cshtml`):
- Outer `div.v2-endorsement` (flex, white card, box-shadow)
- Left panel `div.v2-product-panel`: product image, product name, product description (if shown), CTA button
- Right panel `div.v2-endorsement-panel`: eyebrow label "Expert Endorsement", decorative opening quote mark, quote text, star rating row (if shown), expert badge (`div.v2-expert-badge`) with small expert avatar (if shown) + expert name + expert title, date (if shown)

**Layout V3 – Magazine Pull-Quote** (`_EndorsementWidget_V3.cshtml`):
- Outer `div.v3-endorsement` (CSS grid 2 cols, mid-brown bg, large watermark `::after` quote)
- Main section `div.v3-main`: eyebrow label, blockquote with gold left border, expert row (circular photo (if shown) + name + title + bio (if shown)), meta row (star rating (if shown) + date (if shown) + web URL link (if shown))
- Sidebar `div.v3-product-sidebar` (dark espresso bg): sidebar label, product image, product name, product description (if shown), CTA button

**Shared elements:**
- Star rating: render `★` characters repeated `EndorsementRating` times inside a `span.star-rating`
- Date: format as `MMMM yyyy` (e.g., "February 2026") using `EndorsementDate.ToString("MMMM yyyy")`
- CTA button: `<a class="btn-cta" href="@Url.Content(Model.ProductPageUrl)">Shop Now</a>` — omit the anchor entirely when `ProductPageUrl` is null
- Show "configure widget" placeholder message when `EndorsementQuote` is null or empty (no endorsement selected or data is missing)

### Styling Requirements

- Reuse existing CSS custom properties from `Site.css`: `--c-espresso`, `--c-gold`, `--c-mid-brown`, `--c-link-gold`, `--c-white`, `--c-bg`, `--c-star`.
- Reuse `.btn-cta` and `.star-rating` classes already in Site.css (or landing-page.css).
- Add new scoped CSS classes (prefixed `v1-`, `v2-`, `v3-`) in `Site.css` or `landing-page.css` for the three layout variations, following the design specifications in `design.html`.
- Do not use inline styles; use CSS classes and custom properties throughout.
- Use modern CSS features: `display: flex`, `display: grid`, `gap`, `clamp()`, CSS custom properties.

### Responsive Behavior

- V1: `.v1-expert` stacks to 100% width on narrow screens (`max-width: 640px`); `.v1-content` reduces padding.
- V2: `.v2-product-panel` border-right becomes border-bottom on narrow screens; panels stack vertically.
- V3: `grid-template-columns` collapses to `1fr` on narrow screens (`max-width: 700px`); `.v3-product-sidebar` border-left becomes border-top.
- All responsive breakpoints follow existing breakpoints in the project.

## JavaScript & Client-side Requirements

No JavaScript is required. The widget is server-rendered only.

## Registration Details

Register the widget using the `[assembly: RegisterWidget(...)]` attribute at the top of the `EndorsementWidgetViewComponent.cs` file, following the same pattern as existing widgets:

```csharp
[assembly: RegisterWidget(
    EndorsementWidgetViewComponent.IDENTIFIER,
    typeof(EndorsementWidgetViewComponent),
    "Endorsement Widget",
    typeof(EndorsementWidgetProperties),
    Description = "Displays a product endorsement with expert credentials in one of three layouts.",
    IconClass = "icon-bubble")]
```

Do **not** use resource string keys (e.g., `{$...$}`) for the name or description — use plain string literals, matching the simpler registration style used by the CTAButton widget.

Also add an identifier constant to `ComponentIdentifiers.cs`:
```csharp
public const string ENDORSEMENT_WIDGET = "DancingGoat.LandingPage.EndorsementWidget";
```

## Widget File Structure

```
src/DancingGoat/Components/Widgets/EndorsementWidget/
├── EndorsementWidgetViewComponent.cs     — View component; retrieves Endorsement + ProductPage; builds view model; selects layout view
├── EndorsementWidgetProperties.cs        — IWidgetProperties, layout constants, DropDown provider reference, all property declarations with visibility conditions
├── EndorsementWidgetViewModel.cs         — View model class with all display-ready properties (see View Model Structure table)
├── EndorsementLayoutOptionsProvider.cs   — IDropDownOptionsProvider implementation returning the 3 layout options
├── _EndorsementWidget_V1.cshtml          — Partial view for Layout V1: Expert Spotlight
├── _EndorsementWidget_V2.cshtml          — Partial view for Layout V2: Product Feature Card
└── _EndorsementWidget_V3.cshtml          — Partial view for Layout V3: Magazine Pull-Quote
```

**`EndorsementWidgetViewComponent.cs`:**
- Inject `IContentRetriever` and `IPreferredLanguageRetriever` via constructor.
- `InvokeAsync(EndorsementWidgetProperties properties, CancellationToken cancellationToken)` method.
- Select view path based on `properties.Layout`: `"~/Components/Widgets/EndorsementWidget/_EndorsementWidget_V{1|2|3}.cshtml"`.
- Return `View(viewPath, viewModel)`.

**`EndorsementWidgetProperties.cs`:**
- Implements `IWidgetProperties`.
- Define layout string constants as `public const string`.
- `SelectedEndorsement`: `IEnumerable<ContentItemReference>` with `[ContentItemSelectorComponent(Endorsement.CONTENT_TYPE_NAME, MaximumItems = 1, Label = "Endorsement", Order = 1)]`.
- `Layout`: `string` with `[DropDownComponent(Label = "Layout", DataProviderType = typeof(EndorsementLayoutOptionsProvider), Order = 2)]`, default `= LAYOUT_EXPERT_SPOTLIGHT`.
- Visibility-conditioned boolean properties (see Widget Properties section).

**`EndorsementWidgetViewModel.cs`:**
- Plain C# class with all properties listed in the View Model Structure table.
- Add a static factory method or keep construction in the view component — factory with `EndorsementWidgetViewModel.GetViewModel(endorsement, expert, expertPhoto, product, productPage, properties)` is preferred for testability.

**`EndorsementLayoutOptionsProvider.cs`:**
- Implements `IDropDownOptionsProvider`.
- Returns 3 `DropDownOptionItem` objects with values matching the layout constants and user-friendly display labels.

**`_EndorsementWidget_V1.cshtml` / `_V2.cshtml` / `_V3.cshtml`:**
- `@model DancingGoat.Widgets.EndorsementWidgetViewModel` at top.
- Show a configure-widget placeholder when `Model` or `Model.EndorsementQuote` is null/empty.
- Render the full HTML structure as described in HTML Structure section.
- Use `@Url.Content(...)` for image `src` and CTA `href` attributes.
- Use `@Html.Raw(Model.ExpertBio)` for rich-text bio.

## Implementation Checklist

- [ ] Create `EndorsementWidgetProperties.cs` with all properties, layout constants, visibility conditions, and dropdown provider reference
- [ ] Create `EndorsementLayoutOptionsProvider.cs` implementing `IDropDownOptionsProvider`
- [ ] Create `EndorsementWidgetViewModel.cs` with all view model properties (nullable strings, bools with combined condition)
- [ ] Create `EndorsementWidgetViewComponent.cs` with correct `InvokeAsync`, data retrieval (endorsement + expert + product + productPage), view model mapping, and layout view selection
- [ ] Register the widget with `[assembly: RegisterWidget(...)]` in `EndorsementWidgetViewComponent.cs`
- [ ] Add `ENDORSEMENT_WIDGET` constant to `ComponentIdentifiers.cs`
- [ ] Create `_EndorsementWidget_V1.cshtml` implementing the Expert Spotlight layout from `design.html`
- [ ] Create `_EndorsementWidget_V2.cshtml` implementing the Product Feature Card layout from `design.html`
- [ ] Create `_EndorsementWidget_V3.cshtml` implementing the Magazine Pull-Quote layout from `design.html`
- [ ] Add all required CSS classes and variables to `Site.css` / `landing-page.css`, reusing existing tokens
- [ ] Handle all null/missing data cases gracefully in view component and views
- [ ] Verify all 3 layouts render correctly with complete data, partial data, and no data
- [ ] Confirm visibility conditions show/hide the correct property panels for each layout in the Page Builder editor

## Additional Notes

- The `Endorsement.EndorsementProduct` field is typed as `IEnumerable<IContentItemFieldsSource>` in the generated model. Cast to `IProductFields` (the existing reusable field schema interface) to access `ProductFieldName`, `ProductFieldDescription`, and `ProductFieldImage`.
- For the product page URL, use `.Linking(nameof(ProductPage.ProductPageProduct), new[] { productGuid })` in the `RetrievePages<ProductPage>` query builder to filter pages that reference the product. If no `ProductPage` is found, render the CTA without a URL or omit it entirely.
- The `EndorsementRating` field defaults to `0` when not set in the database (non-nullable `int`). Treat `0` as "no rating" and suppress the rating display regardless of `ShowRating`.
- The `EndorsementDate` defaults to `DateTime.MinValue` when not set. Treat `default(DateTime)` as "no date" and suppress the date regardless of `ShowDate`.
- The `IndustryExpertBio` field is a rich-text (long text HTML) field — render with `@Html.Raw()` and ensure XSS safety by trusting only editor-provided content.
- Follow the `base-pagebuilder.instructions.md` rule: use the Combined Content Selector (not the Web Page Selector) for the endorsement selection property.
- Keep each layout view file self-contained and independently testable.
- Do not extract a shared partial between layouts unless a pattern appears identically in at least two layouts.
