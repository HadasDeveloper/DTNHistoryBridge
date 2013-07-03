using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DTNHistoryBridge.Helpers;
using Microsoft.Win32;

namespace DTNHistoryBridge
{
	public enum FeedState
	{
		Initilized = 0,
		Started = 1,
		Done = 2
	}

	public enum FeedMode
	{
		EOD = 0,
		IntraDay = 1,
		Fundamental = 2,
		SymbolLookUp = 3,
		IntraDay2 = 4,
		IntraDay_Backtesting = 5,
		EOD_Backtesting = 6,
		EOD_Weekly = 7
	}

	public enum FeedPeriod
	{
		Historical = 0,
		Daily = 1
	}

	public class DataFeed
	{
		public delegate void FinishedReceiving(object sender, EventArgs args);
		public event FinishedReceiving IsFinishedReceiving;

		private AsyncCallback m_pfnLookupCallback; // socket communication global variables
		private Socket m_sockLookup;
		private readonly byte[] m_szLookupSocketBuffer = new byte[262144]; // we create the socket buffer global for performance
		private string m_sLookupIncompleteRecord = ""; // stores unprocessed data between reads from the socket
		private bool m_bLookupNeedBeginReceive = true; // flag for tracking when a call to BeginReceive needs called

		private readonly SortedList<string, Bar> barsList = new SortedList<string, Bar>();

		public QuoteDataTable Bars { get; set; }
		public List<FundamentalMessage> Fundamentals = new List<FundamentalMessage>();
		public SortedList<string,Bar> BarsList { get { return barsList; } }

		public FundamentalMessage FundamentalMessage { get; set; }

		public FeedState state = FeedState.Initilized;
		public FeedMode mode;
		public int port;
		public FeedPeriod Period;

		public DataFeed()
		{

			m_sockLookup = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress ipLocalhost = IPAddress.Parse("127.0.0.1");

			int iPort = GetIQFeedPort("Lookup");
			IPEndPoint ipendLocalhost = new IPEndPoint(ipLocalhost, iPort);

			for (int i = 0; i < 10; i++)
			{
				try
				{
					m_sockLookup.Connect(ipendLocalhost);
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine(@"login failed : " + i);
					Thread.Sleep(3000);
				}
			}
		}

		private static int GetIQFeedPort(string sPort)
		{
			int iReturn = 0;
			RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\DTN\\IQFeed\\Startup");
			if (key != null)
			{
				string sData = "";
				switch (sPort)
				{
					case "Level1":
						// the default port for Level 1 data is 5009.
						sData = key.GetValue("Level1Port", "5009").ToString();
						break;
					case "Lookup":
						// the default port for Lookup data is 9100.
						sData = key.GetValue("LookupPort", "9100").ToString();
						break;
					case "Level2":
						// the default port for Level 2 data is 9200.
						sData = key.GetValue("Level2Port", "9200").ToString();
						break;
					case "Admin":
						// the default port for Admin data is 9300.
						sData = key.GetValue("AdminPort", "9200").ToString();
						break;
				}
				iReturn = Convert.ToInt32(sData);
			}
			return iReturn;
		}

		private void GetHistory(string sRequest)
		{

			// send it to the feed via the socket
			byte[] szRequest = Encoding.ASCII.GetBytes(sRequest);

			int iBytesToSend = szRequest.Length;
			m_sockLookup.Send(szRequest, iBytesToSend, SocketFlags.None);

            WaitForData("History");
		}

		public void RetrievesHistoryEndOfDay(string ticker, DateTime beginDate, DateTime endDate)
		{
			string d1 = string.Format("{0:yyyyMMdd}", beginDate);
			string d2 = string.Format("{0:yyyyMMdd}", endDate);
			string command = String.Format("HDT,{0},{1},{2},,1,{3} \r\n", ticker, d1, d2, ticker);
            GetHistory(command);
		}

		public void RetrievesHistoryEndOfDay_Weekly (string ticker, DateTime beginDate, DateTime endDate)
		{
			string command = String.Format("HWT,{0},104,1,{1} \r\n", ticker, ticker);
            GetHistory(command);
        }

		public void RetrievesHistoryIntraDay(string ticker)
		{
			//Defualt number of intraday historical days to bring
			const string maxDataPoints = "";
			int feedPeriod = (Period == FeedPeriod.Daily) ? 1 : 150;

			string command = String.Format("HID,{0},300,{1},{2},093000,160000,1,{3} \r\n", ticker, feedPeriod, maxDataPoints ,ticker);
            GetHistory(command);
        }

		public void RetrievesHistoryIntraDayForBacktesting(string ticker, DateTime? startDate, DateTime? endDate)
		{
			string command = String.Format("HID,{0},300,365,,093000,160000,1,{1} \r\n", ticker, ticker);
            GetHistory(command);
        }

        private void WaitForData(string sSocketName)
        {
            if (sSocketName.Equals("History"))
            {
                // make sure we have a callback created
                if (m_pfnLookupCallback == null)
                    m_pfnLookupCallback = new AsyncCallback(OnReceive);

                // send the notification to the socket.  It is very important that we don't call Begin Reveive more than once per call
                // to EndReceive.  As a result, we set a flag to ignore multiple calls.
                if (m_bLookupNeedBeginReceive)
                {
                    m_bLookupNeedBeginReceive = false;

                    // we pass in the sSocketName in the state parameter so that we can verify the socket data we receive is the data we are looking for
                    m_sockLookup.BeginReceive(m_szLookupSocketBuffer, 0, m_szLookupSocketBuffer.Length, SocketFlags.None, m_pfnLookupCallback, sSocketName);
                }
            }
        }

        private void OnReceive(IAsyncResult asyn)
        {
            if (asyn.AsyncState.ToString().Equals("History"))
            {
                int iReceivedBytes = 0;
                iReceivedBytes = m_sockLookup.EndReceive(asyn);
                m_bLookupNeedBeginReceive = true;

                string sData = m_sLookupIncompleteRecord + Encoding.ASCII.GetString(m_szLookupSocketBuffer, 0, iReceivedBytes);
                m_sLookupIncompleteRecord = "";

                int iNewLinePos = sData.IndexOf("\n");
                int iPos = 0;

                while (iNewLinePos >= 0)
                {
                    iPos = iNewLinePos + 1;
                    iNewLinePos = sData.IndexOf("\n", iPos);
                }

                if (sData.Length > iPos)
                {
                    m_sLookupIncompleteRecord = sData.Substring(iPos);
                    sData = sData.Remove(iPos);
                }


                List<String> lstMessages = new List<string>(sData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                foreach (var lstMessage in lstMessages)
                {
                    if (lstMessage.IndexOf("!ENDMSG!") > -1)
                    {
                        IsFinishedReceiving(this, new EventArgs());
                        return;
                    }

                    try
                    {
                        string instrument = lstMessage.Split(',')[0];
                        DateTime date = DateTime.Parse(lstMessage.Split(',')[1]);
                        double high = double.Parse(lstMessage.Split(',')[2]);
                        double low = double.Parse(lstMessage.Split(',')[3]);
                        double open = double.Parse(lstMessage.Split(',')[4]);
                        double close = double.Parse(lstMessage.Split(',')[5]);
                        int volume = Int32.Parse(lstMessage.Split(',')[6]);

                        Bars.Rows.Add(instrument,
                                      mode == FeedMode.EOD ? date.Date : date,
                                      open,
                                      high,
                                      low,
                                      close,
                                      volume);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                // call wait for data to notify the socket that we are ready to receive another callback
                WaitForData("History");
            }
        }
	}
}
