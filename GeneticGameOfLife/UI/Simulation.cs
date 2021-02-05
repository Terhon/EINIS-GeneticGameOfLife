﻿using System;
using System.Linq;
using GeneticGameOfLife.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GeneticGameOfLife.UI
{
    public class Simulation : IUiElement
    {
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        private SpriteBatch _spriteBatch;
        private Texture2D _cellTexture;
        public int CellSize { get; } = 16;
        public int BoardSize { get; private set; } = 30;
        public int PopSize { get; private set; } = 20;
        public double InitFill { get; private set; } = 0.6;

        public int UpdateRate { get; set; } = 100;
        private double _updateRateTimer;
        public bool IsRunning = true;

        public Algorithm Algorithm;
        public Board Board;

        public void Initialize(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _contentManager = content ?? throw new ArgumentNullException(nameof(content));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            Algorithm = new Algorithm(BoardSize, PopSize, InitFill);
            Board = Algorithm.Boards.First();
        }

        public void LoadContent()
        {
            _cellTexture = new Texture2D(_graphicsDevice, 16, 16);
            var data = new Color[16 * 16];
            for (var i = 0; i < data.Length; ++i) data[i] = Color.Black;
            _cellTexture.SetData(data);
        }

        public void UnloadContent()
        {
            _cellTexture.Dispose();
            _contentManager.Unload();
        }

        public void Update(GameTime gameTime)
        {
            _updateRateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!IsRunning) return;
            if (!(_updateRateTimer > UpdateRate)) return;

            Board.Run();
            _updateRateTimer = 0;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Draw(GameTime gameTime)
        {
            if (_spriteBatch == null) return;

            _spriteBatch.Begin();

            var cells = Board.CurrState;
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y])
                        _spriteBatch.Draw(_cellTexture,
                            new Rectangle(x * CellSize, y * CellSize, CellSize + 1, CellSize + 1),
                            Color.White);
                }
            }

            _spriteBatch.End();
        }

        public void Run(int limit, double mutationRate)
        {
            Algorithm.Run(limit, mutationRate);
            Board = Algorithm.Boards.First();
            Board.Reset();
            Console.WriteLine(Board.SurvivedEpochs);
            Console.WriteLine(Board.SurvivingCells);
            Console.WriteLine(Board.GetCycleLength());
        }

        public void Reset(int boardSize, int popSize, double initFill)
        {
            if (boardSize < 1)
                boardSize = 1;
            BoardSize = boardSize;

            if (popSize < 1)
                popSize = 1;
            PopSize = popSize;

            if (initFill < 0.1)
                initFill = 0.1;
            else if (initFill >= 1)
                initFill = 1;
            InitFill = initFill;

            Algorithm = new Algorithm(BoardSize, PopSize, InitFill);
            Board = Algorithm.Boards.First();
        }
    }
}