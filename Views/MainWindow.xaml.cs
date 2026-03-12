using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VectorLab.Models;
using VectorLab.ViewModels;
using VectorLab.Services;

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

            // Image+Overlay 가 들어있는 Grid에 확대/축소 Transform 연결
            ImageContainer.LayoutTransform = _zoomTransform;

            DataContext = new MainViewModel();

            // Labels 컬렉션이 변경될 때 자동으로 감지하기 위해 연결
            // ViewModel의 PropertyChanged 감지
            if (DataContext is MainViewModel vm)
            {
                vm.Labels.CollectionChanged += Labels_CollectionChanged;
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        // 확대 축소 관리를 위한 ScaleTransform
        private readonly ScaleTransform _zoomTransform = new ScaleTransform(1.0, 1.0);

        private double _zoomScale = 1.0;

        // json 로드 중에는 Labels.Add/Remove가 발생하더라도
        // 다시 저장하지 않도록 막는 플래그 선언
        private bool _isLoadingLabels = false;

        private Point? _start;      // 드래그 시작점
        private Rectangle? _rubber; // 드래그 중 임시로 보이는 사각형(고무줄)

        private readonly Dictionary<LabelRect, Rectangle> _labelToShape = new();

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

            // 라벨 이름 입력창 표시
            var dialog = new LabelInputDialog
            {
                Owner = this
            };

            if(dialog.ShowDialog() != true)
            {
                return;
            }

            // 입력된 라벨 이름
            var labelName = dialog.LabelName;

            // 화면 도형을 "데이터 라벨"로 변환해서 VM에 저장
            var label = new LabelRect
            {
                X = x,
                Y = y,
                Width = w,
                Height = h,
                ClassName = labelName
            };

            if(DataContext is MainViewModel vm)
            {
                vm.Labels.Add(label);

                _labelToShape[label] = _rubber!;
            }

            // 너무 작은 드래그는 라벨로 저장 안 하고 싶으면 여기서 w/h 체크하면 됨

            _start = null;
            _rubber = null;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Delete 키 여부 확인
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                DeleteSelectedLabel();
            }
        }

        // 삭제 버튼
        private void DeleteSeletedButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedLabel();
        }

        // 라벨 삭제
        private void DeleteSelectedLabel()
        {
            // 현재 View가 가지고 있는 ViewModel 가져오기
            if (DataContext is MainViewModel vm)
            {
                // 선택된 라벨이 있을 때만 삭제
                if (vm.SelectedLabel != null)
                {
                    // System.Diagnostics.Debug.WriteLine("Delete 눌림");
                    // Labels 리스트에서 제거
                    vm.Labels.Remove(vm.SelectedLabel);

                    // 선택값 초기화
                    vm.SelectedLabel = null;
                }
            }
        }

        // Labels에서 Remove가 발생하면 자동으로 호출됨
        private void Labels_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // 삭제된 항목이 없으면 종료
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if(item is LabelRect label && _labelToShape.ContainsKey(label))
                    {
                        var rect = _labelToShape[label];

                        // 화면에서 사각형 제거
                        Overlay.Children.Remove(rect);

                        // 연결표에서도 제거
                        _labelToShape.Remove(label);
                    }
                }
            }


            // 로드 중이면 저장하지 않음
            if (_isLoadingLabels) return;

            // 라벨 컬렉션이 변경되면 json 저장
            if(DataContext is MainViewModel vm)
            {
                if (!string.IsNullOrWhiteSpace(vm.CurrentImagePath))
                {
                    LabelFileService.Save(vm.CurrentImagePath, vm.Labels);
                }
            }
        }

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // SelectedLabel이 변경된 경우에만 색을 다시 칠해준다.
            if(e.PropertyName == nameof(MainViewModel.SelectedLabel))
            {
                UpdateSelectionVisual();
            }

            // 현재 이미지 경로가 바뀌면(= 새GeoTIFF를 열었으면)
            if(e.PropertyName == nameof(MainViewModel.CurrentImagePath))
            {
                LoadLabelsForCurrentImage();
            }
        }

        private void UpdateSelectionVisual()
        {
            if (DataContext is not MainViewModel vm) return;

            // Label -> rect 매핑표를 돌면서 모두 색을 다시 지정
            foreach(var pair in _labelToShape)
            {
                var label    = pair.Key;     // 라벨 데이터
                var rect     = pair.Value;   // 화면에 그려진 사각형

                if (label == vm.SelectedLabel)
                    rect.Stroke = Brushes.Red;
                else
                    rect.Stroke = Brushes.Lime;
            }
        }

        // json 파일 읽어서 라벨 그리기
        private void DrawLabelRectangle(LabelRect label)
        {
            var rect = new Rectangle
            {
                Width = label.Width,
                Height = label.Height,
                Stroke = Brushes.Lime,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(rect, label.X);
            Canvas.SetTop(rect, label.Y);

            Overlay.Children.Add(rect);

            _labelToShape[label] = rect;
        }

        // 현재 이미지 경로를 기주으로 json 라벨 파일을 읽어와
        // ViewModel Labels와 화면 사각형을 복원
        private void LoadLabelsForCurrentImage()
        {
            if(DataContext is not MainViewModel vm) return; 

            if(string.IsNullOrWhiteSpace(vm.CurrentImagePath)) return;

            _isLoadingLabels = true;

            try
            {
                // 기존 화면 사각형 모두 제거
                Overlay.Children.Clear();
                _labelToShape.Clear();

                // 기존 vm 라벨 제거
                vm.Labels.Clear();

                // json에서 라벨 불러오기
                var loadedLabels = LabelFileService.Load(vm.CurrentImagePath);

                foreach( var label in loadedLabels)
                {
                    // vm 리스트에 넣기
                    vm.Labels.Add(label);

                    // 화면에 그리기
                    DrawLabelRectangle(label);
                }

                // 선택값 초기화
                vm.SelectedLabel = null;

                // 선택 시각 상태도 초기화
                UpdateSelectionVisual();
            }
            finally
            {
                _isLoadingLabels = false;
            }
        }

        private void ImageScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _zoomScale += 0.1;
            else if (e.Delta < 0)
                _zoomScale -= 0.1;


        }

    }
}