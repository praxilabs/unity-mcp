using System.Collections;
using System.Collections.Generic;
using Table.UI.Views.TableViewStructure;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.Views.TableViewElements.CellTypes
{
    public abstract class BaseTableCell : MonoBehaviour
    {
        public event System.Action OnCellUpdate;

        [SerializeField] protected RectTransform _rectTransform;
        private Vector2 _localPosition;

        /// <summary>
        /// Row this cell belongs to
        /// </summary>
        protected Vector2 _sizeDelta;
        public Row Row { get; set; }

        public int RowNumber { get; set; }
        /// <summary>
        /// Current actual width of the cell.
        /// </summary>
        public float Width
        {
            get => _rectTransform.sizeDelta.x;
            set
            {

                _rectTransform.anchorMin = Vector2.up;
                _rectTransform.anchorMax = Vector2.up;
                _rectTransform.pivot = Vector2.up;

                _sizeDelta = _rectTransform.sizeDelta;
                _sizeDelta.x = value;
                _rectTransform.sizeDelta = _sizeDelta;

            }
        }
        /// <summary>
        /// Current actual height of the cell
        /// </summary>
        public float Height
        {
            get => _rectTransform.sizeDelta.y;
            set
            {
                _sizeDelta = _rectTransform.sizeDelta;
                _sizeDelta.y = value;
                _rectTransform.sizeDelta = _sizeDelta;
            }
        }
        public float PositionX
        {
            get => _rectTransform.localPosition.x;
            set
            {
                _rectTransform.anchorMin = Vector2.up;
                _rectTransform.anchorMax = Vector2.up;
                _rectTransform.pivot = Vector2.up;
                _localPosition = _rectTransform.localPosition;
                _localPosition.x = value;

                _rectTransform.localPosition = _localPosition;
            }
        }
        public float PositionY
        {
            get => _rectTransform.localPosition.y;
            set
            {
                _localPosition = _rectTransform.localPosition;
                _localPosition.y = value;
                _rectTransform.localPosition = _localPosition;
            }
        }
        protected void OnCellUpdateInvoke() => OnCellUpdate?.Invoke();
        /// <summary>
        /// The width preferred for this specific cell type. Implemented for each cell type.
        /// ex. text would grow when you type more, shrink when you delete letters, adjusting to fit all text.
        /// </summary>
        public abstract float PreferredWidth { get; }// => cellText.preferredWidth;
        /// <summary>
        /// The size that is preferred for this specific cell type. implemented for each type
        /// ex. multiline text would grow vertically with new lines, adjusting to fit all text.
        /// </summary>
        public abstract float PreferredHeight { get; }// => cellText.preferredHeight;
        public Table.Structure.TableField TableField { get; set; }
        /// <summary>
        /// Initialization needed for this cell type.
        /// </summary>
        public virtual void Initialize()
        {
            TableField.onValueChange += UpdateVisual;
            UpdateVisual(TableField.Value);
        }
        /// <summary>
        /// Opposite of initialize. called at the end of life of this
        /// </summary>
        public virtual void Terminate()
        {
            TableField.onValueChange -= UpdateVisual;
        }

        /// <summary>
        /// Updates cell type UI object depending on the text value.
        /// ex: checkbox would set on if value is true.
        /// </summary>
        /// <param name="value"></param>
        protected abstract void UpdateVisual(string value);

    }
}