using UnityEngine;

namespace CursorStates
{
    [System.Serializable]
    public class CursorData
    {
        [SerializeField] protected CursorSpriteKey _cursorName;
        [SerializeField] protected Texture2D _cursor;
        [SerializeField] protected Vector2 _offset;
        public CursorSpriteKey CursorName { get => _cursorName; set => _cursorName = value; }
        public Texture2D Cursor { get => _cursor; set => _cursor = value; }
        public Vector2 Offset { get => _offset; set => _offset = value; }
        public enum CursorSpriteKey
        {
            None,
            OpenHand,
            ClosedHand,
            Clickable
        }
    }
}