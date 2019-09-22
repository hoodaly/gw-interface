#region

using System;

#endregion

namespace GuildWarsInterface.Debugging
{
        public static class Debug
        {
                public static Action<Exception> ThrowException = exception
                                                                 => { throw exception; };

                internal static void Requires(bool condition)
                {
                        if (!condition)
                        {
                                ThrowException(new Exception("precondition violated"));
                        }
                }

                public static void Log(string v)
                {
                        System.Diagnostics.Debug.Write(v + "\n");
                }

                public static void LogBytes(byte[] data, string prefix)
                {
                        if (data.Length <= 0)
                        {
                                return;
                        }
                        System.Diagnostics.Debug.Write(prefix + " " + BitConverter.ToString(data) + "\n");
                }
        }
}