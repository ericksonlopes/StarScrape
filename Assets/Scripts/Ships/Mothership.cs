using System;
using UnityEngine;
using StarScrape.Core;

namespace StarScrape.Ships
{
    public class Mothership : MonoBehaviour
    {
        public static Mothership Instance { get; private set; }

        [Header("Stats")]
        [SerializeField] private float baseSpeed = 10f;
        [SerializeField] private float maxHealth = 100f;
        
        [Header("Movement")]
        [SerializeField] private float rotationSpeed = 150f;

        public float CurrentHealth { get; private set; }
        public float CurrentSpeed => baseSpeed; // Can be multiplied by upgrades later

        public event Action<float, float> OnHealthChanged; // Current, Max
        public event Action<float> OnSpeedChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            OnSpeedChanged?.Invoke(CurrentSpeed);
        }

         private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

            float moveX = 0f;

            // Usa o novo Input System (lendo diretamente o teclado)
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed || UnityEngine.InputSystem.Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
                if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed || UnityEngine.InputSystem.Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            }

            if (Mathf.Abs(moveX) > 0.01f)
            {
                // A e D giram a nave no próprio eixo
                transform.Rotate(0, 0, -moveX * rotationSpeed * Time.deltaTime);
            }
        }

        public void TakeDamage(float amount)
        {
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
            
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0)
            {
                GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
            }
        }

        public void UpgradeSpeed(float amount)
        {
            baseSpeed += amount;
            OnSpeedChanged?.Invoke(CurrentSpeed);
        }

        public void Heal(float amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
