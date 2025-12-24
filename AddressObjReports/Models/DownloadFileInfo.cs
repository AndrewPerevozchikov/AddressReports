namespace AddressObjReports.Models
{
    /// <summary>
    /// Класс, который представляет ссылку для скачивания zip.
    /// </summary>
    public class DownloadFileInfo
    {
        /// <summary>
        /// URL дельта версии ГАР в формате XML сжатого в zip.
        /// </summary>
        public required string GarXMLDeltaURL { get; set; }
    }
}
