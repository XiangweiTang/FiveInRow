using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    class BoardLogic
    {
        #region BoardConsts
        const int X0 = 30;
        const int Y0 = 30;              //The coordinate of the left-top of the board is (30,30)
        const int BoardSize = 15;
        const int GridSize = 22;
        const int DeltaSize = 8;        //If the user click is in the [-8,8]*[-8,8] neighbor area, then it is a valid click.
        #endregion

        #region Board Variables
        public int XCoordinate { get; private set; }    //Coordinate means the position on board.
        public int YCoordinate { get; private set; }
        private int XMatrix = 0;
        private int YMatrix = 0;    //Matrix means the position in matrix/array

        private double delta = 0;
        private int XMin = 0;
        private int YMin = 0;
        private int XMax = 0;
        private int YMax = 0;
        private bool isWhite = false;

        private AI ai = new AI();
        #endregion

        public BoardLogic()
        {
            init();
        }
        
        /// <summary>
        /// Initiation
        /// </summary>
        private void init()
        {
            ///These are set before the game begin to increase the speed.            
            delta = (double)DeltaSize / GridSize;
            XMin = X0 - DeltaSize;
            YMin = Y0 - DeltaSize;
            XMax = X0 + (BoardSize - 1) * GridSize + DeltaSize;
            YMax = Y0 + (BoardSize - 1) * GridSize + DeltaSize;
            isWhite = false;               
        }

        /// <summary>
        /// To process user's click
        /// </summary>
        /// <param name="XClick">X coordinate of click</param>
        /// <param name="YClick">Y coordinate of click</param>
        /// <param name="isW">The current step color</param>
        /// <param name="isV">If the current step makes victory</param>
        /// <returns></returns>
        public bool SetUserStep(int XClick, int YClick, out bool isW, out bool isV)
        {
            ///First test if user's step is valid
            ///If so then get the corresponding matrix coordinate, and transform it to the board coordinate.            
            bool valid = isValid(XClick, YClick);
            isW = isWhite;
            if (valid)
            {
                XCoordinate =X0+ XMatrix * GridSize;
                YCoordinate =Y0+ YMatrix * GridSize;
                ai.setCurrentStep(XMatrix, YMatrix, isWhite);
                isWhite = !isWhite;
            }
            isV = ai.isVictory();
            return valid;
        }

        /// <summary>
        /// To process AI's click
        /// </summary>
        /// <param name="isW">The current step color</param>
        /// <param name="isV">If current step makes victory</param>
        /// <param name="startUp">If it is the first step</param>
        public void SetAIStep(out bool isW, out bool isV, bool startUp)
        {
            /// Get the AI step.
            /// Transform it into the board coordinate.
            Tuple<int, int> AIStep = ai.AIStep(isWhite, startUp);
            XCoordinate = X0+AIStep.Item1 * GridSize;
            YCoordinate =Y0+ AIStep.Item2 * GridSize;
            ai.setCurrentStep(AIStep.Item1, AIStep.Item2, isWhite);
            isW = isWhite;
            isWhite = !isWhite;
            isV = ai.isVictory();
        }

        /// <summary>
        /// Test if current click is valid
        /// </summary>
        /// <param name="XClick">X coordinate of click</param>
        /// <param name="YClick">Y coordinate of click</param>
        /// <returns>If valid then true</returns>
        private bool isValid(int XClick, int YClick)
        {
            /// If the click is in the board
            ///     If the click is close to a cross
            ///         If the click is on an unoccupied cross
            ///         Then return true
            /// Otherwise return false         
            bool xInBoard = isInBoard(XClick, XMin, XMax);
            bool yInBoard = isInBoard(YClick, YMin, YMax);
            bool xCloseToCross = isCloseToCross(XClick,X0, out XMatrix);
            bool yCloseToCross = isCloseToCross(YClick,Y0, out YMatrix);

            if (xInBoard && yInBoard)
            {
                if (xCloseToCross && yCloseToCross)
                {
                    if (ai.isUnoccupied(XMatrix, YMatrix))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Test if current click is in the chessboard area.
        /// </summary>
        /// <param name="click">Coordinate of click</param>
        /// <param name="min">The min of the chess board area</param>
        /// <param name="max">The max of the chess board area</param>
        /// <returns>True if in board</returns>
        private bool isInBoard(int click, int min, int max)
        {
            return (click >= min && click <= max);
        }

        /// <summary>
        /// Test if the current click is close to a cross
        /// </summary>
        /// <param name="click">Coordinate of click</param>
        /// <param name="start">Start point of this direction</param>
        /// <param name="matrix">The corresponding matrix coordinate</param>
        /// <returns></returns>
        private bool isCloseToCross(int click,int start, out int matrix)
        {
            /// This function scales the click coordinate into the proper size
            /// If it is close enough to an integer, then it is valid.
            double precisePosition = (double)(click-start) / GridSize;
            double approxPosition = Math.Round(precisePosition);
            double distance = Math.Abs(approxPosition - precisePosition);
            matrix = Convert.ToInt32(approxPosition);
            return distance <= delta;
        }
    }
}
