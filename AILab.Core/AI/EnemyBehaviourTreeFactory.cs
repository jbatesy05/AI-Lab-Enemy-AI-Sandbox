using System.Numerics;
using AILab.Core.BehaviourTree;
using AILab.Core.BehaviourTree.Nodes;
using AILab.Core.Simulation;

namespace AILab.Core.AI
{
    public static class EnemyBehaviourTreeFactory
    {
        public static Node Create(EnemySimulation simulation)
        {
            Vector2 patrolOrigin = simulation.Enemy.Position;

            Vector2[] patrolPoints =
            {
            patrolOrigin + new Vector2(-3, 0),
            patrolOrigin + new Vector2(3, 0)
            };

            simulation.PatrolPoints = patrolPoints;

            EnemyActions actions = new EnemyActions(
                simulation,
                patrolPoints
            );
            ConditionNode canAttack = new ConditionNode(
                "CanAttack",
                board => board.GetValue<bool>("PlayerInAttackRange")
            );

            ActionNode attack = new ActionNode(
                "AttackPlayer",
                board => actions.Attack()
            );

            SequenceNode attackSequence = new SequenceNode(
                "AttackSequence",
                canAttack,
                attack
            );

            ConditionNode canSeePlayer = new ConditionNode(
                "CanSeePlayer",
                board => board.GetValue<bool>("PlayerVisible")
            );

            ActionNode chase = new ActionNode(
                "ChasePlayer",
                board => actions.Chase()
            );

            SequenceNode chaseSequence = new SequenceNode(
                "ChaseSequence",
                canSeePlayer,
                chase
            );
            ConditionNode hasLastKnownPosition = new ConditionNode(
                 "HasLastKnownPosition",
                  board => board.GetValue<bool>("HasLastKnownPosition")
            );

            ActionNode investigate = new ActionNode(
                "InvestigateLastPosition",
                board => actions.Investigate()
            );

            SequenceNode investigateSequence = new SequenceNode(
                "InvestigateSequence",
                hasLastKnownPosition,
                investigate
            );

            ActionNode patrol = new ActionNode(
                "Patrol",
                board => actions.Patrol()
            );

            return new SelectorNode(
                "EnemyBehaviour",
                attackSequence,
                chaseSequence,
                investigateSequence,
                patrol
            );
        }
    }
}