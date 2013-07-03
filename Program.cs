using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DTNHistoryBridge.Helpers;
using Microsoft.Win32;

namespace DTNHistoryBridge
{
    static class Program
    {
        public static DateTime time = DateTime.Now;

        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            string period = "";
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Today.AddDays(-3650);
            if (args != null && args.Length > 0)
            {
                period = args[0];

                Console.WriteLine(period);
                if (args.Length > 1)
                {
                    DateTime.TryParse(args[1], out startDate);
                    DateTime.TryParse(args[2], out endDate);
                    Console.WriteLine(string.Format("Start:{0} End:{1}", startDate, endDate));
                }
            }

            LaunchingTheFeed();

            switch (period)
            {
                case "Intraday":
                    //Fetch intraday data
                    FetchHisoricData(FeedMode.IntraDay, FeedPeriod.Daily);
                    break;

                case "Intraday2":
                    //Fetch intraday data
                    StartReceivingData();
                    break;

                case "Fundamental":
                    //Fetch intraday data
                    FetchFundamentalData();
                    break;

                case "EndOfDay":
                    //Fetch end of day data
                    FetchHisoricData(FeedMode.EOD, FeedPeriod.Historical);
                    break;

                case "EndOfDayWeekly":
                    //Fetch end of day data
                    FetchHisoricData(FeedMode.EOD, FeedPeriod.Historical);
                    break;

                case "IntradayHistorical":
                    //Fetch end of day data
                    FetchHisoricData(FeedMode.IntraDay, FeedPeriod.Historical);
                    break;

                case "IntradayHistoricalForBackTesting":
                    //Fetch end of day data
                    FetchHisoricData(FeedMode.IntraDay_Backtesting, FeedPeriod.Historical, startDate, endDate);
                    break;

                case "EODHistoricalForBackTesting":
                    //Fetch end of day data
                    FetchHisoricData(FeedMode.EOD_Backtesting, FeedPeriod.Historical, startDate, endDate);
                    break;

                default:
                    //Fetch intraday data
                    FetchHisoricData(FeedMode.IntraDay, FeedPeriod.Daily);
                    break;
            }

            Console.Write("Total Time : " + DateTime.Now.Subtract(time));
        }

        private static void FetchHisoricData(FeedMode mode, FeedPeriod period, DateTime? startDate = null, DateTime? endDate = null)
        {
            DataManager manager = new DataManager(9100,mode){ Period = period,
                                                              StartDate = startDate ?? DateTime.Now,
                                                              EndDate = endDate ?? DateTime.Today.AddDays(-3650)};
            manager.InitilizeDataUpload();

            while (manager.State != FeedState.Done)
            {
                if (mode == FeedMode.IntraDay &&
                    period == FeedPeriod.Daily &&
                    DateTime.Now.Subtract(time).TotalMinutes > 10)
                    break;

                Thread.Sleep(500);
            }

            DataHelper.Connect("Signals");
            DataHelper.FixHistoryIntraday(DateTime.Now);
            DataHelper.Disconnect();
        }

        private static void FetchFundamentalData()
        {
            DataManager manager = new DataManager(5009,FeedMode.Fundamental);
            manager.InitilizeDataUpload();

            while (manager.State != FeedState.Done)
            {
                Thread.Sleep(500);
            }
        }

        private static void StartReceivingData()
        {
            DataReceivngManager manager = new DataReceivngManager();
            DateTime startingTime = DateTime.Parse(DateTime.Now.ToShortDateString()).AddHours(6).AddMinutes(30);
            DateTime endTime = DateTime.Parse(DateTime.Now.ToShortDateString()).AddHours(13);
            manager.Start();

            while (DateTime.Now.AddHours(-9) >= startingTime && DateTime.Now.AddHours(-9) <= endTime)
            {
                Thread.Sleep(6000);
                manager.UpdateDB(DateTime.Now);
            }

            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogWriter.Write(e, "C://Work//Logs//IntraDayRanker//", "log " + DateTime.Today.ToShortDateString() + ".txt", true);
        }

        private static void LaunchingTheFeed()
        {
            if (Process.GetProcesses().Any(clsProcess => clsProcess.ProcessName.Contains("iqconnect")))
                return;

            string sArguments = "";
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\DTN\\IQFeed\\Startup");
            if (key != null)
            {
                sArguments += "-product WEINSTEIN_NITZAN_1474 ";
                sArguments += "-version 0.11111111 ";
                sArguments += "-login " + key.GetValue("Login", "") + " ";
                sArguments += "-password " + key.GetValue("Password", "") + " ";
                sArguments += "-savelogininfo " + key.GetValue("SaveLoginPassword", "0") + " ";
                sArguments += "-autoconnect " + key.GetValue("AutoConnect", "0") + " ";
            }

            Process.Start("IQConnect.exe", sArguments);
        }
    }
}