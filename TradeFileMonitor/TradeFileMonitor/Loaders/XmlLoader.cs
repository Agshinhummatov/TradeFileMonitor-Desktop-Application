using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TradeFileMonitor.Constants;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders
{
    public class XmlLoader : ILoader
    {
        public async Task<IEnumerable<DataRecord>> LoadAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var xdoc = XDocument.Load(filePath);
                    return xdoc.Descendants("value").Select(node =>
                    {
                        return new DataRecord
                        {
                            Date = node.Attribute("date")?.Value,
                            Open = decimal.Parse(node.Attribute("open")?.Value),
                            High = decimal.Parse(node.Attribute("high")?.Value),
                            Low = decimal.Parse(node.Attribute("low")?.Value),
                            Close = decimal.Parse(node.Attribute("close")?.Value),
                            Volume = int.Parse(node.Attribute("volume")?.Value)
                        };
                    });
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(ErrorMessages.XmlLoadFailed, filePath), ex);
                }
            });
        }
    }
}

