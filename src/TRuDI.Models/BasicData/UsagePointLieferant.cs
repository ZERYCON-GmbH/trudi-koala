namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse UsagePoint repräsentiert den Zählpunkt und stellt das zentrale Datenelement 
    /// einer Nachricht dar.Jede Nachricht muss mindestens einen Zählpunkt beinhalten.
    /// 
    /// Eine Instanz der Klasse UsagePoint:
    ///	    muss auf eine Instanz der Klasse InvoicingParty verweisen
    ///	    IF_Adapter_TRuDI: Usagepoint muss auf eine Instanz der Klasse Customer verweisen
    ///	    IF_Lieferant_TRuDI: Usagepoint kann auf eine Instanz der Klasse Customer verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse SMGW verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse ServiceCategory verweisen
    ///	    
    ///	    kann auf eine Instanz der Klasse AnalysisProfile verweisen 
    /// </summary>
    public class UsagePointLieferant : UsagePoint
    {
    }
}
