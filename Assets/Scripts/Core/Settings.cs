using UnityEngine;

namespace BubbleShooter
{
    [CreateAssetMenu(fileName = "Settings")]
    public sealed class Settings : ScriptableObject
    {
        [Header("Prefabs")] [SerializeField] private GameObject _bubblePrefab;
        [SerializeField] private GameObject _anchorPrefab;
        [SerializeField] private GameObject _effectorPrefab;
        [Header("Level")] [SerializeField] private int _columns;
        [SerializeField] private float _firstRowRatioVictory;
        [SerializeField] private int _numberOfBubbles;

        [Header("Trajectory")] [SerializeField]
        private float _bubbleRayCastRadius;

        [SerializeField] private float _angularDisplacement;
        [SerializeField] private float _minForce;
        [Header("Bubble")] [SerializeField] private float _bubbleRadius;
        [SerializeField] private float _bubbleMaxSpeed;
        [SerializeField] private float _bubbleDefaultGravity;
        [SerializeField] private float _bubbleStickedDrag;
        [Header("Board")] [SerializeField] private float _boardOffset;
        [SerializeField] private float _boardSpeed;
        [SerializeField] private float _effectorDuration;
        [SerializeField] private float _pullingDistance;
        [Header("Other")] [SerializeField] private Bounds _bounds;


        public GameObject BubblePrefab => _bubblePrefab;

        public GameObject AnchorPrefab => _anchorPrefab;

        public GameObject EffectorPrefab => _effectorPrefab;

        public int NumberOfBubbles => _numberOfBubbles;

        public Bounds Bounds => _bounds;

        public float PullingDistance => _pullingDistance;

        public float BubbleRadius => _bubbleRadius;

        public float BubbleRayCastRadius => _bubbleRayCastRadius;

        public float BubbleMaxSpeed => _bubbleMaxSpeed;

        public float BubbleDefaultGravity => _bubbleDefaultGravity;

        public float BubbleStickedDrag => _bubbleStickedDrag;

        public float BoardOffset => _boardOffset;

        public float BoardSpeed => _boardSpeed;

        public float EffectorDuration => _effectorDuration;

        public float MinForce => _minForce;

        public int Columns => _columns;

        public float FirstRowRatioVictory => _firstRowRatioVictory;

        public float AngularDisplacement => _angularDisplacement;
    }
}