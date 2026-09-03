using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class DefaultCatalogTests
    {
        [Test]
        public void PlayerDecks_HaveNineCards()
        {
            Assert.AreEqual(9, DefaultCatalog.Equilibrado.Count);
            Assert.AreEqual(9, DefaultCatalog.Agressivo.Count);
            Assert.AreEqual(9, DefaultCatalog.Contrario.Count);
            Assert.AreEqual(3, DefaultCatalog.Decks.Count);
        }

        [Test]
        public void Enemies_HaveFiveScriptedCards()
        {
            Assert.AreEqual(5, DefaultCatalog.EstatuaDePedra.Count);
            Assert.AreEqual(5, DefaultCatalog.Mumia.Count);
            Assert.AreEqual(5, DefaultCatalog.Pirata.Count);
            Assert.AreEqual(3, DefaultCatalog.Enemies.Count);
        }

        [Test]
        public void Characters_ArePistoleiroAndThreeParkStatues()
        {
            Assert.AreEqual("Pistoleiro", DefaultCatalog.PlayerName);
            Assert.AreEqual("Estátua de Pedra", DefaultCatalog.Enemies[0].Name);
            Assert.AreEqual(EnemyBehavior.Defensive, DefaultCatalog.Enemies[0].Behavior);
            Assert.AreEqual("Múmia", DefaultCatalog.Enemies[1].Name);
            Assert.AreEqual(EnemyBehavior.Defensive, DefaultCatalog.Enemies[1].Behavior);
            Assert.AreEqual("Pirata", DefaultCatalog.Enemies[2].Name);
            Assert.AreEqual(EnemyBehavior.Aggressive, DefaultCatalog.Enemies[2].Behavior);
        }
    }
}
