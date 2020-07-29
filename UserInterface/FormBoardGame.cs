using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GameLogic;

namespace UserInterface
{
    public partial class FormBoardGame : Form
    {
        private const int k_LeftXBias = 12;
        private const int k_TopYBias = 12;
        private const int k_LeftBiasHelper = 100;
        private const int k_TopBiasHelper = 100;
        private readonly int r_BoardRows;
        private readonly int r_BoardCols;
        private readonly string r_FirstPlayerName;
        private readonly string r_SecondPlayerName;
        private readonly bool r_IsSinglePlayer;
        private readonly CardButton[,] r_ButtonsMatrix;
        private readonly PictureBox r_DefaultPic;
        private readonly Timer r_TimerPcTurn;
        private List<PictureBox> m_ListPictureBox;
        private bool m_IsFirstPlayerTurn;
        private int m_CountTurns;
        private Logic m_GameLogic;
        private Label m_LabelPlayerTurn;
        private Label m_LabelFirstPlayerScore;
        private Label m_LabelSecondPlayerScore;

        public FormBoardGame(
            int i_BoardRows,
            int i_BoardCols,
            string i_FirstPlayerName,
            string i_SecondPlayerName,
            bool i_IsSinglePlayer)
        {
            InitializeComponent();

            // init game features ( they are readonly so we cant init in sub function)
            r_DefaultPic = new PictureBox();
            m_CountTurns = 0;
            IsFirstPlayerTurn = true;
            r_IsSinglePlayer = i_IsSinglePlayer;
            r_FirstPlayerName = i_FirstPlayerName;
            r_SecondPlayerName = i_SecondPlayerName;
            r_BoardRows = i_BoardRows;
            r_BoardCols = i_BoardCols;

            // create GameLogic
            m_GameLogic = new Logic(FirstPlayerName, SecondPlayerName, BoardRows, BoardCols);

            // create card button matrix 
            r_ButtonsMatrix = new CardButton[BoardRows, BoardCols];
            InitPictureBoxList();
            initCardsButtons();

            // resize client according to the matrix
            this.ClientSize = new Size(
                r_ButtonsMatrix[BoardRows - 1, BoardCols - 1].Right + k_LeftXBias,
                m_LabelSecondPlayerScore.Bottom + k_TopYBias);

            // if is single player so we define the Ai
            if (r_IsSinglePlayer)
            {
                // create timer for pc turn (instead of use sleep), init ai
                // timer start condition in line 298 and stop condition in  line 324
                this.r_TimerPcTurn = new Timer { Interval = 500 };
                this.TimerPcTurn.Tick += TimerPcTurn_Tick;
                m_GameLogic.InitAi(BoardRows, BoardCols);
            }
        }

        public List<PictureBox> ListPictureBoxes
        {
            get
            {
                return this.m_ListPictureBox;
            }

            set
            {
                this.m_ListPictureBox = value;
            }
        }

        public string FirstPlayerName
        {
            get
            {
                return this.r_FirstPlayerName;
            }
        }

        public string SecondPlayerName
        {
            get
            {
                return this.r_SecondPlayerName;
            }
        }

        public int BoardRows
        {
            get
            {
                return this.r_BoardRows;
            }
        }

        public int BoardCols
        {
            get
            {
                return this.r_BoardCols;
            }
        }

        public bool IsFirstPlayerTurn
        {
            get
            {
                return this.m_IsFirstPlayerTurn;
            }

            set
            {
                this.m_IsFirstPlayerTurn = value;
            }
        }

        public Timer TimerPcTurn
        {
            get
            {
                return this.r_TimerPcTurn;
            }
        }

