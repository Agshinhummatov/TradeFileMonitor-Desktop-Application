using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CsvFileLoader : IFileLoader
    {
        public List<TradeData> Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new List<TradeData>();
            }

            var lines = File.ReadAllLines(filePath);
            var data = new List<TradeData>();

            foreach (var line in lines.Skip(1)) // İlk satır başlık olduğu için atlanır
            {
                var parts = line.Split(',');
                if (parts.Length == 6)
                {
                    data.Add(new TradeData
                    {
                        Date = DateTime.Parse(parts[0]),
                        Open = decimal.Parse(parts[1]),
                        High = decimal.Parse(parts[2]),
                        Low = decimal.Parse(parts[3]),
                        Close = decimal.Parse(parts[4]),
                        Volume = int.Parse(parts[5])
                    });
                }
            }

            return data;
        }
    }


}
