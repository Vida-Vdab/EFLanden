using Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;

namespace Model.Repositories
{
    public class EFLandenContext : DbContext
    {
        public static IConfigurationRoot configuration;
        bool testMode = false;

        //DbSets
        public DbSet<Land> Landen { get; set; }
        public DbSet<Stad> Steden { get; set; }
        public DbSet<Taal> Talen { get; set; }
        public DbSet<LandTaal> LandenTalen { get; set; }

        //Constructors
        public EFLandenContext() { }
        public EFLandenContext(DbContextOptions<EFLandenContext> options) : base(options) { }

        // Logging
        private ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging
            (
            builder => builder.AddConsole()
            .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
            );
            return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
                var connectionString = configuration.GetConnectionString("eflanden");
                if (connectionString != null)
                {
                    optionsBuilder.UseSqlServer(
                    connectionString
                    , options => options.MaxBatchSize(150))
                    //.UseLoggerFactory(GetLoggerFactory())
                    .EnableSensitiveDataLogging(true) 
                    .UseLazyLoadingProxies();
                }
            }
            else
            {
                testMode = true;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Land
            modelBuilder.Entity<Land>().ToTable("Landen");
            modelBuilder.Entity<Land>().HasKey(c => c.ISOLandCode);
            modelBuilder.Entity<Land>().Property(b => b.ISOLandCode)
                .HasMaxLength(2)
                .ValueGeneratedNever();
            modelBuilder.Entity<Land>().Property(b => b.NISLandCode)
                .HasMaxLength(3)
                .IsRequired();
            modelBuilder.Entity<Land>().HasIndex(b => b.NISLandCode)
                .IsUnique();
            modelBuilder.Entity<Land>().Property(b => b.Naam)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Land>().HasIndex(b => b.Naam)
                .IsUnique();
            modelBuilder.Entity<Land>().Property(b => b.AantalInwoners)
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Land>().Property(b => b.Oppervlakte)
                .HasColumnType("real")
                .IsRequired();

            // Concurrency
            modelBuilder.Entity<Land>().Property(b => b.Aangepast).HasColumnType("timestamp");
            modelBuilder.Entity<Land>().Property(b => b.Aangepast)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

            //Stad
            modelBuilder.Entity<Stad>().ToTable("Steden");
            modelBuilder.Entity<Stad>().HasKey(c => c.StadId);
            modelBuilder.Entity<Stad>().Property(b => b.StadId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Stad>().Property(b => b.Naam)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Stad>().HasIndex(b => b.Naam).IsUnique();
            modelBuilder.Entity<Stad>()
                .HasOne(b => b.Land)
                .WithMany(c => c.Steden)
                .HasForeignKey(b => b.ISOLandCode)
                .HasConstraintName("FK_Steden_LandCode");

            // Concurrency
            modelBuilder.Entity<Stad>().Property(b => b.Aangepast).HasColumnType("timestamp");
            modelBuilder.Entity<Stad>().Property(b => b.Aangepast)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

            //Taal
            modelBuilder.Entity<Taal>().ToTable("Talen");
            modelBuilder.Entity<Taal>().HasKey(c => c.ISOTaalCode);
            modelBuilder.Entity<Taal>().Property(b => b.ISOTaalCode)
               .HasMaxLength(2)
               .ValueGeneratedNever();
            modelBuilder.Entity<Taal>().Property(b => b.NaamNL)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Taal>().HasIndex(b => b.NaamNL).IsUnique();
            modelBuilder.Entity<Taal>().Property(b => b.NaamTaal)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<Taal>().HasIndex(b => b.NaamTaal).IsUnique();

            //LandTaal
            modelBuilder.Entity<LandTaal>().ToTable("LandenTalen");
            modelBuilder.Entity<LandTaal>().HasKey(c => new { c.LandCode, c.TaalCode });
           
            modelBuilder.Entity<LandTaal>()
           .HasOne(x => x.Land)
           .WithMany(y => y.LandenTalen)
           .HasForeignKey(x => x.LandCode)
           .HasConstraintName("FK_LandTaal_Land");

            modelBuilder.Entity<LandTaal>()
            .HasOne(x => x.Taal)
            .WithMany(y => y.LandenTalen)
            .HasForeignKey(x => x.TaalCode)
            .HasConstraintName("FK_LandTaal_Taal");


            // Seeding
            if (!testMode)
            {
                //seeding Landen
                modelBuilder.Entity<Land>().HasData
                (
                    new Land { ISOLandCode = "AT", NISLandCode = "105", Naam = "Oostenrijk", AantalInwoners = 8754413, Oppervlakte = 83871f },
                    new Land { ISOLandCode = "BE", NISLandCode = "150", Naam = "België", AantalInwoners = 11500000, Oppervlakte = 30689f },
                    new Land { ISOLandCode = "CH", NISLandCode = "127", Naam = "Zwitserland", AantalInwoners = 8236303, Oppervlakte = 41285f },
                    new Land { ISOLandCode = "DE", NISLandCode = "103", Naam = "Duitsland", AantalInwoners = 80594017, Oppervlakte = 357022f },
                    new Land { ISOLandCode = "DK", NISLandCode = "108", Naam = "Denemarken", AantalInwoners = 5605948, Oppervlakte = 43094f },
                    new Land { ISOLandCode = "ES", NISLandCode = "109", Naam = "Spanje", AantalInwoners = 48958159, Oppervlakte = 505992f },
                    new Land { ISOLandCode = "FR", NISLandCode = "111", Naam = "Frankrijk", AantalInwoners = 62814233, Oppervlakte = 674843f },
                    new Land { ISOLandCode = "GB", NISLandCode = "112", Naam = "Verenigd Konijngkrijk", AantalInwoners = 64769452, Oppervlakte = 242495f },
                    new Land { ISOLandCode = "IT", NISLandCode = "128", Naam = "Italië", AantalInwoners = 62137802, Oppervlakte = 300000f },
                    new Land { ISOLandCode = "LU", NISLandCode = "113", Naam = "Luxemburg", AantalInwoners = 594130, Oppervlakte = 2586f },
                    new Land { ISOLandCode = "NL", NISLandCode = "129", Naam = "Nederland", AantalInwoners = 17424978, Oppervlakte = 41873f },
                    new Land { ISOLandCode = "NO", NISLandCode = "121", Naam = "Noorwegen", AantalInwoners = 5367580, Oppervlakte = 385207f },
                    new Land { ISOLandCode = "PL", NISLandCode = "139", Naam = "Polen", AantalInwoners = 38476269, Oppervlakte = 311888f },
                    new Land { ISOLandCode = "PT", NISLandCode = "123", Naam = "Portugal", AantalInwoners = 10839541, Oppervlakte = 92212f },
                    new Land { ISOLandCode = "SE", NISLandCode = "126", Naam = "Zweden", AantalInwoners = 9960487, Oppervlakte = 450295f },
                    new Land { ISOLandCode = "US", NISLandCode = "402", Naam = "Verenigde Staten", AantalInwoners = 326625791, Oppervlakte = 9826675f }
                 );

                //seeding Talen
                modelBuilder.Entity<Taal>().HasData
                (
                    new Taal { ISOTaalCode = "bg", NaamNL = "Bulgaars", NaamTaal = "български" },
                    new Taal { ISOTaalCode = "cs", NaamNL = "Tsjechisch ", NaamTaal = "čeština " },
                    new Taal { ISOTaalCode = "da", NaamNL = "Deens", NaamTaal = "dansk" },
                    new Taal { ISOTaalCode = "de", NaamNL = "Duits", NaamTaal = "Deutsch" },
                    new Taal { ISOTaalCode = "el", NaamNL = "Grieks", NaamTaal = "ελληνικά" },
                    new Taal { ISOTaalCode = "en", NaamNL = "Engels", NaamTaal = "English" },
                    new Taal { ISOTaalCode = "es", NaamNL = "Spaans", NaamTaal = "español" },
                    new Taal { ISOTaalCode = "et", NaamNL = "Ests", NaamTaal = "eesti keel" },
                    new Taal { ISOTaalCode = "fi", NaamNL = "Fins", NaamTaal = "suomi" },
                    new Taal { ISOTaalCode = "fr", NaamNL = "Frans", NaamTaal = "français" },
                    new Taal { ISOTaalCode = "ga", NaamNL = "lers", NaamTaal = "Gaeilge" },
                    new Taal { ISOTaalCode = "hu", NaamNL = "Hongaars", NaamTaal = "magyar" },
                    new Taal { ISOTaalCode = "it", NaamNL = "Italiaans", NaamTaal = "italiano" },
                    new Taal { ISOTaalCode = "lt", NaamNL = "Litouws", NaamTaal = "lietuvių kalba" },
                    new Taal { ISOTaalCode = "lv", NaamNL = "Lets", NaamTaal = "latviešu valoda" },
                    new Taal { ISOTaalCode = "mt", NaamNL = "Maltees", NaamTaal = "malti" },
                    new Taal { ISOTaalCode = "nl", NaamNL = "Nederlands", NaamTaal = "Nederlands" },
                    new Taal { ISOTaalCode = "pl", NaamNL = "Pools", NaamTaal = "polski" },
                    new Taal { ISOTaalCode = "pt", NaamNL = "Portugees", NaamTaal = "português" },
                    new Taal { ISOTaalCode = "ro", NaamNL = "Roemeens", NaamTaal = "română" },
                    new Taal { ISOTaalCode = "sk", NaamNL = "Slovaaks", NaamTaal = "slovenčina" },
                    new Taal { ISOTaalCode = "sl", NaamNL = "Sloveens", NaamTaal = "slovenščina" },
                    new Taal { ISOTaalCode = "sv", NaamNL = "Zweeds", NaamTaal = "svenska" }
                 );

                //seeding Steden
                modelBuilder.Entity<Stad>().HasData
                (
                    new Stad { StadId = 1, Naam = "Brussel", ISOLandCode = "BE" },
                    new Stad { StadId = 2, Naam = "Antwerepen", ISOLandCode = "BE" },
                    new Stad { StadId = 3, Naam = "Luik", ISOLandCode = "BE" },
                    new Stad { StadId = 4, Naam = "Amsterdam", ISOLandCode = "NL" },
                    new Stad { StadId = 5, Naam = "Den Haag", ISOLandCode = "NL" },
                    new Stad { StadId = 6, Naam = "Rotterdam", ISOLandCode = "NL" },
                    new Stad { StadId = 7, Naam = "Berlijn", ISOLandCode = "DE" },
                    new Stad { StadId = 8, Naam = "Hamburg", ISOLandCode = "DE" },
                    new Stad { StadId = 9, Naam = "München", ISOLandCode = "DE" },
                    new Stad { StadId = 10, Naam = "Luxemburg", ISOLandCode = "LU" },
                    new Stad { StadId = 11, Naam = "Parijs", ISOLandCode = "FR" },
                    new Stad { StadId = 12, Naam = "Marseille", ISOLandCode = "FR" },
                    new Stad { StadId = 13, Naam = "Lyon", ISOLandCode = "FR" }
                 );

                //seeding LandenTalen
                modelBuilder.Entity<LandTaal>().HasData
                (
                    new LandTaal { LandCode = "BE", TaalCode = "de" },
                    new LandTaal { LandCode = "DE", TaalCode = "de" },
                    new LandTaal { LandCode = "LU", TaalCode = "de" },
                    new LandTaal { LandCode = "BE", TaalCode = "fr" },
                    new LandTaal { LandCode = "FR", TaalCode = "fr" },
                    new LandTaal { LandCode = "LU", TaalCode = "fr" },
                    new LandTaal { LandCode = "BE", TaalCode = "nl" },
                    new LandTaal { LandCode = "NL", TaalCode = "nl" }
                 );

            }
        }
    }
}
