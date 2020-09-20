using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class LevelController : MonoBehaviour
    {
        private const int LevelIndex = 1;

        [SerializeField] private Board _board;
        
        private Level _level;
        private BubblePool _bubblePool;
        private int _bubbleCount;
        private Dictionary<BubbleColor, int> _colorsCount;

        public Board Board => _board;
        
        private void Awake()
        {
            _level = LevelLoader.Load(LevelIndex);
            _colorsCount = new Dictionary<BubbleColor, int>();
            
            _board.Initialize(_level);

            InitializePool();
            CreateBubbles();
        }

        private void OnEnable()
        {
            Context.Instance.LevelController = this;
        }

        private void OnDisable()
        {
            Context.Instance.LevelController = null;
        }
        
        private void InitializePool()
        {
            var poolCount = _level.Columns * _level.Rows;
            var bubblePrefab = Context.Instance.Settings.BubblePrefab;
            
            _bubblePool = BubblePool.GetObjectPool(bubblePrefab, poolCount);
            _bubblePool.transform.SetParent(_board.transform);
            _bubblePool.transform.localPosition = Vector3.zero;
        }

        private void CreateBubbles()
        {
            for (var r = 0; r < _level.Rows; r++)
            {
                for (var c = 0; c < _level.Columns; c++)
                {
                    if (_level.Colors[r][c] is null) continue;
                    
                    var coordinate = new Coordinate(r, c);
                    var color = _level.Colors[r][c];
                    var bubbleObject = _bubblePool.Pop(coordinate);
                    bubbleObject.Bubble.Color = color;

                    _board.AddBubble(coordinate, bubbleObject);

                    //_colorsCount[color]++;
                    _bubbleCount++;
                }
            }
        }
    }
}