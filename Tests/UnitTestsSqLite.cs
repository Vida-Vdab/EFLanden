using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Model.Repositories;
using Model.Entities;
using System;
using Model.Services;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Tests
{
    [TestClass]
    public class UnitTestsSqLite
    {
        SqliteConnection connection;
        DbContextOptions<EFLandenContext> options;

        [TestInitialize]
        public void Initializer()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };

            connection = new SqliteConnection(connectionStringBuilder.ToString());
            connection.Open();

            options = new DbContextOptionsBuilder<EFLandenContext>()
                   .UseSqlite(connection)
                   .Options;

            using var context = new EFLandenContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [TestMethod]
        public void GetStedenVoorLand_Steden_AantalIsDrieSteden()
        {
            // Arrange
            using var context = new EFLandenContext(options);

            //Toevoegen Landen
            context.Landen.Add(new Land() { ISOLandCode = "BE", NISLandCode = "150", Naam = "België", AantalInwoners = 11500000, Oppervlakte = 30689f });

            //Toevoegen Steden
            context.Steden.Add(new Stad() { StadId = 1, Naam = "Brussel", ISOLandCode = "BE" });
            context.Steden.Add(new Stad() { StadId = 2, Naam = "Antwerepen", ISOLandCode = "BE" });
            context.Steden.Add(new Stad() { StadId = 3, Naam = "Luik", ISOLandCode = "BE" });

            context.SaveChanges();

            var landService = new LandService(context);

            // Act
            var steden = landService.GetStedenVoorLand("BE");

            // Assert
            Assert.AreEqual(3, steden.Count());
        }

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void GetSteden_Stad0_ThrowArgumentException()
        {
            // Arrange   
            using var context = new EFLandenContext(options);

            context.Landen.Add(new Land() { ISOLandCode = "BE", NISLandCode = "150", Naam = "België", AantalInwoners = 11500000, Oppervlakte = 30689f });
            context.SaveChanges();

            context.Steden.Add(new Stad() { Naam = "Leuven", ISOLandCode = "BE" });
            context.SaveChanges();

            // Act
            var landService = new LandService(context);
            landService.GetSteden(0);
        }

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void ToevoegenStad_StadZonderLandcode_StadHeeftLandcodeBE()
        {
            // Arrange
            using var context = new EFLandenContext(options);

            var stad = new Stad() { StadId = 4, Naam = "Leuven"};
            context.SaveChanges();
            // Act    
            var landService = new LandService(context);
            landService.ToevoegenStad(stad);
            context.SaveChanges();

            // Assert
            var stad1 = landService.GetSteden(4);
            Assert.AreEqual("Be", stad1.ISOLandCode);
        }

        [TestMethod]
        public void GetTaalEnLandVanLandTaal_TalenEnLanden_AantalIsEenTaalEnTweeLanden()
        {
            // Arrange
            using var context = new EFLandenContext(options);

            //Toevoegen Landen
            context.Landen.Add(new Land() { ISOLandCode = "BE", NISLandCode = "150", Naam = "België", AantalInwoners = 11500000, Oppervlakte = 30689f });
           
            //Toevoegen Talen
            context.Talen.Add(new Taal() { ISOTaalCode = "fr", NaamNL = "Frans", NaamTaal = "français" });
            context.Talen.Add(new Taal() { ISOTaalCode = "nl", NaamNL = "Nederlands", NaamTaal = "Nederlands" });
            context.SaveChanges();

            //Toevoegen LandenTalen
            context.LandenTalen.Add(new LandTaal() { LandCode = "BE", TaalCode = "fr" });
            context.LandenTalen.Add(new LandTaal() { LandCode = "BE", TaalCode = "nl" });
            context.SaveChanges();

            //Act
            var landService = new LandService(context);
            var talen = landService.GetTaalVanLandTaal("nl");
            var landen = landService.GetLandVanLandTaal("BE");

            //Assert
            Assert.AreEqual(1, talen.Count());
            Assert.AreEqual(2, landen.Count());
        }
    }
}
