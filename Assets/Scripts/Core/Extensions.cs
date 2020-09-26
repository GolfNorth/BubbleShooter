using UnityEngine;

namespace BubbleShooter
{
    public static class Extensions
    {
        private const int ContactAngle = 300;
        private static Vector2 _step;
        private static Vector2 _offset;

        static Extensions()
        {
            var bubbleRadius = Context.Instance.Settings.BubbleRadius;

            _step = new Vector2(bubbleRadius * 2, bubbleRadius * Mathf.Sin(ContactAngle * Mathf.Deg2Rad) * 2);
            _offset = new Vector2(bubbleRadius, -bubbleRadius);
        }

        public static Vector2 ToLocalPosition(this Coordinate coordinate)
        {
            var offsetX = coordinate.IsRowEven ? _offset.x : _offset.x * 2;

            return new Vector2(
                offsetX + _step.x * coordinate.Column,
                _offset.y + _step.y * coordinate.Row
            );
        }

        public static Vector2 ToLocalPosition(this Vector2 worldPosition)
        {
            return ToLocalPosition((Vector3) worldPosition);
        }

        public static Vector2 ToLocalPosition(this Vector3 worldPosition)
        {
            var board = Context.Instance.LevelController.Board;

            return board.transform.InverseTransformPoint(worldPosition);
        }

        public static Coordinate ToCoordinateLocal(this Vector2 localPosition)
        {
            return ToCoordinateLocal((Vector3) localPosition);
        }

        public static Coordinate ToCoordinateLocal(this Vector3 localPosition)
        {
            var result = new Coordinate();

            if (localPosition.x < 0 || localPosition.y > 0) return result;

            result.Row = (int) Mathf.Abs(localPosition.y / _step.y);

            var offsetX = result.IsRowEven ? 0 : _offset.x;

            result.Column = (int) Mathf.Abs((localPosition.x - offsetX) / _step.x);

            return result;
        }

        public static Coordinate ToCoordinateWorld(this Vector2 worldPosition)
        {
            return ToCoordinateWorld((Vector3) worldPosition);
        }

        public static Coordinate ToCoordinateWorld(this Vector3 worldPosition)
        {
            return worldPosition.ToLocalPosition().ToCoordinateLocal();
        }
    }
}