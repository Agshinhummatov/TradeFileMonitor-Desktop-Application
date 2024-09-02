using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ListView _listView;

        public FileMonitorService(string directoryPath, LoaderFactory loaderFactory, ListView listView)
        {
            _directoryPath = directoryPath;
            _loaderFactory = loaderFactory;
            _listView = listView; 
            _fileWatcher = new FileSystemWatcher(_directoryPath);
            _fileWatcher.Created += OnNewFileDetected;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); 
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

            var data = loader.Load(e.FullPath)?.ToList(); 

            if (data != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateListView(data, _listView); 
                });
            }
            else
            {
                MessageBox.Show("File could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {     var files = Directory.GetFiles(_directoryPath);
          
            MessageBox.Show("Timer ticked!");
        }

        public void StartMonitoring()
        {
            _fileWatcher.EnableRaisingEvents = true;
            _timer.Start(); 
            UpdateFileListView(GetFilesInDirectory(_directoryPath));
        }

        private List<DataRecord> GetFilesInDirectory(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            var data = new List<DataRecord>();

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                var loader = _loaderFactory.GetLoader(extension);
                if (loader != null)
                {
                    var loadedData = loader.Load(file);
                    if (loadedData != null)
                    {
                        data.AddRange(loadedData);
                    }
                }
            }

            return data;
        }

        public void StopMonitoring()
        {
            _fileWatcher.EnableRaisingEvents = false;
            _timer.Stop(); 
        }

        public void UpdateListView(List<DataRecord> data, ListView listView)
        {
            listView.ItemsSource = data;
        }

        public void UpdateFileListView(List<DataRecord> data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _listView.ItemsSource = data;
            });
        }
    }


}
