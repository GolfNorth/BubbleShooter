using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleAimingState : BubbleState
    {
        private const float AllowableError = 0.01f;
        private Vector2 _startingPosition;
        private InputService _inputService;
        private float _angularDisplacement;
        private float _pullingDistance;
        private float _maxSpeed;
        private float _minForce;
        private BubbleTrajectory _trajectoryA;
        private BubbleTrajectory _trajectoryB;

        public BubbleAimingState(Bubble bubble) : base(bubble, BubbleStateType.Aiming)
        {
        }

        public override void Enter()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            _trajectoryA = new BubbleTrajectory();
            _trajectoryB = new BubbleTrajectory();
            _startingPosition = Bubble.transform.position;
            
            _inputService = Context.Instance.InputService;
            _angularDisplacement = Context.Instance.Settings.AngularDisplacement;
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

                var force = GetForce();
                
                var velocityA = GetDirection() * (force * _maxSpeed);

                if (1 - force < AllowableError)
                {
                    var velocityB = Quaternion.Euler(0, 0, _angularDisplacement) * (velocityA - position);
                    velocityA = Quaternion.Euler(0, 0, -_angularDisplacement) * (velocityA - position);
                    
                    _trajectoryA.SetValues(Bubble.transform.position, velocityA);
                    _trajectoryB.SetValues(Bubble.transform.position, velocityB);
                    
                    Context.Instance.LevelController.Trajectories.DrawTrajectories(_trajectoryA, _trajectoryB);
                }
                else
                {
                    _trajectoryA.SetValues(Bubble.transform.position, velocityA);
                    
                    Context.Instance.LevelController.Trajectories.DrawTrajectory(_trajectoryA);
                }
            }
            else
            {
                var force = GetForce();

                if (force >= _minForce)
                {
                    if (1 - force < AllowableError)
                    {
                        var randomAngle = Random.Range(-_angularDisplacement, _angularDisplacement);
                        var velocity = Quaternion.Euler(0, 0, -randomAngle) * GetDirection() * (force * _maxSpeed);
                        
                        _trajectoryA.SetValues(Bubble.transform.position, velocity);
                    }
                    
                    Bubble.Trajectory = _trajectoryA;
                    
                    Bubble.SwitchState(BubbleStateType.Moving);
                }
                else
                {
                    Bubble.transform.position = _startingPosition;
                }
                
                Context.Instance.LevelController.Trajectories.HideTrajectories();
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