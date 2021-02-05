using System;
using System.Collections.Generic;

namespace GeneticGameOfLife.Core
{
    public class Algorithm
    {
        public List<Board> Boards { get; private set; } = new List<Board>();

        public Algorithm(int boardSize, int popSize, double initDead)
        {
            var rand = new Random();
            for (var i = 0; i < popSize; i++)
            {
                Boards.Add(InitBoard(boardSize, initDead, rand));
            }
        }

        private static Board InitBoard(int boardSize, double initDead, Random rand)
        {
            var board = new bool[boardSize, boardSize];
            for (var i = 0; i < board.GetLength(0); i++)
            {
                for (var j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = rand.NextDouble() > initDead;
                }
            }

            return new Board(board);
        }

        public void Run(int limit, double mutationRate)
        {
            foreach (var board in Boards)
            {
                board.RunFor(limit);
            }

            Boards.Sort();
            Boards.Reverse();

            var count = Boards.Count;
            Boards.RemoveRange((int) Math.Ceiling((double) count / 2), count / 2);

            var rand = new Random();
            for (var i = 0; i < count / 2; i++)
            {
                var child = Boards[rand.Next(0, count / 2 - 1)]
                    .Crossover(Boards[rand.Next(0, count / 2 - 1)], mutationRate, rand);
                Boards.Add(child);
            }
        }
    }
}