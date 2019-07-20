namespace RCi.Tutorials.Csgo.Cheat.External
{
    /// <summary>
    /// https://github.com/frk1/hazedumper/blob/master/csgo.hpp
    /// </summary>
    public static class Offsets
    {
        public const int MAXSTUDIOBONES = 128; // total bones actually used
        public const float weapon_recoil_scale = 2.0f;

        public static int dwClientState = 0x58CCFC;
        public static int dwClientState_ViewAngles = 0x4D88;
        public static int dwEntityList = 0x4D05AD4;
        public static int dwLocalPlayer = 0xCF3A3C;
        public static int dwViewMatrix = 0x4CF7504;

        public static int m_aimPunchAngle = 0x302C;
        public static int m_bDormant = 0xED;
        public static int m_dwBoneMatrix = 0x26A8;
        public static int m_iFOV = 0x31E4;
        public static int m_iHealth = 0x100;
        public static int m_iTeamNum = 0xF4;
        public static int m_lifeState = 0x25F;
        public static int m_pStudioHdr = 0x294C;
        public static int m_vecOrigin = 0x138;
        public static int m_vecViewOffset = 0x108;
    }
}
