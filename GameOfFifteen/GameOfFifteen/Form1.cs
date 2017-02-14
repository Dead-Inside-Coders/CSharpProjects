using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfFifteen
{
    public partial class Form1 : Form
    {
        private Button[] buttonsArray;
        private int[] numArray;
        private int[] goalNumArray;
        private int steps, mapIndex, difficulty;
        private string map;
        private bool isOnAutoGame;
        private BoardState startState;
        private Timer timer;
        AutoSolveClass Solver;

        public Form1()
        {
            steps = 0;
            mapIndex = 0;
            difficulty = 25;
            map = "";
            isOnAutoGame = false;
            startState = new BoardState();
            buttonsArray = new Button[16];
            numArray = new int[16];
            goalNumArray = new int[16];

            timer = new Timer();
            timer.Interval = 500;
            timer.Enabled = false;
            timer.Tick += TimerTick;

            Solver = new AutoSolveClass();



            InitializeComponent();

            InitializeButtons();
            CreateGame();

        }


        private void InitializeButtons()
        {
            int i;
            for (i = 0; i < 16; i++)
            {
                buttonsArray[i] = new Button();
                buttonsArray[i].Size = new Size(90, 90);
                buttonsArray[i].Location = new Point(95 * (i % 4) + 5, 95 * (i / 4) + 75);
                buttonsArray[i].BackColor = Color.FromArgb(163, 193, 133);
                buttonsArray[i].ForeColor = Color.FromArgb(66, 115, 34);
                buttonsArray[i].FlatStyle = FlatStyle.Flat;
                buttonsArray[i].Font = new Font("Impact", 18F);
                buttonsArray[i].Click += ButtonClick;
                this.Controls.Add(buttonsArray[i]);
            }
        }
        

        // Create New Game function
        private void CreateGame()
        {
            int i;
            steps = 0;
            mapIndex = 0;
            map = "";
            isOnAutoGame = false;
            autoGameToolStripMenuItem.Enabled = true;
            aStarToolStripMenuItem.Enabled = true;
            label1.Text = steps.ToString();
            label1.ForeColor = Color.FromArgb(66, 115, 34);

            CreateBoard();

            for (i = 0; i < 16; i++)
            {
                buttonsArray[i].Enabled = true;
                startState.boardNumArray[i] = numArray[i];
                if (numArray[i] != 0)
                    buttonsArray[i].Text = Convert.ToString(numArray[i]);
                else
                    buttonsArray[i].Text = "";
           }
        }


        // Create start board function
        private void CreateBoard()
        {
            int i, zeroIndex = 15, tempIndex = 0, prevIndex = 0;
            bool flag = false;
            int[] actions = new int[] { 1, -1, 4, -4 };
            Random rd = new Random();
            
            for(i = 0; i<16; i++)
                numArray[i] = i + 1;
            numArray[zeroIndex] = 0;
            for(i = 0; i<difficulty;i++)
            {
                do
                {
                    tempIndex = zeroIndex + actions[rd.Next(4)];
                    if (prevIndex!=tempIndex&&isCanMove(zeroIndex, tempIndex))
                    {
                        numArray[zeroIndex] = numArray[tempIndex];
                        numArray[tempIndex] = 0;
                        prevIndex = zeroIndex;
                        zeroIndex = tempIndex;
                        flag = true;
                    }
                    else
                        flag = false;
                } while (!flag);
            }
        }


        // Button click Event
        private void ButtonClick(object sender, EventArgs e)
        {
            int i, tempIndex = 0;

            for(i=0;i<16;i++)
            {
                if ((Button)sender == buttonsArray[i])
                    tempIndex = i;
            }

            for(i = 0; i<16; i++)
            {
                if(numArray[i] == 0)
                {
                    if(isCanMove(i,tempIndex))
                    {
                        buttonsArray[i].Text = buttonsArray[tempIndex].Text;
                        buttonsArray[tempIndex].Text = "";
                        startState.boardNumArray[i] = numArray[tempIndex];
                        startState.boardNumArray[tempIndex] = 0;
                        numArray[i] = numArray[tempIndex];
                        numArray[tempIndex] = 0;
                        steps++;
                        label1.Text = steps.ToString();
                    }
                }
            }

            if(isGameOver())
            {
                label1.ForeColor = Color.FromArgb(54, 72, 36);
                for (i = 0; i < 16; i++)
                    buttonsArray[i].Enabled = false;
                autoGameToolStripMenuItem.Enabled = false;
                MessageBox.Show(String.Format("Congratulation, you won!\nNumber of Steps: {0}", steps), "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // Create Ease New game 
        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 15;
            CreateGame();
        }


        // Create Medium New game
        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 25;
            CreateGame();
        }


        // Create Hard New game
        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 50;
            CreateGame();
            aStarToolStripMenuItem.Enabled = false;
        }


        // Call Auto Solver IDA Star Algorithm
        private void iDAStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map = Solver.GetPathIDAStar(startState);
            label1.ForeColor = Color.FromArgb(54, 72, 36);
            if (map != "")
            {
                for (int i = 0; i < 16; i++)
                    buttonsArray[i].Enabled = false;
                isOnAutoGame = true;
                timer.Enabled = true;
                newGameToolStripMenuItem.Enabled = false;
                autoGameToolStripMenuItem.Enabled = false;
            }
            else
                MessageBox.Show("Solution not found:(");
        }
        

        // Call Auto Solver A Star algorithm
        private void aStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map = Solver.GetPathAStar(startState);
            label1.ForeColor = Color.FromArgb(54, 72, 36);
            if (map != "")
            {
                for (int i = 0; i < 16; i++)
                    buttonsArray[i].Enabled = false;
                isOnAutoGame = true;
                timer.Enabled = true;
                newGameToolStripMenuItem.Enabled = false;
                autoGameToolStripMenuItem.Enabled = false;
            }
            else
                MessageBox.Show("Solution not found:(");
        }


        // Solver Animation per tick
        private void TimerTick(object sender, EventArgs e)
        {
            int i, zeroIndex = 0;
            for (i = 0; i < 16; i++)
                if (numArray[i] == 0)
                {
                    zeroIndex = i;
                    break;
                }
            if (isOnAutoGame)
            {
                switch (map[mapIndex])
                {
                    case 'U':
                        buttonsArray[zeroIndex].Text = buttonsArray[zeroIndex - 4].Text;
                        buttonsArray[zeroIndex - 4].Text = "";
                        numArray[zeroIndex] = numArray[zeroIndex - 4];
                        numArray[zeroIndex - 4] = 0;
                        break;
                    case 'D':
                        buttonsArray[zeroIndex].Text = buttonsArray[zeroIndex + 4].Text;
                        buttonsArray[zeroIndex + 4].Text = "";
                        numArray[zeroIndex] = numArray[zeroIndex + 4];
                        numArray[zeroIndex + 4] = 0;
                        break;
                    case 'R':
                        buttonsArray[zeroIndex].Text = buttonsArray[zeroIndex + 1].Text;
                        buttonsArray[zeroIndex + 1].Text = "";
                        numArray[zeroIndex] = numArray[zeroIndex + 1];
                        numArray[zeroIndex + 1] = 0;
                        break;
                    case 'L':
                        buttonsArray[zeroIndex].Text = buttonsArray[zeroIndex - 1].Text;
                        buttonsArray[zeroIndex - 1].Text = "";
                        numArray[zeroIndex] = numArray[zeroIndex - 1];
                        numArray[zeroIndex - 1] = 0;
                        break;
                }
                mapIndex++;
                steps++;
                label1.Text = steps.ToString();
            }
            if (isGameOver())
            {
                for (i = 0; i < 16; i++)
                    buttonsArray[i].Enabled = false;
                timer.Enabled = false;
                newGameToolStripMenuItem.Enabled = true;
                MessageBox.Show(String.Format("Congratulation, you won!\nNumber of Steps: {0}", steps), "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // Application info
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Game of Fifteen\nCreated by Krasochenko Yegor\n2017", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Menu Exit clidk
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        //Check is next move vald
        private bool isCanMove(int zeroIndex, int curr)
        {
            if(curr<0||curr>15)
                return false;
            if(Math.Abs(zeroIndex- curr) == 1||Math.Abs(zeroIndex - curr)==4)
            {
                if (zeroIndex - curr == 1 && zeroIndex % 4 == 0)
                    return false;
                else if (zeroIndex - curr == -1 && zeroIndex % 4 == 3)
                    return false;
                else
                    return true;
            }
            return false;
        }


        //Check is Game over
        private bool isGameOver()
        {
            for (int i = 0; i < 15; i++)
                if (numArray[i] != i + 1)
                    return false;
            return true;
        }


        
    }
}


