using System;
using System.IO;
using CsvHelper;
using Topshelf.Logging;

namespace PowerScheduler
{
    public class PositionSaver
    {
        private static readonly LogWriter _log = HostLogger.Get<PositionSaver>();
        public string OutputDirectory { get; private set; }

        #region Contructor

        public PositionSaver(string outputDirectory)
        {
            OutputDirectory = outputDirectory;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Save position to CSV file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="position"></param>
        public void SaveToCsv(string fileName, DailyPosition position)
        {
            string filePath = Path.Combine(OutputDirectory, fileName);
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer))
            {
                try
                {
                    csv.WriteRecords(position.PositionsByHour);
                    _log.Info($"Saved position file to {Path.GetFullPath(filePath)}");
                }
                catch (Exception e)
                {
                    _log.Error($"Position file {fileName} could not be saved! {e.Message}");
                    throw;
                }

            }
        }

        #endregion
    }
}
