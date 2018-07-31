using System.Drawing;

namespace AltaOffice
{
    public interface IExcelService
    {
        void Create(string fileName);
        void Open(string fileName);
        void AddSheet(string sheetName = "");
        bool CheckSheetVisibility();
        void CloseExcel();
        void CloseWorkbook();
        void CloseWorkbook(bool toSave);
        void CopyRange(object topLeftRow, object topLeftColumn, object bottomRightRow, object bottomRightColumn, int newY);
        void CopySheetToBuffer(int sheetIndex);
        void DeleteAllPictures();
        void DeleteRow(int row);
        void FindQualificationForVostok(string search, string except1, string except2);
        int FindRow(string search);
        void FindTechSpecForVostok(string search, string except);
        string GetCell(object rowIndex, object columnIndex);
        string GetLabelValue(string label);
        int GetRowsCount();
        int GetSheetsCount(bool all = true);
        int InsertRow(int toRow);
        bool IsSheetOpened();
        void MergeCells(object startRow, object startColumn, object endRow, object endColumn);
        void PasteRangeTo(object row, object column, object row2, object column2, int lotCount);
        void SaveAsPDF(string fileName, bool isPortrait = true);
        void SetActiveSheet(string sheetTitle);
        void SetActiveSheetByName(string sheetTitle);
        void SetCellBackgroundColor(object row, object column, Color colorItem);
        void SetCellBoldStyle(object row, object column, bool isBold = false);
        void SetCellBorder(object row, object column);
        void SetCellFontHAlignment(object row, object column, int type);
        void SetCellFontName(object row, object column, string fontName, bool forAll = false);
        void SetCellFontSize(object row, object column, int fontSize, bool forAll = false);
        void SetCellFontVAlignment(object row, object column, int type);
        void SetCellForegroundColor(object row, object column, Color colorItem);
        void SetCellItalicStyle(object row, object column, bool isItalic = false);
        void SetCells(object row, object column, object value);
        void SetCells(object row, object column, object value, string numberFormat);
        void SetCells(object row, object column, object value, int style);
        void SetCellWrapText(object row, object column, bool wrapText);
        void SetColumnWidth(object row, object column, double columnWidth);
        void SetRange(object row, object column);
        void SetRowAutoFit(object row, object column);
        bool SetSheetByIndex(int index);
        void SetPagesFit();
        void ChangeSheetName(int lotCount, string v);
    }
}