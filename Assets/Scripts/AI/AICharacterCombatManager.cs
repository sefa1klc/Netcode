using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    [Header("Detection")]
    [SerializeField] float _detectionRadius = 15f;
    [SerializeField] float _minDetectionAngle = -35f;
    [SerializeField] float _maxDetectionAngle = 35f;
    public void  FindATargetVialineOfSight(AICharacterManager aiCharacter)
    {
        if (_currentTarget != null) return;

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, _detectionRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if(targetCharacter == null) continue;
            
            if (targetCharacter == aiCharacter) continue;

            if(targetCharacter._isDead.Value) continue;

            if (WorldUtilityManager.Instance.CanDamageThisTarget(aiCharacter._characterGroup,targetCharacter._characterGroup))
            {
                Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float viewebleAngle = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                if(viewebleAngle > _minDetectionAngle && viewebleAngle < _maxDetectionAngle)
                {
                    aiCharacter._characterCombatManager._currentTarget = targetCharacter;
                } 
            }
        }
    }
}
