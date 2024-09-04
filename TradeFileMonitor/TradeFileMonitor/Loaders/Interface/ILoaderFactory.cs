using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeFileMonitor.Helpers;

namespace TradeFileMonitor.Loaders.Interface
{
    public interface ILoaderFactory
    {
       ILoader GetLoader(FileExtension fileExtension);
    }
}
