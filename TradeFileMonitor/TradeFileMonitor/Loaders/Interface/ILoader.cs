using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders.Interface
{
    public interface ILoader
    {
        IEnumerable<DataRecord> Load(string filePath);
    }
}
