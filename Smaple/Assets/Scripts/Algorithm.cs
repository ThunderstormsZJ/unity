using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithm.Common
{
    public class CommonUtils
    {
        public static long TimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数

            return timeStamp;
        }
    }
}

namespace Algorithm.Bezier
{
    public class BezierUtils{

        // 通用贝塞尔曲线算法
        // 德卡斯特里奥算法
        public static Vector3 DrawNJBezierCurve(Vector3[] pn, float t)
        {
            Vector3 result = Vector3.zero;
            int count = pn.Length;
            if (count == 0) return result;

            Vector3[] pnTemp = new Vector3[count + 1]; // 保存新的计算点
            for (int i = 0; i < count; i++)
            {
                for(int j = 0; j < count - i; j++)
                {
                    if (i == 1) // i==1时,第一次迭代,由已知控制点计算
                    {
                        pnTemp[j] = (1 - t) * pn[j] + t * pn[j + 1];
                        continue;
                    }

                    // i != 1时,通过上一次迭代的结果计算
                    pnTemp[j] = (1 - t) * pnTemp[j] + t * pnTemp[j + 1];
                }
            }

            result = pnTemp[0];
            
            return result;
        }
        

        // 二阶阶贝塞尔曲线
        public static Vector3 Draw3JBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Mathf.Pow(1 - t, 2) * p0 + 2 * t * (1 - t) * p1 + Mathf.Pow(t, 2) * p2;
        }

        // 三阶贝塞尔曲线
        public static Vector3 Draw4JBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return Mathf.Pow(1 - t, 3) * p0 + 3 * p1 * t * Mathf.Pow(1 - t, 2) + 3 * p2 * Mathf.Pow(t, 2) * (1 - t) + p3 * Mathf.Pow(t, 3);
        }


        // 绘制 Catmull-Rom spline 曲线
        public static Vector3 DrawCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            // comments are no use here... it's the catmull-rom equation.
            // Un-magic this, lord vector!
            return 0.5f *
                   ((2 * p1) + (-p0 + p2) * t + (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
                    (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t);
        }

        // 绘制 Catmull-Rom spline 曲线 通过贝塞尔曲线
        public static Vector3 DrawCatmullRomByBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 bezierP0 = p1;
            Vector3 bezierP1 = p1 + (p2 - p0) / 6;
            Vector3 bezierP2 = p2 - (p3 - p1) / 6;
            Vector3 bezierP3 = p2;

            return Draw4JBezierCurve(bezierP0, bezierP1, bezierP2, bezierP3, t);
        }
    }
}

// 组合数算法
namespace Algorithm.Combination
{
    public class CombinationUtils
    {
        const int M = 10007; //结果可能非常大，对结果模10007即可

        // 方案一 ：暴力求解 C(n,m) = n*(n-1)...*(n-m+1)/m!
        // 这种求值在相乘过程中很快就会溢出，时间复杂度为O(n)
        public static int Combination1(int n, int m)
        {
            int ans = 1;
            for (int i = n; i >= (n - m + 1); i--)
            {
                ans *= i;
            }
            while (m>0)
            {
                ans /= m--;
            }

            return ans;

        }

        // 方案二: 根据C(n, m) = C(n-1, m)+C(n-1, m-1) 实现生成所有的表
        // 这种方法的时间复杂度为O(n^2) ，占用空间也较高，但是能生成的组合数值比方案一大
        public static int Combination2(int n, int m)
        {
            // 用一个二维数组来保存
            int[,] C = new int[n + 1, n + 1];
            // 初始化
            for (int i = 0; i <= n; i++)
            {
                C[0, i] = 0;
                C[i, 0] = 1;
            }

            for (int i = 1; i <= n; i++)
            {
                for(int j = 1; j <= n; j++)
                {
                    C[i, j] = C[i - 1, j] + C[i - 1, j - 1];
                }
            }

            return C[n, m];
        }

