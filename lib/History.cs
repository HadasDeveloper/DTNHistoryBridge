
//// Sample application to request level1 data and 1 min history

//using System;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Data;
//using DTNHISTORYLOOKUPLib;

//namespace CSharpLevel1Socket
//{
//    /// <summary>
//    /// Summary description for Form1.
//    /// </summary>
//    public class frmMain : System.Windows.Forms.Form
//    {
//        string strSymbol = "";
//        private bool reporting_history = false;

//        private System.Windows.Forms.Label lblSymbol;
//        private System.Windows.Forms.TextBox txtSymbol;
//        private System.Windows.Forms.Button cmdGetData;
//        private System.Windows.Forms.ListBox lstData;
//        private AxIQFEEDYLib.AxIQFeedY IQFeed;
//        private HistoryLookupClass m_HistoryLookupClass;
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.Container components = null;

//        public frmMain()
//        {
//            //
//            // Required for Windows Form Designer support
//            //
//            InitializeComponent();

//            //
//            // TODO: Add any constructor code after InitializeComponent call
//            //

//            this.m_HistoryLookupClass = new HistoryLookupClass();
//            this.m_HistoryLookupClass.MinuteCompleted +=
//            new _IHistoryLookupEvents_MinuteCompletedEventHandler
//            (m_HistoryLookupClass_MinuteCompleted);
//        }

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (components != null)
//                {
//                    components.Dispose();
//                }

//                this.IQFeed.RemoveClientApp();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code
//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
//            this.lblSymbol = new System.Windows.Forms.Label();
//            this.txtSymbol = new System.Windows.Forms.TextBox();
//            this.cmdGetData = new System.Windows.Forms.Button();
//            this.lstData = new System.Windows.Forms.ListBox();
//            this.IQFeed = new AxIQFEEDYLib.AxIQFeedY();
//            ((System.ComponentModel.ISupportInitialize)(this.IQFeed)).BeginInit();
//            this.SuspendLayout();
//            //
//            // lblSymbol
//            //
//            this.lblSymbol.Location = new System.Drawing.Point(8, 8);
//            this.lblSymbol.Name = "lblSymbol";
//            this.lblSymbol.Size = new System.Drawing.Size(48, 16);
//            this.lblSymbol.TabIndex = 0;
//            this.lblSymbol.Text = "Symbol: ";
//            //
//            // txtSymbol
//            //
//            this.txtSymbol.Location = new System.Drawing.Point(72, 8);
//            this.txtSymbol.Name = "txtSymbol";
//            this.txtSymbol.Size = new System.Drawing.Size(136, 20);
//            this.txtSymbol.TabIndex = 1;
//            this.txtSymbol.Text = "txtSymbol";
//            //
//            // cmdGetData
//            //
//            this.cmdGetData.Location = new System.Drawing.Point(216, 8);
//            this.cmdGetData.Name = "cmdGetData";
//            this.cmdGetData.Size = new System.Drawing.Size(88, 24);
//            this.cmdGetData.TabIndex = 2;
//            this.cmdGetData.Text = "&Get Data";
//            this.cmdGetData.Click += new System.EventHandler(this.cmdGetData_Click);
//            //
//            // lstData
//            //
//            this.lstData.HorizontalScrollbar = true;
//            this.lstData.Location = new System.Drawing.Point(8, 40);
//            this.lstData.Name = "lstData";
//            this.lstData.Size = new System.Drawing.Size(656, 316);
//            this.lstData.TabIndex = 3;
//            //
//            // IQFeed
//            //
//            this.IQFeed.Enabled = true;
//            this.IQFeed.Location = new System.Drawing.Point(480, 8);
//            this.IQFeed.Name = "IQFeed";
//            this.IQFeed.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("IQFeed.OcxState")));
//            this.IQFeed.Size = new System.Drawing.Size(100, 24);
//            this.IQFeed.TabIndex = 4;
//            this.IQFeed.Visible = false;
//            this.IQFeed.SummaryMessage +=
//            new AxIQFEEDYLib._DIQFeedYEvents_SummaryMessageEventHandler(this.IQFeed_SummaryMessage);
//            this.IQFeed.FundamentalMessage +=
//            new AxIQFEEDYLib._DIQFeedYEvents_FundamentalMessageEventHandler(this.IQFeed_FundamentalMessage);
//            this.IQFeed.QuoteMessage +=
//            new AxIQFEEDYLib._DIQFeedYEvents_QuoteMessageEventHandler(this.IQFeed_QuoteMessage);
//            this.IQFeed.SystemMessage +=
//            new AxIQFEEDYLib._DIQFeedYEvents_SystemMessageEventHandler(this.IQFeed_SystemMessage);
//            this.IQFeed.TimeStampMessage +=
//            new AxIQFEEDYLib._DIQFeedYEvents_TimeStampMessageEventHandler(this.IQFeed_TimeStampMessage);
//            //
//            // frmMain
//            //
//            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
//            this.ClientSize = new System.Drawing.Size(672, 364);
//            this.Controls.Add(this.IQFeed);
//            this.Controls.Add(this.lstData);
//            this.Controls.Add(this.cmdGetData);
//            this.Controls.Add(this.txtSymbol);
//            this.Controls.Add(this.lblSymbol);
//            this.MaximizeBox = false;
//            this.Name = "frmMain";
//            this.Text = "frmMain";
//            this.Load += new System.EventHandler(this.frmMain_Load);
//            ((System.ComponentModel.ISupportInitialize)(this.IQFeed)).EndInit();
//            this.ResumeLayout(false);

