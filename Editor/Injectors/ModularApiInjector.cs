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
        private readonly string[] injectedExports;
        private readonly OnSdkInjector sdkInjector;
        private readonly string postfix;

        internal delegate string OnSdkInjector(string postfix);

        internal ModularApiInjector(string rootName, string sdkName, string apiName, string postfix, string sourcePath, string[] injectedExports, OnSdkInjector sdkInjector)
        {
            this.rootName = rootName;
            this.sdkName = sdkName;
            this.apiName = apiName;
            this.sourcePath = sourcePath;
            this.injectedExports = injectedExports;
            this.sdkInjector = sdkInjector;
            this.postfix = postfix != null ? $"_{postfix}" : "";
        }

        public bool importSupported
        {
            get => !string.IsNullOrWhiteSpace(sourcePath);
        }

        public StringBuilder InjectImport(StringBuilder sb)
        {
            var injectedMethods = string.Join(", ", this.injectedExports.Select(x => IsMethod(x) ? $"{x} as {x}{postfix}" : x));
            return sb.AppendLine($"import {{ {injectedMethods} }} from \"{sourcePath}\";");
        }

        public bool sdkSupported => !string.IsNullOrWhiteSpace(sdkName) && sdkInjector != null;

        public StringBuilder InjectSdk(StringBuilder sb)
        {
            return sb.AppendLine($"{rootName}.{sdkName} = {sdkInjector(postfix)};");
        }

        public bool apiSupported => !string.IsNullOrWhiteSpace(apiName);

        public StringBuilder InjectApi(StringBuilder sb)
        {
            var injectedMethods = string.Join(", ", this.injectedExports.Select(x => IsMethod(x) ? $"{x}: {x}{postfix}" : x));
            return sb.AppendLine($"{rootName}.{apiName} = {{ {injectedMethods} }};");
        }

        private static bool IsMethod(string name)
        {
            if (name.Length == 0)
                return false;

            return char.IsLower(name[0]);
        }
    }
}
