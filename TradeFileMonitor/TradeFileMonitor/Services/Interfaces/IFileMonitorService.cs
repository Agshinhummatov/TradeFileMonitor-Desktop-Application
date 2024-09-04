using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Services.Interfaces
{
    public interface IFileMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
        void ChangeInterval(double interval);
        Task UpdateFileListView(IEnumerable<DataRecord> records);
    }


}
