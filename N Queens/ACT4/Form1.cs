using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace ACT4
{
    public partial class Form1 : Form
    {
        int side;
        int n = 6;
        SixState startState;
        SixState[] currentStates;
        int moveCounter;
        int countStates;
        int bestState;

        //bool stepMove = true;

        int[,,] hTable;
        ArrayList[] bestMoves;
        Object[] chosenMove;

        public Form1()
        {
            InitializeComponent();

            side = pictureBox1.Width / n;

            startState = randomSixState();

            countStates = 3;
            bestState = 0;
            currentStates = new SixState[numStates];
            chosenMove = new object[numStates];

            currentStates[0] = new SixState(startState);
            currentStates[1] = randomSixState();
            currentStates[2] = randomSixState();

            updateUI();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }

        private void updateUI()
        {
            //pictureBox1.Refresh();
            pictureBox2.Refresh();

            //label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
            label3.Text = "Attacking pairs: " + getAttackingPairs(currentStates[bestState]);
            label4.Text = "Moves: " + moveCounter;
            hTable = getHeuristicTableForPossibleMoves(currentState);
            bMoves = getBestMoves(hTable);

            listBox1.Items.Clear();

            for (int i = 0; i < countStates; i++)
                if (bestMoves[i].Count > 0)
                    chosenMove[i] = chooseMove(bMoves[i]);

            foreach (Point move in bestMoves[bestState])
            {
                listBox1.Items.Add(move);
            }

            label2.Text = "Chosen move: " + chosenMove[bestState];
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // draw squares
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Blue, i * side, j * side, side, side);
                    }
                    // draw queens
                    if (j == startState.Y[i])
                        e.Graphics.FillEllipse(Brushes.Fuchsia, i * side, j * side, side, side);
                }
            }
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            // draw squares
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Black, i * side, j * side, side, side);
                    }
                    // draw queens
                    if (j == currentState[bestState].Y[i])
                        e.Graphics.FillEllipse(Brushes.Fuchsia, i * side, j * side, side, side);
                }
            }
        }

        private SixState randomSixState()
        {
            Random r = new Random();
            SixState random = new SixState(r.Next(n),
                                             r.Next(n),
                                             r.Next(n),
                                             r.Next(n),
                                             r.Next(n),
                                             r.Next(n));

            return random;
        }

        private int getAttackingPairs(SixState f)
        {
            int attackers = 0;
            
            for (int rf = 0; rf < n; rf++)
            {
                for (int tar = rf+1; tar < n; tar++)
                {
                    // get horizontal attackers
                    if (f.Y[rf] == f.Y[tar])
                        attackers++;
                }
                for (int tar = rf+1; tar < n; tar++)
                {
                    // get diagonal down attackers
                    if (f.Y[tar] == f.Y[rf] + tar - rf)
                        attackers++;
                }
                for (int tar = rf+1; tar < n; tar++)
                {
                    // get diagonal up attackers
                    if (f.Y[rf] == f.Y[tar] + tar - rf)
                        attackers++;
                }
            }
            
            return attackers;
        }

        private int[,,] getHeuristicTableForPossibleMoves(SixState[] thisState)
        {
            int[,,] hStates = new int[countStates, n, n];

            for (int i = 0; i < countStates; i++) // go through the indices
            {
                for (int j = 0; j < n; j++) // replace them with a new value
                {
                    for(int l =0; l < n; l++)
                    SixState possible = new SixState(thisState[i]);
                    possible.Y[j] = l;
                    hStates[i, j, l] = getAttackingPairs(possible);
                }
            }

            return hStates;
        }

        private ArrayList[] getBestMoves(int[,,] heuristicTable)
        {
            ArrayList[] bestMoves = new ArrayList[countStates];
            for (int i = 0; i < numStates; i++)
                bestMoves[i] = new ArrayList();

            int[] bestHeuristicValue = new int[countStates];

            for (int i = 0; i < countStates; i++)
            {
                bestHeuristicValues[i] = heuristicTable[i, 0, 0];
                for (int j = 0; j < n; j++)
                {
                    for (int l = 0; l < n; l++)
                    {
                        if (bestHeuristicValue[i] > heuristicTable[i, j, l])
                        {
                            bestHeuristicValue[i] = heuristicTable[i, j, l];
                            bestMoves[i].Clear();
                            if (currentStates[i].Y[i] != j)
                                bestMoves[i].Add(new Point(j, l));
                        }
                        else if (bestHeuristicValue[i] == heuristicTable[i, j, l])
                        {
                            if (currentStates[i].Y[j] != l)
                                bestMoves[i].Add(new Point(j, l));
                        }
                    }
                }
            }

            for(int i = 0;i < countStates; i++)
                if (bestHeuristicValue[bestState] > bestHeuristicValue[i])
                    bestState = i;

            label5.Text = "Possible Moves (H=" + bestHeuristicValue[bestState] + ")";
            return bestMoves;
        }

        private Object chooseMove(ArrayList possibleMoves)
        {
            int arrayLength = possibleMoves.Count;
            Random r = new Random();
            int randomMove = r.Next(arrayLength);

            return possibleMoves[randomMove];
        }

        private void executeMove(Point move)
        {
            for (int i = 0; i < n; i++)
            {
                startState.Y[i] = currentStates[bestState].Y[i];
            }
            currentStates[bestState].Y[move.X] = move.Y;
            moveCounter++;

            for (int i = 0; i < numStates; i++)
                chosenMove[i] = null

            updateUI();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (getAttackingPairs(currentStates[bestState]) > 0)
                executeMove((Point)chosenMove[bestState]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentStates[0] = startState = randomSixState();
            for (int i = 1; i < numStates; i++)
                currentState[i] = new SixState();

            moveCounter = 0;

            updateUI();
            pictureBox1.Refresh();
            label1.Text = "Attacking pairs: " + getAttackingPairs(currentStates[bestState]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (getAttackingPairs(currentStates[bestState]) > 0)
            {
                for (int i = 0; i < countStates; i++)
                    executeMove((Point)chosenMove[i]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }        
    }
}
