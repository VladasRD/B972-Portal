using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.Security.Data;
using Box.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartGeoIot.ViewModels;
using System.IO;
using System.Globalization;

// for excel
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace SmartGeoIot.Services
{
    public partial class ExcelUtils
    {
        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsDJRF(DashboardViewModels[] reports, string id, string startDate, string endDate)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                {
                    workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    // adds style
                    AddStyle();

                    // create columns
                    Columns columns = new Columns();
                    columns.Append(CreateColumnData(1, 1, 15));
                    columns.Append(CreateColumnData(2, 2, 10));
                    columns.Append(CreateColumnData(3, 3, 20));
                    columns.Append(CreateColumnData(4, 7, 7));
                    columns.Append(CreateColumnData(8, 11, 25));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrEmpty(startDate) || startDate.Contains("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }

                    // table headers       
                    AddReportDJRFTableHeader();

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        AddCell($"{report.Date.ToShortDateString().ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.Date.ToShortTimeString().ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell(EstadoDetectorNome(report.EstadoDetector).ToString(culture), row, style: SGICellStyles.Border);
                        AddCell(report.Alimentacao?.ToString(culture), row, style: SGICellStyles.Currency);
                        AddCell("V", row, style: SGICellStyles.Border);
                        AddCell(report.Temperature?.ToString(culture), row, style: SGICellStyles.Currency);
                        AddCell("°C", row, style: SGICellStyles.Border);
                        AddCell(report.ContadorCarencias?.ToString(culture), row, style: SGICellStyles.Border);
                        AddCell(report.ContadorBloqueios?.ToString(culture), row, style: SGICellStyles.Border);

                        if (report.Bits != null)
                        {
                            string tipoEnvio = report.Bits.TipoEnvio ? "Por evento" : "Por tempo";
                            string periodoTransmissao = report.Bits.BaseTempoUpLink ? $"{report.PeriodoTransmissao} Minutos" : $"{report.PeriodoTransmissao} Segundos";
                            AddCell($"{tipoEnvio.ToString(culture)}/{periodoTransmissao.ToString(culture)}", row, style: SGICellStyles.Border);

                            string estadoBloqueio = report.Bits.EstadoBloqueio ? "Sim" : "Não";
                            string estadoSaidaRastreador = report.Bits.EstadoSaidaRastreador ? "Sim" : "Não";
                            AddCell($"{estadoBloqueio.ToString(culture)}/{estadoSaidaRastreador.ToString(culture)}", row, style: SGICellStyles.Border);
                        }
                        else
                        {
                            AddCell("", row, style: SGICellStyles.Border);
                            AddCell("", row, style: SGICellStyles.Border);
                        }
                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:K1") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("D2:E2") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("F2:G2") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }

        private string EstadoDetectorNome(int tipo)
        {
            string[] tipos = new string[] { "Aguardando", "Operacional", "Em Carência", "Em Ciclos", "Em Bloqueio", "Em Dormência" };
            return tipos[tipo];
        }

        private void AddReportDJRFTableHeader()
        {
            //2º Linha
            var row = new Row();
            AddCell("Data", row, style: SGICellStyles.TableHeader);
            AddCell("Hora", row, style: SGICellStyles.TableHeader);
            AddCell("Estado do detector", row, style: SGICellStyles.TableHeader);
            AddCell("Alimentação", row, style: SGICellStyles.TableHeader);
            AddCell("", row, style: SGICellStyles.TableHeader);
            AddCell("Temperatura", row, style: SGICellStyles.TableHeader);
            AddCell("", row, style: SGICellStyles.TableHeader);
            AddCell("Contador de carências", row, style: SGICellStyles.TableHeader);
            AddCell("Contador de bloqueios", row, style: SGICellStyles.TableHeader);
            AddCell("Tipo de envio / Tempo", row, style: SGICellStyles.TableHeader);
            AddCell("Bloqueio / Ras Out", row, style: SGICellStyles.TableHeader);

            sheetData.AppendChild(row);
        }
    }
}