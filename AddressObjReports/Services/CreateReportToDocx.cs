using System.Drawing;
using AddressObjReports.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace AddressObjReports.Services
{
    /// <summary>
    /// Класс, для создания отчета в формате Docx.
    /// </summary>
    public class CreateReportToDocx
    {
        /// <summary>
        /// Создает отчет в формате txt.
        /// </summary>
        /// <param name="groupedAddresses">Словарь с ключом LevelName со значением списка адресов.</param>
        /// <returns></returns>
        public Task CreateReport(Dictionary<string, List<Address>> groupedAddresses)
        {
            Console.Clear();
            var report = DocX.Create("../../../report.docx");
            FillingReport(report, groupedAddresses);
            report.Save();
            return Task.CompletedTask;
        }

        private void FillingReport(DocX report, Dictionary<string, List<Address>> groupedAddresses)
        {
            report.InsertParagraph("Отчет по добавленным адресным объектам за " + GetReportDate())
                .FontSize(20)
                .Color(Color.DarkBlue)
                .Alignment = Alignment.left;

            foreach (var address in groupedAddresses)
            {
                report.InsertParagraph(address.Key)
                    .FontSize(16)
                    .Color(Color.DarkBlue);

                report.InsertParagraph()
                    .InsertTableAfterSelf(CreateTable(report, address.Value));
            }
        }

        private Table CreateTable(DocX report, List<Address> addresses)
        {
            var table = report.AddTable(addresses.Count + 1, 2);
            table.Design = TableDesign.TableGrid;

            table.Rows[0].Cells[0].Paragraphs[0]
                .Append("Тип объекта")
                .FontSize(14)
                .Bold();

            table.Rows[0].Cells[1].Paragraphs[0]
                .Append("Наименование")
                .FontSize(14)
                .Bold();

            for (var i = 0; i < addresses.Count; i++)
            {
                table.Rows[i + 1].Cells[0].Paragraphs[0]
                    .Append(addresses[i].TypeName)
                    .FontSize(14);

                table.Rows[i + 1].Cells[1].Paragraphs[0]
                    .Append(addresses[i].Name)
                    .FontSize(14);
            }

            return table;
        }

        private string GetReportDate()
        {
            using var sr = new StreamReader("gar_delta_xml\\version.txt");
            return sr.ReadLine() ?? throw new InvalidOperationException();
        }
    }
}
