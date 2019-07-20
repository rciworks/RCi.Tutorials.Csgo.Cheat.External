using System;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using RCi.Tutorials.Csgo.Cheat.External.Data.Raw;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Data.Internal
{
    /// <summary>
    /// Entity data.
    /// </summary>
    public class Entity :
        EntityBase
    {
        #region // storage

        /// <summary>
        /// Index in entity list.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Dormant state.
        /// </summary>
        public bool Dormant { get; private set; } = true;

        /// <summary>
        /// Pointer to studio hrd.
        /// </summary>
        private IntPtr AddressStudioHdr { get; set; }

        /// <inheritdoc cref="studiohdr_t"/>
        public studiohdr_t StudioHdr { get; private set; }

        /// <inheritdoc cref="mstudiohitboxset_t"/>
        public mstudiohitboxset_t StudioHitBoxSet { get; private set; }

        /// <inheritdoc cref="mstudiobbox_t"/>
        public mstudiobbox_t[] StudioHitBoxes { get; }

        /// <inheritdoc cref="mstudiobone_t"/>
        public mstudiobone_t[] StudioBones { get; }

        /// <summary>
        /// Bone model to world matrices.
        /// </summary>
        public Matrix[] BonesMatrices { get; }

        /// <summary>
        /// Bone positions in world.
        /// </summary>
        public Vector3[] BonesPos { get; }

        /// <summary>
        /// Skeleton bones.
        /// </summary>
        public (int from, int to)[] Skeleton { get; }

        public int SkeletonCount { get; private set; }

        #endregion

        #region // ctor

        /// <summary />
        public Entity(int index)
        {
            Index = index;
            StudioHitBoxes = new mstudiobbox_t[Offsets.MAXSTUDIOBONES];
            StudioBones = new mstudiobone_t[Offsets.MAXSTUDIOBONES];
            BonesMatrices = new Matrix[Offsets.MAXSTUDIOBONES];
            BonesPos = new Vector3[Offsets.MAXSTUDIOBONES];
            Skeleton = new (int, int)[Offsets.MAXSTUDIOBONES];
        }

        #endregion

        #region // routines

        /// <inheritdoc />
        public override bool IsAlive()
        {
            return base.IsAlive() && !Dormant;
        }

        /// <inheritdoc />
        protected override IntPtr ReadAddressBase(GameProcess gameProcess)
        {
            return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwEntityList + Index * 0x10 /* size */);
        }

        /// <inheritdoc />
        public override bool Update(GameProcess gameProcess)
        {
            if (!base.Update(gameProcess))
            {
                return false;
            }

            Dormant = gameProcess.Process.Read<bool>(AddressBase + Offsets.m_bDormant);
            if (!IsAlive())
            {
                return true;
            }

            UpdateStudioHdr(gameProcess);
            UpdateStudioHitBoxes(gameProcess);
            UpdateStudioBones(gameProcess);
            UpdateBonesMatricesAndPos(gameProcess);
            UpdateSkeleton();

            return true;
        }

        /// <summary>
        /// Update <see cref="AddressStudioHdr"/> and <see cref="StudioHdr"/>.
        /// </summary>
        private void UpdateStudioHdr(GameProcess gameProcess)
        {
            var addressToAddressStudioHdr = gameProcess.Process.Read<IntPtr>(AddressBase + Offsets.m_pStudioHdr);
            AddressStudioHdr = gameProcess.Process.Read<IntPtr>(addressToAddressStudioHdr); // deref
            StudioHdr = gameProcess.Process.Read<studiohdr_t>(AddressStudioHdr);
        }

        /// <summary>
        /// Update <see cref="StudioHitBoxSet"/> and <see cref="StudioHitBoxes"/>.
        /// </summary>
        private void UpdateStudioHitBoxes(GameProcess gameProcess)
        {
            var addressHitBoxSet = AddressStudioHdr + StudioHdr.hitboxsetindex;
            StudioHitBoxSet = gameProcess.Process.Read<mstudiohitboxset_t>(addressHitBoxSet);

            // read
            for (var i = 0; i < StudioHitBoxSet.numhitboxes; i++)
            {
                StudioHitBoxes[i] = gameProcess.Process.Read<mstudiobbox_t>(addressHitBoxSet + StudioHitBoxSet.hitboxindex + i * Marshal.SizeOf<mstudiobbox_t>());
            }
        }

        /// <summary>
        /// Update <see cref="StudioBones"/>.
        /// </summary>
        private void UpdateStudioBones(GameProcess gameProcess)
        {
            for (var i = 0; i < StudioHdr.numbones; i++)
            {
                StudioBones[i] = gameProcess.Process.Read<mstudiobone_t>(AddressStudioHdr + StudioHdr.boneindex + i * Marshal.SizeOf<mstudiobone_t>());
            }
        }

        /// <summary>
        /// Update <see cref="StudioHdr"/>.
        /// </summary>
        private void UpdateBonesMatricesAndPos(GameProcess gameProcess)
        {
            var addressBoneMatrix = gameProcess.Process.Read<IntPtr>(AddressBase + Offsets.m_dwBoneMatrix);
            for (var boneId = 0; boneId < BonesPos.Length; boneId++)
            {
                var matrix = gameProcess.Process.Read<matrix3x4_t>(addressBoneMatrix + boneId * Marshal.SizeOf<matrix3x4_t>());
                BonesMatrices[boneId] = matrix.ToMatrix();
                BonesPos[boneId] = new Vector3(matrix.m30, matrix.m31, matrix.m32);
            }
        }

        /// <summary>
        /// Update <see cref="StudioHdr"/>.
        /// </summary>
        private void UpdateSkeleton()
        {
            // get bones to draw
            var skeletonBoneId = 0;
            for (var i = 0; i < StudioHitBoxSet.numhitboxes; i++)
            {
                var hitbox = StudioHitBoxes[i];
                var bone = StudioBones[hitbox.bone];
                if (bone.parent >= 0 && bone.parent < StudioHdr.numbones)
                {
                    // has valid parent
                    Skeleton[skeletonBoneId] = (hitbox.bone, bone.parent);
                    skeletonBoneId++;
                }
            }
            SkeletonCount = skeletonBoneId;
        }

        #endregion
    }
}
