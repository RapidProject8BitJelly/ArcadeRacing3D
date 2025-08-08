using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshColliderCleaner : EditorWindow
{
    [MenuItem("Tools/Usuń MeshCollidery bez dzieci")]
    public static void ShowWindow()
    {
        GetWindow<MeshColliderCleaner>("Usuń MeshCollidery");
    }

    private void OnGUI()
    {
        GUILayout.Label("Narzędzie do czyszczenia hierarchii", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("To narzędzie usunie wszystkie obiekty GameObject, które posiadają komponent MeshCollider, ale nie mają żadnych obiektów potomnych.", MessageType.Info);

        if (GUILayout.Button("Znajdź i Usuń Obiekty"))
        {
            FindAndRemoveMeshCollidersWithoutChildren();
        }
    }

    private void FindAndRemoveMeshCollidersWithoutChildren()
    {
        List<GameObject> objectsToRemove = new List<GameObject>();

        // Przeszukujemy wszystkie obiekty w scenie
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            // Sprawdzamy, czy obiekt ma MeshCollider
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();

            // Sprawdzamy, czy obiekt nie ma dzieci
            // Transform.childCount zwraca liczbę bezpośrednich dzieci obiektu
            if (meshCollider != null && obj.transform.childCount == 0)
            {
                objectsToRemove.Add(obj);
            }
        }

        if (objectsToRemove.Count > 0)
        {
            // Pytamy użytkownika o potwierdzenie przed usunięciem
            if (EditorUtility.DisplayDialog(
                "Potwierdzenie usunięcia",
                $"Znaleziono {objectsToRemove.Count} obiektów GameObject do usunięcia. Czy na pewno chcesz je usunąć?",
                "Tak, usuń",
                "Anuluj"))
            {
                foreach (GameObject obj in objectsToRemove)
                {
                    // Używamy DestroyImmediate w trybie edytora
                    // Aby operacja była cofnięta (undoable), dodajemy ją do bufora Undo
                    Undo.DestroyObjectImmediate(obj);
                }
                Debug.Log($"Usunięto {objectsToRemove.Count} obiektów z MeshColliderem i bez dzieci.");
            }
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Brak obiektów",
                "Nie znaleziono żadnych obiektów GameObject z MeshColliderem i bez dzieci.",
                "OK");
        }
    }
}