using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms; // Windows Forms için gerekli
using System.Xml.Linq;
using TradeFileMonitor.Loaders;
using TradeFileMonitor.Services;

namespace TradeFileMonitor
{
    public partial class MainWindow : Window
    {
        private FileMonitorService _fileMonitorService;

        public MainWindow()
        {
            InitializeComponent();
            var loaderFactory = new LoaderFactory();
            string csvDirectoryPath = @"C:\Users\akshin.hummatov\Desktop\CSVFiles";
            _fileMonitorService = new FileMonitorService(csvDirectoryPath, loaderFactory);
        }

        private void OnChangeDirectoryClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                try
                {
                    _fileMonitorService = new FileMonitorService(selectedPath, new LoaderFactory());
                    _fileMonitorService.StartMonitoring();
                }
                catch (ArgumentException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnChangeIntervalClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(intervalTextBox.Text, out int interval))
            {
                _fileMonitorService.ChangeInterval(interval);
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid interval value."); // WPF MessageBox
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    DataTable dataTable = LoadData(filePath);
                    dataGrid.ItemsSource = dataTable.DefaultView; // Updated reference to dataGrid
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //private DataTable LoadData(string filePath)
        //{
        //    var dataTable = new DataTable();
        //    var fileExtension = Path.GetExtension(filePath).ToLower();

        //    if (fileExtension == ".csv")
        //    {
        //        using (var reader = new StreamReader(filePath))
        //        {
        //            var headers = reader.ReadLine().Split(',');
        //            foreach (var header in headers)
        //            {
        //                dataTable.Columns.Add(header);
        //            }

        //            while (!reader.EndOfStream)
        //            {
        //                var row = reader.ReadLine().Split(',');
        //                dataTable.Rows.Add(row);
        //            }
        //        }
        //    }
        //    else if (fileExtension == ".txt")
        //    {
        //        using (var reader = new StreamReader(filePath))
        //        {
        //            var headers = reader.ReadLine().Split(';'); // TXT dosyaları için ayırıcı noktalı virgül olabilir
        //            foreach (var header in headers)
        //            {
        //                dataTable.Columns.Add(header);
        //            }

        //            while (!reader.EndOfStream)
        //            {
        //                var row = reader.ReadLine().Split(';'); // Satırları ayırmak için noktalı virgül
        //                dataTable.Rows.Add(row);
        //            }
        //        }
        //    }
        //    else if (fileExtension == ".xml")
        //    {
        //        var xdoc = XDocument.Load(filePath);
        //        var dataRecords = xdoc.Descendants("value").Select(node =>
        //        {
        //            var row = new object[6];
        //            row[0] = node.Attribute("date")?.Value;
        //            row[1] = node.Attribute("open")?.Value;
        //            row[2] = node.Attribute("high")?.Value;
        //            row[3] = node.Attribute("low")?.Value;
        //            row[4] = node.Attribute("close")?.Value;
        //            row[5] = node.Attribute("volume")?.Value;
        //            return row;
        //        });

        //        foreach (var row in dataRecords)
        //        {
        //            dataTable.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        throw new NotSupportedException("File type not supported.");
        //    }

        //    return dataTable;
        //}

        private DataTable LoadData(string filePath)
        {
            var dataTable = new DataTable();
            var fileExtension = Path.GetExtension(filePath).ToLower();

            if (fileExtension == ".csv")
            {
                using (var reader = new StreamReader(filePath))
                {
                    var headers = reader.ReadLine().Split(',');
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine().Split(',');
                        dataTable.Rows.Add(row);
                    }
                }
            }
            else if (fileExtension == ".txt")
            {
                using (var reader = new StreamReader(filePath))
                {
                    var headers = reader.ReadLine().Split(';'); // TXT dosyaları için ayırıcı noktalı virgül olabilir
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine().Split(';'); // Satırları ayırmak için noktalı virgül
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
                throw new NotSupportedException("File type not supported.");
            }

            return dataTable;
        }
    }
}
