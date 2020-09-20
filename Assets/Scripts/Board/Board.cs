using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class Board : MonoBehaviour
    {
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

        public void Initialize(Level level)
        {
            _rows = level.Rows;
            _columns = level.Columns;
            _boardOffset = Context.Instance.Settings.BoardOffset;
            _boardSpeed = Context.Instance.Settings.BoardSpeed;
            _bubbleRadius = Context.Instance.Settings.BubbleRadius;

            InstantiateEffector();
            CalculateDimensions();
            RelocateBoard();
            InitializeTiles();
            InitializePool();

            _isInitialized = true;
        }

        public bool CollisionCheck(Vector2 position)
        {
            var coordinate = transform.InverseTransformPoint(position).ToCoordinate();

            if (_tiles.Count <= coordinate.Row || _tiles[coordinate.Row].Length <= coordinate.Column) return false;

            var tile = _tiles[coordinate.Row][coordinate.Column];

            if (tile.BubbleObject is null) return false;

            var center = tile.BubbleObject.Bubble.transform.position;

            return BubbleCollisionCheck(position, center);
        }

        private bool BubbleCollisionCheck(Vector2 position, Vector2 center)
        {
            var distX = position.x - center.x;
            var distY = position.y - center.y;
            var distance = Mathf.Sqrt(distX * distX + distY * distY);
        
            return distance <= _bubbleRadius;
        }

        private void InstantiateEffector()
        {
            _effector = Instantiate(Context.Instance.Settings.EffectorPrefab);
            _effector.SetActive(false);
        }

        private void RelocateBoard()
        {
            var position = transform.position;
            position.x = - _width / 2;
            position.y = Context.Instance.BoundsService.Bounds.Bottom;

            transform.position = position;
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

        public void AddBubble(Coordinate coordinate, BubbleObject bubbleObject)
        {
            var anchorObject = _anchorPool.Pop(coordinate);
            
            bubbleObject.Bubble.Stick(anchorObject.Anchor.Rigidbody);
            
            _tiles[coordinate.Row][coordinate.Column].AnchorObject = anchorObject;
            _tiles[coordinate.Row][coordinate.Column].BubbleObject = bubbleObject;
        }

        public void StickBubble(Coordinate coordinate, BubbleObject bubbleObject)
        {
            var anchorObject = _anchorPool.Pop(coordinate);
            
            bubbleObject.Bubble.Stick(anchorObject.Anchor.Rigidbody);

            while (coordinate.Row >= _tiles.Count)
            {
                _tiles.Add(new Tile[_columns]);

                _rows++;
                
                CalculateDimensions();
            }

            _tiles[coordinate.Row][coordinate.Column].AnchorObject = anchorObject;
            _tiles[coordinate.Row][coordinate.Column].BubbleObject = bubbleObject;

            StartCoroutine(ActivateEffector(anchorObject.Anchor.transform.position));
        }

        public void UnstickBubble(Coordinate coordinate)
        {
            var anchorObject = _tiles[coordinate.Row][coordinate.Column].AnchorObject;
            var bubbleObject = _tiles[coordinate.Row][coordinate.Column].BubbleObject;
            
            bubbleObject.Bubble.Unstick();
            
            _tiles[coordinate.Row][coordinate.Column].AnchorObject = null;
            _tiles[coordinate.Row][coordinate.Column].BubbleObject = null;
            
            _anchorPool.Push(anchorObject);
        }

        private void OnDisable()
        {
            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized) return;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized) return;
            
            var position = transform.position;
            var targetY = Context.Instance.BoundsService.Bounds.Bottom + _boardOffset + _height;

            if (targetY < position.y)
            {
                position.y -= _boardSpeed * Time.fixedDeltaTime;
                position.y = position.y < targetY ? targetY : position.y;
            }
            else if (targetY > position.y)
            {
                position.y += _boardSpeed * Time.fixedDeltaTime;
                position.y = position.y > targetY ? targetY : position.y;
            }

            transform.position = position;
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