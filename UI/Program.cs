using System;
using Model.Repositories;
using System.Linq;
using Model.Entities;
using Microsoft.EntityFrameworkCore;


namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "LX";
            var keuze = "";
            try
            {
                while (keuze != "X")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine();
                    Console.WriteLine($"=================");
                    Console.WriteLine($"Landen, Steden en Talen");
                    Console.WriteLine($"=================");
                    Console.WriteLine();

                    Console.WriteLine("--------");
                    Console.WriteLine("M E N U - Geen land gekozen");
                    Console.WriteLine("--------");
                    Console.WriteLine("Lijst <L>anden");
                    Console.WriteLine("e<X>it");
                    Console.WriteLine();

                    Console.Write("\tGeef uw keuze (LX)* : ");
                    keuze = Console.ReadLine().ToUpper();

                    while (!input.Contains(keuze))
                    {
                        Console.Write("Geef uw keuze: ");
                        keuze = Console.ReadLine().ToUpper();
                    }

                    switch (keuze)
                    {
                        case "L":
                            ToonLijstVanLanden();

                            break;
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Database is niet gevonden");
            }

            Console.Clear();
        }

        public static void ToonLijstVanLanden()
        {
            using var context = new EFLandenContext();

            var landen = (from land in context.Landen
                          orderby land.Naam
                          select land).ToList();

            if (landen != null)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Lijst van Landen");
                Console.WriteLine("----------------");

                foreach (var land in landen)
                {
                    Console.WriteLine("{0}\t{1,-20}\t{2,20}\t{3,15}", land.ISOLandCode, land.Naam, land.AantalInwoners, land.Oppervlakte);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Geef een landcode in : ");
            string landcode = Console.ReadLine().ToUpper();

            if (landcode != null)
            {
                KiesLandDoorLandcode(landcode);
            }
            else
            {
                Console.WriteLine("Geef een geldig Landcode in");
            }

        }

        public static void KiesLandDoorLandcode(string landcode)
        {
            using var context = new EFLandenContext();

            var land = context.Landen.Where(b => b.ISOLandCode == landcode).FirstOrDefault();

            if (land != null)
            {
                Console.WriteLine();
                Console.WriteLine("--------");
                Console.WriteLine($"M E N U - {land.Naam}");
                Console.WriteLine("--------");
                Console.WriteLine("Lijst <L>anden");
                Console.WriteLine("<W>ijzig Land");
                Console.WriteLine("Lijst <S>teden");
                Console.WriteLine("Lijst T<A>len");
                Console.WriteLine("Stad <T>oevoegen");
                Console.WriteLine("Stad <V>erwijderen");
                Console.WriteLine("e<X>it");
                Console.WriteLine();

                Console.Write("\tGeef uw keuze (LWSATVX)* : ");
                string keuze = Console.ReadLine().ToUpper();

                switch (keuze)
                {
                    case "L":
                        ToonLijstVanLanden();
                        break;

                    case "W":
                        WijzigenLand(landcode);
                        break;

                    case "S":
                        ToonLijstVanSteden(landcode);
                        break;

                    case "A":
                        ToonLijstVanTalen(landcode);
                        break;

                    case "T":
                        ToevoegenSteden(landcode);
                        break;

                    case "V":
                        VerwijderenSteden(landcode);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Land niet gevonden");
            }
        }

        public static void ToonLijstVanSteden(string landcode)
        {
            using var context = new EFLandenContext();

            var steden = context.Steden.Where(b => b.ISOLandCode == landcode).ToList();

            if (steden != null)
            {
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine("Lijst van Steden");
                Console.WriteLine("----------------");

                foreach (var stad in steden)
                {
                    Console.WriteLine(stad.Naam);
                }
            }
            else
            {
                Console.WriteLine("Steden niet gevonden");
            }

            KiesLandDoorLandcode(landcode);
        }

        public static void ToonLijstVanTalen(string landcode)
        {
            using var context = new EFLandenContext();

            var talen = (from taal in context.LandenTalen.Include("Taal")
                         where taal.LandCode == landcode
                         select taal).ToList();

            if (talen != null)
            {
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine("Lijst van Talen");
                Console.WriteLine("----------------");

                foreach (var taal in talen)
                {
                    Console.WriteLine($"{taal.Taal.ISOTaalCode} \t{taal.Taal.NaamNL}");
                }
            }
            else
            {
                Console.WriteLine("Talen niet gevonden");
            }

            KiesLandDoorLandcode(landcode);
        }

        static void WijzigenLand(string landcode)
        {
            using var context = new EFLandenContext();

            var land = context.Landen.Find(landcode);

            if (land != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Gegevens land {land.Naam} :");
                Console.WriteLine($"\tAantal inwoners:  \t{land.AantalInwoners}");
                Console.WriteLine($"\tOppervlakte:      \t{land.Oppervlakte}");
                Console.WriteLine();

                Console.WriteLine("Wijzig aantal inwoners :");
                if (int.TryParse(Console.ReadLine(), out int inwoners))
                {
                    land.AantalInwoners = inwoners;
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("niet wijzigen");
                }

                Console.WriteLine();
                Console.WriteLine("Wijzig aantal oppervlakte :");
                if (float.TryParse(Console.ReadLine(), out float oppervlakte))
                {
                    land.Oppervlakte = oppervlakte;
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("niet wijzigen");
                }
            }
            else
            {
                Console.WriteLine("Landen niet gevonden");
            }

            KiesLandDoorLandcode(landcode);
        }

        static void ToevoegenSteden(string landcode)
        {
            using var context = new EFLandenContext();

            var land = context.Landen.Find(landcode);

            if (land != null)
            {
                Console.WriteLine();
                Console.WriteLine("Geef de naam van een nieuwe stad :");
                var stadNaam = Console.ReadLine();

                if (stadNaam != null)
                {
                    var stad = new Stad { Naam = stadNaam };

                    land.Steden.Add(stad);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Tik een stad");
                }
            }
            else
            {
                Console.WriteLine("Landen niet gevonden");
            }

            KiesLandDoorLandcode(landcode);
        }

        static void VerwijderenSteden(string landcode)
        {
            using var context = new EFLandenContext();

            var steden = context.Steden.Where(b => b.ISOLandCode == landcode).ToList();

            if (steden != null)
            {
                Console.WriteLine();
                Console.WriteLine("Kies de naam van een stad");
                Console.WriteLine("----------------------------------");

                foreach (var stad in steden)
                {
                    Console.WriteLine($"{stad.StadId} \t{stad.Naam}");
                }

                Console.WriteLine("Geef het volgnummer uit de lijst : ");
                if (int.TryParse(Console.ReadLine(), out int stadId))
                {
                    var stad = context.Steden.Find(stadId);

                    if (stad != null && stad.ISOLandCode == landcode)
                    {
                        context.Steden.Remove(stad);
                        context.SaveChanges();
                        Console.WriteLine($"Verwijderen van stad {stad.Naam}");
                    }
                    else
                    {
                        Console.WriteLine("Stad niet gevonden");
                    }
                }
            }
            KiesLandDoorLandcode(landcode);
        }
    }
}
