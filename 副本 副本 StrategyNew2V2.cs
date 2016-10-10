using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using URWPGSim2D.Common;
using URWPGSim2D.StrategyLoader;
using URWPGSim2D.StrategyHelper;


namespace URWPGSim2D.Strategy
{
    public class Strategy : MarshalByRefObject, IStrategy
    {

        #region 函数前叙

        #region  必要的加载，不能改动，不需要看懂
        /// <summary>
        /// 决策类当前对象对应的仿真使命参与队伍的决策数组引用 第一次调用GetDecision时分配空间
        /// </summary>
        private Decision[] decisions = null;

        #region reserved code never be changed or removed
        /// <summary>
        /// override the InitializeLifetimeService to return null instead of a valid ILease implementation
        /// to ensure this type of remote object never dies
        /// </summary>
        /// <returns>null</returns>
        public override object InitializeLifetimeService()
        {
            //return base.InitializeLifetimeService();
            return null; // makes the object live indefinitely
        }
        #endregion
        #endregion

        #region  获取队伍名称

        public string GetTeamName()
        {
            return "天津大学";
        }

        #endregion

        #region  定义使用变量
        public static float xzdangle, BX, BZ, FX, FZ, FBZXangle, FBZXangle1, FBZXangle2, Length1, Length2, Length3, BX0, BZ0, PiontX0, PiontZ0, TV,
                            TEMP, F0BLength, FBZXangle03, FBZXangle01, FBZXangle02, FBZXangle11, FBZXangle12, FBZXangle13, FBZXangle0, PiontX, PiontZ,
                            LengthBXZ1, LengthBXZ2, BX01, BZ01, LengthBX2, PiontX1, PiontZ1, PiontZ2, PiontX2, BZ11, BX11, BX2, BZ2, BX21, BZ21, FBZXangle21,
                            FBZXangle22, FBZXangle23, LengthBX0, LengthBZ0, LengthBX1, LengthBZ1, LengthBZ2, LengthBXZ0, LengthBX, LengthBZ, BX1, BZ1, FBLength,
                            FBZXangle3, jdt0, F0BAnglet0, jdt0101, F0BAnglet3, jdtq, jdt1, F0BAnglet2, F0BAnglet1, F0BLentht, jdt11, F1BAnglet0, jdt101, F1BAnglet3,
                            jdtq1, F1X, F1Z, F1BAnglet2, F1BLentht, jdt111, F1BAnglet1, remainC, remainM, FXP, FZP, FRad, temp_x, temp_z, OFX, OFZ, OFXP, OFZP, OFRad, dest0_x, dest0_z
                            , DFX0, DFZ0, DFX1, DFZ1, DFZ, DFX, DFZ2, DFX2, DFZ3, DFX3, FZS, FXS, FX0, FX1, FZ0, FZ1;

        public int temp_nu, temp_n = 0, temp_nu1, temp, temp1, which, which1, yaball, dingqiu, dingqiu1;

        #endregion

        #region 声明一些静态变量记忆存储数组
        ///add by joee33
        /// <summary>
        /// 声明一些静态变量记忆存储数组
        /// </summary>
        static int
               fish_cur = 0;
        static bool                        //是否到达去对方球门掏球的时刻
              flag_left = true;


        static int[] ball_cur = new int[2] { 5, 4 };          //数组的两个元素分别表示当前鱼所锁定的球的编号                                                      
        #endregion

        #endregion


        #region  子函数集

        #region 基本工具函数 by 机器鱼老前辈们

        #region 将弧度转换为角度
        /// <summary>将弧度转换为角度
        /// 
        /// </summary>add by 兽之哀
        /// <param name="red"></param>弧度
        /// <returns></returns>角度
        float RedToAngle(float red)
        {
            return ((float)((red / Math.PI) * 180));
        }
        #endregion

        #region 将角度转化为弧度
        /// <summary>将角度转化为弧度
        /// 
        /// </summary>add by 兽之哀
        /// <param name="angle"></param>角度
        /// <returns></returns>弧度
        float AngleToRed(float angle)
        {
            return ((float)((angle / 180) * Math.PI));
        }
        #endregion

        #region 返回剩余时间
        float Remain_time(Mission all)             //判断时间的函数   并把还剩的时间赋给变量remainM
        {
            remainC = all.CommonPara.RemainingCycles;
            remainM = (remainC * 100) / 60000;
            return remainM;
        }
        #endregion

        #region  返回Vector3类型的向量
        /// <summary>
        /// 返回目标方向
        /// 场地坐标系定义为：X向右，Z向下，Y置0，负X轴顺时针转回负X轴角度范围为(-PI,PI)的坐标系
        /// 返回Vector3类型的向量（Y置0，只有X和Z有意义）在场地坐标系中方向的角度值
        /// </summary>
        /// <param name="v">待计算角度值的xna.Vector3类型向量</param>
        /// <returns>向量v在场地坐标系中方向的角度值</returns>
        public static float GetAngleDegree(xna.Vector3 v)
        {
            float x = v.X;
            float y = v.Z;
            float angle = 0;

            if (Math.Abs(x) < float.Epsilon)
            {// x = 0 直角反正切不存在
                if (Math.Abs(y) < float.Epsilon) { angle = 0.0f; }
                else if (y > 0) { angle = 90.0f; }
                else if (y < 0) { angle = -90.0f; }
            }
            else if (x < 0)
            {// x < 0 (90,180]或(-180,-90)
                if (y >= 0) { angle = (float)(180 * Math.Atan(y / x) / Math.PI) + 180.0f; }
                else { angle = (float)(180 * Math.Atan(y / x) / Math.PI) - 180.0f; }
            }
            else
            {// x > 0 (-90,90)
                angle = (float)(180 * Math.Atan(y / x) / Math.PI);
            }

            return angle;
        }

        #endregion

        #region//该函数返回鱼要到达定点所需转的角度,cur_x-当前点X坐标,cur_z-当前点Z坐标,dest_x-目标点X坐标,dest_z-目标点Y坐标,返回需要偏转角度
        float Getxzdangle(float cur_x, float cur_z, float dest_x, float dest_z, float fish_rad)
        {
            float curangle;
            curangle = (float)(Math.Abs(Math.Atan((cur_x - dest_x) / (cur_z - dest_z))));
            if ((cur_x > dest_x) && (cur_z > dest_z))//以球为中心，当鱼在球右下角
            {
                if (fish_rad > (-(Math.PI / 2 + curangle)) && fish_rad < (Math.PI / 2 - curangle))
                {
                    xzdangle = (float)(Math.PI / 2 + curangle + fish_rad);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad > (Math.PI / 2 - curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 - fish_rad - curangle);
                }
                else if (fish_rad < (-(Math.PI / 2 + curangle)) && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(-Math.PI / 2 - curangle - fish_rad);
                }
            }
            else if ((cur_x > dest_x) && (cur_z < dest_z))//以球为中心，当鱼在球右上角
            {
                if (fish_rad < (Math.PI / 2 + curangle) && (-(Math.PI / 2 - curangle)) < fish_rad)
                {
                    xzdangle = (float)(Math.PI / 2 + curangle - fish_rad);
                }
                else if ((-(Math.PI / 2 - curangle)) > fish_rad && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 + fish_rad - curangle);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad > (Math.PI / 2 + curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(fish_rad - curangle - Math.PI / 2);
                    xzdangle = -xzdangle;
                }
            }
            else if ((cur_x < dest_x) && (cur_z > dest_z))//以球为中心，当鱼在球左下角
            {
                if (fish_rad > -(Math.PI / 2 - curangle) && fish_rad < (Math.PI / 2 + curangle))
                {
                    xzdangle = (float)(Math.PI / 2 + fish_rad - curangle);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad < -(Math.PI / 2 - curangle) && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(curangle - fish_rad - Math.PI / 2);
                }
                else if (fish_rad > (Math.PI / 2 + curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 + curangle - fish_rad);
                }
            }
            else if ((cur_x < dest_x) && (cur_z < dest_z))//以球为中心，当鱼在球左上角
            {
                if (fish_rad < (Math.PI / 2 - curangle) && fish_rad > -(Math.PI / 2 + curangle))
                {
                    xzdangle = (float)(Math.PI / 2 - curangle - fish_rad);

                }
                else if (fish_rad > (Math.PI / 2 - curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(fish_rad + curangle - Math.PI / 2);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad < -(Math.PI / 2 + curangle) && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 + fish_rad + curangle);
                    xzdangle = -xzdangle;
                }
            }

            return xzdangle;
        }
        #endregion

        #region 该函数返回任意两点间（当前点与目标点。方向为当前点指向目标点）连线与X轴正向夹角

        /// </summary>add by 10级团队
        /// <param name="cur_x">当前点X坐标</param>
        /// <param name="cur_z">当前点Z坐标</param>
        /// <param name="dest_x">目标点X坐标</param>
        /// <param name="dest_z">目标点Z坐标</param>
        /// <param name="?"></param>
        /// <returns>与X轴正向夹角</returns>
        float GetAnyangle(float cur_x, float cur_z, float dest_x, float dest_z)
        {
            float curangle, anyangel;
            curangle = (float)(Math.Abs(Math.Atan((cur_x - dest_x) / (cur_z - dest_z))));
            if (cur_x > dest_x)
            {
                if (cur_z < dest_z)
                    anyangel = (float)(curangle + Math.PI / 2);
                else
                    anyangel = (float)(-(curangle + Math.PI / 2));

            }
            else
            {

                if (cur_z < dest_z)
                    anyangel = (float)(Math.PI / 2 - curangle);
                else
                    anyangel = (float)(-(Math.PI / 2 - curangle));

            }
            return anyangel;


        }
        #endregion

        #region//转换函数
        float GetLengthToDestpoint(float cur_x, float cur_z, float dest_x, float dest_z)
        {
            return (float)Math.Sqrt(Math.Pow((cur_x - dest_x), 2) + Math.Pow((cur_z - dest_z), 2));
        }
        float GetLengthToLength(float cur_a, float cur_b)
        {
            return (float)Math.Sqrt(Math.Pow((cur_a), 2) + Math.Pow((cur_b), 2));
        }

