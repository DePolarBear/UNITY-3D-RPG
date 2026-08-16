using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionScript : MonoBehaviour
{
    public InputActionReference interact;
    public DialogUIScript dialog;
    public PlayerMovementScript pohyb;
    public float dosah = 3f;

    private NpcScript aktivneNpc;
    private int aktualnyUzol;

    private void OnEnable()
    {
        interact.action.Enable();
    }

    private void OnDisable()
    {
        interact.action.Disable();
    }

    private void Update()
    {
        if (dialog.JeOtvoreny)
        {
            SpracujOdpoved();
            return;
        }

        NpcScript blizke = NajdiNpc();

        if (blizke != null && blizke.uzly.Length > 0 && interact.action.WasPressedThisFrame())
        {
            aktivneNpc = blizke;
            aktualnyUzol = 0;

            dialog.Zobraz(aktivneNpc.meno, aktivneNpc.uzly[0]);
            pohyb.zablokovany = true;
        }
    }

    private void SpracujOdpoved()
    {
        DialogUzol uzol = aktivneNpc.uzly[aktualnyUzol];

        for (int i = 0; i < uzol.odpovede.Length; i++)
        {
            if (!StlacenaCislica(i + 1))
            {
                continue;
            }

            int kam = uzol.odpovede[i].kamVedie;

            if (kam < 0 || kam >= aktivneNpc.uzly.Length)
            {
                Zavri();
            }
            else
            {
                aktualnyUzol = kam;
                dialog.Zobraz(aktivneNpc.meno, aktivneNpc.uzly[aktualnyUzol]);
            }

            return;
        }

        // uzol bez odpovedi sa zavrie klavesou E
        if (uzol.odpovede.Length == 0 && interact.action.WasPressedThisFrame())
        {
            Zavri();
        }
    }

    private void Zavri()
    {
        dialog.Skry();
        pohyb.zablokovany = false;
        aktivneNpc = null;
    }

    private bool StlacenaCislica(int cislo)
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        switch (cislo)
        {
            case 1: return Keyboard.current.digit1Key.wasPressedThisFrame;
            case 2: return Keyboard.current.digit2Key.wasPressedThisFrame;
            case 3: return Keyboard.current.digit3Key.wasPressedThisFrame;
            case 4: return Keyboard.current.digit4Key.wasPressedThisFrame;
        }

        return false;
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