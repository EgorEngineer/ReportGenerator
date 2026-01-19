using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ReportGenerator.Services
{
    internal class DocxReportGenerator
    {
        /// Генерирует DOCX-отчет
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

            using (WordprocessingDocument doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                Body body = new Body();

                AppendHeader(body, reportDate, objects.Count);

                foreach (var group in groupedByLevel)
                {
                    string levelName = levels.ContainsKey(group.Key) ? levels[group.Key] : $"Уровень {group.Key}";
                    var sortedObjects = group.OrderBy(x => x.Name).ToList();

                    AppendLevelSection(body, levelName, sortedObjects);
                }

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }

        private void AppendHeader(Body body, DateTime reportDate, int totalCount)
        {
            body.Append(CreateHeading($"Отчет по новым адресным объектам ФИАС {reportDate:dd.MM.yyyy}", 1));
            body.Append(CreateParagraph($"Дата формирования отчета: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"));
            body.Append(CreateParagraph($"Всего объектов: {totalCount}"));
            body.Append(CreateParagraph(""));
        }

        private void AppendLevelSection(Body body, string levelName, List<AddressObject> sortedObjects)
        {
            body.Append(CreateHeading(levelName, 2));
            body.Append(CreateParagraph($"Количество объектов: {sortedObjects.Count}"));

            Table table = CreateTable();

            TableRow headerRow = new TableRow();
            headerRow.Append(CreateTableCell("№", true));
            headerRow.Append(CreateTableCell("Тип", true));
            headerRow.Append(CreateTableCell("Наименование", true));
            table.Append(headerRow);

            for (int i = 0; i < sortedObjects.Count; i++)
            {
                var obj = sortedObjects[i];
                TableRow dataRow = new TableRow();
                dataRow.Append(CreateTableCell((i + 1).ToString()));
                dataRow.Append(CreateTableCell(obj.TypeName ?? "-"));
                dataRow.Append(CreateTableCell(obj.Name ?? "-"));
                table.Append(dataRow);
            }

            body.Append(table);
            body.Append(CreateParagraph(""));
        }

        private Paragraph CreateHeading(string text, int level)
        {
            Paragraph para = new Paragraph();
            Run run = new Run(new Text(text));

            RunProperties runProps = new RunProperties();
            runProps.Append(new Bold());
            runProps.Append(new FontSize { Val = level == 1 ? "32" : "28" });
            run.RunProperties = runProps;

            para.Append(run);

            ParagraphProperties paraProps = new ParagraphProperties();
            paraProps.Append(new SpacingBetweenLines { After = "200" });
            para.ParagraphProperties = paraProps;

            return para;
        }

        private Paragraph CreateParagraph(string text)
        {
            Paragraph para = new Paragraph();
            Run run = new Run(new Text(text));
            para.Append(run);

            ParagraphProperties paraProps = new ParagraphProperties();
            paraProps.Append(new SpacingBetweenLines { After = "100" });
            para.ParagraphProperties = paraProps;

            return para;
        }

        private Table CreateTable()
        {
            Table table = new Table();

            TableProperties tableProps = new TableProperties();
            tableProps.Append(new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            ));
            tableProps.Append(new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct });
            table.Append(tableProps);

            return table;
        }

        private TableCell CreateTableCell(string text, bool isHeader = false)
        {
            TableCell cell = new TableCell();

            Paragraph para = new Paragraph();
            Run run = new Run(new Text(text));

            if (isHeader)
            {
                RunProperties runProps = new RunProperties();
                runProps.Append(new Bold());
                run.RunProperties = runProps;

                TableCellProperties cellProps = new TableCellProperties();
                cellProps.Append(new Shading { Fill = "4CAF50" });
                cell.Append(cellProps);
            }

            para.Append(run);
            cell.Append(para);

            return cell;
        }
    }
}
