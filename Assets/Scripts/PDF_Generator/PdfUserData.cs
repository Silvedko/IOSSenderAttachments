using System;
using Helpers;

namespace PDF_Generator
{
    [Serializable]
    public class PdfUserData
    {
        public string Title;
        public string Name;
        public string FamilyName;
        public string Street;
        public string Zip;
        public string City;
        public string Email;
        public string CarModel;
    }
}
