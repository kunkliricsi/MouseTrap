using System;
using System.Runtime.InteropServices;

namespace MouseTrap
{
    [Flags]
    public enum MouseEvents
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010
    }

    internal struct InternalPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public record Point(int X, int Y)
    {
        internal Point(InternalPoint point) : this(point.X, point.Y) { }
    }

    public class MouseControllerService
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out InternalPoint lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public Point GetCursorPosition()
        {
            var gotPoint = GetCursorPos(out var currentMousePoint);
            return gotPoint
                ? new Point(currentMousePoint)
                : new Point(0, 0);
        }

        public bool GetCursorPosition(out Point point)
        {
            var result = GetCursorPos(out var currentMousePoint);
            point = new Point(currentMousePoint);
            return result;
        }

        public void SetCursorPosition(Point point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public void MouseEvent(MouseEvents value)
        {
            var position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }
    }
}
