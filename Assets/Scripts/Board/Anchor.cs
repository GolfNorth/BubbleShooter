using UnityEngine;

namespace BubbleShooter
{
    public class Anchor : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;

        public Rigidbody2D Rigidbody => _rigidbody;

        public Coordinate Coordinate { get; set; }

        public AnchorObject AnchorObject { get; set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}