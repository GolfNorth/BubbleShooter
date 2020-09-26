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
        private bool _victory;

        private readonly int[][][] _neighborsOffsets =
        {
            new[] {new[] {1, 0}, new[] {0, 1}, new[] {-1, 1}, new[] {-1, 0}, new[] {-1, -1}, new[] {0, -1}},
            new[] {new[] {1, 0}, new[] {1, 1}, new[] {0, 1}, new[] {-1, 0}, new[] {0, -1}, new[] {1, -1}}
        };

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

            transform.position = new Vector3(-_width / 2, positionY);

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

        private void CheckFirstRow()
        {
            var columns = Context.Instance.Settings.Columns;
            var firstRowRatioVictory = Context.Instance.Settings.FirstRowRatioVictory;
            var filledNumber = 0;

            foreach (var tile in _tiles[0])
                if (tile.Bubble != null)
                    filledNumber++;

            if ((float) filledNumber / columns <= firstRowRatioVictory)
            {
                Context.Instance.NotificationService.Notify(NotificationType.Victory);
                _victory = true;

                UnstickAll();
            }
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

        public void UnstickBubble(Coordinate coordinate)
        {
            var anchor = _tiles[coordinate.Row][coordinate.Column].Anchor;
            var bubble = _tiles[coordinate.Row][coordinate.Column].Bubble;

            bubble.Unstick();

            _tiles[coordinate.Row][coordinate.Column].Anchor = null;
            _tiles[coordinate.Row][coordinate.Column].Bubble = null;

            _anchorPool.Push(anchor.AnchorObject);

            if (!_victory && coordinate.Row == 0) CheckFirstRow();
        }

        public void UnstickAll()
        {
            for (var r = 0; r < _tiles.Count; r++)
            for (var c = 0; c < _tiles[r].Length; c++)
                UnstickBubble(new Coordinate(r, c));
        }

        private List<Tile> GetNeighbors(Tile tile)
        {
            var tileRow = tile.Coordinate.Row % 2;
            var neighbors = new List<Tile>();

            var n = _neighborsOffsets[tileRow];

            for (var i = 0; i < n.Length; i++)
            {
                var nx = tile.Coordinate.Column + n[i][0];
                var ny = tile.Coordinate.Row + n[i][1];

                if (nx >= 0 && nx < _columns && ny >= 0 && ny < _rows) neighbors.Add(_tiles[nx][ny]);
            }

            return neighbors;
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