using System;
using System.Numerics;

namespace AILab.Core.Simulation.Entities
{
    public class Enemy
    {
        public Vector2 Position { get; set; }

        public float MovementSpeed { get; set; }

        public float DetectionRange { get; set; }

        public float AttackRange { get; set; }

        public Enemy(
            Vector2 position,
            float movementSpeed,
            float detectionRange,
            float attackRange)
        {
            if (movementSpeed <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(movementSpeed));
            }

            if (detectionRange <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(detectionRange));
            }

            if (attackRange <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackRange));
            }

            Position = position;
            MovementSpeed = movementSpeed;
            DetectionRange = detectionRange;
            AttackRange = attackRange;
        }

        public float GetDistanceTo(Player player)
        {
            return Vector2.Distance(Position, player.Position);
        }

        public bool CanSeePlayer(Player player)
        {
            return GetDistanceTo(player) <= DetectionRange;
        }

        public bool IsPlayerInAttackRange(Player player)
        {
            return GetDistanceTo(player) <= AttackRange;
        }
        public bool MoveTowards(Vector2 targetPosition, float deltaTime)
        {
            Vector2 direction = targetPosition - Position;

            float distance = direction.Length();

            if (distance <= 0.001f)
            {
                return true;
            }

            float movementAmount = MovementSpeed * deltaTime;

            if (movementAmount >= distance)
            {
                Position = targetPosition;
                return true;
            }

            Vector2 movementDirection = Vector2.Normalize(direction);

            Position += movementDirection * movementAmount;

            return false;
        }
        public bool MoveTowards(Vector2 targetPosition, float deltaTime, float stoppingDistance)
        {
            Vector2 direction = targetPosition - Position;

            float distance = direction.Length();

            if (distance <= stoppingDistance)
            {
                return true;
            }

            float movementAmount = MovementSpeed * deltaTime;

            if (movementAmount >= distance - stoppingDistance)
            {
                Position += Vector2.Normalize(direction) * (distance - stoppingDistance);
                return true;
            }

            Vector2 movementDirection = Vector2.Normalize(direction);

            Position += movementDirection * movementAmount;

            return false;
        }
        public bool MoveTowardsWithAvoidance(
    Vector2 targetPosition,
    float deltaTime,
    float stoppingDistance,
    Vector2 avoidanceDirection)
        {
            Vector2 direction = targetPosition - Position;
            float distance = direction.Length();

            if (distance <= stoppingDistance)
                return true;

            float movementAmount = Math.Min(
                MovementSpeed * deltaTime,
                distance - stoppingDistance
            );

            Vector2 movementDirection = Vector2.Normalize(direction);

            if (avoidanceDirection.LengthSquared() > 0.001f)
            {
                Vector2 avoidance = Vector2.Normalize(avoidanceDirection);

                movementDirection = Vector2.Normalize(
                    movementDirection + avoidance * 0.75f
                );
            }

            Position += movementDirection * movementAmount;

            return Vector2.Distance(Position, targetPosition) <= stoppingDistance;
        }
    }
}