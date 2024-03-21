using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SubMarineAnimation : MonoBehaviour
{
    [SerializeField]
    public Image m_Image;
    [SerializeField]
    public Sprite[] m_SpriteArray;
    [SerializeField]
    public float m_Speed = .02f;
    [SerializeField]
    private int m_IndexSprite;
    [SerializeField]
    Coroutine m_CorotineAnim;
    void Start()
    {
        StartCoroutine(Func_PlayAnimUI());
    }

    IEnumerator Func_PlayAnimUI()
    {
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteArray.Length)
        {
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteArray[m_IndexSprite];
        m_IndexSprite += 1;
    }
}
