using System.Numerics;
using AILab.Core.Simulation.Entities;
using Xunit;

namespace AILab.Core.Tests.Simulation.Entities
{
    public class EnemyTests
    {
        [Fact]
        public void GetDistanceTo_ReturnsCorrectDistance()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 3f, 10f, 2f);

            Player player = new Player(
                new Vector2(3, 4));

            // Act
            float distance = enemy.GetDistanceTo(player);

            // Assert
            Assert.Equal(5f, distance);
        }

        [Fact]
        public void CanSeePlayer_ReturnsTrue_WhenPlayerIsInRange()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 3f, 10f, 2f);

            Player player = new Player(
                new Vector2(3, 4));

            // Act
            bool result = enemy.CanSeePlayer(player);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSeePlayer_ReturnsFalse_WhenPlayerIsOutOfRange()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 3f, 5f, 2f);

            Player player = new Player(
                new Vector2(10, 10));

            // Act
            bool result = enemy.CanSeePlayer(player);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPlayerInAttackRange_ReturnsTrue_WhenPlayerIsCloseEnough()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 3f, 10f, 2f);

            Player player = new Player(
                new Vector2(1, 1));

            // Act
            bool result = enemy.IsPlayerInAttackRange(player);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void MoveTowards_UpdatesPosition()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 2f, 10f, 2f);

            // Act
            bool reachedTarget = enemy.MoveTowards(
                new Vector2(10, 0), 1f);

            // Assert
            Assert.Equal(new Vector2(2, 0), enemy.Position);
            Assert.False(reachedTarget);
        }

        [Fact]
        public void MoveTowards_ReturnsTrue_WhenTargetIsReached()
        {
            // Arrange
            Enemy enemy = new Enemy(
                new Vector2(0, 0), 5f, 10f, 2f);

            // Act
            bool reachedTarget = enemy.MoveTowards(
                new Vector2(3, 0), 1f);

            // Assert
            Assert.Equal(new Vector2(3, 0), enemy.Position);
            Assert.True(reachedTarget);
        }
    }
}