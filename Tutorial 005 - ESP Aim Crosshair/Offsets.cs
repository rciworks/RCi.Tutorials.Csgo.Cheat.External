namespace RCi.Tutorials.Csgo.Cheat.External
{
    /// <summary>
    /// https://github.com/frk1/hazedumper/blob/master/csgo.hpp
    /// </summary>
    public static class Offsets
    {
        public static float weapon_recoil_scale = 2.0f;

        public static int dwLocalPlayer = 0xCF3A3C;
        public static int dwClientState = 0x58CCFC;
        public static int dwClientState_ViewAngles = 0x4D88;
        public static int m_vecOrigin = 0x138;
        public static int m_vecViewOffset = 0x108;
        public static int m_aimPunchAngle = 0x302C;
        public static int m_iFOV = 0x31E4;
    }
}
