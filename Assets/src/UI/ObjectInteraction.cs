using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public GameObject MenuObject;
    private Material originMaterial;
    public Material highlightMaterial;

    private Renderer objectRenderer;


    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originMaterial = objectRenderer.material;
        MenuObject.SetActive(false);
    }


    void Update()
    {
        // Erstelle den Ray von der Mausposition
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // �berpr�fe, ob der Raycast auf ein Objekt trifft
        if (Physics.Raycast(ray, out hit))
        {
            // Wenn die Maus �ber dem aktuellen Objekt ist
            if (hit.collider.gameObject == gameObject)
            {
                // �ndere das Material auf das Highlight-Material
                objectRenderer.material = highlightMaterial;

                // �ffne das zugewiesene Men� bei Klick
                if (Input.GetMouseButtonDown(0))
                {
                    MenuObject.SetActive(true); // �ffne das dem Objekt zugewiesene Men�
                }
            }
            else
            {
                // Setze das Material zur�ck, wenn die Maus das Objekt verl�sst
                objectRenderer.material = originMaterial;
            }
        }
        else
        {
            // Setze das Material zur�ck, wenn der Ray nichts trifft
            objectRenderer.material = originMaterial;
        }
    }
}
