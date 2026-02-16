using Runtime.Calc;
using Runtime.Components;
using Runtime.Components.Core;
using Runtime.Input;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Physics.Raycasts;

namespace Project.Editor.Components
{
    public class Draggable : Component
    {
        Vector3 direction;
        public Draggable(Vector3 direction)
        {
            this.direction = direction;
        }
        bool isDragging = false;
        public override void Update()
        {
            if (Mouse.current.LeftPressedThisFrame())
            {
                CheckDrag();
            }

            if (Mouse.current.LeftReleasedThisFrame())
            {
                Mouse.current.ShowCursor();
                isDragging = false;
            }

            if (isDragging)
            {
                if (direction.y == 0)
                    HandleXZ();

                else
                    HandleY();
            }
        }

        private void HandleY()
        {
            Vector2 mouseDelta = Mouse.current.mouseDelta;
            GetComponent<FollowConstraint>()!.target.position += new Vector3(0, -1, 0) * (float)Time.deltaTime * mouseDelta.y;
        }

        private void HandleXZ()
        {
            Camera cam = Camera.main!;

            Vector2 mouseDelta = Mouse.current.mouseDelta;

            Vector3 camRight = cam.GetComponent<Transform>()!.GetRight();
            Vector3 camUp = cam.GetComponent<Transform>()!.GetUp();

            camRight.y = 0f;
            camUp.y = 0f;
            camRight.Normalize();
            camUp.Normalize();

            Vector3 mouseWorldMove =
                (camRight * mouseDelta.x) +
                (camUp * mouseDelta.y);

            Vector3 dragDir = direction.Normalized();
            Vector3 projectedMove = Vector3.Project(mouseWorldMove, dragDir);

            GetComponent<FollowConstraint>()!.target.position +=
                projectedMove * (float)Time.deltaTime;
        }

        private void CheckDrag()
        {
            Raycast? raycast = Camera.main!.GetRaycastFromMouse();
            if (raycast == null)
            {
                Debug.Error("Could not find main camera from Draggable.");
                return;
            }

            RaycastResult? result = raycast!.CastInMainScene();
            if (result == null)
                return;

            if (result.collider.gameObject == gameObject)
            {
                Mouse.current.HideCursor();
                isDragging = true;
            }
        }
    }
}
