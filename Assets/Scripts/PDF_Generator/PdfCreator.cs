using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Threading;
using Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;

namespace PDF_Generator
{
    public struct MarginStruct
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public MarginStruct(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }

	public struct ImageSize
	{
		public float Width;
		public float Height;

		public ImageSize(float width, float heigth)
		{
			Width = width;
			Height = heigth;
		}
	}

    public class PdfCreator
    {
        private readonly MarginStruct _marginStruct = new MarginStruct(60, 60, 30, 15);
        private readonly Rectangle _pageRectangle;
        private string _fontPath;
        private BaseFont _font;
        private readonly BaseColor _backgroundColor = new BaseColor(255, 255, 255, 255);
        private readonly BaseColor _textColor = new BaseColor(64, 63, 69, 255);
        private readonly string _persistentDataPath;
	    private byte[] _logoBuffer;

	    private ImageSize _logoSize = new ImageSize(100, 50);
	    private ImageSize _exteriorSize = new ImageSize(2048 / 5f, 1536 / 5f);
	    private ImageSize _interiorSize = new ImageSize(2048 / 5f, 1536 / 5f);

        public PdfCreator()
        {
			_pageRectangle = new Rectangle(595, 842);
            _persistentDataPath = Application.persistentDataPath;
			_pageRectangle.BackgroundColor = _backgroundColor;
        }

	   

		public void CreatePdf(Action<string> callback)
        {
            new Task(CreatePdfRoutine(callback));
        }

        private IEnumerator CreatePdfRoutine(Action<string> callback)
        {
			
			var path = "";
            var workingThread = new Thread(() =>
            {
                path = Create();
            });
            workingThread.Start();
            while (workingThread.IsAlive)
            {
                yield return null;
            }
            callback(path);
        }

        private string Create()
        {
	        var path = PdfPath();
            var doc = new Document(_pageRectangle, _marginStruct.Left, _marginStruct.Right, _marginStruct.Top, _marginStruct.Bottom);
            var pdfWriter = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();

            CreatePage1(doc, pdfWriter);

			doc.Close();
            doc.Dispose();
            pdfWriter.Close();
            pdfWriter.Dispose();
            return path;
        }

        private string PdfPath()
        {
            var strB = new StringBuilder();
            strB.Append(_persistentDataPath);
            strB.Append("/");
            strB.Append("TestPdf");
            var date = DateTime.Now;
            strB.Append("_");
            strB.Append(date.Hour.ToString("D2"));
            strB.Append(date.Minute.ToString("D2"));
            strB.Append(date.Day.ToString("D2"));
            strB.Append(date.Month.ToString("D2"));
            strB.Append(date.Year);
            strB.Append(".pdf");
            return strB.ToString();
        }

        #region Pages

        private void CreatePage1(Document doc, PdfWriter pdfWriter)
        {
            doc.NewPage();

            var directContent = pdfWriter.DirectContent;
            var subjectDateTable = new PdfPTable(2);
            
            //Subject
            var subjectText = "Hi";
            var subjectFont = new Font(_font, 8, Font.NORMAL, _textColor);
            var subjectPhrase = new Phrase(subjectText, subjectFont);
            var subjectCell = new PdfPCell(subjectPhrase);
            subjectCell.HorizontalAlignment = Element.ALIGN_LEFT;
            subjectCell.Border = Rectangle.NO_BORDER;
            subjectDateTable.AddCell(subjectCell);


            var dateCell = new PdfPCell();
            var dateFontNormal = new Font(_font, 8, Font.NORMAL, _textColor);
            dateCell.Phrase = new Phrase("Hi" + ", " + DateTime.Now.ToString(@"dd\/MM\/yyyy"), dateFontNormal);
            

            dateCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            dateCell.Border = Rectangle.NO_BORDER;
            subjectDateTable.AddCell(dateCell);

            subjectDateTable.TotalWidth = _pageRectangle.Width - _marginStruct.Left - _marginStruct.Right;
            subjectDateTable.WriteSelectedRows(0, -1, _marginStruct.Left, 600, directContent);

            //MainMessage
            var mainFont = new Font(_font, 10.5f, Font.NORMAL, _textColor);
            var mainPhrase = new Phrase(GetMainMessage(), mainFont);
            var columnText = new ColumnText(directContent);
            columnText.SetSimpleColumn(mainPhrase, _pageRectangle.Width - _marginStruct.Right, 0, _marginStruct.Left, 550, 20, Element.ALIGN_LEFT);
            columnText.Go();
        }

        private string GetMainMessage()
        { 
			return "Hello";
        }

        #endregion
    }
}
