using System;
using System.Collections.Generic;

namespace GeneticGameOfLife.Core
{
    public class Board : IComparable<Board>
    {
        public bool[,] BaseState { get; private set; }
        public bool[,] CurrState { get; private set; }
        public int SurvivedEpochs { get; private set; }
        public int SurvivingCells { get; private set; }
        public int Changes { get; private set; }

        private List<Tuple<int, int>> StateTracker;
        private int LastRepeatedIdx;
        private bool InCycle = false;
        private int IterationsInCycle = 0;

        public Board(int x, int y)
        {
            BaseState = new bool[x, y];
            CurrState = BaseState.Clone() as bool[,];
            StateTracker = new List<Tuple<int, int>>();
        }

        public Board(bool[,] board)
        {
            BaseState = board;
            CurrState = board;
            StateTracker = new List<Tuple<int, int>>();
        }

        public void RunFor(int limit)
        {
            SurvivingCells = 0;
            SurvivedEpochs = 0;
            Changes = 0;
            CurrState = BaseState.Clone() as bool[,];

            do
            {
                Run();
                CheckCycle();
            } while (IterationsInCycle < 5 && SurvivedEpochs < limit);
        }

        public void Run()
        {
            var cellsAlive = 0;
            var changedCells = 0;
            var nextState = CurrState.Clone() as bool[,];
            for (var i = 0; i < CurrState.GetLength(0); i++)
            {
                for (var j = 0; j < CurrState.GetLength(1); j++)
                {
                    var living = CurrState[i, j];
                    var count = GetLivingNeighbors(CurrState, i, j);
                    var result = false;

                    if ((living && (count == 2 || count == 3)) || (!living && count == 3))
                    {
                        result = true;
                        cellsAlive++;
                    }

                    if (!result.Equals(living))
                        changedCells++;

                    nextState[i, j] = result;
                }
            }

            SurvivingCells = cellsAlive;
            Changes = changedCells;
            SurvivedEpochs++;
            CurrState = nextState;
        }

        private static int GetLivingNeighbors(bool[,] cells, int x, int y)
        {
            var count = 0;
            var X = cells.GetLength(0);
            var Y = cells.GetLength(1);

            if (x != X - 1)
                if (cells[x + 1, y])
                    count++;

            if (x != X - 1 && y != Y - 1)
                if (cells[x + 1, y + 1])
                    count++;

            if (y != Y - 1)
                if (cells[x, y + 1])
                    count++;

            if (x != 0 && y != Y - 1)
                if (cells[x - 1, y + 1])
                    count++;

            if (x != 0)
                if (cells[x - 1, y])
                    count++;

            if (x != 0 && y != 0)
                if (cells[x - 1, y - 1])
                    count++;

            if (y != 0)
                if (cells[x, y - 1])
                    count++;

            if (x != X - 1 && y != 0)
                if (cells[x + 1, y - 1])
                    count++;

            return count;
        }

        private void CheckCycle()
        {
            var curr = Tuple.Create<int, int>(SurvivingCells, Changes);
            if (StateTracker.Count == 0)
                StateTracker.Add(curr);

            var idx = StateTracker.IndexOf(curr);
            if (InCycle)
            {
                switch (idx)
                {
                    case -1:
                        InCycle = false;
                        IterationsInCycle = 0;
                        StateTracker.Add(curr);
                        break;
                    case 0:
                        LastRepeatedIdx = 0;
                        IterationsInCycle++;
                        break;
                    default:
                    {
                        if (idx == LastRepeatedIdx + 1)
                        {
                            LastRepeatedIdx++;
                        }
                        else
                        {
                            InCycle = false;
                            IterationsInCycle = 0;
                        }

                        break;
                    }
                }
            }
            else
            {
                switch (idx)
                {
                    case -1:
                        StateTracker.Add(curr);
                        break;
                    case 0:
                        InCycle = true;
                        IterationsInCycle = 1;
                        LastRepeatedIdx = 0;
                        break;
                    default:
                        InCycle = true;
                        IterationsInCycle = 1;
                        LastRepeatedIdx = 0;
                        StateTracker.RemoveRange(0, idx);
                        break;
                }
            }
            if(StateTracker.Count > 20)
                StateTracker.RemoveAt(0);
        }

        public int CompareTo(Board other)
        {
            if (GetCycleLength().CompareTo(other.GetCycleLength()) != 0)
                return GetCycleLength().CompareTo(other.GetCycleLength());
            else if (Changes.CompareTo(other.Changes) != 0)
                return Changes.CompareTo(other.Changes);
            else if (SurvivingCells.CompareTo(other.SurvivingCells) != 0)
                return SurvivingCells.CompareTo(other.SurvivingCells);
            else return SurvivedEpochs.CompareTo(other.SurvivedEpochs);
        }

        public Board Crossover(Board other, double mutationRate, Random rand)
        {
            var x = CurrState.GetLength(0);
            var y = CurrState.GetLength(1);
            var newBoard = CurrState.Clone() as bool[,];

            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    newBoard[i, j] = rand.NextDouble() < 0.5 ? CurrState[i, j] : other.CurrState[i, j];
                }
            }

            return new Board(newBoard).Mutate(mutationRate, rand);
        }

        private Board Mutate(double mutationRate, Random rand)
        {
            for (var i = 0; i < BaseState.GetLength(0); i++)
            {
                for (var j = 0; j < BaseState.GetLength(1); j++)
                {
                    if (rand.NextDouble() < mutationRate)
                        BaseState[i, j] = !BaseState[i, j];
                }
            }

            return this;
        }

        public void Reset()
        {
            CurrState = CurrState.Clone() as bool[,];
        }

        public int GetCycleLength()
        {
            return (InCycle && IterationsInCycle>2) ? StateTracker.Count : 0;
        }
    }
}