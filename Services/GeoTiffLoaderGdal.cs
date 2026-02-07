using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OSGeo.GDAL;

namespace VectorLab.Services
{
    internal class GeoTiffLoaderGdal
    {
        public static BitmapSource LoadAsBitmapSource(string path)
        {
            using var ds = Gdal.Open(path, Access.GA_ReadOnly);
            if (ds == null) throw new InvalidOperationException("GDAL failed to open the file.");

            int width = ds.RasterXSize;
            int height = ds.RasterYSize;
            int bandCount = ds.RasterCount;

            // 1밴드면 Gray, 3밴드 이상이면 RGB(1,2,3) 사용
            if(bandCount == 1)
            {
                byte[] gray = ReadBandAsByte(ds.GetRasterBand(1), width, height);
                //Gray8
                var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                wb.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), gray, width, 0);
                wb.Freeze();
                return wb;
            }
            else
            {
                // RGB: band 1, 2, 3 (많은 GeoTIFF가 이 구성이지만, 다르면 나중에 계산)
                byte[] r = ReadBandAsByte(ds.GetRasterBand(1), width, height);
                byte[] g = ReadBandAsByte(ds.GetRasterBand(2), width, height);
                byte[] b = ReadBandAsByte(ds.GetRasterBand(3), width, height);

                // WFP PixelFormats.Bgr24: B, G, R 순서
                byte[] bgr = new byte[width * height * 3];
                for (int i = 0; i < width * height; i++)
                {
                    bgr[i * 3 + 0] = b[i];
                    bgr[i * 3 + 1] = g[i];
                    bgr[i * 3 + 2] = r[i];
                }

                var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
                wb.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), bgr, width * 3, 0);
                wb.Freeze();
                return wb;
            }
        }

        private static byte[] ReadBandAsByte(Band band, int width, int height)
        {
            // band 타입이 UInt16/Float32여도 일단 double로 읽어서 min/max 스트레치
            double[] buf = new double[width * height];
            band.ReadRaster(0, 0, width, height, buf, width, height, 0, 0);

            // NoData 처리(있으면 제외하는게 좋지만, 일단 기본
            double min = buf.Min();
            double max = buf.Max();

            // 정부 같은 값이면(올검정 방지) 그냥 0으로
            if(Math.Abs(max - min) < 1e-12) return new byte[width * height];

            byte[] out8 = new byte[width * height];
            double scale = 255.0 / (max - min);

            for (int i = 0; i < buf.Length; i++)
            {
                double v = (buf[i] - min) * scale;
                if (v < 0) v = 0;
                if (v > 255) v = 255;
                out8[i] = (byte)v;
            }

            return out8;
        }
    }
}
