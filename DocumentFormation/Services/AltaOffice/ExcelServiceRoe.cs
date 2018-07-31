using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using unvell.ReoGrid;
using unvell.ReoGrid.Drawing;

namespace AltaOffice
{
    public class ExcelServiceRoe : IExcelService
    {
        private IWorkbook workbook;
        private Worksheet worksheet;
        private string fileName;

        public ExcelServiceRoe(string fileName)
        {
            Open(fileName);
        }

        private CellPosition GetCellPosition(object row, object column)
        {
            CellPosition cell;
            if (column is int)
                cell = new CellPosition((int)row, (int)column);
            else
                cell = new CellPosition(string.Format("{0}{1}", column, row));
            return cell;
        }

        private RangePosition GetRangePosition(object row, object column)
        {
            return new RangePosition(GetCellPosition(row, column), GetCellPosition(row, column));
        }

        private RangePosition GetRangePosition(object startRow, object startColumn, object endRow, object endColumn)
        {
            return new RangePosition(GetCellPosition(startRow, startColumn), GetCellPosition(endRow, endColumn));
        }

        public void AddSheet(string sheetName = "")
        {
            worksheet = workbook.CreateWorksheet(sheetName);
        }

        public bool CheckSheetVisibility()
        {
            return false;
        }

        public void CloseExcel()
        {
            CloseWorkbook();
        }

        public void CloseWorkbook()
        {
            workbook = null;
        }

        public void CloseWorkbook(bool toSave)
        {
            if (toSave)
            {
                workbook.Save(fileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
            }
            CloseWorkbook();
        }

        public void CopyRange(object topLeftRow, object topLeftColumn, object bottomRightRow, object bottomRightColumn, int newY)
        {
            RangePosition rangeFrom = GetRangePosition(topLeftRow, topLeftColumn, bottomRightRow, bottomRightColumn);
            RangePosition rangeTo = GetRangePosition((int)topLeftRow + newY, topLeftColumn, (int)bottomRightRow + newY, bottomRightColumn);
            worksheet.CopyRange(rangeFrom, rangeTo);
        }

        public void CopySheetToBuffer(int sheetIndex)
        {
            var worksheet = workbook.Worksheets[sheetIndex];
            if (worksheet != null && worksheet.CanCopy())
                worksheet.Copy();
        }

        public void Create(string fileName)
        {
            this.fileName = fileName;
            if (workbook == null)
            {
                workbook = ReoGridControl.CreateMemoryWorkbook();
            }
        }

        public void DeleteAllPictures()
        {
            List<IDrawingObject> ImageObjectByRemove = new List<IDrawingObject>();
            foreach (var imageObject in worksheet.FloatingObjects)
            {
                if (imageObject is ImageObject)
                {
                    ImageObjectByRemove.Add(imageObject);
                }
            }
            ImageObjectByRemove.ForEach(i => worksheet.FloatingObjects.Remove(i));
        }

        public void DeleteRow(int row)
        {
            worksheet.DeleteRows(row, 1);
        }

        private void FindQualificationForVostok(params string[] except)
        {
            foreach (var sheet in workbook.Worksheets)
            {
                if (except.Any(e => sheet.Cells.Any(c => c.DisplayText.Equals(e))))
                {
                    sheet.Copy();
                }
            }
        }

        public void FindQualificationForVostok(string search, string except1, string except2)
        {
            FindQualificationForVostok(search, except1, except2);
        }

        public int FindRow(string search)
        {
            var cell = worksheet.Cells.FirstOrDefault(c => c.DisplayText.Contains(search));
            return cell != null ? cell.Row : 0;
        }

        public void FindTechSpecForVostok(string search, string except)
        {
            FindQualificationForVostok(search, except);
        }

        public string GetCell(object rowIndex, object columnIndex)
        {
            return worksheet.GetCellText(GetCellPosition(rowIndex, columnIndex));
        }

        // TODO: Изменить название функции
        public string GetLabelValue(string label)
        {
            var cell = worksheet.Cells.FirstOrDefault(c => c.DisplayText.Contains(label));
            return cell != null ? worksheet.GetCellText(cell.Row, cell.Column + 1) : string.Empty;

        }

        public int GetRowsCount()
        {
            return worksheet.UsedRange.Rows + 1;
        }

        public int GetSheetsCount(bool all = true)
        {
            return workbook.Worksheets.Count(s => all ? true : s.SelectionMode != WorksheetSelectionMode.None);
        }

        public int InsertRow(int toRow)
        {
            worksheet.InsertRows(toRow, 1);
            return toRow + 1;
        }

        public bool IsSheetOpened()
        {
            return worksheet != null;
        }

        public void MergeCells(object startRow, object startColumn, object endRow, object endColumn)
        {
            worksheet.MergeRange(GetRangePosition(startRow, startColumn, endRow, endColumn));
        }

        public void Open(string fileName)
        {
            Create(fileName);
            workbook.Load(fileName, unvell.ReoGrid.IO.FileFormat.Excel2007, Encoding.ASCII);
            worksheet = workbook.Worksheets[0];
            worksheet.AutoSplitPage();
        }

        public void PasteRangeTo(object row, object column, object row2, object column2, int lotCount)
        {
            var fromWorksheet = workbook.Worksheets[lotCount];
            var toWorksheet = workbook.Worksheets[lotCount + 1];
            if (fromWorksheet != null && toWorksheet != null)
            {
                var range = GetRangePosition(row, column, row2, column2);
                var data = fromWorksheet.GetRangeData(range);
                toWorksheet.SetRangeData(range, data);
            }
        }

        // TODO: Не работает
        public void SaveAsPDF(string fileName, bool isPortrait = true)
        {
            //var gridControl = new ReoGridControl();

            //worksheet.PrintSettings.PrinterName = "Microsoft Print To PDF";
            //worksheet.CreatePrintSession().Print();
            //workbook.Save(fileName.Replace(".xlsx", ".pdf"), unvell.ReoGrid.IO.FileFormat._Auto);
            throw new NotImplementedException();
        }

        public void SetActiveSheet(string sheetTitle)
        {
            worksheet = workbook.GetWorksheetByName(sheetTitle);
        }

        public void SetActiveSheetByName(string sheetTitle)
        {
            SetActiveSheet(sheetTitle);
        }

        private void SetCellStyle(object row, object column, WorksheetRangeStyle style)
        {
            worksheet.SetRangeStyles(GetRangePosition(row, column), style);
        }

        public void SetCellBackgroundColor(object row, object column, Color colorItem)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.BackColor = colorItem;
            worksheet.SetRangeStyles(range, style);
        }

