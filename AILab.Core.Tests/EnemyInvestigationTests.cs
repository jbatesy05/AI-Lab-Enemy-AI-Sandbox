
using System.Numerics;
using AILab.Core.Simulation;
using AILab.Core.Simulation.Entities;
using Xunit;

namespace AILab.Core.Tests
{
    public class EnemyInvestigationTests
    {
        [Fact]
        public void Enemy_RemembersPlayersLastVisiblePosition()
        {
            Player player = new Player(new Vector2(2, 0));
            Enemy enemy = new Enemy(
                Vector2.Zero, 2f, 5f, 1f
            );

            EnemySimulation simulation =
                new EnemySimulation(enemy, player);

            EnemyAgent agent = new EnemyAgent(simulation);

            agent.Update(0.1f);

            Assert.True(
                simulation.Blackboard.GetValue<bool>(
                    "HasLastKnownPosition"
                )
            );

            Assert.Equal(
                player.Position,
                simulation.Blackboard.GetValue<Vector2>(
                    "LastKnownPlayerPosition"
                )
            );
        }

        [Fact]
        public void Enemy_InvestigatesLastKnownPosition()
        {
            Player player = new Player(new Vector2(2, 0));
            Enemy enemy = new Enemy(
                Vector2.Zero, 2f, 5f, 1f
            );

            EnemySimulation simulation =
                new EnemySimulation(enemy, player);

            EnemyAgent agent = new EnemyAgent(simulation);

            agent.Update(0.1f);

            Vector2 lastKnownPosition = new Vector2(2, 0);
            float originalDistance =
                Vector2.Distance(enemy.Position, lastKnownPosition);

            player.Position = new Vector2(20, 0);

            agent.Update(0.1f);

            float newDistance =
                Vector2.Distance(enemy.Position, lastKnownPosition);

            Assert.True(newDistance < originalDistance);
        }

        [Fact]
        public void Enemy_ClearsLastKnownPositionAfterInvestigating()
        {
            Player player = new Player(new Vector2(2, 0));
            Enemy enemy = new Enemy(
                Vector2.Zero, 2f, 5f, 1f
            );

            EnemySimulation simulation =
                new EnemySimulation(enemy, player);

            EnemyAgent agent = new EnemyAgent(simulation);

            agent.Update(0.1f);

            enemy.Position = new Vector2(2, 0);
            player.Position = new Vector2(20, 0);

            agent.Update(0.1f);

            Assert.False(
                simulation.Blackboard.GetValue<bool>(
                    "HasLastKnownPosition"
                )
            );
        }
    }
}
