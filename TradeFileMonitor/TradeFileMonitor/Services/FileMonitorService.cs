using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TradeFileMonitor.Loaders;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Services
{
    public class FileMonitorService
    {
        private readonly string _directoryPath;
        private readonly LoaderFactory _loaderFactory;
        private FileSystemWatcher _fileWatcher;
        private readonly DispatcherTimer _timer;

        public FileMonitorService(string directoryPath, LoaderFactory loaderFactory)
        {
            _directoryPath = directoryPath;
            _loaderFactory = loaderFactory;
            _fileWatcher = new FileSystemWatcher(_directoryPath);
            _fileWatcher.Created += OnNewFileDetected;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); //iInterval
        }

        public void ChangeInterval(int seconds)
        {
            _timer.Interval = TimeSpan.FromSeconds(seconds);
        }

        private void OnNewFileDetected(object sender, FileSystemEventArgs e)
        {
            var extension = Path.GetExtension(e.FullPath);
            var loader = _loaderFactory.GetLoader(extension);

            if (loader == null)
            {
                MessageBox.Show($"Unsupported file extension: {extension}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var data = loader.Load(e.FullPath);

            if (data != null)
            {
                // GUI'yi güncellemek için burada bir çağrı yapabilirsiniz
            }
            else
            {
                MessageBox.Show("File could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StartMonitoring() => _fileWatcher.EnableRaisingEvents = true;

        public void StopMonitoring() => _fileWatcher.EnableRaisingEvents = false;

        public void UpdateListView(List<DataRecord> data, ListView listView)
        {
            listView.ItemsSource = data;
        }
    }

}
