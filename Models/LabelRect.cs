using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorLab.Models
{
    // "사각형 라벨 1개"의 순수 데이터
    internal class LabelRect
    {
        // 좌표 이미지 픽셀 좌표
        public double X { get; set; } // Left
        public double Y { get; set; } // Top
        public double Width { get; set; }
        public double Height { get; set; }

        // 라벨 클래스(예: road, building ... )
        public string ClassName { get; set; } = "default";
    }
}