//        }
//        #endregion

//        /// <summary>
//        /// The main entry point for the application.
//        /// </summary>
//        [STAThread]
//        static void Main()
//        {
//            Application.Run(new frmMain());
//        }

//        //Form load handler, register client app here
//        private void frmMain_Load(object sender, System.EventArgs e)
//        {
//            this.Text = "CSharp Level1 Socket...";
//            this.txtSymbol.Text = "";
//            this.txtSymbol.Enabled = false;

//            string strName = "IQFEED_DEMO";
//            string strVersion = "1.0";
//            string strKey = "1.0";

//            this.IQFeed.RegisterClientApp(ref strName, ref strVersion, ref strKey);
//        }

//        //Watch symbol when the get data button is clicked
//        private void cmdGetData_Click(object sender, System.EventArgs e)
//        {
//            strSymbol = txtSymbol.Text.Trim();
//            if (strSymbol != "")
//            {
//                IQFeed.WatchSymbol(ref strSymbol);
//            }
//        }

//        //Received fundamental message, add to list box
//        private void IQFeed_FundamentalMessage(object sender, AxIQFEEDYLib._DIQFeedYEvents_FundamentalMessageEvent e)
//        {
//            string strData = e.strFundamentalData;
//            lstData.Items.Add(strData);
//        }

//        //Received summary message, add to list box
//        private void IQFeed_SummaryMessage(object sender, AxIQFEEDYLib._DIQFeedYEvents_SummaryMessageEvent e)
//        {
//            string strData = e.strSummaryData;
//            lstData.Items.Add(strData);
//        }

//        //Received quote message, add to list box
//        private void IQFeed_QuoteMessage(object sender, AxIQFEEDYLib._DIQFeedYEvents_QuoteMessageEvent e)
//        {
//            string strData = e.strQuoteData;
//            lstData.Items.Add(strData);
//        }

//        // First Time you receive the System message indicates you are connected.
//        private void IQFeed_SystemMessage(object sender, AxIQFEEDYLib._DIQFeedYEvents_SystemMessageEvent e)
//        {
//            string strData = e.strSystemData;
//            txtSymbol.Enabled = true;
//            lstData.Items.Add(strData);
//        }

//        //Received Time Stamp message, add to list box, request history
//        private void IQFeed_TimeStampMessage(object sender, AxIQFEEDYLib._DIQFeedYEvents_TimeStampMessageEvent e)
//        {
//            string strData = e.strTimeStampData;
//            lstData.Items.Add(strData);

//            if (strSymbol != "")
//            {
//                this.m_HistoryLookupClass.RequestMinuteHistory(strSymbol, 1, 1);
//            }
//        }


//        private void m_HistoryLookupClass_MinuteCompleted(int lMinutesLoaded,
//        object Time,
//        object Open,
//        object Close,
//        object High,
//        object Low,
//        object Volume,
//        object IntVolume)
//        {
//            if (!reporting_history)
//            {
//                reporting_history = true;

//                DateTime[] m_TimeArray = (DateTime[])Time;
//                double[] m_OpenArray = (double[])Open;
//                double[] m_CloseArray = (double[])Close;
//                double[] m_HighArray = (double[])High;
//                double[] m_LowArray = (double[])Low;
//                int[] m_VolumeArray = (int[])Volume;
//                int[] m_IntVolumeArray = (int[])IntVolume;

//                for (long i = lMinutesLoaded; i > 0; i--)
//                {
//                    Console.Write("Time: {0}, ", m_TimeArray.GetValue(i));
//                    Console.Write("Open: {0,8:F}, ", m_OpenArray.GetValue(i));
//                    Console.Write("Close: {0,8:F}, ", m_CloseArray.GetValue(i));
//                    Console.Write("High: {0,8:F}, ", m_HighArray.GetValue(i));
//                    Console.Write("Low: {0,8:F}, ", m_LowArray.GetValue(i));
//                    Console.Write("Volume:{0,6}, ", m_VolumeArray.GetValue(i));
//                    Console.Write("IntVolume:{0,6}, ", m_IntVolumeArray.GetValue(i));
//                    Console.WriteLine();
//                }

//                reporting_history = false;
//            }
//        }
//    }
//}
