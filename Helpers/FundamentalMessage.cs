using System;
using System.Text;

namespace DTNHistoryBridge.Helpers
{
    public class FundamentalMessage
    {
        public string[] items;
        public string Symbol { get { return ConvertToString(2); } }
        public string ExchangeID { get { return ConvertToString(3); } }
        public double PriceEarningsRatio { get { return ConvertToDouble(4); } }
        public int AverageVolume { get { return ConvertToInt32(5); } }
        public double Hi52Week { get { return ConvertToDouble(6); } }
        public double Lo52Week { get { return ConvertToDouble(7); } }
        public double HiCalendarYear { get { return ConvertToDouble(8); } }
        public double LoCalendarYear { get { return ConvertToDouble(9); } }
        public double DividendYield { get { return ConvertToDouble(10); } }
        public double DividendAmount { get { return ConvertToDouble(11); } }
        public double DividendRate { get { return ConvertToDouble(12); } }
        public DateTime PayDate { get { return ConvertToDate(13); } }
        public DateTime ExDividendDate { get { return ConvertToDate(14); } }
        public double CurrentYearEarningsPerShare { get { return ConvertToDouble(20); } }
        public double NextYearEarningsPerShare { get { return ConvertToDouble(21); } }
        public double FiveYearGrowthPercentage { get { return ConvertToDouble(22); } }
        public int FiscalYearEnd { get { return ConvertToInt32(23); } }
        public string CompanyName { get { return ConvertToString(25); } }
        public string RootOptionSymbols { get { return ConvertToString(26); } }
        public double PercentHeldByInstitutions { get { return ConvertToDouble(27); } }
        public double Beta { get { return ConvertToDouble(28); } }
        public string Leaps { get { return ConvertToString(29); } }
        public double CurrentAssets { get { return ConvertToDouble(30); } }
        public double CurrentLiabilities { get { return ConvertToDouble(31); } }
        public DateTime BalanceSheetDate { get { return ConvertToDate(32); } }
        public double LongTermDebt { get { return ConvertToDouble(33); } }
        public double CommonSharesOutstanding { get { return ConvertToDouble(34); } }
        public double SplitFactor1Ratio { get { return m_SplitFactor1Ratio; } }
        public DateTime SplitFactor1Date { get { return m_SplitFactor1Date; } }
        public double SplitFactor2Ratio { get { return m_SplitFactor2Ratio; } }
        public DateTime SplitFactor2Date { get { return m_SplitFactor2Date; } }
        public string MarketCenter { get { return ConvertToString(39); } }
        public string FormatCode { get { return ConvertToString(40); } }
        public int Precision { get { return ConvertToInt32(41); } }
        public int SIC { get { return ConvertToInt32(42); } }
        public double HistoricalVolatility { get { return ConvertToDouble(43); } }
        public string SecurityType { get { return ConvertToString(44); } }
        public string ListedMarket { get { return ConvertToString(45); } }
        public DateTime Hi52WeekDate { get { return ConvertToDate(46); } }
        public DateTime Lo52WeekDate { get { return ConvertToDate(47); } }
        public DateTime HiCalendarYearDate { get { return ConvertToDate(48); } }
        public DateTime LoCalendarYearDate { get { return ConvertToDate(49); } } 

        private readonly double m_SplitFactor1Ratio;
        private readonly DateTime m_SplitFactor1Date;
        private readonly double m_SplitFactor2Ratio;
        private readonly DateTime m_SplitFactor2Date;

        public FundamentalMessage( string[] items )
        {
            char[] sep = {' '};
            int ix = 0;

            try
            {
                ix = 36 - 1;
                if (string.IsNullOrEmpty(items[ix]))
                {
                    m_SplitFactor1Ratio = 0;
                    m_SplitFactor1Date = DateTime.Now;
                }
                else
                {
                    string t1 = items[ix];
                    string[] t2 = t1.Split(sep);
                    if (t2[0] != "")
                        m_SplitFactor1Ratio = double.Parse(t2[0]);
                    m_SplitFactor1Date = ConvertToDate(t2[1]);
                }

                ix = 37 - 1;
                if (string.IsNullOrEmpty(items[ix]))
                {
                    m_SplitFactor2Ratio = 0;
                    m_SplitFactor2Date = DateTime.Now;
                }
                else
                {
                    string t1 = items[ix];
                    string[] t2 = t1.Split(sep);
                    if (t2[0] != "")
                        m_SplitFactor2Ratio = double.Parse(t2[0]);
                    m_SplitFactor2Date = ConvertToDate(t2[1]);
                }
            }
            catch
            {
                Console.WriteLine("FundamentalMessage Conversion problem at item {0}", ix);
                throw new Exception("FundamentalMessage Conversion Error");
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
            sb.Append("'" + Symbol + "',");
            sb.Append("'" + ExchangeID + "',");
            sb.Append(PriceEarningsRatio + ",");
            sb.Append(AverageVolume + ",");
            sb.Append(Hi52Week + ",");
            sb.Append(Lo52Week + ",");
            sb.Append(HiCalendarYear + ",");
            sb.Append(LoCalendarYear + ",");
            sb.Append(DividendYield + ",");
            sb.Append(DividendAmount + ",");
            sb.Append(DividendRate + ",");
            sb.Append("'" + PayDate + "',");
            sb.Append("'" + ExDividendDate + "',");
            sb.Append(CurrentYearEarningsPerShare + ",");
            sb.Append(NextYearEarningsPerShare + ",");
            sb.Append(FiveYearGrowthPercentage + ",");
            sb.Append(FiscalYearEnd + ",");
            sb.Append("'" + CompanyName + "',");
            sb.Append("'" + RootOptionSymbols + "',");
            sb.Append(PercentHeldByInstitutions + ",");
            sb.Append(Beta + ",");
            sb.Append("'" + Leaps + "',");
            sb.Append(CurrentAssets + ",");
            sb.Append(CurrentLiabilities + ",");
            sb.Append("'" + BalanceSheetDate + "',");
            sb.Append(LongTermDebt + ",");
            sb.Append(CommonSharesOutstanding + ",");
            sb.Append(SplitFactor1Ratio + ",");
            sb.Append("'" + SplitFactor1Date + "',");
            sb.Append(SplitFactor2Ratio + ",");
            sb.Append("'" + SplitFactor2Date + "',");
            sb.Append("'" + MarketCenter + "',");
            sb.Append("'" + FormatCode + "',");
            sb.Append(Precision + ",");
            sb.Append(SIC + ",");
            sb.Append(HistoricalVolatility + ",");
            sb.Append("'" + SecurityType + "',");
            sb.Append("'" + ListedMarket + "',");
            sb.Append("'" + Hi52WeekDate + "',");
            sb.Append("'" + Lo52WeekDate + "',");
            sb.Append("'" + HiCalendarYearDate + "',");
            sb.Append("'" + LoCalendarYearDate + "'");
            sb.Append(")");

            return string.Format(sb.ToString(),tableName);
        }
    }
}