        int TransfromAngletoTCode(float angvel)
        {
            if (0 == angvel)
            {
                return 7;
            }
            else if (angvel < 0)
            {
                if (-0.0269 <= angvel && 0 > angvel)
                {
                    if ((0 - angvel) >= (angvel + 0.0269))
                        return 6;
                    else
                        return 7;

                }
                else if (-0.04508 <= angvel && -0.0269 > angvel)
                {
                    if ((-0.0269 - angvel) >= (angvel + 0.04508))
                        return 5;
                    else
                        return 6;

                }
                else if (-0.071013 <= angvel && -0.04508 > angvel)
                {
                    if ((-0.04508 - angvel) >= (angvel + 0.071013))
                        return 4;
                    else
                        return 5;
                }
                else if (-0.09953 <= angvel && -0.071013 > angvel)
                {
                    if ((-0.071013 - angvel) >= (angvel + 0.09953))
                        return 3;
                    else
                        return 4;
                }
                else if (-0.1265 <= angvel && -0.09953 > angvel)
                {
                    if ((-0.09953 - angvel) >= (angvel + 0.1265))
                        return 2;
                    else
                        return 3;
                }
                else if (-0.1680 <= angvel && -0.1265 > angvel)
                {
                    if ((-0.1265 - angvel) >= (angvel + 0.1680))
                        return 1;
                    else
                        return 2;
                }
                else if (-0.20424 <= angvel && -0.1680 > angvel)
                {
                    if ((-0.1680 - angvel) >= (angvel + 0.20424))
                        return 0;
                    else
                        return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (0.0269 >= angvel && 0 < angvel)
                {
                    if (angvel - 0 > 0.0269 - angvel)
                        return 8;
                    else
                        return 7;

                }
                else if (0.04508 >= angvel && 0.0269 < angvel)
                {
                    if (angvel - 0.0269 > 0.04508 - angvel)
                        return 9;
                    else
                        return 8;
                }
                else if (0.071013 >= angvel && 0.04508 < angvel)
                {
                    if (angvel - 0.04508 > 0.071013 - angvel)
                        return 10;
                    else
                        return 9;
                }
                else if (0.09953 >= angvel && 0.071013 < angvel)
                {
                    if (angvel - 0.071013 > 0.09953 - angvel)
                        return 11;
                    else
                        return 10;
                }
                else if (0.1265 >= angvel && 0.09953 < angvel)
                {
                    if (angvel - 0.09953 > 0.1265 - angvel)
                        return 12;
                    else
                        return 11;
                }
                else if (0.1680 >= angvel && 0.1265 < angvel)
                {
                    if (angvel - 0.1265 > 0.1680 - angvel)
                        return 13;
                    else
                        return 12;
                }
                else if (0.1977 >= angvel && 0.1680 < angvel)
                {
                    if (angvel - 0.1680 > 0.1977 - angvel)
                        return 14;
                    else
                        return 13;
                }
                else
                {
                    return 14;
                }

            }
        }
        #endregion

        #region 返回最小角度
        /// </summary>返回最小角度
        /// <param name="x">角度1</param>
        /// <param name="y">角度2</param>
        /// <param name="z">角度3</param>
        /// <returns></returns>

        float Getminangle(float x, float y, float z)
        {
            if (x < y)
            {
                if (x < z)
                    return (x);
                else
                    return (z);
            }
            else
            {
                if (y < z)
                    return (y);
                else
                    return (z);
            }
        }
        #endregion

        #region 球门区
        int dangerarea(float x, float z)
        {
            if (x < -950 && z < 550 && z > -550)
                return 0;
            else if (x > 950 && z < 550 && z > -550)
                return 1;
            else return 2;

        }
        #endregion

        #region 初始化左右场
        void Updata(Mission ball, int teamId)
        {

            #region 判断己方半场在哪儿
            if (ball.TeamsRef[teamId].Para.MyHalfCourt == HalfCourt.LEFT)
            {
                flag_left = true;  //////左场
            }
            else
            {
                flag_left = false;  ///////右场
            }
            #endregion

        }
        #endregion

        #endregion

        #region 此函数定义区域  by  汤纬倩
        string Area(float x, float z)
        {
            if (x < 0 && x > -800)
            {
                if (z > -1000 && z < -600)
                {
                    return "L1_1";
                }
                else if (z > -600 && z < 0)
                {
                    return "L1_2";
                }
                else if (z > 0 && z < 600)
                {
                    return "L1_3";
                }
                else //if (z>620 && z<1000)
                {
                    return "L1_4";
                }
            }
            else if (x < -800 && x > -1170)
            {
                if (z > -1000 && z < -600)
                {
                    return "L2_1";
                }
                else if (z > -600 && z < 0)
                {
                    return "L2_2";
                }
                else if (z > 0 && z < 600)
                {
                    return "L2_3";
                }
                else //if (z>620 && z<1000)
                {
                    return "L2_4";
                }
            }
            else if (x < -1170 && x > -1500)
            {
                if (z > -1000 && z < -600)
                {
                    return "L3_1";
                }
                else if (z > -600 && z < 0)
                {
                    return "L3_2";
                }
                else if (z > 0 && z < 600)
                {
                    return "L3_3";
                }
                else //if (z>620 && z<1000)
                {
                    return "L3_4";
                }
            }
            else if (x < 800 && x > 0)
            {
                if (z > -1000 && z < -600)
                {
                    return "R1_1";
                }
                else if (z > -600 && z < 0)
                {
                    return "R1_2";
                }
                else if (z > 0 && z < 600)
                {
                    return "R1_3";
                }
                else //if (z>620 && z<1000)
                {
                    return "R1_4";
                }
            }
            else if (x < 1170 && x > 800)
            {
                if (z > -1000 && z < -600)
                {
                    return "R2_1";
                }
                else if (z > -600 && z < 0)
                {
                    return "R2_2";
                }
                else if (z > 0 && z < 600)
                {
                    return "R2_3";
                }
                else //if (z>620 && z<1000)
                {
                    return "R2_4";
                }
            }
            else if (x > 1170 && x < 1500)
            {
                if (z > -1000 && z < -600)
                {
                    return "R3_1";
                }
                else if (z > -600 && z < 0)
                {
                    return "R3_2";
                }
                else if (z > 0 && z < 600)
                {
                    return "R3_3";
                }
                else //if (z>620 && z<1000)
                {
                    return "R3_4";
                }

            }
            else
            { return "other"; }

        }
        #endregion

        #region 开场函数  by  荷初蕾
        void OpeningStageLeft(Mission all, ref Decision[] fish, int teamId)
        {
            float FX1, FZ1, FX2, FZ2, BX1, BZ1, BX2, BZ2, LengthBX0, LengthBZ0, LengthBXZ0, PiontX0, PiontZ0, PiontZ01, FBZXangle1, FBZXangle2, LengthBX01, LengthBZ01, FBZXangle01, FBZXangle02, PiontX01, BX0, BZ0, BX01, BZ01, LengthBXZ01;
            xna.Vector3 destPtMm01, destPtMm02;
            float targetDirection01, targetDirection02;
            destPtMm01.X = -30;
            destPtMm01.Y = 0;
            destPtMm01.Z = 235;
            FX1 = all.TeamsRef[teamId].Fishes[0].PositionMm.X;
            FZ1 = all.TeamsRef[teamId].Fishes[0].PositionMm.Z;


            targetDirection01 = GetAngleDegree(destPtMm01);
            StrategyHelper.Helpers.Dribble(ref decisions[0], all.TeamsRef[teamId].Fishes[0], destPtMm01, targetDirection01, 8, 10, 130, 8, 6, 5, 100, false);//9,6,120,8,12


            if (FX1 >= -150 && FX1 <= 170 && FZ1 >= -160 && FZ1 <= 350)
            {

                BX1 = all.EnvRef.Balls[5].PositionMm.X;
                BZ1 = all.EnvRef.Balls[5].PositionMm.Z;

                LengthBX0 = (float)-800 - BX1;    //X轴长度

                LengthBZ0 = (float)-600 - BZ1;         //(1500,0)Z轴长度



                LengthBXZ0 = (float)Math.Sqrt((Math.Pow(LengthBX0, 2) + Math.Pow(LengthBZ0, 2)));   //斜边长度


                PiontX0 = ((float)28 / LengthBXZ0) * (LengthBX0);     //相似三角X长
                PiontZ0 = ((float)28 / LengthBXZ0) * (LengthBZ0);     //相似三角Z长

                BX0 = BX1 + Math.Abs(PiontX0);        //顶球点X坐标
                BZ0 = BZ1 + PiontZ0;    //顶球点Z坐标

                FBZXangle1 = GetAnyangle(FX1, FZ1, BX0, BZ0);

                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                all.EnvRef.Balls[4].PositionMm.X = BX0;
                all.EnvRef.Balls[4].PositionMm.Z = BZ0;
                StrategyHelper.Helpers.Dribble(ref decisions[0], all.TeamsRef[teamId].Fishes[0], all.EnvRef.Balls[5].PositionMm, FBZXangle2, 5, 10, 150, 14, 5, 10, 100, true);
            }



            destPtMm02.X = -30;
            destPtMm02.Y = 0;
            destPtMm02.Z = -235;
            FX2 = all.TeamsRef[teamId].Fishes[1].PositionMm.X;
            FZ2 = all.TeamsRef[teamId].Fishes[1].PositionMm.Z;


            targetDirection02 = GetAngleDegree(destPtMm02);
            StrategyHelper.Helpers.Dribble(ref decisions[1], all.TeamsRef[teamId].Fishes[1], destPtMm02, targetDirection02, 8, 10, 130, 8, 6, 5, 100, false);//9,6,120,8,12


            if (FX2 >= -150 && FX2 <= 170 && FZ2 <= 160 && FZ2 >= -350)
            {

                BX2 = all.EnvRef.Balls[3].PositionMm.X;
                BZ2 = all.EnvRef.Balls[3].PositionMm.Z;

                LengthBX01 = (float)-800 - BX2;    //X轴长度

                LengthBZ01 = (float)600 - BZ2;         //(1500,0)Z轴长度



                LengthBXZ01 = (float)Math.Sqrt((Math.Pow(LengthBX01, 2) + Math.Pow(LengthBZ01, 2)));   //斜边长度


                PiontX01 = ((float)28 / LengthBXZ01) * (LengthBX01);     //相似三角X长
                PiontZ01 = ((float)28 / LengthBXZ01) * (LengthBZ01);     //相似三角Z长





                BX01 = BX2 + Math.Abs(PiontX01);        //顶球点X坐标
                BZ01 = BZ2 - PiontZ01;    //顶球点Z坐标




                FBZXangle01 = GetAnyangle(FX2, FZ2, BX01, BZ01);

                FBZXangle02 = (float)(FBZXangle01 * 180 / Math.PI);
                all.EnvRef.Balls[3].PositionMm.X = BX01;
                all.EnvRef.Balls[3].PositionMm.Z = BZ01;
                StrategyHelper.Helpers.Dribble(ref decisions[1], all.TeamsRef[teamId].Fishes[1], all.EnvRef.Balls[3].PositionMm, FBZXangle02, 5, 10, 150, 14, 5, 10, 100, true);
            }

        }
        void OpeningStageright(Mission all, ref Decision[] fish, int teamId)
        {
            float FX1, FZ1, FX2, FZ2, BX1, BZ1, BX0 = 0, BZ0 = 0, BX2, BZ2, LengthBX0, LengthBZ0, LengthBXZ0, PiontX0, PiontZ0, PiontZ01, FBZXangle1, FBZXangle2, LengthBX01, LengthBZ01, FBZXangle01, FBZXangle02, PiontX01, BX01, BZ01, LengthBXZ01;
            xna.Vector3 destPtMm01, destPtMm02;
            float targetDirection01, targetDirection02;
            destPtMm01.X = 30;
            destPtMm01.Y = 0;
            destPtMm01.Z = 235;
            FX1 = all.TeamsRef[teamId].Fishes[0].PositionMm.X;
            FZ1 = all.TeamsRef[teamId].Fishes[0].PositionMm.Z;
            targetDirection01 = GetAngleDegree(destPtMm01);
            StrategyHelper.Helpers.Dribble(ref decisions[0], all.TeamsRef[teamId].Fishes[0], destPtMm01, targetDirection01, 8, 10, 130, 8, 6, 5, 100, false);//9,6,120,8,12
            if (FX1 <= 150 && FX1 >= -170 && FZ1 >= -160 && FZ1 <= 350)
            {
                BX1 = all.EnvRef.Balls[5].PositionMm.X;
                BZ1 = all.EnvRef.Balls[5].PositionMm.Z;
                LengthBX0 = (float)700 - BX1;    //X轴长度
                LengthBZ0 = (float)-600 - BZ1;         //(1500,0)Z轴长度
                LengthBXZ0 = (float)Math.Sqrt((Math.Pow(LengthBX0, 2) + Math.Pow(LengthBZ0, 2)));   //斜边长度
                PiontX0 = ((float)28 / LengthBXZ0) * (LengthBX0);     //相似三角X长
                PiontZ0 = ((float)28 / LengthBXZ0) * (LengthBZ0);     //相似三角Z长
                BX0 = BX1 - Math.Abs(PiontX0) - 15;        //顶球点X坐标
                BZ0 = BZ1 + PiontZ0 - 10;    //顶球点Z坐标
                FBZXangle1 = GetAnyangle(FX1, FZ1, BX0, BZ0);
                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                all.EnvRef.Balls[4].PositionMm.X = BX0;
                all.EnvRef.Balls[4].PositionMm.Z = BZ0;
                StrategyHelper.Helpers.Dribble(ref decisions[0], all.TeamsRef[teamId].Fishes[0], all.EnvRef.Balls[5].PositionMm, FBZXangle2, 5, 10, 150, 14, 5, 10, 100, true);
            }

            destPtMm02.X = 30;
            destPtMm02.Y = 0;
            destPtMm02.Z = -235;
            FX2 = all.TeamsRef[teamId].Fishes[1].PositionMm.X;
            FZ2 = all.TeamsRef[teamId].Fishes[1].PositionMm.Z;
            targetDirection02 = GetAngleDegree(destPtMm02);
            StrategyHelper.Helpers.Dribble(ref decisions[1], all.TeamsRef[teamId].Fishes[1], destPtMm02, targetDirection02, 8, 10, 130, 8, 6, 5, 100, false);//9,6,120,8,12
            if (FX2 <= 150 && FX2 >= -170 && FZ2 <= 160 && FZ2 >= -350)
            {
                BX2 = all.EnvRef.Balls[3].PositionMm.X;
                BZ2 = all.EnvRef.Balls[3].PositionMm.Z;
                LengthBX01 = (float)700 - BX2;    //X轴长度
                LengthBZ01 = (float)600 - BZ2;         //(1500,0)Z轴长度
                LengthBXZ01 = (float)Math.Sqrt((Math.Pow(LengthBX01, 2) + Math.Pow(LengthBZ01, 2)));   //斜边长度
                PiontX01 = ((float)28 / LengthBXZ01) * (LengthBX01);     //相似三角X长
                PiontZ01 = ((float)28 / LengthBXZ01) * (LengthBZ01);     //相似三角Z长
                BX01 = BX2 - Math.Abs(PiontX01) - 10;        //顶球点X坐标
                BZ01 = BZ2 - PiontZ01 - 10;    //顶球点Z坐标
                FBZXangle01 = GetAnyangle(FX2, FZ2, BX0, BZ0);
                FBZXangle02 = (float)(FBZXangle01 * 180 / Math.PI);
                all.EnvRef.Balls[3].PositionMm.X = BX01;
                all.EnvRef.Balls[3].PositionMm.Z = BZ01;
                StrategyHelper.Helpers.Dribble(ref decisions[1], all.TeamsRef[teamId].Fishes[1], all.EnvRef.Balls[3].PositionMm, FBZXangle02, 5, 10, 150, 14, 5, 10, 100, true);
            }

        }

        #endregion

        #region 此函数让i鱼游向指定点    by 王闯
        /// <summary>
        /// 此函数让i鱼游向指定点
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="i"></param>
        /// <param name="dest_x"></param>
        /// <param name="dest_z"></param>
        /// <param name="teamId"></param>
        void SwimToDest(Mission mission, ref Decision[] fish, int i, float dest_x, float dest_z, int teamId)
        {


            decisions[i].TCode = TransfromAngletoTCode(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
            decisions[i].VCode = 5;
            int angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));

            if ((angle > -20) && (angle < 1))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 14;
            }
            else if ((angle > -40) && (angle < -20))
            {
                decisions[i].TCode = 2;
                decisions[i].VCode = 2;
            }
            else if ((angle > -60) && (angle < -40))
            {
                decisions[i].TCode = 1;
                decisions[i].VCode = 2;
            }
            else if ((angle > -180) && (angle < -60))
            {
                decisions[i].TCode = 0;
                decisions[i].VCode = 1;
            }
            else if ((angle > 1) && (angle < 20))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 14;
            }

