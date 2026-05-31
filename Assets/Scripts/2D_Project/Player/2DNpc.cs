using UnityEngine;

public class Npc : MonoBehaviour
{
    [Header("애니메이터")]
    [SerializeField] private EntityAnimatorController AnimatorController_Entity;


    public void ChangeNpcState(EntityAnimState newState)
    {
        AnimatorController_Entity.SetState(newState);
    }


}
