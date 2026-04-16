# 装備インスタンス API

## エンドポイント一覧

| メソッド | パス | 概要 |
|---|---|---|
| POST | `/api/equipment/instances` | 装備インスタンス作成 |
| GET | `/api/equipment/instances/{instanceId}` | 装備インスタンス取得 |

## 認証

すべてのリクエストに `X-Api-Key` ヘッダーが必要です。

| ヘッダー | 型 | 必須 | 説明 |
|---|---|---|---|
| `X-Api-Key` | string | ✓ | API キー |

---

## POST /api/equipment/instances

### 概要

YAML マスタデータをもとに装備の個別動的データを生成して `dbo.equipment_instance` テーブルに保存します。  
ルーンスロット数のランダム確定・ステータス下限値/上限値候補の保存・エンチャントプール構成の保存をサーバー側で行います。

### リクエスト

```http
POST /api/equipment/instances
Content-Type: application/json
X-Api-Key: <your-api-key>
```

```json
{
	"equipmentId": "iron_sword",
	"accountId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
	"source": "loot_drop",
	"createdBy": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

| フィールド | 型 | 必須 | 説明 |
|---|---|---|---|
| `equipmentId` | string | ✓ | YAML マスタの装備 ID（`category: equipment` のアイテム） |
| `accountId` | GUID | ✓ | 所有アカウントの UUID |
| `source` | string | | 生成元の識別子（例: `loot_drop`, `quest_reward`） |
| `createdBy` | GUID | ✓ | 作成者の UUID（通常はアカウント UUID と同一） |

### レスポンス

#### 201 Created

装備インスタンス作成成功。作成された装備インスタンスのデータと `Location` ヘッダーを返します。

```json
{
	"equipmentInstanceId": "f1e2d3c4-b5a6-7890-fedc-ba0987654321",
	"accountId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
	"itemId": "iron_sword",
	"enhanceLevel": 0,
	"runeMaxSlots": 2,
	"transcendenceRank": 0,
	"durabilityMax": 100,
	"durabilityValue": 100,
	"createdAt": "2026-04-11T12:00:00Z",
	"updatedAt": "2026-04-11T12:00:00Z",
	"statRolls": [
		{
			"statRollId": "a0b1c2d3-e4f5-6789-abcd-ef0123456789",
			"status": "ATTACK",
			"min": "10~20",
			"max": "21~50",
			"sortOrder": 0
		}
	],
	"enchantPools": [
		{
			"poolIndex": 0,
			"recipeId": "enchant_fire_sword_1",
			"requiredMaterialItemId": "fire_essence",
			"requiredMaterialAmount": 3,
			"requiredCurrency": 1000
		}
	]
}
```

| フィールド | 型 | 説明 |
|---|---|---|
| `equipmentInstanceId` | GUID | 生成された装備個体 UUID |
| `accountId` | GUID | 所有アカウント UUID |
| `itemId` | string | YAML マスタの装備 ID |
| `enhanceLevel` | int | 強化レベル（初期値 `0`） |
| `runeMaxSlots` | int | 生成時に確定したルーン最大スロット数 |
| `transcendenceRank` | int | 段階変化ランク（初期値 `0`） |
| `durabilityMax` | int \| null | 耐久上限（YAML に `durability.max` がある場合のみ） |
| `durabilityValue` | int \| null | 現在耐久値（耐久管理対象のときのみ） |
| `createdAt` | datetime | レコード作成日時（UTC） |
| `updatedAt` | datetime | レコード最終更新日時（UTC） |
| `statRolls` | array | ステータス下限値/上限値候補一覧（`value.min / value.max` に範囲候補を持つ stat のみ） |
| `enchantPools` | array | エンチャントプール一覧（YAML の `enchant.pools[]` に対応） |

#### statRolls 要素

| フィールド | 型 | 説明 |
|---|---|---|
| `statRollId` | GUID | ステータス候補レコード UUID |
| `status` | string | 対象ステータス（例: `ATTACK`） |
| `min` | string | YAML 由来の下限値候補 |
| `max` | string | YAML 由来の上限値候補 |
| `sortOrder` | int | YAML 内の定義順（0 始まり） |

#### enchantPools 要素

| フィールド | 型 | 説明 |
|---|---|---|
| `poolIndex` | int | YAML 内プール順序（0 始まり） |
| `recipeId` | string \| null | レシピ参照 ID |
| `requiredMaterialItemId` | string \| null | 必要素材の itemId |
| `requiredMaterialAmount` | int | 必要素材数（デフォルト `1`） |
| `requiredCurrency` | int | 必要通貨（デフォルト `0`） |

#### 404 Not Found

指定した `equipmentId` が存在しない、または `category: equipment` ではない。

#### 401 Unauthorized

API キーが指定されていない、または無効。

---

## GET /api/equipment/instances/{instanceId}

### 概要

`dbo.equipment_instance` テーブルから装備個体 UUID をキーにインスタンス情報を取得します。  
論理削除済みのインスタンスは取得できません。

### リクエスト

#### パスパラメータ

| パラメータ | 型 | 必須 | 説明 |
|---|---|---|---|
| `instanceId` | GUID | ✓ | 装備インスタンスの UUID |

```http
GET /api/equipment/instances/f1e2d3c4-b5a6-7890-fedc-ba0987654321
X-Api-Key: <your-api-key>
```

### レスポンス

#### 200 OK

装備インスタンス取得成功。レスポンス形式は `POST /api/equipment/instances` の 201 レスポンスと同一。

#### 404 Not Found

指定 UUID の装備インスタンスが存在しない、または論理削除済み。

#### 401 Unauthorized

API キーが指定されていない、または無効。
