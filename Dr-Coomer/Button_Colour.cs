using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using SDL2Test;

namespace Dr_Coomer
{
    internal class Button_Colour
    {
        // Variables
        private uint _ID;
        private string _buttonText;
        private IntPtr _font;
        private Texture _buttonTextTexture;
        private SDL.SDL_Rect _rect;
        private SDL.SDL_Color _textColour;
        private SDL.SDL_Color _fillColour;
        private SDL.SDL_Color _fillColour_Hover;
        private SDL.SDL_Color _fillColour_Clicked;
        private Byte _pressed;
        private string _textFormatting;

        // Constructors
        public Button_Colour(
            uint id,
            IntPtr renderer,
            string buttonText,
            IntPtr font,
            SDL.SDL_Rect rect,
            (Byte r, Byte g, Byte b, Byte a) textColour,
            (Byte r, Byte g, Byte b, Byte a) fillColour,
            string textFormatting = "middle-centre"
            )
        {
            _ID = id;
            _buttonText = buttonText;
            _font = font;

            _textColour = new SDL.SDL_Color();
            _textColour.r = textColour.r;
            _textColour.g = textColour.g;
            _textColour.b = textColour.b;
            _textColour.a = textColour.a;

            _buttonTextTexture = SDL_HFC.TextureFromText(renderer, buttonText, font, _textColour);
            _rect = rect;

            _fillColour = new SDL.SDL_Color();
            _fillColour.r = fillColour.r;
            _fillColour.g = fillColour.g;
            _fillColour.b = fillColour.b;
            _fillColour.a = fillColour.a;

            Byte avgColour = (Byte)((fillColour.r + fillColour.g + fillColour.b) / 3);
            int sign = 1;

            if (avgColour < 128)
            {
                sign = -1;
            }

            _fillColour_Hover = new SDL.SDL_Color();
            _fillColour_Hover.r = (Byte)Math.Clamp(fillColour.r - 15 * sign, 0, 256);
            _fillColour_Hover.g = (Byte)Math.Clamp(fillColour.g - 15 * sign, 0, 256);
            _fillColour_Hover.b = (Byte)Math.Clamp(fillColour.b - 15 * sign, 0, 256);
            _fillColour_Hover.a = fillColour.a;

            _fillColour_Clicked = new SDL.SDL_Color();
            _fillColour_Clicked.r = (Byte)Math.Clamp(fillColour.r - 45 * sign, 0, 256);
            _fillColour_Clicked.g = (Byte)Math.Clamp(fillColour.g - 45 * sign, 0, 256);
            _fillColour_Clicked.b = (Byte)Math.Clamp(fillColour.b - 45 * sign, 0, 256);
            _fillColour_Clicked.a = fillColour.a;

            _pressed = 0;
            _textFormatting = textFormatting;
        }

        // Methods
        public uint ID
        {
            get { return _ID; }
        }
        public string ButtonText
        {
            get { return _buttonText; }
        }
        public void SetButtonText(IntPtr renderer, string newText, ref List<IntPtr> textures)
        {
            _buttonText = newText;
            _buttonTextTexture = SDL_HFC.TextureFromText(renderer, _buttonText, _font, _textColour);
            textures.Add(_buttonTextTexture.Pointer);
        }
        public Texture ButtonTextTexture
        {
            get { return _buttonTextTexture; }
        }
        public IntPtr Font
        {
            get { return _font; }
            set { _font = value; }
        }
        public SDL.SDL_Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }
        public SDL.SDL_Color TextColour
        {
            get { return _textColour; }
            set { _textColour = value; }
        }
        public SDL.SDL_Color FillColour
        {
            get { return _fillColour; }
            set { _fillColour = value; }
        }
        public SDL.SDL_Color FillColour_Hover
        {
            get { return _fillColour_Hover; }
            set { _fillColour_Hover = value; }
        }
        public SDL.SDL_Color FillColour_Clicked
        {
            get { return _fillColour_Clicked; }
            set { _fillColour_Clicked = value; }
        }
        public string TextFormatting
        {
            get { return _textFormatting; }
            set { _textFormatting = value; }
        }
        public Byte Pressed
        {
            get { return _pressed; }
            set { _pressed = value; }
        }
    }
}
