using UnityEngine;

namespace BubbleShooter
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class Bubble : MonoBehaviour
    {
        private BubbleColor _bubbleColor;
        private SpringJoint2D _springJoint;
        private Rigidbody2D _rigidbody;
        private MeshRenderer _meshRenderer;
        private BubbleState _currentState;

        private BubbleIdleState _idleState;
        private BubbleAimingState _aimingState;
        private BubbleMovingState _movingState;
        private BubbleStickedState _stickedState;
        private BubbleFallingState _fallingState;

        public BubbleObject BubbleObject { get; set; }
        
        public Anchor Anchor { get; set; }
        
        public BubbleTrajectory Trajectory { get; set; }

        public BubbleColor Color
        {
            get => _bubbleColor;
            set
            {
                _bubbleColor = value;
                _meshRenderer.material.color = _bubbleColor.Color;
            }
        }

        public SpringJoint2D SpringJoint => _springJoint;

        public Rigidbody2D Rigidbody => _rigidbody;

        private void Awake()
        {
            _springJoint = GetComponent<SpringJoint2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _meshRenderer = GetComponent<MeshRenderer>();
            
            _idleState = new BubbleIdleState(this);
            _aimingState = new BubbleAimingState(this);
            _movingState = new BubbleMovingState(this);
            _stickedState = new BubbleStickedState(this);
            _fallingState = new BubbleFallingState(this);
        }

        public void Stick(Anchor anchor)
        {
            Anchor = anchor;
            SwitchState(BubbleStateType.Sticked);
        }

        public void Unstick()
        {
            SwitchState(BubbleStateType.Falling);
            Anchor = null;
        }

        public void SwitchState(BubbleStateType state)
        {
            if (_currentState?.State == state) return;
            
            BubbleState nextState;
            
            switch (state)
            {
                default:
                case BubbleStateType.Idle:
                    nextState = _idleState;
                    break;
                case BubbleStateType.Aiming:
                    nextState = _aimingState;
                    break;
                case BubbleStateType.Moving:
                    nextState = _movingState;
                    break;
                case BubbleStateType.Sticked:
                    nextState = _stickedState;
                    break;
                case BubbleStateType.Falling:
                    nextState = _fallingState;
                    break;
            }
            
            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();
            
        }

        private void Update()
        {
            _currentState?.Update();
        }

        private void LateUpdate()
        {
            _currentState?.LateUpdate();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }
    }
}