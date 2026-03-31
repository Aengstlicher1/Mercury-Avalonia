using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Mercury.Controls
{
    public partial class TitleBar : UserControl
    {
        private bool _isPressed = false;
        private Window? _window;
        
        public TitleBar()
        {
            this.InitializeComponent();

            _ = GetWindow();
        }

        private async Task GetWindow()
        {
            while (_window == null)
            {
                _window = this.FindAncestorOfType<Window>();
                await Task.Delay(50);
            }
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e) => _isPressed = true;
        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) =>  _isPressed = false;
        
        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isPressed &&  _window != null)
            {
                var mPos = e.GetCurrentPoint(_window).Position;
                var winPos = _window.Position.ToPoint(1);
                Debug.WriteLine(mPos);
                Debug.WriteLine(winPos);
                _window.Position = new PixelPoint((int)(winPos.X - mPos.X), (int)(winPos.Y - mPos.Y));
            }
        }

    }
}