        public void InitPictureBoxList()
        {
            // this function get random pictures from the link we got in the assignment
            // we know its possible to get the same picture (low chance),
            // we could get random id with duplicate check and get img from "https://picsum.photos/id/"
            // **we didn't get answer about the random issue (if we can use different link or check by pixels) so we leave it as is**
            ListPictureBoxes = new List<PictureBox>();
            int size = BoardRows * BoardCols / 2;

            for (int i = 0; i < size; i++)
            {
                ListPictureBoxes.Add(new PictureBox());
                ListPictureBoxes[i].Load(@"https://picsum.photos/80");
            }

            r_DefaultPic.Load(@"https://picsum.photos/id/210/80"); // card back picture
        }

        private void initCardsButtons()
        {
            CardButton currentButton;
            int xPoint = 0;
            int yPoint = 0;

            for (int i = 0; i < BoardRows; i++)
            {
                for (int j = 0; j < BoardCols; j++)
                {
                    xPoint = k_LeftXBias + (j * k_LeftBiasHelper);
                    yPoint = k_TopYBias + (i * k_TopBiasHelper);
                    currentButton = new CardButton(i, j, xPoint, yPoint, m_GameLogic.PrintCard(i, j));
                    currentButton.Image = r_DefaultPic.Image;
                    this.Controls.Add(currentButton);
                    currentButton.Click += new EventHandler(button_click);
                    r_ButtonsMatrix[i, j] = currentButton;
                }
            }

            initLabels(yPoint + k_TopBiasHelper);
        }

        private void initLabels(int i_YPoint)
        {
            // Create
            Label unChangedLabelFirstPlayer = new Label();
            Label unChangedLabelSecondPlayer = new Label();
            m_LabelPlayerTurn = new Label();
            m_LabelFirstPlayerScore = new Label();
            m_LabelSecondPlayerScore = new Label();

            // Texts
            m_LabelPlayerTurn.Text = "Current Player: " + FirstPlayerName;
            unChangedLabelFirstPlayer.Text = FirstPlayerName + ": ";
            unChangedLabelSecondPlayer.Text = SecondPlayerName + ": ";
            m_LabelFirstPlayerScore.Text = m_GameLogic.GetPlayerScore(IsFirstPlayerTurn) + " Pairs";
            m_LabelSecondPlayerScore.Text = m_GameLogic.GetPlayerScore(!IsFirstPlayerTurn) + " Pairs";

            // Sizes
            unChangedLabelFirstPlayer.AutoSize = true;
            unChangedLabelSecondPlayer.AutoSize = true;
            m_LabelPlayerTurn.AutoSize = true;
            m_LabelFirstPlayerScore.AutoSize = true;
            m_LabelSecondPlayerScore.AutoSize = true;

            // Add controls (let width get update from auto size for next step)
            this.Controls.Add(m_LabelPlayerTurn);
            this.Controls.Add(unChangedLabelFirstPlayer);
            this.Controls.Add(m_LabelFirstPlayerScore);
            this.Controls.Add(unChangedLabelSecondPlayer);
            this.Controls.Add(m_LabelSecondPlayerScore);

            // Locations
            m_LabelPlayerTurn.Location = new Point(k_LeftXBias, i_YPoint += k_TopYBias * 2);
            unChangedLabelFirstPlayer.Location = new Point(k_LeftXBias, i_YPoint += k_TopYBias * 2);
            unChangedLabelSecondPlayer.Location = new Point(k_LeftXBias, i_YPoint += k_TopYBias * 2);
            m_LabelFirstPlayerScore.Location = new Point(
                unChangedLabelFirstPlayer.Location.X + unChangedLabelFirstPlayer.Width,
                unChangedLabelFirstPlayer.Location.Y);
            m_LabelSecondPlayerScore.Location = new Point(
                unChangedLabelSecondPlayer.Location.X + unChangedLabelSecondPlayer.Width,
                unChangedLabelSecondPlayer.Location.Y);

            // Colors
            unChangedLabelFirstPlayer.BackColor = Color.GreenYellow;
            unChangedLabelSecondPlayer.BackColor = Color.MediumPurple;
            m_LabelFirstPlayerScore.BackColor = unChangedLabelFirstPlayer.BackColor;
            m_LabelSecondPlayerScore.BackColor = unChangedLabelSecondPlayer.BackColor;
            m_LabelPlayerTurn.BackColor = m_LabelFirstPlayerScore.BackColor;
        }

