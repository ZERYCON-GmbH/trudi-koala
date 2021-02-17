namespace TRuDI.Models.CheckData
{
    /// <summary>
    /// Die Klasse TariffChangeTrigger abstrahiert alle weiteren Triggerformen und damit alle weiteren Tarifumschaltgründe.
    /// Eine Instanz der Klasse TariffChangeTrigger:
    ///	    muss auf genau eine Instanz der folgenden Klassen verweisen:
    ///	 
    ///     – ThresholdTrigger
    ///	    – ExternalTrigger
    ///	    – TimeTrigger
    ///	    
    /// Eine Instanz der Klasse TariffChangeTrigger ist optional.
    ///
    /// </summary>
    public class TariffChangeTrigger
    {
        /// <summary>
        /// Kapselt die Triggerklassen
        /// 
        ///  - ThresholdTrigger
        ///  - ExternalTrigger
        ///  - TimeTrigger
        ///  
        /// eine TriggerInstanz ist in jeder Instanz der Klasse TariffChangeTrigger zu finden.
        /// </summary>
        public TimeTrigger TimeTrigger
        {
            get; set;
        }

    }
}
