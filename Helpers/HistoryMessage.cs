using System;

namespace DTNHistoryBridge.Helpers
{
    public class HistoryMessage
    {
        public string[] items;

        public string Instrument { get { return ConvertToString(1); } }
        public DateTime Date { get { return ConvertToDateTime(2); } }
        public double High { get { return ConvertToDouble(3); } }
        public double Low { get { return ConvertToDouble(4); } }
        public double Open { get { return ConvertToDouble(5); } }
        public double Close { get { return ConvertToDouble(6); } }
        public double Volume { get { return ConvertToDouble(7); } }
        public double PeriodVolume { get { return ConvertToDouble(8); } }
        public string Extra { get { return ConvertToString(9); } }

        public HistoryMessage(string[] items)
        {
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

        protected DateTime ConvertToDateTime(int ord)
        {
            DateTime dt = DateTime.Parse(items[ord - 1]);
            return dt;
        }
    }
}
