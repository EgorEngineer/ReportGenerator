using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReportGenerator.Services
{
    internal class FiasApiService
    {
        private readonly HttpClient _httpClient;
        private const string ServiceUrl = "http://fias.nalog.ru/WebServices/Public/GetLastDownloadFileInfo";

        public FiasApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        /// Получает URL последнего пакета изменений из ФИАС
        /// <returns>URL для скачивания пакета изменений</returns>
        public async Task<string> GetLastDeltaUrlAsync()
        {
            var response = await _httpClient.GetAsync(ServiceUrl);
            response.EnsureSuccessStatusCode();

            string jsonContent = await response.Content.ReadAsStringAsync();
            var fileInfo = JsonSerializer.Deserialize<DownloadFileInfo>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (fileInfo == null || string.IsNullOrEmpty(fileInfo.GarXMLDeltaURL))
                throw new Exception("Не удалось получить URL пакета изменений из ответа ФИАС");

            return fileInfo.GarXMLDeltaURL;
        }

        /// Скачивает файл по указанному URL
        /// <param name="url">URL файла</param>
        /// <param name="outputPath">Путь для сохранения файла</param>
        public async Task DownloadFileAsync(string url, string outputPath)
        {
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
            }
        }
    }

    /// Модель информации о файлах для скачивания из ФИАС
    public class DownloadFileInfo
    {
        public int VersionId { get; set; }
        public string TextVersion { get; set; }
        public string FiasCompleteDbfUrl { get; set; }
        public string FiasCompleteXmlUrl { get; set; }
        public string FiasDeltaDbfUrl { get; set; }
        public string FiasDeltaXmlUrl { get; set; }
        public string GarXMLDeltaURL { get; set; }
        public string GarXMLFullURL { get; set; }
        public string Kladr4ArjUrl { get; set; }
        public string Kladr47ZUrl { get; set; }
    }
}