namespace Data.Const
{
    public static class CommEnum
    {
        public enum BULLET_TYPE
        {
            //AP弹
            AP = 1,
            //全金属被甲弹
            FMJ = 2,
            //被甲空尖弹
            JHP = 3,
            //高爆弹
            HE = 4,
            //霰弹
            SHOTGUN_AMMO = 5
        }

        public enum GUN_TYPE
        {
            //手枪
            PISTOL = 1,
            //微型冲锋枪
            SUBMACHINE_GUN = 2,
            //突击步枪
            ASSULT_RIFLE = 3,
            //机枪
            MACHINE_GUN = 4,
            //狙击步枪
            SINPER_RIFLE = 5
        }

        public enum CHARACTER_ATTRIBS
        {
            //基础属性
            //耐力
            STAMINA = 1,
            //反应力
            REACTION = 2,
            //力量
            STRENGTH = 3,

            //精神属性
            //谨慎
            CAUTIOUS = 1001,
            //沉着冷静
            CALM = 1002,
            //细心警觉
            CIRCUSPECTION = 1003,
            //忍耐力
            ENDURANCE = 1004,
            //狡诈
            CUNNING = 1005,
        }
    }
}