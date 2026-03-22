# バフ取得 API

## 概要

ファイルシステム上の buff YAML を起動時にロードし、buff ID をキーに取得します。

## エンドポイント

| 項目 | 値 |
|---|---|
| メソッド | GET |
| パス | `/api/buff/{buffId}` |

## リクエスト

### パスパラメータ

| パラメータ | 型 | 必須 | 説明 |
|---|---|---|---|
| `buffId` | string | ✓ | バフ ID |

```http
GET /api/buff/haste_small
```

## レスポンス

### 200 OK

バフ取得成功。

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

| フィールド | 型 | 説明 |
|---|---|---|
| `schemaVersion` | int | YAML スキーマバージョン |
| `id` | string | バフ ID |
| `type` | string | バフ種別（`BUFF` / `DEBUFF`） |
| `name` | string | バフ名（Minecraft カラーコード含む） |
| `icon` | string \| null | アイコン（Minecraft マテリアル名） |
| `lore` | string[] | 説明文 |
| `durationTicks` | int | 効果時間（tick） |
| `isDebuff` | bool | デバフフラグ |
| `modifiers` | object[] | ステータス変動リスト |
| `modifiers[].status` | string | 対象ステータス |
| `modifiers[].type` | string | 変動タイプ（`SCALAR` / `FLAT`） |
| `modifiers[].value` | number | 変動量 |

### 404 Not Found

指定 buff ID が存在しない。
