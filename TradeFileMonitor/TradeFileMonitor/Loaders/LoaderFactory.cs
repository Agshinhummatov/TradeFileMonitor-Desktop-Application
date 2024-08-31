using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using TradeFileMonitor.Loaders.Interface;

namespace TradeFileMonitor.Loaders
{
    public class LoaderFactory
    {
        public ILoader GetLoader(string extension)
        {
            switch (extension.ToLower())
            {
                case ".csv":
                    return new CsvLoader();
                case ".txt":
                    return new TxtLoader();
                case ".xml":
                    return new XmlLoader();
                default:
                    throw new NotSupportedException($"The file extension {extension} is not supported.");
            }
        }
    }
}
