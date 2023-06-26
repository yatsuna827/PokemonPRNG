
namespace Test
{
    public class Util
    {
        public static bool IsDebug =>
#if DEBUG
            true;
#else
   false;
#endif
    }
}
