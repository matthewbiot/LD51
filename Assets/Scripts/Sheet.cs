using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Sheet : ScriptableObject
{
    [SerializeField] List<Sprite> m_Sprites;
    [SerializeField] int m_FrameRate;

    public List<Sprite> Sprites => m_Sprites;
    public int FrameRate => m_FrameRate;
}