        private void updateLabelPlayerTurn()
        {
            if (IsFirstPlayerTurn)
            {
                m_LabelPlayerTurn.Text = "Current Player: " + FirstPlayerName;
                m_LabelPlayerTurn.BackColor = m_LabelFirstPlayerScore.BackColor;
            }
            else
            {
                m_LabelPlayerTurn.Text = "Current Player: " + SecondPlayerName;
                m_LabelPlayerTurn.BackColor = m_LabelSecondPlayerScore.BackColor;
            }
        }

        private void updateLabelsScore()
        {
            int score = m_GameLogic.GetPlayerScore(IsFirstPlayerTurn);
            string pairs = score == 1 ? " Pair(s)" : " Pairs";

            if (IsFirstPlayerTurn)
            {
                m_LabelFirstPlayerScore.Text = score + pairs;
            }
            else
            {
                m_LabelSecondPlayerScore.Text = score + pairs;
            }
        }

        private void button_click(object i_Sender, EventArgs i_E)
        {
            m_CountTurns++;
            CardButton clickedButton = i_Sender as CardButton;
            string playerPickIndex = clickedButton.CardPosition;

            // send player/pc card choose to the game logic
            eInputError inputError;
            if (!m_GameLogic.ChooseCard(playerPickIndex, out inputError))
            {
                switch (inputError)
                {
                    case eInputError.CardAlreadyPicked:
                        MessageBox.Show("Oops, card is already picked try again");
                        break;
                    default:
                        MessageBox.Show("Something Wrong");
                        break;
                }

                m_CountTurns--;
            }
            else
            {
                updateCardPickedFeatures(ref clickedButton, playerPickIndex);
                if (m_CountTurns % 2 == 0)
                {
                    checkCardsEqual();
                    if (m_GameLogic.IsGameOver())
                    { // game will be over just if turn % 2 == 0 because number of cards are even
                        showGameResults();
                    }
                    else
                    {
                        // if game isnt over so check if its pc turn to play
                        if (!IsFirstPlayerTurn && r_IsSinglePlayer)
                        {
                            if (!TimerPcTurn.Enabled)
                            {
                                TimerPcTurn.Start();
                            }
                        }
                    }
                }
            }
        }

        private void updateCardPickedFeatures(ref CardButton io_ClickedButton, string i_CardPickIndex)
        {
            // init cards value
            if (io_ClickedButton.Value == ' ')
            {
                io_ClickedButton.Value = m_GameLogic.PrintCard((i_CardPickIndex[1] - '0') - 1, i_CardPickIndex[0] - 'A');
            }

            io_ClickedButton.Image = ListPictureBoxes[io_ClickedButton.Value - 'A'].Image;
            io_ClickedButton.IsCardPicked = true;
            io_ClickedButton.BackColor = m_LabelPlayerTurn.BackColor;
            io_ClickedButton.Refresh(); // to show the picture before the sleep method
        }

        private void checkCardsEqual()
        {
            if (TimerPcTurn != null && TimerPcTurn.Enabled)
            { // stop the timer  if there is game vs pc
                TimerPcTurn.Stop();
            }

            if (!m_GameLogic.AreCardsEqual(IsFirstPlayerTurn))
            {
                // wait 1 sec if cards are not equal
                System.Threading.Thread.Sleep(1000);
                IsFirstPlayerTurn = !IsFirstPlayerTurn;
                updateCardButtonImage(); // return buttons images to default
                updateLabelPlayerTurn(); // update label current turn
            }
            else
            {
                updateMatchCards(); // mark the cards that are equal for it wont update their picture to default
                updateLabelsScore(); // update label score , because in this case some player find a match
            }
        }

