using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    class Card
    {
        public bool IsVisible { get; set; } = false;
        public value value { get; private set; }
        public Color Color { get; private set; }
        public Color2 Color2
        {
            get
            {
                if (Color == Color.diamond || Color == Color.heart)
                    return Color2.red;
                else
                    return Color2.black;
            }
        }

        public Card(value vl, Color cl)
        {
            value = vl;
            Color = cl;
        }

        public void ShowCard() => Console.Write(value + " " + Color);

        public override string ToString()
        {
            return this.value.ToString() + "_" + Color.ToString();
        }
    }

    enum Color
    {
        heart = 0,
        diamond = 1,
        spades = 2,
        club = 3
    };

    enum Color2
    {
        black,
        red
    };

    enum value
    {
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10,
        jack = 11,
        queen = 12,
        king = 13,
        ace = 1
    };
}