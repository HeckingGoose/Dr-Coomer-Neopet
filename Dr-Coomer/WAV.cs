using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using SDL2Test;

namespace Dr_Coomer
{
    internal class WAV
    {
        // Variables
        private IntPtr _pointer;
        private SDL.SDL_AudioSpec _audioSpec;
        private IntPtr _buffer;
        private uint _length;
        private double _secondsLength;

        // Constructors
        public WAV(string fileName) // Load WAV from file
        {
            // Load file
            IntPtr pointer = SDL_HFC.LoadWav(
                fileName,
                out SDL.SDL_AudioSpec audioSpec,
                out IntPtr buffer,
                out uint length);

            // Calculate seconds length
            uint bytesPerSample = (uint)SDL.SDL_AUDIO_BITSIZE(audioSpec.format) / 8;
            double secondsLength = (double)length / (bytesPerSample * audioSpec.channels * audioSpec.freq);

            // Store values
            _pointer = pointer;
            _audioSpec = audioSpec;
            _buffer = buffer;
            _length = length;
            _secondsLength = secondsLength;
        }
        public WAV(
            IntPtr pointer,
            SDL.SDL_AudioSpec audioSpec,
            IntPtr buffer,
            uint length,
            double secondsLength
            ) // Create new WAV with given values
        {
            _pointer = pointer;
            _audioSpec = audioSpec;
            _buffer = buffer;
            _length = length;
            _secondsLength = secondsLength;
        }

        // Methods
        public IntPtr Pointer
        {
            get { return _pointer; }
        }
        public SDL.SDL_AudioSpec AudioSpec
        {
            get { return _audioSpec; }
        }
        public IntPtr Buffer
        {
            get { return _buffer; }
        }
        public uint Length
        {
            get { return _length; }
        }
        public double SecondsLength
        {
            get { return _secondsLength; }
        }
    }
}
