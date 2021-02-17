namespace TRuDI.Backend.Models
{
    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// Stores the current progress page state.
    /// </summary>
    public class ProgressData
    {
        public ProgressData()
        {
            this.Progress = ProgressInfo.UnknownProgress;
        }

        public string Title { get; set; }

        public string DetailsViewName { get; set; }

        public string NextPageAfterProgress { get; set; }

        public string StatusText { get; set; }

        public int Progress { get; set; }

        public void Reset(string title = null, string detailsViewName = null)
        {
            this.NextPageAfterProgress = null;
            this.Progress = -1;
            this.StatusText = string.Empty;
            this.DetailsViewName = null;
            this.Title = title;
            this.DetailsViewName = detailsViewName;
        }
    }
}
