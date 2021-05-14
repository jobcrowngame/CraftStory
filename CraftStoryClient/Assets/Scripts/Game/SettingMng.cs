using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
