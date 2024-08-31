using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders.Interface
{
    public interface IFileLoader
    {
        List<TradeData> Load(string filePath);
    }

}
