using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    class AI
    {
        #region Consts

        /// <summary>
        /// Const part, the discount and the weight for different lengths can be adjusted by the real games.
        /// </summary>
        const int BoardSize = 15;
        const int VictorySize = 5;
        const double opposeDiscount = 0.9;
        const int FIVE_coef = 1000;
        const int FOUR_coef = 500;
        const int THREE_coef = 100;
        const int TWO_coef = 20;
        const int ONE_coef = 1;
        #endregion

        #region Variables

        /// <summary>
        /// Variable part.
        /// </summary>
        private int[,] boardMatrix = new int[BoardSize, BoardSize];
        private bool isWhite = false;
        private List<Tuple<int, int>> lengthList = new List<Tuple<int, int>>();

        #endregion
        public AI()
        {
            init();
        }

        /// <summary>
        /// Initiate
        /// </summary>
        private void init()
        {
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    boardMatrix[i, j] = 0;
            lengthList.Clear();
        }

        /// <summary>
        /// Test if current status makes a victory
        /// </summary>
        /// <returns>True if victory</returns>
        public bool isVictory()
        {
            ///If any of the groups contains a length-5 , then victory.
            int targetValue = isWhite ? -1 : 1;
            evalBoard(targetValue);
            return lengthList.Any(x => x.Item2 >= VictorySize);
        }

        /// <summary>
        /// Test if current cross is occupied.
        /// </summary>
        /// <param name="X">The X coordinate</param>
        /// <param name="Y">The Y coordinate</param>
        /// <returns>True if the cross is unoccupied</returns>
        public bool isUnoccupied(int X, int Y)
        {
            return boardMatrix[X, Y] == 0;
        }

        /// <summary>
        /// Set the current step
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="isW">The current step color</param>
        public void setCurrentStep(int x, int y, bool isW)
        {
            isWhite = isW;
            boardMatrix[x, y] = isWhite ? -1 : 1;
        }

        /// <summary>
        /// AI step
        /// </summary>
        /// <param name="isW">The current step color</param>
        /// <param name="startUp">If it is the first step of the game</param>
        /// <returns>The coordinate pair</returns>
        public Tuple<int,int> AIStep(bool isW, bool startUp)
        {
            /// If it is the first step then place it in the middle.
            /// Otherwise use stratagy.
            isWhite = isW;
            if (startUp)
                return new Tuple<int, int>(BoardSize / 2, BoardSize / 2);
            return virtualStepAIStep();
        }

        /// <summary>
        /// A dummy step, valid but no stratagy steps. Only for test.
        /// </summary>
        /// <returns>The coordinate pair</returns>
        private Tuple<int,int> dummyAIStep()
        {
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    if (isUnoccupied(i, j))
                        return new Tuple<int, int>(i, j);
            return new Tuple<int, int>(0, 0);
        }        

        /// <summary>
        /// The algorithm by setting a virtual step and evaluate which one is the best choice.
        /// </summary>
        /// <returns>Coordinate pair</returns>
        private Tuple<int,int> virtualStepAIStep()
        {
            /// If one of the cross makes victory, then make it.
            /// If one of the cross blocks the enemy victory, then make it.
            /// Otherwise return the most "important" cross.
            int ownValue = isWhite ? -1 : 1;
            Dictionary<Tuple<int, int>, StatusEval> ownDict = buildVirtualEvalDict(ownValue);
            Dictionary<Tuple<int, int>, StatusEval> opposeDict = buildVirtualEvalDict(-ownValue);
            if (ownDict.Any(x => x.Value.Five != 0))
                return ownDict.First(x => x.Value.Five != 0).Key;
            if (opposeDict.Any(x => x.Value.Five != 0))
                return opposeDict.First(x => x.Value.Five != 0).Key;
            Tuple<int, int> t = new Tuple<int, int>(0, 0);
            double maxStr = 0;
            foreach(var item in ownDict)
            {
                double str = item.Value.strength + opposeDict[item.Key].strength * opposeDiscount;
                if(str>maxStr)
                {
                    maxStr = str;
                    t = item.Key;
                }
            }
            return t;
        }

        /// <summary>
        /// The procedure to build the evaluation dictionary, used by the virtual step function.
        /// </summary>
        /// <param name="targetValue">The current color</param>
        /// <returns>The dictionary, with key is the coord pair, value is the status class.</returns>
        private Dictionary<Tuple<int, int>, StatusEval> buildVirtualEvalDict(int targetValue)
        {
            /// For each of the crosses on board
            ///     If it is unoccupied
            ///         Set a virtual step on it.
            ///         Eval the current status and add the status into the dict
            ///         Remove the virtual step.
            Dictionary<Tuple<int, int>, StatusEval> dict = new Dictionary<Tuple<int, int>, StatusEval>();
            for (int i=0;i<BoardSize;i++)
                for(int j = 0; j < BoardSize; j++)
                {
                    if (boardMatrix[i, j] == 0)
                    {
                        boardMatrix[i, j] = targetValue;
                        evalBoard(targetValue);
                        StatusEval le = new StatusEval(lengthList, targetValue);
                        dict.Add(new Tuple<int, int>(i, j), le);
                        boardMatrix[i, j] = 0;
                    }
                }
            return dict;
        }        

        #region Evaluation

        /// <summary>
        /// Evaluate the whole chess board. 
        /// </summary>
        /// <param name="targetValue">The color needs to be evaluated.</param>
        private void evalBoard(int targetValue)
        {
            /// For each of the crosses on board
            /// Evaluate the four directions.
            lengthList.Clear();
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    evalCross(i, j, 1, 0, targetValue);     //Evaluate the "-" direction
                    evalCross(i, j, 1, 1, targetValue);     //Evaluate the "\" direction
                    evalCross(i, j, 0, 1, targetValue);     //Evaluate the "|" direction
                    evalCross(i, j, -1, 1, targetValue);    //Evaluate the "/" direction
                }
            }
        }

        /// <summary>
        /// Evaluate a single cross of a certain direction
        /// </summary>
        /// <param name="xStart">X Coordinate of the start point</param>
        /// <param name="yStart">Y Coordinate of the start point</param>
        /// <param name="xVector">X of the vector</param>
        /// <param name="yVector">Y of the vector</param>
        /// <param name="targetValue">The color of current step</param>
        private void evalCross(int xStart, int yStart, int xVector, int yVector, int targetValue)
        {
            /// If the five crosses in the direction is in the board
            /// Add the five crosses in the list
            /// Evaluate the list.
            int xEnd = xStart + (VictorySize - 1) * xVector;
            int yEnd = yStart + (VictorySize - 1) * yVector;
            if (isInBoard(xEnd) && isInBoard(yEnd))
            {
                List<int> list = new List<int>();
                for (int step = 0; step < VictorySize; step++)
                {
                    list.Add(boardMatrix[xStart + xVector * step, yStart + yVector * step]);
                }
                evalDirection(list, targetValue);
            }
        }

        /// <summary>
        /// Evaluate a certain direction
        /// </summary>
        /// <param name="list">The color infomation in the list</param>
        /// <param name="targetValue">The color we are processing</param>
        private void evalDirection(List<int> list, int targetValue)
        {
            /// If the list is not completely unoccupied
            ///     If the list does not contain the other color
            ///         Put the length, color in the list.
            if (list.Any(x => x == targetValue))
            {
                if (!list.Any(x => x == -targetValue))
                {
                    lengthList.Add(new Tuple<int, int>(targetValue, Math.Abs(list.Sum())));
                }
            }
        }

        /// <summary>
        /// Test if the number is in the range of the array
        /// </summary>
        /// <param name="i">The number to be tested</param>
        /// <returns>True if it is in the board.</returns>
        private bool isInBoard(int i)
        {
            return (i >= 0 && i < BoardSize);
        }
        #endregion

        /// <summary>
        /// This class is to store the status infomation.
        /// </summary>
        class StatusEval
        {
            /// <summary>
            /// Test if there is length-3, length-4, length-5 groups in the current status.
            /// It is useless in the virtual step, but maybe useful in some other algorithms.
            /// </summary>
            public int Three { get; private set; }
            public int Four { get; private set; }
            public int Five { get; private set; }
            public int strength { get; private set; }

            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            int targetValue;
            public StatusEval(List<Tuple<int, int>> sList, int value)
            {
                list = sList;
                targetValue = value;
                setLength();
                setStrength();
            }

            /// <summary>
            /// Set the "Three/Four/Five"
            /// </summary>
            private void setLength()
            {
                Three = setSpecificLength(3);
                Four = setSpecificLength(4);
                Five = setSpecificLength(5);
            }

            /// <summary>
            /// Count a certain length. Used by the setLength.
            /// </summary>
            /// <param name="length">The length need to be counted</param>
            /// <returns></returns>
            private int setSpecificLength(int length)
            {
                return (from l in list
                        where l.Item1 == targetValue && l.Item2 == length
                        select l).Count();
            }

            /// <summary>
            /// Set the strngth of current status.
            /// </summary>
            private void setStrength()
            {
                foreach (var t in list)
                {
                    if (t.Item1 == targetValue)
                    {
                        switch (t.Item2)
                        {
                            case 1:
                                strength += ONE_coef;
                                break;
                            case 2:
                                strength += TWO_coef;
                                break;
                            case 3:
                                strength += THREE_coef;
                                break;
                            case 4:
                                strength += FOUR_coef;
                                break;
                            case 5:
                                strength += FIVE_coef;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
