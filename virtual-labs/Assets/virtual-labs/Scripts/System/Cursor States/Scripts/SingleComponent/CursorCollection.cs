using UnityEngine;
using System.Collections.Generic;
namespace CursorStates
{
    [CreateAssetMenu(fileName = "GameObjectsCursorSprites", menuName = "Cursor/Sprites Data")]
    public class CursorsCollection : ScriptableObject
    {
        [SerializeField] private List<CursorData> cursorsAvailable;
        private Dictionary<CursorData.CursorSpriteKey, CursorData> _cursors = new();

        public CursorData this[CursorData.CursorSpriteKey name]
        {
            get
            {
                if (!_cursors.ContainsKey(name))
                    if (cursorsAvailable.Exists(x => x.CursorName == name))
                        _cursors.Add(name, cursorsAvailable.Find(x => x.CursorName == name));
                    else
                    {
                        Debug.LogError($"Cursor ${name} not found in collection!");
                        return null;
                    }
                return _cursors[name];
            }
        }
    }
}