using System;
using System.Timers;

namespace TamaLib
{
    public class Tamagotchi
    {
        private const int Honger_MaxValue = 4;
        private const int Honger_MinValue = 0;

        private const int Geluk_MaxValue = 4;
        private const int Geluk_MinValue = 0;

        private const int Intelligentie_MaxValue = 4;
        private const int Intelligentie_MinValue = 0;

        private readonly Random random = new Random();

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
                if (Honger == Honger_MinValue || Geluk == Geluk_MinValue || Intelligentie == Intelligentie_MinValue)
                    return Levensstadium.Dood;

                TimeSpan leeftijd = DateTime.Now - Geboortedatum;

                // als de cases van een switch niet veel code bevatten (in dit geval enkel return), kan je de switch sinds C# 8 als expression schrijven en het resultaat hiervan meteen returnen
                // cf.: https://learn.microsoft.com/en-US/dotnet/csharp/language-reference/operators/switch-expression
                // deze switch gebruikt ook de "when clause", geïntroduceerd in C# 7 om voorwaarden op te leggen aan een case (daarvoor moest je dit afhandelen met een cascade van if-statements)
                // cf.: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/switch#-the-case-statement-and-the-when-clause
                return leeftijd.TotalMinutes switch
                {
                    double minutes when minutes <= 1 => Levensstadium.Ei,
                    double minutes when minutes <= 10 => Levensstadium.Baby,
                    double minutes when minutes <= 60 => Levensstadium.Kind,
                    double minutes when minutes <= 120 => Levensstadium.Puber,
                    double minutes when minutes <= 300 => Levensstadium.Volwassen,
                    double minutes when minutes <= 400 => Levensstadium.Senior,
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


                /******************************************************************************************************
                 * ter referentie; dezelfde code op de "traditionele" manier, zonder when clause, met een if-cascade: *
                 ******************************************************************************************************/
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
                if (value >= Honger_MinValue && value <= Honger_MaxValue)
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
                if (value >= Geluk_MinValue && value <= Geluk_MaxValue)
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
                if (value >= Intelligentie_MinValue && value <= Intelligentie_MaxValue)
                {
                    intelligentie = value;
                }
            }
        }

        /// <summary>
        /// Constructor voor Tamagotchi. Stelt parameters in op default waarde en start de timer.
        /// </summary>
        /// <param name="naam">Naam van de Tamagotchi (default: Beestje)</param>
        public Tamagotchi(string naam = "Beestje")
        {
            Naam = naam;
            Geboortedatum = DateTime.Now;
            Honger = Geluk = Intelligentie = 2;
            timer = new Timer(1000) { AutoReset = true, Enabled = true };
            timer.Elapsed += CheckLevensstadiumVeranderd;
            timer.Elapsed += VeranderParameters;
            timer.Start();
        }

        /// <summary>
        /// Controleert of het levensstadium is veranderd t.o.v. vorige tick van de timer en vuurt LevensstadiumChangedEvent af indien dit zo is.
        /// Stopt ook de timer als de Tamagotchi dood is.
        /// </summary>        
        private void CheckLevensstadiumVeranderd(object sender, ElapsedEventArgs e)
        {
            if (Levensstadium != vorigLevensstadium)
            {
                vorigLevensstadium = Levensstadium;
                if (vorigLevensstadium == Levensstadium.Dood)
                {
                    timer.Stop();
                }

                if (LevensstadiumChangedEvent != null)
                {
                    LevensstadiumChangedEvent(Levensstadium);
                }

                // bovenstaande if kan ook korter met null-conditional operator (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-)                
                // LevensstadiumChangedEvent?.Invoke(Levensstadium);

            }
        }


        /// <summary>
        /// Laat willekeurig honger, geluk of intelligentie zakken en vuurt ParameterChangedEvent nadat een van deze parameters is gewijzigd.
        /// </summary>
        private void VeranderParameters(object sender, ElapsedEventArgs e)
        {
            int randomNumber = random.Next(0, 10);
            if (randomNumber < 3)
            {
                if (randomNumber == 0)
                {
                    Honger--;
                }
                else if (randomNumber == 1)
                {
                    Geluk--;
                }
                else if (randomNumber == 2)
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