            else if ((angle > 20) && (angle < 40))
            {
                decisions[i].TCode = 12;
                decisions[i].VCode = 3;
            }
            else if ((angle > 40) && (angle < 60))
            {
                decisions[i].TCode = 13;
                decisions[i].VCode = 2;
            }
            else if ((angle > 60) && (angle < 180))
            {
                decisions[i].TCode = 14;
                decisions[i].VCode = 1;
            }

        }
        #endregion

        #region 此函数让i号鱼在球门以水平防守  by 荷初蕾，谭琼慧

        void defendLeft180(Mission mission, ref Decision[] f, int i, int way, int teamId)
        {
            float dest_x = -900, dest_z = -700;
            xna.Vector3 destPtMm;
            destPtMm.X = -1520;
            destPtMm.Y = 0;
            destPtMm.Z = -600;
            FX = mission.TeamsRef[teamId].Fishes[i].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[i].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[i].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[i].PolygonVertices[0].Z;
            FRad = mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad;
            int targetDirection; //= (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, destPtMm.X, destPtMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));

            if (way == 1) //在左上防守
            {
                destPtMm.X = -1520;
                destPtMm.Y = 0;
                destPtMm.Z = -600;
                targetDirection = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, destPtMm.X, destPtMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (FXP > 950 && FZP > -550 && FZP < 550)
                {
                    dest_x = 1300;
                    dest_z = -700;
                    SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                }
                else
                {
                    dest_x = -900; dest_z = -700;
                    if ((Area(FX, FZ) != "L2_1" && Area(FXP, FZP) != "L3_1" && Area(FXP, FZP) != "L3_2" && Area(FXP, FZP) != "L2_2"))
                        SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                    else
                    {
                        SwimToDest(mission, ref decisions, i, destPtMm.X, destPtMm.Z, teamId);
                        if (targetDirection < -120 || targetDirection > 120)
                        {
                            decisions[i].VCode = 14; // 高速
                            decisions[i].TCode = 7; // 直游
                        }
                        else
                        {
                            StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, targetDirection,
                                                    10, 15, 130, 14, 14, 20, 100, false);
                        }
                    }
                }

            }
            else //在左下防守
            {
                destPtMm.X = -1520;
                destPtMm.Y = 0;
                destPtMm.Z = 600;
                targetDirection = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, destPtMm.X, destPtMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (FXP > 950 && FZP > -550 && FZP < 550)
                {
                    dest_x = 1300;
                    dest_z = 700;
                    SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                }
                else
                {
                    dest_x = -820; dest_z = 700;
                    if ((Area(FX, FZ) != "L2_4" && Area(FX, FZ) != "L3_4" && Area(FX, FZ) != "L3_3"))
                        SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                    else
                    {
                        SwimToDest(mission, ref decisions, i, destPtMm.X, destPtMm.Z, teamId);
                        if (targetDirection < -120 || targetDirection > 120)
                        {
                            decisions[i].VCode = 14; // 高速
                            decisions[i].TCode = 7; // 直游
                        }
                        else
                        {
                            StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, targetDirection,
                                                    10, 15, 130, 14, 14, 20, 100, false);
                        }
                    }
                }


            }
        }
        void defendRight180(Mission mission, ref Decision[] f, int i, int way, int teamId)
        {
            float dest_x = 900, dest_z = -700;
            xna.Vector3 destPtMm;
            destPtMm.X = 1520;
            destPtMm.Y = 0;
            destPtMm.Z = -600;

            FX = mission.TeamsRef[teamId].Fishes[i].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[i].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[i].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[i].PolygonVertices[0].Z;
            FRad = mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad;
            int targetDirection;//= (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, destPtMm.X, destPtMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));

            if (way == 1) //在右上防守
            {

                destPtMm.X = 1520;
                destPtMm.Y = 0;
                destPtMm.Z = -600;
                targetDirection = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, destPtMm.X, destPtMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (FXP < -950 && FZP > -550 && FZP < 550)
                {
                    dest_x = -1300;
                    dest_z = -700;
                    SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                }
                else
                {
                    dest_x = 900; dest_z = -700;
                    if ((Area(FX, FZ) != "R2_1" && Area(FXP, FZP) != "R3_1" && Area(FXP, FZP) != "R3_2" && Area(FX, FZ) != "R2_2"))
                        SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                    else
                    {
                        if (GetLengthToDestpoint(FXP, FZP, destPtMm.X, destPtMm.Z) > 100)
                            SwimToDest(mission, ref decisions, i, destPtMm.X, destPtMm.Z, teamId);
                        else
                        {
                            if (targetDirection < -150 || targetDirection > 150)
                            {
                                decisions[i].VCode = 14; // 高速
                                decisions[i].TCode = 7; // 直游
                            }
                            else
                            {

                                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, targetDirection,
                                                        10, 10, 130, 14, 14, 20, 100, false);
                            }
                        }
                    }
                }

            }
            else //在右下防守
            {
                destPtMm.X = 1520;
                destPtMm.Y = 0;
                destPtMm.Z = 600;
                targetDirection = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, destPtMm.X, destPtMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (FXP < -950 && FZP > -550 && FZP < 550)
                {
                    dest_x = -1300;
                    dest_z = 700;
                    SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                }
                else
                {
                    dest_x = 820; dest_z = 700;
                    if ((Area(FX, FZ) != "R2_4" && Area(FX, FZ) != "R3_4" && Area(FX, FZ) != "R3_3"))
                        SwimToDest(mission, ref decisions, i, dest_x, dest_z, teamId);
                    else
                    {
                        SwimToDest(mission, ref decisions, i, destPtMm.X, destPtMm.Z, teamId);
                        if (targetDirection < -150 || targetDirection > 150)
                        {
                            decisions[i].VCode = 14; // 高速
                            decisions[i].TCode = 7; // 直游
                        }
                        else
                        {
                            StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, targetDirection,
                                                    10, 10, 130, 14, 14, 20, 100, false);
                        }
                    }
                }
            }
        }

        #endregion

        #region 顶球时的锁球和解锁球函数  by 李飞海

        #region 左场

        int JudgeSouLeft_ball(Mission mission, ref Decision[] fish, int teamId)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            if ((FRad > -90 && FRad < 90) || dingqiu == 10)
            {
                for (int i = 0; i < 8; i++)
                {

                    if (mission.EnvRef.Balls[i].PositionMm.X > -1030 || (mission.EnvRef.Balls[i].PositionMm.X > -1200 && (mission.EnvRef.Balls[i].PositionMm.Z > 470 || mission.EnvRef.Balls[i].PositionMm.Z < -470)))
                    {
                        dingqiu = i; i = 8;
                    }

                }
                for (int i = dingqiu; i < 8; i++)
                {


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X > -1030 || (mission.EnvRef.Balls[i + 1].PositionMm.X > -1200 && (mission.EnvRef.Balls[i + 1].PositionMm.Z > 470 || mission.EnvRef.Balls[i + 1].PositionMm.Z < -470)))
                    {
                        if (mission.EnvRef.Balls[i + 1].PositionMm.X < mission.EnvRef.Balls[dingqiu].PositionMm.X)
                            dingqiu = i + 1;


                    }
                }

            }

            if (mission.EnvRef.Balls[dingqiu].PositionMm.X < -1200)

                dingqiu = 10;

            return dingqiu;
        }
        int JudgeSouLeft_ball1(Mission mission, ref Decision[] fish, int teamId)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            if ((FRad > -90 && FRad < 90) || dingqiu1 == 10)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i != JudgeSouLeft_ball(mission, ref decisions, teamId))
                        if (mission.EnvRef.Balls[i].PositionMm.X > -1030 || (mission.EnvRef.Balls[i].PositionMm.X > -1200 && (mission.EnvRef.Balls[i].PositionMm.Z > 470 || mission.EnvRef.Balls[i].PositionMm.Z < -470)))
                        {
                            dingqiu1 = i; i = 8;
                        }

                }
                for (int i = dingqiu1; i < 8; i++)
                {
                    if (i + 1 != JudgeSouLeft_ball(mission, ref decisions, teamId))
                        if (mission.EnvRef.Balls[i + 1].PositionMm.X > -1030 || (mission.EnvRef.Balls[i + 1].PositionMm.X > -1200 && (mission.EnvRef.Balls[i + 1].PositionMm.Z > 470 || mission.EnvRef.Balls[i + 1].PositionMm.Z < -470)))
                        {
                            if (mission.EnvRef.Balls[i + 1].PositionMm.X < mission.EnvRef.Balls[dingqiu1].PositionMm.X)
                                dingqiu1 = i + 1;


                        }
                }

            }

            if (mission.EnvRef.Balls[dingqiu1].PositionMm.X < -1200)
                dingqiu1 = 10;

            return dingqiu1;
        }
        int whichball(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {
            if (JudgeSouLeft_ball(mission, ref decisions, teamId) == 10)
                JudgeSouLeft_ball(mission, ref decisions, teamId);
            else
                dingqiu = JudgeSouLeft_ball(mission, ref decisions, teamId);
            if (JudgeSouLeft_ball1(mission, ref decisions, teamId) == 10)
                JudgeSouLeft_ball1(mission, ref decisions, teamId);
            else
                dingqiu1 = JudgeSouLeft_ball1(mission, ref decisions, teamId);
            FX0 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X;
            FZ0 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z;
            FX1 = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X;
            FZ1 = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z;
            if (fish_cur == 0)
            {
                if (GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX0, FZ0) > GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX1, FZ1))
                    return dingqiu1;
                else
                    return dingqiu;
            }

            else
            {
                if (GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX0, FZ0) > GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX1, FZ1))
                    return dingqiu;
                else
                    return dingqiu1;
            }

        }
        #endregion

        #region 右场
        int JudgeSouRight_ball(Mission mission, ref Decision[] fish, int teamId)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            if (!(FRad > -90 && FRad < 90) || dingqiu == 10)
            {
                for (int i = 0; i < 8; i++)
                {

                    if (mission.EnvRef.Balls[i].PositionMm.X < 1030 || (mission.EnvRef.Balls[i].PositionMm.X < 1200 && (mission.EnvRef.Balls[i].PositionMm.Z > 470 || mission.EnvRef.Balls[i].PositionMm.Z < -470)))
                    {
                        dingqiu = i; i = 8;
                    }

                }
                for (int i = dingqiu; i < 8; i++)
                {


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X < 1030 || (mission.EnvRef.Balls[i + 1].PositionMm.X < 1200 && (mission.EnvRef.Balls[i + 1].PositionMm.Z > 470 || mission.EnvRef.Balls[i + 1].PositionMm.Z < -470)))
                    {
                        if (mission.EnvRef.Balls[i + 1].PositionMm.X > mission.EnvRef.Balls[dingqiu].PositionMm.X)
                            dingqiu = i + 1;


                    }
                }

            }

            if (mission.EnvRef.Balls[dingqiu].PositionMm.X > 1200)

                dingqiu = 10;

            return dingqiu;
        }
        int JudgeSouRight_ball1(Mission mission, ref Decision[] fish, int teamId)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            if (!(FRad > -90 && FRad < 90) || dingqiu1 == 10)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i != JudgeSouRight_ball(mission, ref decisions, teamId))
                        if (mission.EnvRef.Balls[i].PositionMm.X < 1030 || (mission.EnvRef.Balls[i].PositionMm.X < 1200 && (mission.EnvRef.Balls[i].PositionMm.Z > 470 || mission.EnvRef.Balls[i].PositionMm.Z < -470)))
                        {
                            dingqiu1 = i; i = 8;
                        }

                }
                for (int i = dingqiu1; i < 8; i++)
                {
                    if (i + 1 != JudgeSouRight_ball(mission, ref decisions, teamId))
                        if (mission.EnvRef.Balls[i + 1].PositionMm.X < 1030 || (mission.EnvRef.Balls[i + 1].PositionMm.X < 1200 && (mission.EnvRef.Balls[i + 1].PositionMm.Z > 470 || mission.EnvRef.Balls[i + 1].PositionMm.Z < -470)))
                        {
                            if (mission.EnvRef.Balls[i + 1].PositionMm.X > mission.EnvRef.Balls[dingqiu1].PositionMm.X)
                                dingqiu1 = i + 1;


                        }
                }

            }

            if (mission.EnvRef.Balls[dingqiu1].PositionMm.X > 1200)
                dingqiu1 = 10;

            return dingqiu1;
        }
        int whichball1(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {
            if (JudgeSouRight_ball(mission, ref decisions, teamId) == 10)
                JudgeSouRight_ball(mission, ref decisions, teamId);
            else
                dingqiu = JudgeSouRight_ball(mission, ref decisions, teamId);
            if (JudgeSouRight_ball1(mission, ref decisions, teamId) == 10)
                JudgeSouRight_ball1(mission, ref decisions, teamId);
            else
                dingqiu1 = JudgeSouRight_ball1(mission, ref decisions, teamId);
            FX0 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X;
            FZ0 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z;
            FX1 = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X;
            FZ1 = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z;
            if (fish_cur == 0)
            {
                if (GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX0, FZ0) > GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX1, FZ1))
                    return dingqiu1;
                else
                    return dingqiu;
            }

            else
            {
                if (GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX0, FZ0) > GetLengthToDestpoint(mission.EnvRef.Balls[dingqiu].PositionMm.X, mission.EnvRef.Balls[dingqiu].PositionMm.Z, FX1, FZ1))
                    return dingqiu;
                else
                    return dingqiu1;
            }

        }
        #endregion

        #endregion

        #region 顶球函数  by  杨林，李飞海，谭琼慧 ，李鑫~
        void SoloArea_left(Mission mission, ref Decision[] fish, int teamId)
        {


            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;//鱼的横轴坐标
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;//鱼的纵轴坐标

            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;

            if (BZ < 0)
            {
                if (BZ > -650 && BX > -900)
                {
                    if (BX < -380)
                    {

                        LengthBX0 = (float)-950 - BX;    //X轴长
                        LengthBZ0 = (float)550 - BZ;         //(1500,0)Z轴长度
                    }
                    else
                    {
                        LengthBX0 = (float)-950 - BX;    //X轴长
                        LengthBZ0 = (float)400 - BZ;         //(1500,0)Z轴长度

                    }
                    LengthBXZ0 = GetLengthToLength(LengthBX0, LengthBZ0);   //斜边长
                    PiontX0 = ((float)28 / LengthBXZ0) * (LengthBX0);     //相似三角X长
                    PiontZ0 = ((float)28 / LengthBXZ0) * (LengthBZ0);
                    BX0 = BX + Math.Abs(PiontX0);        //顶球点X坐标
                    BZ0 = BZ + PiontZ0;
                    FBZXangle1 = GetAnyangle(FX, FZ, BX0, BZ0);
                }
                else
                {
                    BX0 = BX + 10;
                    if (BZ > -800)
                        BZ0 = BZ + 30;
                    else
                        BZ0 = BZ;
                    FBZXangle1 = GetAnyangle(FX, 2 * BZ - FZ, BX0, BZ0);
                }
            }

            else if (BZ > 0)
            {
                if (BZ < 650 && BX > -900)
                {
                    if (BX < -380)
                    {

                        LengthBX0 = (float)-950 - BX;    //X轴长
                        LengthBZ0 = (float)-550 - BZ;         //(1500,0)Z轴长度
                    }
                    else
                    {
                        LengthBX0 = (float)-950 - BX;    //X轴长
                        LengthBZ0 = (float)-400 - BZ;         //(1500,0)Z轴长度

                    }
                    LengthBXZ0 = GetLengthToLength(LengthBX0, LengthBZ0);   //斜边长
                    PiontX0 = ((float)28 / LengthBXZ0) * (LengthBX0);     //相似三角X长
                    PiontZ0 = ((float)28 / LengthBXZ0) * (LengthBZ0);
                    BX0 = BX + Math.Abs(PiontX0);        //顶球点X坐标
                    BZ0 = BZ + PiontZ0;
                    FBZXangle1 = GetAnyangle(FX, FZ, BX0, BZ0);
                }
                else
                {
                    BX0 = BX + 10;
                    if (BZ < 800)
                        BZ0 = BZ + 30;
                    else
                        BZ0 = BZ;
                    FBZXangle1 = GetAnyangle(FX, 2 * BZ - FZ, BX0, BZ0);
                }
            }

            BX01 = BX - Math.Abs(PiontX0);
            BZ01 = BZ + PiontZ0;

            BX1 = BX;
            BZ1 = BZ - PiontZ0 - 100;                  //绕球顶球点Z坐标



            BX2 = BX;
            BZ2 = BZ - PiontZ0 + 100;                  //绕球顶球点Z坐标


            FBLength = GetLengthToDestpoint(BX, BZ, FX, FZ);    //球鱼距离

            FBZXangle = Getxzdangle(FX, FZ, BX0, BZ0, mission.TeamsRef[0].Fishes[fish_cur].BodyDirectionRad);     //正顶球点
            FBZXangle01 = Getxzdangle(FX, FZ, BX01, BZ01, mission.TeamsRef[0].Fishes[fish_cur].BodyDirectionRad);   //相对顶球点
            FBZXangle02 = Math.Abs(mission.TeamsRef[0].Fishes[fish_cur].BodyDirectionRad - GetAnyangle(BX, BZ, dest0_x, dest0_z));  //角度差
            FBZXangle03 = Getxzdangle(FX, FZ, BX, BZ, mission.TeamsRef[0].Fishes[fish_cur].BodyDirectionRad);      //找球心

            FBZXangle1 = Getxzdangle(FX, FZ, BX1, BZ1, mission.TeamsRef[0].Fishes[fish_cur].BodyDirectionRad);     //正顶球点

            FBZXangle2 = Getxzdangle(FX, FZ, BX2, BZ2, mission.TeamsRef[0].Fishes[fish_cur].BodyDirectionRad);     //正顶球点
            if (FX < -1000 && FZ < 0)
            {
                Goout(mission, ref decisions, teamId, 0, 0);

            }
            else if (FX < -1000 && FZ >= 0)
            {
                Goout(mission, ref decisions, teamId, 0, 1);

            }
            else if (BX - 100 > FX && BZ <= FZ)
            {
                decisions[fish_cur].TCode = TransfromAngletoTCode(FBZXangle1);
                TV = TransfromAngletoTCode(FBZXangle1);
                TEMP = Math.Abs(7 - TV);
                switch ((int)TEMP)
                {
                    case 0: decisions[fish_cur].VCode = 14;
                        break;
                    case 1: decisions[fish_cur].VCode = 13;
                        break;
                    case 2: decisions[fish_cur].VCode = 12;
                        break;
                    case 3: decisions[fish_cur].VCode = 11;
                        break;
                    case 4: decisions[fish_cur].VCode = 10;
                        break;
                    case 5: decisions[fish_cur].VCode = 9;
                        break;
                    case 6: decisions[fish_cur].VCode = 8;
                        break;
                    case 7: decisions[fish_cur].VCode = 2;
                        break;
                    default:
                        decisions[fish_cur].VCode = 1;
                        break;
                }
            }
            else if (BX - 100 > FX && BZ > FZ)
            {
                decisions[fish_cur].TCode = TransfromAngletoTCode(FBZXangle2);
                TV = TransfromAngletoTCode(FBZXangle2);
                TEMP = Math.Abs(7 - TV);
                switch ((int)TEMP)
                {
                    case 0: decisions[fish_cur].VCode = 14;
                        break;
                    case 1: decisions[fish_cur].VCode = 13;
                        break;
                    case 2: decisions[fish_cur].VCode = 12;
                        break;
                    case 3: decisions[fish_cur].VCode = 11;
                        break;
                    case 4: decisions[fish_cur].VCode = 10;
                        break;
                    case 5: decisions[fish_cur].VCode = 9;
                        break;
                    case 6: decisions[fish_cur].VCode = 8;
                        break;
                    case 7: decisions[fish_cur].VCode = 2;
                        break;
                    default:
                        decisions[fish_cur].VCode = 1;
                        break;
                }
            }
            else
            {


                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);     //正顶球点

                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX0;
                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ0;
                if (FBLength > 200)
                {
                    StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[0].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 14, 5, 10, 100, true);

                }
                else
                {
                    StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[0].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 8, 150, 14, 5, 8, 100, true);
                }

            }

        }
        void SoloArea_right(Mission mission, ref Decision[] fish, int teamId)
        {
            int dest_x = 950, dest_z = -550;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            if (BZ < 0)
            {
                if (BZ > -650 && BX < 900)
                {
                    if (BX > 380)
                    {

                        LengthBX0 = (float)950 + BX;    //X轴长
                        LengthBZ0 = (float)550 - BZ;         //(1500,0)Z轴长度
                    }
                    else
                    {
                        LengthBX0 = (float)950 + BX;    //X轴长
                        LengthBZ0 = (float)400 - BZ;         //(1500,0)Z轴长度

                    }
                    LengthBXZ0 = GetLengthToLength(LengthBX0, LengthBZ0);   //斜边长
                    PiontX0 = ((float)28 / LengthBXZ0) * (LengthBX0);     //相似三角X长
                    PiontZ0 = ((float)28 / LengthBXZ0) * (LengthBZ0);
                    BX0 = BX - Math.Abs(PiontX0);        //顶球点X坐标
                    BZ0 = BZ + PiontZ0;
                    FBZXangle1 = GetAnyangle(FX, FZ, BX0, BZ0);
                }
                else
                {
                    BX0 = BX - 10;
                    if (BZ > -800)
                        BZ0 = BZ + 30;
                    else
                        BZ0 = BZ;
                    FBZXangle1 = GetAnyangle(FX, 2 * BZ - FZ, BX0, BZ0);
                }
            }

            else if (BZ > 0)
            {
                if (BZ < 650 && BX < 900)
                {
                    if (BX > 380)
                    {

                        LengthBX0 = (float)950 + BX;    //X轴长
                        LengthBZ0 = (float)-550 - BZ;         //(1500,0)Z轴长度
                    }
                    else
                    {
                        LengthBX0 = (float)950 + BX;    //X轴长
                        LengthBZ0 = (float)-400 - BZ;         //(1500,0)Z轴长度

                    }
                    LengthBXZ0 = GetLengthToLength(LengthBX0, LengthBZ0);   //斜边长
                    PiontX0 = ((float)28 / LengthBXZ0) * (LengthBX0);     //相似三角X长
                    PiontZ0 = ((float)28 / LengthBXZ0) * (LengthBZ0);
                    BX0 = BX - Math.Abs(PiontX0);        //顶球点X坐标
                    BZ0 = BZ + PiontZ0;
                    FBZXangle1 = GetAnyangle(FX, FZ, BX0, BZ0);
                }
                else
                {
                    BX0 = BX - 10;
                    if (BZ < 800)
                        BZ0 = BZ + 30;
                    else
                        BZ0 = BZ;
                    FBZXangle1 = GetAnyangle(FX, 2 * BZ - FZ, BX0, BZ0);
                }
            }
            BX01 = BX - Math.Abs(PiontX0);
            BZ01 = BZ + PiontZ0;

            BX1 = BX;
            BZ1 = BZ - PiontZ0 - 100;                  //绕球顶球点Z坐标



            BX2 = BX;
            BZ2 = BZ - PiontZ0 + 100;                  //绕球顶球点Z坐标


            FBLength = GetLengthToDestpoint(BX, BZ, FX, FZ);    //球鱼距离

            FBZXangle = Getxzdangle(FX, FZ, BX0, BZ0, mission.TeamsRef[1].Fishes[fish_cur].BodyDirectionRad);     //正顶球点
            FBZXangle01 = Getxzdangle(FX, FZ, BX01, BZ01, mission.TeamsRef[1].Fishes[fish_cur].BodyDirectionRad);   //相对顶球点
            FBZXangle02 = Math.Abs(mission.TeamsRef[1].Fishes[fish_cur].BodyDirectionRad - GetAnyangle(BX, BZ, dest_x, dest_z));  //角度差
            FBZXangle03 = Getxzdangle(FX, FZ, BX, BZ, mission.TeamsRef[1].Fishes[fish_cur].BodyDirectionRad);      //找球心

            FBZXangle1 = Getxzdangle(FX, FZ, BX1, BZ1, mission.TeamsRef[1].Fishes[fish_cur].BodyDirectionRad);     //正顶球点

            FBZXangle2 = Getxzdangle(FX, FZ, BX2, BZ2, mission.TeamsRef[1].Fishes[fish_cur].BodyDirectionRad);     //正顶球点
            if (FX > 1000 && FZ < 0)
            {
                Goout(mission, ref decisions, teamId, 0, 0);

            }
            else if (FX > 1000 && FZ >= 0)
            {
                Goout(mission, ref decisions, teamId, 0, 1);

            }
            else
            {
                if (BX + 100 < FX && BZ <= FZ)
                {
                    decisions[fish_cur].TCode = TransfromAngletoTCode(FBZXangle1);
                    TV = TransfromAngletoTCode(FBZXangle1);
                    TEMP = Math.Abs(7 - TV);
                    switch ((int)TEMP)
                    {
                        case 0: decisions[fish_cur].VCode = 14;
                            break;
                        case 1: decisions[fish_cur].VCode = 13;
                            break;
                        case 2: decisions[fish_cur].VCode = 12;
                            break;
                        case 3: decisions[fish_cur].VCode = 11;
                            break;
                        case 4: decisions[fish_cur].VCode = 10;
                            break;
                        case 5: decisions[fish_cur].VCode = 9;
                            break;
                        case 6: decisions[fish_cur].VCode = 8;
                            break;
                        case 7: decisions[fish_cur].VCode = 2;
                            break;
                        default:
                            decisions[fish_cur].VCode = 1;
                            break;
                    }
                }
                else if (BX + 100 < FX && BZ > FZ)
                {
                    decisions[fish_cur].TCode = TransfromAngletoTCode(FBZXangle2);
                    TV = TransfromAngletoTCode(FBZXangle2);
                    TEMP = Math.Abs(7 - TV);
                    switch ((int)TEMP)
                    {
                        case 0: decisions[fish_cur].VCode = 14;
                            break;
                        case 1: decisions[fish_cur].VCode = 13;
                            break;
                        case 2: decisions[fish_cur].VCode = 12;
                            break;
                        case 3: decisions[fish_cur].VCode = 11;
                            break;
                        case 4: decisions[fish_cur].VCode = 10;
                            break;
                        case 5: decisions[fish_cur].VCode = 9;
                            break;
                        case 6: decisions[fish_cur].VCode = 8;
                            break;
                        case 7: decisions[fish_cur].VCode = 2;
                            break;
                        default:
                            decisions[fish_cur].VCode = 1;
                            break;
                    }
                }
                else
                {
                    FBZXangle1 = GetAnyangle(FX, FZ, BX0, BZ0);

                    FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);     //正顶球点

                    mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX0;
                    mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ0 + 8;
                    if (FBLength > 200)
                    {
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[1].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 14, 5, 10, 100, true);

                    }
                    else
                    {
                        if (BX > 930)
                        {
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX0;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ0 - 8;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[1].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 8, 150, 7, 8, 8, 100, true);
                        }
                        else
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[1].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 8, 150, 7, 8, 8, 100, true);
                    }
                }
            }
        }
        #endregion

        #region 进球函数  by 李飞海
        void EdgeHandle(Mission mission, ref Decision[] fish, int teamId, int na)
        {
            int na1, li = 1;
            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;

            #region//  判断左右场
            if (na == 1)
            {

                if (BX < -1094 && BZ < 470 && BZ > -470)
                {
                    if (BZ >= 0)
                    {
                        if (FX > 1000 && FZ < 470 && FZ > -470)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 1250, 580, teamId);
                        }

                        else if (FX > -845)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -900, 580, teamId);
                        }
                        else if (FX <= -845 && FX >= -1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -1156, 480, teamId);
                        }
                        else if (FX <= -845 && FX >= -1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -1156, -480, teamId);
                        }
                        else if (FX < -1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX - 58, 480, teamId);
                        }
                        else if (FX < -1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX - 58, -480, teamId);
                        }
                    }
                    else
                    {
                        if (FX > 1000 && FZ < 470 && FZ > -470)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 1250, -580, teamId);
                        }
                        else if (FX > -845)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -900, -580, teamId);
                        }
                        else if (FX < -845 && FX >= -1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -1156, -480, teamId);
                        }
                        else if (FX < -845 && FX >= -1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -1156, 480, teamId);
                        }
                        else if (FX < -1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX - 58, -480, teamId);
                        }
                        else if (FX < -1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX - 58, 480, teamId);
                        }

                    }

                }
            }
            if (na == 2)
            {

                if (BX > 1094 && BZ < 470 && BZ > -470)
                {

                    if (BZ > 0)
                    {
                        if (FX < -1000 && FZ < 470 && FZ > -470)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -1250, 580, teamId);
                        }
                        else if (FX < 845)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 900, 580, teamId);
                        }
                        else if (FX > 845 && FX <= 1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 1156, 480, teamId);
                        }
                        else if (FX > 845 && FX <= 1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 1156, -480, teamId);
                        }
                        else if (FX > 1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX + 58, 480, teamId);
                        }
                        else if (FX > 1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX + 58, -480, teamId);
                        }

                    }
                    else
                    {
                        if (FX < -1000 && FZ < 470 && FZ > -470)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, -1200, -580, teamId);
                        }
                        else if (FX < 845)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 900, -580, teamId);
                        }
                        else if (FX > 845 && FX <= 1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 1156, -480, teamId);
                        }
                        else if (FX > 845 && FX <= 1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, 1156, 480, teamId);
                        }
                        else if (FX > 1156 && FZ < -556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX + 58, -480, teamId);
                        }
                        else if (FX > 1156 && FZ > 556)
                        {
                            SwimToDest(mission, ref  decisions, fish_cur, FX + 58, 480, teamId);
                        }

                    }



                }

            }

            if (BX < -1094 && FX < -1000)
                na1 = 1;
            else if (BX > 1094 && FX > 1000)
                na1 = 2;
            else
                na1 = 0;
            #endregion

            #region//  左场

            if (na1 == 1)
            {
                #region //鱼的角度为负值时

                if (FRad < 0)
                {
                    if (FX > BX - 58 && BX <= -1250)
                    {

                        FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);

                    }
                    else if (BX < -1250 && FZ - 100 > BZ)
                    {
                        FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ + 100);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 100;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                    }

                    else if (BX < -1250)
                    {
                        decisions[fish_cur].TCode = 10;
                        decisions[fish_cur].VCode = 1;
                    }

                    else if (BX > -1250)
                    {

                        if (FZ - 150 >= BZ)

                            li = 1;
                        else if (FZ - 150 < BZ && FZ - 100 >= BZ)

                            li = 2;
                        else if (FZ - 100 <= BZ && BZ >= FZ + 80)

                            li = 3;
                        else if (BZ <= FZ + 80)
                            li = 4;
                        if (li == 1)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX - 29, BZ + 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);
                        }
                        else if (li == 2)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX - 29, BZ + 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                        }
                        else if (li == 3)
                        {
                            if (FX > BX - 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }


                            decisions[fish_cur].TCode = 0;
                            decisions[fish_cur].VCode = 1;

                        }
                        else if (li == 4)
                        {
                            if (FX > BX - 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }

                            if (BX < -1250 && FZ - 100 > BZ)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ + 100);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 100;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                            }

                            decisions[fish_cur].TCode = 14;
                            decisions[fish_cur].VCode = 1;

                        }
                    }


                }
                #endregion

                #region //鱼的角度为正值时

                if (FRad > 0)
                {
                    if (FX > BX - 58 && BX <= -1250)
                    {

                        FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);

                    }
                    else if (BX < -1250 && FZ + 100 < BZ)
                    {
                        FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ - 100);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 100;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                    }

                    else if (BX < -1250)
                    {
                        decisions[fish_cur].TCode = 4;
                        decisions[fish_cur].VCode = 1;
                    }

                    else if (BX > -1250)
                    {

                        if (FZ + 150 <= BZ)

                            li = 1;
                        else if (FZ + 150 > BZ && FZ + 100 <= BZ)

                            li = 2;
                        else if (FZ + 100 >= BZ && BZ <= FZ - 80)

                            li = 3;
                        else if (BZ >= FZ - 80)
                            li = 4;
                        if (li == 1)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX - 29, BZ - 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);
                        }
                        else if (li == 2)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX - 29, BZ - 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                        }
                        else if (li == 3)
                        {
                            if (FX > BX - 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }


                            decisions[fish_cur].TCode = 14;
                            decisions[fish_cur].VCode = 1;

                        }
                        else if (li == 4)
                        {
                            if (FX > BX - 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }
                            if (BX < -1250 && FZ + 100 < BZ)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX - 58, BZ - 100);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX - 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 100;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                            }

                            decisions[fish_cur].TCode = 0;
                            decisions[fish_cur].VCode = 1;

                        }
                    }
                }
            }

                #endregion

            #endregion
            #region//  右场
            if (na1 == 2)
            {
                #region//鱼的角度为负值时

                if (FRad <= 0)
                {
                    if (FX < BX + 58 && BX >= 1250)
                    {

                        FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);

                    }
                    else if (BX > 1250 && FZ - 100 > BZ)
                    {
                        FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ + 100);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 100;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                    }

                    else if (BX > 1250)
                    {
                        decisions[fish_cur].TCode = 4;
                        decisions[fish_cur].VCode = 1;
                    }

                    else if (BX < 1250)
                    {

                        if (FZ - 150 >= BZ)

                            li = 1;
                        else if (FZ - 150 < BZ && FZ - 100 >= BZ)

                            li = 2;
                        else if (FZ - 100 <= BZ && BZ >= FZ + 80)

                            li = 3;
                        else if (BZ <= FZ + 80)
                            li = 4;
                        if (li == 1)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX + 29, BZ + 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);
                        }
                        else if (li == 2)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX + 29, BZ + 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                        }
                        else if (li == 3)
                        {
                            if (FX < BX + 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }


                            decisions[fish_cur].TCode = 14;
                            decisions[fish_cur].VCode = 1;

                        }
                        else if (li == 4)
                        {
                            if (FX < BX + 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }
                            if (BX > 1250 && FZ - 100 > BZ)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ + 100);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ + 100;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                            }

                            decisions[fish_cur].TCode = 0;
                            decisions[fish_cur].VCode = 1;

                        }
                    }


                }

                #endregion

                #region//鱼的角度为正值时
                if (FRad > 0)
                {
                    if (FX < BX + 58 && BX >= 1250)
                    {

                        FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);

                    }
                    else if (BX > 1250 && FZ + 100 < BZ)
                    {
                        FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ - 100);
                        FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                        mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 100;
                        StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                    }

                    else if (BX > 1250)
                    {
                        decisions[fish_cur].TCode = 10;
                        decisions[fish_cur].VCode = 1;
                    }

                    else if (BX < 1250)
                    {

                        if (FZ + 150 <= BZ)

                            li = 1;
                        else if (FZ + 150 > BZ && FZ + 100 <= BZ)

                            li = 2;
                        else if (FZ + 100 >= BZ && BZ <= FZ - 80)

                            li = 3;
                        else if (BZ >= FZ - 80)
                            li = 4;
                        if (li == 1)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX + 29, BZ - 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);
                        }
                        else if (li == 2)
                        {
                            FBZXangle1 = GetAnyangle(FX, FZ, BX + 29, BZ - 29);
                            FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 29;
                            mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 29;
                            StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                        }
                        else if (li == 3)
                        {
                            if (FX < BX + 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }


                            decisions[fish_cur].TCode = 0;
                            decisions[fish_cur].VCode = 1;

                        }
                        else if (li == 4)
                        {
                            if (FX < BX + 58)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 7, 7, 8, 100, true);

                            }

                            if (BX > 1250 && FZ + 100 < BZ)
                            {
                                FBZXangle1 = GetAnyangle(FX, FZ, BX + 58, BZ - 100);
                                FBZXangle2 = (float)(FBZXangle1 * 180 / Math.PI);
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X = BX + 58;
                                mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z = BZ - 100;
                                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[teamId].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 10, 150, 5, 15, 8, 100, true);


                            }
                            decisions[fish_cur].TCode = 14;
                            decisions[fish_cur].VCode = 1;

                        }
                    }

                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region 球门死区鱼游出函数  by 李飞海
        void Goout(Mission mission, ref Decision[] fish, int teamId, int whese, int way)//way0上出1下出
        {
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            if (FX > 915)
            {
                if (way == 1)
                {
                    if (FZ < 566)
                        SwimToDest(mission, ref  decisions, fish_cur, 1200, 600, teamId);
                    else if (whese == 1)
                        SwimToDest(mission, ref  decisions, fish_cur, 900, 566, teamId);
                    else
                        SwimToDest(mission, ref  decisions, fish_cur, -900, FZ, teamId);
                }

                else
                {
                    if (FZ > -566)
                        SwimToDest(mission, ref  decisions, fish_cur, 1200, -600, teamId);
                    else if (whese == 1)
                        SwimToDest(mission, ref  decisions, fish_cur, 900, -566, teamId);
                    else
                        SwimToDest(mission, ref  decisions, fish_cur, -900, FZ, teamId);
                }

            }
            else if (FX < -915)
            {
                if (way == 1)
                {
                    if (FZ < 566)
                        SwimToDest(mission, ref  decisions, fish_cur, -1200, 600, teamId);
                    else if (whese == 1)
                        SwimToDest(mission, ref  decisions, fish_cur, -900, 566, teamId);
                    else
                        SwimToDest(mission, ref  decisions, fish_cur, 900, FZ, teamId);
                }

                else
                {
                    if (FZ > -566)
                        SwimToDest(mission, ref  decisions, fish_cur, -1200, -600, teamId);
                    else if (whese == 1)
                        SwimToDest(mission, ref  decisions, fish_cur, -900, -566, teamId);
                    else
                        SwimToDest(mission, ref  decisions, fish_cur, 900, FZ, teamId);

                }
            }



        }
        #endregion

        #region 掏球函数  by 李飞海
        void Tao_BallRight(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {
            FRad = mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad;

            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            if (FX < -915 && FZ < 566 && FZ >= 0)
            {
                SwimToDest(mission, ref  decisions, fish_cur, -1156, 600, teamId);
            }
            else if (FX < -915 && FZ > -566 && FZ < 0)
            {
                SwimToDest(mission, ref  decisions, fish_cur, -1156, -600, teamId);
            }
            else if (FX <= 915 && FZ >= 0)
            {

                SwimToDest(mission, ref  decisions, fish_cur, 945, 566, teamId);
            }
            else if (FX <= 915 && FZ < 0)
            {

                SwimToDest(mission, ref  decisions, fish_cur, 945, -566, teamId);
            }

            else if (FX > 915 && FX <= 1500 && FZ > 556)
            {
                jdt1 = 0;

                SwimToDest(mission, ref  decisions, fish_cur, 1156, 480, teamId);
            }
            else if (FX > 915 && FX <= 1500 && FZ < -556)
            {
                jdt1 = 1;

                SwimToDest(mission, ref  decisions, fish_cur, 1156, -480, teamId);
            }
            else if (FX > 1200 && FZ < 556 && FZ > -556)
            {

                SwimToDest(mission, ref  decisions, fish_cur, 1080, 0, teamId);
            }
            #region //掏球
            else if (FX <= 1200)
            {
                if (FRad < 0)
                    FRad += 6.28F;

                if (FRad < 3.14)
                    jdt1 = 1;

                if (FZ > 400)
                {
                    #region//判断上掏球
                    jdt11 = 0;
                    if (jdt11 == 0)
                        jdt1 = 0;

                    if (FRad < 0)
                        FRad += 6.28F;
                    if (FRad < 4.012)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 7;
                    }
                    else if (FRad > 4.1867)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 7;

                    }
                    else
                    {
                        decisions[fish_cur].TCode = 7;
                        decisions[fish_cur].VCode = 14;

                    }
                    #endregion
                }


                else if (FZ < -400)
                {
                    #region//判断下掏球
                    jdt11 = 1;
                    if (jdt11 == 1)
                        jdt1 = 1;

                    if (FRad < 0)
                        FRad += 6.28F;
                    if (FRad > 2.267)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 7;
                    }
                    else if (FRad < 2.09333)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 7;
                    }
                    else
                    {
                        decisions[fish_cur].TCode = 7;
                        decisions[fish_cur].VCode = 14;
                    }
                    #endregion
                }
                else
                {

                    #region//上掏球
                    if (jdt1 == 0)
                    {

                        if (FRad < 0)
                            FRad += 6.28F;
                        if (FZ > -150 && FZ < 150)
                        {
                            if (FRad < 3.31444)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad > 4.18666)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;

                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;

                            }
                        }
                        else
                        {
                            if (FRad < 4.0122)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad > 4.1867)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;

                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;

                            }

                        }
                    }
                    #endregion
                    #region//下掏球
                    if (jdt1 == 1)
                    {

                        if (FRad < 0)
                            FRad += 6.28F;
                        if (FZ > -150 && FZ < 150)
                        {
                            if (FRad > 2.26777)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad < 1.577)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;
                            }
                        }
                        else
                        {
                            if (FRad > 2.2667)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad < 2.09333)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;
                            }

                        }
                    }
                    #endregion

                }

            }
            #endregion
        }
        void Tao_BallLeft(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {
            #region//掏左边的球门

            FRad = mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad;

            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            if (dangerarea(FXP, FZP) == 1)
            {
                if (FZ >= 0)
                {

                    SwimToDest(mission, ref  decisions, fish_cur, 1250, 566, teamId);
                }
                else
                {

                    SwimToDest(mission, ref  decisions, fish_cur, 1250, -566, teamId);
                }
            }

            else if ((FX >= -915 && FZ >= 0) && dangerarea(FXP, FZP) != 1)
            {
                jdt11 = 0;

                SwimToDest(mission, ref  decisions, fish_cur, -945, 566, teamId);
            }
            else if ((FX >= -915 && FZ < 0) && dangerarea(FXP, FZP) != 1)
            {
                jdt11 = 1;

                SwimToDest(mission, ref  decisions, fish_cur, -945, -566, teamId);
            }

            else if (FX < -915 && FX >= -1500 && FZ > 556)
            {

                SwimToDest(mission, ref  decisions, fish_cur, -1156, 480, teamId);
            }
            else if (FX < -915 && FX >= -1500 && FZ < -556)
            {

                SwimToDest(mission, ref  decisions, fish_cur, -1156, -480, teamId);
            }
            else if (FX < -1200 && FZ < 556 && FZ > -556)
            {

                SwimToDest(mission, ref  decisions, fish_cur, -1080, 0, teamId);
            }
            else if (FX >= -1200)
            {
                if (FRad < 0)
                    jdt1 = 0;
                if (FZ > 400)
                {
                    jdt11 = 0;
                    if (jdt11 == 0)
                        jdt1 = 0;

                    if (FRad < -1.2211)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 7;
                    }
                    else if (FRad > -0.8722)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 7;

                    }
                    else
                    {
                        decisions[fish_cur].TCode = 7;
                        decisions[fish_cur].VCode = 14;

                    }

                }
                else if (FZ < -400)
                {
                    jdt11 = 1;
                    if (jdt11 == 1)
                        jdt1 = 1;

                    if (FRad > 1.2211)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 7;
                    }
                    else if (FRad < 0.87223)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 7;
                    }
                    else
                    {
                        decisions[fish_cur].TCode = 7;
                        decisions[fish_cur].VCode = 14;
                    }

                }
                else
                {
                    if (jdt1 == 0)
                    {

                        if (FZ > -150 && FZ < 150)
                        {
                            if (FRad < -1.7444)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad > -0.5233)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;

                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;

                            }
                        }
                        else
                        {
                            if (FRad < -0.87223)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad > -1.0466)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;

                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;

                            }

                        }
                    }
                    else if (jdt1 == 1)
                    {

                        if (FZ > -150 && FZ < 150)
                        {
                            if (FRad > 0.52333)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad < 1.7444)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;
                            }
                        }
                        else
                        {
                            if (FRad > 1.04666)
                            {
                                decisions[fish_cur].TCode = 0;
                                decisions[fish_cur].VCode = 14;
                            }
                            else if (FRad < 0.87223)
                            {
                                decisions[fish_cur].TCode = 14;
                                decisions[fish_cur].VCode = 14;
                            }
                            else
                            {
                                decisions[fish_cur].TCode = 7;
                                decisions[fish_cur].VCode = 14;
                            }

                        }
                    }

                }
            }
            #endregion
        }
        #endregion

        #region  防守函数  by 李飞海

        void Ya_BallRightup1(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            if (FRad < 0)
                FRad += 360;
            if (BZ > -250)
            {
                BX0 = (float)BX - 25;
                BZ0 = (float)BZ + 40;
                FBZXangle1 = GetAnyangle(2 * BX - FX, FZ, BX0, BZ0);
                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[0].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 8, 150, 14, 5, 8, 100, true);
            }
            else if (BZ > -350)
            {
                #region //压球
                if (FRad < 190)
                {
                    decisions[fish_cur].TCode = 14;
                    decisions[fish_cur].VCode = 14;
                }
                else
                {

                    decisions[fish_cur].TCode = 5;
                    decisions[fish_cur].VCode = 6;
                }
                #endregion

            }
            else
            {
                #region//防守
                if (FZ > -250)
                {

                    if (FRad < 190)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 14;
                    }
                    else
                    {

                        decisions[fish_cur].TCode = 5;
                        decisions[fish_cur].VCode = 6;
                    }


                }
                else if (FZ < -250)
                {

                    if (FRad < 175)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 1;
                    }
                    else if (FRad < 195)
                    {


                        if (FRad < 180)
                        {
                            decisions[fish_cur].VCode = 14;
                            decisions[fish_cur].TCode = 0;
                        }
                        else
                        {
                            decisions[fish_cur].VCode = 1;
                            decisions[fish_cur].TCode = 5;
                        }
                    }
                    else
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 14;
                    }

                }
                #endregion
            }
        }
        void Ya_BallRightdown1(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            if (FRad < 0)
                FRad += 360;
            if (BZ < 250)
            {
                BX0 = (float)BX - 25;
                BZ0 = (float)BZ - 40;
                FBZXangle1 = GetAnyangle(2 * BX - FX, FZ, BX0, BZ0);
                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[0].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 8, 150, 14, 5, 8, 100, true);
            }
            else if (BZ < 350)
            {
                #region //压球
                if (FRad > 170)
                {
                    decisions[fish_cur].TCode = 0;
                    decisions[fish_cur].VCode = 14;
                }
                else
                {

                    decisions[fish_cur].TCode = 9;
                    decisions[fish_cur].VCode = 6;
                }
                #endregion


            }
            else
            {
                #region//防守
                if (FZ < 250)
                {

                    if (FRad > 170)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 14;
                    }
                    else
                    {

                        decisions[fish_cur].TCode = 9;
                        decisions[fish_cur].VCode = 6;
                    }



                }

                else if (FZ > 250 && FZ < 470)
                {

                    if (FRad > 185)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 1;
                    }
                    else if (FRad > 165)
                    {


                        if (FRad > 180)
                        {
                            decisions[fish_cur].TCode = 14;
                            decisions[fish_cur].VCode = 14;
                        }
                        else
                        {
                            decisions[fish_cur].TCode = 9;
                            decisions[fish_cur].VCode = 1;
                        }
                    }
                    else
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 14;
                    }

                }
                #endregion
            }



        }
        void Ya_BallLeftup1(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);

            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            if (BZ > -250)
            {
                BX0 = (float)BX + 25;
                BZ0 = (float)BZ + 40;
                FBZXangle1 = GetAnyangle(2 * BX - FX, FZ, BX0, BZ0);
                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[0].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle2, 5, 8, 150, 14, 5, 8, 100, true);

            }
            else if (BZ > -350)
            {
                #region //压球
                if (FRad > -10)
                {
                    decisions[fish_cur].TCode = 0;
                    decisions[fish_cur].VCode = 14;
                }
                else
                {

                    decisions[fish_cur].TCode = 9;
                    decisions[fish_cur].VCode = 6;
                }
                #endregion

            }
            else
            {
                #region//防守
                if (FZ > -250)
                {

                    if (FRad > -10)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 14;
                    }
                    else
                    {

                        decisions[fish_cur].TCode = 9;
                        decisions[fish_cur].VCode = 6;
                    }


                }
                else if (FZ < -250)
                {

                    if (FRad > 5)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 1;
                    }
                    else if (FRad > -15)
                    {


                        if (FRad > 0)
                        {
                            decisions[fish_cur].VCode = 14;
                            decisions[fish_cur].TCode = 14;
                        }
                        else
                        {
                            decisions[fish_cur].VCode = 1;
                            decisions[fish_cur].TCode = 9;
                        }
                    }
                    else
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 14;
                    }

                }
                #endregion
            }
        }
        void Ya_BallLeftdown1(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);

            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            if (FX < -1160)
            {
                SwimToDest(mission, ref  decisions, fish_cur, -980, 150, teamId);
            }
            if (BZ < 250)
            {
                BX0 = (float)BX + 25;
                BZ0 = (float)BZ - 40;
                FBZXangle1 = GetAnyangle(2 * BX - FX, FZ, BX0, BZ0);
                StrategyHelper.Helpers.Dribble(ref decisions[fish_cur], mission.TeamsRef[0].Fishes[fish_cur], mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm, FBZXangle1, 5, 8, 150, 14, 5, 8, 100, true);
            }



            if (BZ < 350 && BZ > 250)
            {
                #region //压球
                if (FRad < 10)
                {
                    decisions[fish_cur].TCode = 14;
                    decisions[fish_cur].VCode = 14;
                }
                else
                {

                    decisions[fish_cur].TCode = 5;
                    decisions[fish_cur].VCode = 5;
                }
                #endregion

            }
            else
            {
                #region//防守
                if (FZ < 250)
                {

                    if (FRad < 10)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 14;
                    }
                    else
                    {

                        decisions[fish_cur].TCode = 5;
                        decisions[fish_cur].VCode = 6;
                    }

                }
                else if (FZ > 250)
                {

                    if (FRad < -5)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 1;
                    }
                    else if (FRad < 15)
                    {


                        if (FRad < 0)
                        {
                            decisions[fish_cur].VCode = 14;
                            decisions[fish_cur].TCode = 0;
                        }
                        else
                        {
                            decisions[fish_cur].VCode = 1;
                            decisions[fish_cur].TCode = 5;
                        }
                    }
                    else
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 14;
                    }
                }
                #endregion

            }
        }

        #endregion

        #region 嚎球函数  by  汤纬倩，李飞海
        int Go_onLeftdown(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            int shi = 10;
            xna.Vector3 destPtMmg;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            destPtMmg.X = -1400;
            destPtMmg.Y = 0;
            destPtMmg.Z = 1020;
            float angle = GetAnyangle(FX, FZ, destPtMmg.X, destPtMmg.Z);
            if (FX > 950)
                Goout(mission, ref decisions, teamId, 0, 1);

            if (FX < 950)
            {
                if (FX > -950)
                {
                    shi = 1; SwimToDest(mission, ref decisions, fish_cur, -950, 1150, 0);

                }


                else if (FXP > -1220 && FX < -950 && FZP > 850)
                {
                    shi = 2; SwimToDest(mission, ref decisions, fish_cur, -1530, 1350, 0);
                }

                else if (FXP > -1480 && FX <= -1220 && FZP > 850)
                {
                    shi = 3; SwimToDest(mission, ref decisions, fish_cur, -1530, 1150, 0);
                }

                else
                {

                    if (FZP > 900)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 14;
                    }
                    else if (FZP > 260 && JudgeLeftgo_down_howmanyball(mission, ref decisions, teamId) != 0)
                    {
                        shi = 4;
                        SwimToDest(mission, ref decisions, fish_cur, -2100, 260, 0);

                    }
                    else
                        Goout(mission, ref decisions, teamId, 1, 0);

                }

            }


            return shi;

        }
        int Go_onLeftup(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            int shi = 10;
            xna.Vector3 destPtMmg;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            destPtMmg.X = -1400;
            destPtMmg.Y = 0;
            destPtMmg.Z = 1020;
            float angle = GetAnyangle(FX, FZ, destPtMmg.X, destPtMmg.Z);
            if (FX > 950)
                Goout(mission, ref decisions, teamId, 0, 0);

            if (FX < 950)
            {
                if (FX > -950)
                {
                    shi = 1; SwimToDest(mission, ref decisions, fish_cur, -950, -1150, 0);

                }


                else if (FXP > -1220 && FX < -950 && FZP < -850)
                {
                    shi = 2; SwimToDest(mission, ref decisions, fish_cur, -1530, -1350, 0);
                }

                else if (FXP > -1480 && FX <= -1220 && FZP < -850)
                {
                    shi = 3; SwimToDest(mission, ref decisions, fish_cur, -1530, -1150, 0);
                }

                else
                {

                    if (FZP < -900)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 14;
                    }
                    else if (FZP < -260 && JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0)
                    {
                        shi = 4;
                        SwimToDest(mission, ref decisions, fish_cur, -2100, -260, 0);

                    }
                    else
                        Goout(mission, ref decisions, teamId, 1, 1);

                }

            }


            return shi;

        }
        int Go_onRightdown(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            int shi = 10;
            xna.Vector3 destPtMmg;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            destPtMmg.X = -1400;
            destPtMmg.Y = 0;
            destPtMmg.Z = 1020;
            float angle = GetAnyangle(FX, FZ, destPtMmg.X, destPtMmg.Z);
            if (FX < -950)
                Goout(mission, ref decisions, teamId, 0, 1);

            if (FX > -950)
            {
                if (FX < 950)
                {
                    shi = 1; SwimToDest(mission, ref decisions, fish_cur, 950, 1150, 1);

                }


                else if (FXP < 1220 && FX > 950 && FZP > 850)
                {
                    shi = 2; SwimToDest(mission, ref decisions, fish_cur, 1530, 1350, 1);
                }

                else if (FXP < 1480 && FX >= 1220 && FZP > 850)
                {
                    shi = 3; SwimToDest(mission, ref decisions, fish_cur, 1530, 1150, 1);
                }

                else
                {

                    if (FZP > 900)
                    {
                        decisions[fish_cur].TCode = 0;
                        decisions[fish_cur].VCode = 14;
                    }
                    else if (FZP > 260 && JudgeRightgo_down_howmanyball(mission, ref decisions, teamId) != 0)
                    {
                        shi = 4;
                        SwimToDest(mission, ref decisions, fish_cur, 2100, 260, 1);

                    }
                    else
                        Goout(mission, ref decisions, teamId, 1, 0);

                }

            }


            return shi;

        }
        int Go_onRightup(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            int shi = 10;
            xna.Vector3 destPtMmg;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            destPtMmg.X = -1400;
            destPtMmg.Y = 0;
            destPtMmg.Z = 1020;
            float angle = GetAnyangle(FX, FZ, destPtMmg.X, destPtMmg.Z);
            if (FX < -950)
                Goout(mission, ref decisions, teamId, 0, 0);

            if (FX > -950)
            {
                if (FX < 950)
                {
                    shi = 1; SwimToDest(mission, ref decisions, fish_cur, 950, -1150, 1);

                }


                else if (FXP < 1220 && FX > 950 && FZP < -850)
                {
                    shi = 2; SwimToDest(mission, ref decisions, fish_cur, 1530, -1350, 1);
                }

                else if (FXP < -480 && FX >= 1220 && FZP < -850)
                {
                    shi = 3; SwimToDest(mission, ref decisions, fish_cur, 1530, -1150, 1);
                }

                else
                {

                    if (FZP < -900)
                    {
                        decisions[fish_cur].TCode = 14;
                        decisions[fish_cur].VCode = 14;
                    }
                    else if (FZP < -260 && JudgeRightgo_up_howmanyball(mission, ref decisions, teamId) != 0)
                    {
                        shi = 4;
                        SwimToDest(mission, ref decisions, fish_cur, 2100, -260, 1);

                    }
                    else
                        Goout(mission, ref decisions, teamId, 1, 1);

                }

            }


            return shi;

        }
        #endregion


        #endregion


        #region  局部整体


        #region 整体耗球函数  by 李飞海

        void haoqiuleft(Mission mission, ref Decision[] fish, int teamId)
        {

            int temp = 0; int temp1 = 0;
            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1200 && mission.EnvRef.Balls[i].PositionMm.Z < -400)
                {
                    temp++;

                }

            }
            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1200 && mission.EnvRef.Balls[i].PositionMm.Z > 400)
                {
                    temp1++;

                }

            }


            if (temp > 0 && temp1 > 0)
            {
                if (temp >= temp1)
                {
                    Go_onLeftup(mission, ref decisions, fish_cur, teamId);

                }

                else if (temp < temp1)
                {
                    Go_onLeftdown(mission, ref decisions, fish_cur, teamId);

                }


            }

            else if (temp > 0 && temp1 == 0)
            {
                Go_onLeftup(mission, ref decisions, fish_cur, teamId);

            }

            else if (temp == 0 && temp1 > 0)
            {
                Go_onLeftdown(mission, ref decisions, fish_cur, teamId);

            }

            else
            {
                if (JudgeSouLeft_ball(mission, ref decisions, teamId) == 10)
                    JudgeSouLeft_ball(mission, ref decisions, teamId);
                else
                    ball_cur[fish_cur] = JudgeSouLeft_ball(mission, ref decisions, teamId);
                SoloArea_left(mission, ref decisions, teamId);
            }
            temp = 0; temp1 = 0;

        }
        void haoqiuright(Mission mission, ref Decision[] fish, int teamId)
        {

            int temp = 0, temp1 = 0;
            for (int i = 0; i < 9; ++i)
            {
                if (mission.EnvRef.Balls[i].PositionMm.X > 1200 && mission.EnvRef.Balls[i].PositionMm.Z < -400)
                {
                    temp++;

                }
            }

            for (int i = 0; i < 9; ++i)
            {
                if (mission.EnvRef.Balls[i].PositionMm.X > 1200 && mission.EnvRef.Balls[i].PositionMm.Z > 400)
                {
                    temp1++;

                }

            }
            if (temp > 0 && temp1 > 0)
            {
                if (temp > temp1)
                {
                    Go_onRightup(mission, ref decisions, fish_cur, teamId);

                }

                else if (temp <= temp1)
                {
                    Go_onRightdown(mission, ref decisions, fish_cur, teamId);

                }


            }

            else if (temp > 0 && temp1 == 0)
            {
                Go_onRightup(mission, ref decisions, fish_cur, teamId);

            }

            else if (temp == 0 && temp1 > 0)
            {
                Go_onRightdown(mission, ref decisions, fish_cur, teamId);

            }

            else
            {
                if (JudgeSouRight_ball(mission, ref decisions, teamId) == 10)
                    JudgeSouRight_ball(mission, ref decisions, teamId);
                else
                    ball_cur[fish_cur] = JudgeSouRight_ball(mission, ref decisions, teamId);
                SoloArea_right(mission, ref decisions, teamId);

            }
            temp = 0; temp1 = 0;
        }

        #endregion

        #region 整体进球函数  by 李飞海

        #region //左下整体进球

        int JudgeLeftgo_down_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1094 && mission.EnvRef.Balls[i].PositionMm.Z > 340 && mission.EnvRef.Balls[i].PositionMm.Z < FZS)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int JudgeLeft_ball(Mission mission, ref Decision[] fish, int teamId)
        {
            int LFH;
            int temp_n = 10;
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            for (int i = 0; i < 8; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1094 && mission.EnvRef.Balls[i].PositionMm.Z > 0 && mission.EnvRef.Balls[i].PositionMm.Z < 470)
                {
                    temp_n = i;


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X < -1094 && mission.EnvRef.Balls[i + 1].PositionMm.Z > 0 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 470)
                    {
                        if (FX < -1000 && FZ > 470 && FZ < -470)
                        {
                            if (Math.Abs(mission.EnvRef.Balls[i].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z - FZ))
                                temp_n = i + 1;
                            else
                                temp_n = i;

                        }
                    }


                }

            }

            if (mission.EnvRef.Balls[8].PositionMm.X < -1094 && mission.EnvRef.Balls[8].PositionMm.Z > 0 && mission.EnvRef.Balls[8].PositionMm.Z < 470)
            {
                if (temp_nu == 10)
                    temp_nu = 8;
                else
                {
                    if (Math.Abs(mission.EnvRef.Balls[temp_nu].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[8].PositionMm.Z - FZ))
                        temp_nu = 8;
                }
            }

            LFH = Go_onLeftdown(mission, ref decisions, 0, teamId);
            if (LFH == 4)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (JudgeLeftgo_down_howmanyball(mission, ref decisions, teamId) > 1)


                        if (FZS > mission.EnvRef.Balls[i].PositionMm.Z && (mission.EnvRef.Balls[i].PositionMm.X < -1094 && mission.EnvRef.Balls[i].PositionMm.Z > 340 && mission.EnvRef.Balls[i].PositionMm.Z < 470))
                            temp_nu = 10;
                }
            }
            return temp_nu;
        }
        int JudgeLeftdown_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {

            int temp_nu = 0;


            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -950 && mission.EnvRef.Balls[i].PositionMm.Z > -440 && mission.EnvRef.Balls[i].PositionMm.Z < 1000)
                {
                    temp_nu++;

                }


            }



            return temp_nu;
        }
        void pull_inleftdown(Mission mission, ref Decision[] fish, int teamId, int L)
        {
            if (JudgeLeftdown_howmanyball(mission, ref decisions, teamId) < L)
            {
                if (fish_cur == 0)
                    ball_cur[fish_cur] = whichball(mission, ref decisions, teamId, 0);
                else
                    ball_cur[fish_cur] = whichball(mission, ref decisions, teamId, 1);
                SoloArea_left(mission, ref decisions, teamId);
            }
            else if (JudgeLeft_ball(mission, ref decisions, teamId) == 10)
            {
                Go_onLeftdown(mission, ref decisions, fish_cur, teamId);
            }
            else if (JudgeLeft_ball(mission, ref decisions, teamId) != 10)
            {

                ball_cur[fish_cur] = JudgeLeft_ball(mission, ref decisions, teamId);
                EdgeHandle(mission, ref decisions, teamId, 1);


            }
        }

        #endregion
        #region //左上整体进球

        int JudgeLeftgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1094 && mission.EnvRef.Balls[i].PositionMm.Z < -340 && mission.EnvRef.Balls[i].PositionMm.Z > FZS)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int JudgeLeft_ball1(Mission mission, ref Decision[] fish, int teamId)
        {
            int LFH = 0;
            int temp_nu = 10;
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            for (int i = 0; i < 8; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1094 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i].PositionMm.Z < 0)
                {
                    temp_nu = i;


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X < -1094 && mission.EnvRef.Balls[i + 1].PositionMm.Z > -470 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 0)
                    {
                        if (FX < -1000 && FZ > 470 && FZ < -470)
                        {
                            if (Math.Abs(mission.EnvRef.Balls[i].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z - FZ))
                                temp_nu = i + 1;
                            else
                                temp_nu = i;

                        }
                    }


                }

            }

            if (mission.EnvRef.Balls[8].PositionMm.X < -1094 && mission.EnvRef.Balls[8].PositionMm.Z > -470 && mission.EnvRef.Balls[8].PositionMm.Z < 0)
            {
                if (temp_nu == 10)
                    temp_nu = 8;
                else
                {
                    if (Math.Abs(mission.EnvRef.Balls[temp_nu].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[8].PositionMm.Z - FZ))
                        temp_nu = 8;
                }
            }

            LFH = Go_onLeftup(mission, ref decisions, fish_cur, teamId);
            if (LFH == 4)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) > 1)



                        temp_nu = 10;
                }
            }
            return temp_nu;
        }
        int JudgeLeftup_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {

            int temp_nu = 0;


            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -950 && mission.EnvRef.Balls[i].PositionMm.Z < 440 && mission.EnvRef.Balls[i].PositionMm.Z > -1000)
                {
                    temp_nu++;

                }


            }



            return temp_nu;
        }
        void pull_inleftup(Mission mission, ref Decision[] fish, int teamId, int L)
        {
            if (JudgeLeftup_howmanyball(mission, ref decisions, teamId) < L)
            {
                if (fish_cur == 0)
                    ball_cur[fish_cur] = whichball(mission, ref decisions, teamId, 0);
                else
                    ball_cur[fish_cur] = whichball(mission, ref decisions, teamId, 1);
                SoloArea_left(mission, ref decisions, teamId);
            }
            else if (JudgeLeft_ball1(mission, ref decisions, teamId) == 10)
            {
                Go_onLeftup(mission, ref decisions, fish_cur, teamId);
            }
            else if (JudgeLeft_ball1(mission, ref decisions, teamId) != 10)
            {

                ball_cur[fish_cur] = JudgeLeft_ball1(mission, ref decisions, teamId);
                EdgeHandle(mission, ref decisions, teamId, 1);



            }
        }

        #endregion
        #region //右下整体进球

        int JudgeRightgo_down_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1094 && mission.EnvRef.Balls[i].PositionMm.Z > 340 && mission.EnvRef.Balls[i].PositionMm.Z < FZS)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int JudgeRight_ball(Mission mission, ref Decision[] fish, int teamId)
        {
            int LFH = 0;
            int temp_nu = 10;
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            for (int i = 0; i < 8; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1094 && mission.EnvRef.Balls[i].PositionMm.Z > 0 && mission.EnvRef.Balls[i].PositionMm.Z < 470)
                {
                    temp_nu = i;


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X > 1094 && mission.EnvRef.Balls[i + 1].PositionMm.Z > 0 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 470)
                    {
                        if (FX > 1000 && FZ > 470 && FZ < -470)
                        {
                            if (Math.Abs(mission.EnvRef.Balls[i].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z - FZ))
                                temp_nu = i + 1;
                            else
                                temp_nu = i;

                        }
                    }


                }

            }

            if (mission.EnvRef.Balls[8].PositionMm.X > 1094 && mission.EnvRef.Balls[8].PositionMm.Z > 0 && mission.EnvRef.Balls[8].PositionMm.Z < 470)
            {
                if (temp_nu == 10)
                    temp_nu = 8;
                else
                {
                    if (Math.Abs(mission.EnvRef.Balls[temp_nu].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[8].PositionMm.Z - FZ))
                        temp_nu = 8;
                }
            }

            LFH = Go_onRightdown(mission, ref decisions, fish_cur, teamId);
            if (LFH == 4)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (JudgeRightgo_down_howmanyball(mission, ref decisions, teamId) > 1)


                        if (FZS > mission.EnvRef.Balls[i].PositionMm.Z && (mission.EnvRef.Balls[i].PositionMm.X > 1094 && mission.EnvRef.Balls[i].PositionMm.Z > 340 && mission.EnvRef.Balls[i].PositionMm.Z < 470))
                            temp_nu = 10;
                }
            }
            return temp_nu;
        }
        int JudgeRightdown_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {

            int temp_nu = 0;


            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 950 && mission.EnvRef.Balls[i].PositionMm.Z > -440 && mission.EnvRef.Balls[i].PositionMm.Z < 1000)
                {
                    temp_nu++;

                }


            }



            return temp_nu;
        }
        void pull_inRightdown(Mission mission, ref Decision[] fish, int teamId, int L)
        {
            if (JudgeRightdown_howmanyball(mission, ref decisions, teamId) < L)
            {
                if (fish_cur == 0)
                    ball_cur[fish_cur] = whichball1(mission, ref decisions, teamId, 0);
                else
                    ball_cur[fish_cur] = whichball1(mission, ref decisions, teamId, 1);

                SoloArea_right(mission, ref decisions, teamId);
            }
            else if (JudgeRight_ball(mission, ref decisions, teamId) == 10)
            {
                Go_onRightdown(mission, ref decisions, fish_cur, teamId);
            }
            else if (JudgeRight_ball(mission, ref decisions, teamId) != 10)
            {
                ball_cur[fish_cur] = JudgeRight_ball(mission, ref decisions, teamId);
                EdgeHandle(mission, ref decisions, teamId, 2);


            }
        }

        #endregion
        #region //右上整体进球

        int JudgeRightgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1094 && mission.EnvRef.Balls[i].PositionMm.Z < -340 && mission.EnvRef.Balls[i].PositionMm.Z > FZS)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int JudgeRight_ball1(Mission mission, ref Decision[] fish, int teamId)
        {
            int LFH = 0;
            int temp_nu = 10;
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            for (int i = 0; i < 8; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1094 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i].PositionMm.Z < 0)
                {
                    temp_nu = i;


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X > 1094 && mission.EnvRef.Balls[i + 1].PositionMm.Z > -470 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 0)
                    {
                        if (FX > 1000 && FZ > 470 && FZ < -470)
                        {
                            if (Math.Abs(mission.EnvRef.Balls[i].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z - FZ))
                                temp_nu = i + 1;
                            else
                                temp_nu = i;

                        }
                    }


                }

            }

            if (mission.EnvRef.Balls[8].PositionMm.X > 1094 && mission.EnvRef.Balls[8].PositionMm.Z > -470 && mission.EnvRef.Balls[8].PositionMm.Z < 0)
            {
                if (temp_nu == 10)
                    temp_nu = 8;
                else
                {
                    if (Math.Abs(mission.EnvRef.Balls[temp_nu].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[8].PositionMm.Z - FZ))
                        temp_nu = 8;
                }
            }

            LFH = Go_onRightup(mission, ref decisions, fish_cur, teamId);
            if (LFH == 4)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (JudgeRightgo_up_howmanyball(mission, ref decisions, teamId) > 1)



                        temp_nu = 10;
                }
            }
            return temp_nu;
        }
        int JudgeRightup_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {

            int temp_nu = 0;


            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 950 && mission.EnvRef.Balls[i].PositionMm.Z < 440 && mission.EnvRef.Balls[i].PositionMm.Z > -1000)
                {
                    temp_nu++;

                }


            }



            return temp_nu;
        }
        void pull_inRightup(Mission mission, ref Decision[] fish, int teamId, int L)
        {
            if (JudgeRightup_howmanyball(mission, ref decisions, teamId) < L)
            {
                if (fish_cur == 0)
                    ball_cur[fish_cur] = whichball1(mission, ref decisions, teamId, 0);
                else
                    ball_cur[fish_cur] = whichball1(mission, ref decisions, teamId, 1);
                SoloArea_right(mission, ref decisions, teamId);
            }
            else if (JudgeRight_ball1(mission, ref decisions, teamId) == 10)
            {
                Go_onRightup(mission, ref decisions, fish_cur, teamId);
            }
            else if (JudgeRight_ball1(mission, ref decisions, teamId) != 10)
            {

                ball_cur[fish_cur] = JudgeRight_ball1(mission, ref decisions, teamId);
                EdgeHandle(mission, ref decisions, teamId, 2);



            }
        }

        #endregion


        #region 左场整体进球

        int JudgeLeftall_ball(Mission mission, ref Decision[] fish, int teamId)
        {
            int temp_nu = 10;
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            for (int i = 0; i < 8; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1094 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i].PositionMm.Z < 470)
                {
                    temp_nu = i;


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X < -1094 && mission.EnvRef.Balls[i + 1].PositionMm.Z > -470 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 470)
                    {
                        if (FX < -1000 && FZ > 470 && FZ < -470)
                        {
                            if (Math.Abs(mission.EnvRef.Balls[i].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z - FZ))
                                temp_nu = i + 1;
                            else
                                temp_nu = i;

                        }
                    }


                }

            }

            if (mission.EnvRef.Balls[8].PositionMm.X < -1094 && mission.EnvRef.Balls[8].PositionMm.Z > -470 && mission.EnvRef.Balls[8].PositionMm.Z < 470)
            {
                if (temp_nu == 10)
                    temp_nu = 8;
                else
                {
                    if (Math.Abs(mission.EnvRef.Balls[temp_nu].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[8].PositionMm.Z - FZ))
                        temp_nu = 8;
                }
            }

            return temp_nu;
        }
        void pull_inleft(Mission mission, ref Decision[] fish, int teamId)
        {
            if (JudgeLeftall_ball(mission, ref decisions, teamId) == 10)
                JudgeLeftall_ball(mission, ref decisions, teamId);
            if (JudgeLeftall_ball(mission, ref decisions, teamId) == 10)
            {
                haoqiuleft(mission, ref decisions, teamId);
            }
            else if (JudgeLeftall_ball(mission, ref decisions, teamId) != 10)
            {

                ball_cur[fish_cur] = JudgeLeftall_ball(mission, ref decisions, teamId);
                EdgeHandle(mission, ref decisions, teamId, 1);


            }
        }

        #endregion
        #region 右场整体进球
        int JudgeRightall_ball(Mission mission, ref Decision[] fish, int teamId)
        {
            int temp_nu = 10;
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[4].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            for (int i = 0; i < 8; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1094 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i].PositionMm.Z < 470)
                {
                    temp_nu = i;


                    if (mission.EnvRef.Balls[i + 1].PositionMm.X > 1094 && mission.EnvRef.Balls[i + 1].PositionMm.Z > -470 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 470)
                    {
                        if (FX > 1000 && FZ > 470 && FZ < -470)
                        {
                            if (Math.Abs(mission.EnvRef.Balls[i].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z - FZ))
                                temp_nu = i + 1;
                            else
                                temp_nu = i;

                        }
                    }


                }

            }

            if (mission.EnvRef.Balls[8].PositionMm.X > 1094 && mission.EnvRef.Balls[8].PositionMm.Z > -470 && mission.EnvRef.Balls[8].PositionMm.Z < 470)
            {
                if (temp_nu == 10)
                    temp_nu = 8;
                else
                {
                    if (Math.Abs(mission.EnvRef.Balls[temp_nu].PositionMm.Z - FZ) > Math.Abs(mission.EnvRef.Balls[8].PositionMm.Z - FZ))
                        temp_nu = 8;
                }
            }

            return temp_nu;
        }
        void pull_inright(Mission mission, ref Decision[] fish, int teamId)
        {
            if (JudgeRightall_ball(mission, ref decisions, teamId) == 10)
                JudgeRightall_ball(mission, ref decisions, teamId);
            if (JudgeRightall_ball(mission, ref decisions, teamId) == 10)
            {
                haoqiuright(mission, ref decisions, teamId);
            }
            else if (JudgeRightall_ball(mission, ref decisions, teamId) != 10)
            {

                ball_cur[fish_cur] = JudgeRightall_ball(mission, ref decisions, teamId);
                EdgeHandle(mission, ref decisions, teamId, 2);



            }
        }
        #endregion

        #endregion

        #region 二鱼对对方门的防守  by  胡亮


        int Judge_balldown(Mission mission, ref Decision[] fish, int teamId)
        {
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            int temp_nu = 0;
            for (int i = 0; i < 8; i++)
            {
                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.Z > 0 && mission.EnvRef.Balls[i].PositionMm.Z < mission.TeamsRef[teamId].Fishes[0].PositionMm.Z)
                {
                    temp_nu++;

                }
            }
            return temp_nu;

        }
        int Judge_ballup(Mission mission, ref Decision[] fish, int teamId)
        {
            FX = mission.TeamsRef[0].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[0].Fishes[fish_cur].PositionMm.Z;

            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            int temp_nu = 0;
            for (int i = 0; i < 8; i++)
            {
                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.Z <= 0 && mission.EnvRef.Balls[i].PositionMm.Z > mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z)
                {
                    temp_nu++;
                }
            }


            return temp_nu;
        }
        void Ya_Ballleftdown(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {


            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);

            if (FRad < 0)
                FRad += 360;

            #region //压球
            if (FRad > 170)
            {
                decisions[fish_cur].TCode = 0;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 9;
                decisions[fish_cur].VCode = 6;
            }
        }
            #endregion


        int hl_JudgeLeftgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -800 && mission.EnvRef.Balls[i].PositionMm.Z > 566)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hll_JudgeLeftgo_down_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //  判断向下右压球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -550 && mission.EnvRef.Balls[i].PositionMm.Z > 700)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }

        int hll_JudgeLeftgo_up1_howmanyball(Mission mission, ref Decision[] fish, int teamId)   //判断上右压球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -550 && mission.EnvRef.Balls[i].PositionMm.Z < -700)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hll_JudgeLeftgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)   //整个角都有球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.Z > 566)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int h_JudgeLeftgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //墙壁有球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1350 && mission.EnvRef.Balls[i].PositionMm.X > -1520)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }


        int ball_in_JudgeLeftgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //禁区 内的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.X > -1200 && mission.EnvRef.Balls[i].PositionMm.Z < 480 && mission.EnvRef.Balls[i].PositionMm.Z > -480)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int ball_out_JudgeLeftgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)   //禁区外的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1100 && mission.EnvRef.Balls[i].PositionMm.Z < 600 && mission.EnvRef.Balls[i].PositionMm.Z > -600)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int ball_out_JudgeLeftgo_anyball(Mission mission, ref Decision[] fish, int teamId)   //外的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1100)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hl_JudgeLeft_down_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //禁区中线下的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.Z > 0 && mission.EnvRef.Balls[i].PositionMm.Z < 700)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hl_JudgeLeft_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)     //禁区中线上的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.Z <= 0 && mission.EnvRef.Balls[i].PositionMm.Z > -700)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hl_JudgeLeft_all_howmanyball(Mission mission, ref Decision[] fish, int teamId)     //禁区中线上的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -945 && mission.EnvRef.Balls[i].PositionMm.Z < 700 && mission.EnvRef.Balls[i].PositionMm.Z > -700)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hll_Go_onLeftdown(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            int shi = 10;

            xna.Vector3 destPtMmg;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            FXP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            destPtMmg.X = -1400;
            destPtMmg.Y = 0;
            destPtMmg.Z = -1020;

            float angle = GetAnyangle(FX, FZ, destPtMmg.X, destPtMmg.Z);
            if (FX > 950)
                Goout(mission, ref decisions, teamId, 0, 0);

            if (FX < 950)
            {
                if (FX > -950 && hl_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && ball_out_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0)
                {
                    shi = 1; SwimToDest(mission, ref decisions, fish_cur, -1150, -750, 1);

                }




                else if (FXP > -1220 && FX < -950 && FZP < -850)
                {
                    shi = 2; SwimToDest(mission, ref decisions, fish_cur, -1600, 500, 1);
                    SwimToDest(mission, ref decisions, fish_cur, -1600, 500, 1);


                }



                else
                {


                    if (FZP > -800 && FZP < -200)
                    {
                        SwimToDest(mission, ref decisions, fish_cur, -1630, 500, 1);
                    }
                    else if (FZP > -200 && FZP < 400)
                    {
                        SwimToDest(mission, ref decisions, fish_cur, -1630, 566, 1);
                    }
                    else if (FZP > 400 && FZP < 500)
                    {

                        SwimToDest(mission, ref decisions, fish_cur, -1630, 566, 1);
                    }
                    else if (hll_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && FZ > 500 && FZ < 800)
                    {
                        Ya_Ballleftdown(mission, ref decisions, teamId, fish_cur);
                    }
                    else if (h_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0)
                    {
                        SwimToDest(mission, ref decisions, fish_cur, -1530, 1350, 1);
                    }

                    else if (hl_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && FX < -1300)
                    {
                        SwimToDest(mission, ref decisions, fish_cur, -1100, 1600, 1);
                    }

                    else if (hl_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && FX < -1200)
                    {


                        decisions[fish_cur].TCode = 1;
                        decisions[fish_cur].VCode = 14;
                    }
                    else if (hl_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && FX < -1100)
                    {
                        SwimToDest(mission, ref decisions, fish_cur, -1000, 1600, 1);
                    }
                    else if (hl_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && FX < -1000)
                    {
                        SwimToDest(mission, ref decisions, fish_cur, -900, 1600, 1);
                    }
                    else if (hl_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0 && FX < -900)
                    {
                        Ya_Ball_downright(mission, ref decisions, teamId, fish_cur);
                    }

                    else
                        Ya_Ball_downright(mission, ref decisions, teamId, fish_cur);

                }

            }


            return shi;

        }

        void Ya_Ball_downright(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {


            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);


            #region //压球
            if (FRad > 90)
            {
                decisions[fish_cur].TCode = 0;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 9;
                decisions[fish_cur].VCode = 13;
            }
        }

        void Ya_Ballupright(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            if (FRad < 0)
                FRad += 360;




            #region //压球
            if (FRad < 270)
            {
                decisions[fish_cur].TCode = 14;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 5;
                decisions[fish_cur].VCode = 9;
            }
            #endregion

        }

        void Ya_Ballleftup(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);


            if (FRad < 0)
                FRad += 360;



            #region //压球
            if (FRad < 190)
            {
                decisions[fish_cur].TCode = 14;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 5;
                decisions[fish_cur].VCode = 6;
            }
            #endregion

        }
        void hlleft1(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            if (ball_in_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0)
            {

                Tao_BallLeft(mission, ref decisions, 1, fish_cur);
            }
            else if (hl_JudgeLeft_down_howmanyball(mission, ref decisions, teamId) >= hl_JudgeLeft_up_howmanyball(mission, ref decisions, teamId))
            {




                if (FZ > 250 && FZ < 566 && FX < -945 && FX > -1094)
                {

                    SwimToDest(mission, ref decisions, fish_cur, -945, 0, 1);
                }
                else if (FZ <= 0 && FZ > -566 && FX < -945 && FX > -1094 && ball_in_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) == 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 100, 1);
                }


                else if (ball_out_JudgeLeftgo_up_howmanyball(mission, ref decisions, 1) != 0 && FZ < 780)
                {
                    Ya_Ballleftdown(mission, ref decisions, teamId, fish_cur);
                }
                else
                {
                    Ya_Ballleftdown(mission, ref decisions, teamId, fish_cur);
                }
            }





            else if (hl_JudgeLeft_down_howmanyball(mission, ref decisions, teamId) < hl_JudgeLeft_up_howmanyball(mission, ref decisions, teamId))
            {


                if (FX < -945 && FX > -1150 && FZ < 566 && FZ > -566)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1600, -300, 1);
                }


                else if (FX > -945 && ball_out_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0)
                {

                    SwimToDest(mission, ref decisions, fish_cur, -1120, 750, 1);
                }
                else if (FZ < 950 && FZ > 700 && FX < -945)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1600, 580, 1);
                }
                else if (FZ < 850 && FZ > 600 && FX < -945)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1600, 580, 1);
                }

                else if (FZ > -780 && FX < -945)
                {
                    Ya_Ballleftup(mission, ref decisions, teamId, fish_cur);
                }
                else
                {
                    Ya_Ballupright(mission, ref decisions, teamId, fish_cur);

                }

            }
            else
            {
                Goout(mission, ref decisions, teamId, 0, 1);
            }


        }


        #region    //右场防守

        void Ya_Ballrightup(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);



            #region //压球
            if (FRad > -10)
            {
                decisions[fish_cur].TCode = 0;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 9;
                decisions[fish_cur].VCode = 6;
            }
            #endregion

        }
        void Ya_Ballrightdown(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);



            #region //压球
            if (FRad < 10)
            {
                decisions[fish_cur].TCode = 14;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 5;
                decisions[fish_cur].VCode = 6;
            }
            #endregion

        }
        void Ya_Ballupleft(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {

            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);
            if (FRad < 0)
                FRad += 360;




            #region //压球
            if (FRad > 270)
            {
                decisions[fish_cur].TCode = 0;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 9;
                decisions[fish_cur].VCode = 9;
            }
            #endregion

        }
        void Ya_Balldownleft(Mission mission, ref Decision[] fish, int teamId, int fish_cur)
        {


            FRad = (float)((mission.TeamsRef[teamId].Fishes[fish_cur].BodyDirectionRad / Math.PI) * 180);


            #region //压球
            if (FRad < 90)
            {
                decisions[fish_cur].TCode = 14;
                decisions[fish_cur].VCode = 14;
            }
            else
            {

                decisions[fish_cur].TCode = 5;
                decisions[fish_cur].VCode = 13;
            }
        }

        int hl_Judgerightgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -800 && mission.EnvRef.Balls[i].PositionMm.Z > 566)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }

        int hll_Judgerightgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)   //整个角都有球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 945 && mission.EnvRef.Balls[i].PositionMm.Z < -566)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hll_Judgerightgo_down_howmanyball(Mission mission, ref Decision[] fish, int teamId)   //整个角都有球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 945 && mission.EnvRef.Balls[i].PositionMm.Z > 566)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int h_Judgerightgo_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //墙壁有球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < -1350 && mission.EnvRef.Balls[i].PositionMm.X > -1520)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }


        int ball_in_Judgeright_go_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //禁区 内的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 945 && mission.EnvRef.Balls[i].PositionMm.X < 1200 && mission.EnvRef.Balls[i].PositionMm.Z < 566 && mission.EnvRef.Balls[i].PositionMm.Z > -566)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int ball_out_Judgeright_go_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)   //禁区外的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1100 && mission.EnvRef.Balls[i].PositionMm.Z < 600 && mission.EnvRef.Balls[i].PositionMm.Z > -600)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int ball_out_Judgerightgo_anyball(Mission mission, ref Decision[] fish, int teamId)   //外的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 1100)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hl_Judgeright_down_howmanyball(Mission mission, ref Decision[] fish, int teamId)    //禁区中线下的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 945 && mission.EnvRef.Balls[i].PositionMm.Z > 0 && mission.EnvRef.Balls[i].PositionMm.Z < 800)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }
        int hl_Judgeright_up_howmanyball(Mission mission, ref Decision[] fish, int teamId)     //禁区中线上的球
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[3].Z;
            int temp_nu = 0;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;

            for (int i = 0; i < 9; ++i)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > 945 && mission.EnvRef.Balls[i].PositionMm.Z <= 0 && mission.EnvRef.Balls[i].PositionMm.Z > -800)
                {
                    temp_nu++;
                    ball_cur[fish_cur] = i;
                }


            }



            return temp_nu;
        }



        void hlright(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            if (ball_in_Judgeright_go_up_howmanyball(mission, ref decisions, teamId) != 0)
            {

                Tao_BallRight(mission, ref decisions, teamId, fish_cur);
            }
            else if (hl_Judgeright_down_howmanyball(mission, ref decisions, teamId) >= hl_Judgeright_up_howmanyball(mission, ref decisions, teamId))
            {

                if (FZ < -750 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, -600, 0);
                }
                else if (FZ > -650 && FZ < -450 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, -400, 0);

                }
                else if (FZ > -450 && FZ < -250 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, -200, 0);

                }
                else if (FZ > -250 && FZ < 0 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);

                }






                else if (FZ < 566 && FZ > 350 && FX > 945 && FX < 1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 945, 300, 0);

                }
                else if (FZ < 350 && FZ > 200 && FX > 945 && FX < 1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 945, 150, 0);

                }
                else if (FZ < 150 && FZ > 0 && FX > 945 && FX < 1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);

                }
                else if (FX < 1100 && FX > 945 && FZ < 100 && FZ > 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);
                }
                else if (FX < 1100 && FX > 945 && FZ > -566 && FZ < 566)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);
                }




                else if (FZ < 800 && FZ > 0 && FX > 1350)
                {


                    Ya_Ballrightdown(mission, ref decisions, 0, fish_cur);
                }
                else
                {
                    Ya_Ballrightdown(mission, ref decisions, 0, fish_cur);
                }
            }


            else if ((hl_Judgeright_down_howmanyball(mission, ref decisions, teamId) < hl_Judgeright_up_howmanyball(mission, ref decisions, teamId)))
            {



                if (FZ > 800 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 600, 0);
                }
                else if (FZ < 650 && FZ > 450 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 400, 0);

                }
                else if (FZ < 450 && FZ > 250 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 200, 0);

                }
                else if (FZ < 250 && FZ > 0 && FX > 1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);

                }

                else if (FZ < 566 && FZ > 350 && FX > 945 && FX < 1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 945, 300, 0);

                }
                else if (FZ < 350 && FZ > 200 && FX > 945 && FX < 1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 945, 150, 0);

                }
                else if (FZ < 150 && FZ > 0 && FX > 945 && FX < 1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);

                }

                else if (FX < 1100 && FX > 945 && FZ < 100 && FZ > 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 0, 0);
                }



                else if (FZ > -800 && FZ < 0 && FX > 1350)
                {


                    Ya_Ballrightup(mission, ref decisions, 0, fish_cur);
                }
                else
                {
                    Ya_Ballrightup(mission, ref decisions, 0, fish_cur);
                }

            }

            else if (FZ < -800)
            {
                Ya_Ballupleft(mission, ref decisions, teamId, fish_cur);


            }
            else if (FZ > 800)
            {
                if (FZ < 900)
                {
                    SwimToDest(mission, ref decisions, fish_cur, 1500, 1000, 0);
                }
                else
                {

                    Ya_Balldownleft(mission, ref decisions, teamId, fish_cur);
                }

            }
            else
            {

                Goout(mission, ref decisions, 0, 0, 1);
            }

        }



            #endregion























        void hlleft(Mission mission, ref Decision[] fish, int fish_cur, int teamId)
        {
            FXS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].X;
            FZS = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            FX = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.X;
            FZ = mission.TeamsRef[teamId].Fishes[fish_cur].PositionMm.Z;
            BX = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.X;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            if (ball_in_JudgeLeftgo_up_howmanyball(mission, ref decisions, teamId) != 0)
            {

                Tao_BallLeft(mission, ref decisions, teamId, fish_cur);
            }
            else if (hl_JudgeLeft_down_howmanyball(mission, ref decisions, teamId) >= hl_JudgeLeft_up_howmanyball(mission, ref decisions, teamId) && hl_JudgeLeft_all_howmanyball(mission, ref decisions, teamId) != 0)
            {
                if (FX > -945)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1200, -600, 1);
                }



                else if (FZ < -750 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, -600, 1);
                }
                else if (FZ > -650 && FZ < -450 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, -400, 1);

                }
                else if (FZ > -450 && FZ < -250 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, -200, 1);

                }
                else if (FZ > -250 && FZ < 0 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);

                }






                else if (FZ < 566 && FZ > 350 && FX < -945 && FX > -1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -945, 300, 1);

                }
                else if (FZ < 350 && FZ > 200 && FX < -945 && FX > -1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -945, 150, 1);

                }
                else if (FZ < 150 && FZ > 0 && FX < -945 && FX > -1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);

                }
                else if (FX > -1100 && FX < -945 && FZ < 100 && FZ > 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }
                else if (FX > -1100 && FX < -945 && FZ > -566 && FZ < 566)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }
                else if (Judge_balldown(mission, ref decisions, teamId) != 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }




                else if (FZ < 800 && FZ > 0 && FX < -1350)
                {


                    Ya_Ballleftdown(mission, ref decisions, 1, fish_cur);
                }
                else
                {
                    Ya_Ballleftdown(mission, ref decisions, 1, fish_cur);
                }
            }


            else if (hl_JudgeLeft_down_howmanyball(mission, ref decisions, teamId) < hl_JudgeLeft_up_howmanyball(mission, ref decisions, teamId) && hl_JudgeLeft_all_howmanyball(mission, ref decisions, teamId) != 0)
            {

                if (FX > -945)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 600, 1);
                }

                else if (FZ > 800 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 600, 1);
                }
                else if (FZ < 650 && FZ > 450 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 400, 1);

                }
                else if (FZ < 450 && FZ > 250 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 200, 1);

                }
                else if (FZ < 250 && FZ > 0 && FX < -1350)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);

                }

                else if (FZ < 566 && FZ > 350 && FX < -945 && FX > -1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -945, 300, 1);

                }
                else if (FZ < 350 && FZ > 200 && FX < -945 && FX > -1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -945, 150, 1);

                }
                else if (FZ < 150 && FZ > 0 && FX < -945 && FX > -1100)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);

                }

                else if (FX > -1100 && FX < -945 && FZ < 100 && FZ > 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }
                else if (Judge_ballup(mission, ref decisions, teamId) > 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }
                else if (Judge_ballup(mission, ref decisions, teamId) > 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }
                else if (Judge_ballup(mission, ref decisions, teamId) != 0)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 0, 1);
                }



                else if (FZ > -800 && FZ < 0 && FX < -1350)
                {


                    Ya_Ballleftup(mission, ref decisions, 1, fish_cur);
                }
                else
                {
                    Ya_Ballleftup(mission, ref decisions, 1, fish_cur);
                }

            }

            else if (hl_JudgeLeft_all_howmanyball(mission, ref decisions, teamId) == 0)
            {
                if (FZ < -750 && hll_JudgeLeftgo_up1_howmanyball(mission, ref decisions, teamId) != 0)
                {
                    Ya_Ballupright(mission, ref decisions, teamId, fish_cur);
                }
                else if (FZ > 750 && hll_JudgeLeftgo_down_howmanyball(mission, ref decisions, teamId) != 0)
                {


                    Ya_Ball_downright(mission, ref decisions, teamId, fish_cur);


                }
                else if (FZ > 0 && FZ < 750)
                {
                    SwimToDest(mission, ref decisions, fish_cur, -1500, 800, teamId);
                }

            }
            else if (FZ > 750 && hl_JudgeLeft_all_howmanyball(mission, ref decisions, teamId) == 0)
            {


                Ya_Ball_downright(mission, ref decisions, teamId, fish_cur);


            }
            else
            {

                Goout(mission, ref decisions, 0, 0, 1);
            }

        }



        #endregion

            #endregion
        #endregion

        #region  一鱼对己方门的防守  by  李飞海

        #region  左场防守

        int Judge_yaleft_ball(Mission mission, ref Decision[] fish, int teamId)
        {
            yaball = 10;
            for (int i = 0; i < 8; i++)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X > -1094 && mission.EnvRef.Balls[i].PositionMm.X < -1030 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i].PositionMm.Z < 470)
                {
                    yaball = i; i = 8;
                }

            }
            for (int i = yaball; i < 8; i++)
            {


                if (mission.EnvRef.Balls[i + 1].PositionMm.X > -1094 && mission.EnvRef.Balls[i + 1].PositionMm.X < -1030 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 470)
                {
                    if (Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z) > Math.Abs(mission.EnvRef.Balls[yaball].PositionMm.Z))
                        yaball = i + 1;


                }
            }
            return yaball;
        }
        void defanse_left(Mission mission, ref Decision[] fish, int teamId)
        {
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            if (Judge_yaleft_ball(mission, ref decisions, teamId) == 10)
                Judge_yaleft_ball(mission, ref decisions, teamId);
            if (Judge_yaleft_ball(mission, ref decisions, teamId) != 10)
            {
                ball_cur[fish_cur] = Judge_yaleft_ball(mission, ref decisions, teamId);
                if (BZ > FZP)
                {
                    Ya_BallLeftdown1(mission, ref decisions, teamId, fish_cur);
                }
                else
                {
                    Ya_BallLeftup1(mission, ref decisions, teamId, fish_cur);
                }
            }
            else
                pull_inleft(mission, ref decisions, teamId);


        }

        #endregion

        #region   右场防守

        int Judge_yaright_ball(Mission mission, ref Decision[] fish, int teamId)
        {
            yaball = 10;
            for (int i = 0; i < 8; i++)
            {

                if (mission.EnvRef.Balls[i].PositionMm.X < 1094 && mission.EnvRef.Balls[i].PositionMm.X > 1030 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i].PositionMm.Z < 470)
                {
                    yaball = i; i = 8;
                }

            }
            for (int i = yaball; i < 8; i++)
            {


                if (mission.EnvRef.Balls[i + 1].PositionMm.X < 1094 && mission.EnvRef.Balls[i + 1].PositionMm.X > 1030 && mission.EnvRef.Balls[i].PositionMm.Z > -470 && mission.EnvRef.Balls[i + 1].PositionMm.Z < 470)
                {
                    if (Math.Abs(mission.EnvRef.Balls[i + 1].PositionMm.Z) > Math.Abs(mission.EnvRef.Balls[yaball].PositionMm.Z))
                        yaball = i + 1;


                }
            }
            return yaball;
        }
        void defanse_right(Mission mission, ref Decision[] fish, int teamId)
        {
            FZP = mission.TeamsRef[teamId].Fishes[fish_cur].PolygonVertices[0].Z;
            BZ = mission.EnvRef.Balls[ball_cur[fish_cur]].PositionMm.Z;
            if (Judge_yaright_ball(mission, ref decisions, teamId) == 10)
                Judge_yaright_ball(mission, ref decisions, teamId);
            if (Judge_yaright_ball(mission, ref decisions, teamId) != 10)
            {
                ball_cur[fish_cur] = Judge_yaright_ball(mission, ref decisions, teamId);
                if (BZ > FZP)
                {
                    Ya_BallRightdown1(mission, ref decisions, teamId, fish_cur);
                }
                else
                {

                    Ya_BallRightup1(mission, ref decisions, teamId, fish_cur);
                }
            }
            else
                pull_inright(mission, ref decisions, teamId);

        }

        #endregion

        #endregion



        #endregion


        #region  全局函数
        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }
            int matchPeriod = Convert.ToInt32(mission.HtMissionVariables["CompetitionPeriod"]);


            #region 决策计算过程 需要各参赛队伍实现的部分
            #region 策略编写帮助信息
            //====================我是华丽的分割线====================//
            //======================策略编写指南======================//
            //1.策略编写工作直接目标是给当前队伍决策数组decisions各元素填充决策值
            //2.决策数据类型包括两个int成员，VCode为速度档位值，TCode为转弯档位值
            //3.VCode取值范围0-14共15个整数值，每个整数对应一个速度值，速度值整体但非严格递增
            //有个别档位值对应的速度值低于比它小的档位值对应的速度值，速度值数据来源于实验
            //4.TCode取值范围0-14共15个整数值，每个整数对应一个角速度值
            //整数7对应直游，角速度值为0，整数6-0，8-14分别对应左转和右转，偏离7越远，角度速度值越大
            //5.任意两个速度/转弯档位之间切换，都需要若干个仿真周期，才能达到稳态速度/角速度值
            //目前运动学计算过程决定稳态速度/角速度值接近但小于目标档位对应的速度/角速度值
            //6.决策类Strategy的实例在加载完毕后一直存在于内存中，可以自定义私有成员变量保存必要信息
            //但需要注意的是，保存的信息在中途更换策略时将会丢失
            //====================我是华丽的分割线====================//
            //=======策略中可以使用的比赛环境信息和过程信息说明=======//
            //场地坐标系: 以毫米为单位，矩形场地中心为原点，向右为正X，向下为正Z
            //            负X轴顺时针转回负X轴角度范围为(-PI,PI)的坐标系，也称为世界坐标系
            //mission.CommonPara: 当前仿真使命公共参数
            //mission.CommonPara.FishCntPerTeam: 每支队伍仿真机器鱼数量
            //mission.CommonPara.MsPerCycle: 仿真周期毫秒数
            //mission.CommonPara.RemainingCycles: 当前剩余仿真周期数
            //mission.CommonPara.TeamCount: 当前仿真使命参与队伍数量
            //mission.CommonPara.TotalSeconds: 当前仿真使命运行时间秒数
            //mission.EnvRef.Balls: 
            //当前仿真使命涉及到的仿真水球列表，列表元素的成员意义参见URWPGSim2D.Common.Ball类定义中的注释
            //mission.EnvRef.FieldInfo: 
            //当前仿真使命涉及到的仿真场地，各成员意义参见URWPGSim2D.Common.Field类定义中的注释
            //mission.EnvRef.ObstaclesRect: 
            //当前仿真使命涉及到的方形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RectangularObstacle类定义中的注释
            //mission.EnvRef.ObstaclesRound:
            //当前仿真使命涉及到的圆形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RoundedObstacle类定义中的注释
            //mission.TeamsRef[teamId]:
            //决策类当前对象对应的仿真使命参与队伍（当前队伍）
            //mission.TeamsRef[teamId].Para:
            //当前队伍公共参数，各成员意义参见URWPGSim2D.Common.TeamCommonPara类定义中的注释
            //mission.TeamsRef[teamId].Fishes:
            //当前队伍仿真机器鱼列表，列表元素的成员意义参见URWPGSim2D.Common.RoboFish类定义中的注释
            //mission.TeamsRef[teamId].Fishes[i].PositionMm和PolygonVertices[0],BodyDirectionRad,VelocityMmPs,
            //                                   AngularVelocityRadPs,Tactic:
            //当前队伍第i条仿真机器鱼鱼体矩形中心和鱼头顶点在场地坐标系中的位置（用到X坐标和Z坐标），鱼体方向，速度值，
            //                                   角速度值，决策值
            //====================我是华丽的分割线====================//
            //========================典型循环========================//
            //for (int i = 0; i < mission.CommonPara.FishCntPerTeam; i++)
            //{
            //  decisions[i].VCode = 0; // 静止
            //  decisions[i].TCode = 7; // 直游
            //}
            //====================我是华丽的分割线====================//
            #endregion
            //请从这里开始编写代码  
            Updata(mission, teamId);   //开始，判断鱼所在半场














            #region  //非决赛局
            
                if (Remain_time(mission) > 9.8)
                {
                    if (flag_left)
                        OpeningStageLeft(mission, ref decisions, teamId);
                    else
                        OpeningStageright(mission, ref decisions, teamId);
                }
                else if (Remain_time(mission) < 9.8 && Remain_time(mission) >= 7)
                {
                    #region//左场
                    if (flag_left)
                    {

                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            pull_inleftdown(mission, ref decisions, teamId, 3);
                        }
                        #endregion

                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            pull_inleftup(mission, ref decisions, teamId, 3);

                        }
                        #endregion

                    }
                    #endregion
                    #region//右场
                    else
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            pull_inRightdown(mission, ref decisions, teamId, 3);

                        }
                        #endregion

                        #region//二鱼策略
                        fish_cur = 1;
                        {

                            pull_inRightup(mission, ref decisions, teamId, 3);
                        }
                        #endregion

                    }
                    #endregion

                }
                    
                else if (Remain_time(mission) < 7 && Remain_time(mission) >= 5)
                {
                    #region//左场
                    if (flag_left)
                    {

                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            pull_inleftdown(mission, ref decisions, teamId, 0);
                        }
                        #endregion

                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            pull_inleftup(mission, ref decisions, teamId, 0);
                        }
                        #endregion

                    }
                    #endregion
                    #region//右场
                    else
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            pull_inRightdown(mission, ref decisions, teamId, 0);

                        }
                        #endregion

                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            pull_inRightup(mission, ref decisions, teamId, 0);
                        }
                        #endregion

                    }
                    #endregion

                }
                      
                else if (Remain_time(mission) < 5 && Remain_time(mission) >= 2)
                {
                    #region//左场
                    if (flag_left)
                    {

                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            pull_inleftdown(mission, ref decisions, teamId,0);
                        }
                        #endregion

                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            pull_inleftup(mission, ref decisions, teamId, 0);
                        }
                        #endregion

                    }
                    #endregion
                    #region//右场
                    else
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            pull_inRightdown(mission, ref decisions, teamId, 0);

                        }
                        #endregion

                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            pull_inRightup(mission, ref decisions, teamId, 0);
                        }
                        #endregion

                    }
                    #endregion

                }
                else if (Remain_time(mission) < 2 && Remain_time(mission) >= 0.5)//下半场继续进球
                {
                    #region//左场
                    if (flag_left)
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            //if (JudgeLeftdown_howmanyball(mission, ref decisions, teamId) >= JudgeLeftup_howmanyball(mission, ref decisions, teamId))
                                pull_inleftdown(mission, ref decisions, teamId, 0);
                            //else
                            //    pull_inleftup(mission, ref decisions, teamId, 1);

                        }
                        #endregion
                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            //hlright(mission, ref decisions, fish_cur, teamId);
                            pull_inleftup(mission, ref decisions, teamId, 0);
                        }
                        #endregion
                    }
                    #endregion
                    #region//右场
                    else
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            //if (JudgeRightdown_howmanyball(mission, ref decisions, teamId) >= JudgeRightup_howmanyball(mission, ref decisions, teamId))
                                pull_inRightdown(mission, ref decisions, teamId, 0);
                            //else
                            //    pull_inRightup(mission, ref decisions, teamId, 1);


                        }
                        #endregion
                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            //hlleft(mission, ref decisions, fish_cur, teamId);
                            pull_inRightup(mission, ref decisions, teamId, 0);
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (Remain_time(mission) < 0.5 && Remain_time(mission) >= 0)
                {
                    #region//左场
                    if (flag_left)
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            defanse_left(mission, ref decisions, teamId);
                        }
                        #endregion
                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            Tao_BallRight(mission, ref decisions, teamId, fish_cur);
                        }
                        #endregion
                    }
                    #endregion
                    #region//右场
                    else
                    {
                        #region//一鱼策略
                        fish_cur = 0;
                        {
                            defanse_right(mission, ref decisions, teamId);
                        }
                        #endregion
                        #region//二鱼策略
                        fish_cur = 1;
                        {
                            Tao_BallLeft(mission, ref decisions, teamId, fish_cur);
                        }
                        #endregion
                    }
                    #endregion
                }
            
            #endregion




            #endregion


            return decisions;
        }

        #endregion


    }

}
