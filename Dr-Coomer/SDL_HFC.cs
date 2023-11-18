using Dr_Coomer;
using SDL2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Test
{
    /// <summary>
    /// Helpful Function Class (HFC) for SDL.
    /// </summary>
    internal class SDL_HFC
    {
        #region General functions
        /// <summary>
        /// Initialises and returns a window and renderer.
        /// </summary>
        /// <param name="width">The width of the window in pixels.</param>
        /// <param name="height">The height of the window in pixels.</param>
        public static (IntPtr, IntPtr) Init(string windowTitle, int width, int height)
        {
            // Initialise SDL video, returns less than 0 if there was an error
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Debug.WriteLine($"Error initialising. (Error: {SDL.SDL_GetError()})");
            }
            // Initialise SDL audio, returns less than 0 if there was an error
            if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO) < 0)
            {
                Debug.WriteLine($"Error initialising. (Error: {SDL.SDL_GetError()})");
            }
            // Initialise SDL fonts, returns less than 0 if there was an error
            if (SDL_ttf.TTF_Init() < 0)
            {
                Debug.WriteLine($"Error initialising. (Error: {SDL.SDL_GetError()})");
            }

            // Create a window
            IntPtr window = SDL.SDL_CreateWindow(
                windowTitle,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                width,
                height,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
                );
            if (window == IntPtr.Zero)
            {
                Debug.WriteLine($"Unable to initialise window. (Error: {SDL.SDL_GetError()})");
            }

            // Create a renderer
            IntPtr renderer = SDL.SDL_CreateRenderer(
                window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
                );
            if (renderer == IntPtr.Zero)
            {
                Debug.WriteLine($"Unable to initialise renderer. (Error: {SDL.SDL_GetError()})");
            }

            // Initialise SDL_image for PNGs
            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Debug.WriteLine($"Unable to initialise SDL_image. (Error: {SDL_image.IMG_GetError})");
            }
            return (window, renderer);
        }
        /// <summary>
        /// Destroys the given window, renderer and textures.
        /// </summary>
        /// <param name="window">The window to be destroyed.</param>
        /// <param name="renderer">The renderer to be destroyed.</param>
        /// <param name="textures">The textures to be destroyed.</param>
        /// <param name="audioPointers">The audio pointers to be destroyed.</param>
        /// <param name="fonts">The fonts to be destroyed.</param>
        public static void Cleanup(
            IntPtr window,
            IntPtr renderer,
            List<IntPtr> textures,
            List<IntPtr> audioPointers,
            Dictionary<string, IntPtr> fonts
            )
        {
            // Clean up textures
            foreach (IntPtr texture in textures)
            {
                SDL.SDL_DestroyTexture(texture);
            }
            // Clean up audio
            foreach (IntPtr wav in audioPointers)
            {
                SDL.SDL_FreeWAV(wav);
            }
            // Clean up fonts
            foreach (KeyValuePair<string, IntPtr> font in fonts)
            {
                SDL_ttf.TTF_CloseFont(font.Value);
            }

            // Clean up stuff that was made
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
            SDL_ttf.TTF_Quit();
            SDL.SDL_Quit();
        }
        /// <summary>
        /// Refreshes the display given a fill colour
        /// </summary>
        /// <param name="renderer">The renderer to refresh.</param>
        /// <param name="rgba">The RGBA colour to refresh with.</param>
        public static void DisplayRefresh(IntPtr renderer, (Byte, Byte, Byte, Byte) rgba)
        {
            // Sets the rgba colour that the screen will be cleared with.
            if (SDL.SDL_SetRenderDrawColor(renderer, rgba.Item1, rgba.Item2, rgba.Item3, rgba.Item4) < 0)
            {
                Debug.WriteLine($"Unable to set draw colour. (Error: {SDL.SDL_GetError()})");
            }

            // Clears the current render surface.
            if (SDL.SDL_RenderClear(renderer) < 0)
            {
                Debug.WriteLine($"Unable to clear render surface. (Error: {SDL.SDL_GetError()})");
            }
        }
        #endregion
        #region Image functions
        /// <summary>
        /// Loads an image given a file name.
        /// </summary>
        /// <param name="fileName">The name and path of the file to load.</param>
        public static Image LoadImage(string fileName)
        {
            Image o = new Image(fileName);
            if (o.Pointer == IntPtr.Zero)
            {
                Debug.WriteLine($"Unable to load PNG {fileName}. (Error: {SDL.SDL_GetError()})");
            }
            return o;
        }
        /// <summary>
        /// Loads a texture given a renderer and a file name.
        /// </summary>
        /// <param name="renderer">The renderer pointer to use.</param>
        /// <param name="fileName">The name and path of the file to load.</param>
        public static Texture LoadTexture(IntPtr renderer, string fileName)
        {
            Texture o = new Texture(renderer, fileName);
            if (o.Pointer == IntPtr.Zero)
            {
                Debug.WriteLine($"Unable to load PNG {fileName}. (Error: {SDL.SDL_GetError()})");
            }
            return o;
        }
        /// <summary>
        /// Creates a new texture from a given string.
        /// </summary>
        /// <param name="renderer">The renderer to create the texture with</param>
        /// <param name="text">The text to convert to a texture.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="colour">The colour of the text.</param>
        /// <returns></returns>
        public static Texture TextureFromText(IntPtr renderer, string text, IntPtr font, SDL.SDL_Color colour)
        {
            // Create surface
            IntPtr textSurface = SDL_ttf.TTF_RenderUTF8_Solid(font, text, colour);

            // Convert to texture
            IntPtr textTexture = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);

            // Release surface
            SDL.SDL_FreeSurface(textSurface);

            // Query texture for its rect
            SDL.SDL_QueryTexture(textTexture, out _, out _, out int textureWidth, out int textureHeight);

            // Generate rect
            SDL.SDL_Rect textureRect = new SDL.SDL_Rect();
            textureRect.x = 0;
            textureRect.y = 0;
            textureRect.w = textureWidth;
            textureRect.h = textureHeight;

            // Create new texture
            Texture o = new Texture(textTexture, textureRect);

            // Return texture
            return o;
        }
        /// <summary>
        /// Draws a given texture to a given texture at a given (x, y) position, with optional scale parameters.
        /// </summary>
        /// <param name="renderer">The renderer to draw the texture to.</param>
        /// <param name="texture">The texture to draw to the renderer.</param>
        /// <param name="x">The x position to draw the texture at.</param>
        /// <param name="y">The y position to draw the texture at.</param>
        /// <param name="w">The width of the texture in pixels.</param>
        /// <param name="h">The height of the texture in pixels.</param>
        public static void DrawTexture(IntPtr renderer, Texture texture, int x, int y, int w = -1, int h = -1)
        {
            // Create referable copy of texture rect
            SDL.SDL_Rect texRect = texture.Rect;

            // Create rect to draw texture at/to
            SDL.SDL_Rect drawRect = new SDL.SDL_Rect();
            drawRect.x = x;
            drawRect.y = y;
            if (w < 0)
            {
                drawRect.w = texRect.w;
            }
            else
            {
                drawRect.w = w;
            }
            if (h < 0)
            {
                drawRect.h = texRect.h;
            }
            else
            {
                drawRect.h = h;
            }

            SDL.SDL_RenderCopy(renderer, texture.Pointer, ref texRect, ref drawRect);
        }
        /// <summary>
        /// Draws a button to the screen.
        /// </summary>
        /// <param name="renderer">The renderer to draw the button to.</param>
        /// <param name="button">The button to draw.</param>
        public static void DrawButton_Colour(IntPtr renderer, Button_Colour button)
        {
            // Set draw colour
            switch (button.Pressed)
            {
                case 0: // Button not pressed
                    SDL.SDL_SetRenderDrawColor(
                        renderer,
                        button.FillColour.r,
                        button.FillColour.g,
                        button.FillColour.b,
                        button.FillColour.a
                        );
                    break;
                case 1: // Button hover
                    SDL.SDL_SetRenderDrawColor(
                        renderer,
                        button.FillColour_Hover.r,
                        button.FillColour_Hover.g,
                        button.FillColour_Hover.b,
                        button.FillColour_Hover.a
                        );
                    break;
                case 2: // Button pressed
                    SDL.SDL_SetRenderDrawColor(
                        renderer,
                        button.FillColour_Clicked.r,
                        button.FillColour_Clicked.g,
                        button.FillColour_Clicked.b,
                        button.FillColour_Clicked.a
                        );
                    break;
            }

            // Cache rect
            SDL.SDL_Rect rect = button.Rect;

            // Draw button
            SDL.SDL_RenderFillRect(renderer, ref rect);

            // Calculate button text position
            int x = 0, y = 0;

            switch (button.TextFormatting)
            {
                case "middle-centre":
                    // Centre text on top-left of button
                    x = button.Rect.x - button.ButtonTextTexture.Rect.w / 2;
                    y = button.Rect.y - button.ButtonTextTexture.Rect.h / 2;

                    // Move text to centre of button
                    x += button.Rect.w / 2;
                    y += button.Rect.h / 2;
                    break;
            }

            // Draw button text
            DrawTexture(renderer, button.ButtonTextTexture, x, y);
        }
        #endregion
        #region Audio functions
        /// <summary>
        /// Loads a WAV file, with included error reporting.
        /// </summary>
        /// <param name="fileName">Name of WAV file to load.</param>
        /// <param name="audioChannelSpecs">The SDL.SDL_AudioSpec struct matching the WAV file's audio data.</param>
        /// <param name="buffer">A pointer linking to a buffer of the WAV file's audio data.</param>
        /// <param name="length">The byte-length of a single chunk of audio data.</param>
        /// <returns></returns>
        public static IntPtr LoadWav(string fileName, out SDL.SDL_AudioSpec audioChannelSpecs, out IntPtr buffer, out uint length)
        {
           IntPtr result = SDL.SDL_LoadWAV(
               fileName,
               out audioChannelSpecs,
               out buffer,
               out length);
            
            if (result == IntPtr.Zero)
            {
                Debug.WriteLine($"Failed to open WAV '{fileName}'. (Error: {SDL.SDL_GetError()})");
            }
            return result;
        }
        /// <summary>
        /// Creates an audio output channel, with included error reporting. Returns a valid deviceID on success.
        /// </summary>
        /// <param name="audioChannelSPecs">The required specifications of the audio channel.</param>
        /// <returns></returns>
        public static uint CreateAudioChannel(SDL.SDL_AudioSpec audioChannelSPecs)
        {
            uint deviceID = SDL.SDL_OpenAudioDevice(
                IntPtr.Zero,
                0,
                ref audioChannelSPecs,
                out _,
                0);
            if (deviceID == 0)
            {
                Debug.WriteLine($"Unable to open audio channel. (Error: {SDL.SDL_GetError()})");
            }
            return deviceID;
        }
        #endregion
        #region Font functions
        /// <summary>
        /// Loads a TTF font at a given set of point sizes. Returns an empty dict if all sizes fail to load.
        /// </summary>
        /// <param name="fontName">The name of the font to load.</param>
        /// <param name="pointSizes">An array of point sizes to load the font at.</param>
        /// <returns></returns>
        public static void LoadFonts(string fontName, int[] pointSizes, ref Dictionary<string, IntPtr> output)
        {
            foreach (int pointSize in pointSizes)
            {
                IntPtr font = SDL_ttf.TTF_OpenFont(fontName, pointSize);

                if (font == IntPtr.Zero)
                {
                    Debug.WriteLine($"Unable to load TTF '{fontName}'. (Error: {SDL.SDL_GetError()})");
                }
                else
                {
                    string entryName = $"{fontName.Split('\\').Last().Split('.')[0]}_{pointSize}";
                    output.Add(entryName, font);
                }
            }
        }
        #endregion
        #region Collision tests
        /// <summary>
        /// Returns true if the given (x, y) co-ordinate lies within the given bounding box. Otherwise returns false.
        /// </summary>
        /// <param name="boundingBox">The bounding box to check against.</param>
        /// <param name="x">The x co-ordinate to check.</param>
        /// <param name="y">The y co-ordinate to check.</param>
        /// <returns></returns>
        public static bool TestBoundingBox(SDL.SDL_Rect boundingBox, int x, int y)
        {
            if (x >= boundingBox.x && x<= boundingBox.x + boundingBox.w) // If X co-ords match
            {
                if (y >= boundingBox.y && y <=  boundingBox.y + boundingBox.h) // If Y co-ords match
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
