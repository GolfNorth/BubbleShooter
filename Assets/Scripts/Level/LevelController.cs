using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooter
{
    public sealed class LevelController : MonoBehaviour
    {
        private const int LevelIndex = 1;

        [SerializeField] private Board _board;
        [SerializeField] private Transform _nextBubbleAnchor;
        [SerializeField] private Transform _activeBubbleAnchor;
        [SerializeField] private Text _bubblesLeftText;

        private Level _level;
        private int _bubblesLeft;
        private BubbleObject _nextBubble;
        private BubbleObject _activeBubble;
        private BubbleController _bubbleController;

        public int BubblesLeft
        {
            get => _bubblesLeft;
            private set
            {
                _bubblesLeft = value;
                _bubblesLeftText.text = value.ToString();
            }
        }

        public Board Board => _board;

        public Level Level => _level;

        public BubbleController BubbleController => _bubbleController;

        private void Awake()
        {
            _level = LevelLoader.Load(LevelIndex);
            BubblesLeft = Context.Instance.Settings.NumberOfBubbles;
            
            Context.Instance.LevelController = this;
            
            _board.Initialize();
            
            _bubbleController = new BubbleController();
            _bubbleController.CreateBubbles(_level);

            _nextBubble = _bubbleController.CreateBubble();
            _nextBubble.Bubble.transform.position = _nextBubbleAnchor.position;
            _nextBubble.Bubble.SwitchState(BubbleStateType.Idle);
            
            _activeBubble = _bubbleController.CreateBubble();
            _activeBubble.Bubble.transform.position = _activeBubbleAnchor.position;
            _activeBubble.Bubble.SwitchState(BubbleStateType.Aiming);
            
            Context.Instance.NotificationService.Notification += OnNotification;
        }
        
        private void OnNotification(NotificationType notificationType, object obj)
        {
            if (notificationType == NotificationType.BubbleLaunched)
            {
                if (BubblesLeft == 0) return;
                
                _activeBubble = _nextBubble;
                _activeBubble.Bubble.transform.position = _activeBubbleAnchor.position;
                _activeBubble.Bubble.SwitchState(BubbleStateType.Aiming);
                
                _nextBubble = _bubbleController.CreateBubble();
                _nextBubble.Bubble.transform.position = _nextBubbleAnchor.position;
                _nextBubble.Bubble.SwitchState(BubbleStateType.Idle);

                BubblesLeft--;
            }
        }

        private void Start()
        {
            for (int i = 0; i < 40; i++)
            {
                //Debug.Log(_bubbleController.CreateBubble().Bubble.Color.Abbreviation);
            }
        }

        private void OnDisable()
        {
            Context.Instance.NotificationService.Notification += OnNotification;
            Context.Instance.LevelController = null;
        }
    }
}