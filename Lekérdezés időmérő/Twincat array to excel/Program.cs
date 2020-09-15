using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using excel = Microsoft.Office.Interop.Excel;
using TwinCAT.Ads;
using System.Threading;

namespace Twincat_array_to_excel
{
    class Program
    {
        static excel.Application xlApp;
        static excel.Workbook xlWorkBook;
        static excel.Worksheet xlWorksheet;
        static int row = 1;
        static int col = 1;
        static string columnname;
        static int column_to_write;
        static string ExcelPath;// @"D:\Egyetemes szarok 6. félév\PLC projekt\Adatkiértékelés 3.xlsm";

        static TcAdsClient ads;
        private static int hWriteColumn;
        private static int hColumnName;
        private static int hReady;
        private static int hexcelpath;
        private static int hTimeArray;

        static void Main(string[] args)
        {
            ads = new TcAdsClient();
            ads.Connect(851);
            CreateHandles();
            ReadVariables();
            ExcelInit();
            ExcelWriteArray(ReadTimeArray(), column_to_write, columnname);
            ExcelClose();
            Console.WriteLine("array successfully written to excel");
            
            Thread.Sleep(2000);
            ads.WriteAny(hReady, true);




        }
        static void ExcelInit()
        {
            xlApp = new excel.Application();
            //xlApp.Visible = true;
            xlWorkBook = xlApp.Workbooks.Open(ExcelPath);
            excel.Sheets excelsheets = xlWorkBook.Worksheets;
            xlWorksheet = (excel.Worksheet)excelsheets.get_Item(1);
        }
        static void ExcelClose()
        {
            xlWorkBook.Save();
            xlWorkBook.Close();
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }
        static void ExcelWriteArray(int [] arr, int column, string columnname)
        {
            row = 1;
            xlWorksheet.Cells[row, column] = columnname;
            col = column;
            for (int  i = 0;  i <arr.Length;  i++)
            {
                xlWorksheet.Cells[i + 2, column] = arr[i];
            }

        }
        static void CreateHandles()
        {
            hWriteColumn = ads.CreateVariableHandle("MAIN.ExcelInfo.ColumnNumber");
            hColumnName = ads.CreateVariableHandle("MAIN.ExcelInfo.ColumnName");
            hReady = ads.CreateVariableHandle("MAIN.ExcelInfo.bReady");
            hexcelpath = ads.CreateVariableHandle("MAIN.ExcelInfo.path");
        }
        static void ReadVariables()
        {
            column_to_write = Convert.ToInt32( ads.ReadAny(hWriteColumn, typeof(int)));
            columnname =  ads.ReadAny(hColumnName, typeof(string), new int[] { 50 }).ToString();
            ExcelPath= ads.ReadAny(hexcelpath, typeof(string), new int[] { 100 }).ToString();
        }
        static int[]  ReadTimeArray()
        {
            TimeSpan[] timearray = new TimeSpan[80];
            int[] timearray_milisecounds = new int[80];
            AdsStream adsStream = new AdsStream(4);
            AdsBinaryReader reader = new AdsBinaryReader(adsStream);
            for (int i = 0; i < timearray.Length; i++)
            {
                ///AdsStream adsStream = new AdsStream(4);
                //AdsBinaryReader reader = new AdsBinaryReader(adsStream);
                adsStream.Position = 0;
                hTimeArray = ads.CreateVariableHandle("MAIN.time_array["+(i+1)+"]");
                ads.Read(hTimeArray, adsStream);
                timearray[i] = reader.ReadPlcTIME();
                timearray_milisecounds[i] =Convert.ToInt32( timearray[i].TotalMilliseconds);

            }
            return timearray_milisecounds;          
        }

    }
}
