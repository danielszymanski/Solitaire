using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    class Deck
    {
        public List<Card> cards = new List<Card>();
        public Deck()
        {
            foreach (Color k in Enum.GetValues(typeof(Color)))
                foreach (value val in Enum.GetValues(typeof(value)))
                    cards.Add(new Card(val, k));
        }

        public void ShowCards()
        {
            for (int i = 0; i < 52; i++)
            {
                cards[i].ShowCard();
                if (i == 12 || i == 25 || i == 38)
                    Console.WriteLine();
            }
        }
    }
}