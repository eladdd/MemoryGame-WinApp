using System.Drawing;
using System.Windows.Forms;

namespace UserInterface
{
    public class CardButton : Button
    {
        private const int k_CardWidth = 90;
        private const int k_CardHeight = 90;
        private readonly string r_CardPosition;
        private char m_Value;
        private bool m_IsCardPicked;
        private bool m_IsCardMatch;

        public CardButton(int i_Row, int i_Col, int i_X, int i_Y, char i_Value)
        {
            IsCardPicked = false;
            IsCardMatch = false;
            m_Value = i_Value;

            this.Location = new Point(i_X, i_Y);
            r_CardPosition = string.Format("{0}{1}", (char)(i_Col + 'A'), i_Row + 1);
            this.Size = new Size(k_CardWidth, k_CardHeight);
        }

        public string CardPosition
        {
            get
            {
                return this.r_CardPosition;
            }
        }

        public char Value
        {
            get
            {
                return this.m_Value;
            }

            set
            {
                this.m_Value = value;
            }
        }

        public bool IsCardPicked
        {
            get
            {
                return this.m_IsCardPicked;
            }

            set
            {
                this.m_IsCardPicked = value;
            }
        }

        public bool IsCardMatch
        {
            get
            {
                return this.m_IsCardMatch;
            }

            set
            {
                this.m_IsCardMatch = value;
            }
        }

        public void ResetCard(char i_Value)
        {
            IsCardPicked = false;
            IsCardMatch = false;
            m_Value = i_Value;
        }
    }
}
