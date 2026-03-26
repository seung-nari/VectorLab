using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace VectorLab.Models
{
    // "사각형 라벨 1개"의 순수 데이터
    internal class LabelRect : INotifyPropertyChanged
    {
        // UI가 속성 변경을 감지할 수 있도록 이벤트 제공
        public event PropertyChangedEventHandler? PropertyChanged;

        // 공통 속성 변경 알림 함수
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 좌표 이미지 픽셀 좌표
        private double _x;
        public double X 
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            } 
        } // Left

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        } // Top

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged();
                }
            }
        }

        // 라벨 클래스(예: road, building ... )
        private string _className = "default";
        public string ClassName
        {
            get => _className;
            set
            {
                if (_className != value)
                {
                    _className = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
