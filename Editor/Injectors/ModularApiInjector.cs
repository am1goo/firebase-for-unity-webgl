using System.Linq;
using System.Text;

namespace FirebaseWebGL.Editor
{
    internal class ModularApiInjector
    {
        public readonly string rootName;
        public readonly string sdkName;
        public readonly string apiName;
        private readonly string sourcePath;
        private readonly string[] injectedMethods;
        private readonly OnSdkInjector sdkInjector;
        private readonly string postfix;

        internal delegate string OnSdkInjector(string postfix);

        internal ModularApiInjector(string rootName, string sdkName, string apiName, string sourcePath, string[] injectedMethods, OnSdkInjector sdkInjector, bool usePostfix)
        {
            this.rootName = rootName;
            this.sdkName = sdkName;
            this.apiName = apiName;
            this.sourcePath = sourcePath;
            this.injectedMethods = injectedMethods;
            this.sdkInjector = sdkInjector;
            this.postfix = usePostfix ? $"_{sdkName}" : "";
        }

        public StringBuilder InjectImport(StringBuilder sb)
        {
            var injectedMethods = string.Join(", ", this.injectedMethods.Select(x => $"{x} as {x}{postfix}"));
            return sb.AppendLine($"import {{ {injectedMethods} }} from \"{sourcePath}\";");
        }

        public StringBuilder InjectSdk(StringBuilder sb)
        {
            return sb.AppendLine($"{rootName}.{sdkName} = {sdkInjector(postfix)};");
        }

        public StringBuilder InjectApi(StringBuilder sb)
        {
            var injectedMethods = string.Join(", ", this.injectedMethods.Select(x => $"{x}: {x}{postfix}"));
            return sb.AppendLine($"{rootName}.{apiName} = {{ {injectedMethods} }};");
        }
    }
}
