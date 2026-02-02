using System;
using System.CodeDom.Compiler;
using System.Windows.Input;

namespace VectorLab.Infrastructure
{
    internal class RelayCommand : ICommand
    {
        /*
        실행 로직 (Execute)
        실행 가능 여부 확인 로직 (CanExecute)
        Action: "매개변수 없고 반환값 없는 함수"를 담는 타입

        왜 필드로 들고 있냐?
        RelayCommand는 결국 버튼이 눌렀을 때 실행할 코드를 저장해뒀다가 실행
        그래서:
         - 생성자엣서 받아서 _execute에 저장해두고
         - Execute()가 호출되면 _execute() 실행

        readonly는 왜?
         - Command를 만든 다음에는 "버튼 눌러을 때 실행할 함수"가 바뀌면 혼란스러움.
         - 그래서 생성 시 결정한 실행 로직은 고정시키는 게 안전해.
         - readonly는 "생성자에서 한 번만 할당하고 이후 변경 불가" 의미
         */
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /*
        "CanExecute값이 바뀔 수 있으니, 다시 물어봐" 라고 WPF에게 알리는 신호
        WPF는 버튼을 그릴 때 CanExecute를 보고 Enabled를 정함
        근데 시간이 지나서 조건이 바뀌면 WPF는 그걸 자동으로 모름
        
        예)
         - 이미지 열기 전 : CanSave=flase -> 저장 버튼 비활성
         - 이미지 열고 나서 : CanSave=true -> 저장 버튼 활성화
        그럼 "다시 평가해" 하는게 CanExecuteChanged 이벤트
         */
        public event EventHandler CanExecuteChanged;

        /*
        Func<bool> : "매개변수 없고 bool을 반환하는 함수"를 담는 타입
        버튼을 항상 눌러도 되는게 아니기 때문에
         - 이미지가 없으면 "저장" 버튼은 비활성화
         - 폴리곤 점이 3개 미만이면 "완료" 버튼 비활성화
        이 조건을 CanExecute()로 제공하면 WPF가 알아서 버튼 Enable/Disable 해줌

        readonly 이유는 위와 동일
         - "활성 조건 계산 함수"도 커맨드 생성 이후 바뀌면 구조가 복잡해져서 고정
        
        왜 execute는 필수고 canExecute는 선택이냐?
         - execute 없으면 커맨드가 실행할 게 없으니 의미가 없음 -> 필수
         - canExecute는 조건 없는 버튼도 많음 ("항상 가능") -> 선택
        그래서 CanExecute = null로 기본값을 주고, null 이면 "항상 가능"으로 처리하는 패턴을 사용
         */
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // public bool CanExecute(object parameter) => _canExecute == null || _canExecute(); 이렇게도 쓸 수 있다~
        public bool CanExecute(object parameter)
        {
            // _canExecute가 null이면 조건이 없다 -> 항상 true / 조건이 있으면 _canExecute() 실행 결과를 반환
            // 즉, canExecute 안줬으면 버튼은 항상 활성 / canExecute 줬으면 그 결과로 활성 or 비활성
            return _canExecute == null || _canExecute(); 
        }
        // public void Execute(object parameter) => _execute(); 이렇게도 쓸 수 있다~
        public void Execute(object parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            /*
            "WPF야, 지금부터 CanExecute 다시 평가해줘"
            예를 들어 ImagePath가 바뀌었거나, CurrentPoints 개수가 바뀌었을 때
            Finish 버튼, Save 버튼 활성 / 비활성 조건이 바뀌잖아?
            그때 ViewModel에서 SaveCommand.RaiseCanExecuteChanged(); 호출해주면 WPF가 다ㅣㅅ CanExecute를 호출해서 UI가 업데이트 됨.
             */
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
