using System.IO.Compression;
using System.Net.Http.Json;
using AddressObjReports.Models;

namespace AddressObjReports.Services
{
    /// <summary>
    /// Класс, который представляет сервис по скачиванию файлов.
    /// </summary>
    public class DownloadFileService
    {
        private static readonly HttpClient _httpClient = new();
        
        /// <summary>
        /// Скачивает zip файл по ссылке.
        /// </summary>
        /// <param name="url">Ссылка на JSON с тегом GarXMLDeltaURL, где хранится zip.</param>
        /// <param name="zipName">Название zip файла.</param>
        /// <param name="directoryName">Название каталога, куда распакуется zip.</param>
        /// <returns></returns>
        public async Task DownloadFileToBaseDirectory(string url, string zipName, string directoryName)
        {
            Console.WriteLine("Загрузка zip файла");
        
            var downloadUrl = await GetLastDownloadFileInfo(url);
            await DownloadFileAsync(downloadUrl, zipName);
        
            Console.Clear();
            Console.WriteLine($"Распаковка {zipName} в {directoryName}");
        
            ZipFile.ExtractToDirectory(zipName, directoryName, overwriteFiles: true);
        }

        /// <summary>
        /// Метод получает JSON по URL и извлекает из него ссылку для скачивания архива.
        /// </summary>
        /// <param name="url">URL для скачивания</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<string> GetLastDownloadFileInfo(string url)
        {
            var fileInfo = await _httpClient.GetFromJsonAsync<DownloadFileInfo>(url)
                           ?? throw new InvalidOperationException("Получен пустой ответ от сервера");
        
            return fileInfo.GarXMLDeltaURL 
                   ?? throw new InvalidOperationException("URL для скачивания не найден");
        }

        /// <summary>
        /// Скачивает файл по URL и сохраняет его локально.
        /// </summary>
        /// <param name="url">URL для скачивания</param>
        /// <param name="filePath">Локальный путь для сохранения файла</param>
        private async Task DownloadFileAsync(string url, string filePath)
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fileStream);
        }
    }
}
