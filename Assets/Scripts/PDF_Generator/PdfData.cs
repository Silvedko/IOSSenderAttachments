using System;

namespace PDF_Generator
{
    [Serializable]
    public class PdfData
    {
        public PdfImageData ImageData;
        public PdfUserData UserData;
	    public PdfConfigData ConfigData;
		public int Language;
    }
}
