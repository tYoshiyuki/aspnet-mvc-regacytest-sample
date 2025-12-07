using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace AspNetMvcRegacyTestSample.Tests.Helpers
{
    /// <summary>
    /// 属性ルーティングの情報を保持・検索するためのレジストリクラス
    /// </summary>
    public class AttributeRouteRegistry
    {
        private const string KeySeparator = "|"; // URLとHTTPメソッドを結合するための区切り文字

        /// <summary>
        /// キー: {URL | HTTPメソッド} 、値: RouteDefinition のディクショナリ
        /// </summary>
        private static Dictionary<string, RouteDefinition> routeMap;

        /// <summary>
        /// アセンブリ内の全コントローラを走査し、[Route] 属性情報を辞書化します。
        /// </summary>
        /// <param name="assembly">走査対象のアセンブリ</param>
        public static void Initialize(Assembly assembly)
        {
            routeMap = new Dictionary<string, RouteDefinition>(StringComparer.Ordinal);

            var controllers = assembly.GetTypes()
                .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var controller in controllers)
            {
                var prefixAttr = controller.GetCustomAttribute<RoutePrefixAttribute>();
                var prefix = prefixAttr != null ? prefixAttr.Prefix : string.Empty;

                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                    if (routeAttr == null)
                    {
                        continue;
                    }

                    // URLの結合
                    // NOTE: "~/" や "/" で始まる場合は絶対パス扱いとし、Prefix を無視する
                    string combinedUrl;
                    if (routeAttr.Template.StartsWith("~") || routeAttr.Template.StartsWith("/"))
                    {
                        combinedUrl = routeAttr.Template.TrimStart('~', '/');
                    }
                    else
                    {
                        combinedUrl = string.IsNullOrEmpty(prefix)
                            ? routeAttr.Template
                            : $"{prefix}/{routeAttr.Template}";
                    }

                    // HTTPメソッドの判定
                    var httpMethod = "GET";
                    if (method.GetCustomAttribute<HttpPostAttribute>() != null)
                    {
                        httpMethod = "POST";
                    }
                    else if (method.GetCustomAttribute<HttpPutAttribute>() != null)
                    {
                        httpMethod = "PUT";
                    }
                    else if (method.GetCustomAttribute<HttpDeleteAttribute>() != null)
                    {
                        httpMethod = "DELETE";
                    }

                    // URLとHTTPメソッドを結合して単一の文字列キーを作成
                    var key = $"{combinedUrl}{KeySeparator}{httpMethod}";
                    var definition = new RouteDefinition
                    {
                        Controller = controller.Name.Replace("Controller", ""),
                        Action = method.Name
                    };

                    if (!routeMap.ContainsKey(key))
                    {
                        routeMap.Add(key, definition);
                    }
                }
            }
        }

        /// <summary>
        /// URLとHTTPメソッドに対応する定義を取得します。
        /// </summary>
        /// <param name="url">URL文字列</param>
        /// <param name="httpMethod">HTTPメソッド</param>
        public static RouteDefinition GetDefinition(string url, string httpMethod = "GET")
        {
            var normalizedUrl = url.TrimStart('~', '/');
            var key = $"{normalizedUrl}{KeySeparator}{httpMethod}";
            if (routeMap != null && routeMap.TryGetValue(key, out var definition))
            {
                return definition;
            }
            return null;
        }

        /// <summary>
        /// ルート属性の定義情報を保持するクラス
        /// </summary>
        public class RouteDefinition
        {
            /// <summary>
            /// コントローラ名
            /// </summary>
            public string Controller { get; set; }

            /// <summary>
            /// アクション名
            /// </summary>
            public string Action { get; set; }
        }
    }
}
