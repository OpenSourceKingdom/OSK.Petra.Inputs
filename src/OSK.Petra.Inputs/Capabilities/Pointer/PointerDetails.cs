using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

/// <summary>
/// Describes pointer specific information
/// </summary>
/// <param name="startPosition">The starting position for the pointer</param>
/// <param name="maxRecords">The total number of records for pointer tracking</param>
/// <param name="distanceThreshold">the amount of distance a pointer must move to be considered a full, intentional movement</param>
public class PointerDetails(Vector2 startPosition, int maxRecords, float distanceThreshold) : ICapabilityDetails
{
    #region Variables

    /// <summary>
    /// The pointers original starting position when engaged
    /// </summary>
    public Vector2 StartPosition { get; } = startPosition;

    /// <summary>
    /// The current pointer movement
    /// </summary>
    public PointerMovement Movement { get; private set; } = PointerMovement.Idle;

    /// <summary>
    /// The current position for the pointer
    /// </summary>
    public Vector2 CurrentPosition { get; private set; } = startPosition;

    /// <summary>
    /// The move velocity for the pointer
    /// </summary>
    public Vector2 Velocity { get; private set; }

    /// <summary>
    /// The acceleration for the pointer
    /// </summary>
    public Vector2 Acceleration { get; private set; }

    private readonly Queue<PointerRecord> _records = [];
    private TimeSpan _lastTimeRecorded = TimeSpan.Zero;
    private Vector2 _lastVelocityRecorded = Vector2.Zero;

    #endregion

    #region Api

    internal bool UpdatePosition(Vector2 position, TimeSpan time)
    {
        if ((CurrentPosition - position).LengthSquared() < distanceThreshold)
        {
            Movement = Movement is PointerMovement.Start || Movement is PointerMovement.Active
                ? PointerMovement.Stop
                : PointerMovement.Idle;

            return false;
        }

        if (maxRecords > 0)
        {
            var deltaTime = (float)(time - _lastTimeRecorded).TotalSeconds;
            Velocity = deltaTime > 0
                ? (position - CurrentPosition) / deltaTime
                : Vector2.Zero;

            Acceleration = deltaTime > 0
                ? (Velocity - _lastVelocityRecorded) / deltaTime
                : Vector2.Zero;

            _lastTimeRecorded = time;
            _lastVelocityRecorded = Velocity;

            _records.Enqueue(new(position, time));
            if (_records.Count > maxRecords)
            {
                _records.Dequeue();
            }
        }

        return true;

        CurrentPosition = position;
        Movement = Movement is PointerMovement.Idle || Movement is PointerMovement.Stop
            ? PointerMovement.Start
            : PointerMovement.Active;
    }

    #endregion
}
