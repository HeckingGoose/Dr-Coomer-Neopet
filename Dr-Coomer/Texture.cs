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
        private IntPtr pointer = IntPtr.Zero;
        private SDL.SDL_Rect rect;

        // Constructors
        public Texture(IntPtr renderer, string fileName)
        {
            pointer = SDL_image.IMG_LoadTexture(renderer, fileName);
            rect = new SDL.SDL_Rect();

            if (pointer != IntPtr.Zero)
            {
                SDL.SDL_QueryTexture(pointer, out uint format, out int access, out int w, out int h);
                rect.x = 0;
                rect.y = 0;
                rect.w = w;
                rect.h = h;
            }
        }
        public Texture(IntPtr surfacePointer, SDL.SDL_Rect surfaceRect)
        {
            pointer = surfacePointer;
            rect = surfaceRect;
        }

        // Methods
        public IntPtr Pointer
        {
            get { return pointer; }
        }
        public SDL.SDL_Rect Rect
        {
            get { return rect; }
            set { rect = value; }
        }
    }
}
