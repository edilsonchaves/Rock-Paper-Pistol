using RockPaperPistol.Unity.Battle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RockPaperPistol.Managers
{
    public class MouseClickManager : MonoBehaviour
    {
        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (hit == null)
                return;

            Debug.Log($"Cliquei em: {hit.gameObject.name}");

            CardView clickable = hit.GetComponent<CardView>();

            if (clickable != null)
                clickable.CardClicked();
        }
    }
    }
