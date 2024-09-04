using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using TradeFileMonitor.Constants;
using TradeFileMonitor.Helpers;
using TradeFileMonitor.Loaders;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;
using TradeFileMonitor.Services;
using TradeFileMonitor.Services.Interfaces;

namespace TradeFileMonitor
{
    public partial class MainWindow : Window
    {
        private IFileMonitorService _fileMonitorService;
        private ILoaderFactory _loaderFactory;

        public MainWindow()
        {
            InitializeComponent();
            _loaderFactory = new LoaderFactory();
            InitializeFileMonitorService();
        }

        private void InitializeFileMonitorService()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    try
                    {
                        _fileMonitorService = new FileMonitorService(selectedPath, _loaderFactory, fileListView);
                        _fileMonitorService.StartMonitoring();
                    }
                    catch (ArgumentException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage(string.Format(FileLoaderErrors.UnexpectedErrorMessage, ex.Message));
                    }
                }
                else
                {
                    ShowErrorMessage(FileLoaderErrors.NoDirectorySelectedMessage);
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }

        private void OnChangeDirectoryClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    try
                    {
                        _fileMonitorService.StopMonitoring();
                        _fileMonitorService = new FileMonitorService(selectedPath, _loaderFactory, fileListView);
                        _fileMonitorService.StartMonitoring();
                    }
                    catch (ArgumentException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage(string.Format(FileLoaderErrors.UnexpectedErrorMessage, ex.Message));
                    }
                }
            }
        }

        private void OnChangeIntervalClick(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(intervalTextBox.Text, out var newInterval) && newInterval > 0)
            {
                _fileMonitorService.ChangeInterval(newInterval);
            }
            else
            {
                ShowErrorMessage(FileLoaderErrors.InvalidIntervalMessage);
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private async void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    var dataRecords = await LoadDataFromFileAsync(filePath);
                    // Güncellenmiş dosya içeriğini GUI'ye ekleyin.
                     _fileMonitorService.UpdateFileListView(dataRecords);

                    // DataGrid güncellemesi.
                    DataTable dataTable = LoadData(filePath);
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(string.Format(FileLoaderErrors.ErrorLoadingFileMessage, ex.Message));
                }
            }
        }


        private async Task<List<DataRecord>> LoadDataFromFileAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            var fileExtension = FileExtensionHelper.ParseFileExtension(extension);
            var loader = _loaderFactory.GetLoader(fileExtension);

            if (loader == null)
                return new List<DataRecord>();

            try
            {
                var dataRecords = await loader.LoadAsync(filePath);
                return dataRecords.ToList();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(FileLoaderErrors.ErrorLoadingFileMessage, ex.Message));
                throw;
            }
        }

        private DataTable LoadData(string filePath)
        {
            var dataTable = new DataTable();
            var fileExtension = Path.GetExtension(filePath).ToLower();

            if (fileExtension == ".csv" || fileExtension == ".txt")
            {
                char delimiter = fileExtension == ".csv" ? ',' : ';';

                using (var reader = new StreamReader(filePath))
                {
                    var headers = reader.ReadLine().Split(delimiter);
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine().Split(delimiter);
                        dataTable.Rows.Add(row);
                    }
                }
            }
            else if (fileExtension == ".xml")
            {
                var xdoc = XDocument.Load(filePath);
                dataTable.Columns.Add("Date");
                dataTable.Columns.Add("Open");
                dataTable.Columns.Add("High");
                dataTable.Columns.Add("Low");
                dataTable.Columns.Add("Close");
                dataTable.Columns.Add("Volume");

                var dataRecords = xdoc.Descendants("value").Select(node =>
                {
                    var row = dataTable.NewRow();
                    row["Date"] = node.Attribute("date")?.Value;
                    row["Open"] = node.Attribute("open")?.Value;
                    row["High"] = node.Attribute("high")?.Value;
                    row["Low"] = node.Attribute("low")?.Value;
                    row["Close"] = node.Attribute("close")?.Value;
                    row["Volume"] = node.Attribute("volume")?.Value;
                    return row;
                });

                foreach (var row in dataRecords)
                {
                    dataTable.Rows.Add(row);
                }
            }
            else
            {
                throw new NotSupportedException(FileLoaderErrors.UnsupportedFileTypeMessage);
            }

            return dataTable;
        }

        private void ShowErrorMessage(string message)
        {
            System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
