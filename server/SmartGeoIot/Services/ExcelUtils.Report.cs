using System;
using System.Linq;
using SmartGeoIot.ViewModels;
using System.IO;

// for excel
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartGeoIot.Models;
using SmartGeoIot.Extensions;

namespace SmartGeoIot.Services
{
    public partial class ExcelUtils
    {
        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReports(DashboardViewModels[] reports, string id, string startDate, string endDate)
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

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }
                        

                    // table headers       
                    AddReportTableHeader();

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        AddCell(report.Date.ToString("dd/MM/yyyy"), row, style: SGICellStyles.Border);
                        AddCell(report.Date.ToString("HH:mm"), row, style: SGICellStyles.Border);
                        AddCell($"{report.Level.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.Light.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.Temperature.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.Moisture.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.OxigenioDissolvido.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell(report.Ph.ToString(culture), row, style: SGICellStyles.Border);
                        AddCell($"{report.Condutividade.ToString(culture)}", row, style: SGICellStyles.Border);

                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:H1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }

        private void AddReportTableHeader()
        {
            //2º Linha
            var row = new Row();
            AddCell("Data", row, style: SGICellStyles.TableHeader);
            AddCell("Hora", row, style: SGICellStyles.TableHeader);
            AddCell("Nível (%)", row, style: SGICellStyles.TableHeader);
            AddCell("Luz (%)", row, style: SGICellStyles.TableHeader);

            AddCell("Temperatura (°C)", row, style: SGICellStyles.TableHeader);
            AddCell("Umidade (%)", row, style: SGICellStyles.TableHeader);
            AddCell("Oxigênio (mg/L)", row, style: SGICellStyles.TableHeader);

            AddCell("Ph", row, style: SGICellStyles.TableHeader);
            AddCell("Condutividade (μS)", row, style: SGICellStyles.TableHeader);

            sheetData.AppendChild(row);
        }


        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsPQA(DashboardViewModels[] reports, string id, string startDate, string endDate)
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
                    columns.Append(CreateColumnData(2, 13, 15));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate) && startDate.Equals("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = startDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = endDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }
                        

                    // table headers
                    AddReportPQATableHeader();

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        // verificar data null ou algum dado errado para não aparecer no relatório
                        if (report.Date == default(DateTime))
                            continue;
                        
                        if (report.Temperature == null)
                            continue;

                        AddCell(report.Date.ToString("dd/MM/yyyy"), row, style: SGICellStyles.Border);
                        AddCell(report.Date.ToString("HH:mm"), row, style: SGICellStyles.Border);
                        AddCell($"{report.Temperature?.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell(report.Ph?.ToString(culture), row, style: SGICellStyles.Border);
                        AddCell($"{report.Fluor?.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.Cloro?.ToString(culture)}", row, style: SGICellStyles.Border);
                        AddCell($"{report.Turbidez?.ToString(culture)}", row, style: SGICellStyles.Border);

                        if (report.ReleBoolean != null)
                        {
                            AddCell(report.ReleBoolean.Rele1 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                            AddCell(report.ReleBoolean.Rele2 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                            AddCell(report.ReleBoolean.Rele3 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                            AddCell(report.ReleBoolean.Rele4 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                            AddCell(report.ReleBoolean.Rele5 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                            AddCell(report.ReleBoolean.Rele6 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                            AddCell(report.ReleBoolean.Rele7 ? "Ligado" : "Desligado", row, style: SGICellStyles.Border);
                        }

                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:M1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }

        private void AddReportPQATableHeader()
        {
            //2º Linha
            var row = new Row();
            AddCell("Data", row, style: SGICellStyles.TableHeader);
            AddCell("Hora", row, style: SGICellStyles.TableHeader);
            AddCell("Temperatura (°C)", row, style: SGICellStyles.TableHeader);
            AddCell("Ph", row, style: SGICellStyles.TableHeader);
            AddCell("Flúor (ppm)", row, style: SGICellStyles.TableHeader);
            AddCell("Cloro (ppm)", row, style: SGICellStyles.TableHeader);
            AddCell("Turbidez (NTU)", row, style: SGICellStyles.TableHeader);

            AddCell("pH 1", row, style: SGICellStyles.TableHeader);
            AddCell("pH 2", row, style: SGICellStyles.TableHeader);
            AddCell("Cloro 1", row, style: SGICellStyles.TableHeader);
            AddCell("Cloro 2", row, style: SGICellStyles.TableHeader);
            AddCell("Flúor", row, style: SGICellStyles.TableHeader);
            AddCell("Alarme 1", row, style: SGICellStyles.TableHeader);
            AddCell("Alarme 2", row, style: SGICellStyles.TableHeader);

            // AddCell("Rele 1", row, style: SGICellStyles.TableHeader);
            // AddCell("Rele 2", row, style: SGICellStyles.TableHeader);
            // AddCell("Rele 3", row, style: SGICellStyles.TableHeader);
            // AddCell("Rele 4", row, style: SGICellStyles.TableHeader);
            // AddCell("Rele 5", row, style: SGICellStyles.TableHeader);

            sheetData.AppendChild(row);
        }



        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsTRM(DashboardViewModels[] reports, string id, string startDate, string endDate)
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
                    columns.Append(CreateColumnData(1, 10, 15));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate) && startDate.Equals("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = startDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = endDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }
                
                    var _report = reports[0];
                    // table headers
                    AddReportTRMTableHeader(_report);

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        AddCell(report.Date.ToString("dd/MM/yyyy"), row, style: SGICellStyles.Border);
                        AddCell(report.Date.ToString("HH:mm"), row, style: SGICellStyles.Border);

                        AddCell((bool)report.Bits?.Bed1 ? "On" : "Off", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bed2 ? "On" : "Off", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bed3 ? "On" : "Off", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bed4 ? "On" : "Off", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bsd1 ? "On" : "Off", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bsd2 ? "On" : "Off", row, style: SGICellStyles.Border);

                        AddCell(report.EntradaAnalogica, row, style: SGICellStyles.Border);
                        AddCell(report.SaidaAnalogica, row, style: SGICellStyles.Border);

                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:K1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }

        private void AddReportTRMTableHeader(DashboardViewModels report)
        {
            //2º Linha
            var row = new Row();
            AddCell("Data", row, style: SGICellStyles.TableHeader);
            AddCell("Hora", row, style: SGICellStyles.TableHeader);
            AddCell(report.Ed1 == null ? "ED1": report.Ed1, row, style: SGICellStyles.TableHeader);
            AddCell(report.Ed2 == null ? "ED2": report.Ed2, row, style: SGICellStyles.TableHeader);
            AddCell(report.Ed3 == null ? "ED3": report.Ed3, row, style: SGICellStyles.TableHeader);
            AddCell(report.Ed4 == null ? "ED4": report.Ed4, row, style: SGICellStyles.TableHeader);
            AddCell(report.Sd1 == null ? "SD1": report.Sd1, row, style: SGICellStyles.TableHeader);
            AddCell(report.Sd2 == null ? "SD2": report.Sd2, row, style: SGICellStyles.TableHeader);
            
            AddCell(report.Ea10 == null ? "Entrada EA10" : report.Ea10, row, style: SGICellStyles.TableHeader);
            AddCell(report.Sa3 == null ? "Saída SA3" : report.Sa3, row, style: SGICellStyles.TableHeader);

            sheetData.AppendChild(row);
        }


        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsTRM10(DashboardViewModels[] reports, string id, string startDate, string endDate, ReportResilType reportType = ReportResilType.Analitico)
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
                    columns.Append(CreateColumnData(2, 3, 10));
                    columns.Append(CreateColumnData(4, 7, 20));
                    columns.Append(CreateColumnData(8, 10, 10));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate) && startDate.Equals("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = startDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = endDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }
                        

                    // table headers
                    AddReportTRM10TableHeader(reportType);

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        // analítico
                        if (reportType == ReportResilType.Analitico)
                        {
                            AddCell(report.Date != default(DateTime) ? report.Date.ToString("dd/MM/yyyy") : "", row, style: SGICellStyles.Border);
                            AddCell(report.Date != default(DateTime) ? report.Date.AddHours(-3).ToString("HH:mm:ss") : "", row, style: SGICellStyles.Border);

                            AddCell(report.FluxoAgua, row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoAgua, row, style: SGICellStyles.Border);

                            AddCell(report.ConsumoDia, row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoSemana, row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoMes, row, style: SGICellStyles.Border);

                            AddCell(report.Modo, row, style: SGICellStyles.Border);
                            AddCell(report.Estado, row, style: SGICellStyles.Border);
                            AddCell(report.Valvula, row, style: SGICellStyles.Border);
                        }

                        // hora
                        if (reportType == ReportResilType.Hora)
                        {
                            AddCell(report.Date != default(DateTime) ? report.Date.ToString("dd/MM/yyyy") : "", row, style: SGICellStyles.Border);
                            AddCell(report.Date != default(DateTime) ? report.Date.ToString("HH:00") : "", row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoAgua, row, style: SGICellStyles.Border);
                        }

                        // dia
                        if (reportType == ReportResilType.Dia)
                        {
                            AddCell(report.Date != default(DateTime) ? report.Date.ToString("dd/MM/yyyy") : "", row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoDia, row, style: SGICellStyles.Border);
                        }

                        // semana
                        if (reportType == ReportResilType.Semana)
                        {
                            AddCell(report.Date != default(DateTime) ? report.Date.ToString("dd/MM/yyyy") : "", row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoSemana, row, style: SGICellStyles.Border);
                        }

                        // mês
                        if (reportType == ReportResilType.Mes)
                        {
                            string montName = report.Date != default(DateTime) ? $"{Utils.GetMonthName(report.Date.Month-1)}/{report.Date.Year}" : "";
                            AddCell(montName, row, style: SGICellStyles.Border);
                            AddCell(report.ConsumoMes, row, style: SGICellStyles.Border);
                        }


                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:J1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }
        private void AddReportTRM10TableHeader(ReportResilType reportType)
        {
            //2º Linha
            var row = new Row();

            // analítico
            if (reportType == ReportResilType.Analitico)
            {
                AddCell("Data", row, style: SGICellStyles.TableHeader);
                AddCell("Hora", row, style: SGICellStyles.TableHeader);
                AddCell("Vazão (m³/h)", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo dia (m³)", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo semana (m³)", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo mês (m³)", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo hora (m³/h)", row, style: SGICellStyles.TableHeader);
                AddCell("Modo", row, style: SGICellStyles.TableHeader);
                AddCell("Estado", row, style: SGICellStyles.TableHeader);
                AddCell("Válvula", row, style: SGICellStyles.TableHeader);
            }

            // hora
            if (reportType == ReportResilType.Hora)
            {
                AddCell("Data", row, style: SGICellStyles.TableHeader);
                AddCell("Hora", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo hora (m³/h)", row, style: SGICellStyles.TableHeader);
            }

            // dia
            if (reportType == ReportResilType.Dia)
            {
                AddCell("Data", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo dia (m³)", row, style: SGICellStyles.TableHeader);
            }

            // semana
            if (reportType == ReportResilType.Semana)
            {
                AddCell("Data", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo semana (m³)", row, style: SGICellStyles.TableHeader);
            }

            // mês
            if (reportType == ReportResilType.Mes)
            {
                AddCell("Data", row, style: SGICellStyles.TableHeader);
                AddCell("Consumo mês (m³)", row, style: SGICellStyles.TableHeader);
            }

            sheetData.AppendChild(row);
        }



        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsTSP(DashboardViewModels[] reports, string id, string startDate, string endDate)
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
                    columns.Append(CreateColumnData(2, 5, 12));
                    columns.Append(CreateColumnData(6, 6, 30));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate) && startDate.Equals("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = startDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = endDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }
                        

                    // table headers
                    AddReportTSPTableHeader();

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        AddCell(report.Date != default(DateTime) ? report.Date.ToString("dd/MM/yyyy") : "", row, style: SGICellStyles.Border);
                        AddCell(report.Date != default(DateTime) ? report.Date.AddHours(-3).ToString("HH:mm:ss") : "", row, style: SGICellStyles.Border);

                        AddCell(report.Vazao, row, style: SGICellStyles.Border);
                        AddCell(report.Totalizacao, row, style: SGICellStyles.Border);
                        // AddCell(report.Totalizacao, row, style: SGICellStyles.Border);
                        // AddCell(report.CalhaAlerta, row, style: SGICellStyles.Border);

                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:F1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }
        private void AddReportTSPTableHeader()
        {
            //2º Linha
            var row = new Row();
            AddCell("Data", row, style: SGICellStyles.TableHeader);
            AddCell("Hora", row, style: SGICellStyles.TableHeader);
            AddCell("Vazão Instantânea (m³/h)", row, style: SGICellStyles.TableHeader);
            AddCell("Totalização 2 (m³)", row, style: SGICellStyles.TableHeader);
            // AddCell("Total parcial", row, style: SGICellStyles.TableHeader);
            // AddCell("Alerta", row, style: SGICellStyles.TableHeader);

            sheetData.AppendChild(row);
        }



        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsMCond(MCond[] reports, string id, string startDate, string endDate)
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
                    columns.Append(CreateColumnData(1, 1, 20));
                    columns.Append(CreateColumnData(2, 11, 22));
                    // columns.Append(CreateColumnData(6, 6, 30));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate) && startDate.Equals("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = startDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = endDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }

                    bool packSup = (reports[0].PackSup != null);
                    bool packInf = (reports[0].PackInf != null);
                    bool packPort = (reports[0].PackPort != null);

                    // table headers
                    AddReportMCondTableHeader(packSup, packInf, packPort);

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        AddCell(report.Date != default(DateTime) ? report.Date.ToString("dd/MM/yyyy HH:mm:ss") : "", row, style: SGICellStyles.Border);

                        if (packSup)
                        {
                            AddCell(String.Format("{0:N}", report.SupLevelLitros), row, style: SGICellStyles.Border);
                            AddCell(report.AlarmeSupTextReport, row, style: SGICellStyles.Border);
                        }

                        if (packInf)
                        {
                            AddCell(String.Format("{0:N}", report.InfLevelLitros), row, style: SGICellStyles.Border);
                            AddCell(report.AlarmeInfTextReport, row, style: SGICellStyles.Border);
                        }

                        if (packSup)
                            AddCell(report.SupStateBombTextReport, row, style: SGICellStyles.Border);

                        if (packPort)
                        {
                            AddCell(report.PortFireStateText, row, style: SGICellStyles.Border);
                            AddCell(report.PortIvaStateText, row, style: SGICellStyles.Border);
                        }

                        AddCell(report.Latitude, row, style: SGICellStyles.Border);
                        AddCell(report.Longitude, row, style: SGICellStyles.Border);

                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:F1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }
        private void AddReportMCondTableHeader(bool packSup, bool packInf, bool packPort)
        {
            //2º Linha
            var row = new Row();
            AddCell("Data", row, style: SGICellStyles.TableHeader);

            if (packSup)
            {
                AddCell("Nível", row, style: SGICellStyles.TableHeader);
                AddCell("Alerta de Nível Superior", row, style: SGICellStyles.TableHeader);
            }
            if (packInf)
            {
                AddCell("Nível", row, style: SGICellStyles.TableHeader);
                AddCell("Alerta de Nível Inferior", row, style: SGICellStyles.TableHeader);
            }
            if (packSup)
                AddCell("Bomba", row, style: SGICellStyles.TableHeader);

            if (packPort)
            {
                AddCell("Alarme de Incêndio", row, style: SGICellStyles.TableHeader);
                AddCell("Alarme IVA", row, style: SGICellStyles.TableHeader);
            }
            
            AddCell("Latitude", row, style: SGICellStyles.TableHeader);
            AddCell("Longitude", row, style: SGICellStyles.TableHeader);

            sheetData.AppendChild(row);
        }



        /// <summary>
        /// Exports a collection of results to Excel.
        /// </summary>
        /// <param name="reports">The calc reports</param>
        /// <returns></returns>
        public byte[] ExportReportsB980(DashboardViewModels[] reports, string id, string startDate, string endDate)
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
                    columns.Append(CreateColumnData(1, 10, 15));
                    worksheetPart.Worksheet.Append(columns);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Relatório" };
                    sheets.Append(sheet);

                    sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Title
                    if (string.IsNullOrWhiteSpace(startDate) && startDate.Equals("null"))
                        AddTitle($"Relatório do dispositivo {id}.");
                    else
                    {
                        startDate = startDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(startDate).ToShortDateString();
                        endDate = endDate.Equals("null") ? "ꝏ" : Convert.ToDateTime(endDate).ToShortDateString();
                        AddTitle($"Relatório do dispositivo {id}, período {startDate} - {endDate}");
                    }
                
                    var _report = reports[0];
                    // table headers
                    AddReportTRMTableHeader(_report);

                    // exports the reports
                    foreach (var report in reports)
                    {
                        var row = new Row();

                        AddCell(report.Date.ToString("dd/MM/yyyy"), row, style: SGICellStyles.Border);
                        AddCell(report.Date.ToString("HH:mm"), row, style: SGICellStyles.Border);

                        AddCell((bool)report.Bits?.N1 ? "Alto" : "Baixo", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.N2 ? "Alto" : "Baixo", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.N3 ? "Alto" : "Baixo", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.N4 ? "Alto" : "Baixo", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bsd1 ? "On" : "Off", row, style: SGICellStyles.Border);
                        AddCell((bool)report.Bits?.Bsd2 ? "On" : "Off", row, style: SGICellStyles.Border);

                        AddCell(report.EntradaAnalogica, row, style: SGICellStyles.Border);
                        AddCell(report.SaidaAnalogica, row, style: SGICellStyles.Border);

                        sheetData.AppendChild(row);
                    }

                    // adds the footer
                    AddFooter();

                    // create a MergeCells class to hold each MergeCell
                    MergeCells mergeCells = new MergeCells();
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:K1") });
                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                    workbookPart.Workbook.Save();
                }
                return mem.ToArray();
            }

        }



    }
}