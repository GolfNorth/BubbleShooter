using System;

namespace BubbleShooter
{
    public sealed class NotificationService
    {
        #region Fields

        public event Action<NotificationType, object> Notification;

        #endregion

        #region Methods

        public void Notify(NotificationType notificationType, object obj = null)
        {
            Notification?.Invoke(notificationType, obj);
        }

        #endregion
    }
}