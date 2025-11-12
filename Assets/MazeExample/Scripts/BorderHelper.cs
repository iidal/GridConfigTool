using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderHelper : MonoBehaviour
{
    [SerializeField] private GameObject m_object;
    public void Resize(Vector3 newScale)
    {
        m_object.transform.localScale = newScale;
    }
}
