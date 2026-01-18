using ReportGenerator.Services;

namespace ReportGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Генератор отчета по новым адресным объектам");
                Console.WriteLine("=================================================\n");

                var apiService = new FiasApiService();
                var xmlParser = new XmlParserService();
                var fileManager = new FileManager();
                var htmlGenerator = new HtmlReportGenerator();
                var docxGenerator = new DocxReportGenerator();

                string solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                                               .Parent!
                                               .Parent!
                                               .Parent!
                                               .FullName;

                
                string archivesDir = Path.Combine(solutionDir, "FiasArchives");
                string docsDir = Path.Combine(solutionDir, "FiasDocs");
                string reportDir = Path.Combine(solutionDir, "FiasReports");
                fileManager.EnsureDirectoryExists(archivesDir);
                fileManager.EnsureDirectoryExists(docsDir);
                fileManager.EnsureDirectoryExists(reportDir);

                string zipPath = Path.Combine(archivesDir, "delta.zip");

                string dateStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

                string htmlReportPath = Path.Combine(reportDir, $"report_{dateStamp}.html");
                string docxReportPath = Path.Combine(reportDir, $"report_{dateStamp}.docx");

                Console.WriteLine("1. Получение информации о последнем пакете изменений...");
                string deltaUrl = await apiService.GetLastDeltaUrlAsync();
                Console.WriteLine($"Найден пакет: {deltaUrl}\n");

                Console.WriteLine("2.Скачивание архива...");
                await apiService.DownloadFileAsync(deltaUrl, zipPath);
                Console.WriteLine($"Архив сохранен: {zipPath}\n");

                Console.WriteLine("3.Разархивирование...");
                fileManager.ExtractZip(zipPath, docsDir);
                Console.WriteLine($"Файлы извлечены в: {docsDir}\n");

                Console.WriteLine("4. Обработка адресных объектов...");
                var addrObjects = xmlParser.ParseAddressObjects(docsDir);
                var objectLevels = xmlParser.ParseObjectLevels(docsDir);
                Console.WriteLine($"Найдено адресных объектов: {addrObjects.Count}\n");

                Console.WriteLine("5. Генерация отчетов...");
                htmlGenerator.Generate(addrObjects, objectLevels, htmlReportPath);
                Console.WriteLine($"HTML-отчет: {htmlReportPath}");
                docxGenerator.Generate(addrObjects, objectLevels, docxReportPath);
                Console.WriteLine($"DOCX-отчет: {docxReportPath}\n");

                Console.WriteLine("Готово! Отчеты успешно созданы.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
