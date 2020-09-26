using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    [CreateAssetMenu(fileName = "BubbleColorCollection", menuName = "Bubble Color Collection")]
    public sealed class BubbleColorCollection : ScriptableObject
    {
        #region Fields

        [SerializeField] private BubbleColor[] _colors;
        private Dictionary<string, BubbleColor> _colorsDictionary;

        #endregion

        #region Properties

        public BubbleColor this[string abbreviation]
        {
            get
            {
                abbreviation = abbreviation.ToUpper();

                if (_colorsDictionary is null)
                {
                    _colorsDictionary = new Dictionary<string, BubbleColor>();

                    foreach (var color in _colors)
                        _colorsDictionary.Add(color.Abbreviation.ToUpper(), color);
                }

                return _colorsDictionary.ContainsKey(abbreviation)
                    ? _colorsDictionary[abbreviation]
                    : null;
            }
        }

        #endregion
    }
}