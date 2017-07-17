using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RND_MT19937_32
{
    private static readonly uint MT_WORDSIZE_32 = 32;
    private static readonly uint MT_N_32 = 624;
    private static readonly uint MT_M_32 = 397;
    private static readonly uint MT_R_32 = 31;
    private static readonly uint MT_A_32 = 0x9908B0DF;
    private static readonly uint MT_U_32 = 11;
    private static readonly uint MT_D_32 = 0xFFFFFFFF;
    private static readonly uint MT_S_32 = 7;
    private static readonly uint MT_B_32 = 0x9D2C5680;
    private static readonly uint MT_T_32 = 15;
    private static readonly uint MT_C_32 = 0xEFC60000;
    private static readonly uint MT_L_32 = 18;
    private static readonly uint MT_F_32 = 1812433253;
    private static readonly uint lower_mask_32 = (uint)((1 << (int)MT_R_32) - 1);
    private static readonly uint upper_mask_32 = lower_mask_32 ^ 0xFFFFFFFF;

    private int mt_idx_32;
    private List<uint> MT_32;

    public RND_MT19937_32()
    {
        MT_32 = new List<uint>((int)MT_N_32);
        for (int i = 0; i < MT_N_32; ++i)
        {
            MT_32.Add(0);
        }
    }

    private void twist()
    {
        for (uint i = 0; i < MT_N_32; ++i)
        {
            uint x = (MT_32[(int)i] & upper_mask_32) + (MT_32[(int)((i + 1) % MT_N_32)] & lower_mask_32);
            uint xA = x >> 1;
            if (x % 2 != 0)
                xA ^= MT_A_32;
            MT_32[(int)i] = MT_32[(int)((i + MT_M_32) % MT_N_32)] ^ xA;
        }
        mt_idx_32 = 0;
    }

    public void srnd(UInt32 seed)
    {
        mt_idx_32 = (int)MT_N_32;
        MT_32[0] = seed;
        uint W_MINUS_2 = MT_WORDSIZE_32 - 2;
        for (int i = 1; i < MT_N_32; ++i)
        {
            MT_32[i] = (uint)(0xFFFFFFFF & (MT_F_32 * (MT_32[i - 1] ^ (MT_32[i - 1] >> (int)W_MINUS_2))) + i);
        }
    }

    public uint rnd()
    {
        if (mt_idx_32 > MT_N_32)
            return 0;

        if (mt_idx_32 == MT_N_32)
            twist();

        uint y = MT_32[mt_idx_32];
        y ^= y >> (int)MT_U_32; // and MT_D_32
        y ^= (y << (int)MT_S_32) & MT_B_32;
        y ^= (y << (int)MT_T_32) & MT_C_32;
        y ^= y >> (int)MT_L_32;

        mt_idx_32 += 1;
        return y;
    }

    public int irange(int lower, int upper)
    {
        if (upper <= lower)
            return lower;
        double norm = this.norm();
        int range_len = upper - lower + 1;
        int rs = lower + (int)Math.Floor(range_len * norm);
        return rs > upper ? upper : rs;
    }

    public double norm()
    {
        uint rnd_num = rnd();
        return rnd_num / (double)(UInt32.MaxValue);
    }
}
