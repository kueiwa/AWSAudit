using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace AWSResponderConsole
{
    class ReflectionHelper
    {
        public static string GetMyFunctionName()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            // Displays “WhatsmyName”
            return methodBase.Name;
        }
    }
}
