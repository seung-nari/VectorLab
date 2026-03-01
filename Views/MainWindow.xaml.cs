using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VectorLab.ViewModels;
using VectorLab.Models;

namespace VectorLab.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private Point? _start;      // 드래그 시작점
        private Rectangle? _rubber; // 드래그 중 임시로 보이는 사각형(고무줄)

        private void Overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 1) 시작점 저장
            _start = e.GetPosition(Overlay);

            // 2) 임시 사각형 생성
            _rubber = new Rectangle
            {
                Stroke = Brushes.Lime,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            // 3) 사각형의 시작 위치 지정
            Canvas.SetLeft(_rubber, _start.Value.X);
            Canvas.SetTop(_rubber, _start.Value.Y);

            // 4) Canvas에 올림
            Overlay.Children.Add(_rubber);

            // 5) 드래그 중 이벤트 끊기지 않게 마우스 캡쳐
            Overlay.CaptureMouse();
        }

        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (_start == null || _rubber == null) return;
            if (!Overlay.IsMouseCaptured) return;

            var pos = e.GetPosition(Overlay);

            // 드래그 방향이 어느 방향이든 정상 사각형 되게 처리
            double x = Math.Min(pos.X, _start.Value.X);
            double y = Math.Min(pos.Y, _start.Value.Y);
            double w = Math.Abs(pos.X - _start.Value.X);
            double h = Math.Abs(pos.Y - _start.Value.Y);

            Canvas.SetLeft(_rubber, x);
            Canvas.SetTop(_rubber, y);
            _rubber.Width = w;
            _rubber.Height = h;
        }

        private void Overlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(_start == null || _rubber == null) return;

            Overlay.ReleaseMouseCapture();

            // 임시 사각형의 좌표/크기 읽기
            double x = Canvas.GetLeft(_rubber);
            double y = Canvas.GetTop(_rubber);
            double w = _rubber.Width;
            double h = _rubber.Height;

            // 화면 도형을 "데이터 라벨"로 변환해서 VM에 저장
            if(DataContext is MainViewModel vm)
            {
                vm.Labels.Add(new LabelRect
                {
                    X = x,
                    Y = y,
                    Width = w,
                    Height = h,
                    ClassName = "default"
                });
            }

            // 너무 작은 드래그는 라벨로 저장 안 하고 싶으면 여기서 w/h 체크하면 됨

            _start = null;
            _rubber = null;
        }
    }
}