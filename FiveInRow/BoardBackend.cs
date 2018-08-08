using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    class BoardBackend
    {
        const int BoardSize = 15;
        const int VictoryLength = 5;
        bool?[,] Board = new bool?[BoardSize, BoardSize];
        public bool IsWhite { get; private set; } = false;
        public bool? Victory { get; private set; } = null;

        public bool SetStep(int i, int j)
        {
            if (ValidStep(i, j))
            {
                Board[i, j] = IsWhite;
                IsWhite = !IsWhite;
                EvalVictory();
                return true;
            }
            return false;
        }

        private void EvalVictory()
        {
            foreach (int i in EvalBoard())
            {
                if (i == 5)
                {
                    Victory = true;
                    return;
                }
                if (i == -5)
                {
                    Victory = false;
                    return;
                }
            }
        }

        private IEnumerable<int> EvalBoard()
        {
            for(int i = 0; i < BoardSize; i++)
            {
                for(int j = 0; j < BoardSize; j++)
                {
                    yield return EvalInterval(i, j, 1, 0);
                    yield return EvalInterval(i, j, 1, 1);
                    yield return EvalInterval(i, j, 0, 1);
                    yield return EvalInterval(i, j, -1, 1);
                }
            }
        }

        private int EvalInterval(int xStart, int yStart, int xVector, int yVector)
        {
            return EvalInterval(xStart, yStart, xVector, yVector, VictoryLength);
        }
        
        private int EvalInterval(int xStart, int yStart, int xVector, int yVector, int length)
        {
            List<bool> list = new List<bool>();
            for (int i = 0; i < length; i++)
            {
                int currentX = xStart + (length - 1) * xVector;
                int currentY = yStart + (length - 1) * yVector;
                if (InBoard(currentX) && InBoard(currentY))
                {
                    var current = Board[currentX, currentY];
                    if (current != null)
                        list.Add(Convert.ToBoolean(current));
                }
                else
                    return 0;
            }
            if (list.All(x => x))
                return list.Count;
            if (list.All(x => !x))
                return -list.Count;
            return 0;
        }

        private bool ValidStep(int x, int y)
        {
            if (!InBoard(x) || !InBoard(y))
                return false;
            return Board[x, y] == null;
        }

        private bool InBoard(int index)
        {
            return index >= 0 && index < BoardSize;
        }
    }
}
