using UnityEngine;
using System.Linq;
using System;
/*
    Model class 
    Contains all the game data
*/
namespace Match3.Model
{
    public class Token
    {
        public enum TOKEN_TYPE { BLUE, BROWN, ORANGE, PINK, RED, WHITE };
        protected TOKEN_TYPE type;
        public TOKEN_TYPE Type { get { return type; } set { type = value; } }
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } set { prefab = value; } }
        protected int row;
        public int Row { get { return row; } set { row = value; } }
        protected int column;
        public int Column { get { return column; } set { column = value; } }
    }
    public class ModelGameplay : MonoBehaviour
    {
        public int Width;
        public int Height;
        [Range(0,1)]public float spawnTime;
        public int minChainLength;
        [NonSerialized] public int maxTokens = (int)Enum.GetValues(typeof(Token.TOKEN_TYPE)).Cast<Token.TOKEN_TYPE>().Max();
        public int initialMoves;
        [NonSerialized] public int moves;
        [NonSerialized] public int score;
        public int scoreMultiplier;
        public enum SPAWN_TYPE {Normal,Advanced,Pro };
        [SerializeField] public SPAWN_TYPE SpawnType; 
        public Token[,] tokens;
        public GameObject tokenPrefab;
        public GameObject tilePrefab;
    }
}
