using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleController
    {
        #region Fields

        private readonly Dictionary<BubbleColor, int> _colorsCount;
        private readonly LevelController _levelController;
        private BubblePool _bubblePool;

        #endregion

        #region Constructor

        public BubbleController()
        {
            _levelController = Context.Instance.LevelController;
            _colorsCount = new Dictionary<BubbleColor, int>();

            InitializePool();
        }

        #endregion

        #region Methods

        private void InitializePool()
        {
            var poolCount = _levelController.Level.Columns * _levelController.Level.Rows;
            var bubblePrefab = Context.Instance.Settings.BubblePrefab;

            _bubblePool = BubblePool.GetObjectPool(bubblePrefab, poolCount);
            _bubblePool.transform.SetParent(_levelController.Board.transform);
            _bubblePool.transform.localPosition = Vector3.zero;
        }

        public BubbleObject CreateBubble()
        {
            var color = GetRandomColor();
            var bubbleObject = _bubblePool.Pop();
            bubbleObject.Bubble.Color = color;
            bubbleObject.Bubble.gameObject.SetActive(true);

            _colorsCount[color]++;

            return bubbleObject;
        }

        public void RemoveBubble(Bubble bubble)
        {
            _bubblePool.Push(bubble.BubbleObject);

            var color = bubble.Color;

            if (!_colorsCount.ContainsKey(color)) return;

            _colorsCount[color]--;

            if (_colorsCount[color] == 0)
                _colorsCount.Remove(color);
        }

        public void CreateBubbles(Level level)
        {
            for (var r = 0; r < level.Rows; r++)
            for (var c = 0; c < level.Columns; c++)
            {
                if (level.Colors[r][c] is null) continue;

                var coordinate = new Coordinate(r, c);
                var color = level.Colors[r][c];
                var bubbleObject = _bubblePool.Pop(coordinate.ToLocalPosition());
                bubbleObject.Bubble.Color = color;

                _levelController.Board.AddBubble(bubbleObject.Bubble, coordinate);

                if (!color.IsSpawned) continue;

                if (_colorsCount.ContainsKey(color))
                    _colorsCount[color]++;
                else
                    _colorsCount.Add(color, 1);
            }
        }

        public void ChangeColorWhenNecessary(Bubble bubble)
        {
            var color = bubble.Color;
            var count = _colorsCount.ContainsKey(color) ? _colorsCount[color] : 0;
            
            if (count > 1) return;
            
            if (count == 1) _colorsCount.Remove(color);
            
            color = GetRandomColor();
            bubble.Color = GetRandomColor();
                
            _colorsCount[color]++;
        }

        private BubbleColor GetRandomColor()
        {
            var index = Random.Range(0, _colorsCount.Count);
            
            return _colorsCount.Keys.ElementAt(index);
        }

        #endregion
    }
}