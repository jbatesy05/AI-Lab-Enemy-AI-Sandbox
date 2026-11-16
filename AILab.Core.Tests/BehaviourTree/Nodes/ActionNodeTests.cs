using AILab.Core.BehaviourTree;
using AILab.Core.BehaviourTree.Nodes;
using Xunit;

namespace AILab.Core.Tests.BehaviourTree.Nodes
{
    public class ActionNodeTests
    {
        [Fact]
        public void Tick_ReturnsSuccess_WhenActionSucceeds()
        {
            // Arrange
            Blackboard blackboard = new Blackboard();

            ActionNode node = new ActionNode(
                "ChasePlayer",
                board => NodeStatus.Success
            );

            // Act
            NodeStatus result = node.Tick(blackboard);

            // Assert
            Assert.Equal(NodeStatus.Success, result);
        }

        [Fact]
        public void Tick_ReturnsFailure_WhenActionFails()
        {
            // Arrange
            Blackboard blackboard = new Blackboard();

            ActionNode node = new ActionNode(
                "ChasePlayer",
                board => NodeStatus.Failure
            );

            // Act
            NodeStatus result = node.Tick(blackboard);

            // Assert
            Assert.Equal(NodeStatus.Failure, result);
        }

        [Fact]
        public void Tick_ReturnsRunning_WhenActionIsInProgress()
        {
            // Arrange
            Blackboard blackboard = new Blackboard();

            ActionNode node = new ActionNode(
                "ChasePlayer",
                board => NodeStatus.Running
            );

            // Act
            NodeStatus result = node.Tick(blackboard);

            // Assert
            Assert.Equal(NodeStatus.Running, result);
        }
    }
}