using System;
using System.IO;
using System.Linq;
using System.Threading;
using Word = Microsoft.Office.Interop.Word;

namespace AltaOffice
{
    public sealed class WordService
    {
        private Word.Application word;
        private Word.Document document;


        public WordService(string fileName, bool readOnly)
        {
            OpenDocument(fileName, readOnly);

        }


        public bool IsOpenWord()
        {
            return word != null;
        }


        public Word.Application OpenWord()
        {
            if (!IsOpenWord())
            {
                word = new Word.Application
                {
                    Visible = false,
                    DisplayAlerts = Word.WdAlertLevel.wdAlertsNone
                };
            }
            return word;
        }


        public void SetVisiblity(bool visibility)
        {
            word.Visible = visibility;
        }


        public bool IsOpenDocument()
        {
            return document != null;
        }


        public Word.Document OpenDocument(string fileName)
        {
            return OpenDocument(fileName, true);
        }


        public Word.Document OpenDocument(string fileName, bool readOnly)
        {
            if (!File.Exists(fileName)) return null;

            if (!IsOpenWord()) word = OpenWord();

            if (!IsOpenDocument()) document = word.Documents.Open(fileName, ReadOnly: readOnly);

            return document;
        }


        public void CloseDocument()
        {
            CloseDocument(false);
        }


        public void CloseDocument(bool isSave)
        {
            if (IsOpenWord() && IsOpenDocument())
            {
                var option = isSave ? Word.WdSaveOptions.wdSaveChanges : Word.WdSaveOptions.wdDoNotSaveChanges;
                document.Close(option);
                document = null;
            }
        }


        public void CloseWord(bool isSave)
        {
            if (IsOpenWord())
            {
                var option = isSave ? Word.WdSaveOptions.wdSaveChanges : Word.WdSaveOptions.wdDoNotSaveChanges;
                foreach (Word.Document doc in word.Documents)
                {
                    doc.Close(option);
                }
                word.Quit();
                word = null;
            }
        }


        public string GetParagraph(int paragraphNo)
        {
            var rng = document.Paragraphs[paragraphNo].Range;
            return rng.Text;
        }


        public int GetTablesCount()
        {
            return document.Tables.Count;
        }

        public int GetTableRowsCount(int tableNum)
        {
            return document.Tables[tableNum].Rows.Count;
        }


        public void CopyTable(int table)
        {
            var range = document.Tables[table].Range;
            range.Select();
            range.Copy();
        }


        public void AutoFitTables()
        {
            var tables = document.Tables;
            if (tables.Count > 0)
            {
                foreach (Word.Table table in tables)
                {
                    table.AllowAutoFit = true;
                    table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow);
                }
            }
        }


        public string GetCell(int tableNo, int row, int column)
        {
            var tbl = document.Tables[tableNo];
            return tbl.Cell(row, column).Range.Text;
        }


        public void SetCell(int tableNo, int row, int column, string value)
        {
            document.Tables[tableNo].Cell(row, column).Range.Text = value;
        }

        public void SetBoldInCell(int tableNo, int row, int column, int bold)
        {
            document.Tables[tableNo].Cell(row, column).Range.Bold = bold;
        }

        public void SetItalicInCell(int tableNo, int row, int column, int italic)
        {
            document.Tables[tableNo].Cell(row, column).Range.Italic = italic;
        }

        public void SetTextColorInCell(int tableNo, int row, int column)
        {
            document.Tables[tableNo].Cell(row, column).Range.Font.Color = Word.WdColor.wdColorBlack;
        }

        public void DeleteParagraph(int paragraphNo)
        {
            var rng = document.Paragraphs[paragraphNo].Range;
            rng.Delete();
        }


        public void DeleteTableRows(int table, int[] rows)
        {
            var tbl = document.Tables[table];
            foreach (var row in rows)
            {
                tbl.Rows[row].Delete();
            }
        }


        public void AddTableRow(int table)
        {
            var tbl = document.Tables[table];
            tbl.Rows.Add();
        }


        public void AddTableRow(int table, int beforeRow)
        {
            var tbl = document.Tables[table];
            tbl.Rows.Add(tbl.Rows[beforeRow]);
        }


        public void CopyRange(int start, int end)
        {
            var range = document.Content;
            range.SetRange(start, end);
            range.Copy();
        }


        public void CopyAll()
        {
            document.Content.Select();
            word.Selection.Copy();
        }


        public int GetEnd()
        {
            return document.Content.End;
        }

        public void PasteInTheBookmark(object bookmark)
        {
            document.Bookmarks[bookmark].Range.Paste();
        }

        public void PasteBookmark(object bookmark)
        {
            document.Bookmarks[bookmark].Range.Select();
            document.Bookmarks[bookmark].Range.Paste();

            foreach (Word.Bookmark bm in document.Bookmarks)
            {
                if (bm.Name != "Проект_договора" && bm.Name != "Техническая_спецификация" && bm.Name != "Квалификационные_требования")
                {
                    document.Bookmarks.Add(bookmark.ToString(), bm.Range);
                    bm.Delete();
                    break;
                }
            }

            document.Save();

            AutoFitTables();
        }


        public void DocumentSave()
        {
            document.Save();
        }


        public void SaveAsPDF(string fileName)
        {
            document.ExportAsFixedFormat(fileName.Replace(".docx", ".pdf"), Word.WdExportFormat.wdExportFormatPDF, OptimizeFor: Word.WdExportOptimizeFor.wdExportOptimizeForOnScreen,
                BitmapMissingFonts: true, DocStructureTags: false);
        }


        public void CorrectAfterPaste(string bookmark, string[] validBookmarks)
        {
            foreach (Word.Bookmark bm in document.Bookmarks)
            {
                if (!validBookmarks.Contains(bm.Name))
                {
                    document.Bookmarks.Add(bookmark, bm.Range);
                    bm.Delete();
                    break;
                }
            }
        }


        // Метод поиска и замены текста в Word
        public void FindReplace(string str_old, string str_new)
        {
            var find = word.Selection.Find;

            find.Text = str_old;

            if (str_new.Length < 256) find.Replacement.Text = str_new;
            else find.Replacement.Text = str_new.Substring(0, 254) + "|";

            find.Execute(FindText: Type.Missing, MatchCase: false, MatchWholeWord: false, MatchWildcards: false,
                        MatchSoundsLike: Type.Missing, MatchAllWordForms: false, Forward: true, Wrap: Word.WdFindWrap.wdFindContinue,
                        Format: false, ReplaceWith: Type.Missing, Replace: Word.WdReplace.wdReplaceAll);

            if (str_new.Length > 255)
            {
                Thread.Sleep(2000);

                str_old = "|";
                str_new = str_new.Substring(255);

                FindReplace(str_old, str_new);
            }
        }


        public int FindTextInRow(int table, string toFind, int column)
        {
            for (var iRow = 1; iRow < GetTableRowsCount(table); iRow++)
            {
                if (document.Tables[table].Cell(iRow, column).Range.Text.Contains(toFind)) return iRow;
            }

            return -1;
        }


    }
}
