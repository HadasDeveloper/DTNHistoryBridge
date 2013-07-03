using System;
using System.Text;

namespace DTNHistoryBridge.Helpers
{
    public class UpdateMessage
    {
        private readonly TimeSpan tsTime = new TimeSpan(0);
        private readonly string sType = "";

        public string[] items;
        public string Symbol { get { return ConvertToString(2); } }
        public string ExchangeID { get { return ExchangeCodes.Code(ConvertToString(3)); } }
        public double Last { get { return ConvertToDouble(4); } }
        public double Change { get { return ConvertToDouble(5); } }
        public double PercentChange { get { return ConvertToDouble(6); } }
        public int TotalVolume { get { return ConvertToInt32(7); } }
        public int LastSize { get { return ConvertToInt32(8); } }
        public double High { get { return ConvertToDouble(9); } }
        public double Low { get { return ConvertToDouble(10); } }
        public double Bid { get { return ConvertToDouble(11); } }
        public double Ask { get { return ConvertToDouble(12); } }
        public int BidSize { get { return ConvertToInt32(13); } }
        public int AskSize { get { return ConvertToInt32(14); } }
        public int TickDirection { get { return ConvertToInt32(15); } }
        public int BidTickDirection { get { return ConvertToInt32(16); } }
        public double Range { get { return ConvertToDouble(17); } }
        public TimeSpan Time { get { return tsTime; } }
        public string Type { get { return sType; } }
        public int OpenInterest { get { return ConvertToInt32(19); } }
        public double Open { get { return ConvertToDouble(20); } }
        public double Close { get { return ConvertToDouble(21); } }
        public double Spread { get { return ConvertToDouble(22); } }
        public double Strike { get { return ConvertToDouble(23); } }
        public double Settle { get { return ConvertToDouble(24); } }
        public int Delay { get { return ConvertToInt32(25); } }
        public string MarketCenter { get { return ConvertToString(26); } }
        public string RistrictedCode { get { return ConvertToString(27); } }
        public double NetAssetValueMutualFund { get { return ConvertToDouble(28); } }
        public double AverageMaturity { get { return ConvertToDouble(29); } }
        public double SevenDayYield { get { return ConvertToDouble(30); } }
        public DateTime LastTradeDate { get { return ConvertToDate(31); } }
        public double ExtendedTradingLast { get { return ConvertToDouble(33); } }
        public DateTime ExpirationDate { get { return ConvertToDate(34); } }
        public int RegionalVolume { get { return ConvertToInt32(35); } }
        public double NetAssetValue { get { return ConvertToDouble(36); } }
        public double ExtendedTradingChange { get { return ConvertToDouble(37); } }
        public double ExtendedTradingDifference { get { return ConvertToDouble(38); } }
        public double PriceEarningsRatio { get { return ConvertToDouble(39); } }
        public double AverageTradeVolume30Day { get { return ConvertToDouble(40); } }
        public double BidChange { get { return ConvertToDouble(41); } }
        public double AskChange { get { return ConvertToDouble(42); } }
        public double ChangeFromOpen { get { return ConvertToDouble(43); } }
        public int MarketOpen { get { return ConvertToInt32(44); } }
        public double Volatility { get { return ConvertToDouble(45); } }
        public double MarketCapitalization { get { return ConvertToDouble(46); } }
        public int FractionDisplayCode { get { return ConvertToInt32(47); } }
        public int DecimalPrecision { get { return ConvertToInt32(48); } }
        public string DaysToExpiration { get { return ConvertToString(49); } }
        public int PreviousDaysVolume { get { return ConvertToInt32(50); } }
        public string Regions { get { return ConvertToString(51); } }
        public double OpenRange1 { get { return ConvertToDouble(52); } }
        public double CloseRange1 { get { return ConvertToDouble(53); } }
        public double OpenRange2 { get { return ConvertToDouble(54); } }
        public double CloseRange2 { get { return ConvertToDouble(55); } }
        public int NumberOfTradesToday { get { return ConvertToInt32(56); } }
        public string BidTime { get { return ConvertToString(57); } } // convert to timespan at some point in time
        public string AskTime { get { return ConvertToString(58); } } // convert to timespan at some point in time
        public double VWAP { get { return ConvertToDouble(59); } }
        public int TickID { get { return ConvertToInt32(60); } }
        public string FinancialStatusIndicator { get { return ConvertToString(61); } }

        public UpdateMessage( string[] items )
        {
            int ix = 0;
            
            try
            {

                ix = 18 - 1;

                string t = items[ix];
                switch (t.Length)
                {
                    case 5: // hh:mm
                        tsTime = new TimeSpan(Convert.ToInt32(t.Substring(0, 2)), Convert.ToInt32(t.Substring(3, 2)), 0);
                        throw new Exception("LevelIDataMessage missing type " + items[ix]);
                        break;
                    case 6: // hh:mmc
                        tsTime = new TimeSpan(Convert.ToInt32(t.Substring(0, 2)), Convert.ToInt32(t.Substring(3, 2)), 0);
                        sType = t.Substring(5);
                        break;
                    case 9: // hh:mm:ssc
                        tsTime = new TimeSpan(
                          Convert.ToInt32(t.Substring(0, 2)),
                          Convert.ToInt32(t.Substring(3, 2)),
                          Convert.ToInt32(t.Substring(6, 2)));
                        sType = t.Substring(8);
                        break;
                    default:
                        break;
                }
                if (1 != sType.Length)
                {
                }
            }
            catch
            {
            }

            this.items = items;

        }

        protected double ConvertToDouble(int ord)
        {
            int ix = ord - 1;
            double d = 0;
            if (!string.IsNullOrEmpty(items[ix])) d = Convert.ToDouble(items[ix]);
            return d;
        }

        protected Int32 ConvertToInt32(int ord)
        {
            int ix = ord - 1;
            Int32 i = 0;
            if (!string.IsNullOrEmpty(items[ix])) i = Convert.ToInt32(items[ix]);
            return i;
        }

        protected string ConvertToString(int ord)
        {
            return items[ord - 1];
        }

        protected DateTime ConvertToDate(int ord)
        {
            if (string.IsNullOrEmpty(items[ord - 1].Trim()))
                return DateTime.Now;
            DateTime dt = DateTime.Parse(items[ord - 1]);
            return dt;
        }

        protected DateTime ConvertToDate(string stringDate)
        {
            if (string.IsNullOrEmpty(stringDate.Trim()))
                return DateTime.Now;

            DateTime dt = DateTime.Parse(stringDate);
            return dt;
        }

        public string ToDBInsertQuery(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert into {0} values ( ");
            sb.Append(")");

            return string.Format(sb.ToString(),tableName);
        }
    }
}
