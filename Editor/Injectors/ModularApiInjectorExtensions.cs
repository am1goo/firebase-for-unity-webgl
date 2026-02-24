using System.Text;

namespace FirebaseWebGL.Editor
{
    internal static class ModularApiInjectorExtensions
    {
        internal static StringBuilder InjectImport(this StringBuilder sb, ModularApiInjector injector)
        {
            return injector.InjectImport(sb);
        }

        internal static StringBuilder InjectSdk(this StringBuilder sb, ModularApiInjector injector)
        {
            return injector.InjectSdk(sb);
        }

        internal static StringBuilder InjectApi(this StringBuilder sb, ModularApiInjector injector)
        {
            return injector.InjectApi(sb);
        }
    }
}
