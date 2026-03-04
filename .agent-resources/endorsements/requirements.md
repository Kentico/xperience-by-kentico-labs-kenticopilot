# Endorsement widget requirements

- The widget should support all 3 design variations in design.html, which will be referred to as Layouts
- Each Layout will have its own .cshtml file
- The widget component code will select which .cshtml file is rendered based on the Layout selected by the marketer
- Each Layout starts at the `<div class="section-wrap">` element
- Reuse styles already in Site.css where possible and add new styles where needed
- Styles should use CSS custom properties and other modern CSS features to make CSS rules maintainable long-term
- Gracefully handle missing data in the Endorsement or IndustryExpert content
- Use widget property visibility conditions to only display properties that are relevant for each Layout
- The following should have adjustable visibility - product description, endorsement rating, endorsement date, industry expert photo, industry expert web url, industry expert bio
- The CTA for each Layout should link to the product page of the product being endorsed
- The Endorsement links to the product, not product page. Use the `IContentRetriever` to retrieve the page linking to the endorsement's product.
- The landing page template view has an allowed widgets restriction - ensure this new widget can be used there.
