using System.Collections.Generic;
using UnityEngine;
using Youregone.Factories;
using Youregone.LevelGeneration;

namespace Youregone.ObjectPooling
{
    public class RockBreakPool
    {
        private Queue<RockBreakPiece> _rockPiecePool = new();

        private readonly Factory<RockBreakPiece> _rockPieceFactory;
        private readonly RockBreakPiece _rockPiecePrefab;

        public RockBreakPool(Factory<RockBreakPiece> RockPieceFactory, RockBreakPiece rockPiecePrefab)
        {
            _rockPieceFactory = RockPieceFactory;
            _rockPiecePrefab = rockPiecePrefab;
        }

        public void Enqueue(RockBreakPiece rockPiece)
        {
            rockPiece.gameObject.SetActive(false);
            _rockPiecePool.Enqueue(rockPiece);
        }

        public RockBreakPiece Dequeue(Transform parent = null)
        {
            RockBreakPiece rockBreakPiece;

            if (_rockPiecePool.Count == 0)
                rockBreakPiece = _rockPieceFactory.Create(_rockPiecePrefab);
            else
                rockBreakPiece = _rockPiecePool.Dequeue();

            if (parent != null)
                rockBreakPiece.transform.parent = parent;

            rockBreakPiece.gameObject.SetActive(true);
            return rockBreakPiece;
        }
    }
}