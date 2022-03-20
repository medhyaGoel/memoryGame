using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGameWindowsFormApp
{
    public partial class Form1 : Form
    {

        public Boolean _isRunning = false;
        public Boolean _isStarted = false;
        public double _time;
        public int[,] answerKey;
        public int matchCounter = 0;
        public Boolean timerRunning = false;
        //stores array of whether cards are open or not (0 = closed);
        public Boolean[,] openCards = new Boolean[4, 5];
        //stores cards & id
        IDictionary<int, Image> pictureBook = new Dictionary<int, Image>(){
            {1, Properties.Resources.one},
            {2, Properties.Resources.two},
            {3, Properties.Resources.three },
            {4, Properties.Resources.four },
            {5, Properties.Resources.five },
            {6, Properties.Resources.six },
            {7, Properties.Resources.seven },
            {8, Properties.Resources.eight},
            {9, Properties.Resources.nine },
            {10, Properties.Resources.ten },
        };
        public int firstRow = -1;
        public int secondRow = -1;
        public int firstCol = -1;
        public int secondCol = -1;

        public Form1()
        {
            InitializeComponent();
        }

        public void winner()
        {
            timerGame.Stop();
            //update best winner
            //fetch current best time from registry
        	RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MemoryGameApp", true);
            if (key != null)
            {
                double bestTime = double.Parse(key.GetValue("BestTime").ToString());
                //if no best time or beat best time
                if (bestTime > _time || bestTime == -1)
                {
                    //update label and set bestTime variable accordingly
                    highScoreLabel.Text = "Best score: " + string.Format("{0:0.00}", _time);
                    key.SetValue("BestTime", _time);
                }
                else
                {
                    //update label
                    highScoreLabel.Text = "Best score: "+ string.Format("{0:0.00}", bestTime);
                }
            }
            key.Close();
        }

        public void flip(PictureBox pb, String side, int row, int col)
        {
            if (_isStarted)
            {
                //change image
                //iterate through widgets
                foreach (Control c in this.Controls)
                {
                    // find the corresponding picturebox
                    if (c is PictureBox && c.Tag.Equals(row.ToString() + col.ToString()))
                    {
                        if (side.Equals("open")) //open
                        {
                            ((PictureBox)c).Image = pictureBook[answerKey[row, col]];
                            openCards[row, col] = true;
                        }
                        else //close
                        {
                            ((PictureBox)c).Image = Properties.Resources.closed;
                            openCards[row, col] = false;
                        }
                    }
                }
            }
        }

        public void cardClicked(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            String tag = pb.Tag.ToString();
            int row = int.Parse(tag.Substring(0, 1));
            int col = int.Parse(tag.Substring(1));
            //if both cards have a value, reset and flip over the first card
            if ((firstRow != -1 && secondRow != -1))
            {
                flip(pb, "close", firstRow, firstCol);
                flip(pb, "close", secondRow, secondCol);
                firstCol = -1;
                firstRow = -1;
                secondCol = -1;
                secondRow = -1;
                //open first card
                firstRow = row;
                firstCol = col;
                flip(pb, "open", firstRow, firstCol);
            }
            //else if neither card has a value
            else if ((firstCol == -1 && firstRow == -1))
            {
                //open first card
                firstRow = row;
                firstCol = col;
                flip(pb, "open", firstRow, firstCol);
            }
            //otherwise if first card is opened
            else
            {
                secondCol = col;
                secondRow = row;
                flip(pb, "open", secondRow, secondCol);
                if (answerKey[firstRow, firstCol] == answerKey[secondRow, secondCol])
                {
                    matchCounter++;
                    matchesLabel.Text = "Matches: " + matchCounter;
                    firstCol = -1;
                    firstRow = -1;
                    secondCol = -1;
                    secondRow = -1;
                    if (matchCounter == 10)
                        winner();
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
            {
                this.Close();
            }

        private void startButton_Click(object sender, EventArgs e)
        {
            //change all pictures to hidden
            foreach (Control c in this.Controls)
            {
                // find the corresponding picturebox
                if (c is PictureBox && c.Tag != null)
                {
                    ((PictureBox)c).Image = Properties.Resources.closed;
                }
            }
            //change all openCards to false
            for(int i = 0; i<openCards.GetLength(0); i++)
            {
                for(int j = 0; j<openCards.GetLength(1); j++)
                {
                    openCards[i, j] = false;
                }
            }
            //shuffle by creating empty answer key array
            answerKey = new int[4,5];
            //for each card
            Random rnd = new Random();
            for(int i = 0; i<10; i++)
            {
                //for each copy
                for (int j = 0; j < 2; j++) {
                    Boolean isOccupied;
                    do
                    {
                        //randomly pick a row and col
                        int row = rnd.Next(4);
                        int col = rnd.Next(5);
                        //if not already occupied, assign the card
                        if (answerKey[row, col] == 0)
                        {
                            answerKey[row, col] = i + 1;
                            isOccupied = false;
                        }
                        else
                            isOccupied = true;
                    } while (isOccupied);
                }
            }
            //reset matchcounter
            matchCounter = 0;
            matchesLabel.Text = "Matches: 0";
            //restart timer
            _time = 0.0;
            timerGame.Start();
            _isRunning = true;
            _isStarted = true;
        }

        private void timerGame_Tick(object sender, EventArgs e)
            {
                if (_isRunning)
                {
                    _time += 0.02;
                    timerLabel.Text = "Time: " + string.Format("{0:0.00}",_time);
                }
            }
    }
}

//WHEN CARD CLICKED

//if both cards have a value
//flip both cards back over (change picture + update open cards)
//set both index values to null
//if both cards are null
//store index in first card
//flip over (change picture + update openCards)
//else store in second card
//store index in second card
//flip over (change picture + update openCards)
//if answerkey value of first card & second card match, 
//increment matchCounter
//if matchCounter == 10, won game

//WHEN WON
//Say you won!
//stop timer
//if time < best time or no best time
//set best time to time

//to Restart game
//shuffle?
//change all pictures to hidden
//change all openCards to false
//reset timer
//start timer