using System;
using System.Data;

namespace DTNHistoryBridge.Helpers
{
    public class QuoteDataTable : DataTable 
    {
        public QuoteDataTable()
        {
            Columns.Add("Instrument", typeof(string));
            Columns.Add("Date", typeof(DateTime));
            Columns.Add("OpenPrice", typeof(float));
            Columns.Add("HighPrice", typeof(float));
            Columns.Add("LowPrice", typeof(float));
            Columns.Add("ClosePrice", typeof(float));
            Columns.Add("Volume", typeof(float));
        }
    }
}
