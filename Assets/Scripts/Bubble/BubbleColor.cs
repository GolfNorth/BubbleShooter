using UnityEngine;

namespace BubbleShooter
{
    [CreateAssetMenu(fileName = "BubbleColor", menuName = "Bubble Color")]
    public sealed class BubbleColor : ScriptableObject
    {
        [SerializeField] private bool _isSpawned = true;
        [SerializeField] private Color _color;
        [SerializeField] private string _abbreviation;

        public bool IsSpawned => _isSpawned;

        public Color Color => _color;

        public string Abbreviation => _abbreviation;
    }
}