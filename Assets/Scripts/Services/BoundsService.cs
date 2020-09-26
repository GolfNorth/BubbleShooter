using System;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class BoundsService
    {
        private Bounds _bounds;

        public BoundsService()
        {
            _bounds = new Bounds()
            {
                Left = Context.Instance.Settings.Bounds.Left,
                Right = Context.Instance.Settings.Bounds.Right
            };

            RecalculateBounds();

            Context.Instance.NotificationService.Notification += OnNotification;
        }

        private void OnNotification(NotificationType notificationType, object obj)
        {
            if (notificationType == NotificationType.SceneLoaded) RecalculateBounds();
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

        public void SetTopBound(float topBound)
        {
            _bounds.Top = topBound;
        }
    }
}