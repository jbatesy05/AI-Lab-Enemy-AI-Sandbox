using AILab.Core.BehaviourTree;
using AILab.Core.BehaviourTree.Nodes;
using Xunit;

namespace AILab.Core.Tests.BehaviourTree.Nodes
{
    public class ConditionNodeTests
    {
        [Fact]
        public void Tick_ReturnsSuccess_WhenConditionIsTrue()
        {
            // Arrange
            Blackboard blackboard = new Blackboard();

            ConditionNode node = new ConditionNode(
                "CanSeePlayer",
                board => true
            );

            // Act
            NodeStatus result = node.Tick(blackboard);

            // Assert
            Assert.Equal(NodeStatus.Success, result);
        }

        [Fact]
        public void Tick_ReturnsFailure_WhenConditionIsFalse()
        {
            // Arrange
            Blackboard blackboard = new Blackboard();

            ConditionNode node = new ConditionNode(
                "CanSeePlayer",
                board => false
            );

            // Act
            NodeStatus result = node.Tick(blackboard);

            // Assert
            Assert.Equal(NodeStatus.Failure, result);
        }
    }
}