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

        public Coordinate Coordinate { get; set; }

        public BubbleColor Color
        {
            get => _bubbleColor;
            set
            {
                _bubbleColor = value;
                _meshRenderer.material.color = _bubbleColor.Color;
            }
        }

        private void Awake()
        {
            _springJoint = GetComponent<SpringJoint2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Stick(Rigidbody2D target)
        {
            _springJoint.enabled = true;
            _springJoint.connectedBody = target;
            _rigidbody.gravityScale = 0;
        }

        public void Unstick()
        {
            _springJoint.enabled = false;
            _springJoint.connectedBody = null;
            _rigidbody.gravityScale = 1;
        }

        private void OnDisable()
        {
            Unstick();
        }
    }
}