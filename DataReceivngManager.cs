using System;
using System.Collections.Generic;
using System.Data;
using DTNHistoryBridge.Helpers;

namespace DTNHistoryBridge
{
    public class DataReceivngManager
    {
        private readonly DataFeed feed = new DataFeed();
        public FeedState State { get { return feed.state; }}
        private const int PORT = 5009;

        public DataReceivngManager()
        {
            feed.port = PORT;
            feed.mode = FeedMode.IntraDay2;
            feed.Bars = new QuoteDataTable();
        }

        public void Start()
        {
            DataHelper.Connect("Signals");
            DataTable tickers = DataHelper.GetAllSymbols(false);
            DataHelper.Disconnect();

            //for (int i = 0; i < tickers.Rows.Count; i++)
            //    feed.StartWatchingSymbol(tickers.Rows[i][0].ToString());
            //feed.StartWatchingSymbol("MSFT");
        }

        public void UpdateDB(DateTime time)
        {
            if (feed.BarsList == null || feed.BarsList.Count == 0)
                return;

            Console.WriteLine(time.ToString());
            QuoteDataTable table = new QuoteDataTable();

            lock (feed.BarsList)
            {
                foreach (KeyValuePair<string, Bar> pair in feed.BarsList)
                {
                    table.Rows.Add(pair.Value.Instrument,
                                   time,
                                   pair.Value.Open,
                                   pair.Value.High,
                                   pair.Value.Low,
                                   pair.Value.Close,
                                   pair.Value.Volume);
                }
            }

            DataHelper.Connect("Signals");
            DataHelper.InsertDataTable(table, "InsertHistoryIntraDayData", "@TPV_OHLC");
            DataHelper.Disconnect();
        }
    }
}
