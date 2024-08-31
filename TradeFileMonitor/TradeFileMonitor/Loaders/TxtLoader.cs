using System.Collections.Generic;
using System.IO;
using System.Linq;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders
{
    public class TxtLoader : ILoader
    {
        public IEnumerable<DataRecord> Load(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return lines.Skip(1).Select(line =>
            {
                var columns = line.Split(';');
                return new DataRecord
                {
                    Date = columns[0],
                    Open = decimal.Parse(columns[1]),
                    High = decimal.Parse(columns[2]),
                    Low = decimal.Parse(columns[3]),
                    Close = decimal.Parse(columns[4]),
                    Volume = int.Parse(columns[5])
                };
            });
        }
    }
}