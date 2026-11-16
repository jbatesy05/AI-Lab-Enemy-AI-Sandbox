
using System.Numerics;

namespace AILab.Core.Simulation.Entities
{
    public class Player
    {
        public Vector2 Position { get; set; }

        public float MovementSpeed { get; set; } = 5f;

        public Player(Vector2 position)
        {
            Position = position;
        }

        public bool MoveTowards(Vector2 targetPosition, float deltaTime)
        {
            Vector2 direction = targetPosition - Position;
            float distance = direction.Length();

            if (distance <= 0.001f)
            {
                Position = targetPosition;
                return true;
            }

            float movementAmount = MovementSpeed * deltaTime;

            if (movementAmount >= distance)
            {
                Position = targetPosition;
                return true;
            }

            Position += Vector2.Normalize(direction) * movementAmount;

            return false;
        }
    }
}
