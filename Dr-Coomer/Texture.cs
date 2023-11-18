using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Test
{
    internal class Texture
    {
        // Variables
        private IntPtr _pointer;
        private SDL.SDL_Rect _rect;

        // Constructors
        public Texture() // Default constructor
        {
            _pointer = IntPtr.Zero;
            _rect = new SDL.SDL_Rect();
        }
        public Texture(IntPtr renderer, string fileName) // Load texture from file
        {
            _pointer = SDL_image.IMG_LoadTexture(renderer, fileName);
            _rect = new SDL.SDL_Rect();

            if (_pointer != IntPtr.Zero)
            {
                SDL.SDL_QueryTexture(_pointer, out uint format, out int access, out int w, out int h);
                _rect.x = 0;
                _rect.y = 0;
                _rect.w = w;
                _rect.h = h;
            }
        }
        public Texture(IntPtr pointer, SDL.SDL_Rect rect) // Pass in values
        {
            _pointer = pointer;
            _rect = rect;
        }

        // Methods
        public IntPtr Pointer
        {
            get { return _pointer; }
        }
        public SDL.SDL_Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
    }
}
