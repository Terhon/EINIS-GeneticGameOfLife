using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using GeneticGameOfLife.Core;
using SharpDX;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GeneticGameOfLife.UI
{
    public class Menu : IUiElement
    {
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        private SpriteBatch _spriteBatch;
        private Simulation _simulation;

        private double _mouseClickTimer;

        private Rectangle _btnStartStopRectangle;
        private Rectangle _btnIterateRectangle;
        private Rectangle _btnBoardSizeUpRectangle;
        private Rectangle _btnBoardSizeDownRectangle;
        private Rectangle _btnPopSizeUpRectangle;
        private Rectangle _btnPopSizeDownRectangle;
        private Rectangle _btnInitFillUpRectangle;
        private Rectangle _btnInitFillDownRectangle;
        private Rectangle _btnMutationUpRectangle;
        private Rectangle _btnMutationDownRectangle;

        private SpriteFont _mainFont;
        private Texture2D _startBtn;
        private Texture2D _stopBtn;
        private Texture2D _iterateBtn;
        private Texture2D _changeBtn;

        private readonly int _buttonWidth = 40;
        private readonly int _buttonHeight = 35;

        private double _mutationRate = 0.3;

        public void Initialize(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public void Initialize(GraphicsDevice graphicsDevice, ContentManager content, Simulation simulation)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _contentManager = content ?? throw new ArgumentNullException(nameof(content));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _simulation = simulation;

            _btnStartStopRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50, 2 * _buttonWidth + 20,
                _buttonHeight);
            _btnIterateRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 45, 2 * _buttonWidth + 20,
                _buttonHeight);
            _btnBoardSizeUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 2*45, _buttonWidth, _buttonHeight);
            _btnBoardSizeDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 2*45, _buttonWidth, _buttonHeight);
            _btnPopSizeUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 3*45, _buttonWidth, _buttonHeight);
            _btnPopSizeDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 3*45, _buttonWidth, _buttonHeight);
            _btnInitFillUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 4*45, _buttonWidth, _buttonHeight);
            _btnInitFillDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 4*45, _buttonWidth, _buttonHeight);
            _btnMutationUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 5*45, _buttonWidth, _buttonHeight);
            _btnMutationDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 5*45, _buttonWidth, _buttonHeight);
        }

        public void LoadContent()
        {
            _mainFont = _contentManager.Load<SpriteFont>("interfaceFontSmall");
            _startBtn = new Texture2D(_graphicsDevice, 2 * _buttonWidth + 20, _buttonHeight);
            var data = new Color[(2 * _buttonWidth + 20) * _buttonHeight];
            for (var i = 0; i < data.Length; ++i) data[i] = Color.Green;
            _startBtn.SetData(data);

            _stopBtn = new Texture2D(_graphicsDevice, 2 * _buttonWidth + 20, _buttonHeight);
            for (var i = 0; i < data.Length; ++i) data[i] = Color.Red;
            _stopBtn.SetData(data);

            _iterateBtn = new Texture2D(_graphicsDevice, 2 * _buttonWidth + 20, _buttonHeight);
            for (var i = 0; i < data.Length; ++i) data[i] = Color.Blue;
            _iterateBtn.SetData(data);

            data = new Color[_buttonWidth * _buttonHeight];
            _changeBtn = new Texture2D(_graphicsDevice, _buttonWidth, _buttonHeight);
            for (var i = 0; i < data.Length; ++i) data[i] = Color.Yellow;
            _changeBtn.SetData(data);
        }

        public void UnloadContent()
        {
            _mainFont = null;
            _startBtn.Dispose();
            _stopBtn.Dispose();
            _iterateBtn.Dispose();
            _changeBtn.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            _mouseClickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton != ButtonState.Pressed || !(_mouseClickTimer > 200)) return;
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            if (mouseRect.Intersects(_btnStartStopRectangle))
            {
                if (_simulation.IsRunning)
                    _simulation.Stop();
                else
                    _simulation.Start();
            }

            if (mouseRect.Intersects(_btnIterateRectangle))
            {
                _simulation.Run(5000, _mutationRate);
            }

            if (mouseRect.Intersects(_btnBoardSizeUpRectangle))
                _simulation.Reset(_simulation.BoardSize + 1, _simulation.PopSize, _simulation.InitFill);
            if (mouseRect.Intersects(_btnBoardSizeDownRectangle))
                _simulation.Reset(_simulation.BoardSize - 1, _simulation.PopSize, _simulation.InitFill);
            if (mouseRect.Intersects(_btnPopSizeUpRectangle))
                _simulation.Reset(_simulation.BoardSize, _simulation.PopSize + 1, _simulation.InitFill);
            if (mouseRect.Intersects(_btnPopSizeDownRectangle))
                _simulation.Reset(_simulation.BoardSize, _simulation.PopSize - 1, _simulation.InitFill);
            if (mouseRect.Intersects(_btnInitFillUpRectangle))
                _simulation.Reset(_simulation.BoardSize, _simulation.PopSize, _simulation.InitFill + 0.1);
            if (mouseRect.Intersects(_btnInitFillDownRectangle))
                _simulation.Reset(_simulation.BoardSize, _simulation.PopSize, _simulation.InitFill - 0.1);
            if (mouseRect.Intersects(_btnMutationUpRectangle))
            {
                _mutationRate += 0.1;
                if (_mutationRate > 1) _mutationRate = 1;
            }

            if (mouseRect.Intersects(_btnMutationDownRectangle))
            {
                _mutationRate -= 0.1;
                if (_mutationRate < 0) _mutationRate = 0;
            }
            
            UpdateRectangles();
            _mouseClickTimer = 0;
        }

        private void UpdateRectangles()
        {
            _btnStartStopRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50, 2 * _buttonWidth + 20,
                _buttonHeight);
            _btnIterateRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 45, 2 * _buttonWidth + 20,
                _buttonHeight);
            _btnBoardSizeUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 2*45, _buttonWidth, _buttonHeight);
            _btnBoardSizeDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 2*45, _buttonWidth, _buttonHeight);
            _btnPopSizeUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 3*45, _buttonWidth, _buttonHeight);
            _btnPopSizeDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 3*45, _buttonWidth, _buttonHeight);
            _btnInitFillUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 4*45, _buttonWidth, _buttonHeight);
            _btnInitFillDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 4*45, _buttonWidth, _buttonHeight);
            _btnMutationUpRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 70,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 5*45, _buttonWidth, _buttonHeight);
            _btnMutationDownRectangle = new Rectangle(
                _simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 25,
                _simulation.Board.BaseState.GetLength(1) * _simulation.CellSize + 1 - 50 - 5*45, _buttonWidth, _buttonHeight);
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (_simulation != null)
            {
                if (_simulation.IsRunning)
                {
                    _spriteBatch.Draw(_stopBtn,
                        new Vector2(_btnStartStopRectangle.X,
                            _btnStartStopRectangle.Y), Color.White);
                    _spriteBatch.DrawString(_mainFont, "Status: running",
                        new Vector2(_simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 10, 10),
                        Color.Lime);
                }
                else
                {
                    _spriteBatch.Draw(_startBtn,
                        new Vector2(_btnStartStopRectangle.X,
                            _btnStartStopRectangle.Y), Color.White);
                    _spriteBatch.DrawString(_mainFont, "Status: not running",
                        new Vector2(_simulation.Board.BaseState.GetLength(0) * _simulation.CellSize + 1 + 10, 10),
                        Color.Red);
                }

                _spriteBatch.Draw(_iterateBtn, new Vector2(_btnIterateRectangle.X,
                    _btnIterateRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "Iterate",
                    new Vector2(_btnIterateRectangle.X + 10, _btnIterateRectangle.Y + 5), Color.Yellow);

                _spriteBatch.Draw(_changeBtn, new Vector2(_btnBoardSizeUpRectangle.X,
                    _btnBoardSizeDownRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "+1", new Vector2(_btnBoardSizeUpRectangle.X + 10,
                    _btnBoardSizeDownRectangle.Y + 5), Color.Blue);
                _spriteBatch.DrawString(_mainFont, "Board size: " + _simulation.BoardSize.ToString(), new Vector2(_btnBoardSizeUpRectangle.X + 45,
                    _btnBoardSizeUpRectangle.Y + 5), Color.Blue);
                
                _spriteBatch.Draw(_changeBtn, new Vector2(_btnBoardSizeDownRectangle.X,
                    _btnBoardSizeDownRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "-1", new Vector2(_btnBoardSizeDownRectangle.X + 10,
                    _btnBoardSizeDownRectangle.Y + 5), Color.Blue);

                _spriteBatch.Draw(_changeBtn, new Vector2(_btnPopSizeUpRectangle.X,
                    _btnPopSizeUpRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "+1", new Vector2(_btnPopSizeUpRectangle.X + 10,
                    _btnPopSizeUpRectangle.Y + 5), Color.Blue);
                _spriteBatch.DrawString(_mainFont, "Population size: " + _simulation.PopSize.ToString(), new Vector2(_btnPopSizeUpRectangle.X + 45,
                    _btnPopSizeUpRectangle.Y + 5), Color.Blue);
                
                _spriteBatch.Draw(_changeBtn, new Vector2(_btnPopSizeDownRectangle.X,
                    _btnPopSizeDownRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "-1", new Vector2(_btnPopSizeDownRectangle.X + 10,
                    _btnPopSizeDownRectangle.Y + 5), Color.Blue);

                _spriteBatch.Draw(_changeBtn, new Vector2(_btnInitFillUpRectangle.X,
                    _btnInitFillUpRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "-0.1", new Vector2(_btnInitFillUpRectangle.X + 10,
                    _btnInitFillUpRectangle.Y + 5), Color.Blue);
                _spriteBatch.DrawString(_mainFont, "Initial fill: " + (1.1-_simulation.InitFill).ToString(), new Vector2(_btnInitFillUpRectangle.X + 45,
                    _btnInitFillUpRectangle.Y + 5), Color.Blue);
                
                _spriteBatch.Draw(_changeBtn, new Vector2(_btnInitFillDownRectangle.X,
                    _btnInitFillDownRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "+0.1", new Vector2(_btnInitFillDownRectangle.X + 10,
                    _btnInitFillDownRectangle.Y + 5), Color.Blue);

                _spriteBatch.Draw(_changeBtn, new Vector2(_btnMutationUpRectangle.X,
                    _btnMutationUpRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "+0.1", new Vector2(_btnMutationUpRectangle.X + 10,
                    _btnMutationUpRectangle.Y + 5), Color.Blue);
                _spriteBatch.DrawString(_mainFont, "Mutation rate: " + _mutationRate.ToString(), new Vector2(_btnMutationUpRectangle.X + 45,
                    _btnMutationUpRectangle.Y + 5), Color.Blue);

                _spriteBatch.Draw(_changeBtn, new Vector2(_btnMutationDownRectangle.X,
                    _btnMutationDownRectangle.Y), Color.White);
                _spriteBatch.DrawString(_mainFont, "-0.1", new Vector2(_btnMutationDownRectangle.X + 10,
                    _btnMutationDownRectangle.Y + 5), Color.Blue);
            }

            _spriteBatch.End();
        }
    }
}