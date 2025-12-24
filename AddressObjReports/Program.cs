using AddressObjReports.Services;

namespace AddressObjReports
{
    public class Program
    {
        //возвращает информацию о последней версии файлов, доступных для скачивания(тип DownloadFileInfo).
        static readonly string _url = "https://fias.nalog.ru/WebServices/Public/GetLastDownloadFileInfo";
        static readonly string _zipName = "gar_delta_xml.zip";
        static readonly string _directoryName = "gar_delta_xml";

        static async Task Main(string[] args)
        {
            try
            {
                var downloadFileService = new DownloadFileService();
                await downloadFileService.DownloadFileToBaseDirectory(_url, _zipName, _directoryName);
                
                var addressesService = new XmlAddressesParserService();
                var groupAddresses = addressesService.GetGroupAddresses(_directoryName);

                var reportCreator = new CreateReportToDocx();
                await reportCreator.CreateReport(groupAddresses);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Не удалось установить SSL соединение");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Непредвиденная ошибка");
            }
            finally
            {
                File.Delete(_zipName);
                Directory.Delete(_directoryName, true);
            }
        }
    }
}
