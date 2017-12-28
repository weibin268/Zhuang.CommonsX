using System;

namespace Zhuang.Commons.Utils
{
    public static class GeographicUtil
    {
        /// <summary>
        /// 高斯投影中所选用的参考椭球
        /// </summary>
        public enum GaussSphere
        {
            Beijing54,
            Xian80,
            WGS84,
        }
        
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 计算两个坐标距离
        /// </summary>
        /// <param name="lng1"></param>
        /// <param name="lat1"></param>
        /// <param name="lng2"></param>
        /// <param name="lat2"></param>
        /// <param name="gs"></param>
        /// <returns></returns>
        public static double GetDistancesByTwoPoints(double lng1, double lat1, double lng2, double lat2, GaussSphere gs)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * (gs == GaussSphere.WGS84 ? 6378137.0 : (gs == GaussSphere.Xian80 ? 6378140.0 : 6378245.0));
            s = Math.Round(s * 10000) / 10000;
            return s;
        }


        /// <summary>
        /// 根据Gps获取的经纬度坐标点计算出路线距离
        /// </summary>
        /// <param name="gpsData">数据格式如：113.41997,23.178216|113.419975,23.178218|113.420276,23.178581</param>
        /// <returns>返回单位米</returns>
        public static double GetDistancesByGpsData(string gpsData, double maxTwoPointsDistances = 0)
        {

            var arrCoordinates = gpsData.Split('|');

            double totalDistance = 0;

            for (int i = 0; i < arrCoordinates.Length; i++)
            {
                if (i == 0) continue;
                var aCoordinate = arrCoordinates[i - 1];
                var bCoordinate = arrCoordinates[i];

                var arrPointsA = aCoordinate.Split(',');
                if (arrPointsA.Length < 2) continue;

                double pa1 = 0;
                double pa2 = 0;

                bool parseSucessA1 = double.TryParse(arrPointsA[0], out pa1);
                bool parseSucessA2 = double.TryParse(arrPointsA[1], out pa2);

                var arrPointsB = bCoordinate.Split(',');
                if (arrPointsB.Length < 2) continue;

                double pb1 = 0;
                double pb2 = 0;
                bool parseSucessB1 = double.TryParse(arrPointsB[0], out pb1);
                bool parseSucessB2 = double.TryParse(arrPointsB[1], out pb2);

                if (parseSucessA1 && parseSucessA2 && parseSucessB1 && parseSucessB2)
                {
                    double tempTwoPointsDistances = GetDistancesByTwoPoints(pa1, pa2, pb1, pb2, GaussSphere.Beijing54);

                    if (maxTwoPointsDistances > 0 && tempTwoPointsDistances > maxTwoPointsDistances)
                    {
                        tempTwoPointsDistances = 0;
                    }

                    totalDistance = totalDistance + tempTwoPointsDistances;
                }
            }

            return totalDistance;
        }
    }
}
