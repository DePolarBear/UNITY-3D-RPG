using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionScript : MonoBehaviour
{
    public InputActionReference interact;
    public float dosah = 3f;

    private NpcScript blizkeNpc;

    public DialogUIScript dialog;

    private void Update()
    {
        // ked je dialog otvoreny, E ho zavrie
        if (dialog.JeOtvoreny)
        {
            if (interact.action.WasPressedThisFrame())
            {
                dialog.Skry();
            }

            return;
        }

        blizkeNpc = NajdiNpc();

        if (blizkeNpc != null && interact.action.WasPressedThisFrame())
        {
            dialog.Zobraz(blizkeNpc.meno, blizkeNpc.text);
        }
    }

    private NpcScript NajdiNpc()
    {
        Collider[] okolo = Physics.OverlapSphere(transform.position, dosah);

        NpcScript najblizsie = null;
        float najlepsia = float.MaxValue;

        foreach (Collider c in okolo)
        {
            NpcScript npc = c.GetComponentInParent<NpcScript>();

            if (npc == null)
            {
                continue;
            }

            float d = Vector3.Distance(transform.position, npc.transform.position);

            if (d < najlepsia)
            {
                najlepsia = d;
                najblizsie = npc;
            }
        }

        return najblizsie;
    }
}