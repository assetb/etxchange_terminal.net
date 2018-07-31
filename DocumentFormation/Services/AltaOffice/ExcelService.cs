using AltaLog;
using Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace AltaOffice
{
    public class ExcelService : IExcelService
    {
        protected Excel.Application Application;
        protected Excel.Workbook WorkBook;
        protected Excel.Worksheet WorkSheet;


        public ExcelService(string fileName, bool isNew = false)
        {
            if (!isNew) OpenWorkbook(fileName);
            else CreateWorkBook(fileName);
        }


        #region Create Functions
        private void CreateWorkBook(string fileName)
        {
            if (Application == null) Application = new Excel.Application();

            WorkBook = Application.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            //WorkSheet = (Worksheet)WorkBook.Worksheets[1];
            WorkBook.SaveAs(fileName);

            CloseWorkbook(true);
            CloseExcel();

            OpenWorkbook(fileName);
        }
        #endregion


        #region Open Functions

        private Excel.Application OpenExcel() { return Application ?? (Application = new Excel.Application()); }

        public void Create(string fileName) {
            CreateWorkBook(fileName);
        }

        public void Open(string fileName) {
            OpenWorkbook(fileName);
        }


        private Excel.Workbook OpenWorkbook(string fileName)
        {
            if (!File.Exists(fileName)) return null;
            if (!IsExcelOpened()) OpenExcel();

            Application.Workbooks.Open(fileName);
            WorkBook = Application.ActiveWorkbook;
            WorkSheet = Application.ActiveSheet;

            return WorkBook;
        }

        #endregion

        #region Check Functions

        public bool IsExcelOpened()
        {
            return Application != null;
        }


        public bool IsWorkbookOpened()
        {
            return WorkBook != null;
        }


        private bool IsWorkbookOpened(string fileName)
        {
            if (!IsExcelOpened() || !IsWorkbookOpened()) return false;
            return WorkBook.FullName == fileName;
        }


        public bool IsSheetOpened()
        {
            return WorkSheet != null;
        }


        public bool CheckSheetVisibility()
        {
            return WorkSheet.Visible == Excel.XlSheetVisibility.xlSheetVisible ? true : false;
        }
        #endregion

        #region Close Functions

        public void CloseWorkbook()
        {
            CloseWorkbook(false);
        }


        public void CloseWorkbook(bool toSave)
        {
            if (!IsWorkbookOpened()) return;
            WorkBook.Close(toSave);
            WorkBook = null;
        }


        public void CloseExcel()
        {
            if (!IsExcelOpened()) return;
            try {
                foreach (Excel.Workbook book in Application.Workbooks) book.Close(Excel.XlSaveAction.xlDoNotSaveChanges);
                Application.Quit();
            } catch (Exception ex) {
                //Debug.WriteLine(ex.Message);
                AppJournal.Write(GetType().Name + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            Application = null;
        }

        #endregion

        #region Set Cells functions

        public void SetCells(object row, object column, object value)
        {
            WorkSheet.Cells[row, column] = value;
        }


        public void SetCells(object row, object column, object value, int style)
        {
            //            ((Excel.Range)workSheet.Cells[row, column]).Borders.LineStyle = style;
            ((Excel.Range)WorkSheet.Cells[row, column]).Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            SetCells(row, column, value);
        }


        public void SetCellWrapText(object row, object column, bool wrapText)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Cells.WrapText = wrapText;
        }


        public void SetCells(object row, object column, object value, string numberFormat)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).NumberFormat = numberFormat;
            SetCells(row, column, value);
        }


        //public void SetCells(object row, object column, object value, int style, string numberFormat)
        //{
        //    ((Excel.Range)WorkSheet.Cells[row, column]).NumberFormat = numberFormat;
        //    SetCells(row, column, value, style);
        //}


        public void SetCellBoldStyle(object row, object column, bool isBold = false)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Font.Bold = isBold;
        }


        public void SetCellItalicStyle(object row, object column, bool isItalic = false)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Font.Italic = isItalic;
        }


        //public void SetColumnAutoFit(object row, object column)
        //{
        //    ((Excel.Range)WorkSheet.Cells[row, column]).EntireColumn.AutoFit();
        //}


        public void SetRowAutoFit(object row, object column)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).EntireRow.AutoFit();
        }


        public void SetCellBorder(object row, object column)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
        }


        public void SetCellFontHAlignment(object row, object column, int type)
        {
            var align = Excel.XlHAlign.xlHAlignLeft;

            switch (type) {
                case 1:
                    align = Excel.XlHAlign.xlHAlignLeft;
                    break;
                case 2:
                    align = Excel.XlHAlign.xlHAlignCenter;
                    break;
                case 3:
                    align = Excel.XlHAlign.xlHAlignRight;
                    break;
                case 4:
                    align = Excel.XlHAlign.xlHAlignJustify;
                    break;
            }

            ((Excel.Range)WorkSheet.Cells[row, column]).HorizontalAlignment = align;
        }


        public void SetCellFontVAlignment(object row, object column, int type)
        {
            var align = Excel.XlVAlign.xlVAlignCenter;

            switch (type) {
                case 1:
                    align = Excel.XlVAlign.xlVAlignTop;
                    break;
                case 2:
                    align = Excel.XlVAlign.xlVAlignCenter;
                    break;
                case 3:
                    align = Excel.XlVAlign.xlVAlignBottom;
                    break;
                case 4:
                    align = Excel.XlVAlign.xlVAlignJustify;
                    break;
            }

            ((Excel.Range)WorkSheet.Cells[row, column]).VerticalAlignment = align;
        }


        public void SetRange(object row, object column)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Select();
        }


        public void SetCellBackgroundColor(object row, object column, System.Drawing.Color colorItem)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Interior.Color = System.Drawing.ColorTranslator.ToOle(colorItem);
        }


        public void SetCellForegroundColor(object row, object column, System.Drawing.Color colorItem)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).Font.Color = System.Drawing.ColorTranslator.ToOle(colorItem);
        }


        public void SetCellFontName(object row, object column, string fontName, bool forAll = false)
        {
            if (forAll) ((Excel.Range)WorkSheet.UsedRange).Font.Name = fontName;
            else ((Excel.Range)WorkSheet.Cells[row, column]).Font.Name = fontName;
        }


        public void MergeCells(object startRow, object startColumn, object endRow, object endColumn)
        {
            ((Excel.Range)WorkSheet.Range[WorkSheet.Cells[startRow, startColumn], WorkSheet.Cells[endRow, endColumn]]).Merge();
        }


        public void SetCellFontSize(object row, object column, int fontSize, bool forAll = false)
        {
            if (forAll) ((Excel.Range)WorkSheet.UsedRange).Font.Size = fontSize;
            else ((Excel.Range)WorkSheet.Cells[row, column]).Font.Size = fontSize;
        }


        public void SetColumnWidth(object row, object column, double columnWidth)
        {
            ((Excel.Range)WorkSheet.Cells[row, column]).EntireColumn.ColumnWidth = columnWidth;
        }
        #endregion

        #region Get Functions

        #region Get:    GetLabelValue Functions

        //protected string GetLabelValueOverWorkbook(string label, bool isHorizontal)
        //{
        //    foreach (Excel.Worksheet sheet in WorkBook.Worksheets) {
        //        var value = GetLabelValue(label, isHorizontal, sheet);
        //        if (!string.IsNullOrEmpty(value)) return value;
        //    }
        //    return "";
        //}


        //public string GetLabelValue(string label, bool isHorizontal)
        //{
        //    return GetLabelValue(label, isHorizontal, WorkSheet);
        //}


        //public string GetLabelValue(string label, bool isHorizontal, string sheetTitle)
        //{
        //    var sheet = GetSheetByTitle(sheetTitle);
        //    return GetLabelValue(label, isHorizontal, sheet);
        //}


        //private string GetLabelValue(string label, bool isHorizontal, Excel.Worksheet sheet)
        //{
        //    var range = sheet.UsedRange.Find(label);
        //    if (range == null) return "";
        //    string value = isHorizontal ? sheet.UsedRange.Cells[range.Row - 1, range.Column + 1].Text : sheet.UsedRange.Cells[range.Row + 1, range.Column].Text;
        //    return value ?? "";
        //}


        //public string GetLabelValueOverWorkbook(string label, ValueForLabelDirectionEnum direction)
        //{
        //    foreach (Excel.Worksheet sheet in WorkBook.Worksheets) {
        //        var value = GetLabelValue(label, direction, sheet);
        //        if (!string.IsNullOrEmpty(value)) return value;
        //    }
        //    return "";
        //}


        public string GetLabelValue(string label)
        {
            return GetLabelValue(label, ValueForLabelDirectionEnum.Right, WorkSheet);
        }


        //public string GetLabelValue(string label, ValueForLabelDirectionEnum direction)
        //{
        //    return GetLabelValue(label, direction, WorkSheet);
        //}


        //public string GetLabelValue(string label, ValueForLabelDirectionEnum direction, string sheetTitle)
        //{
        //    var sheet = GetSheetByTitle(sheetTitle);
        //    return GetLabelValue(label, direction, sheet);
        //}


        private static string GetLabelValue(string label, ValueForLabelDirectionEnum valueForLabelDirection, Excel.Worksheet sheet)
        {
            var range = sheet.UsedRange.Find(label);
            if (range == null) return "";

            var value = "";
            switch (valueForLabelDirection) {
                case ValueForLabelDirectionEnum.Right: value = sheet.UsedRange.Cells[range.Row - 1, range.Column + 1].Text; break;
                //case ValueForLabelDirectionEnum.Right: value = sheet.UsedRange.Cells[range.Row, range.Column + 1].Text; break;
                case ValueForLabelDirectionEnum.Down: value = sheet.UsedRange.Cells[range.Row + 1, range.Column].Text; break;
                case ValueForLabelDirectionEnum.Left: value = sheet.UsedRange.Cells[range.Row - 1, range.Column - 1].Text; break;
                case ValueForLabelDirectionEnum.Up: value = sheet.UsedRange.Cells[range.Row - 2, range.Column].Text; break;
            }

            return value ?? "";
        }

        public int GetSheetsCount(bool all = true)
        {
            if (all) return WorkBook.Worksheets.Count;
            else {
                int wCount = 0;

                foreach (Excel.Worksheet item in WorkBook.Worksheets) {
                    if (item.Visible == Excel.XlSheetVisibility.xlSheetVisible) wCount++;
                }

                return wCount;
            }
        }

        public void AddSheet(string sheetName = "")
        {
            WorkBook.Sheets.Add(After: WorkBook.Sheets[WorkBook.Sheets.Count]);
            Excel._Worksheet wSheet = WorkBook.Sheets[WorkBook.Sheets.Count];
            wSheet.Name = sheetName;
        }


        public void ChangeSheetName(int sheetIndex, string name)
        {
            Excel._Worksheet wSheet = WorkBook.Sheets[sheetIndex];
            wSheet.Name = name;
        }
        #endregion


        private Excel.Worksheet GetSheetByName(string sheetName)
        {
            return IsWorkbookOpened() ? WorkBook.Worksheets.Cast<Excel.Worksheet>().FirstOrDefault(sheet => sheet.Name == sheetName) : null;
        }


        private Excel.Worksheet GetSheetByTitle(string sheetTitle)
        {
            foreach (Excel.Worksheet sheet in WorkBook.Worksheets) {
                sheet.Activate();
                if (sheet.UsedRange.Find(sheetTitle) != null && sheet.Visible == Excel.XlSheetVisibility.xlSheetVisible) return sheet;
            }
            return null;
        }
        
        public int GetRowsCount()
        {
            return WorkSheet.UsedRange.Rows.Count;
        }


        public string GetCell(object rowIndex, object columnIndex)
        {
            return WorkSheet.Cells[rowIndex, columnIndex].Text;
        }

        #endregion

        #region Set Functions
        public bool SetSheetByIndex(int index)
        {
            if (index <= WorkBook.Worksheets.Count) {
                WorkSheet = WorkBook.Worksheets[index];
                return true;
            } else return false;
        }


        public void SetActiveSheet(string sheetTitle)
        {
            WorkSheet = GetSheetByTitle(sheetTitle);
        }


        public void SetActiveSheetByName(string sheetTitle)
        {
            WorkSheet = GetSheetByName(sheetTitle);
        }


        //public void SetRow() { }


        //public void SetVisibility(bool visibility)
        //{
        //    Application.Visible = visibility;
        //}

        #endregion

        #region Other Functions
        public int InsertRow(int toRow)
        {
            var range = (Excel.Range)WorkSheet.Rows[toRow];
            range.Insert();
            return range.Row;
        }


        public int FindRow(string search)
        {
            var range = WorkSheet.UsedRange.Find(search);

            return range?.Row ?? 0;
        }

        public void DeleteRow(int row)
        {
            var range = (Excel.Range)WorkSheet.Rows[row];
            range.Delete();
        }

        public void FindQualificationForVostok(string search, string except1, string except2)
        {
            foreach (Excel.Worksheet sheet in WorkBook.Worksheets) {
                if (sheet.Visible == Excel.XlSheetVisibility.xlSheetVisible) {
                    if (sheet.UsedRange.Find(search) != null && sheet.UsedRange.Find(except1) == null && sheet.UsedRange.Find(except2) == null) {
                        sheet.UsedRange.Copy();
                    }
                }
            }
        }


        public void FindTechSpecForVostok(string search, string except)
        {
            foreach (Excel.Worksheet sheet in WorkBook.Worksheets) {
                if (sheet.Visible == Excel.XlSheetVisibility.xlSheetVisible) {
                    if (sheet.UsedRange.Find(search) != null && sheet.UsedRange.Find(except) == null) {
                        sheet.UsedRange.Copy();
                    }
                }
            }
        }


        public void CopySheetToBuffer(int sheetIndex)
        {
            WorkSheet = WorkBook.Worksheets[sheetIndex];

            WorkSheet.UsedRange.Copy();
        }


        public void SaveAsPDF(string fileName, bool isPortrait = true)
        {
            ((Excel._Worksheet)WorkBook.ActiveSheet).PageSetup.Orientation = isPortrait ? Excel.XlPageOrientation.xlPortrait : Excel.XlPageOrientation.xlLandscape;
            WorkBook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, fileName.Replace(".xlsx", ".pdf"));
        }


        public void SetPagesFit()
        {
            ((Excel._Worksheet)WorkBook.ActiveSheet).PageSetup.Zoom = false;
            ((Excel._Worksheet)WorkBook.ActiveSheet).PageSetup.FitToPagesWide = 1;
            ((Excel._Worksheet)WorkBook.ActiveSheet).PageSetup.FitToPagesTall = false;
        }


        public void CopyRange(object topLeftRow, object topLeftColumn, object bottomRightRow, object bottomRightColumn, int newY)
        {
            var fromRange = WorkSheet.get_Range(topLeftColumn.ToString() + topLeftRow.ToString() + ":" + bottomRightColumn.ToString() + bottomRightRow.ToString());
            var toRange = WorkSheet.get_Range(topLeftColumn.ToString() + ((int)topLeftRow + newY).ToString() + ":" + bottomRightColumn.ToString() + ((int)bottomRightRow + newY).ToString());

            fromRange.Copy(toRange);
        }


        public void PasteRangeTo(object row, object column, object row2, object column2, int lotCount)
        {
            Excel._Worksheet wSheet1 = WorkBook.Worksheets[lotCount];
            Excel._Worksheet wSheet2 = WorkBook.Worksheets[lotCount + 1];

            //worksheet1.Copy(worksheet3);


            Excel.Range from = wSheet1.get_Range(string.Format("{1}{0}", row, column) + ":" + string.Format("{1}{0}", row2, column2));
            Excel.Range to = wSheet2.get_Range(string.Format("{1}{0}", row, column) + ":" + string.Format("{1}{0}", row2, column2));

            from.Copy(to);

            //Excel.Range range = (Excel.Range)WorkSheet.Cells[row, column];
            ////range.Select();

            //WorkSheet.Paste(range);
        }


        public void DeleteAllPictures()
        {
            foreach (Excel.Shape item in WorkSheet.Shapes) {
                item.Delete();
            }
        }
        #endregion

    }
}
