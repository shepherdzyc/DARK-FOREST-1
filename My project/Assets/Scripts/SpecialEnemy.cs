using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpecialEnemy : Enemy
{
    public override void Die()
    {
        // 死亡后，在周围生成其他敌人
        GameUtils.AddPosPair(row, col);

        base.Die();
    }

    // 敌人沿x轴移动过程
    IEnumerator MoveAnim(GameObject enemy, Vector3 target)
    {
        Vector3 startPosition = transform.position;
        float journey = 0f;
        float duration = 1f; // 移动所需时间，可以根据需要调整

        while (journey < duration)
        {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);
            transform.position = Vector3.Lerp(startPosition, target, percent);
            yield return null;
        }

        // 确保最后位置精确设置为目标位置
        transform.position = target;
    }
}
