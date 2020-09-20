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
        
        public static Coordinate ToCoordinate(this Vector2 position)
        {
            return ToCoordinate((Vector3) position);
        }

        public static Coordinate ToCoordinate(this Vector3 position)
        {
            var result = new Coordinate();

            if (position.x < 0 || position.y > 0) return result;

            result.Row = (int) Mathf.Abs((position.y - _offset.y) / _step.y);
            
            var offsetX = result.IsRowEven ? _offset.x : _offset.x * 2;
            
            result.Column = (int) Mathf.Abs((position.x - offsetX) / _step.x);

            return result;
        }
    }
}