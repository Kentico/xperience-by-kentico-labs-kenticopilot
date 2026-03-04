using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Provides the layout options for the Endorsement widget drop-down.
    /// </summary>
    public class EndorsementLayoutOptionsProvider : IDropDownOptionsProvider
    {
        /// <inheritdoc />
        public Task<IEnumerable<DropDownOptionItem>> GetOptionItems() =>
            Task.FromResult<IEnumerable<DropDownOptionItem>>(
            [
                new DropDownOptionItem { Value = EndorsementWidgetProperties.LAYOUT_EXPERT_SPOTLIGHT, Text = "Expert Spotlight" },
                new DropDownOptionItem { Value = EndorsementWidgetProperties.LAYOUT_PRODUCT_FEATURE_CARD, Text = "Product Feature Card" },
                new DropDownOptionItem { Value = EndorsementWidgetProperties.LAYOUT_MAGAZINE_PULL_QUOTE, Text = "Magazine Pull-Quote" }
            ]);
    }
}
