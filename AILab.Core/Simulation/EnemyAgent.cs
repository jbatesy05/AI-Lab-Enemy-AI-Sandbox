
using System;
using AILab.Core.AI;
using AILab.Core.BehaviourTree;

namespace AILab.Core.Simulation
{
    public class EnemyAgent
    {
        public Guid Id { get; }

        public string Name { get; }

        public EnemySimulation Simulation { get; }

        public Node BehaviourTree { get; }

        public NodeStatus CurrentStatus { get; internal set; }

        public EnemyAgent(EnemySimulation simulation, string? name = null)
        {
            ArgumentNullException.ThrowIfNull(simulation);

            Id = Guid.NewGuid();

            Name = string.IsNullOrWhiteSpace(name)
                ? $"Enemy {Id.ToString("N")[..8]}"
                : name.Trim();

            Simulation = simulation;
            BehaviourTree = EnemyBehaviourTreeFactory.Create(simulation);

            CurrentStatus = NodeStatus.Running;
        }

        public NodeStatus Update(float deltaTime)
        {
            Simulation.UpdateBlackboard(deltaTime);

            CurrentStatus = BehaviourTree.Tick(
                Simulation.Blackboard
            );

            return CurrentStatus;
        }
    }
}
