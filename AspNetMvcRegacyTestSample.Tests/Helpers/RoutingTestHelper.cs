using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetMvcRegacyTestSample.Web;

namespace AspNetMvcRegacyTestSample.Tests.Helpers
{
    /// <summary>
    /// ルーティングテスト用のヘルパークラス
    /// </summary>
    public static class RoutingTestHelper
    {
        private static RouteCollection routes;
        private static Assembly controllerAssembly;
        private static string[] controllerNamespaces;

        /// <summary>
        /// ルートコレクションを初期化します
        /// </summary>
        public static void InitializeRoutes()
        {
            routes = new RouteCollection();
            
            // RouteConfigからアセンブリを取得
            if (controllerAssembly == null)
            {
                controllerAssembly = typeof(RouteConfig).Assembly;
            }
            
            // 通常のルーティングを登録
            // 注意: 属性ルーティングはユニットテスト環境では登録できないため、
            // 属性を直接チェックするAssertAttributeRouteExistsメソッドを使用してください
            RouteConfig.RegisterRoutes(routes);
        }

        /// <summary>
        /// 指定されたURLからルートデータを取得します
        /// </summary>
        /// <param name="url">テスト対象のURL</param>
        /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）。省略時は"GET"</param>
        /// <returns>ルートデータ。見つからない場合はnull</returns>
        public static RouteData GetRouteData(string url, string httpMethod = "GET")
        {
            if (routes == null)
            {
                InitializeRoutes();
            }

            // URLからルートデータを取得
            var httpContext = CreateHttpContext(url, httpMethod);
            return routes.GetRouteData(httpContext);
        }

        /// <summary>
        /// 指定されたURLがルーティングされ、対応するアクションが存在することを検証します（URLから自動推測）
        /// </summary>
        /// <param name="url">テスト対象のURL</param>
        /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）。省略時は"GET"</param>
        public static void AssertRouteAndActionExists(string url, string httpMethod = "GET")
        {
            // ルートデータを取得
            var routeData = GetRouteData(url, httpMethod);

            // ルートデータが存在することを確認
            Assert.IsNotNull(routeData, $"URL '{url}' (HTTP {httpMethod}) に対応するルートが見つかりませんでした");

            // コントローラー名とアクション名を取得
            var controllerName = routeData.Values["controller"]?.ToString();
            var actionName = routeData.Values["action"]?.ToString();

            Assert.IsNotNull(controllerName, $"URL '{url}' からコントローラー名を取得できませんでした");
            Assert.IsNotNull(actionName, $"URL '{url}' からアクション名を取得できませんでした");

            // コントローラーの型を推測
            var controllerType = FindControllerType(controllerName);
            Assert.IsNotNull(controllerType, $"コントローラー '{controllerName}Controller' が見つかりませんでした");

            // アクションメソッドが実際に存在するか確認
            var actionMethod = controllerType.GetMethod(actionName);
            Assert.IsNotNull(actionMethod, $"{controllerName}Controller.{actionName}アクションメソッドが存在しません");

            // HTTPメソッド属性の確認
            if (httpMethod != null)
            {
                ValidateHttpMethodAttribute(actionMethod, httpMethod);
            }
        }

