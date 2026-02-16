using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Input;

namespace Runtime.Components.Test
{
    /// <summary>
    /// Simple camera controls, usefull for testing.
    /// </summary>
    public class FlightCamera : Objects.Component
    {
        Vector3 moveDelta = Vector3.Zero;
        float flightSpeed = 4.0f;
        public override void Update()
        {
            Vector3 goalDelta = Vector3.Zero;
            flightSpeed += Mouse.current.scroll.y * 1f;
            if (flightSpeed < 0)
                flightSpeed = 0;

            if (Keyboard.current.IsPressed(Key.A))
                goalDelta.x = -1;

            if (Keyboard.current.IsPressed(Key.D))
                goalDelta.x = 1;

            if (Keyboard.current.IsPressed(Key.Q))
                goalDelta.y = -1;

            if (Keyboard.current.IsPressed(Key.E))
                goalDelta.y = 1;

            if (Keyboard.current.IsPressed(Key.W))
                goalDelta.z = -1;

            if (Keyboard.current.IsPressed(Key.S))
                goalDelta.z = 1;

            goalDelta.Normalize();
            moveDelta = Vector3.Lerp(moveDelta, goalDelta, (float)Time.deltaTime * 6f);

            Transform? tr = GetComponent<Transform>();
            if (null != tr)
            {
                tr.position += tr.GetForwards() * -moveDelta.z * (float)Time.deltaTime * flightSpeed;
                tr.position += tr.GetRight() * moveDelta.x * (float)Time.deltaTime * flightSpeed;
                tr.position += tr.GetUp() * moveDelta.y * (float)Time.deltaTime * flightSpeed;
                if (Mouse.current.leftPressed)
                {

                    // Rotating
                    tr.Rotate(0, -Mouse.current.mouseDelta.x / 10, 0);
                    tr.Rotate(-Mouse.current.mouseDelta.y / 10, 0, 0);

                }


            }
        }

    }
}
