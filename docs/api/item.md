# アイテム取得 API

## 概要

ファイルシステム上のアイテム YAML を起動時にロードし、カテゴリとアイテム ID をキーに取得します。

## エンドポイント

| 項目 | 値 |
|---|---|
| メソッド | GET |
| パス | `/api/item/{category}/{itemId}` |

## リクエスト

### パスパラメータ

| パラメータ | 型 | 必須 | 説明 |
|---|---|---|---|
| `category` | string | ✓ | アイテムカテゴリ（下記対応リスト参照） |
| `itemId` | string | ✓ | アイテム ID |

### 対応カテゴリ

| カテゴリ | 説明 |
|---|---|
| `material` | 素材アイテム |
| `consumable` | 消耗品アイテム |

## レスポンス

### 200 OK — material

```http
GET /api/item/material/iron_ingot
```

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

### 200 OK — consumable

```http
GET /api/item/consumable/haste_potion_small
```

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

### 共通フィールド

| フィールド | 型 | 説明 |
|---|---|---|
| `schemaVersion` | int | YAML スキーマバージョン |
| `id` | string | アイテム ID |
| `category` | string | アイテムカテゴリ |
| `name` | string | アイテム名 |
| `icon` | string | Minecraft マテリアル名 |
| `rarity` | string | レアリティ |
| `saleValue` | int | 売却価格 |
| `customModelData` | int \| null | カスタムモデルデータ |
| `lore` | string[] | 説明文 |
| `unTradeable` | bool | 取引不可フラグ |
| `unSellable` | bool | 売却不可フラグ |

### 400 Bad Request

未対応カテゴリを指定。

### 404 Not Found

指定カテゴリ内に対象アイテムが存在しない。
