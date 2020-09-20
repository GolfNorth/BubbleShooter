using UnityEngine;

namespace BubbleShooter
{
    public sealed class BoundsService
    {
        private Bounds _bounds;
        private float _topBound;
        private float _bottomBound;
        private float _leftBound;
        private float _rightBound;

        public BoundsService()
        {
            _bounds = new Bounds();
            
            RecalculateBounds();

            Context.Instance.NotificationService.Notification += OnNotification;
        }

        private void OnNotification(NotificationType notificationType, object obj)
        {
            if (notificationType == NotificationType.SceneLoaded)
            {
                RecalculateBounds();
            }
        }

        public Bounds Bounds => _bounds;

        private void RecalculateBounds()
        {
            var camera = Camera.main;
            
            if (camera == null) return;
            
            var z = camera.gameObject.transform.position.z;
            var topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, -z));
            var bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, -z));

            _bounds.Top = topRight.y;
            _bounds.Bottom = bottomLeft.y;
        }
    }
}