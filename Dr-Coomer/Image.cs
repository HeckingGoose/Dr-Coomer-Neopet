using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Test
{
    internal class Image
    {
        // Variables
        private IntPtr pointer = IntPtr.Zero;

        // Constructor
        public Image(string fileName)
        {
            pointer = SDL_image.IMG_Load(fileName);
        }

        // Methods
        public IntPtr Pointer
        {
            get { return pointer; }
        }
    }
}