        public void SetCellBoldStyle(object row, object column, bool isBold = false)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.Bold = isBold;
            worksheet.SetRangeStyles(range, style);
        }

        public void SetCellBorder(object row, object column)
        {
            worksheet.SetRangeBorders(GetRangePosition(row, column), BorderPositions.All, RangeBorderStyle.SilverSolid);
        }


        // TODO: Параметр type изменить на перечесление.
        public void SetCellFontHAlignment(object row, object column, int type)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            switch (type)
            {
                case 1:
                    style.HAlign = ReoGridHorAlign.Left;
                    break;
                case 2:
                    style.HAlign = ReoGridHorAlign.Center;
                    break;
                case 3:
                    style.HAlign = ReoGridHorAlign.Right;
                    break;
                case 4:
                    style.HAlign = ReoGridHorAlign.General;
                    break;
            }
            worksheet.SetRangeStyles(range, style);
        }


        // TODO: Разделить функцию на две, так как при указании параметра forAll смысла указывать параметры row и column нет.
        public void SetCellFontName(object row, object column, string fontName, bool forAll = false)
        {
            var range = forAll ? worksheet.UsedRange : GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.FontName = fontName;
            worksheet.SetRangeStyles(range, style);
        }

        // TODO: Разделить функцию на две, так как при указании параметра forAll смысла указывать параметры row и column нет.
        public void SetCellFontSize(object row, object column, int fontSize, bool forAll = false)
        {
            var range = forAll ? worksheet.UsedRange : GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.FontSize = fontSize;
            worksheet.SetRangeStyles(range, style);
        }

        // TODO: Параметр type изменить на перечесление.
        public void SetCellFontVAlignment(object row, object column, int type)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.VAlign = ReoGridVerAlign.General;
            switch (type)
            {
                case 1:
                    style.VAlign = ReoGridVerAlign.Top;
                    break;
                case 2:
                    style.VAlign = ReoGridVerAlign.General;
                    break;
                case 3:
                    style.VAlign = ReoGridVerAlign.Bottom;
                    break;
                case 4:
                    style.VAlign = ReoGridVerAlign.Middle;
                    break;
            }
            worksheet.SetRangeStyles(range, style);
        }

        public void SetCellForegroundColor(object row, object column, Color colorItem)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            //style.Flag = PlainStyleFlag.TextColor;
            style.TextColor = colorItem;
            worksheet.SetRangeStyles(range, style);
        }

        public void SetCellItalicStyle(object row, object column, bool isItalic = false)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.Italic = isItalic;
            worksheet.SetRangeStyles(range, style);
        }

        public void SetCells(object row, object column, object value)
        {
            worksheet.SetCellData(GetCellPosition(row, column), value);
        }


        // TODO: Параметр style не используется
        public void SetCells(object row, object column, object value, int style)
        {
            SetCellBorder(row, column);
            SetCells(row, column, value);
        }

        public void SetCells(object row, object column, object value, string numberFormat)
        {
            worksheet.SetRangeDataFormat(GetRangePosition(row, column), unvell.ReoGrid.DataFormat.CellDataFormatFlag.Custom, numberFormat);
        }

        public void SetCellWrapText(object row, object column, bool wrapText)
        {
            var range = GetRangePosition(row, column);
            var style = worksheet.GetRangeStyles(range);
            style.TextWrapMode = wrapText ? TextWrapMode.WordBreak : TextWrapMode.NoWrap;
            worksheet.SetRangeStyles(range, style);
        }

        // TODO: Параметр row не нужен, так как функция поразумевает изменение размера столбца.
        public void SetColumnWidth(object row, object column, double columnWidth)
        {
            worksheet.SetColumnsWidth((int)column, 1, (ushort)columnWidth);
        }

        // TODO: Переименовать функцию.
        public void SetRange(object row, object column)
        {
            RangePosition range = GetRangePosition(row, column);
            worksheet.SelectRange(range);
        }

        // TODO: Переменая column не используется.
        public void SetRowAutoFit(object row, object column)
        {
            worksheet.AutoFitRowHeight((int)row);
        }

        public bool SetSheetByIndex(int index)
        {
            worksheet = workbook.Worksheets[index - 1];
            return workbook != null;
        }

        public void SetPagesFit()
        {
            throw new NotImplementedException();
        }

        public void ChangeSheetName(int lotCount, string v)
        {
            throw new NotImplementedException();
        }
    }
}
