using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using CsvHelper.Configuration.Attributes;

namespace PowerScheduler
{
    public class DailyPosition
    {
        #region Properties

        public DateTime Date { get; private set; }
        public List<HourlyPosition> PositionsByHour { get; private set; }

        #endregion

        #region Constructor

        public DailyPosition(DateTime asOfDate)
        {
            Date = asOfDate;
            PositionsByHour = new List<HourlyPosition>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Aggregate trades into hourly positions
        /// </summary>
        /// <param name="trades"></param>
        public void ComputePosition(List<PowerTrade> trades)
        {
            PositionsByHour.Clear();
            PositionsByHour = Enumerable.Range(-1, 24)
                              .Select(hourlyPeriod => new HourlyPosition()
                              {
                                  LocalTime = $"{Date.AddHours(hourlyPeriod):HH:mm}",
                                  Volume = 0.0
                              })
                              .ToList();
            if (trades.Any())
            {
                foreach (var trade in trades)
                {
                    foreach (var period in trade.Periods)
                    {
                        PositionsByHour[period.Period - 1].Volume += period.Volume;
                    }
                }
            }
        }

        #endregion

    }

    public class HourlyPosition
    {
        [Name("Local Time")]
        public string LocalTime { get; set; }
        [Name("Volume")]
        public double Volume { get; set; }
    }
}
