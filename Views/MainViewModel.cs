using VectorLab.Infrastructure;

namespace VectorLab.Views
{
    internal class MainViewModel : ViewModelBase
    {
        private string _title = "VectorLab";
        public string Title
        {
            get => _title;
            /*
            SetProperty가 없으면
            set 
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
            이게 프로퍼티 하나당 꼭 들어가야해...

            SetProperty를 사용함으로
             - setter는 "이 필드 바꿔줘"만 말하고
             - ViewModel이 훨ㅆ신 읽기 쉬워짐

            SetProperty 내부는
            protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null) 
            {
                if (Equals(field, value)) return false; => 값이 안 바뀌었으면 아무것도 하지 마라
                field = value; => 실제 backing field 값 변경
                OnPropertyChanged(name); => UI한테 "이 프로퍼티 값 바뀌었어"라고 말함
                return true;
            }
            이렇게 되어있어

            [CallerMemberName] string name = null "Title" 같은 문자열을 자동으로 채워주는 장치
                set => SetProperty(ref _title, value);
            이렇게 호출하면 컴파일러가 알아서 : 
                OnPropertyChanged("Title");
                OnPropertyChanged(nameof(Title)); => [CallerMemberName] 없을때 사용하던 안전한 방식
            
            SetProperty(ref _title, value); => SetProperty(ref _title, value, "Title"); 이렇게 바꿔주는거임~
             */
            set => SetProperty(ref _title, value);
        }
    }
}