        /// <summary>
        /// 属性ルーティングのURL設定が正しいことを検証します（URLから自動推測）
        /// </summary>
        /// <param name="url">テスト対象のURL</param>
        /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）。省略時は"GET"</param>
        public static void AssertAttributeRouteExists(string url, string httpMethod = "GET")
        {
            // URLからコントローラー名とアクション名を推測
            var urlParts = url.TrimStart('/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(urlParts.Length >= 2, $"URL '{url}' からコントローラー名とアクション名を取得できませんでした");

            var controllerName = urlParts[0];
            var actionName = urlParts[1];

            // コントローラーの型を推測
            var controllerType = FindControllerType(controllerName);
            Assert.IsNotNull(controllerType, $"コントローラー '{controllerName}Controller' が見つかりませんでした");

            // アクションメソッドが存在することを確認
            var actionMethod = controllerType.GetMethod(actionName);
            Assert.IsNotNull(actionMethod, $"{controllerName}Controller.{actionName}アクションメソッドが存在しません");

            // 属性ルーティングの属性をチェック
            var routePrefix = GetRoutePrefix(controllerType);
            var routeTemplate = GetRouteTemplate(controllerType, actionMethod);
            var actualUrl = BuildAttributeRouteUrl(routePrefix, routeTemplate, actionName);

            // URLが一致することを確認
            Assert.AreEqual(url, actualUrl, 
                $"属性ルーティングのURLが一致しません。期待値: {url}, 実際の値: {actualUrl}");

            // HTTPメソッド属性を確認
            ValidateHttpMethodAttribute(actionMethod, httpMethod);
        }

        /// <summary>
        /// 指定されたURLがルーティングされるが、対応するアクションが存在しないことを検証します
        /// </summary>
        /// <param name="url">テスト対象のURL</param>
        /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）。省略時は"GET"</param>
        public static void AssertRouteExistsButActionNotExists(string url, string httpMethod = "GET")
        {
            // ルートデータを取得
            var routeData = GetRouteData(url, httpMethod);

            // ルートデータが存在することを確認（ルーティング自体は機能している）
            Assert.IsNotNull(routeData, $"URL '{url}' (HTTP {httpMethod}) に対応するルートが見つかりませんでした");

            // コントローラー名とアクション名を取得
            var controllerName = routeData.Values["controller"]?.ToString();
            var actionName = routeData.Values["action"]?.ToString();

            Assert.IsNotNull(controllerName, $"URL '{url}' からコントローラー名を取得できませんでした");
            Assert.IsNotNull(actionName, $"URL '{url}' からアクション名を取得できませんでした");

            // コントローラーの型を推測
            var controllerType = FindControllerType(controllerName);
            Assert.IsNotNull(controllerType, $"コントローラー '{controllerName}Controller' が見つかりませんでした");

            // アクションメソッドが存在しないことを確認
            var actionMethod = controllerType.GetMethod(actionName);
            Assert.IsNull(actionMethod, $"存在しないアクション '{actionName}' が存在してしまっています");
        }

        /// <summary>
        /// ルートコレクションをリセットします
        /// </summary>
        public static void ResetRoutes()
        {
            routes = null;
            controllerNamespaces = null; // 名前空間のキャッシュもリセット
        }

        /// <summary>
        /// コントローラー名からコントローラーの型を検索します
        /// </summary>
        /// <param name="controllerName">コントローラー名（"Home"など）</param>
        /// <returns>コントローラーの型。見つからない場合はnull</returns>
        private static Type FindControllerType(string controllerName)
        {
            // コントローラーアセンブリを取得（初回のみ）
            if (controllerAssembly == null)
            {
                InitializeControllerAssembly();
            }

            // コントローラー名から型名を構築（例: "Home" → "HomeController"）
            var controllerTypeName = $"{controllerName}Controller";
            
            // 動的に取得した名前空間を使用して検索
            var possibleNamespaces = GetControllerNamespaces();

            foreach (var ns in possibleNamespaces)
            {
                var fullTypeName = string.IsNullOrEmpty(ns) ? controllerTypeName : $"{ns}.{controllerTypeName}";
                var controllerType = controllerAssembly.GetType(fullTypeName);
                
                if (controllerType != null && typeof(Controller).IsAssignableFrom(controllerType))
                {
                    return controllerType;
                }
            }

            // 名前空間が見つからない場合、アセンブリ全体を検索
            var allControllerTypes = controllerAssembly.GetTypes()
                .Where(t => typeof(Controller).IsAssignableFrom(t) && 
                           !t.IsAbstract && 
                           t.Name.Equals(controllerTypeName, StringComparison.OrdinalIgnoreCase));

            return allControllerTypes.FirstOrDefault();
        }

        /// <summary>
        /// コントローラーアセンブリを初期化します
        /// </summary>
        private static void InitializeControllerAssembly()
        {
            // RouteConfigからアセンブリを取得
            if (controllerAssembly == null)
            {
                controllerAssembly = typeof(RouteConfig).Assembly;
            }
        }

        /// <summary>
        /// コントローラーの名前空間を動的に取得します
        /// </summary>
        /// <returns>コントローラーの名前空間の配列</returns>
        private static string[] GetControllerNamespaces()
        {
            // キャッシュされている場合はそれを返す
            if (controllerNamespaces != null)
            {
                return controllerNamespaces;
            }

            if (controllerAssembly == null)
            {
                InitializeControllerAssembly();
            }

            if (controllerAssembly == null)
            {
                throw new InvalidOperationException("コントローラーアセンブリが見つかりません。");
            }

            // アセンブリ内のすべてのコントローラーの名前空間を収集
            var namespaces = controllerAssembly.GetTypes()
                .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => t.Namespace)
                .Where(ns => !string.IsNullOrEmpty(ns))
                .Distinct()
                .OrderByDescending(ns => ns.Split('.').Length) // より具体的な名前空間を優先
                .ToArray();

            // 名前空間が見つからない場合、RouteConfigの名前空間を使用
            if (namespaces.Length == 0)
            {
                var routeConfigNamespace = typeof(RouteConfig).Namespace;
                namespaces = new[] { routeConfigNamespace, $"{routeConfigNamespace}.Controllers" };
            }
            else
            {
                // ルート名前空間を推測（最初の名前空間のルート部分を取得）
                var firstNamespace = namespaces[0];
                var rootNamespace = firstNamespace.Contains('.') 
                    ? firstNamespace.Substring(0, firstNamespace.IndexOf('.'))
                    : firstNamespace;
                
                var namespacesList = namespaces.ToList();
                
                // ルート名前空間がまだ含まれていない場合は追加
                if (!namespacesList.Contains(rootNamespace))
                {
                    namespacesList.Add(rootNamespace);
                }
                
                namespaces = namespacesList.ToArray();
            }

            controllerNamespaces = namespaces;
            return controllerNamespaces;
        }

