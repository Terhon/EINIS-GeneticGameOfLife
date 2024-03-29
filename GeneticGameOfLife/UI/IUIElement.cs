﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GeneticGameOfLife.UI
{
    public interface IUiElement
    {
        void Initialize(GraphicsDevice graphicsDevice, ContentManager content);
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}