using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class DefaultCatalogTests
    {
        [Test]
        public void PlayerAndEnemy_ShareTheSameNineCardBaseDeck()
        {
            Assert.AreEqual(DefaultCatalog.BaseDeckSize, DefaultCatalog.BaseDeck.Count);
            Assert.AreEqual(1, DefaultCatalog.Decks.Count);
            Assert.AreEqual("Baralho Base", DefaultCatalog.Decks[0].Name);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, DefaultCatalog.Decks[0].Cards);

            Assert.AreEqual(new Card(Suit.Rock, 1), DefaultCatalog.BaseDeck[0]);
            Assert.AreEqual(new Card(Suit.Rock, 2), DefaultCatalog.BaseDeck[1]);
            Assert.AreEqual(new Card(Suit.Rock, 3), DefaultCatalog.BaseDeck[2]);
            Assert.AreEqual(new Card(Suit.Paper, 1), DefaultCatalog.BaseDeck[3]);
            Assert.AreEqual(new Card(Suit.Paper, 2), DefaultCatalog.BaseDeck[4]);
            Assert.AreEqual(new Card(Suit.Paper, 3), DefaultCatalog.BaseDeck[5]);
            Assert.AreEqual(new Card(Suit.Scissors, 1), DefaultCatalog.BaseDeck[6]);
            Assert.AreEqual(new Card(Suit.Scissors, 2), DefaultCatalog.BaseDeck[7]);
            Assert.AreEqual(new Card(Suit.Scissors, 3), DefaultCatalog.BaseDeck[8]);
        }

        [Test]
        public void Enemies_UseTheSameNineCardBaseDeck()
        {
            Assert.AreEqual(3, DefaultCatalog.Enemies.Count);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, DefaultCatalog.Enemies[0].Sequence);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, DefaultCatalog.Enemies[1].Sequence);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, DefaultCatalog.Enemies[2].Sequence);
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
