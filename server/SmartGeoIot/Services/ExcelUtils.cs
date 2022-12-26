using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.Security.Data;
using Box.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        enum SGICellStyles {
            None = 0,
            Title = 1,
            TableHeader = 2,
            Currency = 3,
            Pct = 4,
            Border = 5,
            BorderMultiline = 6
        }
        private CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        private WorkbookPart workbookPart;
        private WorksheetPart worksheetPart;
        private SheetData sheetData;

        #region GENERIC HELPS

        /// <summary>
        /// Adds the title row.
        /// </summary>
        /// <param name="title">The title</param>
        private void AddTitle(string title){                        
            var row = new Row();
            AddCell(title, row, CellValues.String, "A1", SGICellStyles.Title);
            sheetData.AppendChild(row);
        }

        private void AddTitleWithTotal(string title, string titleTotal){                        
            var row = new Row();
            AddCell(title, row, CellValues.String, "A1", SGICellStyles.Title);
            AddCell(titleTotal, row, CellValues.String, "Q1", SGICellStyles.TableHeader);
            sheetData.AppendChild(row);
        }

        private void AddFooter() {
            var row = new Row();
            sheetData.AppendChild(row);
            row = new Row();
            AddCell("Arquivo gerado em " + DateTime.Now.AddHours(-3).ToString(), row, CellValues.String);
            sheetData.AppendChild(row);
        }


        /// <summary>
        /// Adds a cell to a row
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="row">The cell</param>
        /// <param name="type">The type</param>
        /// <param name="cellReference">The refreence</param>
        /// <param name="style">The style</param>
        /// <returns>The added cell</returns>
        private Cell AddCell(string value, Row row, CellValues type = CellValues.String, String cellReference = null, SGICellStyles style = SGICellStyles.None, CellFormula formula = null){
            Cell cell = new Cell();
            cell.CellReference = cellReference;            
            cell.DataType = type;     
            if (style != SGICellStyles.None) {
                cell.StyleIndex = UInt32Value.FromUInt32((uint)style);
            }            
            cell.Append(new CellValue(value));
            if(formula!=null)
                cell.Append(formula);
            row.Append(cell);
            return cell;

        }

        /// <summary>
        /// Creates the Excel style.
        /// </summary>
        private void AddStyle() {

            var stylesheet = new Stylesheet(
                new NumberingFormats(
                    new NumberingFormat() {
                        NumberFormatId = 164,
                        FormatCode = "\"" + System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol + "\"\\ " + "#,##0.00"
                    },
                    new NumberingFormat() {
                        NumberFormatId = 3453,
                        FormatCode = StringValue.FromString("0.0%")
                    }
                ),
                new Fonts(
                    new Font(                                                               // 0 - The default font.
                        new FontSize(){ Val = 11 },
                        new Color(){ Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName(){ Val = "Calibri" }),
                    new Font(                                                               // 1 - The bold font.
                        new Bold(),
                        new FontSize(){ Val = 11 },
                        new Color(){ Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName(){ Val = "Calibri" }), 
                    new Font(                                                               // 2 - The Italic font.
                        new Italic(),
                        new FontSize(){ Val = 11 },
                        new Color(){ Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName(){ Val = "Calibri" }),
                    new Font(                                                               // 3 - The Big Font                        
                        new FontSize(){ Val = 16 },
                        new Color(){ Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName(){ Val = "Calibri" }),
                    new Font(                                                               // 4 - The Small Font                        
                        new FontSize(){ Val = 8 },
                        new Color(){ Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName(){ Val = "Calibri" })                                                
                ),
                new Fills(
                    new Fill(                                                           // 0 - The default fill.
                        new PatternFill(){ PatternType = PatternValues.None }),
                    new Fill(                                                           // 1 - The default fill of gray 125 (required)
                        new PatternFill(){ PatternType = PatternValues.Gray125}),     
                    new Fill(                                                           // 2 - The gray fill.
                        new PatternFill(new ForegroundColor(){ Rgb = new HexBinaryValue() { Value = "FFCCCCCC"} }){ PatternType = PatternValues.Solid })                    
                ),
                new Borders(
                    new Border(                                                         // 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                new Border(                                                         // 1 - Applies a Left, Right, Top, Bottom border to a cell
                    new LeftBorder(new Color(){ Auto = true }){ Style = BorderStyleValues.Thin },
                    new RightBorder(new Color(){ Auto = true }){ Style = BorderStyleValues.Thin },
                    new TopBorder(new Color(){ Auto = true }){ Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color(){ Auto = true }){ Style = BorderStyleValues.Thin },
                    new DiagonalBorder())
                ),
            new CellFormats(
                new CellFormat(){ FontId = 0, FillId = 0, BorderId = 0},                                // 0 - None
                new CellFormat(AlignHLVC){ FontId = 3, FillId = 0, BorderId = 0, ApplyFont = true },             // 1 - Title
                new CellFormat(AlignHCVC){ FontId = 1, FillId = 2, BorderId = 1, ApplyFont = true, ApplyBorder = true }, // 2 - Table Header                
                new CellFormat(AlignHLVT){ FontId = 4, FillId = 0, BorderId = 1, ApplyFont = true, ApplyBorder = true, NumberFormatId = 164 }, // 3 - Currency 
                new CellFormat(AlignHLVT){ FontId = 4, FillId = 0, BorderId = 1, ApplyFont = true, ApplyBorder = true, NumberFormatId = 3453 }, // 4 - Pct 
                new CellFormat(AlignHLVT){ FontId = 0, FillId = 0, BorderId = 1, ApplyFont = true, ApplyBorder = true }, // 5 - Border
                new CellFormat(AlignHLVTWrapped){ FontId = 0, FillId = 0, BorderId = 1, ApplyFont = true, ApplyBorder = true } // 6 - Multiline
            ));

            // adds the stylesheet
            WorkbookStylesPart sp = workbookPart.AddNewPart<WorkbookStylesPart>();
            sp.Stylesheet = stylesheet;
            sp.Stylesheet.Save();

        }


        private Alignment AlignHCVC {
            get {
                return new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center };
            }
        }

        private Alignment AlignHLVC {
            get {
                return new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center };
            }
        }

        private Alignment AlignHLVT {
            get {
                return new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Top };
            }
        }

        private Alignment AlignHLVTWrapped {
            get {
                return new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Top, WrapText = true };
            }
        }

        /// <summary>
        /// Creates a column.
        /// </summary>
        /// <param name="StartColumnIndex"></param>
        /// <param name="EndColumnIndex"></param>
        /// <param name="ColumnWidth"></param>
        /// <returns></returns>
        private Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth) {
	        Column column;
	        column = new Column();
	        column.Min = StartColumnIndex;
	        column.Max = EndColumnIndex;
	        column.Width = ColumnWidth;
	        column.CustomWidth = true;
	        return column;
        }
        
        #endregion
    }
}