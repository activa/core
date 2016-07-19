using System;

namespace Velox.Core
{
    [Flags]
    public enum DateConversionMethod
    {
        DoubleIsUnix = (1<<0),
        DoubleIsJulian = (1<<1),
        DoubleIsTicks = (1<<2),

        LongIsUnix = (1<<8),
        LongIsTicks = (1<<9),
    }
}