        private void TimerPcTurn_Tick(object i_Sender, EventArgs i_E)
        {
            string cardIndex = m_GameLogic.AiIndexCard();
            r_ButtonsMatrix[cardIndex[1] - '0' - 1, cardIndex[0] - 'A'].PerformClick();
        }

        private void showGameResults()
        {
            int firstPlayerScore =
                m_GameLogic.GetPlayerScore(true); // true = first player score
            int secondPlayerScore =
                m_GameLogic.GetPlayerScore(false); // false = second player score
            string resultsText = string.Format(
                @"Game End, Statistics:
First Player {0} Score: {1}
Second Player {2} Score: {3}",
                FirstPlayerName,
                firstPlayerScore,
                SecondPlayerName,
                secondPlayerScore);

            string winnerText = string.Empty;
            if (firstPlayerScore > secondPlayerScore)
            {
                winnerText = string.Format("First Player {0} is the winner!", FirstPlayerName);
            }
            else if (firstPlayerScore < secondPlayerScore)
            {
                winnerText = string.Format("Second Player {0} is the winner!", SecondPlayerName);
            }
            else
            {
                // case firstPlayerScore == SecondPlayerScore
                winnerText = "DRAW! there is no winner";
            }

            DialogResult result = MessageBox.Show(
                string.Format(
                    @"{0}

{1}

Do you want Try Again?
(May take up to 3-4 sec due to image reload)",
                    winnerText,
                    resultsText),
                "Memory Game - Results",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                restartGame();
            }
            else
            {
                this.Close();
            }
        }

        private void restartGame()
        {
            // create new game logic
            m_GameLogic = new Logic(FirstPlayerName, SecondPlayerName, BoardRows, BoardCols);

            // reset turn for first player
            IsFirstPlayerTurn = true;

            // get new pictures
            InitPictureBoxList(); // pictureBox.load its slow function so its take 3-4 sec

            // restart labels
            updateLabelPlayerTurn();
            m_LabelFirstPlayerScore.Text = "0 Pairs";
            m_LabelSecondPlayerScore.Text = "0 Pairs";

            // restart the cards to their default settings
            for (int i = 0; i < BoardRows; i++)
            {
                for (int j = 0; j < BoardCols; j++)
                {
                    r_ButtonsMatrix[i, j].ResetCard(m_GameLogic.PrintCard(i, j));
                    r_ButtonsMatrix[i, j].Image = r_DefaultPic.Image;
                    r_ButtonsMatrix[i, j].BackColor = DefaultBackColor;
                }
            }

            // if is game vs pc so recreate the pc ai data
            if (r_IsSinglePlayer)
            {
                m_GameLogic.InitAi(BoardRows, BoardCols);
            }
        }

        private void updateMatchCards()
        {
            CardButton currentButton;
            for (int i = 0; i < BoardRows; i++)
            {
                for (int j = 0; j < BoardCols; j++)
                {
                    currentButton = r_ButtonsMatrix[i, j];
                    if (currentButton.IsCardPicked)
                    {
                        currentButton.IsCardPicked = false;
                        currentButton.IsCardMatch = true;
                    }
                }
            }
        }

        private void updateCardButtonImage()
        {
            // run on loop for all the card buttons on matrix
            CardButton currentButton;
            for (int i = 0; i < BoardRows; i++)
            {
                for (int j = 0; j < BoardCols; j++)
                {
                    currentButton = r_ButtonsMatrix[i, j];
                    if (!currentButton.IsCardMatch && currentButton.IsCardPicked)
                    {
                        currentButton.Image = r_DefaultPic.Image;
                        currentButton.BackColor = DefaultBackColor;
                        currentButton.IsCardPicked = false;
                        currentButton.Refresh();
                    }
                }
            }
        }
    }
}
