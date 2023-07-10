using UnityEngine;
/// <summary>
/// This class calculates the velocity used to move a player in seperate steps in contrast to one singular movement all the way
/// </summary>
public class HighSpeedMovement
{
    /// <summary>
    /// Calculates the players potential position in 16 by 16 steps
    /// This is done by calculating the true distance the players needs to be pushed to a singular value rather than a vector
    /// By attaining this singular value it can be then modified to match the terrain allowing it to be dynamic and adjust
    /// <param name="highSpeedData">The data of references the players speed </param>
    /// <param name="player">The player object being moved</param>
    /// </summary>
    public Vector2 CalculateHighSpeedMovement(HighSpeedData highSpeedData, Player player)
    {
        float previousMoveDistance = highSpeedData.currentMoveDistance;
        highSpeedData.currentMoveDistance = Mathf.MoveTowards(highSpeedData.currentMoveDistance, 0, 16);
        highSpeedData.newVelocity = highSpeedData.velocityDirection;
        highSpeedData.newVelocity.x *= previousMoveDistance - highSpeedData.currentMoveDistance;
        highSpeedData.newVelocity.y *= previousMoveDistance - highSpeedData.currentMoveDistance;
        highSpeedData.calculatedVelocity = previousMoveDistance - highSpeedData.currentMoveDistance;

        return this.PredictHighSpeedVelocity(highSpeedData, player);
    }

    /// <summary>
    /// Predicts the players velocity based on the players state before moving ,while moving and the terrain below when grounded  at high speeds
    /// <param name="highSpeedData">The data of references the players speed </param>
    /// <param name="player">The player object being moved</param>
    /// </summary>
    public Vector2 PredictHighSpeedVelocity(HighSpeedData highSpeedData, Player player)
    {
        if (player.GetGrounded() && highSpeedData.delta > 0)//In the first step do not consider terrain below
        {
            if (highSpeedData.calculatedVelocity < 0)
            {
                Debug.Break();
            }
            Vector2 velocity = player.CalculateSlopeMovement(highSpeedData.calculatedVelocity) * Mathf.Sign(player.groundVelocity);
            highSpeedData.velocityDirection = (Vector2.zero + (velocity * 60 * Time.fixedDeltaTime)).normalized;

            return velocity;
        }

        return highSpeedData.newVelocity;
    }

    public class HighSpeedData
    {
        /// The initial velocity when launching high speed movement
        public Vector2 initialVelocity;
        /// The target distance
        public float targetDistance;
        /// The amount of iterations taken place
        public int iterations;
        /// The Direction of the players velocity
        public Vector2 velocityDirection;
        /// The current distance moved by
        public float currentMoveDistance;
        /// Determines whether the high speed data started while grunded
        public bool startedGrounded;
        /// The previous velocity for debugging purposes
        public Vector2 oldVelocity;
        /// The new velocity calculated
        public Vector2 newVelocity;
        public float calculatedVelocity;
        /// The angle before the player got ungrounded 
        public float angleBeforeUnground;
        /// The current delta of the high speed data
        public int delta;
    }
}

