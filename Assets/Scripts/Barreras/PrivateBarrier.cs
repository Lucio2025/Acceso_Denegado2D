using UnityEngine;

public class PrivateBarrier : Barrier
{
    [SerializeField] private Button[] linkedButtons;

    protected override void Awake()
    {
        base.Awake();

        foreach (var btn in linkedButtons)
            if (btn != null)
                btn.OnPressed += CheckAllButtons;
    }

    private void CheckAllButtons()
    {
        foreach (var btn in linkedButtons)
            if (btn == null || !btn.IsPressed()) return;

        Open();
    }
}