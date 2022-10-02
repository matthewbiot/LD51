using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] AudioClip m_Clip;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play()
    {
        Debug.Log("Play");
        AudioManager.instance.PlaySFX(m_Clip);
    }
}
