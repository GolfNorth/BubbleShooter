using UnityEngine;

namespace BubbleShooter
{
    public sealed class InputService
    {
        #region Properties

        public Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

        public bool HoldPressed => Input.GetMouseButton(0);

        #endregion);
    }
}