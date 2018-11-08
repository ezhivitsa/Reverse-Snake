using System;

namespace Assets.ReverseSnake.Scripts.Attributes
{
    class TextureAttribute : Attribute
    {
        internal TextureAttribute(string texture)
        {
            Texture = texture;
        }

        public string Texture { get; private set; }
    }
}
