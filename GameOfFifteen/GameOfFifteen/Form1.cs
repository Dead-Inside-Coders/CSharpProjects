﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfFifteen
{
    public partial class Form1 : Form
    {
        public delegate void MyDelegate( string where);

        private Button[] buttonsArray;
        private int[] numArray;
        //private int[] goalNumArray;
        private int steps, difficulty;
        private BoardState startState;
        AutoSolveClass Solver;
        private bool isManual;
        private int speed = 1000;

        public Form1()
        {
            steps = 0;
            difficulty = 25;
            startState = new BoardState();
            buttonsArray = new Button[16];
            numArray = new int[16];
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
            autoGameToolStripMenuItem.Enabled = true;
            label1.Text = steps.ToString();
            label1.ForeColor = Color.FromArgb(66, 115, 34);
            isManual = true;
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

            int[] actions = new int[] { 1, -1, 4, -4 };
            Random rd = new Random();

            for (i = 0; i < 16; i++)
                numArray[i] = i + 1;
            numArray[zeroIndex] = 0;
            for (i = 0; i < difficulty; i++)
            {
                do
                {
                    tempIndex = zeroIndex + actions[rd.Next(4)];
                    if (prevIndex != tempIndex && isCanMove(zeroIndex, tempIndex))
                    {
                        numArray[zeroIndex] = numArray[tempIndex];
                        numArray[tempIndex] = 0;
                        prevIndex = zeroIndex;
                        zeroIndex = tempIndex;
                        break;
                    }

                } while (true);
            }
        }


        // Button click Event
        private void ButtonClick(object sender, EventArgs e)
        {
            int i, tempIndex = 0;

            for (i = 0; i < 16; i++)
            {
                if ((Button)sender == buttonsArray[i])
                    tempIndex = i;
            }

            for (i = 0; i < 16; i++)
            {
                if (numArray[i] == 0)
                {
                    if (isCanMove(i, tempIndex))
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

            if (isGameOver())
            {
                label1.ForeColor = Color.FromArgb(54, 72, 36);
                setButtonActivity(false);
                autoGameToolStripMenuItem.Enabled = false;
                MessageBox.Show(String.Format("Congratulation, you won!\nNumber of Steps: {0}", steps), "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isManualActiveButton(false);
            }
        }


        // Create Ease New game 
        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 15;
            CreateGame();
            isManualActiveButton(false);
            //listView1.Items.Clear();
        }


        // Create Medium New game
        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 25;
            CreateGame();
            isManualActiveButton(false);
            //listView1.Items.Clear();
        }


        // Create Hard New game
        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficulty = 50;
            CreateGame();
            isManualActiveButton(false);
            // listView1.Items.Clear();
        }


        // Call Auto Solver IDA Star Algorithm
        private void iDAStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Task<String> callback = Task<String>.Factory.StartNew(() =>  Solver.GetSolution(startState) );
            string callback = Solver.GetSolution(startState);
            label1.ForeColor = Color.FromArgb(54, 72, 36);
            if (callback != "")
            {
                isManual = false;
                setButtonActivity(false);
                isManualActiveButton(true);
                Task.Factory.StartNew(() => { applaySolution(callback); }); 
            }
            else
                MessageBox.Show("Solution not found:(");
        }

        private void applaySolution(string solution)
        {
            
            while (solution.Length > 0 && !isManual)
            {
                int zeroIndex = findeEmptyPositonIndex(numArray);
                switch (solution[0])
                {
                    case 'U':
                        changeButtonPosition(zeroIndex, -4, "up");
                        solution = solution.Remove(0, 1);
                        break;
                    case 'D':
                        changeButtonPosition(zeroIndex, 4, "down");
                        solution = solution.Remove(0, 1);
                        break;
                    case 'R':
                        changeButtonPosition(zeroIndex, 1, "right");
                        solution = solution.Remove(0, 1);
                        break;
                    case 'L':
                        changeButtonPosition(zeroIndex, -1, "left");
                        solution = solution.Remove(0, 1);
                        break;
                }
                steps++;
                label1.Invoke(new MethodInvoker(() =>
                {
                    label1.Text = steps.ToString();
                }));
                

                if (isGameOver())
                {
                    setButtonActivity(false);
                    newGameToolStripMenuItem.Enabled = true;
                    isManual = true;
                    MessageBox.Show(String.Format("Congratulation, you won!\nNumber of Steps: {0}", steps), "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private int findeEmptyPositonIndex(int[] numArray)
        {
            for (int i = 0; i < numArray.Length; i++)
            {
                if(numArray[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        private void changeButtonPosition(int index, int newIndex, string where)
        {
            Thread.Sleep(speed);
            //listView1.Items.Add(buttonsArray[index + newIndex].Text + "--->" + where);
            buttonsArray[index].Invoke(new MethodInvoker(() => {
                buttonsArray[index].Text = buttonsArray[index + newIndex].Text;
                buttonsArray[index + newIndex].Text = "";
                startState.boardNumArray[index] = numArray[index + newIndex];
                startState.boardNumArray[index + newIndex] = 0;
                numArray[index] = numArray[index + newIndex];
                numArray[index + newIndex] = 0;
            }));
            
        }

        // Application info
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Game of Fifteen", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Menu Exit clidk
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        //Check is next move vald
        private bool isCanMove(int zeroIndex, int curr)
        {
            if (curr < 0 || curr > 15)
                return false;
            if (Math.Abs(zeroIndex - curr) == 1 || Math.Abs(zeroIndex - curr) == 4)
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

        private void button1_Click(object sender, EventArgs e)
        {
            isManual = true;
            isManualActiveButton(false);
            setButtonActivity(true);
        }

        private void isManualActiveButton(bool value)
        {       
            button1.Visible = value;
            speedUp.Visible = value;
            speedDown.Visible = value;
        }


        //Check is Game over
        private bool isGameOver()
        {
            for (int i = 0; i < 15; i++)
                if (numArray[i] != i + 1)
                    return false;
            return true;
        }

        private void speedUp_Click(object sender, EventArgs e)
        {
            speed -= 200;
        }

        private void speedDown_Click(object sender, EventArgs e)
        {
            speed += 200;
        }

        private void setButtonActivity(bool value)
        {
            for (int i = 0; i < 16; i++)
                buttonsArray[i].Enabled = value;
        }
    }
}


