using System;


namespace TradeFileMonitor.Helpers
{
    public static class FileExtensionHelper
    {
        public static FileExtension ParseFileExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case ".csv":
                    return FileExtension.Csv;
                case ".txt":
                    return FileExtension.Txt;
                case ".xml":
                    return FileExtension.Xml;
                default:
                    return FileExtension.Unknown;
            }
        }

       
    }

}
