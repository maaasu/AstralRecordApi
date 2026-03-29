# アイテム取得 API

## 概要

ファイルシステム上のアイテム YAML を起動時にロードし、カテゴリとアイテム ID をキーに取得します。

## エンドポイント

| 項目 | 値 |
|---|---|
| メソッド | GET |
| パス | `/api/item` |

| 項目 | 値 |
|---|---|
| メソッド | GET |
| パス | `/api/item/{category}/{itemId}` |

## 認証

すべてのリクエストに `X-Api-Key` ヘッダーが必要です。

| ヘッダー | 型 | 必須 | 説明 |
|---|---|---|---|
| `X-Api-Key` | string | ✓ | API キー |

## リクエスト

### 一覧取得

```http
GET /api/item
X-Api-Key: <your-api-key>
```

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
| `equipment` | 装備品アイテム |
| `currency` | 通貨・証 |
| `bundle` | パッケージ・ボックス |

## レスポンス

### 200 OK（一覧取得）

最小限の識別情報として `id` と `category` のみ返却します。

```json
[
	{
		"id": "iron_ingot",
		"category": "material"
	}
]
```

| フィールド | 型 | 説明 |
|---|---|---|
| `id` | string | アイテム ID |
| `category` | string | アイテムカテゴリ |

### 200 OK — material

```http
GET /api/item/material/iron_ingot
X-Api-Key: <your-api-key>
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
X-Api-Key: <your-api-key>
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

### 200 OK — equipment

```http
GET /api/item/equipment/bronze_sword
X-Api-Key: <your-api-key>
```

```json
{
	"schemaVersion": 1,
	"id": "bronze_sword",
	"category": "equipment",
	"name": "ブロンズソード",
	"icon": "IRON_SWORD",
	"rarity": "COMMON",
	"saleValue": 50,
	"customModelData": null,
	"lore": [
		"初心者向けの片手剣。"
	],
	"unTradeable": false,
	"unSellable": false,
	"equipment": {
		"slot": "WEAPON",
		"handType": "ONE",
		"requiredLevel": 3,
		"requiredClasses": [],
		"stats": [
			{
				"status": "ATTACK_POWER",
				"type": "FLAT",
				"value": "12"
			},
			{
				"status": "CRITICAL_CHANCE",
				"type": "FLAT",
				"value": "0.03"
			}
		],
		"durability": {
			"max": 120,
			"consume": 1
		},
		"onUse": {
			"leftClickCooldownTicks": 80,
			"leftClickSkillId": "lefthand_slash",
			"rightClickCooldownTicks": null,
			"rightClickSkillId": null
		},
		"skills": []
	}
}
```

### 200 OK — currency

```http
GET /api/item/currency/boss_proof_golem
X-Api-Key: <your-api-key>
```

```json
{
	"schemaVersion": 1,
	"id": "boss_proof_golem",
	"category": "currency",
	"name": "ゴーレム討伐の証",
	"icon": "AMETHYST_SHARD",
	"rarity": "EPIC",
	"saleValue": 0,
	"customModelData": null,
	"maxStack": 64,
	"lore": [
		"巨像を打ち倒した者に与えられる証。"
	],
	"unTradeable": false,
	"unSellable": true,
	"currency": {
		"type": "BOSS_PROOF",
		"group": "golem",
		"expiresAt": "2026-02-01T00:00:00+09:00"
	}
}
```

### 200 OK — bundle

```http
GET /api/item/bundle/magic_iron_ingot_bundle
X-Api-Key: <your-api-key>
```

```json
{
	"schemaVersion": 1,
	"id": "magic_iron_ingot_bundle",
	"category": "bundle",
	"name": "魔法のパケット",
	"icon": "chest",
	"rarity": "UNCOMMON",
	"saleValue": 0,
	"customModelData": null,
	"maxStack": 64,
	"lore": [
		"魔力を帯びた珍しいパケット。"
	],
	"unTradeable": false,
	"unSellable": false,
	"bundle": {
		"lootTableId": "magic_iron_ingot",
		"onUse": {
			"sound": "block.anvil.land",
			"particle": "block_break"
		}
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
| `maxStack` | int | 最大スタック数 |
| `lore` | string[] | 説明文 |
| `unTradeable` | bool | 取引不可フラグ |
| `unSellable` | bool | 売却不可フラグ |

### 400 Bad Request

未対応カテゴリを指定。

### 401 Unauthorized

API キーが指定されていない、または無効。

### 404 Not Found

指定カテゴリ内に対象アイテムが存在しない。
