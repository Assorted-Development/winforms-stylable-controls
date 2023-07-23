namespace AssortedDevelopment.StylableWinFormsControls
{
    /// <summary>
    /// Win32 Native Window SubClass
    /// </summary>
    /// <remarks>Read more under https://www.codeproject.com/Articles/3234/Subclassing-in-NET-The-pure-NET-way</remarks>
    internal class NativeSubClass : NativeWindow
    {
        public delegate int SubClassWndProcEventHandler(ref Message m);
        public event SubClassWndProcEventHandler SubClassedWndProc;

        public NativeSubClass(IntPtr Handle, bool _subClass)
        {
            AssignHandle(Handle);
            SubClassed = _subClass;
        }

        public bool SubClassed { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (SubClassed)
            {
                if (OnSubClassedWndProc(ref m) != 0)
                    return;
            }
            base.WndProc(ref m);
        }

        private int OnSubClassedWndProc(ref Message m)
        {
            if (SubClassedWndProc != null)
            {
                return SubClassedWndProc(ref m);
            }

            return 0;
        }
    }
}
