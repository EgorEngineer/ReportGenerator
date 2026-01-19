using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator.Services
{
    internal class HtmlReportGenerator
    {
        /// Генерирует HTML-отчет
        /// <param name="objects">Список адресных объектов</param>
        /// <param name="levels">Словарь уровней</param>
        /// <param name="outputPath">Путь для сохранения отчета</param>
        public void Generate(List<AddressObject> objects, Dictionary<int, string> levels, string outputPath)
        {
            var groupedByLevel = objects
                .GroupBy(x => x.Level)
                .OrderBy(g => g.Key)
                .ToList();

            DateTime reportDate = objects.Any() ? objects.Max(x => x.UpdateDate) : DateTime.Now;

            var html = new StringBuilder();

            AppendHeader(html, reportDate, objects.Count);

            foreach (var group in groupedByLevel)
            {
                string levelName = levels.ContainsKey(group.Key) ? levels[group.Key] : $"Уровень {group.Key}";
                var sortedObjects = group.OrderBy(x => x.Name).ToList();

                AppendLevelSection(html, levelName, sortedObjects);
            }

            AppendFooter(html);

            File.WriteAllText(outputPath, html.ToString());
        }

        private void AppendHeader(StringBuilder html, DateTime reportDate, int totalCount)
        {
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='ru'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("    <title>Отчет по новым адресным объектам ФИАС</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }");
            html.AppendLine("        .container { max-width: 1200px; margin: 0 auto; background: white; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        h1 { color: #333; border-bottom: 3px solid #4CAF50; padding-bottom: 10px; }");
            html.AppendLine("        .header { margin-bottom: 30px; }");
            html.AppendLine("        .date { font-size: 16px; color: #666; margin: 10px 0; }");
            html.AppendLine("        .section { margin: 30px 0; }");
            html.AppendLine("        h2 { color: #4CAF50; margin-top: 30px; }");
            html.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
            html.AppendLine("        th { background: #4CAF50; color: white; padding: 12px; text-align: left; font-weight: bold; }");
            html.AppendLine("        td { padding: 10px; border-bottom: 1px solid #ddd; }");
            html.AppendLine("        tr:hover { background: #f9f9f9; }");
            html.AppendLine("        .count { color: #666; font-size: 14px; margin: 5px 0; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class='container'>");
            html.AppendLine("        <div class='header'>");
            html.AppendLine($"            <h1>Отчет по новым адресным объектам ФИАС за {reportDate:dd.MM.yyyy}</h1>");
            html.AppendLine($"            <div class='date'>Дата формирования отчета: <strong>{DateTime.Now:dd.MM.yyyy HH:mm:ss}</strong></div>");
            html.AppendLine($"            <div class='count'>Всего объектов: <strong>{totalCount}</strong></div>");
            html.AppendLine("        </div>");
        }

        private void AppendLevelSection(StringBuilder html, string levelName, List<AddressObject> sortedObjects)
        {
            html.AppendLine("        <div class='section'>");
            html.AppendLine($"            <h2>{levelName}</h2>");
            html.AppendLine($"            <div class='count'>Количество объектов: {sortedObjects.Count}</div>");
            html.AppendLine("            <table>");
            html.AppendLine("                <thead>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <th>№</th>");
            html.AppendLine("                        <th>Тип</th>");
            html.AppendLine("                        <th>Наименование</th>");
            html.AppendLine("                    </tr>");
            html.AppendLine("                </thead>");
            html.AppendLine("                <tbody>");

            for (int i = 0; i < sortedObjects.Count; i++)
            {
                var obj = sortedObjects[i];
                html.AppendLine("                    <tr>");
                html.AppendLine($"                        <td>{i + 1}</td>");
                html.AppendLine($"                        <td>{obj.TypeName ?? "-"}</td>");
                html.AppendLine($"                        <td>{obj.Name ?? "-"}</td>");
                html.AppendLine("                    </tr>");
            }

            html.AppendLine("                </tbody>");
            html.AppendLine("            </table>");
            html.AppendLine("        </div>");
        }

        private void AppendFooter(StringBuilder html)
        {
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
        }
    }
}