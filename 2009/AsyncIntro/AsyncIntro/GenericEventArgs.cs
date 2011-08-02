using System;

namespace AsyncIntro
{
    public class GenericEventArgs<TValue> : EventArgs
    {
        public GenericEventArgs(TValue args)
        {
            this.EventArgs = args;
        }

        public TValue EventArgs { get; internal set; }
    }
}