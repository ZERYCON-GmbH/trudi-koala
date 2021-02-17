namespace TRuDI.Backend.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds the items of the side bar menu (on the right side).
    /// </summary>
    public class SideBarMenu
    {
        /// <summary>
        /// The menu items.
        /// </summary>
        private readonly List<BreadCrumbTrailItem> items = new List<BreadCrumbTrailItem>();

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        public IReadOnlyList<BreadCrumbTrailItem> Items => this.items;

        /// <summary>
        /// Adds a menu item.
        /// </summary>
        /// <param name="name">The name shown to the user.</param>
        /// <param name="link">The link.</param>
        /// <param name="staticItem">if set to <c>true</c> the item is shown on each page.</param>
        /// <param name="useOnClick">if set to <c>true</c> use the OnClick handler.</param>
        public void Add(string name, string link, bool staticItem = false, bool useOnClick = false)
        {
            this.items.Add(new BreadCrumbTrailItem(this.items.Count, name, link) { Static = staticItem, UseOnClick = useOnClick });
        }

        /// <summary>
        /// Removes all non-static menu items.
        /// </summary>
        public void Clear()
        {
            for (int i = this.items.Count - 1; i >= 0; i--)
            {
                if (!this.items[i].Static)
                {
                    this.items.RemoveAt(i);
                }
            }
        }
    }
}