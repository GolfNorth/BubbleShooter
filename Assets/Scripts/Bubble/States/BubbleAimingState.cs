using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleAimingState : BubbleState
    {
        private const float AllowableError = 0.01f;
        private Vector2 _startingPosition;
        private InputService _inputService;
        private float _pullingDistance;
        private float _maxSpeed;
        private float _minForce;
        private BubbleTrajectory _trajectory;

        public BubbleAimingState(Bubble bubble) : base(bubble, BubbleStateType.Aiming)
        {
        }

        public override void Enter()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            _trajectory = new BubbleTrajectory();
            _startingPosition = Bubble.transform.position;
            
            _inputService = Context.Instance.InputService;
            _pullingDistance = Context.Instance.Settings.PullingDistance;
            _maxSpeed = Context.Instance.Settings.BubbleMaxSpeed;
            _minForce = Context.Instance.Settings.MinForce;

            Bubble.transform.SetParent(null);
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Static;
        }
        
        public override void Exit()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Bubble");
            Bubble.transform.SetParent(Context.Instance.LevelController.Board.transform);
        }

        public override void Update()
        {
            if (_inputService.HoldPressed)
            {
                var position = _inputService.CursorPosition;
                var distance = Vector2.Distance(position, _startingPosition);

                if (distance > _pullingDistance)
                    position = _startingPosition + (position - _startingPosition).normalized * _pullingDistance;
                
                if (position.y > _startingPosition.y)
                    position.y = _startingPosition.y;

                Bubble.transform.position = position;
                
                var velocity = GetDirection() * (GetForce() * _maxSpeed);
                
                _trajectory.SetValues(Bubble.transform.position, velocity);
                
                var points = _trajectory.Points;

                var prevPoint = Vector2.zero;
                
                foreach (var point in points)
                {
                    if (prevPoint != Vector2.zero)
                        Debug.DrawLine(prevPoint, point.Value, Color.magenta);
                    
                    prevPoint = point.Value;
                }
            }
            else
            {
                var force = GetForce();

                if (force >= _minForce)
                {
                    Bubble.Trajectory = _trajectory;
                    
                    Bubble.SwitchState(BubbleStateType.Moving);
                }
                else
                {
                    Bubble.transform.position = _startingPosition;
                }
            }
        }

        private float GetForce()
        {
            var force = Vector2.Distance(Bubble.transform.position, _startingPosition) / _pullingDistance;
            force = 1 - force < AllowableError ? 1 : force;

            return force;
        }

        private Vector2 GetDirection()
        {
            Vector2 position = Bubble.transform.position;
            
            return (_startingPosition - position).normalized;
        }
    }
}