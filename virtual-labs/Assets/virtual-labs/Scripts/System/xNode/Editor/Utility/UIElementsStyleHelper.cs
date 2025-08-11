using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Praxilabs.xNode
{
    public static class UIElementsStyleHelper
    {
        public static void SetVisualElementStyle(VisualElement visualElement, float width, float height, string hexBackgroundColor, FlexDirection direction)
        {
            IStyle style = visualElement.style;
            style.width = Length.Percent(width);
            style.height = Length.Percent(height);

            style.backgroundColor = ColorUtility.TryParseHtmlString(hexBackgroundColor, out var color) ? color : Color.clear;
            style.flexDirection = direction;
        }

        public static void SetVisualElementTextureBG(string texturePath, VisualElement parent, float width, float height)
        {
            parent.style.backgroundImage = Background.FromTexture2D((Texture2D)Resources.Load<Texture>(texturePath));

            IStyle imageStyle = parent.style;
            imageStyle.width = Length.Percent(width);
            imageStyle.height = Length.Percent(height);
        }

        public static void SetButtonStyle(Button button, float width, float height, LengthUnit widthUnit, LengthUnit heightUnit, string hexBackgroundColor, string hexTextColor, string hexBorderColor)
        {
            IStyle buttonStyle = button.style;
            buttonStyle.width = new Length(width, widthUnit);
            buttonStyle.height = new Length(height, heightUnit);

            Color buttonBG = new Color();
            ColorUtility.TryParseHtmlString(hexBackgroundColor, out buttonBG);
            buttonStyle.backgroundColor = buttonBG;

            Color textColor = new Color();
            ColorUtility.TryParseHtmlString(hexTextColor, out textColor);
            buttonStyle.color = textColor;

            Color borderColor = new Color();
            ColorUtility.TryParseHtmlString(hexBorderColor, out borderColor);
            buttonStyle.borderTopColor = borderColor;
            buttonStyle.borderBottomColor = borderColor;
            buttonStyle.borderLeftColor = borderColor;
            buttonStyle.borderRightColor = borderColor;
        }

        public static void AddButtonIcon(string texturePath, Button button)
        {
            if (button.childCount > 0) button.RemoveAt(0);

            var iconTexture = Resources.Load<Texture>(texturePath);
            if (iconTexture == null)
            {
                Debug.LogError("Icon texture not found in Resources folder.");
                return;
            }

            button.style.justifyContent = Justify.Center;
            button.style.alignItems = Align.Center;

            var icon = new VisualElement
            {
                style =
                        {
                            backgroundImage = new StyleBackground((Texture2D)iconTexture),
                            width = new Length(80, LengthUnit.Percent),
                            height = new Length(80, LengthUnit.Percent)
                        }
            };

            button.text = string.Empty;
            button.Add(icon);
        }

        public static void SetSearchBarStyle(ToolbarSearchField searchField, float width)
        {
            IStyle buttonStyle = searchField.style;
            buttonStyle.width = Length.Percent(width);
        }

        public static void SetMargin(VisualElement visualElement, float marginTop, float marginBottom, float marginLeft, float marginRight)
        {
            IStyle visualElementStyle = visualElement.style;
            visualElementStyle.marginTop = Length.Percent(marginTop);
            visualElementStyle.marginBottom = Length.Percent(marginBottom);
            visualElementStyle.marginLeft = Length.Percent(marginLeft);
            visualElementStyle.marginRight = Length.Percent(marginRight);
        }

        public static void SetPadding(VisualElement visualElement, float paddingTop, float paddingBottom, float paddingLeft, float paddingRight)
        {
            IStyle visualElementStyle = visualElement.style;
            visualElementStyle.paddingTop = Length.Percent(paddingTop);
            visualElementStyle.paddingBottom = Length.Percent(paddingBottom);
            visualElementStyle.paddingLeft = Length.Percent(paddingLeft);
            visualElementStyle.paddingRight = Length.Percent(paddingRight);
        }
    }
}