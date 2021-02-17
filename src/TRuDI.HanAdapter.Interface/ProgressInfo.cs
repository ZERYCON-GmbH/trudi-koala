namespace TRuDI.HanAdapter.Interface
{
    /// <summary>
    /// Callback data for UI updates during device readout operations.
    /// </summary>
    public class ProgressInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressInfo"/> class used to signal an intermediate progress bar.
        /// </summary>
        /// <param name="message">The message text to display.</param>
        public ProgressInfo(string message)
        {
            this.Message = message;
            this.Progress = UnknownProgress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressInfo" /> class.
        /// </summary>
        /// <param name="progress">The progress in percent (0 to 100).</param>
        /// <param name="message">The message text to display.</param>
        public ProgressInfo(int progress, string message)
        {
            this.Message = message;
            this.Progress = progress;
        }

        /// <summary>
        /// No information about the actual progress available.
        /// </summary>
        public const int UnknownProgress = -1;

        /// <summary>
        /// Message to show on the UI.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates the overall readout progress in percent (0 to 100). 
        /// Set to <see cref="UnknownProgress"/> if there's no information about the actual progress available.
        /// </summary>
        public int Progress { get; set; }
    }
}