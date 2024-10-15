
using System;
using Eto.Drawing;
using Eto.Forms;

namespace DAW.UI;

class SettingsWindow : Dialog
{
    public SettingsWindow()
    {
        Title = "Settings";
        ClientSize = new Size(400, 350);
    }
}