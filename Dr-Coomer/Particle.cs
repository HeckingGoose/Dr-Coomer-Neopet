using SDL2Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dr_Coomer
{
    internal class Particle
    {
        // Variables
        private Texture _texture;
        private Vector2 _position;
        private Vector2 _velocity;
        private Vector2 _accelleration;
        private float _drag;
        private float _lifeTime;
        private bool _gravity;

        // Constructors
        public Particle() // Default constructor
        {
            _texture = new Texture();
            _position = new Vector2();
            _velocity = new Vector2();
            _accelleration = new Vector2();
            _drag = 0;
            _lifeTime = 1;
            _gravity = false;
        }
        public Particle( // Pass in values
            Texture texture,
            Vector2 position,
            Vector2 velocity,
            Vector2 accelleration,
            float drag,
            float lifeTime,
            bool gravity
            )
        {
            _texture = texture;
            _position = position;
            _velocity = velocity;
            _accelleration = accelleration;
            _drag = drag;
            _lifeTime = lifeTime;
            _gravity = gravity;
        }

        // Methods
        public Texture Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        public Vector2 Accelleration
        {
            get { return _accelleration; }
            set { _accelleration = value; }
        }
        public float Drag
        {
            get { return _drag; }
            set { _drag = value; }
        }
        public float LifeTime
        {
            get { return _lifeTime; }
            set { _lifeTime = value; }
        }
        public bool Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }
    }
}
