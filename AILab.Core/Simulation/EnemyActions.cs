using System;
using System.Numerics;
using AILab.Core.BehaviourTree;

namespace AILab.Core.Simulation
{
    public class EnemyActions
    {
        private readonly EnemySimulation _simulation;
        private readonly Vector2[] _patrolPoints;
        private int _currentPatrolPoint;

        public EnemyActions(
            EnemySimulation simulation,
            params Vector2[] patrolPoints)
        {
            ArgumentNullException.ThrowIfNull(simulation);
            ArgumentNullException.ThrowIfNull(patrolPoints);

            if (patrolPoints.Length == 0)
            {
                throw new ArgumentException(
                    "At least one patrol point is required.",
                    nameof(patrolPoints)
                );
            }

            _simulation = simulation;
            _patrolPoints = (Vector2[])patrolPoints.Clone();
        }
        public NodeStatus Attack()
        {
            if (!_simulation.Enemy.IsPlayerInAttackRange(_simulation.Player))
            {
                return NodeStatus.Failure;
            }

            _simulation.Blackboard.SetValue(
                "CurrentBehaviour", "Attacking"
            );

            _simulation.Blackboard.SetValue("IsAttacking", true);

            return NodeStatus.Success;
        }
        public NodeStatus Chase()
        {
            _simulation.Blackboard.SetValue("CurrentBehaviour", "Chasing");

            Vector2 avoidanceDirection =
                _simulation.Blackboard.GetValue<Vector2>(
                    "SeparationDirection"
                );

            bool reachedPlayer =
                _simulation.Enemy.MoveTowardsWithAvoidance(
                    _simulation.Player.Position,
                    _simulation.Blackboard.GetValue<float>("DeltaTime"),
                    _simulation.Enemy.AttackRange,
                    avoidanceDirection
                );

            return reachedPlayer
                ? NodeStatus.Success
                : NodeStatus.Running;
        }
        public NodeStatus Patrol()
        {
            _simulation.Blackboard.SetValue(
             "CurrentBehaviour", "Patrolling"
            );
            Vector2 target = _patrolPoints[_currentPatrolPoint];

            bool reachedPoint = _simulation.Enemy.MoveTowards(
                target,
                _simulation.Blackboard.GetValue<float>("DeltaTime"),
                0.1f
            );

            if (reachedPoint)
            {
                _currentPatrolPoint =
                    (_currentPatrolPoint + 1) % _patrolPoints.Length;
            }

            return NodeStatus.Running;
        }
        public NodeStatus Investigate()
        {
            _simulation.Blackboard.SetValue(
                "CurrentBehaviour",
                "Investigating"
            );

            Vector2 lastKnownPosition =
                _simulation.Blackboard.GetValue<Vector2>(
                    "LastKnownPlayerPosition"
                );

            Vector2 avoidanceDirection =
                _simulation.Blackboard.GetValue<Vector2>(
                    "SeparationDirection"
                );

            bool reachedPosition =
                _simulation.Enemy.MoveTowardsWithAvoidance(
                    lastKnownPosition,
                    _simulation.Blackboard.GetValue<float>("DeltaTime"),
                    0.3f,
                    avoidanceDirection
                );

            if (reachedPosition)
            {
                _simulation.Blackboard.SetValue(
                    "HasLastKnownPosition",
                    false
                );

                return NodeStatus.Success;
            }

            return NodeStatus.Running;
        }
    }
}