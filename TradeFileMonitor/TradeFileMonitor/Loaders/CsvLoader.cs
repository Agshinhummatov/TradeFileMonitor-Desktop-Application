using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;
using TradeFileMonitor.Helpers;
using TradeFileMonitor.Constants;

namespace TradeFileMonitor.Loaders
{
    public class CsvLoader : ILoader
    {
        public async Task<IEnumerable<DataRecord>> LoadAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var lines = File.ReadAllLines(filePath);

                    if (lines.Length == 0)
                    {
                        throw new InvalidOperationException(ErrorMessages.CsvFileEmpty);
                    }

                    var header = lines[0].Split(',');
                    var expectedHeaderLength = 6;

                    if (header.Length != expectedHeaderLength)
                    {
                        throw new InvalidOperationException(string.Format(ErrorMessages.CsvHeaderMismatch, expectedHeaderLength, header.Length));
                    }

                    return lines.Skip(1).Select(line =>
                    {
                        try
                        {
                            var columns = line.Split(',');

                            if (columns.Length != expectedHeaderLength)
                            {
                                throw new InvalidOperationException(ErrorMessages.CsvRowMismatch);
                            }

                            return new DataRecord
                            {
                                Date = columns[0],
                                Open = decimal.Parse(columns[1]),
                                High = decimal.Parse(columns[2]),
                                Low = decimal.Parse(columns[3]),
                                Close = decimal.Parse(columns[4]),
                                Volume = int.Parse(columns[5])
                            };
                        }
                        catch (FormatException ex)
                        {
                            throw new InvalidOperationException(string.Format(ErrorMessages.CsvParsingError, line), ex);
                        }
                    }).ToList();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(ErrorMessages.FileLoadFailed, filePath), ex);
                }
            });
        }
    }
}