        /// <summary>
        /// HTTPメソッド属性を検証します
        /// </summary>
        /// <param name="actionMethod">検証対象のアクションメソッド</param>
        /// <param name="httpMethod">期待されるHTTPメソッド（GET、POSTなど）</param>
        private static void ValidateHttpMethodAttribute(MethodInfo actionMethod, string httpMethod)
        {
            var hasHttpGet = actionMethod.IsDefined(typeof(HttpGetAttribute), false);
            var hasHttpPost = actionMethod.IsDefined(typeof(HttpPostAttribute), false);
            var hasHttpPut = actionMethod.IsDefined(typeof(HttpPutAttribute), false);
            var hasHttpDelete = actionMethod.IsDefined(typeof(HttpDeleteAttribute), false);
            var hasAcceptVerbs = actionMethod.IsDefined(typeof(AcceptVerbsAttribute), false);

            bool isValid;
            switch (httpMethod.ToUpper())
            {
                case "GET":
                    isValid = !hasHttpPost && !hasHttpPut && !hasHttpDelete && (!hasAcceptVerbs || hasHttpGet);
                    break;
                case "POST":
                    isValid = hasHttpPost || (hasAcceptVerbs && !hasHttpGet);
                    break;
                case "PUT":
                    isValid = hasHttpPut || (hasAcceptVerbs && !hasHttpGet);
                    break;
                case "DELETE":
                    isValid = hasHttpDelete || (hasAcceptVerbs && !hasHttpGet);
                    break;
                default:
                    // その他のHTTPメソッドはAcceptVerbs属性で確認
                    isValid = true;
                    break;
            }

            if (!isValid && (hasHttpGet || hasHttpPost || hasHttpPut || hasHttpDelete || hasAcceptVerbs))
            {
                Assert.Fail($"{actionMethod.Name}アクションはHTTP {httpMethod}メソッドを許可していません");
            }
        }

        /// <summary>
        /// HTTPメソッドを指定してHttpContextを作成します
        /// </summary>
        /// <param name="url">テスト対象のURL</param>
        /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）</param>
        /// <returns>テスト用のHttpContextBaseインスタンス</returns>
        private static HttpContextBase CreateHttpContext(string url, string httpMethod)
        {
            // HttpRequestを作成
            var httpRequest = CreateHttpRequest(url, httpMethod);
            var response = new HttpResponse(new System.IO.StringWriter());
            var httpContext = new HttpContext(httpRequest, response);
            
            // カスタムHttpRequestWrapperを使用
            var requestWrapper = new TestHttpRequestWrapper(httpRequest, url, httpMethod);
            var contextWrapper = new TestHttpContextWrapper(httpContext, requestWrapper);
            
            return contextWrapper;
        }

        /// <summary>
        /// テスト用のHttpRequestを作成します
        /// </summary>
        /// <param name="url">テスト対象のURL</param>
        /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）</param>
        /// <returns>テスト用のHttpRequestインスタンス</returns>
        private static HttpRequest CreateHttpRequest(string url, string httpMethod)
        {
            var request = new HttpRequest("", "http://localhost" + url, "");
            
            // ApplicationPathを設定
            SetFieldValue(request, "_applicationPath", "/");
            
            // HTTPメソッドを設定
            SetFieldValue(request, "_httpMethod", httpMethod);
            
            // AppRelativeCurrentExecutionFilePathを設定
            SetFieldValue(request, "_appRelativeCurrentExecutionFilePath", "~" + url);
            
            return request;
        }

        /// <summary>
        /// リフレクションを使用してフィールドの値を設定します
        /// </summary>
        /// <param name="obj">対象のオブジェクト</param>
        /// <param name="fieldName">設定するフィールド名</param>
        /// <param name="value">設定する値</param>
        private static void SetFieldValue(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }

