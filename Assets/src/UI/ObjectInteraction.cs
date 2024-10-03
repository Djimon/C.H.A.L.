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

        // Überprüfe, ob der Raycast auf ein Objekt trifft
        if (Physics.Raycast(ray, out hit))
        {
            // Wenn die Maus über dem aktuellen Objekt ist
            if (hit.collider.gameObject == gameObject)
            {
                // Ändere das Material auf das Highlight-Material
                objectRenderer.material = highlightMaterial;

                // Öffne das zugewiesene Menü bei Klick
                if (Input.GetMouseButtonDown(0))
                {
                    MenuObject.SetActive(true); // Öffne das dem Objekt zugewiesene Menü
                }
            }
            else
            {
                // Setze das Material zurück, wenn die Maus das Objekt verlässt
                objectRenderer.material = originMaterial;
            }
        }
        else
        {
            // Setze das Material zurück, wenn der Ray nichts trifft
            objectRenderer.material = originMaterial;
        }
    }
}
