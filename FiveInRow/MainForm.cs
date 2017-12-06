using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace FiveInRow
{
    public partial class MainForm : Form
    {
        BoardLogic bLogic = new BoardLogic();
        private Image whiteChess;
        private Image blackChess;
        private List<PictureBox> pictureList = new List<PictureBox>();
        private bool isWhite = false;
        private bool userTurn = true;
        private bool isVictory = false;
        
        public MainForm()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            ChessBoard.Enabled = true;
            initChessImages();
            initPictureBoxes();
            bLogic = new BoardLogic();
            initStep();
        }
        private void initChessImages()
        {
            System.Reflection.Assembly blackAssemb = System.Reflection.Assembly.GetExecutingAssembly();
            Stream blackStream = blackAssemb.GetManifestResourceStream("FiveInRow.Resources.black_20.png");
            blackChess = Image.FromStream(blackStream);
            System.Reflection.Assembly whiteAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream whiteStream = whiteAssembly.GetManifestResourceStream("FiveInRow.Resources.white_20.png");
            whiteChess = Image.FromStream(whiteStream);
        }
        private void initPictureBoxes()
        {
            foreach (PictureBox pb in pictureList)
                pb.Dispose();
            pictureList = new List<PictureBox>();
        }
        private void initStep()
        {
            if (!userTurn)
            {
                bLogic.SetAIStep(out isWhite, out isVictory, true);
                drawChess(bLogic.XCoordinate, bLogic.YCoordinate, isWhite);
                userTurn = true;
            }
        }
        private void ChessBoard_MouseClick(object sender, MouseEventArgs e)
        {
            if (userTurn)
                setUserAIStep(e.X, e.Y);
        }
        
        /// <summary>
        /// The main procedure: User turn->Victory?Win:AI turn->Victory?Win:Waiting for user turn
        /// </summary>
        /// <param name="x">the x coordinate of mouse click</param>
        /// <param name="y">the y cooridnate of mouse click</param>
        private void setUserAIStep(int x, int y)
        {
            bool isValid = bLogic.SetUserStep(x, y, out isWhite, out isVictory);            
            /// If current user step is valid
            ///     then draw the chess
            ///     If current user step makes victory
            ///         User win
            ///     Otherwise call AI step
            ///     Draw the chess
            ///     If current AI step makes victory
            ///         AI win
            ///     Wait user's step.
            ///
            if (isValid)
            {
                drawChess(bLogic.XCoordinate, bLogic.YCoordinate, isWhite);
                userTurn = false;
                if (isVictory)
                {
                    MessageBox.Show("User win!");
                    ChessBoard.Enabled = false;
                }
                else
                {
                    bLogic.SetAIStep(out isWhite,out isVictory,false);
                    drawChess(bLogic.XCoordinate, bLogic.YCoordinate, isWhite);
                    if(isVictory)
                    {
                        MessageBox.Show("AI win!");
                        ChessBoard.Enabled = false;
                    }
                    userTurn = true;
                }
            }
        }
        private void Btn_NewGame_Click(object sender, EventArgs e)
        {
            userTurn = !AIFirst.Checked;
            init();
        }        
        
        /// <summary>
        /// The procedure to draw chess.
        /// </summary>
        /// <param name="xCoord">the x coordinate of the chess</param>
        /// <param name="yCoord">the y coordinate of the chess</param>
        /// <param name="isWhite">chess color</param>
        private void drawChess(int xCoord, int yCoord, bool isWhite)
        {            
            CirclePic p = new CirclePic();            
            p.Size = new Size(19, 19);
            p.Left = xCoord + 2;
            p.Top = yCoord + 2;            
            p.Image = isWhite ? whiteChess : blackChess;
            pictureList.Add(p);
            Controls.Add(pictureList.Last());
            pictureList.Last().BringToFront();
        }

        /// <summary>
        /// The class to draw a circle insteand of a rectangle.
        /// </summary>
        private class CirclePic : PictureBox
        {
            protected override void OnCreateControl()
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(this.ClientRectangle);
                Region r = new Region(gp);
                this.Region = r;
                gp.Dispose();
                r.Dispose();
                base.OnCreateControl();
            }
        }
    }

}
