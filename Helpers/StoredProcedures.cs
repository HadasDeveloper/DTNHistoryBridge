namespace DTNHistoryBridge.Helpers
{
    class StoredProcedures
    {
        public const string SqlTruncateTable = " Truncate Table {0} ";

        public static string GetMostLiquidEtfs()
        {
            string sql = " select replace(ticker,'Eqt_','') as ticker " +
                         " from MarketData.dbo.Tickers " +
                         " where category = 'Most Liquid Etfs' and SUBSTRING(ticker,1,2) != 'ID' " +
                         " order by ticker";
            return sql;
        }

        public static string InsertToMarketSnapshot()
        {
            string sql = " insert into MarketSnapshot " +
                         " values(''{0}'',''{1}'',{2},{3},{4},{5},{6}) ";
            return sql;
        }

        public static string TruncateMarketSnapshot()
        {
            string sql = " truncate table MarketSnapshot ";
            return sql;
        }

        public const string GetAllSymbols = " select distinct symbol from [dbo].[Ls_RSI_01_DataSetSymbols] ";

        public const string GetAllSymbolsForBackTesting = " select distinct symbol from [dbo].[Ls_RSI_01_DataSetSymbols] ";

        public const string GetAllSymbolsForEOD = " select distinct symbol from [dbo].[Ls_RSI_01_DataSetSymbols] ";
        //public const string GetAllSymbolsForEOD = " select distinct symbol from Data_ActiveSecurities ";

        public const string DeleteIntradayBarsForCurrentDay = " delete from HistoryIntraDay where date > '{0}'";

        public const string DeleteIntradayBarsForBacktesting = " delete from {0} where date > '{1}' and date < '{2}'";

    }
}

