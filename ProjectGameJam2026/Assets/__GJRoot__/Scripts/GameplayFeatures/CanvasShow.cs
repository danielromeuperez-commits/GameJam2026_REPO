using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject panelA;
    [SerializeField] private GameObject panelB;

    public void CambiarPanel()
    {
        if (panelA == null || panelB == null)
        {
            Debug.LogError("PanelA o PanelB no están asignados en el Inspector");
            return;
        }

        panelA.SetActive(false);
        panelB.SetActive(true);

        Debug.Log("Cambio de panel ejecutado");
    }
}