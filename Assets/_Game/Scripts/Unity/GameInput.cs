using UnityEngine;
using UnityEngine.InputSystem;

namespace RockPaperPistol.Unity
{
    public static class GameInput
    {
        public static bool EscapePressed =>
            Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        public static bool LeftClickPressed =>
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        public static float MouseScrollY =>
            Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;

        public static Vector3 MouseScreenPosition
        {
            get
            {
                if (Mouse.current == null)
                {
                    return Vector3.zero;
                }

                Vector2 position = Mouse.current.position.ReadValue();
                return new Vector3(position.x, position.y, 0f);
            }
        }

        public static bool ImguiButton(string text, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), GUI.skin.button, options);
            Vector2 pointer = GuiPointer();
            bool hover = rect.Contains(pointer);
            if (Event.current.type == EventType.Repaint)
            {
                Color previous = GUI.color;
                if (hover)
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f);
                }

                GUI.Box(rect, text, GUI.skin.button);
                GUI.color = previous;
            }

            return Event.current.type == EventType.Repaint && LeftClickPressed && hover;
        }

        private static Vector2 GuiPointer()
        {
            Vector3 screen = MouseScreenPosition;
            return GUIUtility.ScreenToGUIPoint(new Vector2(screen.x, screen.y));
        }
    }
}
