namespace GLTech.Input
{
    internal interface IKeyboard
    {
        public event Action<ScanCode> KeyDown;
        public event Action<ScanCode> KeyUp;
    }
}
