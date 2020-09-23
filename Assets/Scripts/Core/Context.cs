using UnityEngine;

namespace BubbleShooter
{
    public sealed class Context : MonoBehaviour
    {
        [SerializeField] private Settings _settings;
        [SerializeField] private BubbleColorCollection _colorCollection;
        
        private static Context _instance;
        private static NotificationService _notificationService;
        private static SceneService _sceneService;
        private static BoundsService _boundsService;
        private static InputService _inputService;

        public Settings Settings
        {
            get => _settings;
            set => _settings = value;
        }
        
        public BubbleColorCollection ColorCollection => _colorCollection;
        
        public static Context Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<Context>();

                return _instance != null ? _instance : Create();
            }
        }
        
        

        public NotificationService NotificationService => _notificationService;
        
        public SceneService SceneService => _sceneService;

        public BoundsService BoundsService => _boundsService;

        public InputService InputService => _inputService;
        
        public LevelController LevelController { get; set; }

        private static Context Create()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Context");
            var contextGameObject = GameObject.Instantiate(prefab);
            _instance = contextGameObject.GetComponent<Context>();
            
            _notificationService = new NotificationService();
            _sceneService = new SceneService();
            _boundsService = new BoundsService();
            _inputService = new InputService();

            return _instance;
        }

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void ExitGame()
        {
            _notificationService.Notify(NotificationType.ExitGame);
            _notificationService = null;
            
            Application.Quit();
        }
    }
}