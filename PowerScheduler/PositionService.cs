using System;
using System.Timers;
using System.Configuration;
using Topshelf.Logging;

namespace PowerScheduler
{
    public class PositionService
    {
        #region Fields / Properties

        private readonly string _csvLocation = ConfigurationManager.AppSettings["CsvFileLocation"];
        private readonly string _refreshInterval = ConfigurationManager.AppSettings["RefreshInterval"];
        private readonly Timer _timer;
        private static readonly LogWriter _log = HostLogger.Get<PositionService>();

        private TradeProcessor _tradeProcessor { get; set; }
        private DailyPosition _position { get; set; }
        private DateTime DayAhead
        {
            get { return DateTime.Now.AddDays(1).Date; }
        }
        private PositionSaver _positionSaver { get; set; }

        #endregion

        #region Constructor

        public PositionService()
        {
            _tradeProcessor = new TradeProcessor();
            _tradeProcessor.TradeRetrievalFailed += Restart;

            _position = null;

            _timer = new Timer()
            {
                Interval = GetRefreshIntervalInMilliseconds(),
                AutoReset = true
            };
            _timer.Elapsed += RefreshPosition;

            _positionSaver = new PositionSaver(_csvLocation);
        }



        #endregion

        #region Service Start/Stop Methods

        /// <summary>
        /// Start the position servce logic
        /// </summary>
        /// <returns>True/False indicating if start was successful</returns>
        public bool Start()
        {
            _log.Info("Service started");
            ProcessPosition();
            _timer.Start();
            return true;
        }

        /// <summary>
        /// Stop the position service logic
        /// </summary>
        /// <returns>True/False indicating if stop logic was successful</returns>
        public bool Stop()
        {
            _timer.Stop();
            _log.Info($"Service stopped");
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Process the position from trades
        /// </summary>
        private void ProcessPosition()
        {
            _position = _tradeProcessor.GetPosition(DayAhead);
            if (_position != null)
            {
                _log.Info($"Position processed");
                SavePosition();
            }
        }

        /// <summary>
        /// Save the position to CSV file
        /// </summary>
        private void SavePosition()
        {
            DateTime extractTime = DateTime.Now;
            string fileName = $"PowerPosition_{extractTime:yyyyMMdd}_{extractTime:HHmm}.csv";
            _positionSaver.SaveToCsv(fileName, _position);
        }

        /// <summary>
        /// Convert interval in minutes to milliseconds
        /// </summary>
        /// <returns>integer number of miliseconds</returns>
        private int GetRefreshIntervalInMilliseconds()
        {
            int result;
            if (int.TryParse(_refreshInterval, out result) && result > 0)
            {
                return result * 1000 * 60;
            }
            else
            {
                // Use a default interval of 1 minute
                return 1000 * 60;
            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Restart the service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Restart(object sender, EventArgs e)
        {
            _log.Info($"Service started");
            Stop();
            Start();
        }

        /// <summary>
        /// Refresh the position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshPosition(object sender, ElapsedEventArgs e)
        {
            ProcessPosition();
        }

        #endregion
    }
}
