using System;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooter
{
    public sealed class LevelController : MonoBehaviour
    {
        #region Constatnts

        private const int LevelIndex = 1;

        #endregion

        #region Fields

        [SerializeField] private Board _board;
        [SerializeField] private Trajectories _trajectories;
        [SerializeField] private Transform _nextBubbleAnchor;
        [SerializeField] private Transform _activeBubbleAnchor;
        [SerializeField] private Text _bubblesLeftText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _resultText;

        private Level _level;
        private int _bubblesLeft;
        private int _score;
        private bool _gameEnded;
        private BubbleObject _nextBubble;
        private BubbleObject _activeBubble;
        private BubbleController _bubbleController;

        #endregion

        #region Properties

        public int BubblesLeft
        {
            get => _bubblesLeft;
            private set
            {
                if (value < 0) value = 0;

                _bubblesLeft = value;
                _bubblesLeftText.text = value.ToString();
            }
        }

        public Board Board => _board;

        public Level Level => _level;

        public BubbleController BubbleController => _bubbleController;

        public Trajectories Trajectories => _trajectories;

        #endregion

        #region Methods

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
            if (_gameEnded) return;

            if (notificationType == NotificationType.BubbleLaunched)
            {
                if (BubblesLeft == 0) Context.Instance.NotificationService.Notify(NotificationType.Defeat);

                _activeBubble = _nextBubble;
                _activeBubble.Bubble.transform.position = _activeBubbleAnchor.position;
                _activeBubble.Bubble.SwitchState(BubbleStateType.Aiming);

                _nextBubble = _bubbleController.CreateBubble();
                _nextBubble.Bubble.transform.position = _nextBubbleAnchor.position;
                _nextBubble.Bubble.SwitchState(BubbleStateType.Idle);

                BubblesLeft--;
            }
            else if (notificationType == NotificationType.Victory)
            {
                _resultText.gameObject.SetActive(true);
                _resultText.text = "Victory";
                _gameEnded = true;
            }
            else if (notificationType == NotificationType.Defeat)
            {
                _resultText.gameObject.SetActive(true);
                _resultText.text = "Defeat";
                _gameEnded = true;
            }
        }

        public void AddScore()
        {
            _score++;

            _scoreText.text = $"Score: {_score * 100}";
        }

        private void OnDisable()
        {
            Context.Instance.NotificationService.Notification += OnNotification;
            Context.Instance.LevelController = null;
        }

        private void Update()
        {
            if (_gameEnded && Context.Instance.InputService.HoldPressed)
                Context.Instance.SceneService.LoadScene("MainScene");
        }

        #endregion
    }
}