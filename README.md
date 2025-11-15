# aspnet-mvc-regacytest-sample
ASP.NET MVC にて URLの妥当性をユニットテストで確認するサンプル

## Feature
- ASP.NET MVC
- MsTest

## Note
- 通常のルーティング（従来のルーティング）のテストには `RoutingTestHelper.AssertRouteAndActionExists()` を使用します
- 属性ルーティングのテストには `RoutingTestHelper.AssertAttributeRouteExists()` を使用します
  - 属性ルーティングはユニットテスト環境では登録できないため、属性を直接チェックする必要があります
- テストクラスでは `[TestInitialize]` で `RoutingTestHelper.InitializeRoutes()` を呼び出してルートを初期化してください
- HTTPメソッド（GET、POSTなど）の検証も自動的に行われます