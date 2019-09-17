using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitaire
{
    internal class Board
    {
        private readonly int winningState = 0; //Test

        public Board parent;

        public string hash = "";
        
        public int selection;
        public int heuristic;
        private int counter;

        public List<int[]> availableMoves = new List<int[]>();
        public List<List<Card>> fields = new List<List<Card>>();
        public List<Card> stack = new List<Card>();
        public List<Card>[] heap = Enumerable.Range(0, 4).Select(x => new List<Card>()).ToArray();
        public List<Card>[] columns = Enumerable.Range(0, 7).Select(x => new List<Card>()).ToArray();

        public Deck deck1 = new Deck();

        public Board()
        {
            for (var i = 0; i < columns.Count(); i++)
            {
                switch (winningState)
                {
                    case 0:
                        GiveCards(i + 1, columns[i]);
                        break;
                    case 1:
                        GiveTestColumns(i + 1, columns[i]);
                        break;
                }

                fields.Add(columns[i]);
            }
            //------------------------------------------------------------------------

            var cardsLeft = deck1.cards.Count();

            switch (winningState)
            {
                case 0:
                    GiveCards(cardsLeft, stack, true);
                    break;
                case 1:
                    GiveGuaranteed(cardsLeft, stack, true);
                    break;
            }

            fields.Add(stack);

            //-----------------------------------------------------------------------------------------------------------

            for (var i = 0; i < heap.Count(); i++) fields.Add(heap[i]);

            hash = ToString();
        }

        public Board(ref Board parent)
        {
            this.parent = parent;

            for (var i = 0; i < columns.Count(); i++)
                columns[i] = new List<Card>(parent.columns[i].Select(entry => new Card(entry.value, entry.Color)
                    {IsVisible = entry.IsVisible}));

            for (var i = 0; i < heap.Count(); i++)
                heap[i] = new List<Card>(parent.heap[i].Select(entry => new Card(entry.value, entry.Color)));

            foreach (var k in parent.stack)
                stack.Add(k);                     //kopiowanie karty

            availableMoves = new List<int[]>();

            for (var i = 0; i < columns.Count(); i++) fields.Add(columns[i]);
            fields.Add(stack);
            for (var i = 0; i < heap.Count(); i++) fields.Add(heap[i]);

            hash = ToString();
        }

        private void ShowColumn(List<Card> column)
        {
            if (column.Any())
                for (var i = 0; i < column.Count(); i++)
                    if (column[i].IsVisible == false)
                        Console.Write("*");

                    else
                    {
                        column[i].ShowCard();
                        Console.WriteLine();
                    }
            else
                Console.WriteLine(); // jeśli pusta
        }


        private void
            GiveCards(int howMany, List<Card> column,
                bool visibility = false) // dla sterty będzie true, a dla kolumn false
        {
            var rnd = new Random();
            int card;

            for (var i = 0; i < howMany; i++)
            {
                card = rnd.Next(52 - counter);

                deck1.cards[card].IsVisible = visibility;

                column.Add(deck1.cards[card]);
                deck1.cards.RemoveAt(card);
                counter++;
            }

            column[column.Count() - 1].IsVisible = true; // ostatnia zawsze jest widoczna
        }


        private void GiveTestColumns(int howMany, List<Card> column, bool visibility = false)
        {
            {
                var card = howMany - 1;

                for (var i = 0; i < howMany; i++)
                {
                    deck1.cards[card].IsVisible = visibility;

                    column.Add(deck1.cards[card]);
                    deck1.cards.RemoveAt(card);
                    card--;
                }

                column[column.Count() - 1].IsVisible = true; // ostatnia zawsze jest widoczna
            }
        }

        private void GiveGuaranteed(int howMany, List<Card> column, bool visibility = false)
        {
            {
                for (var i = 0; i < howMany; i++)
                {
                    deck1.cards[0].IsVisible = visibility;

                    column.Add(deck1.cards[0]);
                    deck1.cards.RemoveAt(0);
                }

                column[column.Count() - 1].IsVisible = true; // ostatnia zawsze jest widoczna
            }
        }

        public void Chosen(int number)
        {
            // Console.WriteLine("Wybrano kolumne {0}",number);   

            var howMany = fields[number].Count() - 1; //Inicjalizacja ostatnią kartą
            var movedCard = fields[number][howMany]; //ostatnia karta w wybranej kolumnie
            bool possible; //czy mozna wykonac ruch

            for (var i = 8; i < 12; i++) //sprawdzanie pól do odkładania
            {
                possible = false;
                if (!fields[i].Any()) //jeśli puste tylko as
                {
                    if (movedCard.value == value.ace && movedCard.Color == (Color) (i - 8)
                    ) // (i - 8) bo i startuje od 8
                        possible = true;
                }

                else
                {
                    var howMany2 = fields[i].Count() - 1;

                    var potential = fields[i][howMany2]; //karta na ktora chcemy Movec

                    if (movedCard.value == potential.value + 1 && movedCard.Color == potential.Color
                    ) //porownanie valuei i Colorow
                        possible = true;
                }

                if (possible)
                {
                    int[] move = {number, i, howMany};
                    availableMoves.Add(move); //dodajemy kolumne na którą można przenieść
                }
            }

            while (howMany != -1 && fields[number][howMany].IsVisible)
            {
                movedCard = fields[number][howMany]; //ostatnia karta w wybranej kolumnie

                for (var i = 0; i < 7; i++) //dla kolumn
                {
                    possible = false;

                    if (!fields[i].Any()) //na pustą można tylko kinga
                    {
                        if (movedCard.value == value.king)
                            possible = true;
                    }

                    else
                    {
                        var howMany2 = fields[i].Count() - 1;

                        var potential = fields[i][howMany2]; //karta na ktora chcemy Movec

                        if (movedCard.value == potential.value - 1 && movedCard.Color2 != potential.Color2
                        ) //porownanie value i Colorow
                            possible = true;
                    }


                    if (possible)
                    {
                        int[] move = {number, i, howMany};
                        availableMoves.Add(move); //dodajemy kolumne na którą można przenieść
                    }
                }

                if (number == 7) //SPRAWDZENIE DLA STERTY
                    for (var i = 8; i < 12; i++) //sprawdzanie pól do odkładania
                    {
                        possible = false;
                        if (!fields[i].Any()) //jeśli puste tylko as
                        {
                            if (movedCard.value == value.ace && movedCard.Color == (Color) (i - 8)
                            ) // (i - 8) bo i startuje od 8
                                possible = true;
                        }

                        else
                        {
                            var howMany2 = fields[i].Count() - 1;

                            var potential = fields[i][howMany2]; //karta na ktora chcemy Movec

                            if (movedCard.value == potential.value + 1 && movedCard.Color == potential.Color
                            ) //porownanie valuei i Colorow
                                possible = true;
                        }


                        if (possible)
                        {
                            int[] move = {number, i, howMany};
                            availableMoves.Add(move); //dodajemy kolumne na którą można przenieść
                        }
                    }

                howMany--; //karta "powyżej"
            } //sprawdzamy dla odsłoniętych


            Console.Write("Available moves:");
            for (var x = 0; x < availableMoves.Count(); x++)
                Console.Write("[{0},{1}, pos.{2}]", availableMoves[x][0], availableMoves[x][1],
                    availableMoves[x][2]); //wypisuje możliwe stany + posycja karty
        }


        public void Draw()
        {
            for (var i = 0; i < 7; i++)
            {
                Console.Write("Column {0}: ", i);
                ShowColumn(fields[i]);
            }

            //------------------------------------------------------------------------

            Console.WriteLine();
            Console.WriteLine("7.Stack: ");
            ShowColumn(stack);
            Console.WriteLine("Cards on stack: " + stack.Count()); //howMany jest zakrytych kart
            Console.WriteLine();

            //-----------------------------------------------------------------------------------------------------------

            var numer = 8;
            for (var i = 0; i < heap.Count(); i++)
                if (heap[i].Any())
                {
                    Console.WriteLine();
                    Console.WriteLine(numer + i + ".heap_" + Enum.GetName(typeof(Color), i) + ": ");
                    heap[i][heap[i].Count() - 1].ShowCard();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(numer + i + ".heap_" + Enum.GetName(typeof(Color), i) + ": ");
                }


            if (selection >= 0 && selection < 8)
            {
                Console.WriteLine();

                if (fields[selection].Any())
                    Chosen(selection);

                else
                {
                    Console.WriteLine("Field is empty");
                    for (var i = 0; i < availableMoves.Count(); i++)
                        Console.Write("[{0},{1}, pos.{2}] ", availableMoves[i][0], availableMoves[i][1],
                            availableMoves[i][2]);                  //wypisuje możliwe stany + posycja karty
                }

                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < 12; i++)
            {
                result += i + ":";

                if (fields[i].Any())
                    result += fields[i][fields[i].Count - 1] + fields[i].Count.ToString() + ",";

                else
                    result += "0,";
            }

            return result;
        }

        public static void Move(ref Board copy, int k1, int k2, int pos)
        {
            try
            {
                copy.fields[k1][pos - 1].IsVisible = true; //odslanianie poprzedniej karty
            }

            catch
            {
                //Console.WriteLine("Blad przenoszenia"); Console.ReadLine();
            }

            for (var i = pos; i < copy.fields[k1].Count; i++)
            {
                copy.fields[k2].Add(copy.fields[k1][i]);

                if (k1 == 7) //ze sterty przenosimy jedna
                    break;
            }

            for (var i = pos; i < copy.fields[k1].Count;)
            {
                copy.fields[k1].RemoveAt(pos);

                if (k1 == 7) //ze sterty przenosimy jedna
                    break;
            }

            copy.hash = copy.ToString();
        }

        public int CountHeuristic1()
        {
            var howMany = 0;

            foreach (var s in heap)
            foreach (var k in s)
                howMany++;

            return heuristic = 52 - howMany;
        }

        public int CountHeuristic2()
        {
            var howMany = 0;

            foreach (var col in columns)
            foreach (var c in col)
                if (c.IsVisible == false)
                    howMany++;

            return heuristic = CountHeuristic1() + 20 * howMany;
        }
    }
}