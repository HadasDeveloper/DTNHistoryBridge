using System;
using System.Collections.Generic;
using System.Data;
using DTNHistoryBridge.Helpers;

namespace DTNHistoryBridge
{
    public class DataManager
    {
        private readonly DataFeed feed = new DataFeed();
        private readonly DataTable tickers = new DataTable();
        private readonly Queue<string> tickersList = new Queue<string>();

        public FeedState State { get { return feed.state; } private set { } }
        public FeedMode Mode { get { return feed.mode; } set { feed.mode = value; } }
        public FeedPeriod Period { get { return feed.Period; } set { feed.Period = value; } }
        public int Port { get { return feed.port; } set { feed.port = value; } }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime counter;

        public DataManager(int port, FeedMode mode)
        {
            feed.port = port;
            feed.mode = mode;
            feed.Bars = new QuoteDataTable();
            feed.IsFinishedReceiving += this_IsFinishedReceiving;

            DataHelper.Connect("Signals");
            switch (mode)
            {
                case FeedMode.EOD:
                    tickers = DataHelper.GetAllSymbols(true);
                    break;
                case FeedMode.EOD_Weekly:
                    tickers = DataHelper.GetAllSymbols(true);
                    break;
                case FeedMode.Fundamental:
                    tickers = DataHelper.GetAllSymbols(true);
                    break;
                case FeedMode.IntraDay_Backtesting:
                    tickers = DataHelper.GetSymbolsForBackTesting();
                    break;
                case FeedMode.EOD_Backtesting:
                    tickers = DataHelper.GetSymbolsForBackTesting();
                    break;
                default:
                    tickers = DataHelper.GetAllSymbols(false);
                    break;
            }

            DataHelper.Disconnect();

            for (int i = 0; i < tickers.Rows.Count; i++)
                tickersList.Enqueue(tickers.Rows[i][0].ToString());
            
            //tickersList.Enqueue("SPY");
        }

        public void InitilizeDataUpload()
        {
            if (tickersList.Count > 0)
            {
                //Truncate previous data
                DataHelper.Connect("Signals");
                if (Mode == FeedMode.Fundamental)
                    DataHelper.TruncateTable("FundamentalData");
                if (Mode == FeedMode.IntraDay && Period == FeedPeriod.Historical)
                    DataHelper.TruncateTable("HistoryIntraday");
                if (Mode == FeedMode.EOD && Period == FeedPeriod.Historical)
                    DataHelper.TruncateTable("HistoryEndOfDay");
                if (Mode == FeedMode.EOD_Weekly && Period == FeedPeriod.Historical)
                    DataHelper.TruncateTable("HistoryEndOfDay_Weekly");
                if (Mode == FeedMode.IntraDay_Backtesting)
                    DataHelper.TruncateTable("HistoryIntraDay");
                if (Mode == FeedMode.EOD_Backtesting)
                    DataHelper.DeleteIntradayBarsForBacktesting("HistoryEndOfDay_Backtest", StartDate, EndDate);

                DataHelper.Disconnect();
                
                //Start retreival process by kicking off the first data request
                string ticker = tickersList.Dequeue();

                counter = DateTime.Now;
                GetNextBatchOfData(ticker);
            }
            else
                feed.state = FeedState.Done;
        }

        private void GetNextBatchOfData(string ticker)
        {
            Console.WriteLine(string.Format("ticker : {0}, time:{1}", ticker, DateTime.Now.Subtract(counter).TotalSeconds) );
            counter = DateTime.Now;

            switch (Mode)
            {
                case FeedMode.EOD:
                    feed.RetrievesHistoryEndOfDay(ticker, EndDate, StartDate);
                    break;
                case FeedMode.EOD_Weekly:
                    feed.RetrievesHistoryEndOfDay_Weekly(ticker, EndDate, StartDate);
                    break;
                case FeedMode.IntraDay:
                    feed.RetrievesHistoryIntraDay(ticker);
                    break;
                case FeedMode.Fundamental:
                    //feed.RetrievesFundamentalMessage(ticker);
                    break;
                case FeedMode.IntraDay_Backtesting:
                    feed.RetrievesHistoryIntraDayForBacktesting(ticker, StartDate, EndDate);
                    break;
                case FeedMode.EOD_Backtesting:
                    feed.RetrievesHistoryEndOfDay(ticker, StartDate, EndDate);
                    break;
            }
        }

        private static void feed_DisconnectedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Done");
        }

        private void UploadDataToDB()
        {
            DataHelper.Connect("Signals");

            if (Period == FeedPeriod.Daily)
            {
                DataHelper.DeleteIntradayBarsForCurrentDay(DateTime.Now);
                DataHelper.InsertDataTable(feed.Bars, "InsertHistoryIntraDayData", "@TPV_OHLC");
                feed.Bars = null;
            }

            if (Period == FeedPeriod.Historical && Mode == FeedMode.IntraDay)
            {
                DataHelper.InsertDataTable(feed.Bars, "InsertHistoryIntraDayData", "@TPV_OHLC");
                feed.Bars = new QuoteDataTable();
            }

            if (Period == FeedPeriod.Historical && Mode == FeedMode.EOD)
            {
                DataHelper.InsertDataTable(feed.Bars, "InsertHistoryEndOfDayData", "@TPV_OHLC");
                feed.Bars = new QuoteDataTable();
            }

            if (Period == FeedPeriod.Historical && Mode == FeedMode.EOD_Weekly)
            {
                DataHelper.InsertDataTable(feed.Bars, "InsertHistoryEndOfDayData_Weekly", "@TPV_OHLC");
                feed.Bars = new QuoteDataTable();
            }

            if (Period == FeedPeriod.Historical && Mode == FeedMode.IntraDay_Backtesting)
            {
                DataHelper.InsertDataTable(feed.Bars, "InsertHistoryIntraDayData", "@TPV_OHLC");
                feed.Bars = new QuoteDataTable();
            }

            if (Period == FeedPeriod.Historical && Mode == FeedMode.EOD_Backtesting)
            {
                DataHelper.InsertDataTable(feed.Bars, "InsertHistoryEndOfDayData_backtest", "@TPV_OHLC");
                feed.Bars = new QuoteDataTable();
            }


            if (Mode == FeedMode.Fundamental)
            {
                foreach (FundamentalMessage message in feed.Fundamentals)
                {
                    DataHelper.Connect("Signals");
                    DataHelper.ExecuteSQL(message.ToDBInsertQuery("FundamentalData"));
                    DataHelper.Disconnect();
                }
            }

            DataHelper.Disconnect();
        }

        private void this_IsFinishedReceiving(object sender, EventArgs args)
        {
            if (tickersList.Count > 0)
            {
                string ticker = tickersList.Dequeue();
                if (Mode != FeedMode.Fundamental && Period == FeedPeriod.Historical)
                    UploadDataToDB();
                GetNextBatchOfData(ticker);
            }
            else
            {
                if (feed.Bars != null && feed.Bars.Rows.Count > 0)
                    UploadDataToDB();

                if (feed.Fundamentals.Count > 0)
                    UploadDataToDB();

                feed.state = FeedState.Done;
            }

        }
    }
}
