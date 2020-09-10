using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PacMan
{
    public class DotsController : MonoBehaviour
    {
        public Tilemap rails;
        public TileBase normalDot;
        public TileBase powerDot;
        public Transform aggressiveCornerPoint;
        public Transform pincerCornerPoint;
        public Transform shyCornerPoint;
        public Transform whimsicalCornerPoint;
        public int NumberOfDots { get; private set; }
        public int NumberOfDotsConsumed { get; private set; }

        public bool AllDotsConsumed => NumberOfDots == NumberOfDotsConsumed;

        public const string DotName = "dot";
        public const string PowerDotName = "power_dot";

        private List<Vector3Int> _railPositions;

        private Tilemap _dotTilemap;
        private Tilemap DotTilemap
        {
            get
            {
                if (_dotTilemap == null)
                    _dotTilemap = GetComponent<Tilemap>();

                return _dotTilemap;
            }
        }

        private void Start()
        {
            ResetDots();
        }

        public DotType PlayerMovedToPosition(Vector3Int position)
        {
            if (!DotTilemap.HasTile(position)) return DotType.None;
            
            var currentDotName = DotTilemap.GetTile(position).name;
            DotTilemap.SetTile(position, null);
            NumberOfDotsConsumed++;
            switch (currentDotName)
            {
                case DotName:
                    return DotType.Normal;
                case PowerDotName:
                    return DotType.Power;
                default:
                    throw new Exception("Unexpected dot name");
            }
        }

        public void ResetDots()
        {
            NumberOfDotsConsumed = 0;
            
            if (_railPositions == null) GenerateRailPositions();

            var dotPositions = _railPositions.ToArray();
            var tiles = new TileBase[dotPositions.Length];

            var enemyCornerPositions = new[]
            {
                aggressiveCornerPoint.position,
                pincerCornerPoint.position,
                shyCornerPoint.position,
                whimsicalCornerPoint.position
            };
            var closestIndices = new int[4];
            var closestDistances = new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue};
            
            for (var i = 0; i < tiles.Length; i++)
            {
                // Find the closest positions to enemy corner points
                for (int e = 0; e < enemyCornerPositions.Length; e++)
                {
                    var distance = Vector3.Distance(dotPositions[i], enemyCornerPositions[e]);
                    if (distance < closestDistances[e])
                    {
                        closestDistances[e] = distance;
                        closestIndices[e] = i;
                    }
                }

                // By default set a normal dot on each of the rail positions
                tiles[i] = normalDot;
            }

            // Set the position closest to the enemy's corner point to be a power dot
            foreach (var index in closestIndices)
            {
                tiles[index] = powerDot;
            }

            // Put a dot on all the rail positions
            DotTilemap.SetTiles(dotPositions, tiles);
        }

        private void GenerateRailPositions()
        {
            _railPositions = new List<Vector3Int>();
            BoundsInt bounds = rails.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                Tile tile = rails.GetTile<Tile>(pos);
                if (tile != null)
                {
                    _railPositions.Add(pos);
                }
            }

            NumberOfDots = _railPositions.Count;
        }
    }

    public enum DotType
    {
        None,
        Normal,
        Power
    }
}