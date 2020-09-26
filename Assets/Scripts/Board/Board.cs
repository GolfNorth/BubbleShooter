using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class Board : MonoBehaviour
    {
        #region Fields

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
        private int _fallingBubbles;
        private bool _victory;

        private readonly int[][][] _neighborsOffsets =
        {
            new[] {new[] {1, 0}, new[] {0, 1}, new[] {-1, 1}, new[] {-1, 0}, new[] {-1, -1}, new[] {0, -1}},
            new[] {new[] {1, 0}, new[] {1, 1}, new[] {0, 1}, new[] {-1, 0}, new[] {0, -1}, new[] {1, -1}}
        };

        #endregion

        #region Methods

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
            
            Context.Instance.NotificationService.Notification += OnNotification;
        }

        private void OnDisable()
        {
            Context.Instance.NotificationService.Notification -= OnNotification;
        }

        private void OnNotification(NotificationType notificationType, object obj)
        {
            if (notificationType == NotificationType.BubbleFell)
            {
                _fallingBubbles--;
                
                if (_fallingBubbles == 0)
                {
                    RemoveEmptyTiles();
                    
                    Context.Instance.NotificationService.Notify(NotificationType.BoardReady);
                }
            }
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

            for (var r = 0; r < _rows; r++) AddNewRow();
        }

        private void AddNewRow()
        {
            _tiles.Add(new Tile[_columns]);

            var r = _tiles.Count - 1;

            for (var c = 0; c < _columns; c++)
                _tiles[r][c] = new Tile();
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

        public Bubble RemoveBubble(Coordinate coordinate)
        {
            var anchor = _tiles[coordinate.Row][coordinate.Column].Anchor;
            var bubble = _tiles[coordinate.Row][coordinate.Column].Bubble;

            _tiles[coordinate.Row][coordinate.Column].Anchor = null;
            _tiles[coordinate.Row][coordinate.Column].Bubble = null;

            if (anchor) _anchorPool.Push(anchor.AnchorObject);

            return bubble;
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

            StickBubble(bubble, coordinate);
        }

        private void StickBubble(Bubble bubble, Coordinate coordinate)
        {
            if (coordinate.Row >= _tiles.Count)
            {
                AddNewRow();

                _rows++;
            }

            AddBubble(bubble, coordinate);

            var cluster = GetCluster(coordinate, true, true);

            var position = _tiles[coordinate.Row][coordinate.Column].Anchor.transform.position;

            StartCoroutine(ActivateEffector(position));
            
            if (cluster.Count >= 3)
            {
                foreach (var tile in cluster) BurstsBubble(tile.Coordinate);

                var floatingClusters = GetFloatingClusters();

                foreach (var floatingCluster in floatingClusters)
                foreach (var tile in floatingCluster)
                    UnstickBubble(tile.Coordinate);
            }

            if (_fallingBubbles == 0)
            {
                RemoveEmptyTiles();
                
                Context.Instance.NotificationService.Notify(NotificationType.BoardReady);
            }
        }

        public void ReplaceBubble(Bubble stickedBubble, Bubble newBubble)
        {
            var coordinate = stickedBubble.transform.localPosition.ToCoordinateLocal();

            BurstsBubble(coordinate);
            StickBubble(newBubble, coordinate);
        }

        public void BurstsBubble(Coordinate coordinate)
        {
            var bubble = RemoveBubble(coordinate);

            if (!bubble) return;

            bubble.Burst();

            if (!_victory && coordinate.Row == 0) CheckFirstRow();
        }

        public void UnstickBubble(Coordinate coordinate)
        {
            var bubble = RemoveBubble(coordinate);

            if (!bubble) return;

            bubble.Unstick();

            _fallingBubbles++;

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
                var nColumn = tile.Coordinate.Column + n[i][0];
                var nRow = tile.Coordinate.Row + n[i][1];

                if (nColumn >= 0 && nColumn < _columns && nRow >= 0 && nRow < _rows && _tiles[nRow][nColumn].Anchor)
                    neighbors.Add(_tiles[nRow][nColumn]);
            }

            return neighbors;
        }

        private List<Tile> GetCluster(Coordinate coordinate, bool resetProcessed, bool matchType)
        {
            if (resetProcessed) ResetProcessed();

            var tile = _tiles[coordinate.Row][coordinate.Column];
            var color = tile.Bubble.Color.Color;

            var queue = new Queue<Tile>();
            var cluster = new List<Tile>();

            tile.Processed = true;
            queue.Enqueue(tile);

            while (queue.Count > 0)
            {
                var currentTile = queue.Dequeue();

                if (!currentTile.Bubble) continue;

                var currentColor = currentTile.Bubble.Color.Color;

                if (!matchType || currentColor == color)
                {
                    cluster.Add(currentTile);

                    var neighbors = GetNeighbors(currentTile);

                    foreach (var neighbor in neighbors)
                        if (!neighbor.Processed)
                        {
                            neighbor.Processed = true;
                            queue.Enqueue(neighbor);
                        }
                }
            }

            return cluster;
        }

        private List<List<Tile>> GetFloatingClusters()
        {
            ResetProcessed();

            var clusters = new List<List<Tile>>();

            for (var r = 0; r < _rows; r++)
            for (var c = 0; c < _columns; c++)
            {
                var tile = _tiles[r][c];

                if (tile.Processed || !tile.Anchor) continue;

                var foundCluster = GetCluster(tile.Coordinate, false, false);

                if (foundCluster.Count <= 0) continue;

                var floating = true;

                foreach (var foundTile in foundCluster)
                    if (foundTile.Coordinate.Row == 0)
                    {
                        floating = false;
                        break;
                    }

                if (floating) clusters.Add(foundCluster);
            }

            return clusters;
        }

        private void ResetProcessed()
        {
            for (var r = 0; r < _rows; r++)
            for (var c = 0; c < _columns; c++)
                _tiles[r][c].Processed = false;
        }

        private void RemoveEmptyTiles()
        {
            if (_rows == 0) return;
            
            for (var r = _rows - 1; r >= 0; r--)
            {
                var remove = true;

                for (var c = 0; c < _columns; c++)
                {
                    if (!_tiles[r][c].Anchor) continue;

                    remove = false;
                }

                if (!remove) continue;

                _tiles.Remove(_tiles[r]);

                _rows--;
            }
            
            CalculateDimensions();
            RelocateBoard();
        }

        private IEnumerator ActivateEffector(Vector3 position)
        {
            _effector.transform.position = position;
            _effector.SetActive(true);

            yield return new WaitForSeconds(Context.Instance.Settings.EffectorDuration);

            _effector.SetActive(false);
        }

        #endregion
    }
}