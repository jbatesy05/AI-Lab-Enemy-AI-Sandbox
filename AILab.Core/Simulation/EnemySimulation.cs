using AILab.Core.BehaviourTree;
using AILab.Core.Simulation.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AILab.Core.Simulation
{
    public class EnemySimulation
    {
        public IReadOnlyList<Vector2> PatrolPoints { get; internal set; }
    = Array.Empty<Vector2>();
        public Enemy Enemy { get; }

        public Player Player { get; }

        public Blackboard Blackboard { get; }

        public EnemySimulation(Enemy enemy, Player player)
        {
            Enemy = enemy;
            Player = player;

            Blackboard = new Blackboard();
        }

        public void UpdateBlackboard(float deltaTime)
        {
            bool playerVisible = Enemy.CanSeePlayer(Player);

            Blackboard.SetValue("PlayerVisible", playerVisible);
            Blackboard.SetValue(
                "PlayerInAttackRange",
                Enemy.IsPlayerInAttackRange(Player)
            );

            Blackboard.SetValue("DeltaTime", deltaTime);
            Blackboard.SetValue("IsAttacking", false);

            if (playerVisible)
            {
                Blackboard.SetValue(
                    "LastKnownPlayerPosition",
                    Player.Position
                );

                Blackboard.SetValue("HasLastKnownPosition", true);
            }
        }
    }
}