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

## API

### 疎通確認 API

- メソッド: GET
- パス: /api/health
- 概要: 外部システムから API サーバーへ接続できるかを確認するための疎通確認用エンドポイントです

リクエスト例:

```http
GET /api/health
```

レスポンス例:

```json
{
	"status": "ok",
	"service": "AstralRecordApi",
	"timestamp": "2026-03-20T10:00:00+00:00"
}
```

ステータスコード:

- 200 OK: API サーバーへの接続成功

### ユーザー取得 API

- メソッド: GET
- パス: /api/user/{uuid}
- 概要: dbo.user テーブルから UUID をキーにユーザー情報を取得します

リクエスト例:

```http
GET /api/user/8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46
```

レスポンス例:

```json
{
	"uuid": "8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46",
	"mcid": "PlayerName",
	"joinDate": "2026-03-09T12:34:56",
	"lastJoinDate": "2026-03-20T18:45:12",
	"globalIp": "127.0.0.1",
	"accountId": null,
	"banIndefinite": false,
	"banDate": null,
	"kickIp": true,
	"permission": 0,
	"createdAt": "2026-03-09T12:34:56",
	"updatedAt": "2026-03-20T18:45:12",
	"createdBy": "8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46",
	"updatedBy": "8d8d3d63-6c25-4bde-bf75-6ee3d3f26c46",
	"isDeleted": false
}
```

ステータスコード:

- 200 OK: ユーザー取得成功
- 404 Not Found: 指定 UUID のユーザーが存在しない、または論理削除済み

### アイテム取得 API

- メソッド: GET
- パス: /api/item/{category}/{itemId}
- 概要: ファイルシステム上のアイテム YAML を起動時にロードし、カテゴリとアイテム ID をキーに取得します
- 対応カテゴリ: material, consumable

リクエスト例:

```http
GET /api/item/material/iron_ingot
```

レスポンス例:

```json
{
	"schemaVersion": 1,
	"id": "iron_ingot",
	"category": "material",
	"name": "鉄インゴット",
	"icon": "IRON_INGOT",
	"rarity": "COMMON",
	"saleValue": 10,
	"customModelData": null,
	"lore": [
		"基本的な素材として使用される鉄のインゴット。",
		"武器や防具の作成に欠かせない。"
	],
	"unTradeable": false,
	"unSellable": false
}
```

ステータスコード:

- 200 OK: アイテム取得成功
- 400 Bad Request: 未対応カテゴリを指定
- 404 Not Found: 指定カテゴリ内に対象アイテムが存在しない

### 消耗品アイテム取得例

リクエスト例:

```http
GET /api/item/consumable/haste_potion_small
```

レスポンス例:

```json
{
	"schemaVersion": 1,
	"id": "haste_potion_small",
	"category": "consumable",
	"name": "迅速ポーション(小)",
	"icon": "POTION",
	"rarity": "COMMON",
	"saleValue": 20,
	"customModelData": null,
	"lore": [
		"飲むと一定時間、移動速度が上昇する。",
		"素早さが10%上昇する。"
	],
	"unTradeable": false,
	"unSellable": false,
	"consumable": {
		"onUse": {
			"sound": "entity.generic.drink",
			"effect": "happy_villager",
			"amount": 1
		},
		"effects": [
			{
				"type": "BUFF",
				"rate": 100,
				"value": null,
				"status": null,
				"isPercent": false,
				"buffId": "haste_small"
			}
		]
	}
}
```

### バフ取得 API

- メソッド: GET
- パス: /api/buff/{buffId}
- 概要: ファイルシステム上の buff YAML を起動時にロードし、buff ID をキーに取得します

リクエスト例:

```http
GET /api/buff/haste_small
```

レスポンス例:

```json
{
	"schemaVersion": 1,
	"id": "haste_small",
	"type": "BUFF",
	"name": "&e迅速(小)",
	"icon": null,
	"lore": [
		"&7一定時間、移動速度が上昇する。"
	],
	"durationTicks": 600,
	"isDebuff": false,
	"modifiers": [
		{
			"status": "MOVEMENT_SPEED",
			"type": "SCALAR",
			"value": 0.1
		}
	]
}
```

ステータスコード:

- 200 OK: バフ取得成功
- 404 Not Found: 指定 buff ID が存在しない

## 開発ルール

- API を追加または変更した場合は README.md の API 仕様も必ず更新すること
