# Astral Record API

Minecraft Purpur サーバー向け MMO RPG プラグイン Astral Record と関連 Web サイトのための Web API です。

## 構成概要

- 動的データ: SQL Server から取得
- 静的データ: ファイルシステム上のデータ定義を参照
- ランタイム: .NET 10
- フレームワーク: ASP.NET Core Web API

## 設定

設定は AstralRecordApi/appsettings.json と AstralRecordApi/appsettings.Development.json で管理します。

主な設定項目:

- ConnectionStrings:SqlServer
	- SQL Server 接続文字列
- FileDatabase:RootPath
	- 静的データファイルのルートパス

## API ドキュメント

各 API の詳細仕様は `docs/api/` 配下のファイルを参照してください。

| エンドポイント | 概要 | ドキュメント |
|---|---|---|
| GET /api/health | 疎通確認 | [docs/api/health.md](docs/api/health.md) |
| GET /api/user/{uuid} | ユーザー情報取得 | [docs/api/user.md](docs/api/user.md) |
| POST /api/user | ユーザー登録 | [docs/api/user.md](docs/api/user.md) |
| PUT /api/user/{uuid} | ユーザー情報更新 | [docs/api/user.md](docs/api/user.md) |
| GET /api/account?user_id={user_id} | ユーザーのアカウント一覧取得 | [docs/api/account.md](docs/api/account.md) |
| GET /api/account/{uuid} | アカウント情報取得 | [docs/api/account.md](docs/api/account.md) |
| POST /api/account | アカウント登録 | [docs/api/account.md](docs/api/account.md) |
| PUT /api/account/{uuid} | アカウント情報更新 | [docs/api/account.md](docs/api/account.md) |
| GET /api/item/{category}/{itemId} | アイテム取得 | [docs/api/item.md](docs/api/item.md) |
| GET /api/buff | バフ一覧取得（主要項目のみ） | [docs/api/buff.md](docs/api/buff.md) |
| GET /api/buff/{buffId} | バフ取得 | [docs/api/buff.md](docs/api/buff.md) |

### Scalar API UI

サーバー起動後、以下の URL でインタラクティブな API ドキュメントを参照できます。

```
http://localhost:{port}/scalar
```

OpenAPI スペック (JSON):

```
http://localhost:{port}/openapi/v1.json
```

## 開発ルール

- API を追加または変更した場合は以下をすべて更新すること:
	1. 対応する `docs/api/*.md` の仕様
	2. コントローラーの XML doc コメント (`/// <summary>` 等)
	3. `README.md` の API ドキュメント一覧テーブル（エンドポイントを追加・削除した場合）

