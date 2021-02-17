namespace TRuDI.Backend.Application
{
    /// <summary>
    /// Item within the <see cref="BreadCrumbTrail"/>.
    /// </summary>
    public class BreadCrumbTrailItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BreadCrumbTrailItem"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name shown in the UI.</param>
        /// <param name="link">The link.</param>
        public BreadCrumbTrailItem(int id, string name, string link)
        {
            this.Id = id;
            this.Name = name;
            this.Link = link;
        }

        /// <summary>
        /// Gets or sets the ID of the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the displayed text.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the link href/onclick reference.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the item is active.1
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that this is the selected item.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the link is a Javascript function call and should be placed in the onclick attribute.
        /// </summary>
        public bool UseOnClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is shown on each page.
        /// </summary>
        public bool Static { get; set; }
    }
}