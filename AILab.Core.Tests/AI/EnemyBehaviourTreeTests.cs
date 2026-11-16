using System.Numerics;
using AILab.Core.AI;
using AILab.Core.BehaviourTree;
using AILab.Core.Simulation;
using AILab.Core.Simulation.Entities;
using Xunit;

namespace AILab.Core.Tests.AI
{
    public class EnemyBehaviourTreeTests
    {
        [Fact]
        public void Tick_ChasesPlayer_WhenPlayerIsVisible()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f);

            Player player = new Player(
                new Vector2(8, 0));

            EnemySimulation simulation =
                new EnemySimulation(enemy, player);

            Node behaviourTree =
                EnemyBehaviourTreeFactory.Create(simulation);

            // Act
            simulation.UpdateBlackboard(1f);

            NodeStatus result =
                behaviourTree.Tick(simulation.Blackboard);

            // Assert
            Assert.Equal(NodeStatus.Running, result);
            Assert.Equal(new Vector2(2, 0), enemy.Position);
        }

        [Fact]
        public void Tick_UsesPatrol_WhenPlayerIsNotVisible()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 5f, 2f);

            Player player = new Player(
                new Vector2(20, 0));

            EnemySimulation simulation =
                new EnemySimulation(enemy, player);

            Node behaviourTree =
                EnemyBehaviourTreeFactory.Create(simulation);

            // Act
            simulation.UpdateBlackboard(1f);

            NodeStatus result =
                behaviourTree.Tick(simulation.Blackboard);

            // Assert
            Assert.Equal(NodeStatus.Running, result);
            Assert.Equal(new Vector2(-2, 0), enemy.Position);
        }
        [Fact]
        public void Tick_AttacksPlayer_WhenPlayerIsInAttackRange()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f);

            Player player = new Player(
                new Vector2(1, 0));

            EnemySimulation simulation =
                new EnemySimulation(enemy, player);

            Node behaviourTree =
                EnemyBehaviourTreeFactory.Create(simulation);

            // Act
            simulation.UpdateBlackboard(1f);

            NodeStatus result =
                behaviourTree.Tick(simulation.Blackboard);

            // Assert
            Assert.Equal(NodeStatus.Success, result);

            Assert.True(
                simulation.Blackboard.GetValue<bool>("IsAttacking")
            );
        }
    }
}