using Avalonia;
using System;

namespace GravityBalls;

public class WorldModel
{
    public double BallX;
    public double BallY;
    public double BallRadius;
    public double WorldWidth;
    public double WorldHeight;

    public double VerticalSpeed = 200;
    public double HorizontalSpeed = 158;
    public double ResistanceCoefficient = .1;
    public double RepulsionIntensity = 100;
    public double JumpForce = 100;

    private const double MinimalSpeed = 5;
    private const double Gravity = 9.8;

    private void SimulateBallMovement (double dt)
    {
        VerticalSpeed -= VerticalSpeed * dt * ResistanceCoefficient;
        HorizontalSpeed -= HorizontalSpeed * dt * ResistanceCoefficient;

        if (Math.Abs(HorizontalSpeed) < MinimalSpeed) HorizontalSpeed = 0;

        BallY += VerticalSpeed * dt;
        BallX += HorizontalSpeed * dt;
    }

    private void SimulateGravity (double dt)
    {
        VerticalSpeed += Gravity * dt;
        BallY += VerticalSpeed * dt;
    }

    private void SimulateBallBouncedFromSideWalls (double dt)
    {
        if (BallX + BallRadius >= WorldWidth)
        {
            HorizontalSpeed = -HorizontalSpeed;

            if (BallX >= WorldWidth)
            {
                BallX = WorldWidth - BallRadius;
            }
        }

        if (BallX - BallRadius <= 0)
        {
            HorizontalSpeed = -HorizontalSpeed;

            if (BallX <= 0)
            {
                BallX = BallRadius;
            }
        }
    }

    private void SimulateBallBouncedFromVerticalWalls (double dt)
    {
        if (BallY + BallRadius >= WorldHeight)
        {
            VerticalSpeed = -VerticalSpeed;

            if (BallY >= WorldWidth)
            {
                BallY = WorldHeight - BallRadius;
            }
        }

        if (BallY - BallRadius <= 0)
        {
            VerticalSpeed = -VerticalSpeed;

            if (BallY <= 0)
            {
                BallY = BallRadius;
            }
        }
    }

    public void SimulateCursorRepulsion(Point cursorPosition)
    {
        double distanceToCursor = Math.Pow(Math.Pow(BallX - cursorPosition.X, 2) + Math.Pow(BallY - cursorPosition.Y, 2), .5);
        double repulsionForce = RepulsionIntensity / distanceToCursor;
        double angle = Math.Atan2(cursorPosition.Y - BallY, cursorPosition.X - BallX);

        double repulsionX = repulsionForce * Math.Cos(angle);
        double repulsionY = repulsionForce * Math.Sin(angle);

        VerticalSpeed -= repulsionY;
        HorizontalSpeed -= repulsionX;
    }

    public void BallJump()
    {
        VerticalSpeed -= JumpForce;
    }
    public void SimulateTimeframe(double dt)
    {
        SimulateBallMovement(dt);
        SimulateBallBouncedFromSideWalls(dt);
        SimulateBallBouncedFromVerticalWalls(dt);
        SimulateGravity(dt);
    }
}