        // 方案三：是在方案二上的改进，可以将二维数组改成一维数据节省内存
        public static int Combination3(int n,int m)
        {
            // 可以发现每个值都是基于上一个维度的值，这时候我们可以舍弃之前无用的值
            int[] C = new int[n+1];
            for(int i = 0; i <= n; i++)
            {
                C[i] = 1;
                for (int j = i - 1; j >= 1; j--)
                {
                    C[j] += C[j - 1];
                }
                C[0] = 1;
            }
            return C[m];
        }

        // 方案四：分解质因数
        /// <summary>
        ///  质因数分解，C(n, m)=n!/(m!*(n-m)!)，设n!分解因式后，质因数p的次数为a；
        ///  对应地m!分解后p的次数为b；(n-m)!分解后p的次数为c；则C(n, m)分解后，p的次数为a-b-c。
        /// 
        ///  n!分解后p的指数为：n/p+n/p^2+…+n/p^k。(n/p^k>=0)
        ///  这种方法是由《算法竞赛入门经典》提出的。
        ///  举例：计算26!分解质因数之后5的指数。根据公式知道z=26/5+26/5^2。又因为26！=1*2*3*5*...*26,可以将26/5=5理解为每隔5都会有5的倍数(5,10,15,20,25)
        ///  但是25中包涵有两个5，所以26/5^2=1就将另一个5包涵其中，利用这种方法我们可以计算出所有的素数的指数。
        /// 
        ///  计算出所有质因子的次数，它们的积即为答案，即C(n, m)=p1^(a1-b1-c1)*p2^(a2-b2-c2)…pk^(ak-bk-ck)。
        ///  
        ///  算法的时间复杂度比前两种方案都低，基本上跟n以内的素数个数呈线性关系，而素数个数通常比n都小几个数量级，例如100万以内的素数不到8万个。
        ///  用筛法生成素数的时间接近线性。该方案1秒钟能计算 1kw数量级的组合数。如果要计算更大，内存和时间消耗都比较大。
        /// </summary>
        public static long Combination4(int n, int m)
        {
            List<int> prim = ProducePrimNumber(n);

            long ans = 1;

            for(int i = 0; i < prim.Count; i++)
            {
                int num = CalIndexByPrim(n, prim[i]) - CalIndexByPrim(m, prim[i]) - CalIndexByPrim(n - m, prim[i]);
                ans = ans * (long)Mathf.Pow(prim[i], num);
            }

            return ans;
        }

        //计算n!中素因子p的指数
        private static int CalIndexByPrim(int x, int p)
        {
            int ans = 0;

            long rec = p;

            while (x>=rec)
            {
                ans += (int)(x / rec); 
                rec *= p;
            }

            return ans;
        }

        // 筛选法生成MAX范围内的素数
        private static List<int> ProducePrimNumber(int max)
        {
            bool[] arr = new bool[max + 1];
            List<int> prim = new List<int>();
            prim.Add(2);

            int i, j;

            // 排除偶数
            for(i = 3; i * i <= max; i += 2)
            {
                if (!arr[i])
                {
                    prim.Add(i);
                    for (j = i * i; j <= max; j += i)
                    {
                        arr[j] = true;
                    }
                }
            }

            while(i<=max)
            {
                if (!arr[i])
                    prim.Add(i);
                i += 2;
            }

            return prim;
        }


        // ------------------------------------------方案四 END--------------------------------------------------------------

        public void test()
        {
            //Debug.Log(Combination1(15, 15) + "----方案1");
            //Debug.Log(Combination2(15, 15) + "----方案2");
            //Debug.Log(Combination3(15, 15) + "----方案3");
            //Debug.Log(Common.CommonUtils.TimeStamp());
            //Debug.Log(Combination4(20,16));
            //Debug.Log(Common.CommonUtils.TimeStamp());

            //int n = 25;
            //List<int> prim = ProducePrimNumber(n);
            //for (int i = 0; i < prim.Count;  i++)
            //{
            //    Debug.Log(CalIndexByPrim(n, prim[i]) + "----" + prim[i]);
            //}

            Debug.Log(Combination4(20, 16));
        }

        public void test2()
        {
            Debug.Log(Common.CommonUtils.TimeStamp());
            Debug.Log(Combination3(20, 16));
            Debug.Log(Common.CommonUtils.TimeStamp());
        }
    }
}
