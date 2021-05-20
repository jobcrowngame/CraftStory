using UnityEngine;

public class SettingMng : Single<SettingMng>
{
    private bool mouseCursorLocked = false;
    public bool MouseCursorLocked
    {
        get { return mouseCursorLocked; }
        set
        {
            mouseCursorLocked = value;

            // Mouse Cursor
            Cursor.lockState = value ?
                CursorLockMode.Locked :
                CursorLockMode.None;
        }
    }
}
