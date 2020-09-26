using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleTrajectory
    {
        private const float AllowableError = 0.01f;
        private const float TimeStep = 0.1f;
        private Vector2 _position;
        private Vector2 _velocity;
        private Bounds _bounds;
        private bool _collided;
        private GameObject _collidedWith;
        private float _force;
        private readonly float _gravity;
        private readonly Dictionary<float, Vector2> _points;
        private readonly List<float> _timeStamps;

        public Dictionary<float, Vector2> Points => _points;

        public bool Collided => _collided;

        public GameObject CollidedWith => _collidedWith;

        public BubbleTrajectory()
        {
            _gravity = Physics2D.gravity.y;
            _points = new Dictionary<float, Vector2>();
            _timeStamps = new List<float>();

            var bounds = Context.Instance.BoundsService.Bounds;
            var radius = Context.Instance.Settings.BubbleRadius;

            _bounds = new Bounds()
            {
                Left = bounds.Left + radius,
                Right = bounds.Right - radius,
                Top = bounds.Top - radius,
                Bottom = bounds.Bottom - radius
            };
        }

        public void SetValues(Vector2 position, Vector2 velocity, float force)
        {
            if (_position.Equals(position) && _velocity.Equals(velocity)) return;

            _bounds.Top = Context.Instance.BoundsService.Bounds.Top - Context.Instance.Settings.BubbleRadius;
            _position = position;
            _velocity = velocity;
            _collided = false;
            _collidedWith = null;
            _force = force;

            _timeStamps.Clear();
            _points.Clear();

            GenerateTimeStamps();
            GenerateTrajectory();
        }

        private void GenerateTimeStamps()
        {
            var time = Mathf.Abs(_velocity.y / _gravity);
            var yTop = _position.y + _velocity.y * time + _gravity * time * time / 2;

            if (yTop > _bounds.Top)
            {
                var sYTop = _bounds.Top - _position.y;
                var vYTop = Mathf.Sqrt(sYTop * 2 * _gravity + _velocity.y * _velocity.y);
                time = (vYTop - _velocity.y) / _gravity;

                _timeStamps.Add(time);
            }
            else
            {
                _timeStamps.Add(time);

                var sYBottom = yTop - _bounds.Bottom;
                var vYBottom = -Mathf.Sqrt(Mathf.Abs(sYBottom * 2 * _gravity));
                time += vYBottom / _gravity;

                _timeStamps.Add(time);
            }

            var tX = 0f;
            var positionX = _position.x;
            var velocityX = _velocity.x;

            while (true)
            {
                var xBound = velocityX > 0 ? _bounds.Right : _bounds.Left;
                tX += Mathf.Abs((positionX - xBound) / velocityX);

                if (tX > time) break;

                positionX = xBound;
                velocityX = -velocityX;

                _timeStamps.Add(tX);
            }

            _timeStamps.Sort();
        }

        private void GenerateTrajectory()
        {
            var point = Vector2.zero;
            var prevPoint = Vector2.zero;
            var time = 0f;
            var velocity = new Vector2(_velocity.x, _velocity.y);
            var position = new Vector2(_position.x, _position.y);

            _points.Add(0, position);

            foreach (var timeStamp in _timeStamps)
            {
                _collided = false;

                while (true)
                {
                    if (time == timeStamp) break;

                    time += TimeStep;

                    if (time > timeStamp) time = timeStamp;

                    point = new Vector2
                    {
                        x = position.x + velocity.x * time,
                        y = position.y + velocity.y * time + _gravity * time * time / 2
                    };

                    if (_points.Count > 1)
                    {
                        var direction = (point - prevPoint).normalized;
                        var distance = Vector2.Distance(prevPoint, point);

                        var hit = Physics2D.CircleCast(prevPoint, Context.Instance.Settings.BubbleRayCastRadius,
                            direction, distance, 1 << LayerMask.NameToLayer("Bubble"));

                        if (hit.collider != null)
                        {
                            point = hit.centroid;
                            _collided = true;

                            if (_force == 1) _collidedWith = hit.collider.gameObject;
                        }
                    }

                    if (!_points.ContainsKey(time))
                    {
                        _points.Add(time, point);
                        prevPoint = point;
                    }

                    if (_collided) return;
                }

                if (Math.Abs(point.x - _bounds.Left) < AllowableError ||
                    Math.Abs(point.x - _bounds.Right) < AllowableError)
                {
                    var distance = Mathf.Abs(_velocity.x * time);

                    velocity.x = -velocity.x;
                    position.x = velocity.x > 0
                        ? -distance + _bounds.Left
                        : distance + _bounds.Right;
                }
                else if (Math.Abs(point.y - _bounds.Top) < AllowableError)
                {
                    _collided = true;
                }
            }
        }
    }
}