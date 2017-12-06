namespace FiveInRow
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ChessBoard = new System.Windows.Forms.PictureBox();
            this.Btn_NewGame = new System.Windows.Forms.Button();
            this.AIFirst = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChessBoard)).BeginInit();
            this.SuspendLayout();
            // 
            // ChessBoard
            // 
            this.ChessBoard.Image = ((System.Drawing.Image)(resources.GetObject("ChessBoard.Image")));
            this.ChessBoard.Location = new System.Drawing.Point(12, 11);
            this.ChessBoard.Name = "ChessBoard";
            this.ChessBoard.Size = new System.Drawing.Size(370, 370);
            this.ChessBoard.TabIndex = 0;
            this.ChessBoard.TabStop = false;
            this.ChessBoard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChessBoard_MouseClick);
            // 
            // Btn_NewGame
            // 
            this.Btn_NewGame.Location = new System.Drawing.Point(389, 12);
            this.Btn_NewGame.Name = "Btn_NewGame";
            this.Btn_NewGame.Size = new System.Drawing.Size(75, 21);
            this.Btn_NewGame.TabIndex = 1;
            this.Btn_NewGame.Text = "NewGame";
            this.Btn_NewGame.UseVisualStyleBackColor = true;
            this.Btn_NewGame.Click += new System.EventHandler(this.Btn_NewGame_Click);
            // 
            // AIFirst
            // 
            this.AIFirst.AutoSize = true;
            this.AIFirst.Location = new System.Drawing.Point(389, 40);
            this.AIFirst.Name = "AIFirst";
            this.AIFirst.Size = new System.Drawing.Size(72, 16);
            this.AIFirst.TabIndex = 2;
            this.AIFirst.Text = "AI first";
            this.AIFirst.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 392);
            this.Controls.Add(this.AIFirst);
            this.Controls.Add(this.Btn_NewGame);
            this.Controls.Add(this.ChessBoard);
            this.Name = "MainForm";
            this.Text = "Five In Row";
            ((System.ComponentModel.ISupportInitialize)(this.ChessBoard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ChessBoard;
        private System.Windows.Forms.Button Btn_NewGame;
        private System.Windows.Forms.CheckBox AIFirst;
    }
}