        /// <summary>
        /// テスト用のHttpRequestWrapper
        /// </summary>
        private class TestHttpRequestWrapper : HttpRequestWrapper
        {
            private readonly string url;

            /// <summary>
            /// TestHttpRequestWrapperのインスタンスを初期化します
            /// </summary>
            /// <param name="httpRequest">ラップするHttpRequest</param>
            /// <param name="url">テスト対象のURL</param>
            /// <param name="httpMethod">HTTPメソッド（GET、POSTなど）</param>
            public TestHttpRequestWrapper(HttpRequest httpRequest, string url, string httpMethod) 
                : base(httpRequest)
            {
                this.url = url;
                HttpMethod = httpMethod;
            }

            public override string ApplicationPath => "/";

            public override string AppRelativeCurrentExecutionFilePath => "~" + url;

            public override string HttpMethod { get; }
        }

        /// <summary>
        /// テスト用のHttpContextWrapper
        /// </summary>
        private class TestHttpContextWrapper : HttpContextWrapper
        {
            /// <summary>
            /// TestHttpContextWrapperのインスタンスを初期化します
            /// </summary>
            /// <param name="httpContext">ラップするHttpContext</param>
            /// <param name="request">テスト用のHttpRequestBase</param>
            public TestHttpContextWrapper(HttpContext httpContext, HttpRequestBase request) 
                : base(httpContext)
            {
                Request = request;
            }

            public override HttpRequestBase Request { get; }
        }

        /// <summary>
        /// コントローラーのRoutePrefix属性を取得します
        /// </summary>
        /// <param name="controllerType">コントローラーの型</param>
        /// <returns>RoutePrefix属性の値。属性が存在しない場合は空文字列</returns>
        private static string GetRoutePrefix(Type controllerType)
        {
            var routePrefixAttribute = controllerType.GetCustomAttribute(typeof(RoutePrefixAttribute)) as RoutePrefixAttribute;
            return routePrefixAttribute?.Prefix ?? string.Empty;
        }

        /// <summary>
        /// ルートテンプレートを取得します（コントローラーレベルまたはアクションレベル）
        /// </summary>
        /// <param name="controllerType">コントローラーの型</param>
        /// <param name="actionMethod">アクションメソッド</param>
        /// <returns>ルートテンプレート。見つからない場合は"{action}"</returns>
        private static string GetRouteTemplate(Type controllerType, MethodInfo actionMethod)
        {
            // アクションレベルのRoute属性を優先
            if (actionMethod.GetCustomAttribute(typeof(RouteAttribute)) is RouteAttribute routeAttribute)
            {
                return routeAttribute.Template;
            }

            // コントローラーレベルのRoute属性を確認
            if (controllerType.GetCustomAttribute(typeof(RouteAttribute)) is RouteAttribute controllerRouteAttribute)
            {
                return controllerRouteAttribute.Template;
            }

            return "{action}"; // デフォルト
        }

        /// <summary>
        /// 属性ルーティングのURLを構築します
        /// </summary>
        /// <param name="routePrefix">ルートプレフィックス</param>
        /// <param name="routeTemplate">ルートテンプレート</param>
        /// <param name="actionName">アクション名</param>
        /// <returns>構築されたURL</returns>
        private static string BuildAttributeRouteUrl(string routePrefix, string routeTemplate, string actionName)
        {
            var url = string.Empty;

            // RoutePrefixを追加
            if (!string.IsNullOrEmpty(routePrefix))
            {
                url = "/" + routePrefix.TrimStart('/');
            }

            // Routeテンプレートを処理
            if (!string.IsNullOrEmpty(routeTemplate))
            {
                // {action}を実際のアクション名に置換
                var processedTemplate = routeTemplate.Replace("{action}", actionName);
                
                if (processedTemplate.StartsWith("/"))
                {
                    // 絶対パスの場合、RoutePrefixを無視
                    url = processedTemplate;
                }
                else
                {
                    // 相対パスの場合、RoutePrefixに追加
                    if (!string.IsNullOrEmpty(url))
                    {
                        url += "/" + processedTemplate.TrimStart('/');
                    }
                    else
                    {
                        url = "/" + processedTemplate.TrimStart('/');
                    }
                }
            }
            else
            {
                // テンプレートがない場合、アクション名を追加
                if (!string.IsNullOrEmpty(url))
                {
                    url += "/" + actionName;
                }
                else
                {
                    url = "/" + actionName;
                }
            }

            // 重複するスラッシュを削除
            url = url.Replace("//", "/");
            
            return url;
        }
    }
}

