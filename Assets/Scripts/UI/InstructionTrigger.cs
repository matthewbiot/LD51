using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstructionTrigger : MonoBehaviour
{
    [SerializeField] Instruction m_Instruction;
    [SerializeField] float m_FadeTime;
    [SerializeField] bool m_Show;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;

        Debug.Log("keyboard: " + Gamepad.current == null);
        if (m_Show)
            m_Instruction.Show(m_FadeTime);
        else
            m_Instruction.Hide(m_FadeTime);
    }
}
