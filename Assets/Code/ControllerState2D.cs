using UnityEngine;
using System.Collections;


public class ControllerState2D
{
    public bool IsCollidingRight { get; set; }

    public bool IsCollidingLeft { get; set; }

    public bool IsCollidingAbove { get; set; }

    public bool IsCollidingBlow { get; set; }

    public bool IsMovingDownSlope { get; set; }

    public bool IsMovingUpSlope { get; set; }

    public bool IsGrounded { get { return IsCollidingBlow; } }

    public float SlopeAngel { get; set; }

    public bool HasCollisions { get { return IsCollidingAbove || IsCollidingBlow || IsCollidingLeft || IsCollidingRight; } }

    public void Reset()
    {
        IsMovingDownSlope =
            IsMovingUpSlope =
            IsCollidingLeft =
            IsCollidingRight =
            IsCollidingAbove =
            IsCollidingBlow = false;

        SlopeAngel = 0;
    }

    public override string ToString()
    {
        return string.Format("(Controller: r:{0} l:{1} a:{2} b:{3} down-slope:{4} up-slope:{5} angle:{6})",
            IsCollidingRight,
            IsCollidingLeft,
            IsCollidingAbove,
            IsCollidingBlow,
            IsMovingDownSlope,
            IsMovingUpSlope,
            SlopeAngel);
    }
}
