using UnityEngine;

namespace BubbleShooter
{
    [CreateAssetMenu(fileName = "Settings")]
    public sealed class Settings : ScriptableObject
    {
        [SerializeField] private GameObject _bubblePrefab;
        [SerializeField] private GameObject _anchorPrefab;
        [SerializeField] private GameObject _effectorPrefab;
        [SerializeField] private float _bubbleRadius;
        [SerializeField] private float _bubbleMaxSpeed;
        [SerializeField] private float _boardOffset;
        [SerializeField] private float _boardSpeed;
        [SerializeField] private float _effectorDuration;
        [SerializeField] private Bounds _bounds;

        public GameObject BubblePrefab => _bubblePrefab;

        public GameObject AnchorPrefab => _anchorPrefab;
        
        public GameObject EffectorPrefab => _effectorPrefab;
        
        public float BubbleRadius => _bubbleRadius;
        
        public float BubbleMaxSpeed => _bubbleMaxSpeed;
        
        public float BoardOffset => _boardOffset;

        public float BoardSpeed => _boardSpeed;

        public float EffectorDuration => _effectorDuration;

        public Bounds Bounds => _bounds;
    }
}