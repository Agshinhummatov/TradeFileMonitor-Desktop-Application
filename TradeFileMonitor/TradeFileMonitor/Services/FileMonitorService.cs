using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using TradeFileMonitor.Constants;
using TradeFileMonitor.Helpers;
using TradeFileMonitor.Loaders;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;
using TradeFileMonitor.Services.Interfaces;

public class FileMonitorService : IFileMonitorService
{
    private readonly string _directoryPath;
    private readonly ILoaderFactory _loaderFactory;
    private readonly ListView _fileListView;
    private readonly Timer _timer;
    private readonly ConcurrentDictionary<string, DateTime> _fileTimestamps = new ConcurrentDictionary<string, DateTime>();
    private const double DefaultInterval = 5000;

    public FileMonitorService(string directoryPath, ILoaderFactory loaderFactory, ListView fileListView, double interval = DefaultInterval)
    {
        _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
        _loaderFactory = loaderFactory ?? throw new ArgumentNullException(nameof(loaderFactory));
        _fileListView = fileListView ?? throw new ArgumentNullException(nameof(fileListView));
        _timer = new Timer(interval);
        _timer.Elapsed += async (sender, e) => await OnTimedEvent(sender, e);


    }

    public void StartMonitoring() => _timer.Start();

    public void StopMonitoring() => _timer.Stop();

    public void ChangeInterval(double interval)
    {
        if (interval <= 0)
        {
            throw new ArgumentException(ErrorMessages.IntervalMustBePositive, nameof(interval));
        }

        _timer.Interval = interval;
    }

    private async Task OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        await FileProcessor.LoadNewFilesAsync(_directoryPath, _loaderFactory, _fileListView, _fileTimestamps);
    }


    public async Task UpdateFileListView(IEnumerable<DataRecord> dataRecords)
    {
        await FileProcessor.UpdateFileListViewAsync(dataRecords, _fileListView);
    }
}
