using System;
using RCi.Tutorials.Csgo.Cheat.External.Math;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Data
{
    /// <summary>
    /// Player class.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Update data from process.
        /// </summary>
        public void Update(GameProcess gameProcess)
        {
            var addressBase = gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayer);
            if (addressBase == IntPtr.Zero)
            {
                return;
            }

            var origin = gameProcess.Process.Read<Vector3>(addressBase + Offsets.m_vecOrigin);
            var viewOffset = gameProcess.Process.Read<Vector3>(addressBase + Offsets.m_vecViewOffset);
            var eyePosition = origin + viewOffset;

            Console.WriteLine($"{eyePosition.X:0.00} {eyePosition.Y:0.00} {eyePosition.Z:0.00}");
        }
    }
}
