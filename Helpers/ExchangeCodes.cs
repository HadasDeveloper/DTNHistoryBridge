using System.Collections;

namespace DTNHistoryBridge.Helpers
{
    public static class ExchangeCodes
    {
        public static readonly Hashtable htCodes;

        static ExchangeCodes() 
        {
            htCodes = new Hashtable(50) { {"F", new ExchangeCode("NASDAQ", "Nasdaq")},
                                          {"B", new ExchangeCode("NAS OTC", "Nasdaq OTC")},
                                          {"E", new ExchangeCode("AMEX", "American Stock Exchange ")},
                                          {"D", new ExchangeCode("NYSE", "New York Stock Exchange ")},
                                          {"C", new ExchangeCode("OPRA", "OPRA System ")},
                                          {"IE", new ExchangeCode("DJ", "Dow Jones-Wilshire Indexes (not currently supported) ")},
                                          {"1B", new ExchangeCode("DTN", "DTN (Indexes/Statistics) ")},
                                          {"13", new ExchangeCode("CBOT", "Chicago Board Of Trade")},
                                          {"1D", new ExchangeCode("KCBT", "Kansas City Board Of Trade ")},
                                          {"10", new ExchangeCode("CME", "Chicago Mercantile Exchange ")},
                                          {"14", new ExchangeCode("MGE", "Minneapolis Grain Exchange ")},
                                          {"1A", new ExchangeCode("NYMEX", "New York Mercantile Exchange ")},
                                          {"12", new ExchangeCode("COMEX", "Commodities Exchange Center ")},
                                          {"18", new ExchangeCode("NYBOT", "New York Board Of Trade ")},
                                          {"28", new ExchangeCode("ONECH", "One Chicago")},
                                          {"29", new ExchangeCode("NQLX", "NQLX ")},
                                          {"4", new ExchangeCode("WPG", "Winnipeg Commodities Exchange ")},
                                          {"6", new ExchangeCode("LIFFE", "London International Financial Futures Exchange")},
                                          {"17", new ExchangeCode("LME", "London Metals Exchange ")},
                                          {"8", new ExchangeCode("IPE", "International Petroleum Exchange ")},
                                          {"7", new ExchangeCode("SGX", "Singapore International Monetary Exchange ")},
                                          {"15", new ExchangeCode("EUREX", "European Exchange ")},
                                          {"2", new ExchangeCode("EID", "EURONEXT Index Derivatives ")},
                                          {"9", new ExchangeCode("EIR", "EURONEXT Interest Rates ")},
                                          {"16", new ExchangeCode("EURONEXT", "EURONEXT Commodities ")},
                                          {"48", new ExchangeCode("Tullet", "Tullett Liberty (Forex) ")},
                                          {"49", new ExchangeCode("Barclays", "Barclays Bank (Forex)")},
                                          {"4A", new ExchangeCode("HotSpot", "Hotspot Forex ")},
                                          {"4B", new ExchangeCode("WH", "Warenterminborse Hannover (not currently supported) ")}};
        }

        public static string Code( string id ) 
        {
            string code = id;
            if (htCodes.ContainsKey(id)) 
            {
                ExchangeCode ec = htCodes[id] as ExchangeCode;
                code = ec.code;
            }
            return code;
        }
    }

    public class ExchangeCode 
    {
        public string id;
        internal string code;
        internal string desc;

        public ExchangeCode( string id, string code, string desc ) 
        {
            this.id = id;
            this.code = code;
            this.desc = desc;
        }

        public ExchangeCode( string code, string desc ) 
        {
            this.code = code;
            this.desc = desc;
        }
    }
}
