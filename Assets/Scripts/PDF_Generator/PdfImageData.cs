using System;
using UnityEngine;

namespace PDF_Generator
{
    [Serializable]
    public class PdfImageData
    {
        public byte[][] Images;

        private int _index;

        public PdfImageData()
        {
            
            _index = 0;
            Images = new byte[4][];
        }

        public void AddImage(byte[] image)
        {
            Debug.Log("Add im");
            Images[_index] = image;
            _index ++;
        }
    }
}
