using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TradeFileMonitor.Constants;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders
{
    public class TxtLoader : ILoader
    {
        public async Task<IEnumerable<DataRecord>> LoadAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var lines = File.ReadAllLines(filePath);
                    return lines.Skip(1).Select(line =>
                    {
                        var columns = line.Split(';'); // Semicolon as delimiter for TXT files
                        return new DataRecord
                        {
                            Date = columns[0],
                            Open = decimal.Parse(columns[1]),
                            High = decimal.Parse(columns[2]),
                            Low = decimal.Parse(columns[3]),
                            Close = decimal.Parse(columns[4]),
                            Volume = int.Parse(columns[5])
                        };
                    }).ToList();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(ErrorMessages.TxtLoadFailed, filePath), ex);
                }
            });
        }
    }
}