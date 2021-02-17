namespace TRuDI.TafAdapter.Interface
{
    using System;

    /// <summary>
    /// Result object of TAF adapters.
    /// </summary>
    public class TafAdapterData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TafAdapterData"/> class.
        /// </summary>
        /// <param name="summaryView">The summary view component class.</param>
        /// <param name="detailView">The detail view component class.</param>
        /// <param name="data">The data passed to the view component.</param>
        public TafAdapterData(Type summaryView, Type detailView, ITafData data)
        {
            this.SummaryView = summaryView;
            this.DetailView = detailView;
            this.Data = data;
        }

        /// <summary>
        /// Gets the type of the summary view component class.
        /// </summary>
        public Type SummaryView { get; }

        /// <summary>
        /// Gets the type of the detail view component class.
        /// </summary>
        public Type DetailView { get; }

        /// <summary>
        /// Gets the calculated TAF data. This object is passed by the backend to the view components.
        /// </summary>
        public ITafData Data { get; }
    }
}