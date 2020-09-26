using UnityEngine;

namespace BubbleShooter
{
    public class Anchor : MonoBehaviour
    {
        #region Fields

        private Rigidbody2D _rigidbody;

        #endregion

        #region Properties

        public Rigidbody2D Rigidbody => _rigidbody;

        public Coordinate Coordinate { get; set; }

        public AnchorObject AnchorObject { get; set; }

        #endregion

        #region Methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        #endregion
    }
}