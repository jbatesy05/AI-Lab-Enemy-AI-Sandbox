
using System.Numerics;
using AILab.Core.BehaviourTree;
using AILab.Core.Simulation;
using AILab.Core.Simulation.Entities;
using Xunit;

namespace AILab.Core.Tests.Simulation
{
    public class SimulationEngineTests
    {
        [Fact]
        public void Update_MovesEnemyTowardsVisiblePlayer()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(8, 0));

            EnemySimulation simulation = new EnemySimulation(
                enemy, player
            );

            SimulationEngine engine = new SimulationEngine(simulation);

            NodeStatus result = engine.Update(1f);

            Assert.Equal(NodeStatus.Running, result);
            Assert.Equal(new Vector2(2, 0), enemy.Position);
        }

        [Fact]
        public void Update_AttacksPlayerWhenInRange()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(1, 0));

            EnemySimulation simulation = new EnemySimulation(
                enemy, player
            );

            SimulationEngine engine = new SimulationEngine(simulation);

            NodeStatus result = engine.Update(1f);

            Assert.Equal(NodeStatus.Success, result);
            Assert.True(
                simulation.Blackboard.GetValue<bool>("IsAttacking")
            );
        }

        [Fact]
        public void Update_PatrolsWhenPlayerIsNotVisible()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 5f, 2f
            );

            Player player = new Player(new Vector2(20, 0));

            EnemySimulation simulation = new EnemySimulation(
                enemy, player
            );

            SimulationEngine engine = new SimulationEngine(simulation);

            NodeStatus result = engine.Update(1f);

            Assert.Equal(NodeStatus.Running, result);
            Assert.Equal(new Vector2(-2, 0), enemy.Position);
        }

        [Fact]
        public void Update_MovesFasterWhenSimulationSpeedIsIncreased()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(8, 0));

            EnemySimulation simulation = new EnemySimulation(enemy, player);
            SimulationEngine engine = new SimulationEngine(simulation);

            engine.SimulationSpeed = 2f;

            engine.Update(1f);

            Assert.Equal(new Vector2(4, 0), enemy.Position);
        }

        [Fact]
        public void Update_DoesNotMoveEnemyWhenPaused()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(8, 0));

            EnemySimulation simulation = new EnemySimulation(enemy, player);
            SimulationEngine engine = new SimulationEngine(simulation);

            engine.IsPaused = true;

            engine.Update(1f);

            Assert.Equal(new Vector2(0, 0), enemy.Position);
        }

        [Fact]
        public void SimulationSpeed_RejectsZero()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(8, 0));

            EnemySimulation simulation = new EnemySimulation(enemy, player);
            SimulationEngine engine = new SimulationEngine(simulation);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                engine.SimulationSpeed = 0f
            );
        }

        [Fact]
        public void AddEnemy_CreatesAdditionalAgent()
        {
            Enemy enemy1 = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(8, 0));

            EnemySimulation simulation1 = new EnemySimulation(
                enemy1, player
            );

            SimulationEngine engine = new SimulationEngine(simulation1);

            Enemy enemy2 = new Enemy(
                new Vector2(0, 5), 2f, 10f, 2f
            );

            Player player2 = new Player(new Vector2(8, 5));

            EnemySimulation simulation2 = new EnemySimulation(
                enemy2, player2
            );

            engine.AddEnemy(simulation2);

            Assert.Equal(2, engine.Agents.Count);
        }

        [Fact]
        public void Update_MovesMultipleEnemiesIndependently()
        {
            Player player = new Player(new Vector2(8, 0));

            Enemy enemy1 = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Enemy enemy2 = new Enemy(
                new Vector2(0, 5), 2f, 10f, 2f
            );

            EnemySimulation simulation1 = new EnemySimulation(
                enemy1, player
            );

            EnemySimulation simulation2 = new EnemySimulation(
                enemy2, player
            );

            SimulationEngine engine = new SimulationEngine(simulation1);

            engine.AddEnemy(simulation2);

            engine.Update(1f);

            Assert.Equal(new Vector2(2, 0), enemy1.Position);

            float enemy2Movement = Vector2.Distance(
                new Vector2(0, 5),
                enemy2.Position
            );

            Assert.Equal(2f, enemy2Movement, 0.001f);
        }

        [Fact]
        public void AddEnemy_AssignsUniqueIdAndName()
        {
            Player player = new Player(new Vector2(8, 0));

            Enemy enemy1 = new Enemy(new Vector2(0, 0), 2f, 10f, 2f);
            Enemy enemy2 = new Enemy(new Vector2(0, 5), 2f, 10f, 2f);

            EnemySimulation simulation1 = new EnemySimulation(enemy1, player);
            EnemySimulation simulation2 = new EnemySimulation(enemy2, player);

            SimulationEngine engine = new SimulationEngine(simulation1);

            EnemyAgent agent = engine.AddEnemy(simulation2, "Guard");

            Assert.Equal("Guard", agent.Name);
            Assert.NotEqual(Guid.Empty, agent.Id);
            Assert.Equal(2, engine.Agents.Count);
            Assert.NotEqual(engine.Agents[0].Id, agent.Id);
        }

        [Fact]
        public void RemoveEnemy_RemovesCorrectAgent()
        {
            Player player = new Player(new Vector2(8, 0));

            Enemy enemy1 = new Enemy(new Vector2(0, 0), 2f, 10f, 2f);
            Enemy enemy2 = new Enemy(new Vector2(0, 5), 2f, 10f, 2f);

            EnemySimulation simulation1 = new EnemySimulation(enemy1, player);
            EnemySimulation simulation2 = new EnemySimulation(enemy2, player);

            SimulationEngine engine = new SimulationEngine(simulation1);

            EnemyAgent firstAgent = engine.Agents[0];
            EnemyAgent secondAgent = engine.AddEnemy(simulation2, "Guard");

            bool removed = engine.RemoveEnemy(secondAgent.Id);

            Assert.True(removed);
            Assert.Single(engine.Agents);
            Assert.Same(firstAgent, engine.Agents[0]);
        }

        [Fact]
        public void RemoveEnemy_CannotRemoveFinalEnemy()
        {
            Player player = new Player(new Vector2(8, 0));

            Enemy enemy = new Enemy(new Vector2(0, 0), 2f, 10f, 2f);

            EnemySimulation simulation = new EnemySimulation(enemy, player);

            SimulationEngine engine = new SimulationEngine(simulation);

            bool removed = engine.RemoveEnemy(engine.Agents[0].Id);

            Assert.False(removed);
            Assert.Single(engine.Agents);
        }
        [Fact]
        public void AddEnemy_RejectsDuplicateSimulation()
        {
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f
            );

            Player player = new Player(new Vector2(8, 0));

            EnemySimulation simulation = new EnemySimulation(
                enemy, player
            );

            SimulationEngine engine = new SimulationEngine(simulation);

            Assert.Throws<ArgumentException>(() =>
                engine.AddEnemy(simulation)
            );
        }

    }
}
