using UnityEngine;

public class RandomAnimationStart : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Отримуємо інформацію про поточний стан анімації
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            // Задаємо випадкову точку старту (від 0.0 до 1.0, де 1.0 — це кінець циклу)
            animator.Play(state.fullPathHash, -1, Random.value);
        }
    }
}