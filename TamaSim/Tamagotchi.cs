using System;
using System.Timers;

namespace TamaSim
{
    enum Levensstadium 
    {
        Ei,
        Baby,
        Kind,
        Puber,
        Volwassen,
        Senior,
        Dood
    }

    class Tamagotchi
    {
        private readonly Timer timer;
        private Levensstadium vorigLevensstadium;

        public delegate void LevensstadiumChanged(Levensstadium levensstadium);
        public event LevensstadiumChanged LevensstadiumChangedEvent;

        public delegate void ParameterChanged();
        public event ParameterChanged ParameterChangedEvent;

        public string Naam { get; set; }
        public DateTime Geboortedatum { get; set; }
        public Levensstadium Levensstadium
        {
            get
            {
                if (Honger == 0 || Geluk == 0 || Intelligentie == 0)
                    return Levensstadium.Dood;

                TimeSpan leeftijd = DateTime.Now - Geboortedatum;

                // als de cases van een switch niet veel code bevatten (in dit geval enkel return), kan je de switch sinds C# 8 als expression schrijven en het resultaat hiervan meteen returnen
                // cf.: https://www.c-sharpcorner.com/article/c-sharp-8-0-new-feature-swtich-expression/
                // deze switch gebruikt ook de "when clause", geïntroduceerd in C# 7 om voorwaarden op te leggen aan een case (daarvoor moest je dit afhandelen met een cascade van if-statements)
                // cf.: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/switch#-the-case-statement-and-the-when-clause
                return (leeftijd.TotalMinutes) switch
                {
                    double minutes when (minutes <= 1) => Levensstadium.Ei,
                    double minutes when (minutes <= 10) => Levensstadium.Baby,
                    double minutes when (minutes <= 60) => Levensstadium.Kind,
                    double minutes when (minutes <= 120) => Levensstadium.Puber,
                    double minutes when (minutes <= 300) => Levensstadium.Volwassen,
                    double minutes when (minutes <= 400) => Levensstadium.Senior,
                    _ => Levensstadium.Dood,
                };

                /*****************************************************************************
                 * ter referentie; dezelfde switch, maar dan niet als expression (pre C# 8): *
                 *****************************************************************************/
                //switch (leeftijd.TotalMinutes)
                //{
                //    case double minutes when (minutes <= 1):
                //        return Levensstadium.Ei;
                //    case double minutes when (minutes <= 10):
                //        return Levensstadium.Baby;
                //    case double minutes when (minutes <= 60):
                //        return Levensstadium.Kind;
                //    case double minutes when (minutes <= 120):
                //        return Levensstadium.Puber;
                //    case double minutes when (minutes <= 300):
                //        return Levensstadium.Volwassen;
                //    case double minutes when (minutes <= 400):
                //        return Levensstadium.Senior;
                //    default:
                //        return Levensstadium.Dood;
                //}


                /*********************************************************************************
                 * ter referentie; dezelfde code op de "traditionele" manier met een if cascade: *
                 *********************************************************************************/
                //if(leeftijd.TotalMinutes <= 1)
                //{
                //    return Levensstadium.Ei;
                //}
                //else if (leeftijd.TotalMinutes <= 10)
                //{
                //    return Levensstadium.Baby;
                //}
                //else if (leeftijd.TotalMinutes <= 60)
                //{
                //    return Levensstadium.Kind;
                //}
                //else if (leeftijd.TotalMinutes <= 120)
                //{
                //    return Levensstadium.Puber;
                //}
                //else if (leeftijd.TotalMinutes <= 300)
                //{
                //    return Levensstadium.Volwassen;
                //}
                //else if (leeftijd.TotalMinutes <= 400)
                //{
                //    return Levensstadium.Senior;
                //}
                //else
                //{
                //    return Levensstadium.Dood;
                //}

            }
        }

        private int honger;

        public int Honger
        {
            get { return honger; }
            set
            {
                if (value >= 0 && value <= 4)
                {
                    honger = value;
                }
            }
        }

        private int geluk;

        public int Geluk
        {
            get { return geluk; }
            set
            {
                if (value >= 0 && value <= 4)
                {
                    geluk = value;
                }
            }
        }

        private int intelligentie;

        public int Intelligentie
        {
            get { return intelligentie; }
            set
            {
                if (value >= 0 && value <= 4)
                {
                    intelligentie = value;
                }
            }
        }

        public Tamagotchi(string naam = "Beestje")
        {
            Naam = naam;
            Geboortedatum = DateTime.Now;
            Honger = Geluk = Intelligentie = 4;
            timer = new Timer(1000) { AutoReset = true, Enabled = true };
            timer.Elapsed += IsLevensstadiumVeranderd;
            timer.Elapsed += VeranderParameters;
            timer.Start();
        }

        /// <summary>
        /// Controleert of het levensstadium is veranderd t.o.v. vorige tick van de timer en vuurt LevensstadiumChangedEvent af indien dit zo is.
        /// Stopt ook de timer als de Tamagotchi dood is.
        /// </summary>        
        private void IsLevensstadiumVeranderd(object sender, ElapsedEventArgs e)
        {
            if(Levensstadium != vorigLevensstadium)
            {
                vorigLevensstadium = Levensstadium;
                if(vorigLevensstadium == Levensstadium.Dood)
                {
                    timer.Stop();
                }
               
                if (LevensstadiumChangedEvent != null)
                {
                    LevensstadiumChangedEvent(Levensstadium);
                }
            }
        }

        /// <summary>
        /// Laat willekeurig honger, geluk of intelligentie zakken en vuurt ParameterChangedEvent nadat een van deze parameters is gewijzigd.
        /// </summary>
        private void VeranderParameters(object sender, ElapsedEventArgs e)
        {
            int random = new Random().Next(0, 10);
            if (random < 3)
            {
                if (random == 0)
                {
                    Honger--;
                }
                else if (random == 1)
                {
                    Geluk--;
                }
                else if (random == 2)
                {
                    Intelligentie--;
                }

                if (ParameterChangedEvent != null)
                {
                    ParameterChangedEvent();
                }
            }
        }

    }
}
