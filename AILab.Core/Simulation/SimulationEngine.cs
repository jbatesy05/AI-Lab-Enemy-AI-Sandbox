
using AILab.Core.BehaviourTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AILab.Core.Simulation
{
    public class SimulationEngine
    {
        private readonly List<EnemyAgent> _agents = new();
        private float _simulationSpeed = 1f;

        public IReadOnlyList<EnemyAgent> Agents => _agents;

        public EnemySimulation Simulation => _agents[0].Simulation;

        public Node BehaviourTree => _agents[0].BehaviourTree;

        public NodeStatus CurrentStatus => _agents[0].CurrentStatus;

        public bool IsPaused { get; set; }

        public float SimulationSpeed
        {
            get => _simulationSpeed;

            set
            {
                if (value <= 0 || !float.IsFinite(value))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "Simulation speed must be greater than zero."
                    );
                }

                _simulationSpeed = value;
            }
        }

        public SimulationEngine(
            EnemySimulation simulation,
            string? name = null)
        {
            AddEnemy(simulation, name);
        }
        public EnemyAgent AddEnemy(
            EnemySimulation simulation,
            string? name = null)
        {
            ArgumentNullException.ThrowIfNull(simulation);

            if (_agents.Any(agent =>
                ReferenceEquals(agent.Simulation, simulation)))
            {
                throw new ArgumentException(
                    "This enemy simulation has already been added.",
                    nameof(simulation)
                );
            }

            EnemyAgent agent = new EnemyAgent(simulation, name);

            _agents.Add(agent);

            return agent;
        }
        public bool RemoveEnemy(Guid enemyId)
        {
            if (_agents.Count <= 1)
            {
                return false;
            }

            EnemyAgent? agent = _agents.FirstOrDefault(
                item => item.Id == enemyId
            );

            if (agent == null)
            {
                return false;
            }

            return _agents.Remove(agent);
        }
        public NodeStatus Update(float deltaTime)
        {
            if (deltaTime < 0 || !float.IsFinite(deltaTime))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            if (IsPaused)
            {
                return CurrentStatus;
            }

            float scaledDeltaTime = deltaTime * SimulationSpeed;

            foreach (EnemyAgent agent in _agents)
            {
                Vector2 separation = Vector2.Zero;

                foreach (EnemyAgent other in _agents)
                {
                    if (agent == other)
                        continue;

                    Vector2 difference =
                        agent.Simulation.Enemy.Position -
                        other.Simulation.Enemy.Position;

                    float distance = difference.Length();

                    if (distance > 0.001f && distance < 2.5f)
                    {
                        separation += Vector2.Normalize(difference) / distance;
                    }
                }

                agent.Simulation.Blackboard.SetValue(
                    "SeparationDirection",
                    separation
                );

                agent.Update(scaledDeltaTime);
            }

            return CurrentStatus;
        }
    }
}
