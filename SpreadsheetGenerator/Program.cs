using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KeyboardTest;

// using Excel = Microsoft.Office.Interop.Excel;



// https://msdn.microsoft.com/en-us/library/office/cc861607.aspx
// Added references:
//  DocumentFormat.OpenXml
//  WindowsBase


namespace SpreadsheetGenerator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            // https://msdn.microsoft.com/en-us/library/documentformat.openxml.packaging.spreadsheetdocument(v=office.14).aspx#Examples

            string fileName = @"C:\Users\Jens\Documents\temp\PerkinsKeys.xlsx";

            // Create a spreadsheet document by supplying the file name.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                Create(fileName, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "PerkinsKeys"         
            };
            sheets.Append(sheet);


            // https://forums.asp.net/t/1949583.aspx?Open+XML+Adding+rows+to+a+worksheet
            // http://msdn.microsoft.com/en-us/library/office/gg278309.aspx

            // Get the sheetData cell table.
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


            //AddRow(sheetData, 1, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
#if true
            //// Add a row to the cell table.

            //sheetData.Append(row);
            //addCells(row, new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            //row = new Row() { RowIndex = index++ };
            //sheetData.Append(row);
            //addCells(row, new List<int>() { 11, 22, 33, 44, 55, 66, 77, 88, 99 });
            ////addCells(row, new List<int>() { 111, 222, 333, 444, 555, 666, 777, 888, 999 });

            //row = new Row() { RowIndex = index++ };
            //sheetData.Append(row);
            //addCells(row, new List<int>() { 111, 222, 333, 444, 555, 666, 777, 888, 999 });

            //row = new Row() { RowIndex = index++ };
            //sheetData.Append(row);
            //addCells(row, new List<string>() { "1111", "2222", "3333", "4444", "5555", "6666", "7777", "8888", "NiNiNiNi" });


            UInt32Value index = 1;
            Row row;
            row = new Row() { RowIndex = index++ };
            
            Keyboard Focus14 = Keyboard.CreateFocus14Keyboard();
            Keyboard Braillant = Keyboard.CreateBraillantKeyboard();
            Keyboard BrailleNoteTouct = Keyboard.CreateBrailleNoteTouchKeyboard();
            Keyboard HimsEdge = Keyboard.CreateKeyboardHimsEdgeKeyboard();
            List<Keyboard> keyboards = new List<Keyboard>() { Focus14, Braillant, BrailleNoteTouct, HimsEdge };
            List<string> rowTexts;
            TestDefinitions testDefinitions = TestDefinitions.Create();
            List<TestStep> testSteps = testDefinitions.TestMusicXmlReaderKeys(true); // True : all tests

            row = new Row() { RowIndex = index++ };
            sheetData.Append(row);
            rowTexts = new List<string>() { "Querty" };
            foreach (Keyboard keyboard in keyboards)
            {
                rowTexts.Add(keyboard.ToString());
            }
            addCells(row, rowTexts);

            // Add one row per testStep
            foreach (TestStep testStep in testSteps)
            {
                row = new Row() { RowIndex = index++ };
                sheetData.Append(row);
                rowTexts = new List<string>() {  };
                rowTexts.Add(testStep.Keys.ToString());

                foreach (Keyboard keyboard in keyboards)
                {
                    PerkinsKeySequence perkinsKeySequence = (testStep.Keys == Keys.None) ? keyboard.GetPerkinsSequence(testStep.KeySequenceList) : keyboard.GetPerkinsSequence(testStep.Keys);                                    
                    rowTexts.Add(perkinsKeySequence.ToString());
                }
                addCells(row, rowTexts);
            }

#endif

                // Close the document.
                spreadsheetDocument.Close();

            Console.WriteLine("The spreadsheet document has been created.\nPress a key.");
            Console.ReadKey();

        }

        static void addCell(Row row, string reference, string value)
        {
            Cell refCell = null;
            Cell newCell2 = new Cell() { CellReference = reference };
            row.InsertBefore(newCell2, refCell);
            // Set the cell value to be a numeric value of 100.
            newCell2.CellValue = new CellValue(value);
            newCell2.DataType = new EnumValue<CellValues>(CellValues.String);
        }

        static void addCells(Row row, List<int> values)
        {
            char colName = 'A';
            foreach (int i in values)
            {
                string name = colName + row.RowIndex.ToString();// "1";
                colName = (char)((int)colName + 1);
                addCell(row, name, i.ToString());
            }
        }

        static void addCells(Row row, List<string> values)
        {
            char colName = 'A';
            foreach (string s  in values)
            {
                string name = colName + row.RowIndex.ToString();// "1";
                colName = (char)((int)colName + 1);
                addCell(row, name, s);
            }
        }



        //static void AddRow(SheetData sheetData, int index, List<int> cells)
        ////static void AddRow(SheetData sheetData, int index)
        //{
        //    // Add a row to the cell table.
        //    Row row;
        //    row = new Row() { RowIndex = 1 };
        //    sheetData.Append(row);
        //    // Add the cell to the cell table at A1.

        //    char rowName = 'A';
        //    foreach (int i in cells)
        //    {
        //        string name = rowName + i.ToString();
        //        rowName = (char)((int)rowName + 1);
        //        Cell cell = new Cell() { CellReference = name };
        //        row.InsertBefore(cell, null);

        //        // Set the cell value to be a numeric value of 100.
        //        cell.CellValue = new CellValue(i.ToString());
        //        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        //    }

        //    //Cell newCell = new Cell() { CellReference = "A1" };
        //    //row.InsertBefore(newCell, null);

        //    //// Set the cell value to be a numeric value of 100.
        //    //newCell.CellValue = new CellValue("103");
        //    //newCell.DataType = new EnumValue<CellValues>(CellValues.Number);

        //    //Cell newCell1 = new Cell() { CellReference = "A1" };
        //    //row.InsertBefore(newCell1, null);

        //    //// Set the cell value to be a numeric value of 100.
        //    //newCell1.CellValue = new CellValue("107");
        //    //newCell1.DataType = new EnumValue<CellValues>(CellValues.Number);


        //    /// ;;;  
        //}

#if false
        // Given a document name and text, 
        // inserts a new work sheet and writes the text to cell "A1" of the new worksheet.

        public static void InsertText(string docName, string text)
        {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
            {
                // Get the SharedStringTablePart. If it does not exist, create a new one.
                SharedStringTablePart shareStringPart;
                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                // Insert the text into the SharedStringTablePart.
                int index = InsertSharedStringItem(text, shareStringPart);

                // Insert a new worksheet.
                WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);

                // Insert cell A1 into the new worksheet.
                Cell cell = InsertCellInWorksheet("A", 1, worksheetPart);

                // Set the value of cell A1.
                cell.CellValue = new CellValue(index.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                // Save the new worksheet.
                worksheetPart.Worksheet.Save();
            }
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a WorkbookPart, inserts a new worksheet.
        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
#endif



    }
}
