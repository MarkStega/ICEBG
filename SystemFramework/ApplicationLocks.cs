using System;

namespace ICEBG.SystemFramework;

public static class ApplicationLocks
{
    public static Object EventStoreItemLogic = new Object();
    public static Object LogSequencer = new Object();
    public static Object ProcessLogic = new Object();
    public static Object PSS_ElementalOperationSequencer = new Object();
}


