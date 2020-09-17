using UnityEngine;

namespace BubbleShooter
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PoolObject<BubblePool, BubbleObject, Vector2> bubblePoolObject;

        public PoolObject<BubblePool, BubbleObject, Vector2> BubblePoolObject
        {
            get => bubblePoolObject;
            set => bubblePoolObject = value;
        }

        public Camera MainCamera
        {
            get => mainCamera;
            set => mainCamera = value;
        }
    }
}