namespace TRuDI.Backend.Application
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class is used to manage the bread crumb trail navigation on top of the page.
    /// </summary>
    public class BreadCrumbTrail
    {
        /// <summary>
        /// The current items.
        /// </summary>
        private readonly List<BreadCrumbTrailItem> items = new List<BreadCrumbTrailItem>();

        /// <summary>
        /// Gets the current items.
        /// </summary>
        public IReadOnlyList<BreadCrumbTrailItem> Items => this.items;

        /// <summary>
        /// Adds a new bread crumb trail item. If the item already exists, it's marked as the active one.
        /// </summary>
        /// <param name="name">The name displayed to the user.</param>
        /// <param name="link">The link.</param>
        /// <param name="removeFollowingItems">if set to <c>true</c> items after this items are removed (otherwise just they are just marked as not active).</param>
        public void Add(string name, string link, bool removeFollowingItems)
        {
            var existingItem = this.items.FirstOrDefault(i => i.Link == link);
            if (existingItem == null)
            {
                foreach (var item in this.items)
                {
                    item.IsActive = true;
                    item.IsSelected = false;
                }

                this.items.Add(new BreadCrumbTrailItem(this.items.Count, name, link) { IsSelected = true });
            }
            else
            {
                this.BackTo(existingItem.Id, removeFollowingItems);
            }
        }

        /// <summary>
        /// Removes the unselected/inactive items.
        /// </summary>
        public void RemoveUnselectedItems()
        {
            this.items.RemoveAll(i => !(i.IsActive || i.IsSelected));
        }

        /// <summary>
        /// Moves back to the item with the specified id.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <param name="removeFollowingItems">if set to <c>true</c> items after this items are removed (otherwise just they are just marked as not active).</param>
        /// <returns>The link text of the item.</returns>
        public string BackTo(int id, bool removeFollowingItems)
        {
            if (removeFollowingItems && this.items.Count > id + 1)
            {
                this.items.RemoveRange(id + 1, this.items.Count - id - 1);

                for (int i = 0; i < this.items.Count; i++)
                {
                    this.items[i].IsActive = true;
                    this.Items[i].IsSelected = i == id;
                }

                return this.items.Last().Link;
            }

            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i].IsActive = i < id;
                this.Items[i].IsSelected = i == id;
            }

            return this.items[id].Link;
        }

        /// <summary>
        /// Removes all but the first item.
        /// </summary>
        public void Reset()
        {
            this.BackTo(0, true);
        }
    }
}
