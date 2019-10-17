using System;
using System.Collections.Generic;
using Services;
using Topshelf.Logging;

namespace PowerScheduler
{
    public class TradeProcessor
    {
        private static readonly LogWriter _log = HostLogger.Get<TradeProcessor>();
        private IPowerService _powerService = new PowerService();
        private List<PowerTrade> _powerTrades;

        public event EventHandler TradeRetrievalFailed;

        #region Constructor

        public TradeProcessor()
        {
            _powerTrades = new List<PowerTrade>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Compute daily position from list of trades
        /// </summary>
        /// <param name="asOfDate"></param>
        /// <returns>DailyPosition object</returns>
        public DailyPosition GetPosition(DateTime asOfDate)
        {
            bool tradesReceived = RefreshTrades(asOfDate);
            if (tradesReceived)
            {
                DailyPosition dailyPosition = new DailyPosition(asOfDate);
                dailyPosition.ComputePosition(_powerTrades);
                return dailyPosition;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Retrieve trades from the PowerService
        /// </summary>
        /// <param name="asOfDate">Date to retrieve positions for</param>
        private bool RefreshTrades(DateTime asOfDate)
        {
            try
            {
                _powerTrades.Clear();
                _powerTrades.AddRange(_powerService.GetTrades(asOfDate));
                _log.Info($"Trades retrieved from PowerService");
                return true;
            }
            catch (PowerServiceException e)
            {
                _log.Warn($"Error getting trades from PowerService!... {e.Message}");
                OnTradeRetrievalFailed(EventArgs.Empty);
                return false;
            }
        }

        #endregion

        protected virtual void OnTradeRetrievalFailed(EventArgs e)
        {
            EventHandler handler = TradeRetrievalFailed;
            handler?.Invoke(this, e);
        }
    }
}
