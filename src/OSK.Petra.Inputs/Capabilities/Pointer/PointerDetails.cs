using OSK.Petra.Inputs.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OSK.Petra.Inputs.Capabilities.Pointer;

public class PointerDetails(Vector2 startPosition, int maxRecords, float moveThreshold) : ICapabilityDetails
{
    #region Variables

    public Vector2 StartPosition { get; } = startPosition;

    public PointerMovement Movement { get; private set; } = PointerMovement.Idle;

    public Vector2 CurrentPosition { get; private set; } = startPosition;

    public Vector2 Velocity { get; private set; }

    public Vector2 Acceleration { get; private set; }

    private readonly Queue<PointerRecord> _records = [];
    private TimeSpan _lastTimeRecorded = TimeSpan.Zero;
    private Vector2 _lastVelocityRecorded = Vector2.Zero;

    #endregion

    #region Api

    internal void UpdatePosition(Vector2 position, TimeSpan time)
    {
        if ((CurrentPosition - position).LengthSquared() < moveThreshold)
        {
            Movement = Movement is PointerMovement.Start || Movement is PointerMovement.Active
                ? PointerMovement.Stop
                : PointerMovement.Idle;

            return;
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

        CurrentPosition = position;
        Movement = Movement is PointerMovement.Idle || Movement is PointerMovement.Stop
            ? PointerMovement.Start
            : PointerMovement.Active;
    }

    #endregion
}
