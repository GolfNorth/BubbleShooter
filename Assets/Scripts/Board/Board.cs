using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class Board : MonoBehaviour
    {
        private LevelController _levelController;
        private GameObject _effector;
        private BubblePool _bubblePool;
        private AnchorPool _anchorPool;
        private bool _isInitialized;
        private int _rows;
        private int _columns;
        private float _width;
        private float _height;
        private List<Tile[]> _tiles;
        private float _bubbleRadius;
        private float _boardOffset;
        private float _boardSpeed;
        private int _bubbleCount;

        public void Initialize()
        {
            _levelController = Context.Instance.LevelController;
            
            _rows = _levelController.Level.Rows;
            _columns = _levelController.Level.Columns;
            _boardOffset = Context.Instance.Settings.BoardOffset;
            _boardSpeed = Context.Instance.Settings.BoardSpeed;
            _bubbleRadius = Context.Instance.Settings.BubbleRadius;

            InstantiateEffector();
            InitializeTiles();
            InitializePool();
            
            CalculateDimensions();
            RelocateBoard();

            _isInitialized = true;
        }

        private void InstantiateEffector()
        {
            _effector = Instantiate(Context.Instance.Settings.EffectorPrefab);
            _effector.SetActive(false);
        }

        private void RelocateBoard()
        {
            var positionY = Context.Instance.BoundsService.Bounds.Bottom + _boardOffset + _height;
            
            transform.position = new Vector3(- _width / 2, positionY);

            Context.Instance.BoundsService.SetTopBound(positionY);
        }

        private void CalculateDimensions()
        {
            _width = _columns * _bubbleRadius * 2 + _bubbleRadius;
            _height = _rows * _bubbleRadius * 2;
        }
        
        private void InitializeTiles()
        {
            _tiles = new List<Tile[]>();

            for (var i = 0; i < _rows; i++)
                _tiles.Add(new Tile[_columns]);
        }

        private void InitializePool()
        {
            var poolCount = _rows * _columns;
            var tilePrefab = Context.Instance.Settings.AnchorPrefab;
            
            _anchorPool = AnchorPool.GetObjectPool(tilePrefab, poolCount);
            _anchorPool.transform.SetParent(transform);
            _anchorPool.transform.localPosition = Vector3.zero;
        }

        public void AddBubble(Bubble bubble, Coordinate coordinate)
        {
            var anchorObject = _anchorPool.Pop(coordinate);
            
            bubble.Stick(anchorObject.Anchor);
            
            _tiles[coordinate.Row][coordinate.Column].Anchor = anchorObject.Anchor;
            _tiles[coordinate.Row][coordinate.Column].Bubble = bubble;
        }
        
        public void RemoveBubble(Coordinate coordinate)
        {
            var anchor = _tiles[coordinate.Row][coordinate.Column].Anchor;
            var bubble = _tiles[coordinate.Row][coordinate.Column].Bubble;
            
            bubble.Unstick();
            
            _tiles[coordinate.Row][coordinate.Column].Anchor = null;
            _tiles[coordinate.Row][coordinate.Column].Bubble = null;
            
            _anchorPool.Push(anchor.AnchorObject);
        }

        public void StickBubble(Bubble bubble)
        {
            var coordinate = bubble.transform.position.ToCoordinateWorld();

            if (coordinate.Row >= _tiles.Count)
            {
                _tiles.Add(new Tile[_columns]);
            
                _rows++;
                
                CalculateDimensions();
                RelocateBoard();
            }
            
            AddBubble(bubble, coordinate);
            
            var position = _tiles[coordinate.Row][coordinate.Column].Anchor.transform.position;
            
            StartCoroutine(ActivateEffector(position));
        }

        private IEnumerator ActivateEffector(Vector3 position)
        {
            _effector.transform.position = position;
            _effector.SetActive(true);

            yield return new WaitForSeconds(Context.Instance.Settings.EffectorDuration);
            
            _effector.SetActive(false);
        }
    }
}