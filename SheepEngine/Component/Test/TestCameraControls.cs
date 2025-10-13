using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Runtime.Component.Core;
using Runtime.Calc;
using Runtime.Input;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Test
{
    public class TestCameraControls : IComponent
    {
        public override void Update()
        {
            float deltaX = 0;
            if (Keyboard.current.IsPressed(Keys.A))
                deltaX = -1;

            if (Keyboard.current.IsPressed(Keys.D))
                deltaX = 1;


            float deltaY = 0;
            if (Keyboard.current.IsPressed(Keys.Q))
                deltaY = -1;

            if (Keyboard.current.IsPressed(Keys.E))
                deltaY = 1;

            float deltaZ = 0;
            if (Keyboard.current.IsPressed(Keys.W))
                deltaZ = -1;

            if (Keyboard.current.IsPressed(Keys.S))
                deltaZ = 1;

            float speed = 4;

            Transform tr = GetComponent<Transform>();
            tr.position += tr.GetForwards() * -deltaZ * (float)Time.deltaTime * speed;
            tr.position += tr.GetRight() * deltaX * (float)Time.deltaTime * speed;


            // Rotating
            tr.Rotate(0, -Mouse.current.mouseDelta.X / 10, 0);
            tr.Rotate(Mouse.current.mouseDelta.Y / 10, 0, 0);
        }

    }
}
