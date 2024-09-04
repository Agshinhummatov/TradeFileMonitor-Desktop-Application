using System;
using TradeFileMonitor.Helpers;
using TradeFileMonitor.Loaders.Interface;
using TradeFileMonitor.Models;

namespace TradeFileMonitor.Loaders
{
    public class LoaderFactory : ILoaderFactory
    {
        public ILoader GetLoader(string fileExtension)
        {
            var extension = FileExtensionHelper.ParseFileExtension(fileExtension);
            return GetLoader(extension);
        }
        public ILoader GetLoader(FileExtension fileExtension)
        {
            switch (fileExtension)
            {
                case FileExtension.Csv:
                    return new CsvLoader();
                case FileExtension.Txt:
                    return new TxtLoader();
                case FileExtension.Xml:
                    return new XmlLoader();
                default:
                      throw new NotSupportedException($"File extension '{fileExtension}' is not supported.");
            }
        }

    }
}
