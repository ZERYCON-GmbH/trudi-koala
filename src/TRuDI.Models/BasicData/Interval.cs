namespace TRuDI.Models.BasicData
{
    using System;

    /// <summary>
    /// Eine Instanz der Klasse Interval beschreibt einen Zeitraum
    /// durch einen Startzeitpunkt und eine Dauer, angegeben in
    /// ganzzahligen Sekunden.
    /// </summary>
    public class Interval
    {
        /// <summary>
        /// Der Startzeitpunkt des Intervals
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Die Dauer des Intervals. Wird in ganzzahligen Sekunden angegeben.
        /// </summary>
        public uint? Duration { get; set; }
    }
}
