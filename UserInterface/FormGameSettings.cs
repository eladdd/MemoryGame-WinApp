using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class FormGameSettings : Form
    {
        private readonly List<string> r_GameBoardSizesList;
        private bool m_IsSinglePlayer;
        private int m_ListIndex;

        public FormGameSettings()
        {
            InitializeComponent();
            IsSinglePlayer = true;
            r_GameBoardSizesList = new List<string>
                                       {
                                           "4 x 4",
                                           "4 x 5",
                                           "4 x 6",
                                           "5 x 4",
                                           "5 x 6",
                                           "6 x 4",
                                           "6 x 5",
                                           "6 x 6"
                                       };
            ListIndex = 0;
        }

        public bool IsSinglePlayer
        {
            get
            {
                return this.m_IsSinglePlayer;
            }

            set
            {
                this.m_IsSinglePlayer = value;
            }
        }

        public List<string> GameBoardSizeList
        {
            get
            {
                return this.r_GameBoardSizesList;
            }
        }

        public int ListIndex
        {
            get
            {
                return this.m_ListIndex;
            }

            set
            {
                this.m_ListIndex = value;
            }
        }

        private void buttonAgainst_Click(object i_Sender, EventArgs i_E)
        {
            this.textBoxSecondPlayerName.Enabled = !this.textBoxSecondPlayerName.Enabled;
            this.IsSinglePlayer = !this.IsSinglePlayer;

            if (this.textBoxSecondPlayerName.Enabled)
            {
                this.textBoxSecondPlayerName.Text = string.Empty;
                this.buttonAgainst.Text = "Against Computer";
            }
            else
            {
                this.textBoxSecondPlayerName.Text = "- computer -";
                this.buttonAgainst.Text = "Against a Friend";
            }
        }

        private void buttonBoardSize_Click(object i_Sender, EventArgs i_E)
        {
            ListIndex = (ListIndex + 1) % GameBoardSizeList.Count;
            this.buttonBoardSize.Text = GameBoardSizeList[ListIndex];
        }

        private void checkSettings()
        {
            if (string.IsNullOrEmpty(textBoxFirstPlayerName.Text))
            {
                textBoxFirstPlayerName.Text = "Player1";
            }

            if (string.IsNullOrEmpty(textBoxSecondPlayerName.Text))
            {
                textBoxSecondPlayerName.Text = "Player2";
            }
        }

        private void buttonStart_Click(object i_Sender, EventArgs i_E)
        {
            // require to start the game when click on "x" or start game
            // so instead of duplicate the code we call to this.close
            this.Close();
        }

        protected override void OnClosed(EventArgs i_E)
        {
            base.OnClosed(i_E);
            checkSettings();
            string size = GameBoardSizeList[ListIndex]; // 4 x 4
            int boardRows = size[0] - '0';
            int boardCols = size[size.Length - 1] - '0';
            FormBoardGame game = new FormBoardGame(
                boardRows,
                boardCols,
                textBoxFirstPlayerName.Text,
                textBoxSecondPlayerName.Text,
                IsSinglePlayer);
            this.Visible = false;
            game.ShowDialog();
        }
    }
}
