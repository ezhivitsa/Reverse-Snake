using System;

namespace Assets.ReverseSnake.Scripts.Models
{
    [Serializable]
    public class SettingsModel
    {
        public bool UseMusic;

        public bool UseSounds;

        public SettingsModel()
        {
            UseMusic = false;
            UseSounds = false;
        }
    }
}
