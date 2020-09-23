﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BubbleShooter
{
    public sealed class BubbleController
    {
        private readonly LevelController _levelController;
        private readonly Dictionary<BubbleColor, int> _colorsCount;
        private BubblePool _bubblePool;

        public BubbleController()
        {
            _levelController = Context.Instance.LevelController;
            _colorsCount = new Dictionary<BubbleColor, int>();

            InitializePool();
        }

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
            var index = Random.Range(0, _colorsCount.Count);
            var color = _colorsCount.Keys.ElementAt(index);
            var bubbleObject = _bubblePool.Pop();
            bubbleObject.Bubble.Color = color;
            bubbleObject.Bubble.gameObject.SetActive(true);

            return bubbleObject;
        }

        public void RemoveBubble(BubbleObject bubbleObject)
        {
            _bubblePool.Push(bubbleObject);

            var color = bubbleObject.Bubble.Color;
            
            if (!_colorsCount.ContainsKey(color)) return;
                
            _colorsCount[color]--;

            if (_colorsCount[color] == 0)
                _colorsCount.Remove(color);
        }
        
        public void CreateBubbles(Level level)
        {
            for (var r = 0; r < level.Rows; r++)
            {
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
        }
    }
}