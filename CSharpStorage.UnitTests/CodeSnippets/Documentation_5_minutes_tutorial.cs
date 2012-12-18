﻿using System;
using NDatabase.UnitTests.CodeSnippets.Data;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;

namespace NDatabase.UnitTests.CodeSnippets
{
    public class Documentation_5_minutes_tutorial
    {
        public const string TutorialDb5MinName = "tutorial.5min.db";

        [SetUp]
        public void SetUp()
        {
            OdbFactory.Delete(TutorialDb5MinName);
        }

        [Test]
        public void Snippet_for_tutorial()
        {
            Step1();
            Step2();
            Step3();
            Step4();
            Step5();
            Step6();
            Step7();
            Step8();
            Step9();
            Step10();
        }

        private static void Step1()
        {
            var sport = new Sport("volley-ball");

            using (var odb = OdbFactory.Open(TutorialDb5MinName))
                odb.Store(sport);
        }

        private static void Step2()
        {
            // Create instance
            var volleyball = new Sport("volley-ball");

            // Create 4 players
            var player1 = new Player("julia", DateTime.Now, volleyball);
            var player2 = new Player("magdalena", DateTime.Now, volleyball);
            var player3 = new Player("jacek", DateTime.Now, volleyball);
            var player4 = new Player("michal", DateTime.Now, volleyball);

            // Create two teams
            var team1 = new Team("Krakow");
            var team2 = new Team("Skawina");

            // Set players for team1
            team1.AddPlayer(player1);
            team1.AddPlayer(player2);

            // Set players for team2
            team2.AddPlayer(player3);
            team2.AddPlayer(player4);

            // Then create a volley ball game for the two teams
            var game = new Game(DateTime.Now, volleyball, team1, team2);

            using (var odb = OdbFactory.Open(TutorialDb5MinName))
                odb.Store(game);
        }

        private static void Step3()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                IQuery query = odb.Query<Player>();
                query.Descend("Name").Equal("julia");
                
                var players = query.Execute<Player>();
                
                Console.WriteLine("\nStep 3 : Players with name julia");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(1));
            }
        }

//        private static void Step3Linq()
//        {
//            using (var odb = OdbFactory.Open(TutorialDb5MinName))
//            {
//                var players = odb.CriteriaQuery<Player>().Where(player => player.Name.Equals("julia"));

//                Console.WriteLine("\nStep 3 : Players with name julia");

//                foreach (var player in players)
//                    Console.WriteLine("\t{0}", player);

//                Assert.That(players, Has.Count.EqualTo(1));
//            }
//        }


        private static void Step4()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                var agassi = new Player("André Agassi", DateTime.Now, new Sport("Tennis"));
                odb.Store(agassi);

                IQuery query = odb.Query<Player>();
                query.Descend("FavoriteSport._name").Equal("volley-ball");
//                query.Descend("FavoriteSport").Descend("_name").Constrain("volley-ball").Equal();

                var players = query.Execute<Player>();

                Console.WriteLine("\nStep 4 : Players of Voller-ball");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(4));
            }
        }

        private static void Step5()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                // retrieve the volley ball sport object
                IQuery query = odb.Query<Sport>();
                query.Descend("_name").Equal("volley-ball");
                var volleyBall = query.Execute<Sport>().GetFirst();
 
                Assert.That(volleyBall.Name, Is.EqualTo("volley-ball"));

                // Now build a query to get all players that play volley ball, using
                // the volley ball object
                query = odb.Query<Player>();
                query.Descend("FavoriteSport").Equal(volleyBall);
 
                var players = query.Execute<Player>();

                Console.WriteLine("\nStep 5: Players of Voller-ball");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(4));
            }
        }

        private static void Step6()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                IQuery query =
                    odb.Query<Player>();

                query.Descend("FavoriteSport._name").Equal("volley-ball").Or(
                    query.Descend("FavoriteSport._name").Like("%nnis"));

                var players = query.Execute<Player>();

                Console.WriteLine("\nStep 6 : Volley-ball and Tennis Players");
 
                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(5));
            }
        }

        private static void Step7()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                IQuery query = odb.Query<Player>();
                query.Descend("FavoriteSport._name").Equal("volley-ball").Not();
 
                var players = query.Execute<Player>();
 
                Console.WriteLine("\nStep 7 : Players that don't play Volley-ball");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(1));
            }
        }

        private static void Step8()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                IQuery query = odb.Query<Player>();
                query.Descend("FavoriteSport._name").InvariantLike("volley%");
 
                var players = query.Execute<Player>();
 
                Console.WriteLine("\nStep 8 bis: Players that play Volley-ball");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(4));
            }
        }

        private static void Step9()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                IQuery query = odb.Query<Player>();
                query.Descend("Name").Equal("magdalena");
                var players = query.Execute<Player>();

                var magdalena = players.GetFirst();
 
                // builds a query to get all teams where mihn plays
                query = odb.Query<Team>();
                query.Descend("Players").Contain(magdalena);
                
                var teams = query.Execute<Team>();

                Console.WriteLine("\nStep 9: Team where magdalena plays");

                foreach (var team in teams)
                    Console.WriteLine("\t{0}", team);

                Assert.That(teams, Has.Count.EqualTo(1));
            }
        }

        private static void Step10()
        {
            using (var odb = OdbFactory.Open(TutorialDb5MinName))
            {
                IQuery query = odb.Query<Player>();
                query.OrderByAsc("Name");

                var players = query.Execute<Player>();

                Console.WriteLine("\nStep 10: Players ordered by name asc");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(5));

                query.OrderByDesc("Name");
                players = query.Execute<Player>();

                Console.WriteLine("\nStep 10: Players ordered by name desc");

                foreach (var player in players)
                    Console.WriteLine("\t{0}", player);

                Assert.That(players, Has.Count.EqualTo(5));
            }
        }
    }
}
